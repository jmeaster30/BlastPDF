
using System;
using System.Collections.Generic;
using System.Linq;
using BlastPDF.Internal.Exceptions;
using BlastPDF.Internal.Structure;

namespace BlastPDF.Internal
{
  public class Parser
  {
    private Lexer lexer { get; }
    public Parser(Lexer l)
    {
      lexer = l;
    }

    public PdfObject ParseObject()
    {
      while (true)
      {
        var token = lexer.GetToken();

        if (token.Type is TokenType.EOF) return null;

        switch (token.Lexeme)
        {
          case '%':
            return ParseComment();
          case '/':
            return ParseName();
          case '+' or '-' or '.' or >= '0' and <= '9':
            return ParseNumbers();
          case 't' or 'f':
            return ParseBooleans();
          case 'n':
            return ParseNull();
          case '(':
            return ParseLiteralString();
          case '<':
            return ParseHexString(); // need to change
          case '[':
            return ParseArray();
          default:
            if (token.Lexeme is '\n' or '\r')
              ParseEndOfLine();
            else if (token.Type is TokenType.WHITESPACE)
              ParseWhitespace();
            else
            {
              Console.WriteLine($"Unknown token: '{token.Lexeme}'");
              lexer.ConsumeToken();
            }

            break;
        }
      }
    }

    public void ParseWhitespace()
    {
      var token = lexer.GetToken();
      var anyWhitespace = false;
      
      while (token.Type is not TokenType.EOF && token.Type is TokenType.WHITESPACE)
      {
        anyWhitespace = true;
        lexer.ConsumeToken();
        token = lexer.GetToken();
      }

      if (!anyWhitespace)
        throw new PdfParseException($"Expected some whitespace but instead found '{token.Lexeme}'");
    }

    public void ParseEndOfLine()
    {
      var token = lexer.GetToken();

      if (token.Lexeme != '\r' && token.Lexeme != '\n')
        throw new PdfParseException($"Expected to find end of line but instead found '{token.Lexeme}'.");

      if (token.Lexeme == '\n')
      {
        lexer.ConsumeToken();
      }
      else // token.Lexeme == '\r'
      {
        lexer.ConsumeToken();
        if (lexer.GetToken().Lexeme == '\n')
          lexer.ConsumeToken();
      }
    }

    public void ParseOptionalWhiteSpace()
    {
      var token = lexer.GetToken();
      while (token.Type is not TokenType.EOF && token.Type is TokenType.WHITESPACE)
      {
        lexer.ConsumeToken();
        token = lexer.GetToken();
      }
    }

    public PdfComment ParseComment()
    {
      var comment = new List<Token>();

      if (lexer.GetToken().Lexeme == '%')
      {
        comment.Add(lexer.GetToken());
        lexer.ConsumeToken();
        while (lexer.GetToken().Lexeme is not ('\r' or '\n') && lexer.GetToken().Type is not TokenType.EOF)
        {
          comment.Add(lexer.GetToken());
          lexer.ConsumeToken();
        }
      }
      else
      {
        throw new PdfParseException($"Expected a % but got a {lexer.GetToken().Lexeme}.");
      }
      
      return new PdfComment(comment);
    }

    public PdfNumeric ParseNumbers()
    {
      var value = new List<Token>();
      bool isReal;
      List<Token> nums;

      var startToken = lexer.GetToken();
      switch (startToken.Lexeme)
      {
        case '+' or '-':
          value.Add(startToken);
          lexer.ConsumeToken();
          (nums, isReal) = ConsumeNumbers(lexer);
          break;
        case '.':
        case >= '0' and <= '9':
          (nums, isReal) = ConsumeNumbers(lexer);
          break;
        default:
          throw new PdfParseException($"Expected +, -, ., or a digit but found '{startToken.Lexeme}'");
      }
      
      value.AddRange(nums);

      return new PdfNumeric(value, isReal);
    }

    private (List<Token>, bool) ConsumeNumbers(Lexer lexer)
    {
      var nums = new List<Token>();
      var point = false;
      var token = lexer.GetToken();
      while (token.Type is not TokenType.EOF)
      {
        if (token.Lexeme == '.' && !point)
          point = true;
        else if (token.Lexeme == '.' && point || token.Lexeme is < '0' or > '9')
          break;
        
        nums.Add(token);
        lexer.ConsumeToken();
        token = lexer.GetToken();
      }

      if (!nums.Any() || nums.Count == 1 && nums.First().Lexeme == '.')
        throw new PdfParseException(
          "Expected to find some numbers to parse but only found nothing or a single decimal point.");

      return (nums, point);
    }
    
