using System;
using System.Collections.Generic;
using System.Linq;
using BlastSharp.Lists;

namespace BlastPDF.Filter.Implementations;

public class LzwParameters : IFilterParameters
{
    public int Predictor { get; set; } = 1;
    public int Colors { get; set; } = 1;
    public int BitsPerComponent { get; set; } = 8;
    public int Columns { get; set; } = 1;
    public int EarlyChange { get; set; } = 1;
}

public class Lzw : IFilterAlgorithm
{
    // FIXME actually utilize this parameters :)
    private LzwParameters LzwParameters { get; } = new();

    private Dictionary<string, int> codewords { get; set; } = new();
    private Dictionary<int, byte[]> values { get; set; } = new();

    public Lzw(LzwParameters parameters)
    {
        if (parameters == null) return;
        if (parameters.Predictor is not (1 or 2 or 10 or 11 or 12 or 13 or 14 or 15))
            throw new ArgumentOutOfRangeException(nameof(parameters.Predictor), "Predictor should be 1, 2, 10, 11, 12, 13, 14, or 15.");
        if (parameters.Colors < 1)
            throw new ArgumentOutOfRangeException(nameof(parameters.Colors), "Colors must be greater than or equal to 1.");
        if (parameters.BitsPerComponent is not (1 or 2 or 4 or 8 or 16))
            throw new ArgumentOutOfRangeException(nameof(parameters.BitsPerComponent),
                "BitsPerComponent must be 1, 2, 4, 8, or 16.");
        if (parameters.EarlyChange is not 1 and not 2)
            throw new ArgumentOutOfRangeException(nameof(parameters.EarlyChange), "EarlyChange must be 0 or 1.");
        LzwParameters = parameters;
    }

    private int GetCodeword(byte[] input)
    {
        return codewords[input.Hash()];
    }

    private bool ContainsCodeword(byte[] input)
    {
        return codewords.ContainsKey(input.Hash());
    }

    private void InsertCodeword(byte[] input, int code)
    {
        codewords[input.Hash()] = code;
    }

    private (int, int) ClearCodewords()
    {
        for (int i = 0; i < 256; i++)
        {
            InsertCodeword(new[] {(byte) i}, i);
        }
        return (EOD + 1, 9);
    }
    
    private (int, int) ClearValues()
    {
        for (int i = 0; i < 256; i++)
        {
            InsertValue(i, new[] {(byte) i});
        }
        return (EOD + 1, 9);
    }
    
    private byte[] GetValue(int input)
    {
        return values[input];
    }

    private bool ContainsValue(int input)
    {
        return values.ContainsKey(input);
    }

    private void InsertValue(int input, byte[] value)
    {
        values[input] = value;
    }

    private const int EOD = 257;
    private const int CLEAR_TABLE = 256;

    public IEnumerable<byte> Encode(IEnumerable<byte> input)
    {
        var (currentCodeValue, currentCodeLength) = ClearCodewords();

        var buffer = new byte[]{};
        var result = new BitList();

        foreach (var b in input)
        {
            var combined = buffer.Append(b).ToArray();
            if (ContainsCodeword(combined))
            {
                buffer = combined.ToArray();
            }
            else
            {
                var codeword = GetCodeword(buffer);
                result.AppendBits(codeword, currentCodeLength);
                
                InsertCodeword(combined, currentCodeValue);
                currentCodeValue += 1;
                if (currentCodeValue == 4096)
                {
                    Console.WriteLine("Emitted!!!");
                    result.AppendBits(CLEAR_TABLE, currentCodeLength);
                    buffer = Array.Empty<byte>();
                    (currentCodeValue, currentCodeLength) = ClearCodewords();
                    continue;
                }
                
                buffer = new[] {b};
                currentCodeLength += currentCodeValue switch
                {
                    512 or 1024 or 2048 => 1,
                    _ => 0
                };
            }
        }
        result.AppendBits(GetCodeword(buffer), currentCodeLength);
        result.AppendBits(EOD, currentCodeLength); // insert end of data

        return result.ToByteArray();
    }

    private static int GetBits(BitList inputBits, int offset, int amount)
    {
        var bits = inputBits.ReadBits(offset, amount).PadLeft(4, (byte) 0);
        if (BitConverter.IsLittleEndian)
            bits = bits.Reverse();
        return BitConverter.ToInt32(bits.ToArray());
    }

    public IEnumerable<byte> Decode(IEnumerable<byte> input)
    {
        var (currentCodeValue, currentCodeLength) = ClearValues();
        var currentBitOffset = 0;

        var inputBits = new BitList(input);
        var result = new List<byte>();
        
        var priorCodeWord = GetBits(inputBits, currentBitOffset, currentCodeLength);
        // output prior code word
        result.AddRange(GetValue(priorCodeWord));
        
        currentBitOffset += currentCodeLength;
        while (currentBitOffset < inputBits.Count)
        {
            var codeword = GetBits(inputBits, currentBitOffset, currentCodeLength);
            
            if (codeword == EOD)
            {
                break;
            }
            
            if (codeword == CLEAR_TABLE)
            {
                currentBitOffset += currentCodeLength;
                (currentCodeValue, currentCodeLength) = ClearValues();
                priorCodeWord = GetBits(inputBits, currentBitOffset, currentCodeLength);
                result.AddRange(GetValue(priorCodeWord));
                currentBitOffset += currentCodeLength;
                continue;
            }

            if (ContainsValue(codeword))
            {
                InsertValue(currentCodeValue, GetValue(priorCodeWord).Append(GetValue(codeword)[0]).ToArray());
                currentCodeValue += 1;

                result.AddRange(GetValue(codeword));
            }
            else
            {
                InsertValue(currentCodeValue, GetValue(priorCodeWord).Append(GetValue(priorCodeWord)[0]).ToArray());
                currentCodeValue += 1;

                result.AddRange(GetValue(priorCodeWord).Append(GetValue(priorCodeWord)[0]));
            }
            priorCodeWord = codeword; 
            currentBitOffset += currentCodeLength;
            if (currentCodeValue < 4096)
            {
                currentCodeLength += currentCodeValue switch
                {
                    511 or 1023 or 2047 => 1,
                    _ => 0
                };
            }
        }
        
        return result;
    }
}