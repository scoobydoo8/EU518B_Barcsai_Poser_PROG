// <copyright file="AdminProgramLimitationAddOrEditProgramLimitationWindow.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParentalControl.View.Admin
{
    using System;
    using System.Collections.Generic;
    using System.IO;
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
    using Microsoft.Win32;
    using ParentalControl.Interface.Database;
    using ParentalControl.Interface.ViewModel;
    using ParentalControl.VM;

    /// <summary>
    /// Interaction logic for AdminProgramLimitationAddOrEditProgramLimitationWindow.xaml.
    /// </summary>
    public partial class AdminProgramLimitationAddOrEditProgramLimitationWindow : Window
    {
        private IAdminViewModel viewModel;
        private IProgramLimitation programLimitation;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminProgramLimitationAddOrEditProgramLimitationWindow"/> class.
        /// </summary>
        public AdminProgramLimitationAddOrEditProgramLimitationWindow()
        {
            this.InitializeComponent();
            this.viewModel = ViewModel.Get();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminProgramLimitationAddOrEditProgramLimitationWindow"/> class.
        /// </summary>
        /// <param name="programLimitation">Program limitation.</param>
        public AdminProgramLimitationAddOrEditProgramLimitationWindow(IProgramLimitation programLimitation)
            : this()
        {
            this.programLimitation = programLimitation;
            this.Title = "Program korlátozás szerkesztése";
            this.txtName.Text = this.programLimitation.Name;
            this.txtPath.Text = this.programLimitation.Path;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (this.txtName.Text == string.Empty ||
                this.txtPath.Text == string.Empty)
            {
                MessageBox.Show("Egyik bemenet sem lehet üres!", "Üres bemenet!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!this.viewModel.BL.Database.ReadProgramLimitations(x => x.Path == this.txtPath.Text).Any())
            {
                if (this.programLimitation == null)
                {
                    this.viewModel.BL.Database.Transaction(() => this.viewModel.BL.Database.CreateProgramLimitation(this.viewModel.SelectedManagedUser.ID, this.txtName.Text, this.txtPath.Text));
                }
                else
                {
                    this.viewModel.BL.Database.Transaction(() => this.viewModel.BL.Database.UpdateProgramLimitations(
                        x =>
                        {
                            x.Name = this.txtName.Text;
                            x.Path = this.txtPath.Text.ToLower();
                        }, x => x.ID == this.programLimitation.ID));
                }

                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("A program korlátozás már létezik!", "Létező program korlátozás!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Futtatható fájlok (*.exe)|*.exe|Parancsikonok (*.lnk)|*.lnk";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (openFileDialog.ShowDialog() == true)
            {
                var path = openFileDialog.FileName;
                if (Path.GetExtension(path) == ".lnk")
                {
                    path = path.GetShortcutTargetFile();
                }

                this.txtPath.Text = path;
            }
        }
    }
}
