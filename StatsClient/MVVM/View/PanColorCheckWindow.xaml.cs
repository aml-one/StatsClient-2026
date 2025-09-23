using StatsClient.MVVM.Core;
using StatsClient.MVVM.ViewModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;


namespace StatsClient.MVVM.View
{
    /// <summary>
    /// Interaction logic for PanColorCheckWindow.xaml
    /// </summary>
    public partial class PanColorCheckWindow : Window
    {
        private static PanColorCheckWindow? staticInstance;
        public static PanColorCheckWindow StaticInstance
        {
            get => staticInstance!;
            set
            {
                staticInstance = value;
            }
        }

        public PanColorCheckWindow()
        {
            StaticInstance = this;
            InitializeComponent();
        }

        private void Border_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                try
                {
                    this.DragMove();
                    panNumberBox.Focus();
                    SaveWindowPosition();
                }
                catch { }
        }

        private void SaveWindowPosition()
        {
            double MaxTopPosition = SystemParameters.MaximizedPrimaryScreenHeight - (StaticInstance.Height + 17);
            double MaxLeftPosition = SystemParameters.MaximizedPrimaryScreenWidth - (StaticInstance.Width + 20);

            LocalSettingsDB.WriteLocalSetting("ColorCheckWindowPosTop", Top.ToString());
            LocalSettingsDB.WriteLocalSetting("ColorCheckWindowPosLeft", Left.ToString());
            LocalSettingsDB.WriteLocalSetting("ColorCheckWindowIsOpen", "true");

            if (Top > MaxTopPosition)
                LocalSettingsDB.WriteLocalSetting("ColorCheckWindowPosTop", MaxTopPosition.ToString());

            if (Top < 0)
                LocalSettingsDB.WriteLocalSetting("ColorCheckWindowPosTop", "0");


            if (Left > MaxLeftPosition)
                LocalSettingsDB.WriteLocalSetting("ColorCheckWindowPosLeft", MaxLeftPosition.ToString());

            if (Left < 0)
                LocalSettingsDB.WriteLocalSetting("ColorCheckWindowPosLeft", "0");

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _ = int.TryParse(LocalSettingsDB.ReadLocalSetting("ColorCheckWindowPosTop"), out int wTop);
            _ = int.TryParse(LocalSettingsDB.ReadLocalSetting("ColorCheckWindowPosLeft"), out int wLeft);

            if (wTop != 0 && wLeft != 0)
            {
                Top = wTop;
                Left = wLeft;
            }

            double MaxTopPosition = SystemParameters.MaximizedPrimaryScreenHeight - (StaticInstance.Height + 17);
            double MaxLeftPosition = SystemParameters.MaximizedPrimaryScreenWidth - (StaticInstance.Width + 20);

            if (Top > MaxTopPosition)
                Top = 10;
            
            if (Left > MaxLeftPosition)
                Left = 10;
        }

        private void Window_Activated(object sender, EventArgs e)
        {   
            PanColorCheckViewModel.StaticInstance.PcPanColor = "#777777";
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            PanColorCheckViewModel.StaticInstance.PcPanColor = "#565656";
            PanColorCheckViewModel.StaticInstance.HideLabelVisibility = Visibility.Hidden;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            return;
        }

        private void PanNumberBox_MouseEnter(object sender, MouseEventArgs e)
        {
            PanColorCheckViewModel.StaticInstance.HideLabelVisibility = Visibility.Visible;
        }

        private void PanNumberBox_MouseLeave(object sender, MouseEventArgs e)
        {
            PanColorCheckViewModel.StaticInstance.HideLabelVisibility = Visibility.Hidden;
        }










        private Point offset = new ();

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (PanColorCheckViewModel.StaticInstance.AddingANumberNow)
            {
                Mouse.Capture(null);
                return;
            }

            Point cursorPos = PointToScreen(Mouse.GetPosition(this));
            Point windowPos = new (this.Left, this.Top);
            offset = (Point)(cursorPos - windowPos);

            // capturing the mouse here will redirect all events to this window, even if
            // the mouse cursor should leave the window area
            Mouse.Capture(this, CaptureMode.Element);
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (PanColorCheckViewModel.StaticInstance.AddingANumberNow)
            {
                Mouse.Capture(null);
                return;
            }

            Mouse.Capture(null);
            panNumberBox.Focus();
            SaveWindowPosition();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (PanColorCheckViewModel.StaticInstance.AddingANumberNow)
                return;

            if (Mouse.Captured == this && Mouse.LeftButton == MouseButtonState.Pressed)
            {
                Point cursorPos = PointToScreen(Mouse.GetPosition(this));
                double newLeft = cursorPos.X - offset.X;
                double newTop = cursorPos.Y - offset.Y;

                // here you can change the window position and implement
                // the snapping behaviour that you need

                int snappingMargin = 25;

                if (Math.Abs(SystemParameters.WorkArea.Left - newLeft) < snappingMargin)
                    newLeft = SystemParameters.WorkArea.Left;
                else if (Math.Abs(newLeft + this.ActualWidth - SystemParameters.WorkArea.Left - SystemParameters.WorkArea.Width) < snappingMargin)
                    newLeft = SystemParameters.WorkArea.Left + SystemParameters.WorkArea.Width - this.ActualWidth;

                if (Math.Abs(SystemParameters.WorkArea.Top - newTop) < snappingMargin)
                    newTop = SystemParameters.WorkArea.Top;
                else if (Math.Abs(newTop + this.ActualHeight - SystemParameters.WorkArea.Top - SystemParameters.WorkArea.Height) < snappingMargin)
                    newTop = SystemParameters.WorkArea.Top + SystemParameters.WorkArea.Height - this.ActualHeight;


                this.Left = newLeft;
                this.Top = newTop;
            }

            
        }
    }
}
