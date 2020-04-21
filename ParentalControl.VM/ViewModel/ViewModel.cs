// <copyright file="ViewModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParentalControl.VM
{
    using System;
    using System.Collections.Generic;
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
    public class ViewModel : INotifyPropertyChanged, IViewModel, IAdminViewModel
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

            // DEBUG
            Console.WriteLine(this.businessLogic.Registration("Username", "Password", "?", "!"));
        }

        /// <summary>
        /// Property changed event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        public IBusinessLogic BL { get => this.businessLogic; }

        /// <inheritdoc/>
        public IUser ActiveUser { get; private set; }

        /// <inheritdoc/>
        public List<IUser> ManagedUsers { get; private set; }

        /// <inheritdoc/>
        public List<IProgramLimitation> ManagedProgramLimitations { get; private set; }

        /// <inheritdoc/>
        public List<IWebLimitation> ManagedWebLimitations { get; private set; }

        /// <inheritdoc/>
        public List<IKeyword> ManagedKeywords { get; private set; }

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

        private void Database_DatabaseChanged(object sender, string e)
        {
            if (this.ActiveUser != null && this.ActiveUser.ID == this.businessLogic.Database.AdminID)
            {
                if (e == "Users")
                {
                    this.ManagedUsers = this.businessLogic.Database.ReadUsers(x => x.ID != this.businessLogic.Database.AdminID);
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ManagedUsers)));
                }
                else if (e == "ProgramLimitations")
                {
                    this.ManagedProgramLimitations = this.businessLogic.Database.ReadProgramLimitations();
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ManagedProgramLimitations)));
                }
                else if (e == "WebLimitations")
                {
                    this.ManagedWebLimitations = this.businessLogic.Database.ReadWebLimitations();
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ManagedWebLimitations)));
                }
                else if (e == "Keywords")
                {
                    this.ManagedKeywords = this.businessLogic.Database.ReadKeywords();
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ManagedKeywords)));
                }
            }
        }

        private void BusinessLogic_UserLoggedInFull(object sender, EventArgs e)
        {
            this.ActiveUser = this.businessLogic.ActiveUser;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ActiveUser)));
            if (this.ActiveUser.ID == this.businessLogic.Database.AdminID)
            {
                this.ManagedUsers = this.businessLogic.Database.ReadUsers(x => x.ID != this.businessLogic.Database.AdminID);
                this.ManagedProgramLimitations = this.businessLogic.Database.ReadProgramLimitations();
                this.ManagedWebLimitations = this.businessLogic.Database.ReadWebLimitations();
                this.ManagedKeywords = this.businessLogic.Database.ReadKeywords();
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ManagedUsers)));
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ManagedProgramLimitations)));
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ManagedWebLimitations)));
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
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.businessLogic.TimeRemainingTime)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.businessLogic.ProgramRemainingTime)));
        }
    }
}
