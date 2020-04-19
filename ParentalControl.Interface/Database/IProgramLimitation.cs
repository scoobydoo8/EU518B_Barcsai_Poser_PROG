// <copyright file="IProgramLimitation.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParentalControl.Interface.Database
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Program limitation interface.
    /// </summary>
    public interface IProgramLimitation
    {
        /// <summary>
        /// Gets iD.
        /// </summary>
        int ID { get; }

        /// <summary>
        /// Gets userID.
        /// </summary>
        int UserID { get; }

        /// <summary>
        /// Gets or sets name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets path.
        /// </summary>
        string Path { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether full limit.
        /// </summary>
        bool IsFullLimit { get; set; }
    }
}
