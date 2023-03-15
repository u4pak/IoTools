﻿using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

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

    public unsafe void WriteList<T>(List<T> data)
    {
        // probably not the best code but it'll do.
        
        if (typeof(T) == typeof(string))
        {
            foreach(string str in data.ConvertAll(x => x.ToString()))
            {
                Write(Encoding.ASCII.GetBytes(str));
            }
        }
        else if (typeof(T) == typeof(int))
        {
            foreach(int datai in data.ConvertAll(x => int.Parse(x.ToString())))
            {
                Write(BitConverter.GetBytes(datai));
            }
        }
        else if (typeof(T) == typeof(long))
        {
            foreach(int datai in data.ConvertAll(x => long.Parse(x.ToString())))
            {
                Write(BitConverter.GetBytes(datai));
            }
        }
        else if (typeof(T) == typeof(ulong))
        {
            foreach(int datai in data.ConvertAll(x => ulong.Parse(x.ToString())))
            {
                Write(BitConverter.GetBytes(datai));
            }
        }
        else if (typeof(T) == typeof(ushort))
        {
            foreach(int datai in data.ConvertAll(x => ushort.Parse(x.ToString())))
            {
                Write(BitConverter.GetBytes(datai));
            }
        }
        else if (typeof(T) == typeof(short))
        {
            foreach(int datai in data.ConvertAll(x => short.Parse(x.ToString())))
            {
                Write(BitConverter.GetBytes(datai));
            }
        }
        else if (typeof(T) == typeof(uint))
        {
            foreach(int datai in data.ConvertAll(x => uint.Parse(x.ToString())))
            {
                Write(BitConverter.GetBytes(datai));
            }
        }
        else if (typeof(T) == typeof(byte))
        {
            foreach(int datai in data.ConvertAll(x => byte.Parse(x.ToString())))
            {
                Write(Convert.ToByte(datai));
            }
        }
        else if (typeof(T) == typeof(byte[]))
        {
            // probably bad code but it'll do for now.
            
            List<byte[]> byteArrayList = new List<byte[]>();
            BinaryFormatter formatter = new BinaryFormatter();
            
            foreach(T datai in data)
            {
                using (MemoryStream stream = new MemoryStream()) {
                    formatter.Serialize(stream, datai);
                    byteArrayList.Add(stream.ToArray());
                }
            }

            foreach (var bytearray in byteArrayList)
                Write(bytearray);
        }
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