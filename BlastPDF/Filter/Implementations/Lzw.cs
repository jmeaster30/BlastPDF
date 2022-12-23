using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BlastSharp.Lists;

namespace BlastPDF.Filter.Implementations;

public class LzwParameters : IFilterParameters
{
    public int Predictor { get; set; }
    public int Colors { get; set; }
    public int BitsPerComponent { get; set; }
    public int Columns { get; set; }
    public int EarlyChange { get; set; }
}

public class Lzw : IFilterAlgorithm
{
    private LzwParameters LzwParameters { get; } = new()
    {
        Predictor = 1,
        Colors = 1,
        BitsPerComponent = 8,
        Columns = 1,
        EarlyChange = 1,
    };

    private Dictionary<byte[], int> codewords { get; set; } = new();

    public Lzw(LzwParameters parameters)
    {
        if (parameters != null)
        {
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
    }

    private int GetCodeword(byte[] input)
    {
        return codewords[input];
    }

    private bool ContainsCodeword(byte[] input)
    {
        return codewords.ContainsKey(input);
    }

    private void InsertCodeword(byte[] input, int code)
    {
        codewords[input] = code;
    }
    
    public IEnumerable<byte> Encode(IEnumerable<byte> input)
    {
        // table: code -> bits, value
        for (int i = 0; i < 256; i++)
        {
            InsertCodeword(new[] {(byte) i}, i);
        }
        // 256 is the clear table marker
        // 257 is the EOD marker
        var currentCodeValue = 258;
        var currentCodeLength = 9;

        var buffer = new[]{input.First()};
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
                
                // TODO Emit a clear table marker when we hit the max code value
                if (currentCodeValue < 4096)
                {
                    InsertCodeword(combined, currentCodeValue);
                    currentCodeValue += 1;
                }
                
                
                buffer = new[] {b};
                if (currentCodeValue < 4096)
                {
                    currentCodeLength += currentCodeValue switch
                    {
                        512 or 1024 or 2048 => 1,
                        _ => 0
                    };
                }
            }
        }
        result.AppendBits(GetCodeword(buffer), currentCodeLength);

        return result.ToByteArray();
    }

    public IEnumerable<byte> Decode(IEnumerable<byte> input)
    {
        // enter all letters into table
        for (int i = 0; i < 256; i++)
        {
            InsertCodeword(new[] {(byte) i}, i);
        }
        // 256 is the clear table marker
        // 257 is the EOD marker
        var currentCodeValue = 258;
        var currentCodeLength = 9;
        
        // read prior code word
        // output prior code word
        // foreach codeword in input
        //      if codeword is not in table 
        //          enter in table string(priorcodeword) + firstchar(string(priorcodeword))
        //          output string(priorcodeword) + firstchar(string(priorcodeword))
        //      else
        //          enter in table string(priorcodeword) + firstchar(string(codeword))
        //          output string(codeword)
        //      priorcodeword = codeword
        throw new System.NotImplementedException();
    }
}