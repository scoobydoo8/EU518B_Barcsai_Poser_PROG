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
    /// IUser interface.
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
    }
}
