using System.Text;
using BlastSharp.Streams;
using BlastType.Internal;

namespace BlastType;

public class BlastFont
{
    public ushort NumberOfTables { get; set; }
    public ushort SearchRange { get; set; }
    public ushort EntrySelector { get; set; }
    public ushort RangeShift { get; set; }
    public List<TableRecord> TableRecords { get; set; }
    public List<IFontTable> Tables { get; set; }

    public static BlastFont Load(string fontFilename)
    {
        return Load(new FileStream(fontFilename, FileMode.Open));
    }
    
    public static BlastFont Load(Stream fontFile)
    {
        var magicBytes = fontFile.ReadBytes(4);
        var magicString = Encoding.UTF8.GetString(magicBytes);
        var magicNumber = BitConverter.ToUInt32(magicBytes);
        switch (magicString)
        {
            case "OTTO":
                return ParseFile(fontFile);
            case "ttcf":
                throw new NotImplementedException("Can't handle font collections yet :(");
            case "true":
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
        var blastFont = new BlastFont
        {
            NumberOfTables = fontFile.ReadU16(),
            SearchRange = fontFile.ReadU16(),
            EntrySelector = fontFile.ReadU16(),
            RangeShift = fontFile.ReadU16(),
            TableRecords = new(),
            Tables = new(),
        };

        for (int i = 0; i < blastFont.NumberOfTables; i++)
        {
            blastFont.TableRecords.Add(TableRecord.Load(fontFile));
        }

        Console.WriteLine($"NUMBER OF TABLE RECORDS: {blastFont.TableRecords.Count}");
        foreach (var record in blastFont.TableRecords)
        {
            Console.WriteLine("-----------");
            Console.WriteLine($"TableTag {Encoding.UTF8.GetString(record.TableTag, 0, record.TableTag.Length)}");
            Console.WriteLine($"Checksum {record.Checksum}");
            Console.WriteLine($"Offset {record.Offset}");
            Console.WriteLine($"Length {record.Length}");

            fontFile.Seek(record.Offset, SeekOrigin.Begin);
            
            var table = record.TableTagString switch
            {
                "head" => FontHeader.Load(fontFile),
                _ => null
            };

            if (table != null)
            {
                Console.WriteLine("TABLE::");
                Console.WriteLine(table.ToString());
                blastFont.Tables.Add(table);
            }
        }

        return blastFont;
    }
}