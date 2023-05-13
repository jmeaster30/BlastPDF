using Microsoft.CodeAnalysis;

namespace BlastPDF.Template;

public interface IValue
{
    public string ToString();
    public string GenerateSource();
    public bool IsObjectOfType(string objectName);
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

    public bool IsObjectOfType(string s)
    {
        return s == "Error";
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
    
    public bool IsObjectOfType(string s)
    {
        return Name.Lexeme == s;
    }
    
    public string GenerateSource()
    {
        var result = "";
        if (Name.Lexeme == "Document")
        {
            result = "PdfDocument.Create()";
            // turn argument list into metadata dictonary
            if (ArgumentList is not null)
            {
                result += ".AddMetadata(\n";
                result += ArgumentList.GenerateDocumentMetadata();
                result += "\n)";
            }
            result += Body.GenerateSource();
            return result;
        }
        else if (Name.Lexeme == "Imports")
        {
            foreach (var content in Body.Values)
            {
                // TODO need to error instead of just ignoring invalid objects
                if (content is Literal literal && literal.Value.Type == TokenType.EmbeddedExpression)
                {
                    result += $"using {string.Join("", literal.Value.Lexeme.Skip(2).Take(literal.Value.Lexeme.Length - 3))};\n";
                }
            }
        }
        else if (Name.Lexeme == "Variables")
        {
            foreach (var content in Body.Values)
            {
                // TODO need to error instead of just ignoring invalid objects
                if (content is Literal literal && literal.Value.Type == TokenType.EmbeddedExpression)
                {
                    result += $"public {string.Join("", literal.Value.Lexeme.Skip(2).Take(literal.Value.Lexeme.Length - 3))} {{ get; set; }}\n";
                }
            }
        }
        else if (Name.Lexeme == "Namespace")
        {
            var namespaceLiteral = Body.Values.Single(x => x.IsObjectOfType("Literal") && (x as Literal)?.Value.Type == TokenType.EmbeddedExpression);
            if (namespaceLiteral is Literal literal)
            {
                result = $"namespace {string.Join("", literal.Value.Lexeme.Skip(2).Take(literal.Value.Lexeme.Length - 3))};";
            }
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
    
    public bool IsObjectOfType(string s)
    {
        return s == "Literal";
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
    public string GenerateDocumentMetadata();
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

    public string GenerateDocumentMetadata()
    {
        var value = "new Dictionary<string, IPdfValue>{\n";
        for (var i = 0; i < ArgumentValues.Count; i++)
        {
            value += ArgumentValues[i].GenerateDocumentMetadata();
            if (i < ArgumentValues.Count - 1)
            {
                value += ",";
            }

            value += "\n";
        }
        value += "}\n";

        var result = "";
        if (Name is null)
        {
            result = value;
        }
        else
        {
            result = $"{{\"{Name.Lexeme}\", ({value}).ToPdfValue()}}";
        }

        return result;
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

    public string GenerateDocumentMetadata()
    {
        var value = Value.Lexeme;
        if (Value.Type == TokenType.EmbeddedExpression)
        {
            value = string.Join("", Value.Lexeme.Skip(2).Take(Value.Lexeme.Length - 3));
        }
        else if (Value.Type == TokenType.String)
        {
            value = "$\"";
            var stringIndex = 1;
            while (stringIndex < Value.Lexeme.Length - 1)
            {
                switch (Value.Lexeme[stringIndex])
                {
                    case '"':
                        value += "\\\"";
                        break;
                    case '@':
                    {
                        stringIndex += 1;
                        while (stringIndex < Value.Lexeme.Length - 1 && Value.Lexeme[stringIndex] != '}')
                        {
                            value += Value.Lexeme[stringIndex];
                            stringIndex += 1;
                        }
                        value += Value.Lexeme[stringIndex];
                        break;
                    }
                    default:
                        value += Value.Lexeme[stringIndex];
                        break;
                }
                stringIndex += 1;
            }
            value += '"';
        }

        return $"{{\"{Name.Lexeme}\", ({value}).ToPdfValue()}}";
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
