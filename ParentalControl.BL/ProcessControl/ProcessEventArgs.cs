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

    /// <summary>
    /// Process event handler.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="e">Event args.</param>
    public delegate void ProcessEventHandler(object sender, ProcessEventArgs e);

    /// <summary>
    /// Process event args class.
    /// </summary>
    public class ProcessEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessEventArgs"/> class.
        /// </summary>
        public ProcessEventArgs()
            : base()
        {
        }

        /// <summary>
        /// Gets iD.
        /// </summary>
        public int ID { get; internal set; }

        /// <summary>
        /// Gets process name.
        /// </summary>
        public string ProcessName { get; internal set; }

        /// <summary>
        /// Gets from time.
        /// </summary>
        public TimeSpan FromTime { get; internal set; }
    }
}
