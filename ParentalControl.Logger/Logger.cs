// <copyright file="Logger.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParentalControl
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ParentalControl.Interface;

    /// <summary>
    /// Logger class.
    /// </summary>
    public class Logger
    {
        private static Logger logger;
        private object lockObject = new object();

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
        /// <param name="e">Exception.</param>
        public void LogException(Exception e)
        {
            var exceptionMessage = e.GetType().Name + ":" + e.Message;
            this.GetInnerExceptionMassage(e.InnerException, ref exceptionMessage);
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
            var now = FreshDateTime.Now;
            string message = string.Format("{0};{1};{2}", now.ToString("HH:mm:ss"), (int)logType, param).Replace(Environment.NewLine, "\t");
            string directory = "Log\\";
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string fileName = string.Format("{0}{1}{2}.log", directory, now.ToString("yyyy-MM-dd"), logType == LogType.Exception ? "_exceptions" : string.Empty);
            Task.Run(() =>
            {
                lock (this.lockObject)
                {
                    using (var sw = new StreamWriter(fileName, true))
                    {
                        sw.WriteLine(message);
                    }
                }
            });
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            this.LogException(e.ExceptionObject as Exception);
        }

        private void GetInnerExceptionMassage(Exception e, ref string exceptionMessage)
        {
            if (e != null)
            {
                exceptionMessage += "::" + e.GetType().Name + ":" + e.Message;
                this.GetInnerExceptionMassage(e.InnerException, ref exceptionMessage);
            }
        }
    }
}