using System.Diagnostics;
using System.Management;
using System.Reflection;

namespace PlaystationCameraService.Loader;

public class DeviceService
{
    ManagementEventWatcher deviceConnectedWatcher = new ManagementEventWatcher();
    ManagementEventWatcher deviceDisconnectedWatcher = new ManagementEventWatcher();


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

      




        UploadToAllCameras();
    }

    public void Stop()
    {
        Debug.WriteLine("Stopping DeviceService...");
        // Stop listening and clean up
        if (deviceConnectedWatcher != null)
        {
            deviceConnectedWatcher.Stop();
            deviceConnectedWatcher.EventArrived -= DeviceConnected;
        }
        if (deviceDisconnectedWatcher != null)
        {
            deviceDisconnectedWatcher.Stop();
            deviceDisconnectedWatcher.EventArrived -= DeviceDisconnected;
        }
       

        Debug.WriteLine($"deviceConnectedWatcher: {deviceConnectedWatcher.Query}");

      

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
