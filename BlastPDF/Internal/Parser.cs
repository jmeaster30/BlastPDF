
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
      var token = lexer.GetToken();
      PdfObject result = null;
      switch (token.Type)
      {
        case TokenType.NAME:
          result = new PdfName(token);
          lexer.ConsumeToken();
          break;
        case TokenType.COMMENT:
          result = new PdfComment(token);
          lexer.ConsumeToken();
          break;
        case TokenType.INTEGER or TokenType.REAL:
          result = new PdfNumeric(token);
          lexer.ConsumeToken();
          break;
        case TokenType.BOOLEAN:
          result = new PdfBoolean(token);
          lexer.ConsumeToken();
          break;
        case TokenType.LITERAL:
          result = new PdfLiteralString(token);
          lexer.ConsumeToken();
          break;
        case TokenType.HEX:
          result = new PdfHexString(token);
          lexer.ConsumeToken();
          break;
        case TokenType.NULL:
          result = new PdfNull(token);
          lexer.ConsumeToken();
          break;
        case TokenType.WHITESPACE or TokenType.EOL:
          ConsumeOptionalWhitespace();
          result = ParseObject();
          break;
        case TokenType.ARRAY_OPEN:
          result = ParseArray();
          break;
        case TokenType.DICT_OPEN:
          result = ParseDictionary();
          break;
      };
      lexer.ConsumeToken();
      return result;
    }

    public void ConsumeOptionalWhitespace()
    {
      var token = lexer.GetToken();
      while (token.Type is not TokenType.EOF)
      {
        if (token.Type is not (TokenType.WHITESPACE or TokenType.EOL))
          break;
        lexer.ConsumeToken();
        token = lexer.GetToken();
      }
    }

    public PdfArray ParseArray()
    {
      var values = new List<PdfObject>();
      var token = lexer.GetToken();
      var endArray = false;

      if (token.Type is not TokenType.ARRAY_OPEN)
        throw new PdfParseException($"Expected [ but found '{token.Lexeme}'");

      lexer.ConsumeToken();
      token = lexer.GetToken();
      while (token.Type is not TokenType.EOF)
      {
        if (token.Type is TokenType.ARRAY_CLOSE)
        {
          lexer.ConsumeToken();
          endArray = true;
          break;
        }

        values.Add(ParseObject());
        ConsumeOptionalWhitespace();
        token = lexer.GetToken();
      }

      if (!endArray)
        throw new PdfParseException("Unclosed array!!");

      return new PdfArray(values);
    }

    public PdfDictionary ParseDictionary()
    {
      var dictionary = new Dictionary<PdfName, PdfObject>();
      var foundEnd = false;
      var start = lexer.GetToken();
      if (start.Type is not TokenType.DICT_OPEN) throw new PdfParseException($"Expected to find << but only found '{lexer.GetToken()}'");
      lexer.ConsumeToken();
      
      var token = lexer.GetToken();
      while (token.Type is not TokenType.EOF)
      {
        if (token.Type is TokenType.DICT_CLOSE)
        {
          lexer.ConsumeToken();
          foundEnd = true;
          break;
        }

        ConsumeOptionalWhitespace();
        if (lexer.GetToken().Type is not TokenType.NAME)
          throw new PdfParseException($"Expected to find a name but found '{lexer.GetToken().Lexeme}'");
        var name = new PdfName(lexer.GetToken());
        lexer.ConsumeToken();

        ConsumeOptionalWhitespace();
        if (lexer.GetToken().Type is TokenType.EOF or TokenType.DICT_CLOSE)
          throw new PdfParseException($"Expected to find a value but found '{lexer.GetToken().Lexeme}'");
        var value = ParseObject();

        ConsumeOptionalWhitespace();
        dictionary.Add(name, value);
        token = lexer.GetToken();
      }

      if (!foundEnd)
        throw new PdfParseException("Unclosed dictionary!!!!");
      
      return new PdfDictionary(dictionary);
    }
  }
}