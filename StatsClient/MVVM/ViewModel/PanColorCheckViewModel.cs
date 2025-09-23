using StatsClient.MVVM.Core;
using StatsClient.MVVM.View;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using static StatsClient.MVVM.Core.DatabaseOperations;
using static StatsClient.MVVM.Core.Functions;
using static StatsClient.MVVM.Core.LocalSettingsDB;
using static StatsClient.MVVM.ViewModel.MainViewModel;

namespace StatsClient.MVVM.ViewModel;

public class PanColorCheckViewModel : ObservableObject
{
    private PanColorCheckViewModel? instance;
    public PanColorCheckViewModel Instance
    {
        get => instance!;
        set
        {
            instance = value;
            RaisePropertyChanged(nameof(Instance));
        }
    }
    
    private static PanColorCheckViewModel? staticInstance;
    public static PanColorCheckViewModel StaticInstance
    {
        get => staticInstance!;
        set
        {
            staticInstance = value;
            RaisePropertyChangedStatic(nameof(StaticInstance));
        }
    }

    private bool pancolorCheckWindowIsSnapped = true;
    public bool PancolorCheckWindowIsSnapped
    {
        get => pancolorCheckWindowIsSnapped!;
        set
        {
            pancolorCheckWindowIsSnapped = value;
            RaisePropertyChangedStatic(nameof(PancolorCheckWindowIsSnapped));
        }
    }
    
    
    private bool addingANumberNow = false;
    public bool AddingANumberNow
    {
        get => addingANumberNow!;
        set
        {
            addingANumberNow = value;
            RaisePropertyChangedStatic(nameof(AddingANumberNow));
        }
    }

    private string pcPanNumber = "";
    public string PcPanNumber
    {
        get => pcPanNumber;
        set
        {
            if (pcPanNumber != value)
            {
                pcPanNumber = value;
                RaisePropertyChanged(nameof(PcPanNumber));
            }
        }
    }
    
    private string previousPcPanNumber = "";
    public string PreviousPcPanNumber
    {
        get => previousPcPanNumber;
        set
        {
            previousPcPanNumber = value;
            RaisePropertyChanged(nameof(PreviousPcPanNumber));
        }
    }

    private bool isItDarkColor = true;
    public bool IsItDarkColor
    {
        get => isItDarkColor;
        set
        {
            isItDarkColor = value;
            RaisePropertyChanged(nameof(IsItDarkColor));
        }
    }

    private Visibility panColorShowsNow = Visibility.Hidden;
    public Visibility PanColorShowsNow
    {
        get => panColorShowsNow;
        set
        {
            panColorShowsNow = value;
            RaisePropertyChanged(nameof(PanColorShowsNow));
        }
    }
    
    private Visibility noNumberRegisteredShowsNow = Visibility.Hidden;
    public Visibility NoNumberRegisteredShowsNow
    {
        get => noNumberRegisteredShowsNow;
        set
        {
            noNumberRegisteredShowsNow = value;
            RaisePropertyChanged(nameof(NoNumberRegisteredShowsNow));
        }
    }
    
    private Visibility hideLabelVisibility = Visibility.Hidden;
    public Visibility HideLabelVisibility
    {
        get => hideLabelVisibility;
        set
        {
            hideLabelVisibility = value;
            RaisePropertyChanged(nameof(HideLabelVisibility));
        }
    }
    
    private string pcPanColor = "#888";
    public string PcPanColor
    {
        get => pcPanColor;
        set
        {
            pcPanColor = value;
            RaisePropertyChanged(nameof(PcPanColor));
        }
    }
    
    private string panColorWindowBorderColor = "BlanchedAlmond";
    public string PanColorWindowBorderColor
    {
        get => panColorWindowBorderColor;
        set
        {
            panColorWindowBorderColor = value;
            RaisePropertyChanged(nameof(PanColorWindowBorderColor));
        }
    }
    
    private string originalRgbColor = "";
    public string OriginalRgbColor
    {
        get => originalRgbColor;
        set
        {
            originalRgbColor = value;
            RaisePropertyChanged(nameof(OriginalRgbColor));
        }
    }

    private string pcPanColorFriendlyName = "Check Pan Color";
    public string PcPanColorFriendlyName
    {
        get => pcPanColorFriendlyName;
        set
        {
            pcPanColorFriendlyName = value;
            RaisePropertyChanged(nameof(PcPanColorFriendlyName));
        }
    }

    private readonly DispatcherTimer WindowHideTimer = new();
    private readonly DispatcherTimer MouseReleaseTimer = new();

    public RelayCommand CloseWindowCommand { get; set; }
    public RelayCommand PcCheckPanColorCommand { get; set; }
    public RelayCommand ChangeColorCommand { get; set; }
    public RelayCommand AddNewNumberCommand { get; set; }
    public RelayCommand HidePanColorCheckWindowCommand { get; set; }

    public PanColorCheckViewModel()
    {
        Instance = this;
        StaticInstance = this;

        CloseWindowCommand = new RelayCommand(o => CloseWindow());
        PcCheckPanColorCommand = new RelayCommand(o => PcCheckPanColor());
        ChangeColorCommand = new RelayCommand(o => ChangeColor());
        AddNewNumberCommand = new RelayCommand(o => AddNewNumber());
        HidePanColorCheckWindowCommand = new RelayCommand(o => HidePanColorCheckWindow());

        WindowHideTimer.Tick += WindowHideTimer_Tick;
        WindowHideTimer.Interval = new TimeSpan(0, 0, 10);

        MouseReleaseTimer.Tick += MouseReleaseTimer_Tick;
        MouseReleaseTimer.Interval = new TimeSpan(0, 0, 5);

        PancolorCheckWindowIsSnapped = MainWindow.Instance.PancolorCheckWindowIsSnapped;
    }

