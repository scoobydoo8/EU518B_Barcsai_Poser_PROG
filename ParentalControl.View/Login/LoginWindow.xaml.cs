// <copyright file="LoginWindow.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParentalControl.View.Login
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
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
    using ParentalControl.Interface.ProcessControl;
    using ParentalControl.Interface.ViewModel;
    using ParentalControl.View.Admin;
    using ParentalControl.View.User;
    using ParentalControl.VM;

    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class LoginWindow : Window
    {
        private IViewModel viewModel;
        private AdminWindow adminWindow;
        private TimeRemainingWindow timeRemainingWindow;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginWindow"/> class.
        /// </summary>
        public LoginWindow()
        {
            this.InitializeComponent();
            this.notifyIcon.Icon = Properties.Resources.favicon;
            this.mniOccasionalTime.Icon = new Image { Source = Properties.Resources.clock.ToImageSource() };
            this.mniOccasionalProgram.Icon = new Image { Source = Properties.Resources.clock.ToImageSource() };
            this.mniChangePassword.Icon = new Image { Source = Properties.Resources.gear.ToImageSource() };
            this.mniLogOut.Icon = new Image { Source = Properties.Resources.logout.ToImageSource() };
            this.viewModel = ViewModel.Get();
            this.viewModel.BL.UserLoggedInFull += this.BL_UserLoggedInFull;
            this.viewModel.BL.UserLoggedInOccassional += this.BL_UserLoggedInOccassional;
            this.viewModel.BL.UserLoggedInOrderlyOrActiveOccasional += this.BL_UserLoggedInOrderlyOrActiveOccasional;
            this.viewModel.BL.UserLoggedOut += this.BL_UserLoggedOut;
            this.viewModel.BL.ProcessController.ProgramStartedFullLimit += this.ProcessController_ProgramStartedFullLimit;
            this.viewModel.BL.ProcessController.ProgramStartedOccassional += this.ProcessController_ProgramStartedOccassional;
            this.viewModel.BL.ProcessController.ProgramStartedOrderlyOrActiveOccasional += this.ProcessController_ProgramStartedOrderlyOrActiveOccasional;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.txtUsername.Focus();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (this.txtUsername.Text == string.Empty ||
                this.pswPassword.Password == string.Empty)
            {
                MessageBox.Show("Egyik bemenet sem lehet üres!", "Üres bemenet!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!this.viewModel.BL.LogIn(this.txtUsername.Text, this.pswPassword.Password))
            {
                MessageBox.Show("A bejelentkezés sikertelen!\nA felhasználónév vagy a jelszó nem megfelelő!", "Sikertelen!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PasswordRecovery_Click(object sender, RoutedEventArgs e)
        {
            new LoginPasswordRecoveryWindow().ShowDialog();
        }

        private void BL_UserLoggedInFull(object sender, EventArgs e)
        {
            this.Hide();
            this.mniChangePassword.IsEnabled = true;
            this.mniLogOut.IsEnabled = true;
            if (this.viewModel.ActiveUser.ID == this.viewModel.BL.Database.AdminID)
            {
                this.notifyIcon.MouseDoubleClick += this.NotifyIcon_MouseDoubleClick_Admin;
                this.notifyIcon.BalloonTipText = "A gyorsindító ikonra dupla kattintással érhető el az adminisztrációs felület.";
                this.notifyIcon.ShowBalloonTip(2000);
            }
        }

        private void BL_UserLoggedInOccassional(object sender, EventArgs e)
        {
            AdminPasswordWindow adminPasswordWindow = new AdminPasswordWindow(OccasionalPermission.TimeLimit);
            if (adminPasswordWindow.ShowDialog() == true)
            {
                this.LimitedUserLoggedIn();
            }
            else
            {
                this.viewModel.BL.LogOut();
            }
        }

        private void BL_UserLoggedInOrderlyOrActiveOccasional(object sender, EventArgs e)
        {
            this.LimitedUserLoggedIn();
        }

        private void LimitedUserLoggedIn()
        {
            this.Hide();
            this.mniOccasionalTime.IsEnabled = true;
            this.mniChangePassword.IsEnabled = true;
            this.mniLogOut.IsEnabled = true;
            this.notifyIcon.MouseDoubleClick += this.NotifyIcon_MouseDoubleClick_User;
            this.notifyIcon.BalloonTipText = "A gyorsindító ikonra dupla kattintással tekinthető meg a hátralévő idő.";
            this.notifyIcon.ShowBalloonTip(2000);
        }

        private void BL_UserLoggedOut(object sender, EventArgs e)
        {
            this.txtUsername.Text = string.Empty;
            this.pswPassword.Password = string.Empty;
            this.mniOccasionalTime.IsEnabled = false;
            this.mniOccasionalProgram.IsEnabled = false;
            this.mniChangePassword.IsEnabled = false;
            this.mniLogOut.IsEnabled = false;
            this.notifyIcon.MouseDoubleClick -= this.NotifyIcon_MouseDoubleClick_Admin;
            this.notifyIcon.MouseDoubleClick -= this.NotifyIcon_MouseDoubleClick_User;
            if (this.adminWindow != null)
            {
                this.adminWindow.Close();
                this.adminWindow = null;
            }

            if (this.timeRemainingWindow != null)
            {
                this.timeRemainingWindow.Close();
                this.timeRemainingWindow = null;
            }

            this.Show();
        }

        private void ProcessController_ProgramStartedFullLimit(object sender, IProcessEventArgs e)
        {
            MessageBox.Show("Nincs engedélyed ezen program futtatására!\nProgram: " + e.ProcessName, "Tiltott!", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void ProcessController_ProgramStartedOccassional(object sender, IProcessEventArgs e)
        {
            AdminPasswordWindow adminPasswordWindow = new AdminPasswordWindow(OccasionalPermission.ProgramLimit, e.ID);
            if (adminPasswordWindow.ShowDialog() == true)
            {
                this.LimitedProgramStarted();
            }
        }

        private void ProcessController_ProgramStartedOrderlyOrActiveOccasional(object sender, IProcessEventArgs e)
        {
            this.LimitedProgramStarted();
        }

        private void LimitedProgramStarted()
        {
            this.mniOccasionalProgram.IsEnabled = true;
            this.notifyIcon.MouseDoubleClick += this.NotifyIcon_MouseDoubleClick_User;
            this.notifyIcon.BalloonTipText = "A gyorsindító ikonra dupla kattintással tekinthető meg a hátralévő idő.";
            this.notifyIcon.ShowBalloonTip(2000);
        }

        private void NotifyIcon_MouseDoubleClick_Admin(object sender, MouseButtonEventArgs e)
        {
            if (this.adminWindow == null)
            {
                this.adminWindow = new AdminWindow();
                this.adminWindow.Closed += this.AdminWindow_Closed;
                this.adminWindow?.Show();
            }
            else
            {
                this.adminWindow?.Focus();
            }
        }

        private void AdminWindow_Closed(object sender, EventArgs e)
        {
            this.adminWindow = null;
        }

        private void NotifyIcon_MouseDoubleClick_User(object sender, MouseButtonEventArgs e)
        {
            if (this.timeRemainingWindow == null)
            {
                this.timeRemainingWindow = new TimeRemainingWindow();
                this.timeRemainingWindow.Closed += this.TimeRemainingWindow_Closed;
                this.timeRemainingWindow?.Show();
            }
            else
            {
                this.timeRemainingWindow?.Focus();
            }
        }

        private void TimeRemainingWindow_Closed(object sender, EventArgs e)
        {
            this.timeRemainingWindow = null;
        }

        private void OccasionalTime_Click(object sender, RoutedEventArgs e)
        {
            AdminPasswordWindow adminPasswordWindow = new AdminPasswordWindow(OccasionalPermission.TimeLimit);
            adminPasswordWindow.ShowDialog();
        }

        private void OccasionalProgram_Click(object sender, RoutedEventArgs e)
        {
            AdminPasswordWindow adminPasswordWindow = new AdminPasswordWindow(OccasionalPermission.ProgramLimit);
            adminPasswordWindow.ShowDialog();
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            ChangePasswordWindow changePasswordWindow = new ChangePasswordWindow();
            changePasswordWindow.ShowDialog();
        }

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            this.viewModel.BL.LogOut();
        }
    }
}
