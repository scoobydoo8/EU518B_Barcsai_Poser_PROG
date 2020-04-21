// <copyright file="ParentalControlService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParentalControl.Service
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;
    using System.ServiceProcess;
    using System.Text;
    using System.Threading.Tasks;
    using ParentalControl.View;
    using ParentalControl.View.Login;

    /// <summary>
    /// Parental control service class.
    /// </summary>
    public partial class ParentalControlService : ServiceBase
    {
        private EventLog eventLog;
        private Process process;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParentalControlService"/> class.
        /// </summary>
        public ParentalControlService()
        {
            this.InitializeComponent();
            this.CanStop = false;
            this.eventLog = new EventLog() { Log = "Application", Source = "ParentalControlService" };
        }

        /// <summary>
        /// On start.
        /// </summary>
        /// <param name="args">Args.</param>
        protected override void OnStart(string[] args)
        {
            this.StartProcess();
        }

        /// <summary>
        /// On stop.
        /// </summary>
        protected override void OnStop()
        {
            this.process.Exited -= this.Process_Exited;
            this.process.Kill();
        }

        private void StartProcess()
        {
            var file = typeof(LoginWindow).Assembly.Location;
            ApplicationLoader.PROCESS_INFORMATION procInfo = default;
            ApplicationLoader.StartProcessAndBypassUAC(file, out procInfo);
            this.process = Process.GetProcessById(Convert.ToInt32(procInfo.dwProcessId));
            this.process.EnableRaisingEvents = true;
            this.process.Exited += this.Process_Exited;
            this.eventLog.WriteEntry(string.Format("Process {0} started.", this.process.Id));
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            this.eventLog.WriteEntry(string.Format("Process {0} exited.", this.process.Id));
            this.process.Exited -= this.Process_Exited;
            this.StartProcess();
        }
    }
}
