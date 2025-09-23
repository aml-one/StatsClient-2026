using StatsClient.MVVM.Core;
using StatsClient.MVVM.ViewModel;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StatsClient.MVVM.View
{
    /// <summary>
    /// Interaction logic for SplashWindow.xaml
    /// </summary>
    public partial class SplashWindow : Window, INotifyPropertyChanged
    {
        public static event PropertyChangedEventHandler? PropertyChangedStatic;
#pragma warning disable CS0067 // The event 'SplashWindow.PropertyChanged' is never used
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067 // The event 'SplashWindow.PropertyChanged' is never used

        public static void RaisePropertyChangedStatic([CallerMemberName] string? propertyname = null)
        {
            PropertyChangedStatic?.Invoke(typeof(ObservableObject), new PropertyChangedEventArgs(propertyname));
        }

        private static SplashWindow? instance;
        public static SplashWindow Instance
        {
            get => instance!;
            set
            {
                instance = value;
                RaisePropertyChangedStatic(nameof(Instance));
            }
        }

        public SplashWindow()
        {
            Instance = this;
            InitializeComponent();
        }

        public void CloseApp()
        {
            Application.Current.Shutdown();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BeginStoryboard? sb = this.FindResource("LogoOpacityAnimation")! as BeginStoryboard;
            sb!.Storyboard.Completed += LogoOpacityStoryboard_Completed;
            sb!.Storyboard.Begin();
            //BeginStoryboard? sb2 = this.FindResource("BackgroundOpacityAnimation")! as BeginStoryboard;
            //sb2!.Storyboard.Begin();
        }

        private void LogoOpacityStoryboard_Completed(object? sender, EventArgs e)
        {
            //BeginStoryboard? sb21 = this.FindResource("BackgroundZoomAndMoveAnimation")! as BeginStoryboard;
            //sb21!.Storyboard.Begin();
            BeginStoryboard? sbBG4 = this.FindResource("BackgroundEaseInAnimation")! as BeginStoryboard;
            sbBG4!.Storyboard.Completed += BackgroundEaseInStoryboard_Completed;
            sbBG4!.Storyboard.Begin();


            BeginStoryboard? sb2 = this.FindResource("LogoMoveAnimation")! as BeginStoryboard;
            sb2!.Storyboard.Begin();
            BeginStoryboard? sb3 = this.FindResource("LogoSizeAnimation")! as BeginStoryboard;
            sb3!.Storyboard.Completed += LogoSizeStoryboard_Completed;
            sb3!.Storyboard.Begin();
        }

        
        private void BackgroundEaseInStoryboard_Completed(object? sender, EventArgs e)
        {
            
        }

        private void LogoSizeStoryboard_Completed(object? sender, EventArgs e)
        {
            BeginStoryboard? sb = this.FindResource("PanelOpacityAnimation")! as BeginStoryboard;
            sb!.Storyboard.Completed += BackgroundOpacityStoryboard_Completed;
            sb!.Storyboard.Begin();

            BeginStoryboard? sbVers = this.FindResource("VersionNumberEaseInAnimation")! as BeginStoryboard;
            sbVers!.Storyboard.Begin();
        }


        private void BackgroundOpacityStoryboard_Completed(object? sender, EventArgs e)
        {
            SplashViewModel.Instance.StartLoading();
        }
    }
}
