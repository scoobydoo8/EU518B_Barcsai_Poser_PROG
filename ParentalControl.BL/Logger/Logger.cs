// <copyright file="Logger.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParentalControl.BL
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Logger class.
    /// </summary>
    public class Logger
    {
        private static Logger logger;

        private Logger()
        {
            AppDomain.CurrentDomain.UnhandledException += this.CurrentDomain_UnhandledException;
        }

        private enum LogType
        {
            Exception = 0,
            Login = 1,
            Logout = 2,
            StartProcess = 3,
            StopProcess = 4,
            HttpUrl = 5,
        }

        /// <summary>
        /// Singleton.
        /// </summary>
        /// <returns>Logger.</returns>
        public static Logger Get()
        {
            if (logger == null)
            {
                logger = new Logger();
            }

            return logger;
        }

        /// <summary>
        /// Log exception.
        /// </summary>
        /// <param name="exceptionMessage">Exception message.</param>
        public void LogException(string exceptionMessage)
        {
            this.Log(exceptionMessage, LogType.Exception);
        }

        /// <summary>
        /// Log login.
        /// </summary>
        /// <param name="username">Username.</param>
        public void LogLogin(string username)
        {
            this.Log(username, LogType.Login);
        }

        /// <summary>
        /// Log logout.
        /// </summary>
        /// <param name="username">Username.</param>
        public void LogLogout(string username)
        {
            this.Log(username, LogType.Logout);
        }

        /// <summary>
        /// Log start process.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="processName">Process name.</param>
        public void LogStartProcess(string username, string processName)
        {
            this.Log(username + ":" + processName, LogType.StartProcess);
        }

        /// <summary>
        /// Log stop process.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="processName">Process name.</param>
        public void LogStopProcess(string username, string processName)
        {
            this.Log(username + ":" + processName, LogType.StopProcess);
        }

        /// <summary>
        /// Log http url.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="httpUrl">Http url.</param>
        public void LogHttpUrl(string username, string httpUrl)
        {
            this.Log(username + ":" + httpUrl, LogType.HttpUrl);
        }

        private void Log(string param, LogType logType)
        {
            string message = string.Format("{0};{1};{2}", DateTime.Now.ToString("HH:mm:ss"), (int)logType, param).Replace(Environment.NewLine, "\t");
            string directory = "Log\\";
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string fileName = string.Format("{0}{1}{2}.log", directory, DateTime.Now.ToString("yyyy-MM-dd"), logType == LogType.Exception ? "_exceptions" : string.Empty);
            using (var sw = new StreamWriter(fileName, true))
            {
                sw.WriteLine(message);
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exceptionMessage = e.ExceptionObject.GetType().Name + ":" + (e.ExceptionObject as Exception).Message;
            this.LogException(exceptionMessage);
        }
    }
}
