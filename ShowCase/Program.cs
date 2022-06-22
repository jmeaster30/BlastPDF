using BlastPDF;

namespace ShowCase
{
  class Program
  {
    static void Main(string[] args)
    {
      System.Console.WriteLine("STARTING....");
      var pdf = PDF.LoadFromFile("../Samples/pdf-sample.pdf");

//       var pdf = PDF.LoadFromString(@"
// % PDF 1.7
// 1 0 obj
//   << /wow 5 /test 234 /fart [1 2 3]>>
// stream
//   oh fuck yean baby steak monday
// endstream
// endobj
// ");

      pdf.printAllObjects();
    }
  }
}
