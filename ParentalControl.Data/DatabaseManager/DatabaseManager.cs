// <copyright file="DatabaseManager.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParentalControl.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ParentalControl.Data.Database;
    using ParentalControl.Interface.Database;
    using ParentalControl.Interface.DatabaseManager;

    /// <summary>
    /// DatabaseManager class.
    /// </summary>
    public class DatabaseManager : IDatabaseManager
    {
        private static DatabaseManager databaseManager = null;
        private ParentalControlEntities entities;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseManager"/> class.
        /// </summary>
        private DatabaseManager()
        {
            this.entities = new ParentalControlEntities();
        }

        /// <summary>
        /// Singleton.
        /// </summary>
        /// <returns>DatabaseManager.</returns>
        public static DatabaseManager Get()
        {
            if (databaseManager == null)
            {
                databaseManager = new DatabaseManager();
            }

            return databaseManager;
        }

        /// <summary>
        /// This must be used for create, update and delete transacions.
        /// </summary>
        /// <param name="action">Transaction action.</param>
        public void Transaction(Action action)
        {
            using (var transaction = this.entities.Database.BeginTransaction())
            {
                try
                {
                    action();
                    this.entities.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }
        }

        /// <summary>
        /// Create user.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <param name="securityQuestion">Security question.</param>
        /// <param name="securityAnswer">Security answer.</param>
        public void CreateUser(string username, string password, string securityQuestion, string securityAnswer)
        {
            this.entities.Users.Create(new User(username, password, securityQuestion, securityAnswer));
        }

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
        public void CreateProgramSetting(int userID, string name, string path, bool occasional = default, int minutes = default, bool repeat = default, int pause = default, int quantity = default, bool orderly = default, TimeSpan fromTime = default, TimeSpan toTime = default)
        {
            this.entities.ProgramSettings.Create(new ProgramSetting(userID, name, path, occasional, minutes, repeat, pause, quantity, orderly, fromTime, toTime));
        }

        /// <summary>
        /// Create time setting.
        /// </summary>
        /// <param name="userID">UserID.</param>
        /// <param name="occasional">Occasional.</param>
        /// <param name="minutes">Minutes.</param>
        /// <param name="orderly">Orderly.</param>
        /// <param name="fromTime">From time.</param>
        /// <param name="toTime">To time.</param>
        public void CreateTimeSetting(int userID, bool occasional = default, int minutes = default, bool orderly = default, TimeSpan fromTime = default, TimeSpan toTime = default)
        {
            this.entities.TimeSettings.Create(new TimeSetting(userID, occasional, minutes, orderly, fromTime, toTime));
        }

        /// <summary>
        /// Create web setting.
        /// </summary>
        /// <param name="userID">UserID.</param>
        /// <param name="keywordID">KeywordID.</param>
        public void CreateWebSetting(int userID, int keywordID)
        {
            this.entities.WebSettings.Create(new WebSetting(userID, keywordID));
        }

        /// <summary>
        /// Create keyword.
        /// </summary>
        /// <param name="name">Name.</param>
        public void CreateKeyword(string name)
        {
            this.entities.Keywords.Create(new Keyword(name));
        }

        /// <summary>
        /// Read users.
        /// </summary>
        /// <param name="condition">Condition.</param>
        /// <returns>List.</returns>
        public List<User> ReadUsers(Func<User, bool> condition = null)
        {
            return this.entities.Users.Read(condition);
        }

        /// <summary>
        /// Read program setting.
        /// </summary>
        /// <param name="condition">Condition.</param>
        /// <returns>List.</returns>
        public List<ProgramSetting> ReadProgramSettings(Func<ProgramSetting, bool> condition = null)
        {
            return this.entities.ProgramSettings.Read(condition);
        }

        /// <summary>
        /// Read time setting.
        /// </summary>
        /// <param name="condition">Condition.</param>
        /// <returns>List.</returns>
        public List<TimeSetting> ReadTimeSettings(Func<TimeSetting, bool> condition = null)
        {
            return this.entities.TimeSettings.Read(condition);
        }

        /// <summary>
        /// Read web setting.
        /// </summary>
        /// <param name="condition">Condition.</param>
        /// <returns>List.</returns>
        public List<WebSetting> ReadWebSettings(Func<WebSetting, bool> condition = null)
        {
            return this.entities.WebSettings.Read(condition);
        }

        /// <summary>
        /// Read keyword.
        /// </summary>
        /// <param name="condition">Condition.</param>
        /// <returns>List.</returns>
        public List<Keyword> ReadKeywords(Func<Keyword, bool> condition = null)
        {
            return this.entities.Keywords.Read(condition);
        }

        /// <summary>
        /// Update users.
        /// </summary>
        /// <param name="action">Action.</param>
        /// <param name="condition">Condition.</param>
        public void UpdateUsers(Action<User> action, Func<User, bool> condition = null)
        {
            this.entities.Users.Update(action, condition);
        }

        /// <summary>
        /// Update program settings.
        /// </summary>
        /// <param name="action">Action.</param>
        /// <param name="condition">Condition.</param>
        public void UpdateProgramSettings(Action<ProgramSetting> action, Func<ProgramSetting, bool> condition = null)
        {
            this.entities.ProgramSettings.Update(action, condition);
        }

        /// <summary>
        /// Update time settings.
        /// </summary>
        /// <param name="action">Action.</param>
        /// <param name="condition">Condition.</param>
        public void UpdateTimeSettings(Action<TimeSetting> action, Func<TimeSetting, bool> condition = null)
        {
            this.entities.TimeSettings.Update(action, condition);
        }

        /// <summary>
        /// Update web settings.
        /// </summary>
        /// <param name="action">Action.</param>
        /// <param name="condition">Condition.</param>
        public void UpdateWebSettings(Action<WebSetting> action, Func<WebSetting, bool> condition = null)
        {
            this.entities.WebSettings.Update(action, condition);
        }

        /// <summary>
        /// Update keywords.
        /// </summary>
        /// <param name="action">Action.</param>
        /// <param name="condition">Condition.</param>
        public void UpdateKeywords(Action<Keyword> action, Func<Keyword, bool> condition = null)
        {
            this.entities.Keywords.Update(action, condition);
        }

        /// <summary>
        /// Delete users.
        /// </summary>
        /// <param name="condition">Condition.</param>
        public void DeleteUsers(Func<User, bool> condition = null)
        {
            var users = this.ReadUsers(condition);
            foreach (var user in users)
            {
                this.DeleteProgramSettings(x => x.UserID == user.ID);
                this.DeleteTimeSettings(x => x.UserID == user.ID);
                this.DeleteWebSettings(x => x.UserID == user.ID);
            }

            this.entities.Users.Delete(condition);
        }

        /// <summary>
        /// Delete program settings.
        /// </summary>
        /// <param name="condition">Condition.</param>
        public void DeleteProgramSettings(Func<ProgramSetting, bool> condition = null)
        {
            this.entities.ProgramSettings.Delete(condition);
        }

        /// <summary>
        /// Delete time settings.
        /// </summary>
        /// <param name="condition">Condition.</param>
        public void DeleteTimeSettings(Func<TimeSetting, bool> condition = null)
        {
            this.entities.TimeSettings.Delete(condition);
        }

        /// <summary>
        /// Delete web settings.
        /// </summary>
        /// <param name="condition">Condition.</param>
        public void DeleteWebSettings(Func<WebSetting, bool> condition = null)
        {
            this.entities.WebSettings.Delete(condition);
        }

        /// <summary>
        /// Delete keywords.
        /// </summary>
        /// <param name="condition">Condition.</param>
        public void DeleteKeywords(Func<Keyword, bool> condition = null)
        {
            var keywords = this.ReadKeywords(condition);
            foreach (var keyword in keywords)
            {
                this.DeleteWebSettings(x => x.KeywordID == keyword.ID);
            }

            this.entities.Keywords.Delete(condition);
        }

        /// <inheritdoc/>
        public List<IUser> ReadUsers(Func<IUser, bool> condition = null)
        {
            Func<User, bool> userCondition = x => condition == null || condition(x);
            return this.ReadUsers(userCondition).ToList<IUser>();
        }

        /// <inheritdoc/>
        public List<IProgramSetting> ReadProgramSettings(Func<IProgramSetting, bool> condition = null)
        {
            Func<ProgramSetting, bool> programSettingCondition = x => condition == null || condition(x);
            return this.ReadProgramSettings(programSettingCondition).ToList<IProgramSetting>();
        }

        /// <inheritdoc/>
        public List<ITimeSetting> ReadTimeSettings(Func<ITimeSetting, bool> condition = null)
        {
            Func<TimeSetting, bool> timeSettingCondition = x => condition == null || condition(x);
            return this.ReadTimeSettings(timeSettingCondition).ToList<ITimeSetting>();
        }

        /// <inheritdoc/>
        public List<IWebSetting> ReadWebSettings(Func<IWebSetting, bool> condition = null)
        {
            Func<WebSetting, bool> webSettingCondition = x => condition == null || condition(x);
            return this.ReadWebSettings(webSettingCondition).ToList<IWebSetting>();
        }

        /// <inheritdoc/>
        public List<IKeyword> ReadKeywords(Func<IKeyword, bool> condition = null)
        {
            Func<Keyword, bool> keywordCondition = x => condition == null || condition(x);
            return this.ReadKeywords(keywordCondition).ToList<IKeyword>();
        }

        /// <inheritdoc/>
        public void UpdateUsers(Action<IUser> action, Func<IUser, bool> condition = null)
        {
            Action<User> userAction = x => action(x);
            Func<User, bool> userCondition = x => condition == null || condition(x);
            this.UpdateUsers(userAction, userCondition);
        }

        /// <inheritdoc/>
        public void UpdateProgramSettings(Action<IProgramSetting> action, Func<IProgramSetting, bool> condition = null)
        {
            Action<ProgramSetting> programSettingAction = x => action(x);
            Func<ProgramSetting, bool> programSettingCondition = x => condition == null || condition(x);
            this.UpdateProgramSettings(programSettingAction, programSettingCondition);
        }

        /// <inheritdoc/>
        public void UpdateTimeSettings(Action<ITimeSetting> action, Func<ITimeSetting, bool> condition = null)
        {
            Action<TimeSetting> timeSettingAction = x => action(x);
            Func<TimeSetting, bool> timeSettingCondition = x => condition == null || condition(x);
            this.UpdateTimeSettings(timeSettingAction, timeSettingCondition);
        }

        /// <inheritdoc/>
        public void UpdateWebSettings(Action<IWebSetting> action, Func<IWebSetting, bool> condition = null)
        {
            Action<WebSetting> webSettingAction = x => action(x);
            Func<WebSetting, bool> webSettingCondition = x => condition == null || condition(x);
            this.UpdateWebSettings(webSettingAction, webSettingCondition);
        }

        /// <inheritdoc/>
        public void UpdateKeywords(Action<IKeyword> action, Func<IKeyword, bool> condition = null)
        {
            Action<Keyword> keywordAction = x => action(x);
            Func<Keyword, bool> keywordCondition = x => condition == null || condition(x);
            this.UpdateKeywords(keywordAction, keywordCondition);
        }

        /// <inheritdoc/>
        public void DeleteUsers(Func<IUser, bool> condition = null)
        {
            Func<User, bool> userCondition = x => condition == null || condition(x);
            this.DeleteUsers(userCondition);
        }

        /// <inheritdoc/>
        public void DeleteProgramSettings(Func<IProgramSetting, bool> condition = null)
        {
            Func<ProgramSetting, bool> programSettingCondition = x => condition == null || condition(x);
            this.DeleteProgramSettings(programSettingCondition);
        }

        /// <inheritdoc/>
        public void DeleteTimeSettings(Func<ITimeSetting, bool> condition = null)
        {
            Func<TimeSetting, bool> timeSettingCondition = x => condition == null || condition(x);
            this.DeleteTimeSettings(timeSettingCondition);
        }

        /// <inheritdoc/>
        public void DeleteWebSettings(Func<IWebSetting, bool> condition = null)
        {
            Func<WebSetting, bool> webSettingCondition = x => condition == null || condition(x);
            this.DeleteWebSettings(webSettingCondition);
        }

        /// <inheritdoc/>
        public void DeleteKeywords(Func<IKeyword, bool> condition = null)
        {
            Func<Keyword, bool> keywordCondition = x => condition == null || condition(x);
            this.DeleteKeywords(keywordCondition);
        }
    }
}