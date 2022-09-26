using System;
using BlastIMG;
using BlastIMG.ImageLoaders;

namespace ShowCase;

public class ImageParsingExample
{
    public static void Run(string imagePath)
    {
        Console.WriteLine($"Loading: {imagePath}");
        var loaded = QoiLoader.Load(imagePath);
    }
}