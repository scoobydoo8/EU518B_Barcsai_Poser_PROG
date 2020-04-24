// <copyright file="KeywordAndWebLimitationToBooleanConverter.cs" company="PlaceholderCompany">
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
    /// Keyword and web limitation to boolean converter class.
    /// </summary>
    public class KeywordAndWebLimitationToBooleanConverter : IValueConverter
    {
        private IAdminViewModel viewModel;

        /// <summary>
        /// Convert.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="parameter">Parameter.</param>
        /// <param name="culture">Culture.</param>
        /// <returns>Boolean.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IKeyword keyword = value as IKeyword;
            this.viewModel = ViewModel.Get();
            var webLimits = this.viewModel.SelectedManagedUser != null ? this.viewModel.BL.Database.ReadWebLimitations(x => x.UserID == this.viewModel.SelectedManagedUser.ID && x.KeywordID == keyword.ID) : null;
            if (webLimits != null && keyword != null && webLimits.Where(x => x.KeywordID == keyword.ID).Any())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Convert back.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="parameter">Parameter.</param>
        /// <param name="culture">Culture.</param>
        /// <returns>Not implemented exception.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
