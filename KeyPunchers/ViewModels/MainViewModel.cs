using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using KeyPunchers.Models;
using OxyPlot;
using OxyPlot.Axes;

namespace KeyPunchers.ViewModels
{

    public class MainViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        public string Text { get; private set; } = @"dfsdafafdsafsa";

        public int CurrentSymbolIndex { get; private set; } = 0;

        public double TextVerticalOffset => (double)Text.Length / CurrentSymbolIndex;

        public string TextBefore => Text.Substring(0, CurrentSymbolIndex);
        public string TextAfter  => Text.Substring(CurrentSymbolIndex+1, Text.Length - CurrentSymbolIndex-1);
        public string CurrentSymbol => Text[CurrentSymbolIndex].ToString();

        public double Сorrectness { get; set; }
        public DateTime StartTime { get; private set; }
        public DateTime CurrentTime { get; private set; }

        public TimeSpan TypingDateTime => CurrentTime - StartTime;

        public bool Finish { get; set; }

        public ObservableCollection<DataPoint> SpeedData { get; } = new ObservableCollection<DataPoint>();


        public List<InputKeyboardSymbolModel> InputKeyboardSymbolModels { get; }= new List<InputKeyboardSymbolModel>();
        public List<InputKeyboardSymbolModel> CorrectInputKeyboardSymbolModels { get; } = new List<InputKeyboardSymbolModel>();

        public MainViewModel()
        {
            StartTime = DateTime.Now;
            CurrentTime = StartTime;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += (sender, args) =>
            {
                if (Finish) return;
                CurrentTime = DateTime.Now;
                var count = CorrectInputKeyboardSymbolModels.Count;
                if (count < 2) return;
                for (int i = count - 1; i >= 0; i--)
                {
                    var span = CorrectInputKeyboardSymbolModels[count - 1].Time - CorrectInputKeyboardSymbolModels[i].Time;
                    if (span.TotalMilliseconds > 5000)
                    {
                        SpeedData.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), (count - 1 - i) / span.TotalMinutes));
                        while(SpeedData.Count > 60) SpeedData.RemoveAt(0);
                        break;
                    }
                }
            };
            timer.Start();
            
        }


        public void InputSymbol(string text)
        {
            if (Finish) return; 

            bool correctness = CurrentSymbol.Equals(text);
            var model = new InputKeyboardSymbolModel(text, DateTime.Now, correctness);
            InputKeyboardSymbolModels.Add(model);

            int count = InputKeyboardSymbolModels.Count;
            if (count > 1)
            {
                var correctnessCount = (double)InputKeyboardSymbolModels.Count(x => x.Сorrectness);
                Сorrectness = correctnessCount / InputKeyboardSymbolModels.Count;
            }
            
            if (correctness)
            {
                CorrectInputKeyboardSymbolModels.Add(model);
                if (CurrentSymbolIndex == Text.Length - 1) Finish = true; 
                else CurrentSymbolIndex++;
            }

        }

     

        public void SetText(string text)
        {
            StartTime = DateTime.Now;
            Finish = false;
            InputKeyboardSymbolModels.Clear();
            CorrectInputKeyboardSymbolModels.Clear();
            Text = text;
            CurrentSymbolIndex = 0;
        }
        
    }
}
