using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace BlastPDF.Template;

public interface IAstNode
{
    public string GenerateSource();
    public bool Is<T>();
    public IEnumerable<Diagnostic> GetErrors(string filepath);
}

public interface IDocumentNode : IAstNode {}
public interface IPageNode : IAstNode {}
public interface IExpressionNode : IAstNode {}

public class DocumentError : IDocumentNode
{
    public List<Token> ErroredTokens { get; set; }
    public string Message { get; set; }
    public DiagnosticSeverity Severity { get; set; }

    public string GenerateSource()
    {
        return "ERROR";
    }

    public bool Is<T>()
    {
        return typeof(T) == typeof(DocumentError);
    }

    public IEnumerable<Diagnostic> GetErrors(string filepath)
    {
        var start = ErroredTokens.FirstOrDefault();
        var end = ErroredTokens.LastOrDefault();
        
        var textSpanStart = start?.Offset.Item1 ?? 0;
        var textSpanEnd = end?.Offset.Item2 ?? 0;

        var lineStart = start == null ? 0 : start.Line.Item1 - 1;
        var lineEnd = end == null ? 0 : end.Line.Item2 - 1;
        var columnStart = start == null ? 0 : start.Column.Item1 - 1;
        var columnEnd = end == null ? 0 : end.Column.Item2 - 1;

        return new List<Diagnostic>
        {
            Diagnostic.Create(
                new DiagnosticDescriptor(
                    "BLASTPDF",
                    "BlastPDF Template Error",
                    Message,
                    "BlastPDF Template Error",
                    Severity,
                    true),
                Location.Create(
                    filepath,
                    new TextSpan(textSpanStart, textSpanEnd - textSpanStart),
                    new LinePositionSpan(
                        new LinePosition(lineStart, columnStart),
                        new LinePosition(lineEnd, columnEnd))))
        };
    }
}

public class PageError : IPageNode
{
    public List<Token> ErroredTokens { get; set; }
    public string Message { get; set; }
    public DiagnosticSeverity Severity { get; set; }
    public string GenerateSource()
    {
        throw new NotImplementedException();
    }

    public bool Is<T>()
    {
        return typeof(T) == typeof(PageError);
    }

    public IEnumerable<Diagnostic> GetErrors(string filepath)
    {
        var start = ErroredTokens.FirstOrDefault();
        var end = ErroredTokens.LastOrDefault();
        
        var textSpanStart = start?.Offset.Item1 ?? 0;
        var textSpanEnd = end?.Offset.Item2 ?? 0;

        var lineStart = start == null ? 0 : start.Line.Item1 - 1;
        var lineEnd = end == null ? 0 : end.Line.Item2 - 1;
        var columnStart = start == null ? 0 : start.Column.Item1 - 1;
        var columnEnd = end == null ? 0 : end.Column.Item2 - 1;

        return new List<Diagnostic>
        {
            Diagnostic.Create(
                new DiagnosticDescriptor(
                    "BLASTPDF",
                    "BlastPDF Template Error",
                    Message,
                    "BlastPDF Template Error",
                    Severity,
                    true),
                Location.Create(
                    filepath,
                    new TextSpan(textSpanStart, textSpanEnd - textSpanStart),
                    new LinePositionSpan(
                        new LinePosition(lineStart, columnStart),
                        new LinePosition(lineEnd, columnEnd))))
        };
    }
}

public class ExpressionError : IExpressionNode
{
    public List<Token> ErroredTokens { get; set; }
    public string Message { get; set; }
    public DiagnosticSeverity Severity { get; set; }
    public string GenerateSource()
    {
        throw new NotImplementedException();
    }

    public bool Is<T>()
    {
        return typeof(T) == typeof(ExpressionError);
    }

    public IEnumerable<Diagnostic> GetErrors(string filepath)
    {
        var start = ErroredTokens.FirstOrDefault();
        var end = ErroredTokens.LastOrDefault();
        
        var textSpanStart = start?.Offset.Item1 ?? 0;
        var textSpanEnd = end?.Offset.Item2 ?? 0;

        var lineStart = start == null ? 0 : start.Line.Item1 - 1;
        var lineEnd = end == null ? 0 : end.Line.Item2 - 1;
        var columnStart = start == null ? 0 : start.Column.Item1 - 1;
        var columnEnd = end == null ? 0 : end.Column.Item2 - 1;

        return new List<Diagnostic>
        {
            Diagnostic.Create(
                new DiagnosticDescriptor(
                    "BLASTPDF",
                    "BlastPDF Template Error",
                    Message,
                    "BlastPDF Template Error",
                    Severity,
                    true),
                Location.Create(
                    filepath,
                    new TextSpan(textSpanStart, textSpanEnd - textSpanStart),
                    new LinePositionSpan(
                        new LinePosition(lineStart, columnStart),
                        new LinePosition(lineEnd, columnEnd))))
        };
    }
}

