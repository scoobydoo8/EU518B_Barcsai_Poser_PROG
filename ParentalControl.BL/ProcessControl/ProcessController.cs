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

    /// <summary>
    /// Process controller class.
    /// </summary>
    internal class ProcessController
    {
        private BusinessLogic businessLogic;
        private List<ProgramLimitation> programLimitations;
        private bool canRunProcess = false;
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

        /// <summary>
        /// Program started with orderly active time interval event.
        /// </summary>
        public event ProcessEventHandler ProgramStartedWithOrderlyActiveTimeInterval;

        /// <summary>
        /// Program started with orderly inactive time interval event.
        /// </summary>
        public event ProcessEventHandler ProgramStartedWithOrderlyInactiveTimeInterval;

        /// <summary>
        /// Program started with occassional permission event.
        /// </summary>
        public event ProcessEventHandler ProgramStartedWithOccassionalPermission;

        /// <summary>
        /// Program started without permission event.
        /// </summary>
        public event ProcessEventHandler ProgramStartedWithoutPermission;

        /// <summary>
        /// Program stop.
        /// </summary>
        /// <param name="programLimitations">Program limitations.</param>
        public void ProgramStop(List<ProgramLimitation> programLimitations)
        {
            this.businessLogic = BusinessLogic.Get();
            if (this.businessLogic.ActiveUser == null)
            {
                throw new ArgumentNullException(nameof(this.businessLogic.ActiveUser));
            }

            this.programLimitations = programLimitations;
        }

        /// <summary>
        /// Program start.
        /// </summary>
        /// <param name="processID">Process ID.</param>
        public void ProgramStart(int processID)
        {
            var process = Process.GetProcessById(processID);
            process?.Resume();
            this.ranProcessesWhileTime.Add(new RanProcessInfo(process));
        }

        /// <summary>
        /// All process stop.
        /// </summary>
        public void AllProcessStop()
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

            this.canRunProcess = false;

            if (!this.eventWatchersStarted)
            {
                this.processStartedEventWatcher.Start();
                this.processStoppedEventWatcher.Start();
                this.eventWatchersStarted = true;
            }
        }

        /// <summary>
        /// All process start.
        /// </summary>
        public void AllProcessStart()
        {
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

            this.canRunProcess = true;
        }

        private void ProcessStartedEventWatcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            var id = int.Parse(e.NewEvent.Properties["ProcessID"].Value.ToString());
            try
            {
                var process = Process.GetProcessById(id);
                var fileName = process.MainModule.FileName;
                process?.Suspend();
                if (this.canRunProcess)
                {
                    if (this.businessLogic.ActiveUser.ID != 0)
                    {
                        var program = this.programLimitations.Where(x => x.Path == fileName).FirstOrDefault();
                        if (process.ProcessName.ToLower() == "cmd" || process.ProcessName.ToLower() == "powershell" || process.ProcessName.ToLower() == "taskmgr")
                        {
                            process?.Kill();
                        }
                        else if (program != null)
                        {
                            bool isOrderlyActive = program.Orderly && BusinessLogic.IsOrderlyActive(program.FromTime, program.ToTime);
                            if (isOrderlyActive)
                            {
                                process?.Resume();
                                this.ProgramStartedWithOrderlyActiveTimeInterval?.Invoke(this, new ProcessEventArgs() { ID = process.Id, ProcessName = process.ProcessName });
                                this.ranProcessesWhileTime.Add(new RanProcessInfo(process));
                            }
                            else if (program.Occasional)
                            {
                                this.ProgramStartedWithOccassionalPermission?.Invoke(this, new ProcessEventArgs() { ID = process.Id, ProcessName = process.ProcessName });
                            }
                            else if (program.Orderly && !program.Occasional)
                            {
                                this.ProgramStartedWithOrderlyInactiveTimeInterval?.Invoke(this, new ProcessEventArgs() { ProcessName = process.ProcessName, FromTime = program.FromTime });
                                process?.Kill();
                            }
                        }
                        else if (program == null)
                        {
                            process?.Resume();
                        }
                    }
                    else
                    {
                        process?.Resume();
                    }
                }
                else
                {
                    this.ProgramStartedWithoutPermission?.Invoke(this, new ProcessEventArgs() { ProcessName = process.ProcessName });
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
