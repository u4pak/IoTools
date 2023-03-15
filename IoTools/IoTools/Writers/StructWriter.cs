using System.Runtime.CompilerServices;

namespace IoTools.Writers;

public class StructWriter : BinaryWriter
{
    public List<byte> WrittenBytes = new();

    public unsafe void WriteStruct<T>(T data)
    {
        int size = Unsafe.SizeOf<T>();
        var buffer = new byte[size];
        Unsafe.WriteUnaligned(ref buffer[0], data);
        WrittenBytes.AddRange(buffer);
    }
    
    public unsafe void InsertStruct<T>(T data, int offset)
    {
        int size = Unsafe.SizeOf<T>();
        var buffer = new byte[size];
        Unsafe.WriteUnaligned(ref buffer[0], data);
        WrittenBytes.InsertRange(offset,buffer);
    }

    public unsafe void Write(byte[] buffer)
    {
        WrittenBytes.AddRange(buffer);
    }
    
    public unsafe void Insert(byte[] buffer, int offset)
    {
        WrittenBytes.InsertRange(offset,buffer);
    }
}