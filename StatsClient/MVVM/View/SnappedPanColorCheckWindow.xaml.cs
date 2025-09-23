using StatsClient.MVVM.Core;
using StatsClient.MVVM.ViewModel;
using System.Windows;
using System.Windows.Input;

namespace StatsClient.MVVM.View
{
    /// <summary>
    /// Interaction logic for SnappedPanColorCheckWindow.xaml
    /// </summary>
    public partial class SnappedPanColorCheckWindow : Window
    {
        bool inDrag = false;
        Point anchorPoint;

        private double wHeight = 49;
        private double pHeight = 45;

        private static SnappedPanColorCheckWindow? staticInstance;
        public static SnappedPanColorCheckWindow StaticInstance
        {
            get => staticInstance!;
            set
            {
                staticInstance = value;
            }
        }

        public SnappedPanColorCheckWindow()
        {
            StaticInstance = this;
            InitializeComponent();
        }

        private void GeneralTimer_Tick(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Grid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //if (e.LeftButton != MouseButtonState.Pressed) return;
            if (e.ChangedButton == MouseButton.Left)
            {
                anchorPoint = PointToScreen(e.GetPosition(this));
                inDrag = true;
            }
        }

        
        private void Grid_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            //if (inDrag)
            //{
                inDrag = false;
                //ReleaseMouseCapture();
                //e.Handled = true;
            //}
        }

        private void Grid_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                inDrag = false;
                return;
            }

            
            if (inDrag)
            {
                Point currentPoint = PointToScreen(e.GetPosition(this));
                this.Left = this.Left + currentPoint.X - anchorPoint.X; // only allow horizontal movements
                if (this.Left < 0)
                    this.Left = 0;

                double MaxLeftPosition = SystemParameters.MaximizedPrimaryScreenWidth - (Width + 10);

                if (this.Left > MaxLeftPosition)
                    this.Left = MaxLeftPosition;

                this.anchorPoint = currentPoint;

                SaveWindowPosition();
            }
        }


        private void SaveWindowPosition()
        {
            LocalSettingsDB.WriteLocalSetting("SnappedColorCheckWindowPosLeft", Left.ToString());
            LocalSettingsDB.WriteLocalSetting("ColorCheckWindowIsOpen", "true");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _ = int.TryParse(LocalSettingsDB.ReadLocalSetting("SnappedColorCheckWindowPosLeft"), out int wLeft);

            if (wLeft != 0)
            {
                Top = 0;
                Left = wLeft;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            return;
        }

        private void TextBlock_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            HideWindow();
        }

        private void HideWindow()
        {
            panNumberBox.Text = "";
            for (int i = 0; i <= 8; i++)
            {
                path.Height = pHeight;
                pHeight -= 5;
               
                Height = wHeight;
                wHeight -= 5;
                Thread.Sleep(10);
            }
            PanColorCheckViewModel.StaticInstance.HideLabelVisibility = Visibility.Hidden;
            PanColorCheckViewModel.StaticInstance.PanColorShowsNow = Visibility.Hidden;
            PanColorCheckViewModel.StaticInstance.NoNumberRegisteredShowsNow = Visibility.Hidden;
            tbCheckPanColor.Visibility = Visibility.Hidden;
            panNumberBox.Margin = new Thickness(0, 0, 0, 0); // fixing the box position after the window was hidden
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Height < 20)
            {
                for (int i = 0; i <= 8; i++)
                {
                    path.Height = pHeight;
                    pHeight += 5;

                    Height = wHeight;
                    wHeight += 5;
                    Thread.Sleep(10);
                }
            }
            PanColorCheckViewModel.StaticInstance.HideLabelVisibility = Visibility.Visible;
            tbCheckPanColor.Visibility = Visibility.Visible;
            tbCheckPanColor.Focus();
            Activate();
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            PanColorCheckViewModel.StaticInstance.HideLabelVisibility = Visibility.Hidden;
            if (inDrag)
            {
                inDrag = false;
                ReleaseMouseCapture();
                e.Handled = true;
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            inDrag = false;
            PanColorCheckViewModel.StaticInstance.PcPanColor = "#777777";
            PanColorCheckViewModel.StaticInstance.PanColorWindowBorderColor = "BlanchedAlmond";
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            PanColorCheckViewModel.StaticInstance.PcPanColor = "#565656";
            PanColorCheckViewModel.StaticInstance.PanColorWindowBorderColor = "#999";
            PanColorCheckViewModel.StaticInstance.HideLabelVisibility = Visibility.Hidden;
        }
    }
}
