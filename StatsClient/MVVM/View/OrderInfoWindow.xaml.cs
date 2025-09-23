using StatsClient.MVVM.Core;
using StatsClient.MVVM.Model;
using StatsClient.MVVM.ViewModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using static StatsClient.MVVM.Core.MessageBoxes;
using DataFormats = System.Windows.DataFormats;
using Ookii.Dialogs.Wpf;
using TaskDialog = Ookii.Dialogs.Wpf.TaskDialog;
using TaskDialogIcon = Ookii.Dialogs.Wpf.TaskDialogIcon;
using TaskDialogButton = Ookii.Dialogs.Wpf.TaskDialogButton;
using ListView = System.Windows.Controls.ListView;
using System.Windows.Input;
using KeyEventHandler = System.Windows.Input.KeyEventHandler;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace StatsClient.MVVM.View;

public partial class OrderInfoWindow : Window, INotifyPropertyChanged
{
    private OrderInfoWindow? instance;
    public OrderInfoWindow Instance
    {
        get => instance!;
        set
        {
            instance = value;
            RaisePropertyChanged(nameof(Instance));
        }
    }
    
    private static OrderInfoWindow? staticInstance;
    public static OrderInfoWindow StaticInstance
    {
        get => staticInstance!;
        set
        {
            staticInstance = value;
            RaisePropertyChangedStatic(nameof(StaticInstance));
        }
    }

    public OrderInfoWindow(ThreeShapeOrdersModel ThreeShapeObject)
    {
        Instance = this;
        StaticInstance = this;
        InitializeComponent();
        OrderInfoViewModel.Instance._InfoWindow = this;
        OrderInfoViewModel.Instance.ThreeShapeObject = ThreeShapeObject;
        OrderInfoViewModel.Instance.UpdateForm();

        this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
    }

    private void HandleEsc(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
            Close();
    }

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



    private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.Source is System.Windows.Controls.TabControl)
        {
            if (tabPageImages.IsSelected)
                OrderInfoViewModel.Instance.ReloadImages(false);
        }
    }


    private void Window_Closing(object sender, CancelEventArgs e)
    {
        OrderInfoViewModel.Instance.WindowClosing();
    }


    private void Thumbnails_Drop(object sender, System.Windows.DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            OrderInfoViewModel.Instance.SaveImagesInto3ShapeOrder(files);
        }
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
