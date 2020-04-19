// <copyright file="ProcessEventArgs.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParentalControl.BL.ProcessControl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ParentalControl.Interface.ProcessControl;

    /// <summary>
    /// Process event args class.
    /// </summary>
    public class ProcessEventArgs : EventArgs, IProcessEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessEventArgs"/> class.
        /// </summary>
        public ProcessEventArgs()
            : base()
        {
        }

        /// <inheritdoc/>
        public int ID { get; internal set; }

        /// <inheritdoc/>
        public string ProcessName { get; internal set; }
    }
}
