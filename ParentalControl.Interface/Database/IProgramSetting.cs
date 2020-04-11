// <copyright file="IProgramSetting.cs" company="PlaceholderCompany">
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
    /// IProgramSetting interface.
    /// </summary>
    public interface IProgramSetting
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
        /// Gets or sets a value indicating whether occasional.
        /// </summary>
        bool Occasional { get; set; }

        /// <summary>
        /// Gets or sets minutes.
        /// </summary>
        int Minutes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether repeat.
        /// </summary>
        bool Repeat { get; set; }

        /// <summary>
        /// Gets or sets pause.
        /// </summary>
        int Pause { get; set; }

        /// <summary>
        /// Gets or sets quantity.
        /// </summary>
        int Quantity { get; set; }

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
