// <copyright file="AdminRegistrationWindow.xaml.cs" company="PlaceholderCompany">
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
    /// Interaction logic for AdminRegistrationWindow.xaml.
    /// </summary>
    public partial class AdminRegistrationWindow : Window
    {
        private IViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminRegistrationWindow"/> class.
        /// </summary>
        public AdminRegistrationWindow()
        {
            this.InitializeComponent();
            this.viewModel = ViewModel.Get();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.txtUsername.Focus();
        }

        private void Registration_Click(object sender, RoutedEventArgs e)
        {
            if (this.txtUsername.Text == string.Empty ||
                this.pswPassword.Password == string.Empty ||
                this.txtSecurityQuestion.Text == string.Empty ||
                this.pswSecurityAnswer.Password == string.Empty)
            {
                MessageBox.Show("Egyik bemenet sem lehet üres!", "Üres bemenet!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = this.viewModel.BL.Database.ReadUsers(x => x.Username == this.txtUsername.Text).FirstOrDefault();
            if (user == null)
            {
                if (this.viewModel.BL.Registration(this.txtUsername.Text, this.pswPassword.Password, this.txtSecurityQuestion.Text, this.pswSecurityAnswer.Password))
                {
                    MessageBox.Show("A regisztráció sikeres!", "Siker!", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                }
                else
                {
                    MessageBox.Show("A regisztráció nem sikerült!", "Sikertelen!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Ilyen felhasználó már létezik!", "Létező felhasználó!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}