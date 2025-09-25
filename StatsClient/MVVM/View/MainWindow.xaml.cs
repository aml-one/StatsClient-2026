using Microsoft.Web.WebView2.Core;
using StatsClient.MVVM.Core;
using StatsClient.MVVM.ViewModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using static StatsClient.MVVM.Core.DatabaseOperations;
using static StatsClient.MVVM.Core.Functions;
using static StatsClient.MVVM.Core.LocalSettingsDB;
using static System.Windows.Forms.LinkLabel;
using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;

namespace StatsClient.MVVM.View;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, INotifyPropertyChanged
{
    
    public static event PropertyChangedEventHandler? PropertyChangedStatic;
    public event PropertyChangedEventHandler? PropertyChanged;

    public static void RaisePropertyChangedStatic([CallerMemberName] string? propertyname = null)
    {
        PropertyChangedStatic?.Invoke(typeof(ObservableObject), new PropertyChangedEventArgs(propertyname));
    }

    private static MainWindow? instance;
    public static MainWindow Instance
    {
        get => instance!;
        set
        {
            instance = value;
            RaisePropertyChangedStatic(nameof(Instance));
        }
    }
    
    public MainWindow(string url)
    {
        InitializeComponent();

        webviewLabnext.Source = new Uri(url);
    }

    public MainWindow()
    {
        //Register Syncfusion license
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NBaF1cWWhIfEx1RHxQdld5ZFRHallYTnNWUj0eQnxTdEFjXH1fcH1QR2VdVE1xWw==");
        Instance = this;
        InitializeComponent();
        DataContext = MainViewModel.Instance;

        MainViewModel.Instance._MainWindow = this;
        pb3ShapeProgressBar.Value = 0;
        pbArchivesProgressBar.Value = 0;

        _ = int.TryParse(ReadLocalSetting("WindowWidth"), out int wWidth);
        _ = int.TryParse(ReadLocalSetting("WindowHeight"), out int wHeight);
        _ = int.TryParse(ReadLocalSetting("WindowTop"), out int wTop);
        _ = int.TryParse(ReadLocalSetting("WindowLeft"), out int wLeft);

        Width = wWidth;
        Height = wHeight;
        Top = wTop;
        Left = wLeft;

        string groupProp = ReadLocalSetting("GroupBy");
        if (groupProp != null)
        {
            GroupBy.SelectedItem = groupProp;
            MainViewModel.Instance.GroupList();
        }

        string filterUsed = ReadLocalSetting("FilterUsed");
        if (!string.IsNullOrEmpty(filterUsed)) 
        {
            MainViewModel.Instance.Search(filterUsed, true);
            MainViewModel.Instance.ShowNotificationMessage("Startup", "Last view was restored!");
        }

        tbSearch.PreviewKeyDown += new KeyEventHandler(HandleEsc);

        zipArchiveIcon.Width = 0;
    }


    private async void Window_Closing(object sender, CancelEventArgs e)
    {
        await ResetPingDifferenceInDatabaseOnClose();
        Application.Current.Shutdown();
    }

