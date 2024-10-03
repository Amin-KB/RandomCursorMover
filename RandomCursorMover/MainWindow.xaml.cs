using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RandomCursorMover
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Cursor _cursor;

        public MainWindow()
        {
            InitializeComponent();
            _cursor = new Cursor();
            this.Closed += _cursor.MainWindowClosed;
        }

        private  void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var input = textBox.Text;
            var isInt = int.TryParse(input, out var intValue);
            if (!isInt)
            {
                InvalidInput();
                textBox.Text=string.Empty;
            }
            _cursor.SetDuration(intValue);
            Task.Run(async() =>await _cursor.Start());
        }

        private  void InvalidInput()
        {
            string messageBoxText = "input is not valid";
            string caption = "invalid";
            MessageBoxButton button = MessageBoxButton.OKCancel;
            MessageBoxImage icon = MessageBoxImage.Warning;
            MessageBoxResult result;
            result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
           
        }
    }
}