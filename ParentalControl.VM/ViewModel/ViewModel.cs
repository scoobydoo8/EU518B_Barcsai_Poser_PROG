// <copyright file="ViewModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParentalControl.VM
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ParentalControl.BL;

    /// <summary>
    /// View model class.
    /// </summary>
    public class ViewModel
    {
        private static ViewModel viewModel;
        private BusinessLogic businessLogic;

        private ViewModel()
        {
            this.businessLogic = BusinessLogic.Get();
        }

        /// <summary>
        /// Singleton.
        /// </summary>
        /// <returns>View model.</returns>
        public static ViewModel Get()
        {
            if (viewModel == null)
            {
                viewModel = new ViewModel();
            }

            return viewModel;
        }
    }
}
