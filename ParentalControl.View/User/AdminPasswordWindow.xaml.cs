// <copyright file="AdminPasswordWindow.xaml.cs" company="PlaceholderCompany">
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
    using ParentalControl.Interface.ViewModel;
    using ParentalControl.VM;

    /// <summary>
    /// Occasional permission enum.
    /// </summary>
    public enum OccasionalPermission
    {
        /// <summary>
        /// Time limit.
        /// </summary>
        TimeLimit = 0,

        /// <summary>
        /// Program limit.
        /// </summary>
        ProgramLimit = 1,
    }

    /// <summary>
    /// Interaction logic for AdminPasswordWindow.xaml.
    /// </summary>
    public partial class AdminPasswordWindow : Window
    {
        private IViewModel viewModel;
        private OccasionalPermission occasionalPermission;
        private int processID;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminPasswordWindow"/> class.
        /// </summary>
        /// <param name="occasionalPermission">Occasional permission.</param>
        /// <param name="processID">Process ID.</param>
        public AdminPasswordWindow(OccasionalPermission occasionalPermission, int processID = 0)
        {
            this.InitializeComponent();
            this.viewModel = ViewModel.Get();
            this.occasionalPermission = occasionalPermission;
            if (this.occasionalPermission == OccasionalPermission.TimeLimit)
            {
                this.txtMinutes.Text = this.viewModel.ActiveUser.TimeLimitOccasionalMinutes.ToString();
            }
            else
            {
                this.txtMinutes.Text = this.viewModel.ActiveUser.ProgramLimitOccasionalMinutes.ToString();
                this.processID = processID;
                if (this.processID != 0 && this.viewModel.BL.ProgramRemainingTime != default)
                {
                    if (this.viewModel.BL.ProcessController.GetOccasionalPermission(processID))
                    {
                        this.DialogResult = true;
                    }
                    else
                    {
                        this.DialogResult = false;
                    }
                }
            }
        }

        private void OccasionalPermission_Click(object sender, RoutedEventArgs e)
        {
            if (this.txtUsername.Text == string.Empty ||
                this.pswPassword.Password == string.Empty ||
                this.txtMinutes.Text == string.Empty)
            {
                MessageBox.Show("Egyik bemenet sem lehet üres!", "Üres bemenet!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (int.TryParse(this.txtMinutes.Text, out int minutes))
            {
                if (minutes < 1)
                {
                    MessageBox.Show("A perc csak nullánál nagyobb szám lehet!", "Hibás perc!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (this.occasionalPermission == OccasionalPermission.TimeLimit)
                {
                    if (this.viewModel.BL.TimeRemainingTime == default || this.viewModel.BL.TimeRemainingTime.TotalMinutes < minutes)
                    {
                        if (this.viewModel.BL.IsOccassionalPermission(this.txtUsername.Text, this.pswPassword.Password, minutes))
                        {
                            this.DialogResult = true;
                        }
                        else
                        {
                            MessageBox.Show("Az eseti engedélyeztetés sikertelen!\nValószínűleg hibás a belépés!", "Sikertelen!", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Az idő csak növelhető, változás nem történt!", "Nincs változás", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    if (this.viewModel.BL.ProgramRemainingTime == default || this.viewModel.BL.ProgramRemainingTime.TotalMinutes < minutes)
                    {
                        if (this.viewModel.BL.ProcessController.IsOccassionalPermission(this.txtUsername.Text, this.pswPassword.Password, minutes, this.processID))
                        {
                            this.DialogResult = true;
                        }
                        else
                        {
                            MessageBox.Show("Az eseti engedélyeztetés sikertelen!\nValószínűleg hibás a belépés!", "Sikertelen!", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Az idő csak növelhető, változás nem történt!", "Nincs változás", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            else
            {
                MessageBox.Show("Nem megfelelő perc!", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.txtUsername.Focus();
        }
    }
}
