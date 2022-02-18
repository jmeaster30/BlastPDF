using System;

namespace BlastPDF.Internal.Exceptions;

public class PdfParseException : Exception
{
    public PdfParseException(string message) : base($"PDF Parse Exception :: {message}") { }
}