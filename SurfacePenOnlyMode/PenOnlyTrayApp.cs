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
    #region private fields
    private ContextMenu trayMenu;
    private NotifyIcon trayIcon;
    private readonly Func<string, bool> matchFunc = s => s.ToLower() == "hid-compliant touch screen";
    #endregion

    #region public methods
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
    #endregion

    #region private event handlers
    private void OnClickDisablePenOnlyMode(object sender, EventArgs e)
    {
      try
      {
        SetDriverOn();

        trayMenu.MenuItems[0].Click -= OnClickDisablePenOnlyMode;
        trayMenu.MenuItems[0].Click += OnClickEnablePenOnlyMode;
        trayMenu.MenuItems[0].Text = "Enable Pen Only Mode";
      }
      catch (Exception except)
      {
        MessageBox.Show("Error while trying to set driver on: " + except.Message);
      }
    }

    private void OnClickEnablePenOnlyMode(object sender, EventArgs e)
    {
      try
      {
        SetDriverOff();

        trayMenu.MenuItems[0].Click -= OnClickEnablePenOnlyMode;
        trayMenu.MenuItems[0].Click += OnClickDisablePenOnlyMode;
        trayMenu.MenuItems[0].Text = "Disable Pen Only Mode";
      }
      catch (Exception except)
      {
        MessageBox.Show("Error while trying to set driver off: " + except.Message);
      }
    }

    private void OnDeviceStateChange(object sender, EventArgs e)
    {
      SetDriverOn();
    }
    private void OnClickExit(object sender, EventArgs e)
    {
      Application.Exit();
    }
    #endregion

    #region private methods
    private void SetDriverOn()
    {
      HardwareManager.SetDeviceState(matchFunc, false);
    }

    private void SetDriverOff()
    {
      HardwareManager.SetDeviceState(matchFunc, true);
    }
    #endregion

    #region overidden methods
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
    #endregion
  }
}
