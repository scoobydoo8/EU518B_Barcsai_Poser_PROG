// <copyright file="AdminWindow.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParentalControl.View.Admin
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
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
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using ParentalControl.Interface.ViewModel;
    using ParentalControl.VM;

    /// <summary>
    /// Interaction logic for AdminWindow.xaml.
    /// </summary>
    public partial class AdminWindow : Window
    {
        private IAdminViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminWindow"/> class.
        /// </summary>
        public AdminWindow()
        {
            this.InitializeComponent();
            this.viewModel = ViewModel.Get();
            this.DataContext = this.viewModel;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.cmbUser.Focus();
            this.Left = (SystemParameters.PrimaryScreenWidth - this.ActualWidth) / 2;
            this.Top = SystemParameters.PrimaryScreenHeight * 0.2;
        }

        private void User_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.viewModel.SelectedManagedUser != null)
            {
                this.viewModel.User_SelectionChanged();
                this.tbcLimits.IsEnabled = true;
                this.frmTimeLimit.Navigate(new AdminTimeLimitationPage());
                this.frmProgramLimit.Navigate(new AdminProgramLimitationPage());
                this.frmWebLimit.Navigate(new AdminWebLimitationPage());
            }
        }

        private void Registration_Click(object sender, RoutedEventArgs e)
        {
            AdminRegistrationWindow adminRegistrationWindow = new AdminRegistrationWindow();
            adminRegistrationWindow.Tag = "child";
            adminRegistrationWindow.ShowDialog();
            this.cmbUser.ItemsSource = this.viewModel.ManagedUsers;
            this.cmbUser.SelectedItem = null;
            this.tbcLimits.IsEnabled = false;
        }
    }
}
