// <copyright file="IProcessEventArgs.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParentalControl.Interface.ProcessControl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Process event args interface.
    /// </summary>
    public interface IProcessEventArgs
    {
        /// <summary>
        /// Gets iD.
        /// </summary>
        int ID { get; }

        /// <summary>
        /// Gets process name.
        /// </summary>
        string ProcessName { get; }
    }
}
