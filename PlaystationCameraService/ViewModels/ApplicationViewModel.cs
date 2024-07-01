using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using PlaystationCameraService.Loader;
using PlaystationCameraService.Services;


namespace PlaystationCameraService.ViewModels;

public partial class ApplicationViewModel : ObservableObject
{

   
    DeviceService deviceService = new DeviceService();

    public ApplicationViewModel()
    {
       deviceService.Initialize();

        deviceService.CameraChanged += DeviceService_CameraChanged;
        this.AutoStart = Startup.IsInStartup();
    }

    private void DeviceService_CameraChanged(object? sender, CameraChangedEventArgs e)
    {
       Debug.WriteLine($"Camera changed: {e.Status}");
    }

    #region shutdown
    [RelayCommand]
    private void ApplicationExit()
    {
        deviceService.Stop();
        OnShutdownRequested();
    }

    public EventHandler<EventArgs> ShutdownRequested;
    // in conjunction with typical .NET events, we will create the used "OnXy" method
    protected virtual void OnShutdownRequested()
    {
        ShutdownRequested?.Invoke(this, new EventArgs());
    }
    #endregion

    #region Firmware Selection

    [RelayCommand]
    private void ReadFirmware()
    {

    }


    [ObservableProperty]
    private string firmwarePath = string.Empty;


    [ObservableProperty]
    private bool autoStart = false;

    [RelayCommand]
    private void ToggleAutoStart()
    {
        this.AutoStart = !this.AutoStart;
        if (autoStart)
        {
            Startup.RunOnStartup();
        }
        else
        {
            Startup.RemoveFromStartup();

        }
    }
    #endregion

    #region Autostart

    #endregion




}