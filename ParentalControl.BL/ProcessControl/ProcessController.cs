// <copyright file="ProcessController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParentalControl.BL.ProcessControl
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Management;
    using System.Text;
    using System.Threading.Tasks;
    using System.Timers;
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
        private List<Process> enabledProcesses;
        private ManagementEventWatcher processStartedEventWatcher;
        private ManagementEventWatcher processStoppedEventWatcher;
        private bool eventWatchersStarted;
        private Logger logger;
        private Timer timer;
        private bool programStartedTimer = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessController"/> class.
        /// </summary>
        public ProcessController()
        {
            this.timer = new Timer(1000);
            this.timer.Elapsed += this.Timer_Elapsed;
            this.timer.AutoReset = true;
            this.logger = Logger.Get();
            this.ranProcessesWhileTime = new List<Process>();
            this.enabledProcesses = new List<Process>();
            this.processStartedEventWatcher = new ManagementEventWatcher("SELECT * FROM Win32_ProcessStartTrace");
            this.processStartedEventWatcher.EventArrived += this.ProcessStartedEventWatcher_EventArrived;
            this.processStoppedEventWatcher = new ManagementEventWatcher("SELECT * FROM Win32_ProcessStopTrace");
            this.processStoppedEventWatcher.EventArrived += this.ProcessStoppedEventWatcher_EventArrived;
        }

        /// <inheritdoc/>
        public event EventHandler<IProcessEventArgs> ProgramStartedOrderlyOrActiveOccasional;

        /// <inheritdoc/>
        public event EventHandler<IProcessEventArgs> ProgramStartedOccassional;

        /// <inheritdoc/>
        public event EventHandler<IProcessEventArgs> ProgramStartedFullLimit;

        /// <inheritdoc/>
        public bool IsOccassionalPermission(string adminUsername, string adminPassword, int minutes, int processID)
        {
            if (processID == 0 && this.businessLogic.ProgramRemainingTime == default)
            {
                throw new ArgumentOutOfRangeException(nameof(processID));
            }

            Process process = null;
            if (processID != 0)
            {
                process = Process.GetProcessById(processID);
            }

            var admin = this.businessLogic.Database.ReadUsers(x => x.ID == this.businessLogic.Database.AdminID).FirstOrDefault() as User;
            if (admin.Username == adminUsername && BusinessLogic.ValidateHash(adminPassword, admin.Password))
            {
                var now = DateTime.Now;
                var toDate = now.AddMinutes(minutes);
                var toTime = new TimeSpan(toDate.Hour, toDate.Minute, 0);
                if (this.businessLogic.ActiveUser.IsProgramLimitOrderly &&
                    this.businessLogic.ActiveUser.ProgramLimitToTime != default &&
                    this.businessLogic.ActiveUser.ProgramLimitFromTime <= toTime &&
                    this.businessLogic.ActiveUser.ProgramLimitToTime > toTime)
                {
                    toTime = this.businessLogic.ActiveUser.ProgramLimitToTime;
                }

                this.businessLogic.ProgramRemainingTime = toTime - new TimeSpan(now.Hour, now.Minute, 0);
                this.businessLogic.Database.Transaction(() => this.businessLogic.Database.UpdateUsers(x => (x as User).ProgramLimitOccasionalDateTime = now.Add(this.businessLogic.ProgramRemainingTime), x => x.ID == this.businessLogic.ActiveUser.ID));
                process?.Resume();
                this.ranProcessesWhileTime.Add(process);
                this.enabledProcesses.Add(process);
                this.programStartedTimer = true;
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public bool GetOccasionalPermission(int processID)
        {
            try
            {
                if (this.businessLogic.ProgramRemainingTime == default)
                {
                    return false;
                }

                var process = Process.GetProcessById(processID);
                process?.Resume();
                this.ranProcessesWhileTime.Add(process);
                this.enabledProcesses.Add(process);
                this.programStartedTimer = true;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public void KillProcess(int processID)
        {
            if (processID == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(processID));
            }

            var process = Process.GetProcessById(processID);
            try
            {
                process?.Kill();
            }
            catch (Exception e)
            {
                this.logger.LogException(e);
                process?.Resume();
            }
        }

        /// <summary>
        /// All process limitation start.
        /// </summary>
        public void AllProcessLimitStart()
        {
            this.programStartedTimer = false;
            foreach (var ranProcess in this.ranProcessesWhileTime)
            {
                try
                {
                    ranProcess?.Kill();
                }
                catch (Exception e)
                {
                    this.logger.LogException(e);
                    ranProcess?.Resume();
                }
            }

            this.ranProcessesWhileTime = new List<Process>();
            this.enabledProcesses = new List<Process>();

            var toBeKilledProcessesList = this.GetAllToBeKilledProcesses("taskmgr", "cmd", "powershell");
            foreach (var toBeKilledProcesses in toBeKilledProcessesList)
            {
                foreach (var toBeKilledProcess in toBeKilledProcesses)
                {
                    try
                    {
                        toBeKilledProcess?.Kill();
                    }
                    catch (Exception e)
                    {
                        this.logger.LogException(e);
                        toBeKilledProcess?.Resume();
                    }
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

            this.timer.Start();
            if (this.businessLogic.ActiveUser.ID != this.businessLogic.Database.AdminID)
            {
                this.programLimitations = this.businessLogic.Database.ReadProgramLimitations(x => x.UserID == this.businessLogic.ActiveUser.ID);
                if (!this.programLimitations.Any())
                {
                    this.programLimitations = null;
                }
            }
        }

        private void ProcessStartedEventWatcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            var id = int.Parse(e.NewEvent.Properties["ProcessID"].Value.ToString());
            try
            {
                var process = Process.GetProcessById(id);
                var fileName = process.MainModule.FileName;

                process?.Suspend();
                if (this.businessLogic != null && this.businessLogic.ActiveUser != null)
                {
                    this.logger.LogStartProcess(this.businessLogic.ActiveUser.Username, process.ProcessName);
                    if (this.businessLogic.ActiveUser.ID != this.businessLogic.Database.AdminID && this.programLimitations != null)
                    {
                        if (process.ProcessName.ToLower() == "cmd" || process.ProcessName.ToLower() == "powershell" || process.ProcessName.ToLower() == "taskmgr")
                        {
                            try
                            {
                                process?.Kill();
                            }
                            catch (Exception ex)
                            {
                                this.logger.LogException(ex);
                                process?.Resume();
                            }

                            return;
                        }

                        var program = this.programLimitations.Where(x => x?.Path == fileName).FirstOrDefault();
                        if (program != null)
                        {
                            if (program.IsFullLimit)
                            {
                                try
                                {
                                    process?.Kill();
                                }
                                catch (Exception ex)
                                {
                                    this.logger.LogException(ex);
                                    process?.Resume();
                                }

                                this.ProgramStartedFullLimit?.Invoke(this, new ProcessEventArgs() { ProcessName = process.ProcessName });
                                return;
                            }

                            if (this.businessLogic.ProgramRemainingTime != default)
                            {
                                process?.Resume();
                                this.ranProcessesWhileTime.Add(process);
                                this.enabledProcesses.Add(process);
                                this.programStartedTimer = true;
                                return;
                            }

                            var now = DateTime.Now;
                            TimeSpan time = default;
                            TimeSpan timeOccasional = default;
                            TimeSpan timeOrderly = default;
                            var programLimitOccasionalDateTime = (this.businessLogic.ActiveUser as User).ProgramLimitOccasionalDateTime;
                            bool isOccasionalActive = programLimitOccasionalDateTime != default && programLimitOccasionalDateTime > now;
                            bool isOrderly = this.businessLogic.ActiveUser.IsProgramLimitOrderly && BusinessLogic.IsOrderlyActive(this.businessLogic.ActiveUser.ProgramLimitFromTime, this.businessLogic.ActiveUser.ProgramLimitToTime);
                            if (isOccasionalActive)
                            {
                                var toTime = new TimeSpan(programLimitOccasionalDateTime.Hour, programLimitOccasionalDateTime.Minute, 0);
                                if (this.businessLogic.ActiveUser.IsProgramLimitOrderly &&
                                    this.businessLogic.ActiveUser.ProgramLimitToTime != default &&
                                    this.businessLogic.ActiveUser.ProgramLimitFromTime <= toTime &&
                                    this.businessLogic.ActiveUser.ProgramLimitToTime > toTime)
                                {
                                    timeOccasional = this.businessLogic.ActiveUser.ProgramLimitToTime - new TimeSpan(now.Hour, now.Minute, 0);
                                }
                                else
                                {
                                    timeOccasional = programLimitOccasionalDateTime - now;
                                }
                            }

                            if (isOrderly)
                            {
                                timeOrderly = this.businessLogic.ActiveUser.ProgramLimitToTime - new TimeSpan(now.Hour, now.Minute, 0);
                            }

                            time = timeOccasional > timeOrderly ? timeOccasional : timeOrderly;
                            if (isOccasionalActive || isOrderly)
                            {
                                this.businessLogic.ProgramRemainingTime = time;
                                process?.Resume();
                                this.ranProcessesWhileTime.Add(process);
                                this.enabledProcesses.Add(process);
                                this.programStartedTimer = true;
                                this.ProgramStartedOrderlyOrActiveOccasional?.Invoke(this, new ProcessEventArgs() { ID = process.Id, ProcessName = process.ProcessName });
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
                    try
                    {
                        process?.Kill();
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogException(ex);
                        process?.Resume();
                    }
                }
            }
            catch (Win32Exception)
            {
                var process = Process.GetProcessById(id);
                try
                {
                    process?.Kill();
                }
                catch (Exception)
                {
                    process?.Resume();
                }
            }
            catch (Exception)
            {
            }
        }

        private void ProcessStoppedEventWatcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            var processName = e.NewEvent.Properties["ProcessName"].Value.ToString().Replace(".exe", string.Empty);
            try
            {
                if (this.businessLogic != null && this.businessLogic.ActiveUser != null)
                {
                    this.logger.LogStopProcess(this.businessLogic.ActiveUser.Username, processName);
                }
            }
            catch (Exception)
            {
            }

            this.ranProcessesWhileTime.Remove(this.ranProcessesWhileTime.Where(x => x?.ProcessName == processName).FirstOrDefault());
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

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var sub = BusinessLogic.Sub;
            if (this.businessLogic.ProgramRemainingTime != default && this.businessLogic.ProgramRemainingTime.TotalSeconds > 0)
            {
                this.businessLogic.ProgramRemainingTime -= sub;
            }
            else if ((this.businessLogic.ProgramRemainingTime == default || (this.businessLogic.ProgramRemainingTime != default && this.businessLogic.ProgramRemainingTime.TotalSeconds <= 0)) && this.programStartedTimer)
            {
                this.programStartedTimer = false;
                this.businessLogic.ProgramRemainingTime = default;
                this.ranProcessesWhileTime = this.ranProcessesWhileTime.Except(this.enabledProcesses).ToList();
                foreach (var process in this.enabledProcesses)
                {
                    try
                    {
                        process?.Kill();
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogException(ex);
                        process?.Resume();
                    }
                }

                this.enabledProcesses = new List<Process>();
            }
        }
    }
}
