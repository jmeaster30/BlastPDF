using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace BlastPDF.Template;

public interface IAstNode
{
    public bool Is<T>();
    public IEnumerable<Diagnostic> GetErrors(string filepath);
}

public interface IDocumentNode : IAstNode {}
public interface IPageNode : IAstNode {}
public interface IExpressionNode : IAstNode {}
public interface IContentNode : IAstNode {}

public class DocumentError : IDocumentNode
{
    public List<Token> ErroredTokens { get; set; } = default!;
    public string Message { get; set; } = default!;
    public DiagnosticSeverity Severity { get; set; }

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
    public List<Token> ErroredTokens { get; set; } = default!;
    public string Message { get; set; } = default!;
    public DiagnosticSeverity Severity { get; set; }

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
    public List<Token> ErroredTokens { get; set; } = default!;
    public string Message { get; set; } = default!;
    public DiagnosticSeverity Severity { get; set; } = default!;

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

public class ContentError : IContentNode
{
    public List<Token> ErroredTokens { get; set; } = default!;
    public string Message { get; set; } = default!;
    public DiagnosticSeverity Severity { get; set; } = default!;

    public bool Is<T>()
    {
        return typeof(T) == typeof(ContentError);
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
    public Token Value { get; set; } = default!;
    public Token Unit { get; set; } = default!;

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
    public Token Value { get; set; } = default!;

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
    public Token Value { get; set; } = default!;

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
    public Token NamespaceToken { get; set; } = default!;
    public Token Value { get; set; } = default!;

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
    public Token ImportToken { get; set; } = default!;
    public Token Value { get; set; } = default!;

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
    public Token VariableToken { get; set; } = default!;
    public Token Value { get; set; } = default!;

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
    public Token TitleToken { get; set; } = default!;
    public IExpressionNode Expression { get; set; } = default!;

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
    public Token CreationDateToken { get; set; } = default!;
    public IExpressionNode Expression { get; set; } = default!;

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
    public Token AuthorToken { get; set; } = default!;
    public IExpressionNode Expression { get; set; } = default!;

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
    public Token LoadToken { get; set; } = default!;
    public Token TypeToken { get; set; } = default!;
    public Token IdentifierToken { get; set; } = default!;
    public IExpressionNode? Expression { get; set; } = default!;

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
    public Token PageToken { get; set; } = default!;
    public Token TypeToken { get; set; } = default!;
    public List<IPageNode> PageNodes { get; set; } = default!;
    public Token EndToken { get; set; } = default!;
    
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
    public Token WidthToken { get; set; } = default!;
    public IExpressionNode Expression { get; set; } = default!;

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
    public Token HeightToken { get; set; } = default!;
    public IExpressionNode Expression { get; set; } = default!;

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
    public Token MarginToken { get; set; } = default!;
    public Token MarginTypeToken { get; set; } = default!;
    public IExpressionNode Expression { get; set; } = default!;

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
    public Token DpiToken { get; set; } = default!;
    public IExpressionNode Expression { get; set; } = default!;

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
    public Token HeaderToken { get; set; } = default!;
    public List<IContentNode> Contents { get; set; } = default!;
    public Token EndToken { get; set; } = default!;
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
    public Token BodyToken { get; set; } = default!;
    public List<IContentNode> Contents { get; set; } = default!;
    public Token EndToken { get; set; } = default!;

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
    public Token FooterToken { get; set; } = default!;
    public List<IContentNode> Contents { get; set; } = default!;
    public Token EndToken { get; set; } = default!;

    public bool Is<T>()
    {
        return typeof(T) == typeof(FooterNode);
    }

    public IEnumerable<Diagnostic> GetErrors(string filepath)
    {
        return new List<Diagnostic>();
    }
}

public class TextNode : IContentNode
{
    public Token TextToken { get; set; } = default!;
    public IExpressionNode Expression { get; set; } = default!;

    public bool Is<T>()
    {
        return typeof(T) == typeof(TextNode);
    }

    public IEnumerable<Diagnostic> GetErrors(string filepath)
    {
        return new List<Diagnostic>();
    }
}

public class ImageNode : IContentNode
{
    public Token ImageToken { get; set; } = default!;
    public IExpressionNode Width { get; set; } = default!;
    public IExpressionNode Height { get; set; } = default!;
    public Token Identifier { get; set; }  = default!;
    
    public bool Is<T>()
    {
        return typeof(T) == typeof(ImageNode);
    }

    public IEnumerable<Diagnostic> GetErrors(string filepath)
    {
        return new List<Diagnostic>();
    }
}

/*public class BranchNode : IContentNode, IPageNode, IDocumentNode
{
    
}

public class LoopNode : IContentNode, IPageNode, IDocumentNode
{
    
}*/

public static class AstExtensions
{
    public static IEnumerable<Diagnostic> GetErrors(this IEnumerable<IAstNode> nodes, string filepath)
    {
        return nodes.SelectMany(x => x.GetErrors(filepath));
    }
}