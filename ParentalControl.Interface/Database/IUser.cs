// <copyright file="IUser.cs" company="PlaceholderCompany">
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
    /// User interface.
    /// </summary>
    public interface IUser
    {
        /// <summary>
        /// Gets iD.
        /// </summary>
        int ID { get; }

        /// <summary>
        /// Gets username.
        /// </summary>
        string Username { get; }

        /// <summary>
        /// Gets security question.
        /// </summary>
        string SecurityQuestion { get; }

        /// <summary>
        /// Gets or sets a value indicating whether occasional.
        /// </summary>
        bool Occasional { get; set; }

        /// <summary>
        /// Gets or sets minutes.
        /// </summary>
        int Minutes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether orderly.
        /// </summary>
        bool Orderly { get; set; }

        /// <summary>
        /// Gets or sets from time.
        /// </summary>
        TimeSpan FromTime { get; set; }

        /// <summary>
        /// Gets or sets to time.
        /// </summary>
        TimeSpan ToTime { get; set; }
    }
}
