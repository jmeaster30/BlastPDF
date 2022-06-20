using BlastPDF;

namespace ShowCase
{
  class Program
  {
    static void Main(string[] args)
    {
      System.Console.WriteLine("STARTING....");
      var pdf = PDF.LoadFromString(@"
<<
  /Length 5
  /Filter /FlateDecode
>>
stream
oh fuck yeah baby steak monday
endstream
");

      pdf.printAllObjects();
    }
  }
}
