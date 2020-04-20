// <copyright file="IAdminViewModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParentalControl.Interface.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ParentalControl.Interface.Database;

    /// <summary>
    /// Admin view model class.
    /// </summary>
    public interface IAdminViewModel : IViewModel
    {
        /// <summary>
        /// Gets managed users.
        /// </summary>
        List<IUser> ManagedUsers { get; }

        /// <summary>
        /// Gets managed program limitations.
        /// </summary>
        List<IProgramLimitation> ManagedProgramLimitations { get; }

        /// <summary>
        /// Gets managed web limitations.
        /// </summary>
        List<IWebLimitation> ManagedWebLimitations { get; }

        /// <summary>
        /// Gets managed keywords.
        /// </summary>
        List<IKeyword> ManagedKeywords { get; }
    }
}
