using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaystationCameraService.Loader;

public static class ControlTransferStatusExtensions
{
    internal static int toInt(this ControlTransferStatus i)
    {
        return (int)i;
    }

    internal static string description(this ControlTransferStatus i)
    {
        return Constants.ControlTransferMessages[i.toInt()];
    }
}


public static class ArrayExtensions
{

    public static List<byte[]> Chunkinate(this byte[] source, int size)
    {
        var result = new List<byte[]>();
        for (int i = 0; i < source.Length; i += size)
        {
            byte[] chunk = new byte[Math.Min(size, source.Length - i)];
            Array.Copy(source, i, chunk, 0, chunk.Length);
            result.Add(chunk);
        }
        return result;
    }
}