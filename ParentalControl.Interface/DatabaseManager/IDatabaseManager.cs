// <copyright file="IDatabaseManager.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParentalControl.Interface.DatabaseManager
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ParentalControl.Interface.Database;

    /// <summary>
    /// IDatabaseManager interface.
    /// </summary>
    public interface IDatabaseManager
    {
        /// <summary>
        /// This must be used for create, update and delete transacions.
        /// </summary>
        /// <param name="action">Transaction action.</param>
        void Transaction(Action action);

        /// <summary>
        /// Create program limitation.
        /// </summary>
        /// <param name="userID">UserID.</param>
        /// <param name="name">Name.</param>
        /// <param name="path">Path.</param>
        /// <param name="occasional">Occasional.</param>
        /// <param name="minutes">Minutes.</param>
        /// <param name="repeat">Repeat.</param>
        /// <param name="pause">Puase.</param>
        /// <param name="quantity">Quantity.</param>
        /// <param name="orderly">Orderly.</param>
        /// <param name="fromTime">From time.</param>
        /// <param name="toTime">To time.</param>
        /// <returns>Success.</returns>
        bool CreateProgramLimitation(int userID, string name, string path, bool occasional = default, int minutes = default, bool repeat = default, int pause = default, int quantity = default, bool orderly = default, TimeSpan fromTime = default, TimeSpan toTime = default);

        /// <summary>
        /// Create web limitation.
        /// </summary>
        /// <param name="userID">UserID.</param>
        /// <param name="keywordID">KeywordID.</param>
        void CreateWebLimitation(int userID, int keywordID);

        /// <summary>
        /// Create keyword.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <returns>Success.</returns>
        bool CreateKeyword(string name);

        /// <summary>
        /// Read users.
        /// </summary>
        /// <param name="condition">Condition.</param>
        /// <returns>List.</returns>
        List<IUser> ReadUsers(Func<IUser, bool> condition = null);

        /// <summary>
        /// Read program limitation.
        /// </summary>
        /// <param name="condition">Condition.</param>
        /// <returns>List.</returns>
        List<IProgramLimitation> ReadProgramLimitations(Func<IProgramLimitation, bool> condition = null);

        /// <summary>
        /// Read web limitation.
        /// </summary>
        /// <param name="condition">Condition.</param>
        /// <returns>List.</returns>
        List<IWebLimitation> ReadWebLimitations(Func<IWebLimitation, bool> condition = null);

        /// <summary>
        /// Read keyword.
        /// </summary>
        /// <param name="condition">Condition.</param>
        /// <returns>List.</returns>
        List<IKeyword> ReadKeywords(Func<IKeyword, bool> condition = null);

        /// <summary>
        /// Update users.
        /// </summary>
        /// <param name="action">Action.</param>
        /// <param name="condition">Condition.</param>
        void UpdateUsers(Action<IUser> action, Func<IUser, bool> condition = null);

        /// <summary>
        /// Update program limitation.
        /// </summary>
        /// <param name="action">Action.</param>
        /// <param name="condition">Condition.</param>
        void UpdateProgramLimitations(Action<IProgramLimitation> action, Func<IProgramLimitation, bool> condition = null);

        /// <summary>
        /// Update keywords.
        /// </summary>
        /// <param name="action">Action.</param>
        /// <param name="condition">Condition.</param>
        void UpdateKeywords(Action<IKeyword> action, Func<IKeyword, bool> condition = null);

        /// <summary>
        /// Delete users.
        /// </summary>
        /// <param name="condition">Condition.</param>
        void DeleteUsers(Func<IUser, bool> condition = null);

        /// <summary>
        /// Delete program limitation.
        /// </summary>
        /// <param name="condition">Condition.</param>
        void DeleteProgramLimitations(Func<IProgramLimitation, bool> condition = null);

        /// <summary>
        /// Delete web limitation.
        /// </summary>
        /// <param name="condition">Condition.</param>
        void DeleteWebLimitations(Func<IWebLimitation, bool> condition = null);

        /// <summary>
        /// Delete keywords.
        /// </summary>
        /// <param name="condition">Condition.</param>
        void DeleteKeywords(Func<IKeyword, bool> condition = null);
    }
}
