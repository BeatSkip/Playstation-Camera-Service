using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaystationCameraService.Loader;

public class OrbisEyeCamFrame
{
    public byte[] Header { get; set; }
    public byte[] Audio { get; set; }
    public byte[][] VideoFrame { get; set; } = new byte[2][];
    public byte[][] VideoFrameHigh { get; set; } = new byte[2][];
    public byte[][] VideoFrameMedium { get; set; } = new byte[2][];
    public byte[][] VideoFrameLow { get; set; } = new byte[2][];
    public byte[] Mode { get; set; }
}
