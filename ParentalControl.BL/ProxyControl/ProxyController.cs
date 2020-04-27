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
        private static readonly char[] UTF8Charaters = { 'á', 'é', 'í', 'ó', 'ö', 'ő', 'ú', 'ü', 'ű' };
        private ProxyServer proxyServer;
        private BusinessLogic businessLogic;
        private RegistryMonitor registryMonitor;
        private ExplicitProxyEndPoint proxyEndPoint;
        private List<string> keywords;
        private Logger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyController"/> class.
        /// </summary>
        public ProxyController()
        {
            this.logger = Logger.Get();
            this.proxyServer = new ProxyServer()
            {
                ExceptionFunc = exception =>
                {
                    this.logger.LogException(exception);
                },
            };

            this.registryMonitor = new RegistryMonitor("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings");
            this.registryMonitor.RegChanged += this.OnProxyRegChanged;
        }

        /// <summary>
        /// Start proxy.
        /// </summary>
        public void Start()
        {
            if (!this.proxyServer.ProxyRunning)
            {
                this.businessLogic = BusinessLogic.Get();
                if (this.businessLogic.ActiveUser == null)
                {
                    throw new ArgumentNullException(nameof(this.businessLogic.ActiveUser));
                }

                this.keywords = new List<string>();
                var webLimitations = this.businessLogic.Database.ReadWebLimitations(x => x.UserID == this.businessLogic.ActiveUser.ID);
                foreach (var webLimitation in webLimitations)
                {
                    this.keywords.Add(this.businessLogic.Database.ReadKeywords(x => x.ID == webLimitation.KeywordID).FirstOrDefault()?.Name);
                }

                this.proxyServer.BeforeRequest += this.ProxyServer_BeforeRequest;
                this.proxyEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, 6969);
                this.proxyServer.AddEndPoint(this.proxyEndPoint);
                this.proxyServer.Start();
                this.proxyServer.SetAsSystemProxy(this.proxyEndPoint, ProxyProtocolType.AllHttp);
                this.registryMonitor.Start();
            }
        }

        /// <summary>
        /// Stop proxy.
        /// </summary>
        public void Stop()
        {
            if (this.proxyServer.ProxyRunning)
            {
                this.registryMonitor.Stop();
                this.proxyServer.BeforeRequest -= this.ProxyServer_BeforeRequest;
                this.proxyServer.Stop();
            }
        }

        private static string ReplaceUTF8CharactersToUTF8Codes(string utf8String)
        {
            utf8String = utf8String.ToLower();
            foreach (var utf8Charater in UTF8Charaters)
            {
                if (utf8String.Contains(utf8Charater))
                {
                    string replace = Encoding.UTF8.GetBytes(utf8Charater.ToString())
                        .Select(x => string.Format("{0:X}", x).ToLower())
                        .Aggregate((x, y) => string.Format("%{0}%{1}", x, y));
                    utf8String = utf8String.Replace(utf8Charater.ToString(), replace);
                }
            }

            return utf8String;
        }

        private Task ProxyServer_BeforeRequest(object sender, SessionEventArgs e)
        {
            this.logger.LogHttpUrl(this.businessLogic.ActiveUser.Username, e.HttpClient.Request.Url);
            if (this.businessLogic.ActiveUser == null)
            {
                throw new ArgumentNullException(nameof(this.businessLogic.ActiveUser));
            }

            var absoluteUri = e.HttpClient.Request.RequestUri.AbsoluteUri.ToLower();
            if (this.keywords.Where(
                x =>
                absoluteUri.Contains(x) ||
                absoluteUri.Contains(HttpUtility.HtmlEncode(x)) ||
                absoluteUri.Contains(ReplaceUTF8CharactersToUTF8Codes(x))).Any())
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
            if (this.proxyServer.ProxyRunning)
            {
                this.proxyServer.SetAsSystemProxy(this.proxyEndPoint, ProxyProtocolType.AllHttp);
            }
        }
    }
}
