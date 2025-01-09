using System;

namespace KeyPunchers.Models
{
    public class InputSymbolModel
    {
        public string Symbol;
        public DateTime Time;
        public bool IsСorrect;

        public InputSymbolModel(string symbol, DateTime time, bool isСorrect)
        {
            Symbol = symbol;
            Time = time;
            IsСorrect = isСorrect;
        }
    }
}
