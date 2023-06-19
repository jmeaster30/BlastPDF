namespace BlastType.Internal.DataTypes;

public class LongDateTime
{
    private long Seconds { get; set; }
    public DateTime DateTime => ToDateTime();

    public static LongDateTime FromBytes(byte[] bytes)
    {
        var b = bytes;
        //?? Should this function be agnostic to endianness or is this useful?
        if (BitConverter.IsLittleEndian)
        {
            b = bytes.Reverse().ToArray();
        }
        return new LongDateTime
        {
            Seconds = BitConverter.ToInt64(b)
        };
    }

    public DateTime ToDateTime()
    {
        return new DateTime(1904, 1, 1).AddSeconds(Seconds);
    } 
}