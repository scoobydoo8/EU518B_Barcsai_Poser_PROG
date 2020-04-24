// <copyright file="AdminProgramLimitationSettingsWindow.xaml.cs" company="PlaceholderCompany">
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
    using ParentalControl.Interface.ViewModel;
    using ParentalControl.VM;

    /// <summary>
    /// Interaction logic for AdminProgramLimitationSettingsWindow.xaml
    /// </summary>
    public partial class AdminProgramLimitationSettingsWindow : Window
    {
        private IAdminViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminProgramLimitationSettingsWindow"/> class.
        /// </summary>
        public AdminProgramLimitationSettingsWindow()
        {
            this.InitializeComponent();
            this.viewModel = ViewModel.Get();
        }

        private void OrderlyFree_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void OrderlyFree_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
