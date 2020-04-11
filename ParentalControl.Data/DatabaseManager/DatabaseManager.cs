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

    public class DatabaseManager
    {
        private static DatabaseManager databaseManager = null;
        private ParentalControlEntities entities;
        private DbContextTransaction transaction;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseManager"/> class.
        /// </summary>
        private DatabaseManager()
        {
            this.entities = new ParentalControlEntities();
            this.transaction = null;
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
        /// Prepare transaction.
        /// </summary>
        public void BeginTransaction()
        {
            this.transaction = this.entities.Database.BeginTransaction();
        }

        /// <summary>
        /// Commit changes.
        /// </summary>
        public void Commit()
        {
            this.transaction.Commit();
            this.transaction.Dispose();
            this.transaction = null;
        }

        /// <summary>
        /// Rollback changes.
        /// </summary>
        public void Rollback()
        {
            this.transaction.Rollback();
            this.transaction.Dispose();
            this.transaction = null;
        }

        public void Transaction(Action action)
        {
            try
            {
                this.BeginTransaction();
                action();
                this.entities.SaveChanges();
                this.Commit();
            }
            catch (Exception)
            {
                this.Rollback();
            }
        }

        public void CreateUser(string username, string password, string securityQuestion, string securityAnswer)
        {
            this.entities.Users.Create(new User(username, password, securityQuestion, securityAnswer));
        }

        public void CreateProgramSetting(int userID, string name, string path, bool occasional = default, int minutes = default, bool repeat = default, int pause = default, int quantity = default, bool orderly = default, TimeSpan fromTime = default, TimeSpan toTime = default)
        {
            this.entities.ProgramSettings.Create(new ProgramSetting(userID, name, path, occasional, minutes, repeat, pause, quantity, orderly, fromTime, toTime));
        }

        public void CreateTimeSetting(int userID, bool occasional = default, int minutes = default, bool orderly = default, TimeSpan fromTime = default, TimeSpan toTime = default)
        {
            this.entities.TimeSettings.Create(new TimeSetting(userID, occasional, minutes, orderly, fromTime, toTime));
        }

        public void CreateWebSetting(int userID, int keywordID)
        {
            this.entities.WebSettings.Create(new WebSetting(userID, keywordID));
        }

        public void CreateKeyword(string name)
        {
            this.entities.Keywords.Create(new Keyword(name));
        }

        public List<User> ReadUsers(Func<User, bool> condition = null)
        {
            return this.entities.Users.Read(condition);
        }

        public List<ProgramSetting> ReadProgramSettings(Func<ProgramSetting, bool> condition = null)
        {
            return this.entities.ProgramSettings.Read(condition);
        }

        public List<TimeSetting> ReadTimeSettings(Func<TimeSetting, bool> condition = null)
        {
            return this.entities.TimeSettings.Read(condition);
        }

        public List<WebSetting> ReadWebSettings(Func<WebSetting, bool> condition = null)
        {
            return this.entities.WebSettings.Read(condition);
        }

        public List<Keyword> ReadKeywords(Func<Keyword, bool> condition = null)
        {
            return this.entities.Keywords.Read(condition);
        }

        public void UpdateUsers(Action<User> action, Func<User, bool> condition = null)
        {
            this.entities.Users.Update(action, condition);
        }

        public void UpdateProgramSettings(Action<ProgramSetting> action, Func<ProgramSetting, bool> condition = null)
        {
            this.entities.ProgramSettings.Update(action, condition);
        }

        public void UpdateTimeSettings(Action<TimeSetting> action, Func<TimeSetting, bool> condition = null)
        {
            this.entities.TimeSettings.Update(action, condition);
        }

        public void UpdateWebSettings(Action<WebSetting> action, Func<WebSetting, bool> condition = null)
        {
            this.entities.WebSettings.Update(action, condition);
        }

        public void UpdateKeywords(Action<Keyword> action, Func<Keyword, bool> condition = null)
        {
            this.entities.Keywords.Update(action, condition);
        }

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

        public void DeleteProgramSettings(Func<ProgramSetting, bool> condition = null)
        {
            this.entities.ProgramSettings.Delete(condition);
        }

        public void DeleteTimeSettings(Func<TimeSetting, bool> condition = null)
        {
            this.entities.TimeSettings.Delete(condition);
        }

        public void DeleteWebSettings(Func<WebSetting, bool> condition = null)
        {
            this.entities.WebSettings.Delete(condition);
        }

        public void DeleteKeywords(Func<Keyword, bool> condition = null)
        {
            var keywords = this.ReadKeywords(condition);
            foreach (var keyword in keywords)
            {
                this.DeleteWebSettings(x => x.KeywordID == keyword.ID);
            }

            this.entities.Keywords.Delete(condition);
        }
    }
}
