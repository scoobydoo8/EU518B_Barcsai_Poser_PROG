// <copyright file="IBusinessLogic.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParentalControl.Interface.BusinessLogic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ParentalControl.Interface.Database;
    using ParentalControl.Interface.DatabaseManager;
    using ParentalControl.Interface.ProcessControl;

    /// <summary>
    /// Business logic interface.
    /// </summary>
    public interface IBusinessLogic
    {
        /// <summary>
        /// User logged in with orderly permission event.
        /// </summary>
        event EventHandler UserLoggedInOrderly;

        /// <summary>
        /// User logged in with occassional permission event.
        /// </summary>
        event EventHandler UserLoggedInOccassional;

        /// <summary>
        /// User logged in with full permission event.
        /// </summary>
        event EventHandler UserLoggedInFull;

        /// <summary>
        /// User logged out event.
        /// </summary>
        event EventHandler UserLoggedOut;

        /// <summary>
        /// Gets database.
        /// </summary>
        IDatabaseManager Database { get; }

        /// <summary>
        /// Gets process controller.
        /// </summary>
        IProcessController ProcessController { get; }

        /// <summary>
        /// Login.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <returns>ActiveUser.</returns>
        IUser LogIn(string username, string password);

        /// <summary>
        /// Logout.
        /// </summary>
        void LogOut();

        /// <summary>
        /// Registration.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <param name="securityQuestion">Security question.</param>
        /// <param name="securityAnswer">Security answer.</param>
        /// <returns>Success.</returns>
        bool Registration(string username, string password, string securityQuestion, string securityAnswer);

        /// <summary>
        /// Password recovery.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="securityAnswer">Security answer.</param>
        /// <param name="newPassword">New password.</param>
        /// <returns>Success.</returns>
        bool PasswordRecovery(string username, string securityAnswer, string newPassword);

        /// <summary>
        /// Is occassional permission.
        /// </summary>
        /// <param name="adminUsername">Admin username.</param>
        /// <param name="adminPassword">Admin password.</param>
        /// <returns>Valid.</returns>
        bool IsOccassionalPermission(string adminUsername, string adminPassword);
    }
}
