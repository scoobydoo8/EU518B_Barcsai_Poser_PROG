﻿// <copyright file="MainWindow.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParentalControl.AdminConfig
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.ServiceProcess;
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
    using ParentalControl.Data;
    using ParentalControl.Data.Database;

    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        private DatabaseManager databaseManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.databaseManager = DatabaseManager.Get();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.databaseManager.ReadUsers((Func<User, bool>)null).Count > 0)
            {
                this.StartService("ParentalControl.Service", 5000);
                this.Close();
            }

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

            if (this.databaseManager.Transaction(() => this.databaseManager.CreateUser(this.txtUsername.Text, this.GetHash(this.pswPassword.Password), this.txtSecurityQuestion.Text, this.GetHash(this.pswSecurityAnswer.Password))))
            {
                MessageBox.Show("A regisztráció sikeres!", "Siker!", MessageBoxButton.OK, MessageBoxImage.Information);
                this.StartService("ParentalControl.Service", 5000);
                this.Close();
            }
            else
            {
                MessageBox.Show("A regisztráció nem sikerült!", "Sikertelen!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void StartService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch (Exception e)
            {
                MessageBox.Show("A szolgáltatás elindítása nem sikerült!\nLehetséges, hogy a program nem települt teljesen, kérjük telepítse újra!\nA hiba részletes leírása: " + e.Message, "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetHash(string rawstring)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawstring));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}