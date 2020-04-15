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
        /// <returns>Success.</returns>
        public bool CreateUser(string username, string password, string securityQuestion, string securityAnswer)
        {
            if (!this.ReadUsers(x => x.Username == username).Any())
            {
                this.entities.Users.Create(new User(username, password, securityQuestion, securityAnswer));
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public bool CreateProgramLimitation(int userID, string name, string path, bool occasional = default, int minutes = default, bool repeat = default, int pause = default, int quantity = default, bool orderly = default, TimeSpan fromTime = default, TimeSpan toTime = default)
        {
            var user = this.ReadUsers(x => x.ID == userID).FirstOrDefault();
            if (user != null && !this.ReadProgramLimitations(x => x.Path == path).Any())
            {
                if (user.Orderly && orderly)
                {
                    if (user.FromTime > fromTime)
                    {
                        fromTime = user.FromTime;
                    }

                    if (user.ToTime < toTime)
                    {
                        toTime = user.ToTime;
                    }
                }

                this.entities.ProgramLimitations.Create(new ProgramLimitation(userID, name, path, occasional, minutes, repeat, pause, quantity, orderly, fromTime, toTime));
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public void CreateWebLimitation(int userID, int keywordID)
        {
            if (this.ReadUsers(x => x.ID == userID).Any() && this.ReadKeywords(x => x.ID == keywordID).Any())
            {
                this.entities.WebLimitations.Create(new WebLimitation(userID, keywordID));
            }
        }

        /// <inheritdoc/>
        public bool CreateKeyword(string name)
        {
            if (!this.ReadKeywords(x => x.Name == name).Any())
            {
                this.entities.Keywords.Create(new Keyword(name));
                return true;
            }

            return false;
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

        /// <inheritdoc/>
        public List<IUser> ReadUsers(Func<IUser, bool> condition = null)
        {
            return this.ReadUsers(condition == null ? null : (Func<User, bool>)condition).ToList<IUser>();
        }

        /// <summary>
        /// Read program limitations.
        /// </summary>
        /// <param name="condition">Condition.</param>
        /// <returns>List.</returns>
        public List<ProgramLimitation> ReadProgramLimitations(Func<ProgramLimitation, bool> condition = null)
        {
            return this.entities.ProgramLimitations.Read(condition);
        }

        /// <inheritdoc/>
        public List<IProgramLimitation> ReadProgramLimitations(Func<IProgramLimitation, bool> condition = null)
        {
            return this.ReadProgramLimitations(condition == null ? null : (Func<ProgramLimitation, bool>)condition).ToList<IProgramLimitation>();
        }

        /// <summary>
        /// Read web limitations.
        /// </summary>
        /// <param name="condition">Condition.</param>
        /// <returns>List.</returns>
        public List<WebLimitation> ReadWebLimitations(Func<WebLimitation, bool> condition = null)
        {
            return this.entities.WebLimitations.Read(condition);
        }

        /// <inheritdoc/>
        public List<IWebLimitation> ReadWebLimitations(Func<IWebLimitation, bool> condition = null)
        {
            return this.ReadWebLimitations(condition == null ? null : (Func<WebLimitation, bool>)condition).ToList<IWebLimitation>();
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

        /// <inheritdoc/>
        public List<IKeyword> ReadKeywords(Func<IKeyword, bool> condition = null)
        {
            return this.ReadKeywords(condition == null ? null : (Func<Keyword, bool>)condition).ToList<IKeyword>();
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

        /// <inheritdoc/>
        public void UpdateUsers(Action<IUser> action, Func<IUser, bool> condition = null)
        {
            this.UpdateUsers(action == null ? null : (Action<User>)action, condition == null ? null : (Func<User, bool>)condition);
        }

        /// <summary>
        /// Update program limitations.
        /// </summary>
        /// <param name="action">Action.</param>
        /// <param name="condition">Condition.</param>
        public void UpdateProgramLimitations(Action<ProgramLimitation> action, Func<ProgramLimitation, bool> condition = null)
        {
            this.entities.ProgramLimitations.Update(action, condition);
        }

        /// <inheritdoc/>
        public void UpdateProgramLimitations(Action<IProgramLimitation> action, Func<IProgramLimitation, bool> condition = null)
        {
            this.UpdateProgramLimitations(action == null ? null : (Action<ProgramLimitation>)action, condition == null ? null : (Func<ProgramLimitation, bool>)condition);
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

        /// <inheritdoc/>
        public void UpdateKeywords(Action<IKeyword> action, Func<IKeyword, bool> condition = null)
        {
            this.UpdateKeywords(action == null ? null : (Action<Keyword>)action, condition == null ? null : (Func<Keyword, bool>)condition);
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
                this.DeleteProgramLimitations((ProgramLimitation x) => x.UserID == user.ID);
                this.DeleteWebLimitations((WebLimitation x) => x.UserID == user.ID);
            }

            this.entities.Users.Delete(condition);
        }

        /// <inheritdoc/>
        public void DeleteUsers(Func<IUser, bool> condition = null)
        {
            this.DeleteUsers(condition == null ? null : (Func<User, bool>)condition);
        }

        /// <summary>
        /// Delete program limitations.
        /// </summary>
        /// <param name="condition">Condition.</param>
        public void DeleteProgramLimitations(Func<ProgramLimitation, bool> condition = null)
        {
            this.entities.ProgramLimitations.Delete(condition);
        }

        /// <inheritdoc/>
        public void DeleteProgramLimitations(Func<IProgramLimitation, bool> condition = null)
        {
            this.DeleteProgramLimitations(condition == null ? null : (Func<ProgramLimitation, bool>)condition);
        }

        /// <summary>
        /// Delete web limitations.
        /// </summary>
        /// <param name="condition">Condition.</param>
        public void DeleteWebLimitations(Func<WebLimitation, bool> condition = null)
        {
            this.entities.WebLimitations.Delete(condition);
        }

        /// <inheritdoc/>
        public void DeleteWebLimitations(Func<IWebLimitation, bool> condition = null)
        {
            this.DeleteWebLimitations(condition == null ? null : (Func<WebLimitation, bool>)condition);
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
                this.DeleteWebLimitations(x => x.KeywordID == keyword.ID);
            }

            this.entities.Keywords.Delete(condition);
        }

        /// <inheritdoc/>
        public void DeleteKeywords(Func<IKeyword, bool> condition = null)
        {
            this.DeleteKeywords(condition == null ? null : (Func<Keyword, bool>)condition);
        }
    }
}