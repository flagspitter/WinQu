using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QuickCalc.Parser.Parser
{
    internal partial class ParserParser
    {
        public ParserParser() : base(null) { }

        public bool Parse(string s)
        {
            byte[] inputBuffer = System.Text.Encoding.Default.GetBytes(s);
            MemoryStream stream = new MemoryStream(inputBuffer);
            this.Scanner = new ParserScanner(stream);
            return this.Parse();
        }
    }
    
    public class ExpressionException : Exception
    {
        public ExpressionException() : base() {}
        public ExpressionException( string s ) : base(s) {}
    }
}
