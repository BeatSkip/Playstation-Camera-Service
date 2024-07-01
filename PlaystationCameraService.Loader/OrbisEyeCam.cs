using Nefarius.Drivers.WinUSB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaystationCameraService.Loader;


public class OrbisEyeCam : IDisposable
{
    private readonly USBDevice UsbDev;
    private const int CHUNK_SIZE = 512;
    public string VendorID => "0x" + UsbDev.Descriptor.VID.ToString("X4");
    public string ProductID => "0x" + UsbDev.Descriptor.PID.ToString("X4");
    public string firmwarePath { get; set; }

    public OrbisEyeCam(USBDeviceInfo device, bool check_firmware = true)
    {
        UsbDev = new USBDevice(device.DevicePath);
    }

    public async Task InitializeFirmware(string path)
    {
        var bytes = System.IO.File.ReadAllBytes(path);
        var result = await UploadFirmwareBytes(bytes);
        if (result)
        {
            Console.WriteLine("Firmware uploaded successfully");
        }
        else
        {
            Console.WriteLine("Firmware upload failed");
        }
    }

    public async Task InitializeFirmware()
    {

        var fwbytes = Constants.ExtractResource("fw_fixed.bin");
        await UploadFirmwareBytes(fwbytes);
    }

    private async Task<bool> UploadFirmwareBytes(byte[] firmware)
    {
        var chunks = firmware.Chunkinate(CHUNK_SIZE);
        var chunkIndex = 0;
        byte[] chunk = new byte[CHUNK_SIZE];

        try
        {
            long length = firmware.Length;


            ushort index = 0x14;
            ushort value = 0;

            for (int pos = 0; pos < length; pos += CHUNK_SIZE)
            {
                ushort size = (ushort)Math.Min(CHUNK_SIZE, length - pos);
                chunk = chunks[chunkIndex++];
                var status = await UsbDev.ControlOutAsync(0x40, 0x0, value, index, chunk, size);
                if ((value + size) > 0xFFFF)
                {
                    index += 1;
                }
                value += size;
            }

            chunk[0] = 0x5B;
            int status_finalize = await UsbDev.ControlOutAsync(0x40, 0x0, 0x2200, 0x8018, chunk, 1);

            Console.WriteLine("Firmware uploaded...");
            return true;

        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        return false;
    }

    public static List<OrbisEyeCam> getDevices()
    {
        return USBDevice.GetDevices(Constants.InterfaceGuid).Select(x => new OrbisEyeCam(x)).ToList();
    }

    public static async Task UploadFirmwareToAny(byte[] fw)
    {
        var devs = getDevices();
        foreach (var dev in devs)
        {
            await dev.UploadFirmwareBytes(fw);
        }
    }

    public static async Task UploadFirmwareToAny()
    {
        var fwbytes = Constants.ExtractResource("PlaystationCameraService.Loader.fw_fixed.bin");
        Debug.WriteLine("Firmware bytes: " + fwbytes.Length);
        await UploadFirmwareToAny(fwbytes);

    }

    public void Dispose()
    {
        UsbDev.Dispose();
    }
}
