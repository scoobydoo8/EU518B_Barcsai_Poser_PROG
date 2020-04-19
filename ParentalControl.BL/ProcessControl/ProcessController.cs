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
        private List<RanProcessInfo> ranProcessesWhileTime;
        private ManagementEventWatcher processStartedEventWatcher;
        private ManagementEventWatcher processStoppedEventWatcher;
        private bool eventWatchersStarted;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessController"/> class.
        /// </summary>
        public ProcessController()
        {
            this.ranProcessesWhileTime = new List<RanProcessInfo>();
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
            this.ranProcessesWhileTime.Add(new RanProcessInfo(process));
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
                ranProcess.Process?.Suspend();

                // ranProcess.To = DateTime.Now;

                // TODO Logger
            }

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
            if (this.businessLogic.User == null)
            {
                throw new ArgumentNullException(nameof(this.businessLogic.User));
            }

            if (this.businessLogic.User.ID != 0)
            {
                this.programLimitations = this.businessLogic.Database.ReadProgramLimitations(x => x.UserID == this.businessLogic.User.ID);
                if (!this.programLimitations.Any())
                {
                    this.programLimitations = null;
                }
            }

            foreach (var ranProcess in this.ranProcessesWhileTime)
            {
                ranProcess.Process?.Resume();

                // TODO Logger
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
                process?.Suspend();
                if (this.businessLogic != null && this.businessLogic.User != null)
                {
                    if (this.businessLogic.User.ID != 0 && this.programLimitations != null)
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

                            bool isOrderly = this.businessLogic.User.IsProgramLimitOrderly && BusinessLogic.IsOrderlyActive(this.businessLogic.User.ProgramLimitFromTime, this.businessLogic.User.ProgramLimitToTime);
                            if (isOrderly)
                            {
                                process?.Resume();
                                this.ranProcessesWhileTime.Add(new RanProcessInfo(process));
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
                            this.ranProcessesWhileTime.Add(new RanProcessInfo(process));
                        }
                    }
                    else
                    {
                        process?.Resume();
                        this.ranProcessesWhileTime.Add(new RanProcessInfo(process));
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
            var process = this.ranProcessesWhileTime.Where(x => x.Process.Id == id).FirstOrDefault();
            if (process != null)
            {
                process.To = DateTime.Now;

                // TODO Logger
            }

            this.ranProcessesWhileTime.Remove(process);
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
