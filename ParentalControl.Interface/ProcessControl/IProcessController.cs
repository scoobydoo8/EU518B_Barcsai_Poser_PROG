// <copyright file="IProcessController.cs" company="PlaceholderCompany">
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
        /// Program started with orderly permission event.
        /// </summary>
        event EventHandler<IProcessEventArgs> ProgramStartedOrderly;

        /// <summary>
        /// Program started with occassional permission event.
        /// </summary>
        event EventHandler<IProcessEventArgs> ProgramStartedOccassional;

        /// <summary>
        /// Program started with full limit event.
        /// </summary>
        event EventHandler<IProcessEventArgs> ProgramStartedFullLimit;

        /// <summary>
        /// Program resume.
        /// </summary>
        /// <param name="processID">Process ID.</param>
        void ProgramResume(int processID);

        /// <summary>
        /// Program kill.
        /// </summary>
        /// <param name="processID">Process ID.</param>
        void ProgramKill(int processID);
    }
}
