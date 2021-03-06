﻿// <copyright file="IProcessController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParentalControl.Interface.ProcessControl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Process controller interface.
    /// </summary>
    public interface IProcessController
    {
        /// <summary>
        /// Program started with orderly or active occasional permission event.
        /// </summary>
        event EventHandler<IProcessEventArgs> ProgramStartedOrderlyOrActiveOccasional;

        /// <summary>
        /// Program started with occassional permission event.
        /// </summary>
        event EventHandler<IProcessEventArgs> ProgramStartedOccassional;

        /// <summary>
        /// Program started with full limit event.
        /// </summary>
        event EventHandler<IProcessEventArgs> ProgramStartedFullLimit;

        /// <summary>
        /// Is occassional permission.
        /// </summary>
        /// <param name="adminUsername">Admin username.</param>
        /// <param name="adminPassword">Admin password.</param>
        /// <param name="minutes">Minutes.</param>
        /// <param name="processID">Process ID.</param>
        /// <returns>Valid.</returns>
        bool IsOccassionalPermission(string adminUsername, string adminPassword, int minutes, int processID);

        /// <summary>
        /// Get occasional permission.
        /// </summary>
        /// <param name="processID">ProcessID.</param>
        /// <returns>Success.</returns>
        bool GetOccasionalPermission(int processID);

        /// <summary>
        /// Kill process.
        /// </summary>
        /// <param name="processID">ProcessID.</param>
        void KillProcess(int processID);
    }
}
