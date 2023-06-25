namespace BlastType.Internal;

public class SignatureRecord
{
    public uint Format { get; set; }
    public uint Length { get; set; }
    public uint Offset { get; set; }
    
    // TODO implement loading
    // TODO implement Format1 signatures
}