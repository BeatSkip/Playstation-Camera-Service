using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace PlaystationCameraService.Loader;

public class DeviceService
{
    ManagementEventWatcher deviceConnectedWatcher = new ManagementEventWatcher();
    ManagementEventWatcher cameraConnectedWatcher = new ManagementEventWatcher();
    ManagementEventWatcher deviceDisconnectedWatcher = new ManagementEventWatcher();
    ManagementEventWatcher cameraDisconnectedWatcher = new ManagementEventWatcher();

    public event EventHandler<CameraChangedEventArgs> CameraChanged;

    public DeviceService()
    {

    }

    ~DeviceService()
    {
        Stop();
    }

    public void Initialize()
    {
        // Query for specific USB device connections
        var queryConnected = new WqlEventQuery($"SELECT * FROM __InstanceCreationEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_PnPEntity' AND TargetInstance.ClassGuid LIKE '%{Constants.UsbDevClass}%'");
        deviceConnectedWatcher.EventArrived += new EventArrivedEventHandler(DeviceConnected);
        deviceConnectedWatcher.Query = queryConnected;  
        deviceConnectedWatcher.Start();

        // Query for specific USB device disconnections
        var queryDisconnected = new WqlEventQuery($"SELECT * FROM __InstanceDeletionEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_PnPEntity' AND TargetInstance.ClassGuid LIKE '%{Constants.UsbDevClass}%'");
        deviceDisconnectedWatcher.EventArrived += new EventArrivedEventHandler(DeviceDisconnected);
        deviceDisconnectedWatcher.Query = queryDisconnected;
        deviceDisconnectedWatcher.Start();

        var queryCamConnected = new WqlEventQuery($"SELECT * FROM __InstanceCreationEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_PnPEntity' AND TargetInstance.ClassGuid LIKE '%{Constants.CamClassGuid}%'");
        cameraConnectedWatcher.EventArrived += new EventArrivedEventHandler(CameraConnected);
        cameraConnectedWatcher.Query = queryCamConnected;
        cameraConnectedWatcher.Start();

        // Query for specific USB device disconnections
        var queryCamDisconnected = new WqlEventQuery($"SELECT * FROM __InstanceDeletionEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_PnPEntity' AND TargetInstance.ClassGuid LIKE '%{Constants.CamClassGuid}%'");
        deviceDisconnectedWatcher.EventArrived += new EventArrivedEventHandler(CameraDisconnected);
        deviceDisconnectedWatcher.Query = queryCamDisconnected;
        deviceDisconnectedWatcher.Start();
        Debug.WriteLine($"USB Watcher initialized...");

        UploadToAllCameras();
    }

    public void Stop()
    {
        // Stop listening and clean up
        deviceConnectedWatcher.Stop();
        deviceDisconnectedWatcher.Stop();
        cameraConnectedWatcher.Stop();
        cameraDisconnectedWatcher.Stop();
        deviceConnectedWatcher.Dispose();
        deviceDisconnectedWatcher.Dispose();
        cameraConnectedWatcher.Dispose();
        cameraDisconnectedWatcher.Dispose();
    }

    public async void UploadToAllCameras()
    {
        Debug.WriteLine("Uploading firmware to all cameras...");
        OrbisEyeCam.UploadFirmwareToAny();
    }

    private void DeviceConnected(object sender, EventArrivedEventArgs e)
    {
        Debug.WriteLine($"USB device connected!");
        CameraChanged?.Invoke(this, CameraChangedEventArgs.DeviceDetected);
        OrbisEyeCam.UploadFirmwareToAny();



    }

    private void DeviceDisconnected(object sender, EventArrivedEventArgs e)
    {
        CameraChanged?.Invoke(this, CameraChangedEventArgs.FirmwareUploading);
        Debug.WriteLine("Specific USB device disconnected.");
    }

    private void CameraConnected(object sender, EventArrivedEventArgs e)
    {
        Debug.WriteLine("Camera connected.");
        CameraChanged?.Invoke(this, CameraChangedEventArgs.CameraDetected);
    }

    private void CameraDisconnected(object sender, EventArrivedEventArgs e)
    {
        Debug.WriteLine("Camera disconnected.");
        CameraChanged?.Invoke(this, CameraChangedEventArgs.Disconnected);
    }
}

public class CameraChangedEventArgs
{
    public CameraStatus Status { get; init; }

    public CameraChangedEventArgs(CameraStatus status)
    {
        Status = status;
    }

    public static CameraChangedEventArgs DeviceDetected => new CameraChangedEventArgs(CameraStatus.DeviceDetected);
    public static CameraChangedEventArgs FirmwareUploading => new CameraChangedEventArgs(CameraStatus.FirmwareUploading);
    public static CameraChangedEventArgs CameraDetected => new CameraChangedEventArgs(CameraStatus.CameraDetected);
    public static CameraChangedEventArgs Connected => new CameraChangedEventArgs(CameraStatus.Connected);
    public static CameraChangedEventArgs Disconnected => new CameraChangedEventArgs(CameraStatus.Disconnected);
    public static CameraChangedEventArgs Error => new CameraChangedEventArgs(CameraStatus.Error);
}

public enum CameraStatus
{
    DeviceDetected,
    FirmwareUploading,
    CameraDetected,
    Connected,
    Disconnected,
    Error,
}
