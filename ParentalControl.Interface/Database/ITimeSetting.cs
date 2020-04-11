﻿// <copyright file="ITimeSetting.cs" company="PlaceholderCompany">
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
    /// ITimeSetting interface.
    /// </summary>
    public interface ITimeSetting
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