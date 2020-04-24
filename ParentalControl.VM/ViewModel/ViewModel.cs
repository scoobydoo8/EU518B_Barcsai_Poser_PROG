// <copyright file="ViewModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParentalControl.VM
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ParentalControl.BL;
    using ParentalControl.Interface.BusinessLogic;
    using ParentalControl.Interface.Database;
    using ParentalControl.Interface.ViewModel;

    /// <summary>
    /// View model class.
    /// </summary>
    public class ViewModel : INotifyPropertyChanged, IViewModel, IAdminViewModel, ITimeRemainingViewModel
    {
        private static ViewModel viewModel;
        private BusinessLogic businessLogic;

        private ViewModel()
        {
            this.businessLogic = BusinessLogic.Get();
            this.businessLogic.Database.DatabaseChanged += this.Database_DatabaseChanged;
            this.businessLogic.UserLoggedInFull += this.BusinessLogic_UserLoggedInFull;
            this.businessLogic.UserLoggedInOccassional += this.BusinessLogic_UserLoggedIn;
            this.businessLogic.UserLoggedInOrderlyOrActiveOccasional += this.BusinessLogic_UserLoggedIn;
            this.businessLogic.UserLoggedOut += this.BusinessLogic_UserLoggedOut;
            this.businessLogic.TimerTick += this.BusinessLogic_TimerTick;
        }

        /// <summary>
        /// Property changed event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        public IBusinessLogic BL { get => this.businessLogic; }

        /// <inheritdoc/>
        public TimeSpan TimeRemainingTime { get => this.businessLogic.TimeRemainingTime; }

        /// <inheritdoc/>
        public TimeSpan ProgramRemainingTime { get => this.businessLogic.ProgramRemainingTime; }

        /// <inheritdoc/>
        public IUser ActiveUser { get; private set; }

        /// <inheritdoc/>
        public ObservableCollection<IUser> ManagedUsers { get; private set; }

        /// <inheritdoc/>
        public IUser SelectedManagedUser { get; set; }

        /// <inheritdoc/>
        public ObservableCollection<IProgramLimitation> ManagedProgramLimitations { get; private set; }

        /// <inheritdoc/>
        public IProgramLimitation SelectedManagedProgramLimitation { get; set; }

        /// <inheritdoc/>
        public ObservableCollection<IWebLimitation> ManagedWebLimitations { get; private set; }

        /// <inheritdoc/>
        public IWebLimitation SelectedManagedWebLimitation { get; set; }

        /// <inheritdoc/>
        public ObservableCollection<IKeyword> ManagedKeywords { get; private set; }

        /// <inheritdoc/>
        public IKeyword SelectedManagedKeyword { get; set; }

        /// <summary>
        /// Singleton.
        /// </summary>
        /// <returns>View model.</returns>
        public static ViewModel Get()
        {
            if (viewModel == null)
            {
                viewModel = new ViewModel();
            }

            return viewModel;
        }

        /// <inheritdoc/>
        public void User_SelectionChanged()
        {
            if (this.SelectedManagedUser != null)
            {
                this.ManagedProgramLimitations = new ObservableCollection<IProgramLimitation>(this.businessLogic.Database.ReadProgramLimitations(x => x.UserID == this.SelectedManagedUser.ID));
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ManagedProgramLimitations)));
                this.ManagedWebLimitations = new ObservableCollection<IWebLimitation>(this.businessLogic.Database.ReadWebLimitations(x => x.UserID == this.SelectedManagedUser.ID));
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ManagedWebLimitations)));
            }
        }

        private void Database_DatabaseChanged(object sender, EventArgs e)
        {
            if (this.ActiveUser != null && this.ActiveUser.ID == this.businessLogic.Database.AdminID)
            {
                this.ManagedUsers = new ObservableCollection<IUser>(this.businessLogic.Database.ReadUsers(x => x.ID != this.businessLogic.Database.AdminID));
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ManagedUsers)));
                this.ManagedKeywords = new ObservableCollection<IKeyword>(this.businessLogic.Database.ReadKeywords());
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ManagedKeywords)));
                if (this.SelectedManagedUser != null)
                {
                    this.ManagedProgramLimitations = new ObservableCollection<IProgramLimitation>(this.businessLogic.Database.ReadProgramLimitations(x => x.UserID == this.SelectedManagedUser.ID));
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ManagedProgramLimitations)));
                    this.ManagedWebLimitations = new ObservableCollection<IWebLimitation>(this.businessLogic.Database.ReadWebLimitations(x => x.UserID == this.SelectedManagedUser.ID));
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ManagedWebLimitations)));
                }
            }
        }

        private void BusinessLogic_UserLoggedInFull(object sender, EventArgs e)
        {
            this.ActiveUser = this.businessLogic.ActiveUser;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ActiveUser)));
            if (this.ActiveUser.ID == this.businessLogic.Database.AdminID)
            {
                this.ManagedUsers = new ObservableCollection<IUser>(this.businessLogic.Database.ReadUsers(x => x.ID != this.businessLogic.Database.AdminID));
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ManagedUsers)));
                this.ManagedKeywords = new ObservableCollection<IKeyword>(this.businessLogic.Database.ReadKeywords());
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ManagedKeywords)));
            }
        }

        private void BusinessLogic_UserLoggedIn(object sender, EventArgs e)
        {
            this.ActiveUser = this.businessLogic.ActiveUser;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ActiveUser)));
        }

        private void BusinessLogic_UserLoggedOut(object sender, EventArgs e)
        {
            this.ActiveUser = null;
            this.ManagedUsers = null;
            this.ManagedProgramLimitations = null;
            this.ManagedWebLimitations = null;
            this.ManagedKeywords = null;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ActiveUser)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ManagedUsers)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ManagedProgramLimitations)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ManagedWebLimitations)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ManagedKeywords)));
        }

        private void BusinessLogic_TimerTick(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.TimeRemainingTime)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ProgramRemainingTime)));
        }
    }
}
