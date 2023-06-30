using System;
using BlastIMG;
using BlastIMG.ImageLoaders;

namespace ShowCase;

public class ImageParsingExample
{
    public static void Run(string imagePath)
    {
        Console.WriteLine($"Loading: {imagePath}");
        var loaded = GifLoader.Load(imagePath);
        foreach (var pix in loaded.Pixels)
        {
            Console.WriteLine($"X: {pix.X} Y: {pix.Y} R: {pix.R} G: {pix.G} B: {pix.B} A: {pix.A}");
        }
    }
}