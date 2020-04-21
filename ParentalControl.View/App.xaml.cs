﻿// <auto-generated/>
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ParentalControl.View
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var exceptionMessage = e.Exception.GetType().Name + ":" + e.Exception.Message;
            Logger.Get().LogException(exceptionMessage);
            MessageBox.Show("Hiba történt:\n" + e.Exception.Message, "Hina!", MessageBoxButton.OK, MessageBoxImage.Warning);
            e.Handled = true;
        }
    }
}
