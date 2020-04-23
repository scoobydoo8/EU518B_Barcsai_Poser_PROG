// <copyright file="TimeRemainingWindow.xaml.cs" company="PlaceholderCompany">
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
    /// Interaction logic for TimeRemainingWindow.xaml.
    /// </summary>
    public partial class TimeRemainingWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeRemainingWindow"/> class.
        /// </summary>
        public TimeRemainingWindow()
        {
            this.InitializeComponent();
            this.DataContext = (ITimeRemainingViewModel)ViewModel.Get();
        }
    }
}
