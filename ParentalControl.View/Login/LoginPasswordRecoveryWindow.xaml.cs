// <copyright file="LoginPasswordRecoveryWindow.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParentalControl.View.Login
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
    using System.Windows.Shapes;
    using ParentalControl.Interface.ViewModel;
    using ParentalControl.VM;

    /// <summary>
    /// Interaction logic for LoginPasswordRecoveryWindow.xaml.
    /// </summary>
    public partial class LoginPasswordRecoveryWindow : Window, INotifyPropertyChanged
    {
        private IViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginPasswordRecoveryWindow"/> class.
        /// </summary>
        public LoginPasswordRecoveryWindow()
        {
            this.InitializeComponent();
            this.IsUsernameValid = false;
            this.DataContext = this;
            this.viewModel = ViewModel.Get();
        }

        /// <summary>
        /// Property changed event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets a value indicating whether username is valid.
        /// </summary>
        public bool IsUsernameValid { get; private set; }

        /// <summary>
        /// Gets a value indicating whether username is not valid.
        /// </summary>
        public bool IsUsernameNotValid { get => !this.IsUsernameValid; }

        private void IsUsernameValid_Click(object sender, RoutedEventArgs e)
        {
            if (this.txtUsername.Text == string.Empty)
            {
                MessageBox.Show("A felhasználónév nem lehet üres!", "Üres felhasználó!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = this.viewModel.BL.Database.ReadUsers(x => x.Username == this.txtUsername.Text).FirstOrDefault();
            if (user != null)
            {
                this.txtSecurityQuestion.Text = user.SecurityQuestion;
                this.IsUsernameValid = true;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.IsUsernameValid)));
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.IsUsernameNotValid)));
                this.pswSecurityAnswer.Focus();
            }
            else
            {
                MessageBox.Show("Nem létezik ilyen felhasználó!", "Nem létező felhasználó!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void PasswordRecovery_Click(object sender, RoutedEventArgs e)
        {
            if (this.pswNewPassword.Password == string.Empty ||
                this.pswNewPasswordAgain.Password == string.Empty ||
                this.pswSecurityAnswer.Password == string.Empty)
            {
                MessageBox.Show("Egyik bemenet sem lehet üres!", "Üres bemenet!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (this.pswNewPassword.Password != this.pswNewPasswordAgain.Password)
            {
                MessageBox.Show("A két jelszó nem azonos!", "Hibás jelszó!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (this.viewModel.BL.PasswordRecovery(this.txtUsername.Text, this.pswSecurityAnswer.Password, this.pswNewPassword.Password))
            {
                MessageBox.Show("A jelszó visszaállítás sikeres!", "Siker!", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("A jelszó visszaállítás sikertelen!\nA biztonsági válasz nem megfelelő!", "Sikertelen!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.txtUsername.Focus();
        }
    }
}
