// <copyright file="BusinessLogic.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParentalControl.BL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using ParentalControl.BL.ProcessControl;
    using ParentalControl.BL.ProxyControl;
    using ParentalControl.Data;
    using ParentalControl.Data.Database;
    using ParentalControl.Interface.BusinessLogic;
    using ParentalControl.Interface.Database;
    using ParentalControl.Interface.DatabaseManager;
    using ParentalControl.Interface.ProcessControl;

    /// <summary>
    /// Business logic class.
    /// </summary>
    public class BusinessLogic : IBusinessLogic
    {
        private static BusinessLogic businessLogic;
        private DatabaseManager database;
        private ProxyController proxyController;
        private ProcessController processController;

        private BusinessLogic()
        {
            this.database = DatabaseManager.Get();
            this.proxyController = new ProxyController();
            this.processController = new ProcessController();
            this.processController.AllProcessLimitStart();
        }

        /// <inheritdoc/>
        public event EventHandler UserLoggedInOrderly;

        /// <inheritdoc/>
        public event EventHandler UserLoggedInOccassional;

        /// <inheritdoc/>
        public event EventHandler UserLoggedInFull;

        /// <inheritdoc/>
        public event EventHandler UserLoggedOut;

        /// <inheritdoc/>
        public IDatabaseManager Database { get => this.database; }

        /// <summary>
        /// Gets activeUser.
        /// </summary>
        public User User { get; private set; }

        /// <inheritdoc/>
        public IUser LoggedInUser { get => this.User; }

        /// <inheritdoc/>
        public IProcessController ProcessController { get => this.processController; }

        /// <summary>
        /// Singleton.
        /// </summary>
        /// <returns>BusinessLogic.</returns>
        public static BusinessLogic Get()
        {
            if (businessLogic == null)
            {
                businessLogic = new BusinessLogic();
            }

            return businessLogic;
        }

        /// <summary>
        /// Is orderly active.
        /// </summary>
        /// <param name="fromTime">From time.</param>
        /// <param name="toTime">To time.</param>
        /// <returns>Bool.</returns>
        public static bool IsOrderlyActive(TimeSpan fromTime, TimeSpan toTime)
        {
            var now = DateTime.Now;
            TimeSpan nowTimeSpan = new TimeSpan(now.Hour, now.Minute, 0);
            return fromTime <= nowTimeSpan && nowTimeSpan < toTime;
        }

        /// <inheritdoc/>
        public IUser LogIn(string username, string password)
        {
            this.CheckInput(username, password);
            this.User = this.database.ReadUsers(x => x.Username == username && this.ValidateHash(password, x.Password)).FirstOrDefault();
            if (this.User != null)
            {
                if (this.User.ID != 0)
                {
                    if (!this.User.IsTimeLimitInactive)
                    {
                        bool isOrderly = this.User.IsTimeLimitOrderly && IsOrderlyActive(this.User.TimeLimitFromTime, this.User.TimeLimitToTime);
                        if (isOrderly)
                        {
                            this.processController.AllProcessLimitStop();
                            this.proxyController.Start();
                            this.UserLoggedInOrderly?.Invoke(this, EventArgs.Empty);
                        }
                        else
                        {
                            this.UserLoggedInOccassional?.Invoke(this, EventArgs.Empty);
                        }
                    }
                    else
                    {
                        this.processController.AllProcessLimitStop();
                        this.proxyController.Start();
                        this.UserLoggedInFull?.Invoke(this, EventArgs.Empty);
                    }
                }
            }

            return this.LoggedInUser;
        }

        /// <inheritdoc/>
        public void LogOut()
        {
            this.User = default;
            this.processController.AllProcessLimitStart();
            this.UserLoggedOut?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc/>
        public bool Registration(string username, string password, string securityQuestion, string securityAnswer)
        {
            this.CheckInput(username, password, securityQuestion, securityAnswer);
            return this.database.CreateUser(username, this.GetHash(password), securityQuestion, this.GetHash(securityAnswer));
        }

        /// <inheritdoc/>
        public bool IsValidUsername(string username)
        {
            return this.database.ReadUsers(x => x.Username == username).Any();
        }

        /// <inheritdoc/>
        public bool PasswordRecovery(string username, string securityAnswer, string newPassword)
        {
            this.CheckInput(username, securityAnswer, newPassword);
            Func<User, bool> condition = x => x.Username == username;
            var user = this.database.ReadUsers(condition).FirstOrDefault();
            if (user == null)
            {
                throw new ArgumentException("Nem létezik ilyen felhasználó!", nameof(username));
            }

            if (this.ValidateHash(securityAnswer, user.SecurityAnswer))
            {
                this.database.UpdateUsers(x => x.Password = this.GetHash(newPassword), x => x.Username == username);
                return true;
            }

            return false;
        }

        private string GetHash(string rawstring)
        {
            this.CheckInput(rawstring);
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawstring));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        private bool ValidateHash(string rawstring, string hash)
        {
            this.CheckInput(rawstring, hash);
            string hashedString = this.GetHash(rawstring);
            return hashedString == hash;
        }

        private void CheckInput(params string[] inputs)
        {
            foreach (var input in inputs)
            {
                if (input == null)
                {
                    throw new ArgumentNullException();
                }

                if (input == string.Empty)
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
