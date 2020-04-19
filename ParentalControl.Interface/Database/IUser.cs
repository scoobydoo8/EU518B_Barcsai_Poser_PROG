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
        /// Gets or sets a value indicating whether time limitation is inactive.
        /// </summary>
        bool IsTimeLimitInactive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether time limitation is orderly.
        /// </summary>
        bool IsTimeLimitOrderly { get; set; }

        /// <summary>
        /// Gets or sets time limitation from time.
        /// </summary>
        TimeSpan TimeLimitFromTime { get; set; }

        /// <summary>
        /// Gets or sets time limitation to time.
        /// </summary>
        TimeSpan TimeLimitToTime { get; set; }

        /// <summary>
        /// Gets or sets time limitation occasional minutes.
        /// </summary>
        int TimeLimitOccasionalMinutes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether program limitation is orderly.
        /// </summary>
        bool IsProgramLimitOrderly { get; set; }

        /// <summary>
        /// Gets or sets program limitation from time.
        /// </summary>
        TimeSpan ProgramLimitFromTime { get; set; }

        /// <summary>
        /// Gets or sets program limitation to time.
        /// </summary>
        TimeSpan ProgramLimitToTime { get; set; }

        /// <summary>
        /// Gets or sets program limitation occasional minutes.
        /// </summary>
        int ProgramLimitOccasionalMinutes { get; set; }
    }
}
