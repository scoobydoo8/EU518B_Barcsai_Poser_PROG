// <copyright file="IAdminViewModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParentalControl.Interface.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
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
        ObservableCollection<IUser> ManagedUsers { get; }

        /// <summary>
        /// Gets or sets selected managed user.
        /// </summary>
        IUser SelectedManagedUser { get; set; }

        /// <summary>
        /// Gets managed program limitations.
        /// </summary>
        ObservableCollection<IProgramLimitation> ManagedProgramLimitations { get; }

        /// <summary>
        /// Gets or sets selected managed program limitation.
        /// </summary>
        IProgramLimitation SelectedManagedProgramLimitation { get; set; }

        /// <summary>
        /// Gets managed web limitations.
        /// </summary>
        ObservableCollection<IWebLimitation> ManagedWebLimitations { get; }

        /// <summary>
        /// Gets or sets selected managed web limitation.
        /// </summary>
        IWebLimitation SelectedManagedWebLimitation { get; set; }

        /// <summary>
        /// Gets managed keywords.
        /// </summary>
        ObservableCollection<IKeyword> ManagedKeywords { get; }

        /// <summary>
        /// Gets or sets selected managed keyword.
        /// </summary>
        IKeyword SelectedManagedKeyword { get; set; }

        /// <summary>
        /// User selection changed.
        /// </summary>
        void User_SelectionChanged();
    }
}
