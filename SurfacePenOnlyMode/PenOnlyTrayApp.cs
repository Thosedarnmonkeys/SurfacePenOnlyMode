using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using SurfacePenOnlyMode.Properties;

namespace SurfacePenOnlyMode
{
  public class PenOnlyTrayApp : Form
  {
    #region private fields
    private ContextMenu trayMenu;
    private NotifyIcon trayIcon;

    private string instancePath = @"HID\VEN_8086&DEV_9D3E&SUBSYS_00000000&REV_21&COL05\4&F87CE30&0&0004";
    private Guid deviceGuid = new Guid("{745a17a0-74d3-11d0-b6fe-00a0c90f57da}");
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
      trayIcon.Text = "Pen Only Mode";

      Bitmap bmp = Resources.PenIcon;
      trayIcon.Icon = Icon.FromHandle(bmp.GetHicon());

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
      Close();
    }
    #endregion

    #region private methods
    private void SetDriverOn()
    {
      HardwareManager.DeviceHelper.SetDeviceEnabled(deviceGuid, instancePath, true);
    }

    private void SetDriverOff()
    {
      HardwareManager.DeviceHelper.SetDeviceEnabled(deviceGuid, instancePath, false);
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
      OnDeviceStateChange(this, new EventArgs());

      trayIcon.Icon = null;

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
