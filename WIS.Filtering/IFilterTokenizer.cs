using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WIS.Filtering.Tokens;

namespace WIS.Filtering
{
    public interface IFilterTokenizer
    {
        List<IFilterToken> Tokens { get; set; }
        void Tokenize(string text);
        List<IFilterToken> GetPolishNotation();
        bool IsValid();
    }
}
