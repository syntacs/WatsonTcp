﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using WatsonTcp;

namespace Test.WinFormServer
{
    public partial class ServerForm : Form
    {
        private string _ClientIpPort = null;
        private WatsonTcpServer _Server = null;
        delegate void _LogDelegate(string msg);
        
        public ServerForm()
        {
            InitializeComponent();

            label1.Text = "";

            _Server = new WatsonTcpServer("127.0.0.1", 9000);
            // _server.Settings.MaxConnections = 1;
            _Server.Events.MessageReceived += OnMessageReceived;
            _Server.Events.ClientConnected += OnClientConnected;
            _Server.Events.ClientDisconnected += OnClientDisconnected;
            _Server.Settings.Logger = Logger; 

            Logger("Server started.");
        }
         
        private void OnClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            Logger("Client " + e.IpPort + " disconnected: " + e.Reason.ToString());
            _ClientIpPort = string.Empty;
        }

        private void OnClientConnected(object sender, ClientConnectedEventArgs e)
        {
            Logger("Client " + e.IpPort + " connected");
            _ClientIpPort = e.IpPort;
        }

        private void OnMessageReceived(object sender, MessageReceivedFromClientEventArgs e)
        {
            Logger("Client " + e.IpPort + ": " + Encoding.UTF8.GetString(e.Data));
        }

        private void bSend_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(_ClientIpPort))
            {
                _Server.Send(_ClientIpPort, "Hello world!");
                Logger("Sent 'Hello world!' to client " + _ClientIpPort);
            }
            else
            {
                Logger("No client connected");
            }
        }

        private void Logger(string msg)
        {
            // If this is called by another thread we have to use Invoke           
            if (this.InvokeRequired)
                // if the GUI (main) thread is blocked  (e.g. Console.ReadLine), 
                //    this needs to be  called asynchroneously, otherwise will deadlock
                this.BeginInvoke(new _LogDelegate(Logger), new object[] { msg });
            else
                label1.Text += Environment.NewLine + msg;
        }

        private void bStop_Click(object sender, EventArgs e)
        {
            try
            {
                _Server.Stop();
            }
            catch (Exception ex)
            {
                Logger("Stop exception");
                Logger(SerializationHelper.SerializeJson(ex, true));
            }

            Logger("Leaving bStop_Click");
        }

        private void bStart_Click(object sender, EventArgs e)
        {
            try
            {
                _Server.Start();
            }
            catch (Exception ex)
            {
                Logger("Start exception");
                Logger(SerializationHelper.SerializeJson(ex, true));
            }

            Logger("Leaving bStart_Click");
        }
    }

    internal static class SerializationHelper
    {
        private static readonly JsonSerializerSettings HardenedSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.None // Prevents CS2328 style attacks if a project is allowing automatic type resolution elsewhere.
        };

        internal static T DeserializeJson<T>(string json)
        {
            if (String.IsNullOrEmpty(json)) throw new ArgumentNullException(nameof(json));
            return JsonConvert.DeserializeObject<T>(json, HardenedSerializerSettings);
        }

        private static readonly JsonSerializerSettings SerializerDefaults = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            DateTimeZoneHandling = DateTimeZoneHandling.Local,
        };

        internal static string SerializeJson(object obj, bool pretty)
        {
            if (obj == null) return null;
            string json;

            if (pretty)
            {
                json = JsonConvert.SerializeObject(
                    obj,
                    Newtonsoft.Json.Formatting.Indented,
                    SerializerDefaults
                );
            }
            else
            {
                json = JsonConvert.SerializeObject(
                    obj,
                    SerializerDefaults
                );
            }

            return json;
        }
    }
}
