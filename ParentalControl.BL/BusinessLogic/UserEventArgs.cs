// <copyright file="UserEventArgs.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParentalControl.BL
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// User event handler.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="e">Event args.</param>
    public delegate void UserEventHandler(object sender, UserEventArgs e);

    /// <summary>
    /// User event args class.
    /// </summary>
    public class UserEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserEventArgs"/> class.
        /// </summary>
        public UserEventArgs()
            : base()
        {
        }

        /// <summary>
        /// Gets from time.
        /// </summary>
        public TimeSpan FromTime { get; internal set; }
    }
}