    public PdfBoolean ParseBooleans()
    {
      var value = lexer.TryGetTokens("true");
      if (!value.Any()) value = lexer.TryGetTokens("false");
      if (!value.Any()) throw new PdfParseException($"Expected true or false but found '{lexer.GetToken().Lexeme}'({lexer.GetToken().Type})");
      return new PdfBoolean(value.Count() == 4);
    }

    public PdfNull ParseNull()
    {
      var value = lexer.TryGetTokens("null");
      if (!value.Any()) throw new PdfParseException($"Expected null but found '{lexer.GetToken().Lexeme}'({lexer.GetToken().Type})");
      return new PdfNull("null");
    }

    public PdfName ParseName()
    {
      var value = new List<Token>();

      var token = lexer.GetToken();
      if (token.Lexeme != '/')
        throw new PdfParseException($"Expected / but found '{token.Lexeme}'.");
      
      value.Add(token);
      lexer.ConsumeToken();
      token = lexer.GetToken();

      while (token.Type is TokenType.REGULAR && token.Lexeme is >= '!' and <= '~')
      {
        value.Add(token);
        lexer.ConsumeToken();
        token = lexer.GetToken();
      }

      return new PdfName(value);
    }

    public PdfLiteralString ParseLiteralString()
    {
      var value = new List<Token>();

      var depth = 0;
      var escape = false;
      var token = lexer.GetToken();
      
      if (token.Lexeme != '(') throw new PdfParseException($"Expected ( but found '{lexer.GetToken().Lexeme}'");

      value.Add(token);
      lexer.ConsumeToken();
      token = lexer.GetToken();

      while (token.Type is not TokenType.EOF)
      {
        if (token.Lexeme == '(' && !escape)
        {
          depth += 1;
        }
        else if (token.Lexeme == ')' && !escape && depth != 0)
        {
          depth -= 1;
        }
        else if (token.Lexeme == ')' && !escape && depth == 0)
        {
          value.Add(token);
          lexer.ConsumeToken();
          break;
        }

        if (token.Lexeme == '\\' && !escape)
          escape = true;
        else if (escape)
          escape = false;
        
        value.Add(token);
        lexer.ConsumeToken();
        token = lexer.GetToken();
      }
      
      if (value.Last().Lexeme != ')' || value.Last().Lexeme == ')' && depth != 0)
        throw new PdfParseException("Unclosed string. Make sure parentheses are properly balanced.");

      return new PdfLiteralString(value);
    }

    public PdfHexString ParseHexString()
    {
      var value = new List<Token>();

      var token = lexer.GetToken();
      
      if (token.Lexeme != '<') throw new PdfParseException($"Expected < but found '{lexer.GetToken().Lexeme}'");
      
      value.Add(token);
      lexer.ConsumeToken();
      token = lexer.GetToken();

      while (token.Type is not TokenType.EOF)
      {
        if (!IsHex(token.Lexeme) && !IsWhiteSpace(token.Lexeme)) break;

        value.Add(token);
        lexer.ConsumeToken();
        token = lexer.GetToken();
      }

      if (token.Lexeme == '>')
      {
        value.Add(token);
        lexer.ConsumeToken();
      }
      else
      {
        throw new PdfParseException($"Expected a > but found a {token.Lexeme}");
      }

      return new PdfHexString(value);
    }
    
    private static bool IsHex(char c) => c is >= '0' and <= '9' or >= 'a' and <= 'f' or >= 'A' and <= 'F';

    private static bool IsWhiteSpace(char c) => c is ' ' or '\t' or '\n' or '\f' or '\r' or '\0';

    public PdfArray ParseArray()
    {
      var values = new List<PdfObject>();
      var token = lexer.GetToken();
      var endArray = false;

      if (token.Lexeme != '[')
        throw new PdfParseException($"Expected [ but found '{token.Lexeme}'");

      lexer.ConsumeToken();
      token = lexer.GetToken();
      while (token.Type is not TokenType.EOF)
      {
        if (token.Lexeme is ']')
        {
          lexer.ConsumeToken();
          endArray = true;
          break;
        }
        
        values.Add(ParseObject());
        ParseOptionalWhiteSpace();
        token = lexer.GetToken();
      }

      if (!endArray)
        throw new PdfParseException("Unclosed array!!");

      return new PdfArray(values);
    }
  }
}