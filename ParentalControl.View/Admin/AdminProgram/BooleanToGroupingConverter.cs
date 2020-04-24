// <copyright file="BooleanToGroupingConverter.cs" company="PlaceholderCompany">
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
    using System.Windows.Data;
    using ParentalControl.Interface.ViewModel;

    /// <summary>
    /// Boolean to grouping converter class.
    /// </summary>
    public class BooleanToGroupingConverter : IValueConverter
    {
        private IAdminViewModel viewModel;

        /// <summary>
        /// Convert.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="parameter">Parameter.</param>
        /// <param name="culture">Culture.</param>
        /// <returns>Grouping.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool fullLimit = (value as bool?) == true;
            if (fullLimit)
            {
                return "Tiltott";
            }

            return "Részlegesen tiltott";
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
