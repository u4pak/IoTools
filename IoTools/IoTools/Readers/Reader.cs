using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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
}