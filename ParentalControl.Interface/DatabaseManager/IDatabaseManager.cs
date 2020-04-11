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
        /// Create user.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <param name="securityQuestion">Security question.</param>
        /// <param name="securityAnswer">Security answer.</param>
        void CreateUser(string username, string password, string securityQuestion, string securityAnswer);

        /// <summary>
        /// Create program setting.
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
        void CreateProgramSetting(int userID, string name, string path, bool occasional = default, int minutes = default, bool repeat = default, int pause = default, int quantity = default, bool orderly = default, TimeSpan fromTime = default, TimeSpan toTime = default);

        /// <summary>
        /// Create time setting.
        /// </summary>
        /// <param name="userID">UserID.</param>
        /// <param name="occasional">Occasional.</param>
        /// <param name="minutes">Minutes.</param>
        /// <param name="orderly">Orderly.</param>
        /// <param name="fromTime">From time.</param>
        /// <param name="toTime">To time.</param>
        void CreateTimeSetting(int userID, bool occasional = default, int minutes = default, bool orderly = default, TimeSpan fromTime = default, TimeSpan toTime = default);

        /// <summary>
        /// Create web setting.
        /// </summary>
        /// <param name="userID">UserID.</param>
        /// <param name="keywordID">KeywordID.</param>
        void CreateWebSetting(int userID, int keywordID);

        /// <summary>
        /// Create keyword.
        /// </summary>
        /// <param name="name">Name.</param>
        void CreateKeyword(string name);

        /// <summary>
        /// Read users.
        /// </summary>
        /// <param name="condition">Condition.</param>
        /// <returns>List.</returns>
        List<IUser> ReadUsers(Func<IUser, bool> condition = null);

        /// <summary>
        /// Read program setting.
        /// </summary>
        /// <param name="condition">Condition.</param>
        /// <returns>List.</returns>
        List<IProgramSetting> ReadProgramSettings(Func<IProgramSetting, bool> condition = null);

        /// <summary>
        /// Read time setting.
        /// </summary>
        /// <param name="condition">Condition.</param>
        /// <returns>List.</returns>
        List<ITimeSetting> ReadTimeSettings(Func<ITimeSetting, bool> condition = null);

        /// <summary>
        /// Read web setting.
        /// </summary>
        /// <param name="condition">Condition.</param>
        /// <returns>List.</returns>
        List<IWebSetting> ReadWebSettings(Func<IWebSetting, bool> condition = null);

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
        /// Update program settings.
        /// </summary>
        /// <param name="action">Action.</param>
        /// <param name="condition">Condition.</param>
        void UpdateProgramSettings(Action<IProgramSetting> action, Func<IProgramSetting, bool> condition = null);

        /// <summary>
        /// Update time settings.
        /// </summary>
        /// <param name="action">Action.</param>
        /// <param name="condition">Condition.</param>
        void UpdateTimeSettings(Action<ITimeSetting> action, Func<ITimeSetting, bool> condition = null);

        /// <summary>
        /// Update web settings.
        /// </summary>
        /// <param name="action">Action.</param>
        /// <param name="condition">Condition.</param>
        void UpdateWebSettings(Action<IWebSetting> action, Func<IWebSetting, bool> condition = null);

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
        /// Delete program settings.
        /// </summary>
        /// <param name="condition">Condition.</param>
        void DeleteProgramSettings(Func<IProgramSetting, bool> condition = null);

        /// <summary>
        /// Delete time settings.
        /// </summary>
        /// <param name="condition">Condition.</param>
        void DeleteTimeSettings(Func<ITimeSetting, bool> condition = null);

        /// <summary>
        /// Delete web settings.
        /// </summary>
        /// <param name="condition">Condition.</param>
        void DeleteWebSettings(Func<IWebSetting, bool> condition = null);

        /// <summary>
        /// Delete keywords.
        /// </summary>
        /// <param name="condition">Condition.</param>
        void DeleteKeywords(Func<IKeyword, bool> condition = null);
    }
}
