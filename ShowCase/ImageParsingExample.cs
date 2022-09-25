using System;
using BlastIMG;
using BlastIMG.ImageLoaders;

namespace ShowCase;

public class ImageParsingExample
{
    public static void Run(string imagePath)
    {
        var loaded = BmpLoader.Load(imagePath);
        
        Console.WriteLine($"Loaded: {imagePath}");
        Console.WriteLine($"Width: {loaded.Width}");
        Console.WriteLine($"Height: {loaded.Height}");
    }
}