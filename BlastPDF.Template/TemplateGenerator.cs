﻿using Microsoft.CodeAnalysis;
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
                (name: Path.GetFileNameWithoutExtension(file.Path), content: file.GetText(cancellationToken)?.ToString()));
        
        context.RegisterSourceOutput(templates, (spc, template) =>
        {
            spc.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(
                    "asdf",
                    "sadf",
                    "kfjs",
                    "lskdjf",
                    DiagnosticSeverity.Warning,
                    true
                ), Location.Create("BlastPDF.Template/TemplateGenerator.cs", TextSpan.FromBounds(0, 10), new LinePositionSpan())));
            spc.AddSource($"BlastPDF.Template.{template.name}.g.cs", $@"
using System;

public class {template.name} {{
    public static void OHYEAH() {{
        Console.WriteLine(""Okay I got the source generator working...."");
    }}
}}

");
        });
        
    }
}