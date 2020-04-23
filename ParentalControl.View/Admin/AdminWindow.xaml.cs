// <copyright file="AdminWindow.xaml.cs" company="PlaceholderCompany">
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
    /// Interaction logic for AdminWindow.xaml.
    /// </summary>
    public partial class AdminWindow : Window
    {
        private IAdminViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminWindow"/> class.
        /// </summary>
        public AdminWindow()
        {
            this.InitializeComponent();
            this.viewModel = ViewModel.Get();
            this.DataContext = this.viewModel;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
