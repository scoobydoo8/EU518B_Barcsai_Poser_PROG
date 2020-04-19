// <copyright file="ProcessController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParentalControl.BL.ProcessControl
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Management;
    using System.Text;
    using System.Threading.Tasks;
    using ParentalControl.Data.Database;
    using ParentalControl.Interface.Database;
    using ParentalControl.Interface.ProcessControl;

    /// <summary>
    /// Process controller class.
    /// </summary>
    public class ProcessController : IProcessController
    {
        private BusinessLogic businessLogic;
        private List<IProgramLimitation> programLimitations;
        private List<Process> ranProcessesWhileTime;
        private ManagementEventWatcher processStartedEventWatcher;
        private ManagementEventWatcher processStoppedEventWatcher;
        private bool eventWatchersStarted;
        private Logger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessController"/> class.
        /// </summary>
        public ProcessController()
        {
            this.logger = Logger.Get();
            this.ranProcessesWhileTime = new List<Process>();
            this.processStartedEventWatcher = new ManagementEventWatcher("SELECT * FROM Win32_ProcessStartTrace");
            this.processStartedEventWatcher.EventArrived += this.ProcessStartedEventWatcher_EventArrived;
            this.processStoppedEventWatcher = new ManagementEventWatcher("SELECT * FROM Win32_ProcessStopTrace");
            this.processStoppedEventWatcher.EventArrived += this.ProcessStoppedEventWatcher_EventArrived;
        }

        /// <inheritdoc/>
        public event EventHandler<IProcessEventArgs> ProgramStartedOrderly;

        /// <inheritdoc/>
        public event EventHandler<IProcessEventArgs> ProgramStartedOccassional;

        /// <inheritdoc/>
        public event EventHandler<IProcessEventArgs> ProgramStartedFullLimit;

        /// <inheritdoc/>
        public void ProgramResume(int processID)
        {
            var process = Process.GetProcessById(processID);
            process?.Resume();
            this.ranProcessesWhileTime.Add(process);
        }

        /// <inheritdoc/>
        public void ProgramKill(int processID)
        {
            var process = Process.GetProcessById(processID);
            process?.Kill();
        }

        /// <summary>
        /// All process limitation start.
        /// </summary>
        public void AllProcessLimitStart()
        {
            foreach (var ranProcess in this.ranProcessesWhileTime)
            {
                ranProcess?.Kill();
            }

            this.ranProcessesWhileTime = new List<Process>();

            /*var explorerProcesses = Process.GetProcessesByName("explorer");
            foreach (var explorer in explorerProcesses)
            {
                explorer?.Resume();
            }*/

            var toBeKilledProcessesList = this.GetAllToBeKilledProcesses("taskmgr", "cmd", "powershell");
            foreach (var toBeKilledProcesses in toBeKilledProcessesList)
            {
                foreach (var toBeKilledProcess in toBeKilledProcesses)
                {
                    toBeKilledProcess?.Kill();
                }
            }

            if (!this.eventWatchersStarted)
            {
                this.processStartedEventWatcher.Start();
                this.processStoppedEventWatcher.Start();
                this.eventWatchersStarted = true;
            }
        }

        /// <summary>
        /// All process limitation stop.
        /// </summary>
        public void AllProcessLimitStop()
        {
            this.businessLogic = BusinessLogic.Get();
            if (this.businessLogic.ActiveUser == null)
            {
                throw new ArgumentNullException(nameof(this.businessLogic.ActiveUser));
            }

            if (this.businessLogic.ActiveUser.ID != 0)
            {
                this.programLimitations = this.businessLogic.Database.ReadProgramLimitations(x => x.UserID == this.businessLogic.ActiveUser.ID);
                if (!this.programLimitations.Any())
                {
                    this.programLimitations = null;
                }
            }

            /*var explorerProcesses = Process.GetProcessesByName("explorer");
            foreach (var explorer in explorerProcesses)
            {
                explorer?.Resume();
            }*/
        }

        private void ProcessStartedEventWatcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            var id = int.Parse(e.NewEvent.Properties["ProcessID"].Value.ToString());
            try
            {
                var process = Process.GetProcessById(id);
                var fileName = process.MainModule.FileName;

                this.logger.LogStartProcess(this.businessLogic.ActiveUser.Username, process.ProcessName);
                process?.Suspend();
                if (this.businessLogic != null && this.businessLogic.ActiveUser != null)
                {
                    if (this.businessLogic.ActiveUser.ID != 0 && this.programLimitations != null)
                    {
                        if (process.ProcessName.ToLower() == "cmd" || process.ProcessName.ToLower() == "powershell" || process.ProcessName.ToLower() == "taskmgr")
                        {
                            process?.Kill();
                            return;
                        }

                        var program = this.programLimitations.Where(x => x.Path == fileName).FirstOrDefault();
                        if (program != null)
                        {
                            if (program.IsFullLimit)
                            {
                                process?.Kill();
                                this.ProgramStartedFullLimit?.Invoke(this, new ProcessEventArgs() { ProcessName = process.ProcessName });
                                return;
                            }

                            bool isOrderly = this.businessLogic.ActiveUser.IsProgramLimitOrderly && BusinessLogic.IsOrderlyActive(this.businessLogic.ActiveUser.ProgramLimitFromTime, this.businessLogic.ActiveUser.ProgramLimitToTime);
                            if (isOrderly)
                            {
                                process?.Resume();
                                this.ranProcessesWhileTime.Add(process);
                                this.ProgramStartedOrderly?.Invoke(this, new ProcessEventArgs() { ID = process.Id, ProcessName = process.ProcessName });
                            }
                            else
                            {
                                this.ProgramStartedOccassional?.Invoke(this, new ProcessEventArgs() { ID = process.Id, ProcessName = process.ProcessName });
                            }
                        }
                        else
                        {
                            process?.Resume();
                            this.ranProcessesWhileTime.Add(process);
                        }
                    }
                    else
                    {
                        process?.Resume();
                        this.ranProcessesWhileTime.Add(process);
                    }
                }
                else
                {
                    process?.Kill();
                }
            }
            catch (ArgumentException)
            {
            }
        }

        private void ProcessStoppedEventWatcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            var id = int.Parse(e.NewEvent.Properties["ProcessID"].Value.ToString());
            try
            {
                this.logger.LogStopProcess(this.businessLogic.ActiveUser.Username, Process.GetProcessById(id).ProcessName);
            }
            catch (Exception)
            {
            }

            this.ranProcessesWhileTime.Remove(this.ranProcessesWhileTime.Where(x => x.Id == id).FirstOrDefault());
        }

        private List<Process[]> GetAllToBeKilledProcesses(params string[] processNames)
        {
            List<Process[]> processes = new List<Process[]>();
            foreach (var processName in processNames)
            {
                processes.Add(Process.GetProcessesByName(processName));
            }

            return processes;
        }
    }
}