    public void TitleBar_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount >= 2)
            BtnMaximize_Click(sender, e);

        if (e.ChangedButton == MouseButton.Left)
            try
            {
                this.DragMove();
            }
            catch { }
    }

    private void BtnMinimize_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void BtnMaximize_Click(object sender, RoutedEventArgs e)
    {
        MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;

        if (WindowState == WindowState.Maximized)
        {
            WindowState = WindowState.Normal;
            this.BorderThickness = new Thickness(0);
            btnMaximize.Content = "▣";
        }
        else if (WindowState == WindowState.Normal)
        {
            WindowState = WindowState.Maximized;
            this.BorderThickness = new Thickness(6, 6, 6, 6);
            btnMaximize.Content = "⧉";
        }

    }

    private async void BtnCloseApplication_Click(object sender, RoutedEventArgs e)
    {
        await ResetPingDifferenceInDatabaseOnClose();
        Application.Current.Shutdown();
    }

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;

        if (WindowState == WindowState.Maximized)
        {
            this.BorderThickness = new Thickness(6, 6, 6, 6);
            btnMaximize.Content = "⧉";
        }
        else if (WindowState == WindowState.Normal)
        {
            this.BorderThickness = new Thickness(0);
            btnMaximize.Content = "▣";
        }

        if (WindowState != WindowState.Minimized)
        {
            WriteLocalSetting("WindowWidth", Width.ToString());
            WriteLocalSetting("WindowHeight", Height.ToString());
        }

       
    }

    private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!ThreeShapeTab.IsSelected)
        {
            MainViewModel.Instance.ThreeShapeObject = null;
            MainViewModel.Instance.Is3ShapeTabSelected = false;
        }
        else
        {
            MainViewModel.Instance.Is3ShapeTabSelected = true;
        }

        if (infoTab is not null)
            if (!infoTab.IsSelected)
                aboutTab.IsSelected = true;
    }

    private void BtnFilter_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            Button? button = sender as Button;
            ContextMenu contextMenu = button!.ContextMenu;
            contextMenu.PlacementTarget = button;
            contextMenu.Placement = PlacementMode.Top;
            contextMenu.IsOpen = true;
            e.Handled = true;
        }
    }

    private void GridViewColumnHeader_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        // preventing icon column from resize
        e.Handled = true;
        ((GridViewColumnHeader)sender).Column.Width = 117;
    }
    
    private void GridViewForButtonsColumnHeader_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        // preventing button column from resize
        e.Handled = true;
        ((GridViewColumnHeader)sender).Column.Width = 170;
    }
    
    private void GridViewForShadeColumnHeader_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        // preventing shade column from resize
        e.Handled = true;
        ((GridViewColumnHeader)sender).Column.Width = 44;
    }

    private void Window_LocationChanged(object sender, EventArgs e)
    {
        WriteLocalSetting("WindowTop", Top.ToString());
        WriteLocalSetting("WindowLeft", Left.ToString());
    }

    
    private void ListView3ShapeOrders_MouseDown(object sender, MouseButtonEventArgs e)
    {
        tbSearch.Focus();
    }

    private void HandleEsc(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
            tbSearch.Clear();
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        settingsTab.IsSelected = true;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        MainViewModel.Instance.SmartOrderNamesWindow.Owner = this;
    }

    private void Webview_CoreWebView2InitializationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
    {
        webview.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
        webview.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;

        MainViewModel.Instance.ServerLogWebViewIsInitialized = true;
    }

    private void Webview_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        MainViewModel.Instance.ScrollServerLogToBottom = false;
    }

    private void WebviewLabnext_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
    {
        webviewLabnext.CoreWebView2.Settings.IsPasswordAutosaveEnabled = true;
        webviewLabnext.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
        webviewLabnext.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
        webviewLabnext.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = true;
        webviewLabnext.CoreWebView2.Settings.IsScriptEnabled = true;
        webviewLabnext.CoreWebView2.Settings.IsWebMessageEnabled = true;
        webviewLabnext.CoreWebView2.Settings.IsZoomControlEnabled = false;
        webviewLabnext.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
    }

    private void CoreWebView2_NewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
    {
        //CoreWebView2 cwv2 = (CoreWebView2)sender;

        CoreWebView2Deferral deferral = e.GetDeferral();

        //LabnextChildWindow childWindow = new(e.Uri)
        //{
        //    Title = "Child Window"
        //};
        //childWindow.Show();

        //e.Handled = true;
        deferral.Complete();
    }

    private void ArcGridViewColumnHeaderIcon_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        // preventing column from resize
        e.Handled = true;
        ((GridViewColumnHeader)sender).Column.Width = 30;
    }

    private void ArcGridViewColumnHeaderCaseId_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        // preventing column from resize
        e.Handled = true;
        ((GridViewColumnHeader)sender).Column.Width = 460;
    }

    private void ArcGridViewColumnHeaderAction_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        // preventing column from resize
        e.Handled = true;
        ((GridViewColumnHeader)sender).Column.Width = 100;
    }

    private void ArcGridViewColumnHeaderDesigner_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        // preventing column from resize
        e.Handled = true;
        ((GridViewColumnHeader)sender).Column.Width = 140;
    }

    private void ArcGridViewColumnHeaderFromYear_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        // preventing column from resize
        e.Handled = true;
        ((GridViewColumnHeader)sender).Column.Width = 80;
    }

    private void ArcGridViewColumnHeaderCustomer_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        // preventing column from resize
        e.Handled = true;
        ((GridViewColumnHeader)sender).Column.Width = 272;
    }

    private void ArcGridViewColumnHeaderDates_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        // preventing column from resize
        e.Handled = true;
        ((GridViewColumnHeader)sender).Column.Width = 260;
    }

    
    private void WebviewLabnext_ContentLoading(object sender, CoreWebView2ContentLoadingEventArgs e)
    {
        if (webviewLabnext.Source is not null)
            MainViewModel.Instance.LabNextWebViewStatusText = webviewLabnext.Source.ToString().Replace($"https://{MainViewModel.Instance.LabnextLabID}.labnext.net/lab", "");
    }
}
