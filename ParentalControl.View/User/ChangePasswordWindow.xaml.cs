// <copyright file="ChangePasswordWindow.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParentalControl.View.User
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
    using ParentalControl.VM;

    /// <summary>
    /// Interaction logic for ChangePasswordWindow.xaml.
    /// </summary>
    public partial class ChangePasswordWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangePasswordWindow"/> class.
        /// </summary>
        public ChangePasswordWindow()
        {
            this.InitializeComponent();
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            if (this.pswNewPassword.Password == string.Empty ||
                this.pswNewPasswordAgain.Password == string.Empty)
            {
                MessageBox.Show("Egyik bemenet sem lehet üres!", "Üres bemenet!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (this.pswNewPassword.Password != this.pswNewPasswordAgain.Password)
            {
                MessageBox.Show("A két jelszó nem azonos!", "Hibás jelszó!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                ViewModel.Get().BL.ChangePassword(this.pswNewPassword.Password);
                MessageBox.Show("A jelszóváltoztatás sikeres!", "Siker!", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
        }
    }
}
