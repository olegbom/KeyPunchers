﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Threading;
using KeyPunchers.Models;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace KeyPunchers.ViewModels
{

    public class MainViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        public string Text { get; private set; } = @"рпаырпавырпаврпавр";

        public int CurrentSymbolIndex { get; private set; } = 0;

        public double TextVerticalOffset { get; set; }

        public bool Finish { get; set; }

        public string TextBefore => Finish ? Text : Text.Substring(0, CurrentSymbolIndex);
        public string TextAfter  => Finish ? " " : Text.Substring(CurrentSymbolIndex+1, Text.Length - CurrentSymbolIndex-1);
        public string CurrentSymbol => Finish? " " : Text[CurrentSymbolIndex].ToString();

        public double Сorrectness { get; set; }
        
        public double FullSpeed { get; private set; }
        
        public int CorrectSymbolsCount { get; private set; }

        public string BadKeysSummary { get; private set; } = "";

        public TimeSpan FullTime { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime CurrentTime { get; private set; }

        public TimeSpan TypingDateTime => CurrentTime - StartTime;


        public PlotModel PlotModel { get; private set; } = new PlotModel();
        public ObservableCollection<DataPoint> SpeedData { get; } = new ObservableCollection<DataPoint>();


        public List<InputSymbolModel> InputKeyboardSymbolModels { get; }= new List<InputSymbolModel>();
        public List<InputSymbolModel> CorrectInputKeyboardSymbolModels { get; } = new List<InputSymbolModel>();

        public MainViewModel()
        {
            PlotModel.Axes.Add(new LinearAxis()
            {
                Title = "Скорость, зн./мин",
                Position = AxisPosition.Left,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dash,
                Minimum = 0,
                Maximum = 300,
            });
            PlotModel.Axes.Add(new TimeSpanAxis()
            {
                Title = "Время",
                StringFormat = "mm:ss",
                Position = AxisPosition.Bottom,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dash
            });

            PlotModel.Series.Add(new LineSeries(){ItemsSource = SpeedData });

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
                        SpeedData.Add(new DataPoint(TimeSpanAxis.ToDouble(CurrentTime - StartTime), (count - 1 - i) / span.TotalMinutes));
                        while(SpeedData.Count > 60) SpeedData.RemoveAt(0);
                        break;
                    }
                }
                PlotModel.InvalidatePlot(true);
            };
            timer.Start();
            
        }



        public void InputSymbol(string text, int firstLineEnd)
        {
            if (Finish) return;

            if (text.Equals("\r"))
                text = "\n";

            bool isCorrect = CurrentSymbol.Equals(text);
            var model = new InputSymbolModel(text, DateTime.Now, isCorrect);
            InputKeyboardSymbolModels.Add(model);

            int count = InputKeyboardSymbolModels.Count;
            if (count > 1)
            {
                var correctnessCount = (double)InputKeyboardSymbolModels.Count(x => x.IsСorrect);
                Сorrectness = correctnessCount / InputKeyboardSymbolModels.Count;
            }
            
            if (isCorrect)
            {
                CorrectInputKeyboardSymbolModels.Add(model);
                if (CurrentSymbolIndex == Text.Length - 1)
                {
                    FullTime = InputKeyboardSymbolModels.Last().Time - InputKeyboardSymbolModels.First().Time;
                    CorrectSymbolsCount = CorrectInputKeyboardSymbolModels.Count;
                    FullSpeed = CorrectSymbolsCount / FullTime.TotalMinutes;
                    Finish = true;
                    Dictionary<string, int> errorsDict = new Dictionary<string, int>();
                    int numberOfConsecutiveErrors = 0;
                    for (int i = 0; i < InputKeyboardSymbolModels.Count; i++)
                    {
                        if (!InputKeyboardSymbolModels[i].IsСorrect)
                        {
                            numberOfConsecutiveErrors++;
                        }
                        else if(numberOfConsecutiveErrors != 0)
                        {
                            string symbol = InputKeyboardSymbolModels[i].Symbol;
                            if (!errorsDict.ContainsKey(symbol))
                                errorsDict[symbol] = numberOfConsecutiveErrors;
                            else
                                errorsDict[symbol] += numberOfConsecutiveErrors;
                            numberOfConsecutiveErrors = 0;
                        }
                    }
                    
                    StringBuilder sb = new StringBuilder();
                    foreach (var pairs in errorsDict.OrderBy(p => -p.Value))
                    {
                        sb.AppendLine($"{pairs.Key}: {pairs.Value}");
                    }

                    BadKeysSummary = sb.ToString();
                }
                else
                {
                    CurrentSymbolIndex++;
                    if (CurrentSymbolIndex >= firstLineEnd - 3)
                    {
                        int buff = CurrentSymbolIndex;
                        CurrentSymbolIndex = 0;
                        Text = Text.Substring(buff);
                    }
                }
            }

        }
        
        public void SetText(string text)
        {
            text = text.Replace('–', '-');
            text = text.Replace('—', '-');
            text = text.Replace(Environment.NewLine, "\n");
            
            StartTime = DateTime.Now;
            Text = text;
            CurrentSymbolIndex = 0;
            Finish = false;
            SpeedData.Clear();
            InputKeyboardSymbolModels.Clear();
            CorrectInputKeyboardSymbolModels.Clear();
            PlotModel.InvalidatePlot(true);
        }
        
    }
}
