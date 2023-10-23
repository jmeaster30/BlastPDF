using MyLib.Enumerables;

namespace BlastType.Internal.DataTypes;

public class Fixed
{
    public int IntegerSize { get; set; }
    public int FractionalSize { get; set; }

    public int IntegerPart { get; set; }
    public int FractionalPart { get; set; }

    public decimal Value => ToDecimal();

    public static Fixed FromBytes(int integerSize, int fractionalSize, byte[] bytes)
    {
        var bitList = bytes.ToBitList();
        var integerPart = bitList.ReadBits(0, integerSize).AsEnumerable().PadLeft(4, (byte)0);
        //?? Should this function be agnostic to endianness or is this useful?
        if (BitConverter.IsLittleEndian)
        {
            integerPart = integerPart.Reverse();
        }
        var fractionalPart = bitList.ReadBits(integerSize, fractionalSize).AsEnumerable().PadLeft(4, (byte)0);
        //?? Should this function be agnostic to endianness or is this useful?
        if (BitConverter.IsLittleEndian)
        {
            fractionalPart = fractionalPart.Reverse();
        }
        return new Fixed
        {
            IntegerSize = integerSize,
            FractionalSize = fractionalSize,
            IntegerPart = BitConverter.ToInt32(integerPart.ToArray()),
            FractionalPart = BitConverter.ToInt32(fractionalPart.ToArray())
        };
    }

    public override string ToString()
    {
        return ToDecimal().ToString();
    }

    public decimal ToDecimal()
    {
        return IntegerPart + FractionalPart / (decimal)(1 << FractionalSize);
    }
}