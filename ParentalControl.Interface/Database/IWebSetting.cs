// <copyright file="IWebSetting.cs" company="PlaceholderCompany">
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
    /// IWebSetting interface.
    /// </summary>
    public interface IWebSetting
    {
        /// <summary>
        /// Gets or sets iD.
        /// </summary>
        int ID { get; set; }

        /// <summary>
        /// Gets or sets userID.
        /// </summary>
        int UserID { get; set; }

        /// <summary>
        /// Gets or sets keywordID.
        /// </summary>
        int KeywordID { get; set; }
    }
}