    private void MouseReleaseTimer_Tick(object? sender, EventArgs e)
    {
        AddingANumberNow = false;
        MouseReleaseTimer.Stop();
    }

    public void ShowHideLabel()
    {
        HideLabelVisibility = Visibility.Visible;
    }

    private void WindowHideTimer_Tick(object? sender, EventArgs e)
    {
        PanColorCheckWindow.StaticInstance.Show();
        WindowHideTimer.Stop();
    }

    private void HidePanColorCheckWindow()
    {
        HideLabelVisibility = Visibility.Hidden;
        PanColorCheckWindow.StaticInstance.Hide();
        WindowHideTimer.Start();
    }

    private void AddNewNumber()
    {
        AddingANumberNow = true;
        _ = bool.TryParse(ReadLocalSetting("PanColorCheckWndwIsSnapped"), out bool panColorCheckWndwIsSnapped);
        PancolorCheckWindowIsSnapped = panColorCheckWndwIsSnapped;

        SetPanColorWindow setPanColorWindow = new(PreviousPcPanNumber, "0-0-0");

        if (PancolorCheckWindowIsSnapped)
            setPanColorWindow.Owner = SnappedPanColorCheckWindow.StaticInstance;
        else
            setPanColorWindow.Owner = PanColorCheckWindow.StaticInstance;

        setPanColorWindow.ShowDialog();
        MouseReleaseTimer.Start();
    }

    private void ChangeColor()
    {
        AddingANumberNow = true;
        _ = bool.TryParse(ReadLocalSetting("PanColorCheckWndwIsSnapped"), out bool panColorCheckWndwIsSnapped);
        PancolorCheckWindowIsSnapped = panColorCheckWndwIsSnapped;

        SetPanColorWindow setPanColorWindow = new(PreviousPcPanNumber, OriginalRgbColor);
        
        if (PancolorCheckWindowIsSnapped)
            setPanColorWindow.Owner = SnappedPanColorCheckWindow.StaticInstance;
        else
            setPanColorWindow.Owner = PanColorCheckWindow.StaticInstance;

        setPanColorWindow.ShowDialog();
        MouseReleaseTimer.Start();
    }

    private async void PcCheckPanColor()
    {
        string panNumber = PcPanNumber;
        if (panNumber.Length < 1)
            return;

        if (!int.TryParse(panNumber, out int num))
        {
            PcPanNumber = "";
            return;
        }

        PreviousPcPanNumber = panNumber;

        string rgbColor = GetPanColorByNumber(num);
        if (rgbColor == "0-0-0")
        {
            HideLabelVisibility = Visibility.Hidden;
            NoNumberRegisteredShowsNow = Visibility.Visible;
            PcPanColor = "#46494F";
            PcPanColorFriendlyName = "NUMBER NOT REGISTERED!!";
            PcPanNumber = "";

            await Task.Delay(3500);
            PcPanColor = "#888";
            PcPanColorFriendlyName = "Check Pan Color";
            NoNumberRegisteredShowsNow = Visibility.Hidden;
            //HideLabelVisibility = Visibility.Visible;
        }
        else
        {
            IsItDarkColor = CheckIfItsDarkColor(rgbColor);

            HideLabelVisibility = Visibility.Hidden;
            PanColorShowsNow = Visibility.Visible;
            string[] panColorParts = rgbColor.Split('-');

            _ = int.TryParse(panColorParts[0], out int colorR);
            _ = int.TryParse(panColorParts[1], out int colorG);
            _ = int.TryParse(panColorParts[2], out int colorB);

            Brush panColor = new SolidColorBrush(Color.FromArgb(255, (byte)colorR, (byte)colorG, (byte)colorB));
            PcPanColor = panColor.ToString();
            OriginalRgbColor = PcPanColor;
            PcPanColorFriendlyName = GetPanColorNameByNumber(num);
            PcPanNumber = "";

            await Task.Delay(3500);
            PcPanColor = "#888";
            PcPanColorFriendlyName = "Check pan color";
            PanColorShowsNow = Visibility.Hidden;
            //HideLabelVisibility = Visibility.Visible;
            IsItDarkColor = true;
        }
    }

    private void CloseWindow()
    {
        PanColorCheckWindow.StaticInstance.Hide();
        try
        {
            PanColorCheckWindow.StaticInstance.Top = (((MainWindow)PanColorCheckWindow.StaticInstance.Owner).Height / 2 + ((MainWindow)PanColorCheckWindow.StaticInstance.Owner).Top)-50;
            PanColorCheckWindow.StaticInstance.Left = (((MainWindow)PanColorCheckWindow.StaticInstance.Owner).Width / 2 + ((MainWindow)PanColorCheckWindow.StaticInstance.Owner).Left)-100;
        }
        catch (Exception ex)
        {
            MainViewModel.Instance.AddDebugLine(ex);
        }

        LocalSettingsDB.WriteLocalSetting("ColorCheckWindowPosTop", "");
        LocalSettingsDB.WriteLocalSetting("ColorCheckWindowPosLeft", "");
        LocalSettingsDB.WriteLocalSetting("ColorCheckWindowIsOpen", "false");
    }
}