public class NumberValue : IExpressionNode
{
    public Token Value { get; set; }
    public Token Unit { get; set; }
    public string GenerateSource()
    {
        throw new NotImplementedException();
    }

    public bool Is<T>()
    {
        return typeof(T) == typeof(NumberValue);
    }

    public IEnumerable<Diagnostic> GetErrors(string filepath)
    {
        return new List<Diagnostic>();
    }
}

public class StringValue : IExpressionNode
{
    public Token Value { get; set; }
    public string GenerateSource()
    {
        throw new NotImplementedException();
    }

    public bool Is<T>()
    {
        return typeof(T) == typeof(StringValue);
    }

    public IEnumerable<Diagnostic> GetErrors(string filepath)
    {
        return new List<Diagnostic>();
    }
}

public class ExpressionValue : IExpressionNode
{
    public Token Value { get; set; }
    public string GenerateSource()
    {
        throw new NotImplementedException();
    }

    public bool Is<T>()
    {
        return typeof(T) == typeof(StringValue);
    }

    public IEnumerable<Diagnostic> GetErrors(string filepath)
    {
        return new List<Diagnostic>();
    }
}

public class NamespaceNode : IDocumentNode
{
    public Token NamespaceToken { get; set; }
    public Token Value { get; set; }
    public string GenerateSource()
    {
        throw new NotImplementedException();
    }

    public bool Is<T>()
    {
        return typeof(T) == typeof(NamespaceNode);
    }

    public IEnumerable<Diagnostic> GetErrors(string filepath)
    {
        return new List<Diagnostic>();
    }
}

public class ImportNode : IDocumentNode
{
    public Token ImportToken { get; set; }
    public Token Value { get; set; }
    public string GenerateSource()
    {
        throw new NotImplementedException();
    }

    public bool Is<T>()
    {
        return typeof(T) == typeof(ImportNode);
    }

    public IEnumerable<Diagnostic> GetErrors(string filepaths)
    {
        return new List<Diagnostic>();
    }
}

public class VariableNode : IDocumentNode
{
    public Token VariableToken { get; set; }
    public Token Value { get; set; }
    public string GenerateSource()
    {
        throw new NotImplementedException();
    }

    public bool Is<T>()
    {
        return typeof(T) == typeof(VariableNode);
    }

    public IEnumerable<Diagnostic> GetErrors(string filepath)
    {
        return new List<Diagnostic>();
    }
}

public class TitleNode : IDocumentNode
{
    public Token TitleToken { get; set; }
    public IExpressionNode Expression { get; set; }
    public string GenerateSource()
    {
        throw new NotImplementedException();
    }

    public bool Is<T>()
    {
        return typeof(T) == typeof(TitleNode);
    }

    public IEnumerable<Diagnostic> GetErrors(string filepath)
    {
        return Expression.GetErrors(filepath);
    }
}

public class CreationDateNode : IDocumentNode
{
    public Token CreationDateToken { get; set; }
    public IExpressionNode Expression { get; set; }
    public string GenerateSource()
    {
        throw new NotImplementedException();
    }

    public bool Is<T>()
    {
        return typeof(T) == typeof(CreationDateNode);
    }

    public IEnumerable<Diagnostic> GetErrors(string filepath)
    {
        return Expression.GetErrors(filepath);
    }
}

public class AuthorNode : IDocumentNode
{
    public Token AuthorToken { get; set; }
    public IExpressionNode Expression { get; set; }
    public string GenerateSource()
    {
        throw new NotImplementedException();
    }

    public bool Is<T>()
    {
        return typeof(T) == typeof(AuthorNode);
    }

    public IEnumerable<Diagnostic> GetErrors(string filepath)
    {
        return Expression.GetErrors(filepath);
    }
}

