using System.Collections.Generic;

namespace BlastPDF.Internal
{
    public static class KeywordsOperators
    {
        public static Dictionary<string, TokenType> Values { get; } = new()
        {
            //BASE
            {"true", TokenType.BOOLEAN},
            {"false", TokenType.BOOLEAN},
            {"null", TokenType.NULL},

            //KEYWORDS
            {"obj", TokenType.KEYWORD},
            {"endobj", TokenType.KEYWORD},
            {"stream", TokenType.KEYWORD},
            {"endstream", TokenType.KEYWORD},
            {"startxref", TokenType.KEYWORD},
            {"xref", TokenType.KEYWORD},
            {"trailer", TokenType.KEYWORD},
            {"R", TokenType.KEYWORD},
            {"n", TokenType.KEYWORD},
            {"f", TokenType.KEYWORD}
        };
    }
}
