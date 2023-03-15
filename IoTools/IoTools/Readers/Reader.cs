using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace IoTools.Readers;

public class Reader
{
    public static T ReadStruct<T>(byte[] buffer, int offset)
    {
        byte[] bytes = new byte[Marshal.SizeOf<T>()];
        Buffer.BlockCopy(buffer, offset, bytes, 0, Marshal.SizeOf<T>());
        GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        T data = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));

        return data;
    }

    /*public static T ReadValue<T>(byte[] buffer, int offset, int length = 0)
    {
        int size = Unsafe.SizeOf<T>();
        byte[] data = new byte[size];

        if (typeof(T) == typeof(string))
        {
            return Encoding.ASCII.GetString(
                FromHex(BitConverter.ToString(UReader.Read(Partition.ToArray(), 0, (int)Length2))));
        }

        return null;
    }*/
    
    public static byte[] FromHex(string hex)
    {
        hex = hex.Replace("-", "");
        byte[] raw = new byte[hex.Length / 2];
        for (int i = 0; i < raw.Length; i++)
        {
            raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        }
        return raw;
    }
}