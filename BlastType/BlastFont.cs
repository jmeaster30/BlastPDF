using System.Text;
using BlastSharp.Streams;

namespace BlastType;

public class BlastFont
{
    private class TableRecord
    {
        public byte[] TableTag { get; set; } = default!;
        public uint Checksum { get; set; }
        public uint Offset { get; set; }
        public uint Length { get; set; }
    }
    
    public static BlastFont Load(string fontFilename)
    {
        return Load(new FileStream(fontFilename, FileMode.Open));
    }
    
    public static BlastFont Load(Stream fontFile)
    {
        var magicBytes = fontFile.ReadBytes(4);
        var magicString = Encoding.UTF8.GetString(magicBytes);
        var magicNumber = BitConverter.ToUInt32(magicBytes);
        if (magicString == "OTTO")
        {
            return ParseFile(fontFile);
        }
        if (magicString == "ttcf")
        {
            throw new NotImplementedException("Can't handle font collections yet :(");
        }
        if (magicString == "true")
        {
            return ParseFile(fontFile);
        }
        if (magicNumber == 0x00010000)
        {
            return ParseFile(fontFile);
        }

        throw new ArgumentException("File is not a valid font file", nameof(fontFile));
    }

    private static BlastFont ParseFile(Stream fontFile)
    {
        var numberOfTables = fontFile.ReadU16();
        var searchRange = fontFile.ReadU16();
        var entrySelector = fontFile.ReadU16();
        var rangeShift = fontFile.ReadU16();

        var tableRecords = new List<TableRecord>();

        for (int i = 0; i < numberOfTables; i++)
        {
            tableRecords.Add(new TableRecord
            {
                TableTag = fontFile.ReadBytes(4),
                Checksum = fontFile.ReadU32(),
                Offset = fontFile.ReadU32(),
                Length = fontFile.ReadU32(),
            });
        }

        Console.WriteLine($"NUMBER OF TABLE RECORDS: {tableRecords.Count}");
        foreach (var record in tableRecords)
        {
            Console.WriteLine("-----------");
            Console.WriteLine($"TableTag {Encoding.UTF8.GetString(record.TableTag, 0, record.TableTag.Length)}");
            Console.WriteLine($"Checksum {record.Checksum}");
            Console.WriteLine($"Offset {record.Offset}");
            Console.WriteLine($"Length {record.Length}");
        }

        return new BlastFont();
    }
}