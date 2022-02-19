
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.ExceptionServices;
using BlastPDF.Internal.Exceptions;
using BlastPDF.Internal.Filters;
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

    public void ParseWhitespace()
    {
      
    }

    public void ParseEndOfLine()
    {
       
    }

    public PdfComment ParseComment()
    {
      var comment = new List<Token>();

      if (lexer.GetToken().Lexeme == "%")
      {
        comment.Add(lexer.GetToken());
        lexer.ConsumeToken();
        while (lexer.GetToken().Lexeme is not ("\r" or "\n"))
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

    public PdfLiteralString ParseLiteralString()
    {
      var value = new List<Token>();

      var depth = 0;
      var escape = false;
      var token = lexer.GetToken();
      
      if (token.Lexeme != "(") throw new PdfParseException($"Expected ( but found '{lexer.GetToken().Lexeme}'");
      depth += 1;
      
      value.Add(token);
      lexer.ConsumeToken();
      token = lexer.GetToken();

      while (token.Type is not TokenType.EOF)
      {
        if (token.Lexeme == "(" && !escape)
        {
          depth += 1;
        }
        else if (token.Lexeme == ")" && !escape && depth != 0)
        {
          depth -= 1;
        }
        else if (token.Lexeme == ")" && !escape && depth == 0)
        {
          value.Add(token);
          lexer.ConsumeToken();
          break;
        }

        if (token.Lexeme == "\\" && !escape)
          escape = true;
        else if (escape)
          escape = false;
        
        value.Add(token);
        lexer.ConsumeToken();
        token = lexer.GetToken();
      }

      if (value.Last().Lexeme != ")" && depth == 0)
        throw new PdfParseException("Unclosed string. Make sure parentheses are properly balanced.");

      return new PdfLiteralString(value);
    }

    public PdfHexString ParseHexString()
    {
      var value = new List<Token>();

      var token = lexer.GetToken();
      
      if (token.Lexeme != "<") throw new PdfParseException($"Expected < but found '{lexer.GetToken().Lexeme}'");
      
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

      if (token.Lexeme == ">")
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
    
    private bool IsHex(string c) => string.CompareOrdinal(c, "0") >= 0 && string.CompareOrdinal(c, "9") <= 0 ||
                                    string.CompareOrdinal(c, "a") >= 0 && string.CompareOrdinal(c, "f") <= 0 ||
                                    string.CompareOrdinal(c, "A") >= 0 && string.CompareOrdinal(c, "F") <= 0;

    private bool IsWhiteSpace(string c) => string.CompareOrdinal(c, " ") == 0 || string.CompareOrdinal(c, "\t") == 0 ||
                                           string.CompareOrdinal(c, "\n") == 0 || string.CompareOrdinal(c, "\f") == 0 ||
                                           string.CompareOrdinal(c, "\r") == 0 || string.CompareOrdinal(c, "\0") == 0;
  }
}