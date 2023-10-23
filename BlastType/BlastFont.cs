using System.Text;
using BlastType.Internal;
using MyLib.Enumerables;
using MyLib.Streams;

namespace BlastType;

public class BlastFont
{
    public ushort NumberOfTables { get; set; }
    public ushort SearchRange { get; set; }
    public ushort EntrySelector { get; set; }
    public ushort RangeShift { get; set; }
    public List<TableRecord> TableRecords { get; set; } = new();
    public List<IFontTable> Tables { get; set; } = new();

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
        };

        for (int i = 0; i < blastFont.NumberOfTables; i++)
        {
            blastFont.TableRecords.Add(TableRecord.Load(fontFile));
        }

        Console.WriteLine($"NUMBER OF TABLE RECORDS: {blastFont.TableRecords.Count}");
        // some tables require data in other tables so we order by the offset
        foreach (var record in blastFont.TableRecords.OrderBy(x => x.Offset))
        {
            Console.WriteLine("-----------");
            Console.WriteLine($"TableTag '{Encoding.UTF8.GetString(record.TableTag, 0, record.TableTag.Length)}'");
            Console.WriteLine($"Checksum {record.Checksum}");
            Console.WriteLine($"Offset {record.Offset}");
            Console.WriteLine($"Length {record.Length}");

            fontFile.Seek(record.Offset, SeekOrigin.Begin);
            
            // TODO table should be non-nullable if we can process all table tags
            IFontTable? table = record.TableTagString switch
            {
                "CFF " => CompactFontFormatTable.Load(fontFile),
                "cmap" => CharacterMapTable.Load(fontFile),
                "DSIG" => DigitalSignature.Load(fontFile),
                "GDEF" => GlyphDefinitionTable.Load(fontFile),
                "GPOS" => GlyphPositioningTable.Load(fontFile),
                "head" => FontHeader.Load(fontFile),
                "hhea" => HorizontalHeader.Load(fontFile),
                "hmtx" => HorizontalMetrics.Load(fontFile, blastFont.Tables),
                "maxp" => MaximumProfile.Load(fontFile),
                "name" => NameTable.Load(fontFile),
                "OS/2" => Os2.Load(fontFile),
                "post" => PostTable.Load(fontFile),
                _ => null
            };

            if (table == null)
            { 
                Console.WriteLine("UNIMPLEMENTED");
                fontFile.Seek(record.Offset, SeekOrigin.Begin);
                var bytes = fontFile.ReadBytes((int)Math.Min(10, record.Length));
                Console.WriteLine(bytes.Select(x => x.ToString("X2")).Join(" "));
                continue;
            }
            Console.WriteLine("TABLE::");
            Console.WriteLine(table.ToString());
            blastFont.Tables.Add(table);
        }

        return blastFont;
    }
}