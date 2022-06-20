
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
            ParseDictionary();
            break;
          case TokenType.KEYWORD:
            ParseKeyword();
            break;
          case TokenType.OPERATOR:
            Lexer.ConsumeToken();
            break;
          case TokenType.ERROR:
            Console.WriteLine($"Lexing Error: {token.ErrorMessage}");
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

    private void ConsumeOptionalWhitespace()
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

    private void ParseArray()
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

    private void ParseDictionary()
    {
      var dictionary = new Dictionary<PdfName, PdfObject>();
      var values = new List<PdfObject>();
      
      Lexer.ConsumeToken();

      while (Stack.Any() && Stack.Peek().ObjectType != PdfObjectType.DICT_START)
      {
        var val = Stack.Pop();
        if (val.ObjectType > PdfObjectType.INDIRECT_REF)
          throw new PdfParseException("Invalid object type in array :(");
        values.Insert(0, val);
      }
      
      if (!Stack.Any() || Stack.Peek().ObjectType != PdfObjectType.DICT_START)
        throw new PdfParseException("Found end of the array but could not find where the array started :(");

      var chunks = values.Chunk(2);
      foreach (var chunk in chunks)
      {
        if (chunk.Length != 2)
          throw new PdfParseException("Found dictionary key with no associated value :(");

        var key = chunk[0];
        var value = chunk[1];
        if (key.ObjectType != PdfObjectType.NAME)
          throw new PdfParseException($"Key must be a Name object but got a {key.ObjectType}");
        
        dictionary.Add(key as PdfName, value);
      }
      
      Stack.Pop(); // pop array start
      Stack.Push(new PdfDictionary(dictionary));
    }

    private void ParseKeyword()
    {
      var token = Lexer.GetToken();
      switch (token.Lexeme)
      {
        case "obj":
          Stack.Push(new PdfObject(PdfObjectType.OBJ_START));
          break;
        case "stream":
          ParseStream();
          break;
        case "xref":
          Stack.Push(new PdfObject(PdfObjectType.XREF_START));
          break;
        case "trailer":
          Stack.Push(new PdfObject(PdfObjectType.TRAILER));
          break;
        case "R":
          ParseIndirectReference();
          break;
        case "endstream":
          break;
        default:
          throw new PdfParseException($"Unhandled keyword {token.Lexeme} :(");
      }
    }

    private void ParseStream()
    {
      Lexer.ConsumeToken(); // consume stream keyword
      Lexer.TryConsumeToken(new List<Token> {
        new (TokenType.EOL, "\r\n"),
        new (TokenType.EOL, "\n")
      });

      var content = Lexer.GetStreamContentToken();
      Lexer.ConsumeToken();
      Lexer.TryConsumeToken(TokenType.EOL);
      var endStream = Lexer.GetToken();
      if (endStream.Type != TokenType.KEYWORD || endStream.Lexeme != "endstream")
        throw new PdfParseException($"Expected endstream but found '{endStream.Lexeme}' :(");
      Lexer.ConsumeToken();

      if (!Stack.Any() || Stack.Peek().ObjectType != PdfObjectType.DICTIONARY)
        throw new PdfParseException("Stream objects must be preceded by a dictionary");

      var dictionary = Stack.Pop() as PdfDictionary;
      
      Stack.Push(new PdfStream(dictionary, content));
    }

    private void ParseIndirectReference()
    {
      Lexer.ConsumeToken();
      if (Stack.Count < 2)
        throw new PdfParseException("Unexpected reference keyword. A reference requires to be preceded by 2 integer objects");

      var generationNumber = Stack.Pop();
      var objectNumber = Stack.Pop();

      if (generationNumber.ObjectType is PdfObjectType.NUMERIC && !(generationNumber as PdfNumeric).IsReal &&
          objectNumber.ObjectType is PdfObjectType.NUMERIC && !(objectNumber as PdfNumeric).IsReal)
      {
        Stack.Push(new PdfIndirectReference(objectNumber as PdfNumeric, generationNumber as PdfNumeric));
      }
      else
      {
        throw new PdfParseException($"An indirect reference must be preceded by 2 integer numerics but this one was preceded by a {objectNumber.ObjectType} and a {generationNumber.ObjectType}");
      }
    }
  }
}