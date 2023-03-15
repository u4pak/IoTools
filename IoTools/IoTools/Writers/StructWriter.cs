using System.Runtime.CompilerServices;

namespace IoTools.Writers;

public class StructWriter : BinaryWriter
{
    public StructWriter(Stream stream) : base(stream) {}

    public unsafe void WriteStruct<T>(T data)
    {
        int size = Unsafe.SizeOf<T>();
        var buffer = new byte[size];
        Unsafe.WriteUnaligned(ref buffer[0], data);
        Write(buffer);
    }
}