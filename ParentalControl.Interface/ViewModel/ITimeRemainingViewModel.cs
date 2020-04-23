// <copyright file="ITimeRemainingViewModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParentalControl.Interface.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Time remaining view model interface.
    /// </summary>
    public interface ITimeRemainingViewModel : IViewModel
    {
        /// <summary>
        /// Gets time limitation remaining time.
        /// </summary>
        TimeSpan TimeRemainingTime { get; }

        /// <summary>
        /// Gets program limitation remining time.
        /// </summary>
        TimeSpan ProgramRemainingTime { get; }
    }
}
