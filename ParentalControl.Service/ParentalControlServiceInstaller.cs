// <copyright file="ParentalControlServiceInstaller.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParentalControl.Service
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration.Install;
    using System.Linq;
    using System.ServiceProcess;
    using System.Threading.Tasks;

    /// <summary>
    /// Parental control service installer class.
    /// </summary>
    [RunInstaller(true)]
    public partial class ParentalControlServiceInstaller : Installer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParentalControlServiceInstaller"/> class.
        /// </summary>
        public ParentalControlServiceInstaller()
        {
            this.InitializeComponent();
            var processInstaller = new ServiceProcessInstaller();
            var serviceInstaller = new ServiceInstaller();

            processInstaller.Account = ServiceAccount.LocalSystem;

            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.ServiceName = "ParentalControlService";

            this.Installers.Add(serviceInstaller);
            this.Installers.Add(processInstaller);
        }
    }
}
