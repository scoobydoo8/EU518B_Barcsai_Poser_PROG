// <copyright file="IViewModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParentalControl.Interface.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ParentalControl.Interface.BusinessLogic;
    using ParentalControl.Interface.Database;

    /// <summary>
    /// View model interface.
    /// </summary>
    public interface IViewModel
    {
        /// <summary>
        /// Gets business logic.
        /// </summary>
        IBusinessLogic BL { get; }

        /// <summary>
        /// Gets active user.
        /// </summary>
        IUser ActiveUser { get; }
    }
}
