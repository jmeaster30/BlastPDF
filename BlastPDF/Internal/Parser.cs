
using System;
using System.Collections.Generic;
using System.Linq;
using BlastPDF.Internal.Exceptions;
using BlastPDF.Internal.Structure;
using BlastPDF.Internal.Structure.File;
using BlastPDF.Internal.Structure.Objects;

namespace BlastPDF.Internal
{
  public class Parser
  {
    private Lexer Lexer { get; }

    private Stack<PdfObject> Stack { get; }

    public Parser(Lexer l)
    {
      Lexer = l;
      Stack = new Stack<PdfObject>();
    }

    public IEnumerable<PdfObject> Parse()
    {
      while (Lexer.GetToken().Type is not TokenType.EOF)
      {
        var token = Lexer.GetToken();
        switch (token.Type)
        {
          case TokenType.NAME:
            Stack.Push(new PdfName(token));
            Lexer.ConsumeToken();
            break;
          case TokenType.COMMENT:
            Stack.Push(new PdfComment(token));
            Lexer.ConsumeToken();
            break;
          case TokenType.INTEGER or TokenType.REAL:
            Stack.Push(new PdfNumeric(token));
            Lexer.ConsumeToken();
            break;
          case TokenType.BOOLEAN:
            Stack.Push(new PdfBoolean(token));
            Lexer.ConsumeToken();
            break;
          case TokenType.LITERAL:
            Stack.Push(new PdfLiteralString(token));
            Lexer.ConsumeToken();
            break;
          case TokenType.HEX:
            Stack.Push(new PdfHexString(token));
            Lexer.ConsumeToken();
            break;
          case TokenType.NULL:
            Stack.Push(new PdfNull(token));
            Lexer.ConsumeToken();
            break;
          case TokenType.WHITESPACE or TokenType.EOL:
            ConsumeOptionalWhitespace();
            break;
          case TokenType.ARRAY_OPEN:
            Stack.Push(new PdfObject(PdfObjectType.ARRAY_START));
            Lexer.ConsumeToken();
            break;
          case TokenType.DICT_OPEN:
            Stack.Push(new PdfObject(PdfObjectType.DICT_START));
            Lexer.ConsumeToken();
            break;
          case TokenType.ARRAY_CLOSE:
            ParseArray();
            break;
          case TokenType.DICT_CLOSE:
            //ParseDictionary();
            Lexer.ConsumeToken();
            break;
          case TokenType.KEYWORD:
            Lexer.ConsumeToken();
            break;
          case TokenType.OPERATOR:
            Lexer.ConsumeToken();
            break;
        }
      }
      
      var firstInvalidToken =
        Stack.FirstOrDefault(x => x.ObjectType is PdfObjectType.ARRAY_START or PdfObjectType.DICT_START);
      if (firstInvalidToken is not null)
        throw new PdfParseException(
          $"Unclosed {(firstInvalidToken.ObjectType is PdfObjectType.ARRAY_START ? "array" : "dictionary")} :(");
      
      return Stack.Reverse().ToList();
    }

    public void ConsumeOptionalWhitespace()
    {
      var token = Lexer.GetToken();
      while (token.Type is not TokenType.EOF)
      {
        if (token.Type is not (TokenType.WHITESPACE or TokenType.EOL))
          break;
        Lexer.ConsumeToken();
        token = Lexer.GetToken();
      }
    }

    public void ParseArray()
    {
      Lexer.ConsumeToken();
      var values = new List<PdfObject>();
      while (Stack.Any() && Stack.Peek().ObjectType != PdfObjectType.ARRAY_START)
      {
        var val = Stack.Pop();
        if (val.ObjectType > PdfObjectType.INDIRECT_REF)
          throw new PdfParseException("Invalid object type in array :(");
        values.Insert(0, val);
      }

      if (!Stack.Any() || Stack.Peek().ObjectType != PdfObjectType.ARRAY_START)
        throw new PdfParseException("Found end of the array but could not find where the array started :(");

      Stack.Pop(); // pop array start
      Stack.Push(new PdfArray(values));
    }

    public void ParseDictionary()
    {
      /*var dictionary = new Dictionary<PdfName, PdfObject>();
      var foundEnd = false;
      var start = Lexer.GetToken();
      if (start.Type is not TokenType.DICT_OPEN) throw new PdfParseException($"Expected to find << but only found '{Lexer.GetToken()}'");
      Lexer.ConsumeToken();
      
      var token = Lexer.GetToken();
      while (token.Type is not TokenType.EOF)
      {
        if (token.Type is TokenType.DICT_CLOSE)
        {
          Lexer.ConsumeToken();
          foundEnd = true;
          break;
        }

        ConsumeOptionalWhitespace();
        if (Lexer.GetToken().Type is not TokenType.NAME)
          throw new PdfParseException($"Expected to find a name but found '{Lexer.GetToken().Lexeme}'");
        var name = new PdfName(Lexer.GetToken());
        Lexer.ConsumeToken();

        ConsumeOptionalWhitespace();
        if (Lexer.GetToken().Type is TokenType.EOF or TokenType.DICT_CLOSE)
          throw new PdfParseException($"Expected to find a value but found '{Lexer.GetToken().Lexeme}'");
        var value = ParseObject();

        ConsumeOptionalWhitespace();
        dictionary.Add(name, value);
        token = Lexer.GetToken();
      }

      if (!foundEnd)
        throw new PdfParseException("Unclosed dictionary!!!!");
      
      return new PdfDictionary(dictionary);*/
    }
  }
}