// <copyright file="AdminWebLimitationPage.xaml.cs" company="PlaceholderCompany">
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
    /// Interaction logic for AdminWebLimitationPage.xaml.
    /// </summary>
    public partial class AdminWebLimitationPage : Page
    {
        private IAdminViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminWebLimitationPage"/> class.
        /// </summary>
        public AdminWebLimitationPage()
        {
            this.InitializeComponent();
            this.viewModel = ViewModel.Get();
            this.DataContext = this.viewModel;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded)
            {
                var chbKeyword = sender as CheckBox;
                var stpKeyword = chbKeyword.Parent as StackPanel;
                if (stpKeyword.Children.Count > 1)
                {
                    var lblKeyword = stpKeyword.Children[1] as Label;
                    string keywordName = lblKeyword.Content.ToString();
                    var keyword = this.viewModel.BL.Database.ReadKeywords(x => x.Name == keywordName).First();
                    var webLimit = this.viewModel.BL.Database.ReadWebLimitations(x => x.UserID == this.viewModel.SelectedManagedUser.ID && x.KeywordID == keyword.ID).FirstOrDefault();
                    if (webLimit == null)
                    {
                        this.viewModel.BL.Database.Transaction(() => this.viewModel.BL.Database.CreateWebLimitation(this.viewModel.SelectedManagedUser.ID, keyword.ID));
                    }
                }
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded)
            {
                var chbKeyword = sender as CheckBox;
                var stpKeyword = chbKeyword.Parent as StackPanel;
                if (stpKeyword.Children.Count > 1)
                {
                    var lblKeyword = stpKeyword.Children[1] as Label;
                    string keywordName = lblKeyword.Content.ToString();
                    var keyword = this.viewModel.BL.Database.ReadKeywords(x => x.Name == keywordName).First();
                    var webLimit = this.viewModel.BL.Database.ReadWebLimitations(x => x.UserID == this.viewModel.SelectedManagedUser.ID && x.KeywordID == keyword.ID).FirstOrDefault();
                    if (webLimit != null)
                    {
                        this.viewModel.BL.Database.Transaction(() => this.viewModel.BL.Database.DeleteWebLimitations(x => x.UserID == this.viewModel.SelectedManagedUser.ID && x.KeywordID == keyword.ID));
                    }
                }
            }
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            AdminWebLimitationAddOrEditKeywordWindow adminWebLimitationAddOrEditKeywordWindow = new AdminWebLimitationAddOrEditKeywordWindow();
            adminWebLimitationAddOrEditKeywordWindow.Tag = "child";
            adminWebLimitationAddOrEditKeywordWindow.ShowDialog();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            AdminWebLimitationAddOrEditKeywordWindow adminWebLimitationAddOrEditKeywordWindow = new AdminWebLimitationAddOrEditKeywordWindow(this.viewModel.SelectedManagedKeyword);
            adminWebLimitationAddOrEditKeywordWindow.Tag = "child";
            adminWebLimitationAddOrEditKeywordWindow.ShowDialog();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            this.viewModel.BL.Database.Transaction(() => this.viewModel.BL.Database.DeleteKeywords(x => x.ID == this.viewModel.SelectedManagedKeyword.ID));
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.viewModel.SelectedManagedKeyword != null)
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
