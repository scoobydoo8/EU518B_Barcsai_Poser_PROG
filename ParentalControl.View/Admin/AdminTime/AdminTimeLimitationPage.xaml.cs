// <copyright file="AdminTimeLimitationPage.xaml.cs" company="PlaceholderCompany">
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
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using ParentalControl.Interface.ViewModel;
    using ParentalControl.VM;

    /// <summary>
    /// Interaction logic for AdminTimeLimitationPage.xaml.
    /// </summary>
    public partial class AdminTimeLimitationPage : Page
    {
        private IAdminViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminTimeLimitationPage"/> class.
        /// </summary>
        public AdminTimeLimitationPage()
        {
            this.InitializeComponent();
            this.viewModel = ViewModel.Get();
            this.DataContext = this.viewModel.SelectedManagedUser;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.chbOrderlyFree.IsEnabled = this.chbFullFree.IsChecked != true;
            this.txtFromTime.IsEnabled = this.chbFullFree.IsChecked != true && this.chbOrderlyFree.IsChecked == true;
            this.txtToTime.IsEnabled = this.chbFullFree.IsChecked != true && this.chbOrderlyFree.IsChecked == true;
            this.txtOccasionalMinutes.IsEnabled = this.chbFullFree.IsChecked != true;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (this.txtFromTime.Text == "00:00:00" && this.txtToTime.Text == "00:00:00")
            {
                this.txtToTime.Text = "23:59:00";
            }

            if (TimeSpan.Parse(this.txtFromTime.Text) > TimeSpan.Parse(this.txtToTime.Text))
            {
                var help = this.txtFromTime.Text;
                this.txtFromTime.Text = this.txtToTime.Text;
                this.txtToTime.Text = help;
            }

            this.viewModel.BL.Database.Transaction(() => this.viewModel.BL.Database.UpdateUsers(
                x =>
                {
                    x.IsTimeLimitInactive = this.viewModel.SelectedManagedUser.IsTimeLimitInactive;
                    x.IsTimeLimitOrderly = this.viewModel.SelectedManagedUser.IsTimeLimitOrderly;
                    x.TimeLimitFromTime = this.viewModel.SelectedManagedUser.TimeLimitFromTime;
                    x.TimeLimitToTime = this.viewModel.SelectedManagedUser.TimeLimitToTime;
                    x.TimeLimitOccasionalMinutes = this.viewModel.SelectedManagedUser.TimeLimitOccasionalMinutes;
                },
                x => x.ID == this.viewModel.SelectedManagedUser.ID));
        }

        private void FullFree_Checked(object sender, RoutedEventArgs e)
        {
            this.chbOrderlyFree.IsEnabled = false;
            this.txtFromTime.IsEnabled = false;
            this.txtToTime.IsEnabled = false;
            this.txtOccasionalMinutes.IsEnabled = false;
        }

        private void FullFree_Unchecked(object sender, RoutedEventArgs e)
        {
            this.chbOrderlyFree.IsEnabled = true;
            this.txtFromTime.IsEnabled = this.chbOrderlyFree.IsChecked == true;
            this.txtToTime.IsEnabled = this.chbOrderlyFree.IsChecked == true;
            this.txtOccasionalMinutes.IsEnabled = true;
        }

        private void OrderlyFree_Checked(object sender, RoutedEventArgs e)
        {
            this.txtFromTime.IsEnabled = true;
            this.txtToTime.IsEnabled = true;
            if (this.txtFromTime.Text == "00:00:00" && this.txtToTime.Text == "00:00:00")
            {
                this.txtToTime.Text = "23:59:00";
            }
        }

        private void OrderlyFree_Unchecked(object sender, RoutedEventArgs e)
        {
            this.txtFromTime.IsEnabled = false;
            this.txtToTime.IsEnabled = false;
        }
    }
}
