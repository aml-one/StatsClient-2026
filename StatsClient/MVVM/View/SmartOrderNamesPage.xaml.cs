using StatsClient.MVVM.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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

namespace StatsClient.MVVM.View
{
    /// <summary>
    /// Interaction logic for SmartOrderNamesPage.xaml
    /// </summary>
    public partial class SmartOrderNamesPage : Window, INotifyPropertyChanged
    {
        public static event PropertyChangedEventHandler? PropertyChangedStatic;
        public event PropertyChangedEventHandler? PropertyChanged;

        public static void RaisePropertyChangedStatic([CallerMemberName] string? propertyname = null)
        {
            PropertyChangedStatic?.Invoke(typeof(ObservableObject), new PropertyChangedEventArgs(propertyname));
        }

        public void RaisePropertyChanged([CallerMemberName] string? propertyname = null)
        {
            PropertyChanged?.Invoke(typeof(ObservableObject), new PropertyChangedEventArgs(propertyname));
        }

        private static SmartOrderNamesPage? staticInstance;
        public static SmartOrderNamesPage? StaticInstance
        {
            get => staticInstance;
            set
            {
                staticInstance = value;
                RaisePropertyChangedStatic(nameof(StaticInstance));
            }
        }


        public SmartOrderNamesPage()
        {
            StaticInstance = this;
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Hide();
        }

        private void renameButton_GotFocus(object sender, RoutedEventArgs e)
        {
            renameButton.Background = new SolidColorBrush(Color.FromArgb(255, 0, 128, 72));
            renameButton.Foreground = Brushes.Black;
        }

        private void renameButton_LostFocus(object sender, RoutedEventArgs e)
        {
            renameButton.Background = new SolidColorBrush(Color.FromArgb(255, 82, 105, 94));
            renameButton.Foreground = Brushes.Silver;
        }

        private void CheckBox_GotFocus(object sender, RoutedEventArgs e)
        {
            cbScrewRetained.Foreground = Brushes.Yellow;
        }

        private void CheckBox_LostFocus(object sender, RoutedEventArgs e)
        {
            cbScrewRetained.Foreground = Brushes.Black;
        }

        public void TitleBar_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                try
                {
                    this.DragMove();
                }
                catch { }
        }
    }
}
