using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;

namespace SurfacePenOnlyMode
{
    public class PenOnlyTrayApp : Form
    {
        private ContextMenu trayMenu;
        private NotifyIcon trayIcon;
        private readonly string[] matchArray = new string[] { "HID-compliant" , "touch", "screen"};
 
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Application.Run(new PenOnlyTrayApp());
        }

        public PenOnlyTrayApp()
        {
            trayMenu = new ContextMenu();

            trayMenu.MenuItems.Add("Enable Pen Only Mode", OnClickEnablePenOnlyMode);
            trayMenu.MenuItems.Add("-"); //Creates a seperator
            trayMenu.MenuItems.Add("Exit", OnClickExit);


            trayIcon = new NotifyIcon();
            trayIcon.Text = "MyTrayApp";
            trayIcon.Icon = new Icon(SystemIcons.Asterisk, 40, 40);

            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;

            //set driver enabled under any kind of power change
            SystemEvents.SessionEnding += OnDeviceStateChange; 
            SystemEvents.PowerModeChanged += OnDeviceStateChange;
        }

        private void OnClickDisablePenOnlyMode(object sender, EventArgs e)
        {
            SetDriverOn();

            trayMenu.MenuItems[0].Click -= OnClickDisablePenOnlyMode;
            trayMenu.MenuItems[0].Click += OnClickEnablePenOnlyMode;
            trayMenu.MenuItems[0].Text = "Enable Pen Only Mode";
        }

        private void OnClickEnablePenOnlyMode(object sender, EventArgs e)
        {
            SetDriverOff();

            trayMenu.MenuItems[0].Click -= OnClickEnablePenOnlyMode;
            trayMenu.MenuItems[0].Click += OnClickDisablePenOnlyMode;
            trayMenu.MenuItems[0].Text = "Disable Pen Only Mode";
        }

        private void OnDeviceStateChange(object sender, EventArgs e)
        {
            SetDriverOn();
        }

        private void SetDriverOn()
        {
            hwHelper.SetDeviceState(matchArray, true);
        }

        private void SetDriverOff()
        {
            hwHelper.SetDeviceState(matchArray, false);
        }

        private void OnClickExit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false;
            ShowInTaskbar = false;

            base.OnLoad(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            trayIcon = null;

            OnDeviceStateChange(this, new EventArgs());

            base.OnClosing(e);
        }

        protected override void Dispose(bool disposing)
        {
            SystemEvents.PowerModeChanged -= OnDeviceStateChange;
            SystemEvents.SessionEnding -= OnDeviceStateChange;

            base.Dispose(disposing);
        }
    }
}
