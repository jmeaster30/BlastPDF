using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using BlastPDF.Internal.Exceptions;
using BlastPDF.Internal.Helpers;
using BlastPDF.Internal.Structure;
using BlastPDF.Internal.Structure.Objects;
using BlastPDF.Internal.Structure.File;

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

    private Regex pdfVersionHeader = new Regex(@"^%PDF-1\.[0-7]$");

    public PdfInternalFile ParseFile()
    {
      var file = new PdfInternalFile();
      var firstToken = Lexer.GetToken();
      if (firstToken.Type != TokenType.COMMENT) throw new PdfParseException($"Expected header at start of file but got {Lexer.GetToken().Type}");
      if (!pdfVersionHeader.IsMatch(firstToken.Lexeme)) throw new PdfParseException($"Header did not match form of '%PDF-1.[0-7]' found '{firstToken.Lexeme}'");
      Lexer.ConsumeToken();
      file.Header = new PdfComment(firstToken);

      var objects = ParseBodyContents();
      Console.WriteLine($"{objects.Count()} objs");
      var content_groups = objects.ToList().Split((prev, curr) => 
        (prev == null || prev.ObjectType == PdfObjectType.TRAILER) &&
        curr.ObjectType == PdfObjectType.COMMENT &&
        (curr as PdfComment).Comment.Lexeme == "%%EOF", true);

      Console.WriteLine($"{content_groups.Count()} list");
      foreach (var group in content_groups) {
        if (group.Count() == 0) continue; // error?

        var bodyContent = new PdfInternalBody();

        if (group.Count() == 1) {
          if (group.ElementAt(0).ObjectType == PdfObjectType.COMMENT) {
            bodyContent.Footer = group.ElementAt(0) as PdfComment;
            file.Body.Add(bodyContent);
            continue;
          }
          throw new PdfParseException("Found body section with only 1 element.");
        }
        
        bodyContent.Trailer = group.ElementAt(group.Count() - 1) as PdfTrailer;

        var endOffset = 1;
        var possibleXref = group.ElementAt(group.Count() - 2);
        if (possibleXref.ObjectType == PdfObjectType.XREF_TABLE) {
          bodyContent.CrossReferenceTable = possibleXref as PdfXReferenceTable;
          endOffset = 2;
        }

        bodyContent.Contents = group.Take(group.Count() - endOffset - 1).ToList();
        file.Body.Add(bodyContent);
      }

      return file;
    }

    public IEnumerable<PdfObject> ParseBodyContents()
    {
      while (Lexer.GetToken().Type is not TokenType.EOF)
      {
        ParseObject();
      }

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

    private void ParseObject()
    {
      ConsumeOptionalWhitespace();
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
        case TokenType.ARRAY_OPEN:
          ParseArray();
          break;
        case TokenType.DICT_OPEN:
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

    private void ParseArray()
    {
      var values = new List<PdfObject>();

      Lexer.ConsumeToken();
      Stack.Push(new PdfObject(PdfObjectType.ARRAY_START));

      while (Lexer.GetToken().Type is not (TokenType.ARRAY_CLOSE or TokenType.EOF)) {
        ParseObject();
        ConsumeOptionalWhitespace();
        if (!Stack.Any()) throw new PdfParseException("Unexpected empty stack while parsing array contents :(");
      }

      if (Lexer.GetToken().Type is TokenType.EOF) throw new PdfParseException("Unexpected end of file while parsing array contents");
      Lexer.ConsumeToken(); // consume array close

      while (Stack.Any() && Stack.Peek().ObjectType != PdfObjectType.ARRAY_START)
      {
        var val = Stack.Pop();
        if (val.ObjectType > PdfObjectType.INDIRECT_REF)
          throw new PdfParseException("Invalid object type in array :(");
        values.Insert(0, val);
      }

      if (!Stack.Any()) throw new PdfParseException("WHOOPS WE ATE TOO MANY OBJECTS UWU");
      Stack.Pop();

      Stack.Push(new PdfArray(values));
    }

    private void ParseDictionary()
    {
      var dictionary = new Dictionary<PdfName, PdfObject>();
      var values = new List<PdfObject>();
      
      Lexer.ConsumeToken();
      Stack.Push(new PdfObject(PdfObjectType.DICT_START));

      while (Lexer.GetToken().Type is not (TokenType.DICT_CLOSE or TokenType.EOF)) {
        ParseObject();
        ConsumeOptionalWhitespace();
        if (!Stack.Any()) throw new PdfParseException("Unexpected empty stack while parsing dictionary contents :(");
      }

      if (Lexer.GetToken().Type is TokenType.EOF) throw new PdfParseException("Unexpected end of file while parsing dictionary contents");
      Lexer.ConsumeToken(); // consume dictionary close

      while (Stack.Any() && Stack.Peek().ObjectType != PdfObjectType.DICT_START)
      {
        var val = Stack.Pop();
        if (val.ObjectType > PdfObjectType.INDIRECT_REF)
          throw new PdfParseException("Invalid object type in array :(");
        values.Insert(0, val);
      }

      if (!Stack.Any()) throw new PdfParseException("WHOOPS WE ATE TOO MANY OBJECTS UWU");
      Stack.Pop();

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

      Stack.Push(new PdfDictionary(dictionary));
    }

    private void ParseKeyword()
    {
      var token = Lexer.GetToken();
      switch (token.Lexeme)
      {
        case "obj":
          ParseIndirectObject();
          break;
        case "stream":
          ParseStream();
          break;
        case "startxref":
          ParseStartXref();
          if (!Stack.Any() || Stack.Peek().ObjectType != PdfObjectType.START_XREF)
            throw new PdfParseException($"Expected a startxref to be on the stack :(");
          Stack.Push(new PdfTrailer(null, Stack.Pop() as PdfStartXref));
          break;
        case "xref":
          Stack.Push(new PdfObject(PdfObjectType.XREF_START));
          Lexer.ConsumeToken();
          // TODO finish this
          break;
        case "trailer":
          ParseTrailer();
          break;
        case "R":
          ParseIndirectReference();
          break;
        default:
          throw new PdfParseException($"Unhandled keyword {token.Lexeme} :(");
      }
    }

    private void ParseStream()
    {
      Lexer.ConsumeToken(); // consume stream keyword
      Lexer.TryConsumeToken(new List<Token> {
        new (TokenType.EOL, "\r\n", 0, 0),
        new (TokenType.EOL, "\n", 0, 0)
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

    private void ParseIndirectObject()
    {
      Lexer.ConsumeToken();
      if (Stack.Count < 2)
        throw new PdfParseException("Unexpected obj keyword. An indirect object requires to be preceded by 2 integer objects");

      var generationNumber = Stack.Pop();
      var objectNumber = Stack.Pop();

      if (generationNumber.ObjectType is PdfObjectType.NUMERIC && !(generationNumber as PdfNumeric).IsReal &&
          objectNumber.ObjectType is PdfObjectType.NUMERIC && !(objectNumber as PdfNumeric).IsReal)
      {
        Stack.Push(new PdfObject(PdfObjectType.OBJ_START));
        var top = Lexer.GetToken();
        while (!(top.Type == TokenType.KEYWORD && top.Lexeme == "endobj"))
        {
          ParseObject();
          ConsumeOptionalWhitespace();
          top = Lexer.GetToken();
        }
        
        var values = new List<PdfObject>();
        while (Stack.Any() && Stack.Peek().ObjectType != PdfObjectType.OBJ_START)
        {
          var val = Stack.Pop();
          if (val.ObjectType > PdfObjectType.INDIRECT_REF)
            throw new PdfParseException("Invalid object type in indirect object :(");
          values.Insert(0, val);
        }

        if (!Stack.Any()) throw new PdfParseException("WHOOPS WE ATE TOO MANY OBJECTS UWU");
        Stack.Pop();

        if(values.Count != 1) throw new PdfParseException($"Unexpected amount of indirect object contents ({values.Count}) :(");

        Lexer.TryConsumeToken(TokenType.KEYWORD, "endbj");
        var indirect_object = new PdfIndirectObject(generationNumber as PdfNumeric, objectNumber as PdfNumeric, values[0]);
        Stack.Push(indirect_object);
      }
      else
      {
        throw new PdfParseException($"An indirect reference must be preceded by 2 integer numerics but this one was preceded by a {objectNumber.ObjectType} and a {generationNumber.ObjectType}");
      }
    }

    private void ParseStartXref()
    {
      Lexer.ConsumeToken();

      Lexer.TryConsumeToken(TokenType.EOL);
      var next = Lexer.GetToken();
      if (next.Type != TokenType.INTEGER) throw new PdfParseException($"Expected an integer to follow the 'startxref' keyword.");
      Lexer.ConsumeToken();
      Stack.Push(new PdfStartXref(new PdfNumeric(next)));
    }

    private void ParseTrailer()
    {
      Lexer.ConsumeToken();
      ConsumeOptionalWhitespace();
      var top = Lexer.GetToken();
      if (top.Type != TokenType.DICT_OPEN) throw new PdfParseException($"Expected a dictionary following the 'trailer' keyword.");
      ParseDictionary();
      if (!Stack.Any() || Stack.Peek().ObjectType != PdfObjectType.DICTIONARY)
        throw new PdfParseException($"Expected a dictionary following the 'trailer' keyword.");
      
      var dict = Stack.Pop() as PdfDictionary;
      ConsumeOptionalWhitespace();
      var nextToken = Lexer.GetToken();
      if (nextToken.Type != TokenType.KEYWORD && nextToken.Lexeme != "startxref") throw new PdfParseException($"Expected startxref after the trailer dictionary.");
      Lexer.ConsumeToken();
      ParseStartXref();
      if (!Stack.Any() || Stack.Peek().ObjectType != PdfObjectType.START_XREF)
        throw new PdfParseException($"Expected a startxref to be on the stack :(");
      
      Stack.Push(new PdfTrailer(dict, Stack.Pop() as PdfStartXref));
    }
  }
}