// <copyright file="AdminWebLimitationAddOrEditKeywordWindow.xaml.cs" company="PlaceholderCompany">
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
    using ParentalControl.Interface.Database;
    using ParentalControl.Interface.ViewModel;
    using ParentalControl.VM;

    /// <summary>
    /// Interaction logic for AdminWebLimitationAddOrEditKeywordWindow.xaml.
    /// </summary>
    public partial class AdminWebLimitationAddOrEditKeywordWindow : Window
    {
        private IAdminViewModel viewModel;
        private IKeyword keyword;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminWebLimitationAddOrEditKeywordWindow"/> class.
        /// </summary>
        public AdminWebLimitationAddOrEditKeywordWindow()
        {
            this.InitializeComponent();
            this.viewModel = ViewModel.Get();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminWebLimitationAddOrEditKeywordWindow"/> class.
        /// </summary>
        /// <param name="keyword">Keyword.</param>
        public AdminWebLimitationAddOrEditKeywordWindow(IKeyword keyword)
            : this()
        {
            this.keyword = keyword;
            this.Title = "Kulcsszó szerkesztése";
            this.txtName.Text = this.keyword.Name;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.txtName.Focus();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (this.txtName.Text == string.Empty)
            {
                MessageBox.Show("A kulcsszó nem lehet üres!", "Üres kulcsszó!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!this.viewModel.BL.Database.ReadKeywords(x => x.Name == this.txtName.Text).Any())
            {
                if (this.keyword == null)
                {
                    this.viewModel.BL.Database.Transaction(() => this.viewModel.BL.Database.CreateKeyword(this.txtName.Text));
                }
                else
                {
                    this.viewModel.BL.Database.Transaction(() => this.viewModel.BL.Database.UpdateKeywords(x => x.Name = this.txtName.Text, x => x.ID == this.keyword.ID));
                }

                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("A kulcsszó már létezik!", "Létező kulcsszó!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
