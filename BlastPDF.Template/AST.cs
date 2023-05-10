using Microsoft.CodeAnalysis;

namespace BlastPDF.Template;

public interface IValue
{
    public string ToString();
    public string GenerateSource();
}

public class Error : IValue
{
    public Token ErrorToken { get; set; }
    public string Message { get; set; }
    public DiagnosticSeverity Severity { get; set; }

    public new string ToString()
    {
        return $"(ERROR {Severity} '{Message.Replace("\"", "\"\"")}' '{ErrorToken.Lexeme.Replace("\"", "\"\"")}')";
    }

    public string GenerateSource()
    {
        return "";
    }
}

public class Object : IValue
{
    public Token Name { get; set; }
    public ArgumentVector? ArgumentList { get; set; }
    public ObjectBody Body { get; set; }
    
    public new string ToString()
    {
        return $"(Object '{Name.Lexeme.Replace("\"", "\"\"")}' {ArgumentList?.ToString() ?? "(No Args)"} {Body.ToString()})";
    }
    
    public string GenerateSource()
    {
        var result = "";
        if (Name.Lexeme == "Document")
        {
            result = "PdfDocument.Create()";
            // turn argument list into metadata dictonary
            result += Body.GenerateSource();
            return result;
        }
        return result;
    }
}

public class Literal : IValue
{
    public Token Value { get; set; }
    
    public new string ToString()
    {
        return $"(Value {Value.Type} '{Value.Lexeme.Replace("\"", "\"\"")}')";
    }
    
    public string GenerateSource()
    {
        return "";
    }
}
 
public interface IArgumentValue
{
    public Token? Name { get; set; }
    public Token? Colon { get; set; }
    public string ToString();
    public string GenerateSource();
}

public class ObjectBody
{
    public Token OpenBrace { get; set; }
    public List<IValue> Values { get; set; }
    public Token CloseBrace { get; set; }
    
    public new string ToString()
    {
        return Values.Aggregate("(Body ", (current, value) => current + value.ToString() + " ") + ")";
    }
    
    public string GenerateSource()
    {
        return "";
    }
}

public class ArgumentVector : IArgumentValue
{
    public Token? Name { get; set; }
    public Token? Colon { get; set; }
    public Token OpenParen { get; set; }
    public List<IArgumentValue> ArgumentValues { get; set; }
    public Token CloseParen { get; set; }
    
    public new string ToString()
    {
        if (Name == null)
        {
            return ArgumentValues.Aggregate("(Args ", (current, value) => current + value.ToString() + " ") + ")";
        }

        return ArgumentValues.Aggregate($"({Name.Lexeme.Replace("\"", "\"\"")} : ",
            (current, value) => current + value.ToString() + " ") + ")";
    }
    
    public string GenerateSource()
    {
        return "";
    }
}

public class ArgumentScalar : IArgumentValue
{
    public Token? Name { get; set; }
    public Token? Colon { get; set; }
    public Token Value { get; set; }
    
    public new string ToString()
    {
        return $"({Name.Lexeme.Replace("\"", "\"\"")} : {Value.Lexeme.Replace("\"", "\"\"")})";
    }
    
    public string GenerateSource()
    {
        return "";
    }
}

public static class AstExtensions
{
    public static string String(this IEnumerable<IValue> values)
    {
        return values.Aggregate("(", (current, value) => current + value.ToString() + " ") + ")";
    }
}
