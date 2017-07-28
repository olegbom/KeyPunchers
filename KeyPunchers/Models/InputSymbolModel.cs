using System;

namespace KeyPunchers.Models
{
    public class InputSymbolModel
    {
        public string Symbol;
        public DateTime Time;
        public bool Сorrectness;

        public InputSymbolModel(string symbol, DateTime time, bool сorrectness)
        {
            Symbol = symbol;
            Time = time;
            Сorrectness = сorrectness;
        }
    }
}
