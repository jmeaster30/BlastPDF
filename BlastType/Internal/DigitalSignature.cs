using MyLib.Streams;
using Newtonsoft.Json;

namespace BlastType.Internal;

public class DigitalSignature : IFontTable
{
    public uint Version { get; set; }
    public ushort NumberOfSignatures { get; set; }
    public ushort Flags { get; set; }
    public List<SignatureRecord> SignatureRecords { get; set; } = new();

    public static DigitalSignature Load(Stream stream)
    {
        var version = stream.ReadU32();
        var numSignatures = stream.ReadU16();
        var flags = stream.ReadU16();
        
        // TODO load the signature records
        
        return new DigitalSignature
        {
            Version = version,
            NumberOfSignatures = numSignatures,
            Flags = flags
        };
    }
    
    public bool Is<T>()
    {
        return typeof(T) == typeof(DigitalSignature);
    }

    public new string? ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}