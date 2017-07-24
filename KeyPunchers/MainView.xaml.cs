using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using KeyPunchers.Models;
using KeyPunchers.ViewModels;
using Microsoft.Win32;

namespace KeyPunchers
{
    /// <summary>
    /// Логика взаимодействия для MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainViewModel ViewModel => (MainViewModel) DataContext;
       
        public MainView()
        {
            InputManager.Current.PreNotifyInput += PreNotifyInput;
            // This is where we handle all the rest of the keys
            TextCompositionManager.AddPreviewTextInputStartHandler(
                Application.Current.MainWindow,
                PreviewTextInputHandler);

            InitializeComponent();
        }

        private void PreNotifyInput(object sender, NotifyInputEventArgs e)
        {
            // I'm only interested in key down events
            if (e.StagingItem.Input.RoutedEvent != Keyboard.KeyDownEvent)
                return;
            var args = e.StagingItem.Input as KeyEventArgs;
            // I only care about the space key being pressed
            // you might have to check for other characters
            if (args == null || args.Key != Key.Space)
                return;
            // stop event processing here
            args.Handled = true;
            // this is my internal method for handling a keystroke
            HanleKeystroke(" ");
        }

        private void PreviewTextInputHandler(object sender,
            TextCompositionEventArgs e)
        {
            HanleKeystroke(e.Text);
        }

        private void HanleKeystroke(string text)
        {
            ViewModel.InputSymbol(text);
           
        }

        private void MenOpenFile_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog opd = new OpenFileDialog();

            if (opd.ShowDialog() == true)
            {
                ViewModel.SetText(File.ReadAllText(opd.FileName));
            }
        }

        private void ContextPaste_OnClick(object sender, RoutedEventArgs e)
        {
            var text = Clipboard.GetText();
            if (text != "")
            {
                ViewModel.SetText(text);
            }
        }

        
    }
}
