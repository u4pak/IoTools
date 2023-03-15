using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace IoTools.Writers;

public class StructWriter : BinaryWriter
{
    public List<byte> WrittenBytes = new();

    public void WriteStruct<T>(T data)
    {
        int size = Unsafe.SizeOf<T>();
        var buffer = new byte[size];
        Unsafe.WriteUnaligned(ref buffer[0], data);
        WrittenBytes.AddRange(buffer);
    }
    
    public void InsertStruct<T>(T data, int offset)
    {
        int size = Unsafe.SizeOf<T>();
        var buffer = new byte[size];
        Unsafe.WriteUnaligned(ref buffer[0], data);
        WrittenBytes.InsertRange(offset,buffer);
    }

    public void WriteList<T>(List<T> data)
    {
        // probably not the best code but it'll do.
        
        if (typeof(T) == typeof(string))
        {
            var strings = data.ConvertAll(x => x.ToString());
            foreach(string str in strings)
                Write(Encoding.ASCII.GetBytes(str));
        }
        else
            data.ForEach(x => WriteStruct(x));
    }

    public void Write(byte[] buffer)
    {
        WrittenBytes.AddRange(buffer);
    }
    
    public void Insert(byte[] buffer, int offset)
    {
        WrittenBytes.InsertRange(offset,buffer);
    }
}