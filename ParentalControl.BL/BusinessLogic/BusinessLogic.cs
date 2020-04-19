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

    /// <summary>
    /// BusinessLogic class.
    /// </summary>
    public class BusinessLogic
    {
        private static BusinessLogic businessLogic;
        private ProxyController proxyController;
        private ProcessController processController;

        private BusinessLogic()
        {
            this.Database = DatabaseManager.Get();
            this.proxyController = new ProxyController();
            this.processController = new ProcessController();
            this.processController.AllProcessStop();
        }

        /// <summary>
        /// User logged in with orderly active time interval event.
        /// </summary>
        public event UserEventHandler UserLoggedInWithOrderlyActiveTimeInterval;

        /// <summary>
        /// User logged in with orderly inactive time interval event.
        /// </summary>
        public event UserEventHandler UserLoggedInWithOrderlyInactiveTimeInterval;

        /// <summary>
        /// User logged in with occassional permission event.
        /// </summary>
        public event UserEventHandler UserLoggedInWithOccassionalPermission;

        /// <summary>
        /// User logged in without permission event.
        /// </summary>
        public event UserEventHandler UserLoggedInWithoutPermission;

        /// <summary>
        /// Logged out event.
        /// </summary>
        public event UserEventHandler LoggedOut;

        /// <summary>
        /// Gets database.
        /// </summary>
        public DatabaseManager Database { get; private set; }

        /// <summary>
        /// Gets activeUser.
        /// </summary>
        public User ActiveUser { get; private set; }

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

        /// <summary>
        /// Login.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <returns>ActiveUser.</returns>
        public User LogIn(string username, string password)
        {
            this.CheckInput(username, password);
            this.ActiveUser = this.Database.ReadUsers(x => x.Username == username && this.ValidateHash(password, x.Password)).FirstOrDefault();
            if (this.ActiveUser != null)
            {
                if (this.ActiveUser.ID != 0)
                {
                    Func<ProgramLimitation, bool> programLimitationCondition = x => x.UserID == this.ActiveUser.ID;
                    var programLimitations = this.Database.ReadProgramLimitations(programLimitationCondition);
                    if (programLimitations.Any())
                    {
                        this.processController.ProgramStop(programLimitations);
                    }

                    var keywords = new List<string>();
                    Func<WebLimitation, bool> webLimitationCondition = x => x.UserID == this.ActiveUser.ID;
                    var webLimitations = this.Database.ReadWebLimitations(webLimitationCondition);
                    foreach (var webLimitation in webLimitations)
                    {
                        Func<Keyword, bool> keywordCondition = x => x.ID == webLimitation.KeywordID;
                        keywords.Add(this.Database.ReadKeywords(keywordCondition).FirstOrDefault()?.Name);
                    }

                    if (keywords.Count != 0)
                    {
                        this.proxyController.Start(keywords);
                    }

                    if (this.ActiveUser.IsTimeLimitationActive)
                    {
                        bool isOrderlyActive = this.ActiveUser.Orderly && IsOrderlyActive(this.ActiveUser.FromTime, this.ActiveUser.ToTime);
                        if (isOrderlyActive)
                        {
                            this.processController.AllProcessStart();
                            this.UserLoggedInWithOrderlyActiveTimeInterval?.Invoke(this, null);
                        }
                        else if (this.ActiveUser.Occasional)
                        {
                            this.UserLoggedInWithOccassionalPermission?.Invoke(this, null);
                        }
                        else if (this.ActiveUser.Orderly && !this.ActiveUser.Occasional)
                        {
                            this.UserLoggedInWithOrderlyInactiveTimeInterval?.Invoke(this, new UserEventArgs() { FromTime = this.ActiveUser.FromTime });
                            this.LogOut();
                        }
                        else
                        {
                            this.UserLoggedInWithoutPermission?.Invoke(this, null);
                            this.LogOut();
                        }
                    }
                    else
                    {
                        this.processController.AllProcessStart();
                    }
                }
            }

            return this.ActiveUser;
        }

        /// <summary>
        /// Logout.
        /// </summary>
        public void LogOut()
        {
            this.ActiveUser = default;
            this.LoggedOut?.Invoke(this, null);
        }

        /// <summary>
        /// Registration.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <param name="securityQuestion">Security question.</param>
        /// <param name="securityAnswer">Security answer.</param>
        /// <returns>Success.</returns>
        public bool Registration(string username, string password, string securityQuestion, string securityAnswer)
        {
            this.CheckInput(username, password, securityQuestion, securityAnswer);
            return this.Database.CreateUser(username, this.GetHash(password), securityQuestion, this.GetHash(securityAnswer));
        }

        /// <summary>
        /// Check valid username.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <returns>Valid.</returns>
        public bool IsValidUsername(string username)
        {
            return this.Database.ReadUsers(x => x.Username == username).Any();
        }

        /// <summary>
        /// Password recovery.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="securityAnswer">Security answer.</param>
        /// <param name="newPassword">New password.</param>
        /// <returns>Success.</returns>
        public bool PasswordRecovery(string username, string securityAnswer, string newPassword)
        {
            this.CheckInput(username, securityAnswer, newPassword);
            Func<User, bool> condition = x => x.Username == username;
            var user = this.Database.ReadUsers(condition).FirstOrDefault();
            if (user == null)
            {
                throw new ArgumentException("Nem létezik ilyen felhasználó!", nameof(username));
            }

            if (this.ValidateHash(securityAnswer, user.SecurityAnswer))
            {
                this.Database.UpdateUsers(x => x.Password = this.GetHash(newPassword), x => x.Username == username);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Content filtering start.
        /// </summary>
        public void ContentFilteringStart()
        {
            this.proxyController.Start();
        }

        /// <summary>
        /// Content filtering stop.
        /// </summary>
        public void ContentFilteringStop()
        {
            this.proxyController.Stop();
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
