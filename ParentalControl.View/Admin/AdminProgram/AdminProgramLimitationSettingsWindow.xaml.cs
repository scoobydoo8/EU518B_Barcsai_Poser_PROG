// <copyright file="AdminProgramLimitationSettingsWindow.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParentalControl.View.Admin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using ParentalControl.Interface.ViewModel;
    using ParentalControl.VM;

    /// <summary>
    /// Interaction logic for AdminProgramLimitationSettingsWindow.xaml.
    /// </summary>
    public partial class AdminProgramLimitationSettingsWindow : Window
    {
        private IAdminViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminProgramLimitationSettingsWindow"/> class.
        /// </summary>
        public AdminProgramLimitationSettingsWindow()
        {
            this.InitializeComponent();
            this.viewModel = ViewModel.Get();
            this.DataContext = this.viewModel.SelectedManagedUser;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.bindingGroup.BeginEdit();
            this.txtFromTime.IsEnabled = this.chbOrderlyFree.IsChecked == true;
            this.txtToTime.IsEnabled = this.chbOrderlyFree.IsChecked == true;
        }

        private void OrderlyFree_Checked(object sender, RoutedEventArgs e)
        {
            this.txtFromTime.IsEnabled = true;
            this.txtToTime.IsEnabled = true;
            if (this.txtFromTime.Text == "00:00:00" && this.txtToTime.Text == "00:00:00")
            {
                this.txtToTime.Text = "23:59:00";
                this.viewModel.SelectedManagedUser.ProgramLimitToTime = TimeSpan.Parse("23:59:00");
            }
        }

        private void OrderlyFree_Unchecked(object sender, RoutedEventArgs e)
        {
            this.txtFromTime.IsEnabled = false;
            this.txtToTime.IsEnabled = false;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            this.bindingGroup.CommitEdit();
            if (this.txtFromTime.Text == "00:00:00" && this.txtToTime.Text == "00:00:00")
            {
                this.viewModel.SelectedManagedUser.ProgramLimitToTime = TimeSpan.Parse("23:59:00");
            }

            if (this.viewModel.SelectedManagedUser.ProgramLimitFromTime > this.viewModel.SelectedManagedUser.ProgramLimitToTime)
            {
                var help = this.viewModel.SelectedManagedUser.ProgramLimitFromTime;
                this.viewModel.SelectedManagedUser.ProgramLimitFromTime = this.viewModel.SelectedManagedUser.ProgramLimitToTime;
                this.viewModel.SelectedManagedUser.ProgramLimitToTime = help;
            }

            if (this.viewModel.SelectedManagedUser.ProgramLimitOccasionalMinutes < 1)
            {
                this.viewModel.SelectedManagedUser.ProgramLimitOccasionalMinutes = 30;
            }

            bool fromTime = this.viewModel.SelectedManagedUser.TimeLimitFromTime > this.viewModel.SelectedManagedUser.ProgramLimitFromTime;
            bool toTime = this.viewModel.SelectedManagedUser.TimeLimitToTime - TimeSpan.FromMinutes(5) < this.viewModel.SelectedManagedUser.ProgramLimitToTime;

            if (!this.viewModel.SelectedManagedUser.IsTimeLimitOrderly && this.viewModel.SelectedManagedUser.IsProgramLimitOrderly)
            {
                this.viewModel.SelectedManagedUser.IsProgramLimitOrderly = false;
                MessageBox.Show("A programokat nem lehet rendszeresen engedélyezni, amíg a számítógép nincs rendszeresen engedélyezve!\nAutomatikusan korrigálva!", "Rendszeres engedély", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (this.viewModel.SelectedManagedUser.IsTimeLimitOrderly && this.viewModel.SelectedManagedUser.IsProgramLimitOrderly && (fromTime || toTime))
            {
                if (fromTime)
                {
                    this.viewModel.SelectedManagedUser.ProgramLimitFromTime = this.viewModel.SelectedManagedUser.TimeLimitFromTime;
                    MessageBox.Show("A programokat nem lehet korábban kezdődő rendszeres engedéllyel ellátni, mint a számítógép rendszeres engedély kezdése!\nAutomatikusan korrigálva!", "Rendszeres engedély", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                if (toTime)
                {
                    this.viewModel.SelectedManagedUser.ProgramLimitToTime = this.viewModel.SelectedManagedUser.TimeLimitToTime - TimeSpan.FromMinutes(5);
                    MessageBox.Show("A programokat nem lehet később végződő rendszeres engedéllyel ellátni, mint a számítógép rendszeres engedély vége!\nAutomatikusan korrigálva!", "Rendszeres engedély", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

            if (this.viewModel.SelectedManagedUser.TimeLimitOccasionalMinutes - 5 < this.viewModel.SelectedManagedUser.ProgramLimitOccasionalMinutes)
            {
                this.viewModel.SelectedManagedUser.ProgramLimitOccasionalMinutes = this.viewModel.SelectedManagedUser.TimeLimitOccasionalMinutes - 5;
                MessageBox.Show("A program eseti engedély percét nem lehet nagyobbra állítani, mint a számítógép eseti engedélye!\nAutomatikusan korrigálva!", "Eseti engedély", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            this.viewModel.BL.Database.Transaction(() => this.viewModel.BL.Database.UpdateUsers(
                x =>
                {
                    x.IsProgramLimitOrderly = this.viewModel.SelectedManagedUser.IsProgramLimitOrderly;
                    x.ProgramLimitFromTime = this.viewModel.SelectedManagedUser.ProgramLimitFromTime;
                    x.ProgramLimitToTime = this.viewModel.SelectedManagedUser.ProgramLimitToTime;
                    x.ProgramLimitOccasionalMinutes = this.viewModel.SelectedManagedUser.ProgramLimitOccasionalMinutes;
                },
                x => x.ID == this.viewModel.SelectedManagedUser.ID));
            this.DialogResult = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.bindingGroup.CancelEdit();
        }
    }
}
