namespace BlastIMG.ImageLoaders;

public interface IImageLoader
{
    public static Image Load(string filename)
    {
        throw new NotSupportedException("Calling static method of IImageLoader");
    }
}