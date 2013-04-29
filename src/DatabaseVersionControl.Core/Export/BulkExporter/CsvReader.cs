using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public sealed class CsvReader : System.IDisposable
{
    public CsvReader(string fileName)
        : this(new FileStream(fileName, FileMode.Open, FileAccess.Read))
    {
    }

    public CsvReader(Stream stream)
        : this(new StreamReader(stream))
    {
    }

    public CsvReader(TextReader stream)
    {
        _textReader = stream;
    }


    public IEnumerable<string[]> RowEnumerator
    {
        get
        {
            if (null == _textReader)
                throw new System.ApplicationException("I can't start reading without CSV input.");

            _rowno = 0;
            string sLine;
            string sNextLine;

            while (null != (sLine = _textReader.ReadLine()))
            {
                while (_rexRunOnLine.IsMatch(sLine) && null != (sNextLine = _textReader.ReadLine()))
                    sLine += "\n" + sNextLine;

                _rowno++;
                string[] values = _rexCsvSplitter.Split(sLine);

                for (int i = 0; i < values.Length; i++)
                    values[i] = Csv.Unescape(values[i]);

                yield return values;
            }

            _textReader.Close();
        }
    }

    public long RowIndex { get { return _rowno; } }

    public void Dispose()
    {
        if (null != _textReader) _textReader.Dispose();
    }

    //============================================


    private long _rowno = 0;
    private TextReader _textReader;
    private static Regex _rexCsvSplitter = new Regex(@",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))");
    private static Regex _rexRunOnLine = new Regex(@"^[^""]*(?:""[^""]*""[^""]*)*""[^""]*$");

    public static class Csv
    {
        public static string Escape(string s)
        {
            if (s.Contains(Quote))
                s = s.Replace(Quote, EscapedQuote);

            if (s.IndexOfAny(CharactersThatMustBeQuoted) > -1)
                s = Quote + s + Quote;

            return s;
        }

        public static string Unescape(string s)
        {
            if (s.StartsWith(Quote) && s.EndsWith(Quote))
            {
                s = s.Substring(1, s.Length - 2);

                if (s.Contains(EscapedQuote))
                    s = s.Replace(EscapedQuote, Quote);
            }

            return s;
        }


        private const string Quote = "\"";
        private const string EscapedQuote = "\"\"";
        private static readonly char[] CharactersThatMustBeQuoted = { ',', '"', '\n' };
    }
}