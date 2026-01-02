using Microsoft.Data.SqlClient;
using StatsClient.MVVM.Core;
using StatsClient.MVVM.View;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using static StatsClient.MVVM.Core.Enums;
using static StatsClient.MVVM.Core.Functions;
using static StatsClient.MVVM.Core.LocalSettingsDB;
using static StatsClient.MVVM.ViewModel.MainViewModel;

namespace StatsClient.MVVM.ViewModel;

public class SplashViewModel : ObservableObject
{
    public MainWindow? mainWindow;
    public bool isEverythingOkay = true;
    private readonly DispatcherTimer timerCheckServerConnectionFirstTime = new ();
    
    private static SplashViewModel? instance;
    public static SplashViewModel Instance
    {
        get => instance!;
        set
        {
            instance = value;
            RaisePropertyChangedStatic(nameof(Instance));
        }
    }

    private string loadingText = "";
    public string LoadingText
    {
        get => loadingText;
        set
        {
            loadingText = value;
            RaisePropertyChanged(nameof(LoadingText));
        }
    }
    
    private SMessageBoxResult sMessageBoxxResult;
    public SMessageBoxResult SMessageBoxxResult
    {
        get => sMessageBoxxResult;
        set
        {
            sMessageBoxxResult = value;
            RaisePropertyChanged(nameof(SMessageBoxxResult));
        }
    }

    private string softwareVersion = "0";
    public string SoftwareVersion
    {
        get => softwareVersion;
        set
        {
            softwareVersion = value;
            RaisePropertyChanged(nameof(SoftwareVersion));
        }
    }

    private bool finishedWithServerConnectionCheck;
    public bool FinishedWithServerConnectionCheck
    {
        get => finishedWithServerConnectionCheck;
        set
        {
            finishedWithServerConnectionCheck = value;
            RaisePropertyChanged(nameof(FinishedWithServerConnectionCheck));
        }
    }

    private bool cbSettingGlassyEffect = true;
    public bool CbSettingGlassyEffect
    {
        get => cbSettingGlassyEffect;
        set
        {
            cbSettingGlassyEffect = value;
            RaisePropertyChanged(nameof(CbSettingGlassyEffect));
        }
    }
    
    private ImageSource backgroundPicture;
    public ImageSource BackgroundPicture
    {
        get => backgroundPicture;
        set
        {
            backgroundPicture = value;
            RaisePropertyChanged(nameof(BackgroundPicture));
        }
    }
    
    private Visibility windowPosResetText = Visibility.Hidden;
    public Visibility WindowPosResetText
    {
        get => windowPosResetText;
        set
        {
            windowPosResetText = value;
            RaisePropertyChanged(nameof(WindowPosResetText));
        }
    }

    
    public RelayCommand ResetWindowPositionCommand { get; set; }


    public SplashViewModel()
    {
        Instance = this;

        _ = SetAppVersion();

        timerCheckServerConnectionFirstTime.Tick += TimerCheckServerConnectionFirstTime_Tick;
        timerCheckServerConnectionFirstTime.Interval = new TimeSpan(0, 0, 1);

        _ = bool.TryParse(ReadLocalSetting("GlassyEffect"), out bool GlassyEffect);
        CbSettingGlassyEffect = GlassyEffect;

        ResetWindowPositionCommand = new RelayCommand(o => ResetWindowPosition());

        int day = 31;
        day = DateTime.Now.Day;
        BackgroundPicture = new BitmapImage(new Uri($"/Images/Splash/{day}.jpg", UriKind.Relative));
    }

    private void ResetWindowPosition()
    {
        WriteLocalSetting("WindowTop", "10");
        WriteLocalSetting("WindowLeft", "10");

        WriteLocalSetting("WindowWidth", "1120");
        WriteLocalSetting("WindowHeight", "550");

        WindowPosResetText = Visibility.Visible;
    }
    
    private async Task CheckSavedWindowPosition()
    {
        _ = double.TryParse(ReadLocalSetting("WindowTop"), out double WindowTop);
        _ = double.TryParse(ReadLocalSetting("WindowLeft"), out double WindowLeft);

        _ = double.TryParse(ReadLocalSetting("WindowWidth"), out double WindowWidth);
        _ = double.TryParse(ReadLocalSetting("WindowHeight"), out double WindowHeight);

        double screenWidth = SystemParameters.WorkArea.Width;
        double screenHeight = SystemParameters.WorkArea.Height;

        if (WindowTop < 0 ||
            WindowTop > screenHeight ||
            WindowLeft < 0 ||
            WindowLeft > screenWidth)
        {
            WriteLocalSetting("WindowTop", "5");
            WriteLocalSetting("WindowLeft", "5");
            WindowPosResetText = Visibility.Visible;
        }
         
        if (WindowWidth < 1120 || WindowWidth > screenWidth)
            WriteLocalSetting("WindowWidth", "1120");
        
        if (WindowHeight < 550 ||WindowHeight > screenHeight)
            WriteLocalSetting("WindowHeight", "550");

        await Task.Delay(10);
    }



