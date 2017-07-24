using System;

namespace KeyPunchers.Models
{
    public class InputKeyboardSymbolModel
    {
        public string Symbol;
        public DateTime Time;
        public bool Сorrectness;

        public InputKeyboardSymbolModel(string symbol, DateTime time, bool сorrectness)
        {
            Symbol = symbol;
            Time = time;
            Сorrectness = сorrectness;
        }
    }
}
