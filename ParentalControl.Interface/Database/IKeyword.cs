// <copyright file="IKeyword.cs" company="PlaceholderCompany">
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
    /// Keyword interface.
    /// </summary>
    public interface IKeyword
    {
        /// <summary>
        /// Gets iD.
        /// </summary>
        int ID { get; }

        /// <summary>
        /// Gets or sets name.
        /// </summary>
        string Name { get; set; }
    }
}
