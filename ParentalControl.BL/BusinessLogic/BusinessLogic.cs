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
    using System.Timers;
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
        /// <summary>
        /// Sub.
        /// </summary>
        internal static readonly TimeSpan Sub = new TimeSpan(0, 0, 1);

        private static BusinessLogic businessLogic;
        private DatabaseManager database;
        private ProxyController proxyController;
        private ProcessController processController;
        private Logger logger;
        private Timer timer;
        private bool loggedInTimer = false;

        private BusinessLogic()
        {
            this.TimeRemainingTime = default;
            this.ProgramRemainingTime = default;
            this.timer = new Timer(1000);
            this.timer.Elapsed += this.Timer_Elapsed;
            this.timer.AutoReset = true;
            this.timer.Start();
            this.logger = Logger.Get();
            this.database = DatabaseManager.Get();
            this.proxyController = new ProxyController();
            this.processController = new ProcessController();
            this.processController.AllProcessLimitStart();
        }

        /// <inheritdoc/>
        public event EventHandler UserLoggedInOrderlyOrActiveOccasional;

        /// <inheritdoc/>
        public event EventHandler UserLoggedInOccassional;

        /// <inheritdoc/>
        public event EventHandler UserLoggedInFull;

        /// <inheritdoc/>
        public event EventHandler UserLoggedOut;

        /// <summary>
        /// Timer tick event.
        /// </summary>
        public event ElapsedEventHandler TimerTick { add => this.timer.Elapsed += value; remove => this.timer.Elapsed -= value; }

        /// <inheritdoc/>
        public IDatabaseManager Database { get => this.database; }

        /// <inheritdoc/>
        public IProcessController ProcessController { get => this.processController; }

        /// <inheritdoc/>
        public TimeSpan TimeRemainingTime { get; internal set; }

        /// <inheritdoc/>
        public TimeSpan ProgramRemainingTime { get; internal set; }

        /// <summary>
        /// Gets active user.
        /// </summary>
        public IUser ActiveUser { get; private set; }

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
        /// Check input.
        /// </summary>
        /// <param name="inputs">Inputs.</param>
        public static void CheckInput(params string[] inputs)
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

        /// <summary>
        /// Get hash.
        /// </summary>
        /// <param name="rawstring">Raw string.</param>
        /// <returns>Hash.</returns>
        public static string GetHash(string rawstring)
        {
            CheckInput(rawstring);
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

        /// <summary>
        /// Validate hash.
        /// </summary>
        /// <param name="rawstring">Raw string.</param>
        /// <param name="hash">Hash.</param>
        /// <returns>Valid.</returns>
        public static bool ValidateHash(string rawstring, string hash)
        {
            CheckInput(rawstring, hash);
            string hashedString = GetHash(rawstring);
            return hashedString == hash;
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
        public bool LogIn(string username, string password)
        {
            CheckInput(username, password);
            this.ActiveUser = this.database.ReadUsers(x => x.Username == username && ValidateHash(password, x.Password)).FirstOrDefault();
            if (this.ActiveUser != null)
            {
                if (this.ActiveUser.ID != this.database.AdminID)
                {
                    if (!this.ActiveUser.IsTimeLimitInactive)
                    {
                        var now = DateTime.Now;
                        TimeSpan time = default;
                        TimeSpan timeOccasional = default;
                        TimeSpan timeOrderly = default;
                        var timeLimitOccasionalDateTime = (this.ActiveUser as User).TimeLimitOccasionalDateTime;
                        bool isOccasionalActive = timeLimitOccasionalDateTime != default && timeLimitOccasionalDateTime > now;
                        bool isOrderly = this.ActiveUser.IsTimeLimitOrderly && IsOrderlyActive(this.ActiveUser.TimeLimitFromTime, this.ActiveUser.TimeLimitToTime);
                        if (isOccasionalActive)
                        {
                            var toTime = new TimeSpan(timeLimitOccasionalDateTime.Hour, timeLimitOccasionalDateTime.Minute, 0);
                            if (this.ActiveUser.IsTimeLimitOrderly &&
                                this.ActiveUser.TimeLimitToTime != default &&
                                this.ActiveUser.TimeLimitFromTime <= toTime)
                            {
                                timeOccasional = this.ActiveUser.TimeLimitToTime - new TimeSpan(now.Hour, now.Minute, 0);
                            }
                            else
                            {
                                timeOccasional = timeLimitOccasionalDateTime - now;
                            }
                        }

                        if (isOrderly)
                        {
                            timeOrderly = this.ActiveUser.TimeLimitToTime - new TimeSpan(now.Hour, now.Minute, 0);
                        }

                        time = timeOccasional > timeOrderly ? timeOccasional : timeOrderly;
                        if (isOccasionalActive || isOrderly)
                        {
                            this.TimeRemainingTime = time;
                            this.processController.AllProcessLimitStop();
                            this.proxyController.Start();
                            this.UserLoggedInOrderlyOrActiveOccasional?.Invoke(this, EventArgs.Empty);
                            this.loggedInTimer = true;
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
                else
                {
                    this.processController.AllProcessLimitStop();
                    this.proxyController.Start();
                    this.UserLoggedInFull?.Invoke(this, EventArgs.Empty);
                    this.logger.LogLogin(this.ActiveUser.Username);
                }

                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public void LogOut()
        {
            this.logger.LogLogout(this.ActiveUser.Username);
            this.loggedInTimer = false;
            this.TimeRemainingTime = default;
            this.ProgramRemainingTime = default;
            this.ActiveUser = default;
            this.processController.AllProcessLimitStart();
            this.proxyController.Stop();
            this.UserLoggedOut?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc/>
        public bool Registration(string username, string password, string securityQuestion, string securityAnswer)
        {
            CheckInput(username, password, securityQuestion, securityAnswer);
            return this.database.Transaction(() => this.database.CreateUser(username, GetHash(password), securityQuestion, GetHash(securityAnswer)));
        }

        /// <inheritdoc/>
        public bool PasswordRecovery(string username, string securityAnswer, string newPassword)
        {
            CheckInput(username, securityAnswer, newPassword);
            Func<User, bool> condition = x => x.Username == username;
            var user = this.database.ReadUsers(condition).FirstOrDefault();
            if (user == null)
            {
                throw new ArgumentException("Nem létezik ilyen felhasználó!", nameof(username));
            }

            if (ValidateHash(securityAnswer, user.SecurityAnswer))
            {
                this.database.Transaction(() => this.database.UpdateUsers(x => x.Password = GetHash(newPassword), condition));
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public void ChangePassword(string password)
        {
            CheckInput(password);
            if (this.ActiveUser == null)
            {
                throw new ArgumentException("Nincs bejelentkezett felhasználó!");
            }

            Func<User, bool> condition = x => x.ID == this.ActiveUser.ID;
            this.database.Transaction(() => this.database.UpdateUsers(x => x.Password = GetHash(password), condition));
        }

        /// <inheritdoc/>
        public bool IsOccassionalPermission(string adminUsername, string adminPassword, int minutes)
        {
            this.logger.LogLogin(this.ActiveUser.Username);
            var admin = this.database.ReadUsers(x => x.ID == this.database.AdminID).FirstOrDefault() as User;
            if (admin.Username == adminUsername && ValidateHash(adminPassword, admin.Password))
            {
                var now = DateTime.Now;
                var toDate = now.AddMinutes(minutes);
                var toTime = new TimeSpan(toDate.Hour, toDate.Minute, 0);
                if (this.ActiveUser.IsTimeLimitOrderly &&
                    this.ActiveUser.TimeLimitToTime != default &&
                    this.ActiveUser.TimeLimitFromTime <= toTime)
                {
                    toTime = this.ActiveUser.TimeLimitToTime;
                }

                this.TimeRemainingTime = toTime - new TimeSpan(now.Hour, now.Minute, 0);
                this.database.Transaction(() => this.database.UpdateUsers(x => x.TimeLimitOccasionalDateTime = now.Add(this.TimeRemainingTime), x => x.ID == this.ActiveUser.ID));
                this.processController.AllProcessLimitStop();
                this.proxyController.Start();
                this.loggedInTimer = true;
                return true;
            }

            return false;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (this.TimeRemainingTime != default && this.TimeRemainingTime.TotalSeconds > 0)
            {
                this.TimeRemainingTime -= Sub;
            }
            else if ((this.TimeRemainingTime == default || (this.TimeRemainingTime != default && this.TimeRemainingTime.TotalSeconds <= 0)) && this.loggedInTimer)
            {
                this.LogOut();
            }
        }
    }
}
