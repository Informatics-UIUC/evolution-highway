using System;

namespace EvolutionHighwayApp.Exceptions
{
    public class ParseErrorException : Exception
    {
        public string Text { get; private set; }
        public int? LineNumber { get; private set; }

        public ParseErrorException(string text, int? lineNumber) : this("A parse error has occurred", text, lineNumber) { }

        public ParseErrorException(string message, string text = null, int? lineNumber = null) : base(message)
        {
            Text = text;
            LineNumber = lineNumber;
        }
    }
}
