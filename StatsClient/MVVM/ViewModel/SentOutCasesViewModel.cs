﻿using StatsClient.MVVM.Core;
using StatsClient.MVVM.Model;
using static StatsClient.MVVM.Core.DatabaseOperations;
using static StatsClient.MVVM.ViewModel.MainViewModel;
using System.Windows;
using System.Timers;
using StatsClient.MVVM.View;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace StatsClient.MVVM.ViewModel;

public partial class SentOutCasesViewModel : ObservableObject
{
    #region Properties
    private bool panelsAddedalready = false;
    public bool PanelsAddedalready
    {
        get => panelsAddedalready;
        set
        {
            panelsAddedalready = value;
            RaisePropertyChanged(nameof(PanelsAddedalready));
        }
    }
    
    private SentOutCasesViewModel? instance;
    public SentOutCasesViewModel? Instance
    {
        get => instance;
        set
        {
            instance = value;
            RaisePropertyChanged(nameof(Instance));
        }
    }
    
    private static SentOutCasesViewModel? staticInstance;
    public static SentOutCasesViewModel? StaticInstance
    {
        get => staticInstance;
        set
        {
            staticInstance = value;
            RaisePropertyChangedStatic(nameof(StaticInstance));
        }
    }

    private int panelCount = 1;
    public int PanelCount
    {
        get => panelCount;
        set
        {
            panelCount = value;
            RaisePropertyChanged(nameof(PanelCount));
        }
    }

    private List<DesignerModel>? designersModel;
    public List<DesignerModel> DesignersModel
    {
        get => designersModel!;
        set
        {
            designersModel = value;
            RaisePropertyChanged(nameof(DesignersModel));
        }
    }
    
    
    private Dictionary<string, double>? designerPagesTotalUnits = [];
    public Dictionary<string, double> DesignerPagesTotalUnits
    {
        get => designerPagesTotalUnits!;
        set
        {
            designerPagesTotalUnits = value;
            RaisePropertyChanged(nameof(DesignerPagesTotalUnits));
        }
    }
    
    private bool startCheckingForUnitNumbersToo = false;
    public bool StartCheckingForUnitNumbersToo
    {
        get => startCheckingForUnitNumbersToo!;
        set
        {
            startCheckingForUnitNumbersToo = value;
            RaisePropertyChanged(nameof(StartCheckingForUnitNumbersToo));
        }
    }


    private StatsDBSettingsModel? serverInfoModel;
    public StatsDBSettingsModel ServerInfoModel
    {
        get => serverInfoModel!;
        set
        {
            serverInfoModel = value;
            RaisePropertyChanged(nameof(ServerInfoModel));
        }
    }



    private string lastDBUpdate = "";
    public string LastDBUpdate
    {
        get => lastDBUpdate;
        set
        {
            lastDBUpdate = value;
            RaisePropertyChanged(nameof(LastDBUpdate));
        }
    }

    private string lastDBUpdateLocalTime = "Fetching data..";
    public string LastDBUpdateLocalTime
    {
        get => lastDBUpdateLocalTime;
        set
        {
            lastDBUpdateLocalTime = value;
            RaisePropertyChanged(nameof(LastDBUpdateLocalTime));
        }
    }

    private double updateTimeOpacity = 1;
    public double UpdateTimeOpacity
    {
        get => updateTimeOpacity;
        set
        {
            updateTimeOpacity = value;
            RaisePropertyChanged(nameof(UpdateTimeOpacity));
        }
    }

    private string statusColor = "LightGreen";
    public string StatusColor
    {
        get => statusColor;
        set
        {
            statusColor = value;
            RaisePropertyChanged(nameof(StatusColor));
        }
    }

    private string updateTimeColor = "LightGreen";
    public string UpdateTimeColor
    {
        get => updateTimeColor;
        set
        {
            updateTimeColor = value;
            RaisePropertyChanged(nameof(UpdateTimeColor));
        }
    }
    

    private ObservableCollection<CheckedOutCasesModel> sentOutCasesModel = [];
    public ObservableCollection<CheckedOutCasesModel> SentOutCasesModel
    {
        get => sentOutCasesModel;
        set
        {
            sentOutCasesModel = value;
            RaisePropertyChanged(nameof(SentOutCasesModel));
        }
    }

    
    #endregion Properties

    private int Counter = 10;
    public System.Timers.Timer _timer;

    public SentOutCasesViewModel()
    {
        Instance = this;
        StaticInstance = this;
        MainViewModel.Instance.SentOutCasesViewModel = this;


        _timer = new System.Timers.Timer(10000);
        _timer.Elapsed += Timer_Elapsed;
        _timer.Start();

        _ = GetServerInfo();
    }


    private async void Timer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        if (ServerInfoModel is not null)
        {
            if (LastDBUpdate != ServerInfoModel.LastDBUpdate && !ServerInfoModel.ServerIsWritingDatabase)
            {
                LastDBUpdateLocalTime = DateTime.Now.ToString("MMM d - h:mm:ss tt");
                UpdateTimeColor = "LightGreen";
                UpdateTimeOpacity = 1;
                LastDBUpdate = ServerInfoModel.LastDBUpdate!;
            }
                    
            await GetServerInfo();

            // creating panels corresponding to the number of designers
            if (!PanelsAddedalready)
            {
                DesignersModel = await GetDesignersModel();
                PanelCount = DesignersModel.Count;

                PanelsAddedalready = true;
                for (int i = 0; i < PanelCount; i++)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        string designerID = DesignersModel.ToArray()[i].DesignerID!;

                        UserPanel userPanel = new(designerID);
                        ColumnDefinition column = new()
                        {
                            Width = new GridLength(1, GridUnitType.Star),
                            Name = $"gridColumn{designerID}"
                        };
                        SentOutCasesPage.Instance.mainGrid.ColumnDefinitions.Add(column);
                        Grid.SetColumn(userPanel, i);

                        DesignerPagesTotalUnits.TryAdd(designerID, 0);


                        SentOutCasesPage.Instance.mainGrid.Children.Add(userPanel);
                    });
                }
            }
            else
                await GetTotalUnitsForEachDesigner();


            if (DesignerPagesTotalUnits.Count > 0 && PanelsAddedalready)
            {
                foreach (var item in DesignerPagesTotalUnits)
                {
                    if (item.Value < 1)
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() => {
                            SentOutCasesPage.Instance.mainGrid.ColumnDefinitions.FirstOrDefault(x => x.Name == $"gridColumn{item.Key}")!.Width = new GridLength(1, GridUnitType.Pixel);
                        }));
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() => {
                            SentOutCasesPage.Instance.mainGrid.ColumnDefinitions.FirstOrDefault(x => x.Name == $"gridColumn{item.Key}")!.Width = new GridLength(1, GridUnitType.Star);
                        }));
                    }
                }
            }
        }
    }

    private async Task GetTotalUnitsForEachDesigner()
    {
        if (DesignerPagesTotalUnits.Count < 0)
            return;

        List<DesignerUnitsModel> designerUnitsModels = await GetDesignerUnitsModel();
        if (designerUnitsModels.Count > 0)
        {
            foreach (var item in designerUnitsModels)
            {
                DesignerPagesTotalUnits[item.DesignerID!] = item.TotalUnits;
            }
        }
    }


    private async Task GetServerInfo()
    {
        try
        {
            ServerInfoModel = await Task.Run(GetStatsDBSettingsModel);
        }
        catch (Exception ex)
        {
            MainViewModel.Instance.AddDebugLine(ex);
        }
    }
}
