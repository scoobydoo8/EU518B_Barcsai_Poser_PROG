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
    using ParentalControl.BL.Process;
    using ParentalControl.BL.Proxy;
    using ParentalControl.Data;
    using ParentalControl.Data.Database;

    /// <summary>
    /// BusinessLogic class.
    /// </summary>
    public class BusinessLogic : INotifyPropertyChanged
    {
        private static BusinessLogic businessLogic;
        private ProxyController proxyController;
        private ProcessController processController;

        private BusinessLogic()
        {
            this.Database = DatabaseManager.Get();
            this.proxyController = new ProxyController();
            this.processController = new ProcessController();
        }

        /// <summary>
        /// PropertyChanged event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

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
        /// Login.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <returns>Success.</returns>
        public bool LogIn(string username, string password)
        {
            this.CheckInput(username, password);
            this.ActiveUser = this.Database.ReadUsers(x => x.Username == username && this.ValidateHash(password, x.Password)).FirstOrDefault();
            if (this.ActiveUser != null)
            {
                if (this.ActiveUser.ID != 0)
                {
                    var programSettings = this.Database.ReadProgramSettings(x => x.UserID == this.ActiveUser.ID);
                    var timeSettings = this.Database.ReadTimeSettings(x => x.UserID == this.ActiveUser.ID);
                    if (programSettings.Any())
                    {

                    }
                }

                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ActiveUser)));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Logout.
        /// </summary>
        public void LogOut()
        {
            this.ActiveUser = default;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ActiveUser)));
        }

        /// <summary>
        /// Registration.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <param name="securityQuestion">Security question.</param>
        /// <param name="securityAnswer">Security answer.</param>
        public void Registration(string username, string password, string securityQuestion, string securityAnswer)
        {
            this.CheckInput(username, password, securityQuestion, securityAnswer);
            this.Database.CreateUser(username, this.GetHash(password), securityQuestion, this.GetHash(securityAnswer));

            // TODO Event!
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

        /// <summary>
        /// Program limitation start.
        /// </summary>
        /*public void ProgramLimitationStart()
        {
            this.processController.ProgramLimitationStart();
        }*/

        /// <summary>
        /// Program limitation stop.
        /// </summary>
        /*public void ProgramLimitationStop()
        {
            this.processController.ProgramLimitationStop();
        }*/

        /// <summary>
        /// Time limitation start.
        /// </summary>
        /*public void TimeLimitationStart()
        {
            this.processController.TimeLimitationStart();
        }*/

        /// <summary>
        /// Time limitation stop.
        /// </summary>
        /*public void TimeLimitationStop()
        {
            this.processController.TimeLimitationStop();
        }*/

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
