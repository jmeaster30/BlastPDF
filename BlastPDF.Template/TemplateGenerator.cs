using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace BlastPDF.Template;

[Generator]
public class TemplateGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var templates = context.AdditionalTextsProvider
            .Where(static file => file.Path.EndsWith(".bxpdf"))
            .Select(static (file, cancellationToken) => 
                (path: file.Path, name: Path.GetFileNameWithoutExtension(file.Path), content: file.GetText(cancellationToken)?.ToString() ?? ""));
        
        context.RegisterSourceOutput(templates, (spc, template) =>
        {
            var ast = Parser.Parse(template.content);

            var errors = ast.GetErrors(template.path).ToList();
            if (errors.Count != 0)
            {
                errors.ForEach(spc.ReportDiagnostic);
                return;
            }

            spc.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(
                    "BLASTPDF",
                    "BlastPDF Template Error",
                    "NO ERRORS ?????????",
                    "BlastPDF Template Error",
                    DiagnosticSeverity.Error,
                    true),
                Location.Create(
                    template.path,
                    new TextSpan(0, 1),
                    new LinePositionSpan(
                        new LinePosition(0, 0),
                        new LinePosition(0, 0)))));
        });
        
    }
}