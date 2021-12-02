﻿using ReloadPreview;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SharpConstraintLayout.Example
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string IP = "172.30.80.1";//"192.168.0.108";
        public static int Port = 450;
        public static ReloadClient ReloadClient;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ReloadClient = new ReloadClient(IP, Port);
            ReloadClient.Start();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            ReloadClient.Stop();
        }
    }
}
