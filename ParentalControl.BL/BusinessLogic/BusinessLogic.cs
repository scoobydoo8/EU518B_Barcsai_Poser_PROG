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
        private Logger logger;

        private BusinessLogic()
        {
            this.logger = Logger.Get();
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

        /// <inheritdoc/>
        public IProcessController ProcessController { get => this.processController; }

        /// <summary>
        /// Gets active user.
        /// </summary>
        internal User ActiveUser { get; private set; }

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
            this.ActiveUser = this.database.ReadUsers(x => x.Username == username && this.ValidateHash(password, x.Password)).FirstOrDefault();
            if (this.ActiveUser != null)
            {
                if (this.ActiveUser.ID != 0)
                {
                    if (!this.ActiveUser.IsTimeLimitInactive)
                    {
                        bool isOrderly = this.ActiveUser.IsTimeLimitOrderly && IsOrderlyActive(this.ActiveUser.TimeLimitFromTime, this.ActiveUser.TimeLimitToTime);
                        if (isOrderly)
                        {
                            this.processController.AllProcessLimitStop();
                            this.proxyController.Start();
                            this.UserLoggedInOrderly?.Invoke(this, EventArgs.Empty);
                            this.logger.LogLogin(this.ActiveUser.Username);
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
                        this.logger.LogLogin(this.ActiveUser.Username);
                    }
                }
            }

            return this.ActiveUser;
        }

        /// <inheritdoc/>
        public void LogOut()
        {
            this.ActiveUser = default;
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

        /// <inheritdoc/>
        public bool IsOccassionalPermission(string adminUsername, string adminPassword)
        {
            this.logger.LogLogin(this.ActiveUser.Username);
            var admin = this.database.ReadUsers(x => x.ID == 0).FirstOrDefault() as User;
            if (admin.Username == adminUsername && this.ValidateHash(adminPassword, admin.Password))
            {
                this.processController.AllProcessLimitStop();
                this.proxyController.Start();
                return true;
            }

            this.LogOut();
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
