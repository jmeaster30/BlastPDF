using System.Text;
using MyLib.Streams;

namespace BlastType.Internal;

public class TableRecord
{
    public byte[] TableTag { get; set; } = default!;
    public uint Checksum { get; set; }
    public uint Offset { get; set; }
    public uint Length { get; set; }

    public string TableTagString => Encoding.UTF8.GetString(TableTag, 0, TableTag.Length);
    
    public static TableRecord Load(Stream stream)
    {
        return new TableRecord
        {
            TableTag = stream.ReadBytes(4),
            Checksum = stream.ReadU32(),
            Offset = stream.ReadU32(),
            Length = stream.ReadU32(),
        };
    }
}