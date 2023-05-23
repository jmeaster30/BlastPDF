namespace BlastPDF.Template;

public interface IPdfTemplate
{
    public void Save(Stream stream);
}