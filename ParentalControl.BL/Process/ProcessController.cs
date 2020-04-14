// <copyright file="ProcessController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParentalControl.BL.Process
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management;
    using System.Text;
    using System.Threading.Tasks;
    using ParentalControl.Data.Database;

    /// <summary>
    /// Process controller class.
    /// </summary>
    internal class ProcessController
    {
        private BusinessLogic businessLogic;
        private ManagementEventWatcher eventWatcher;
        private List<ProgramSetting> programSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessController"/> class.
        /// </summary>
        public ProcessController()
        {
            this.eventWatcher = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
            this.eventWatcher.EventArrived += this.StartWatch_EventArrived;
        }

        /// <summary>
        /// Program limitation start.
        /// </summary>
        /// <param name="programSettings">Program settings.</param>
        public void ProgramLimitationStart(List<ProgramSetting> programSettings)
        {
            this.businessLogic = BusinessLogic.Get();
            if (this.businessLogic.ActiveUser == null)
            {
                throw new ArgumentNullException(nameof(this.businessLogic.ActiveUser));
            }

            this.programSettings = programSettings;
        }

        /// <summary>
        /// Program limitation stop.
        /// </summary>
        public void ProgramLimitationStop()
        {
        }

        /// <summary>
        /// Time limitation start.
        /// </summary>
        /// <param name="timeSettings">Time settings.</param>
        public void TimeLimitationStart(List<TimeSetting> timeSettings)
        {
            this.businessLogic = BusinessLogic.Get();
            if (this.businessLogic.ActiveUser == null)
            {
                throw new ArgumentNullException(nameof(this.businessLogic.ActiveUser));
            }

        }

        /// <summary>
        /// Time limitation stop.
        /// </summary>
        public void TimeLimitationStop()
        {
        }

        private void StartWatch_EventArrived(object sender, EventArrivedEventArgs e)
        {
            string processName = e.NewEvent.Properties["ProcessName"].Value.ToString();

        }
    }
}