    private async Task SetAppVersion()
    {
        SoftwareVersion = await GetAppVersion();
    }

    private void TimerCheckServerConnectionFirstTime_Tick(object? sender, EventArgs e)
    {
        timerCheckServerConnectionFirstTime.Stop();
        CheckStatsServerConnection();
    }

    internal async void StartLoading()
    {
        await Task.Run(DatabaseConnection.SetCredentials); // getting credentials for SQL server from BaseSettings.Config file

        LoadingText = "Checking local configs..";
        await Task.Run(CreatingLocalConfigFiles); // first try.. initialize database

        timerCheckServerConnectionFirstTime.Start();
    }

    private async void CheckStatsServerConnection()
    {
        isEverythingOkay = true;
        await Task.Run(CreatingLocalConfigFiles); // double tap.. to make sure database initialized correctly
        LoadingText = "Checking server connection..";
        await Task.Run(() => Thread.Sleep(500));
        try
        {
            using (var connection = new SqlConnection(DatabaseConnection.ConnectionStrToStatsDatabase()))
            {
                var query = "select 1";
                var command = new SqlCommand(query, connection);
                connection.Open();
                command.ExecuteScalar();
            }
            FinishedWithServerConnectionCheck = true;
            LoadingText = "Successfully connected to server!";
            await Task.Run(() => Thread.Sleep(500));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
            MainViewModel.Instance?.AddDebugLine(ex);
            if (ex.Message.Contains("Login failed for user"))
            {
                ShowMessageBox("Error", $"{ex.Message}\n\nApplication will shutdown!", SMessageBoxButtons.Ok, NotificationIcon.Error, 15, SplashWindow.Instance);
                SplashWindow.Instance.Close();
                return;
            }
            else
            {
                try
                {
                    DatabaseConnection.StatsdbInstance = "";
                    using (var connection = new SqlConnection(DatabaseConnection.ConnectionStrToStatsDatabase()))
                    {
                        var query = "select 1";
                        var command = new SqlCommand(query, connection);
                        connection.Open();
                        command.ExecuteScalar();
                    }
                    FinishedWithServerConnectionCheck = true;
                    LoadingText = "Successfully connected to server!";
                    await Task.Run(() => Thread.Sleep(500));
                }
                catch (Exception exx)
                {
                    MainViewModel.Instance?.AddDebugLine(exx);
                    Debug.WriteLine($"[{exx.LineNumber()}] {exx.Message}");
                    isEverythingOkay = false;
                    LoadingText = "Couldn't connect to server..";
                    await Task.Run(() => Thread.Sleep(500));
                    LoadingText = exx.Message;
                }
            }
        }

        FinishedWithServerConnectionCheck = isEverythingOkay;
        AfterServerConnectionChecked();
    }

    private async void AfterServerConnectionChecked()
    {
        if (!isEverythingOkay)
        {
            SMessageBoxResult dg = ShowMessageBox("Error", $"Could not connect to DataBase server!\nServer might be offline or not accessible.", SMessageBoxButtons.TryAgainClose, NotificationIcon.Warning, 15, SplashWindow.Instance);
            if (dg == SMessageBoxResult.TryAgain)
            {
                isEverythingOkay = true;
                CheckStatsServerConnection();
            }
            else if (dg == SMessageBoxResult.Close)
            {
                SplashWindow.Instance.Close();
            }
        }
        else
        {
            await CheckSavedWindowPosition();

            mainWindow = new();
            mainWindow.Show();
            mainWindow.Hide();


            MainViewModel.StartInitialTasks();
        }
    }

    public SMessageBoxResult ShowMessageBox(string Title, string Message, SMessageBoxButtons Buttons,
                                              NotificationIcon MessageBoxIcon,
                                              double DismissAfterSeconds = 300,
                                              Window? Owner = null)
    {
        SMessageBox sMessageBox = new(Title, Message, Buttons, MessageBoxIcon, DismissAfterSeconds);
        if (Owner is null)
            sMessageBox.Owner = MainWindow.Instance;
        else
            sMessageBox.Owner = Owner;

        sMessageBox.ShowDialog();

        if (MainViewModel.Instance is not null)
            return MainViewModel.Instance.SMessageBoxxResult;
        else
            return SMessageBoxxResult;
    }
}
