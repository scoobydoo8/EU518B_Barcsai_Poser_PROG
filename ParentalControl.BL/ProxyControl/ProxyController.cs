// <copyright file="ProxyController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ParentalControl.BL.ProxyControl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using ParentalControl.Data.Database;
    using Titanium.Web.Proxy;
    using Titanium.Web.Proxy.EventArguments;
    using Titanium.Web.Proxy.Exceptions;
    using Titanium.Web.Proxy.Models;

    /// <summary>
    /// ProxyController class.
    /// </summary>
    internal class ProxyController
    {
        private ProxyServer proxyServer;
        private BusinessLogic businessLogic;
        private ExplicitProxyEndPoint proxyEndPoint;
        private RegistryMonitor registryMonitor;
        private List<string> keywords;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyController"/> class.
        /// </summary>
        public ProxyController()
        {
            this.proxyServer = new ProxyServer()
            {
                ExceptionFunc = exception =>
                {
                    if (exception is ProxyHttpException phex)
                    {
                        // TODO LOGGER (exception.Message + ": " + phex.InnerException?.Message);
                    }
                    else
                    {
                        // TODO LOGGER (exception.Message);
                    }
                },
            };

            this.proxyEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, 6969);
            this.proxyServer.AddEndPoint(this.proxyEndPoint);
            this.registryMonitor = new RegistryMonitor("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings");
            this.registryMonitor.RegChanged += this.OnProxyRegChanged;
        }

        /// <summary>
        /// Start proxy.
        /// </summary>
        /// <param name="keywords">Keywords.</param>
        public void Start(List<string> keywords)
        {
            this.businessLogic = BusinessLogic.Get();
            this.keywords = keywords;
            this.proxyServer.BeforeRequest += this.ProxyServer_BeforeRequest;
            this.proxyServer.Start();
            this.proxyServer.SetAsSystemProxy(this.proxyEndPoint, ProxyProtocolType.AllHttp);
        }

        /// <summary>
        /// Stop proxy.
        /// </summary>
        public void Stop()
        {
            this.proxyServer.BeforeRequest -= this.ProxyServer_BeforeRequest;
            this.proxyServer.Stop();
        }

        private Task ProxyServer_BeforeRequest(object sender, SessionEventArgs e)
        {
            // TODO LOGGER (e.HttpClient.Request.Url);
            if (this.businessLogic.ActiveUser == null)
            {
                throw new ArgumentNullException(nameof(this.businessLogic.ActiveUser));
            }

            var absoluteUri = e.HttpClient.Request.RequestUri.AbsoluteUri;
            if (this.keywords.Where(x => absoluteUri.Contains(x)).Any() ||
                this.keywords.Where(x => absoluteUri.Contains(HttpUtility.HtmlEncode(x))).Any())
            {
                e.Ok("<!DOCTYPE html>" +
                      "<html><body><h1>" +
                      "Weboldal blokkolva!" +
                      "</h1>" +
                      "<p>A megadott oldal tiltott!</p>" +
                      "</body>" +
                      "</html>");
            }

            return Task.CompletedTask;
        }

        private void OnProxyRegChanged(object sender, EventArgs e)
        {
            this.proxyServer.SetAsSystemProxy(this.proxyEndPoint, ProxyProtocolType.AllHttp);
        }
    }
}
