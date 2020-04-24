// <copyright file="AdminProgramLimitationPage.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParentalControl.View.Admin
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
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
    using ParentalControl.Interface.Database;
    using ParentalControl.Interface.ViewModel;
    using ParentalControl.VM;

    /// <summary>
    /// Interaction logic for AdminProgramLimitationPage.xaml.
    /// </summary>
    public partial class AdminProgramLimitationPage : Page
    {
        private IAdminViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminProgramLimitationPage"/> class.
        /// </summary>
        public AdminProgramLimitationPage()
        {
            this.InitializeComponent();
            this.viewModel = ViewModel.Get();
            this.DataContext = this.viewModel;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded)
            {
                var chbProgramLimitation = sender as CheckBox;
                var stpProgramLimitation = chbProgramLimitation.Parent as StackPanel;
                if (stpProgramLimitation.Children.Count > 1)
                {
                    var lblProgramLimitation = stpProgramLimitation.Children[1] as Label;
                    string programLimitationString = lblProgramLimitation.Content.ToString();
                    this.viewModel.BL.Database.Transaction(() => this.viewModel.BL.Database.UpdateProgramLimitations(x => x.IsFullLimit = true, x => x.ToString() == programLimitationString));
                }
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded)
            {
                var chbProgramLimitation = sender as CheckBox;
                var stpProgramLimitation = chbProgramLimitation.Parent as StackPanel;
                if (stpProgramLimitation.Children.Count > 1)
                {
                    var lblProgramLimitation = stpProgramLimitation.Children[1] as Label;
                    string programLimitationString = lblProgramLimitation.Content.ToString();
                    this.viewModel.BL.Database.Transaction(() => this.viewModel.BL.Database.UpdateProgramLimitations(x => x.IsFullLimit = false, x => x.ToString() == programLimitationString));
                }
            }
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            AdminProgramLimitationSettingsWindow adminProgramLimitationSettingsWindow = new AdminProgramLimitationSettingsWindow();
            adminProgramLimitationSettingsWindow.Tag = "child";
            adminProgramLimitationSettingsWindow.ShowDialog();
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            AdminProgramLimitationAddOrEditProgramLimitationWindow adminProgramLimitationAddOrEditProgramLimitationWindow = new AdminProgramLimitationAddOrEditProgramLimitationWindow();
            adminProgramLimitationAddOrEditProgramLimitationWindow.Tag = "child";
            adminProgramLimitationAddOrEditProgramLimitationWindow.ShowDialog();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            AdminProgramLimitationAddOrEditProgramLimitationWindow adminProgramLimitationAddOrEditProgramLimitationWindow = new AdminProgramLimitationAddOrEditProgramLimitationWindow(this.viewModel.SelectedManagedProgramLimitation);
            adminProgramLimitationAddOrEditProgramLimitationWindow.Tag = "child";
            adminProgramLimitationAddOrEditProgramLimitationWindow.ShowDialog();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            this.viewModel.BL.Database.Transaction(() => this.viewModel.BL.Database.DeleteProgramLimitations(x => x.ID == this.viewModel.SelectedManagedProgramLimitation.ID));
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.viewModel.SelectedManagedProgramLimitation != null)
            {
                this.btnEdit.IsEnabled = true;
                this.btnDelete.IsEnabled = true;
            }
            else
            {
                this.btnEdit.IsEnabled = false;
                this.btnDelete.IsEnabled = false;
            }
        }
    }
}