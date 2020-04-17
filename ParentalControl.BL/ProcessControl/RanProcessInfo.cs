// <copyright file="RanProcessInfo.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParentalControl.BL.ProcessControl
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Ran process info class.
    /// </summary>
    public class RanProcessInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RanProcessInfo"/> class.
        /// </summary>
        /// <param name="process">Process.</param>
        public RanProcessInfo(Process process)
        {
            this.Process = process;
            this.From = DateTime.Now;
        }

        /// <summary>
        /// Gets or sets iD.
        /// </summary>
        public Process Process { get; set; }

        /// <summary>
        /// Gets or sets from.
        /// </summary>
        public DateTime From { get; set; }

        /// <summary>
        /// Gets or sets to.
        /// </summary>
        public DateTime To { get; set; }
    }
}