public class LoadNode : IDocumentNode
{
    public Token LoadToken { get; set; }
    public Token TypeToken { get; set; }
    public Token IdentifierToken { get; set; }
    public IExpressionNode? Expression { get; set; }
    public string GenerateSource()
    {
        throw new NotImplementedException();
    }

    public bool Is<T>()
    {
        return typeof(T) == typeof(LoadNode);
    }

    public IEnumerable<Diagnostic> GetErrors(string filepath)
    {
        return Expression?.GetErrors(filepath) ?? new List<Diagnostic>();
    }
}

public class AddPageNode : IDocumentNode
{
    public Token PageToken { get; set; }
    public Token TypeToken { get; set; }
    public List<IPageNode> PageNodes { get; set; }
    public Token EndToken { get; set; }
    public string GenerateSource()
    {
        throw new NotImplementedException();
    }

    public bool Is<T>()
    {
        return typeof(T) == typeof(AddPageNode);
    }

    public IEnumerable<Diagnostic> GetErrors(string filepath)
    {
        return PageNodes.GetErrors(filepath);
    }
}

public class WidthNode : IPageNode
{
    public Token WidthToken { get; set; }
    public IExpressionNode Expression { get; set; }
    public string GenerateSource()
    {
        throw new NotImplementedException();
    }

    public bool Is<T>()
    {
        return typeof(T) == typeof(WidthNode);
    }

    public IEnumerable<Diagnostic> GetErrors(string filepath)
    {
        return new List<Diagnostic>();
    }
}

public class HeightNode : IPageNode
{
    public Token HeightToken { get; set; }
    public IExpressionNode Expression { get; set; }
    public string GenerateSource()
    {
        throw new NotImplementedException();
    }

    public bool Is<T>()
    {
        return typeof(T) == typeof(HeightNode);
    }

    public IEnumerable<Diagnostic> GetErrors(string filepath)
    {
        return new List<Diagnostic>();
    }
}

public class MarginNode : IPageNode
{
    public Token MarginToken { get; set; }
    public Token MarginTypeToken { get; set; }
    public IExpressionNode Expression { get; set; }
    public string GenerateSource()
    {
        throw new NotImplementedException();
    }

    public bool Is<T>()
    {
        return typeof(T) == typeof(MarginNode);
    }

    public IEnumerable<Diagnostic> GetErrors(string filepath)
    {
        return new List<Diagnostic>();
    }
}

public class DpiNode : IPageNode
{
    public Token DpiToken { get; set; }
    public IExpressionNode Expression { get; set; }
    public string GenerateSource()
    {
        throw new NotImplementedException();
    }

    public bool Is<T>()
    {
        return typeof(T) == typeof(DpiNode);
    }

    public IEnumerable<Diagnostic> GetErrors(string filepath)
    {
        return new List<Diagnostic>();
    }
}

public class HeaderNode : IPageNode
{
    public Token HeaderToken { get; set; }
    public Token EndToken { get; set; }
    public string GenerateSource()
    {
        throw new NotImplementedException();
    }

    public bool Is<T>()
    {
        return typeof(T) == typeof(HeaderNode);
    }

    public IEnumerable<Diagnostic> GetErrors(string filepath)
    {
        return new List<Diagnostic>();
    }
}

public class BodyNode : IPageNode
{
    public Token BodyToken { get; set; }
    public Token EndToken { get; set; }
    public string GenerateSource()
    {
        throw new NotImplementedException();
    }

    public bool Is<T>()
    {
        return typeof(T) == typeof(BodyNode);
    }

    public IEnumerable<Diagnostic> GetErrors(string filepath)
    {
        return new List<Diagnostic>();
    }
}

public class FooterNode : IPageNode
{
    public Token FooterToken { get; set; }
    public Token EndToken { get; set; }
    public string GenerateSource()
    {
        throw new NotImplementedException();
    }

    public bool Is<T>()
    {
        return typeof(T) == typeof(FooterNode);
    }

    public IEnumerable<Diagnostic> GetErrors(string filepath)
    {
        return new List<Diagnostic>();
    }
}

public static class AstExtensions
{
    public static IEnumerable<Diagnostic> GetErrors(this IEnumerable<IAstNode> nodes, string filepath)
    {
        return nodes.SelectMany(x => x.GetErrors(filepath));
    }
}