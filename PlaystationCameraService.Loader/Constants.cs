namespace PlaystationCameraService.Loader;



public class Constants
{
    public const string InterfaceGuid = "932F61A9-6CF0-6FAF-8861-DA0D8B023C5F";
    public const string UsbDevClass = "88BAE032-5A81-49F0-BC3D-A4FF138216D6";
    public const string UsbClassGuid = "{88bae032-5a81-49f0-bc3d-a4ff138216d6}";
    public const string CameraGuid = "{ca3e7ab9-b4c3-4ae6-8251-579ef933890f}";
    public const string CamClassGuid = "CA3E7AB9-B4C3-4AE6-8251-579EF933890F";




    internal static readonly Dictionary<int, string> ControlTransferMessages = new Dictionary<int, string>()
    {
        {0, "Transfer completed without error"},
        {1460, "Transfer timed out"},
        {995, "Transfer was cancelled"},
        {31, "Stall"},
        {38, "Device was disconnected"},
        {-1, "UNKNOWN_ERROR"},
    };

    internal static byte[] ExtractResource(String filename)
    {
        System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
        using (Stream resFilestream = a.GetManifestResourceStream(filename))
        {
            if (resFilestream == null) return null;
            byte[] ba = new byte[resFilestream.Length];
            resFilestream.Read(ba, 0, ba.Length);
            return ba;
        }
    }
}
