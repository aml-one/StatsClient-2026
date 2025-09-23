using StatsClient.MVVM.Core;
using StatsClient.MVVM.Model;
using static StatsClient.MVVM.Core.DatabaseOperations;
using static StatsClient.MVVM.Core.Functions;
using static StatsClient.MVVM.Core.LocalSettingsDB;
using System.Collections.ObjectModel;
using System.Timers;
using StatsClient.MVVM.View;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Diagnostics;
using static StatsClient.MVVM.Core.Enums;
using static StatsClient.MVVM.ViewModel.MainViewModel;
using System.Text.RegularExpressions;



namespace StatsClient.MVVM.ViewModel;

public partial class SmartOrderNamesViewModel : ObservableObject
{
    private static SmartOrderNamesViewModel? staticInstance;
    public static SmartOrderNamesViewModel StaticInstance
    {
        get => staticInstance!;
        set
        {
            staticInstance = value;
            RaisePropertyChangedStatic(nameof(StaticInstance));
        }
    }

    private ObservableCollection<ThreeShapeOrdersModel> newOrdersByMe = [];
    public ObservableCollection<ThreeShapeOrdersModel> NewOrdersByMe
    {
        get => newOrdersByMe;
        set
        {
            if (value != newOrdersByMe)
            {
                newOrdersByMe = value;
                RaisePropertyChanged(nameof(NewOrdersByMe));
                if (NewOrdersByMe.Count == 0 && PreviouslySelectedOrder is not null)
                {
                    ResetNameForm();
                }
            }
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

    private readonly List<string> digitalSystems = ["None", "ABUTMENT-ONLY", "ATLANTIS", "CARESTREAM", "DEXIS", "DSCORE", "DROPBOX", "EMAIL", "MEDIT", "TRIOS", "iTERO", "IS3D", "SIRONA"  ];
    public List<string> DigitalSystems
    {
        get => digitalSystems;
    }

    private string logMessage = "";
    public string LogMessage
    {
        get => logMessage!;
        set
        {
            logMessage = value;
            RaisePropertyChanged(nameof(LogMessage));
        }
    }

    private List<string> logMessages = [];
    public List<string> LogMessages
    {
        get => logMessages!;
        set
        {
            logMessages = value;
            RaisePropertyChanged(nameof(LogMessages));
        }
    }
    
    private List<string> customerSuggestionsList = [];
    public List<string> CustomerSuggestionsList
    {
        get => customerSuggestionsList!;
        set
        {
            customerSuggestionsList = value;
            RaisePropertyChanged(nameof(CustomerSuggestionsList));
        }
    }

    private string selectedCustomerName;
    public string SelectedCustomerName
    {
        get => selectedCustomerName;
        set
        {
            selectedCustomerName = value;
            RaisePropertyChanged(nameof(SelectedCustomerName));
            BuildName();
            FocusOnRenameButton();
        }
    }
    
    private string panNumber;
    public string PanNumber
    {
        get => panNumber;
        set
        {
            panNumber = value;
            RaisePropertyChanged(nameof(PanNumber));
        }
    }
    
    private string carestreamID = "";
    public string CarestreamID
    {
        get => carestreamID;
        set
        {
            carestreamID = value;
            RaisePropertyChanged(nameof(CarestreamID));
        }
    }

    private bool namingCustomerFirst = false;
    public bool NamingCustomerFirst
    {
        get => namingCustomerFirst;
        set
        {
            namingCustomerFirst = value;
            RaisePropertyChanged(nameof(NamingCustomerFirst));
        }
    }
    
    private bool autoSelectFirstOrder = true;
    public bool AutoSelectFirstOrder
    {
        get => autoSelectFirstOrder;
        set
        {
            autoSelectFirstOrder = value;
            RaisePropertyChanged(nameof(AutoSelectFirstOrder));
        }
    }
    
    private bool smartOrderNamesModuleIsActive = true;
    public bool SmartOrderNamesModuleIsActive
    {
        get => smartOrderNamesModuleIsActive;
        set
        {
            smartOrderNamesModuleIsActive = value;
            RaisePropertyChanged(nameof(SmartOrderNamesModuleIsActive));
        }
    }
    
    private bool namingCustomerLast = true;
    public bool NamingCustomerLast
    {
        get => namingCustomerLast;
        set
        {
            namingCustomerLast = value;
            RaisePropertyChanged(nameof(NamingCustomerLast));
        }
    }
    
    private bool isScrewRetained = false;
    public bool IsScrewRetained
    {
        get => isScrewRetained;
        set
        {
            isScrewRetained = value;
            RaisePropertyChanged(nameof(IsScrewRetained));
        }
    }

    private bool showCasesWithoutNumber = true;
    public bool ShowCasesWithoutNumber
    {
        get => showCasesWithoutNumber;
        set
        {
            showCasesWithoutNumber = value;
            RaisePropertyChanged(nameof(ShowCasesWithoutNumber));
        }
    }
    
    private bool hitRenameOnShadeSelect = false;
    public bool HitRenameOnShadeSelect
    {
        get => hitRenameOnShadeSelect;
        set
        {
            hitRenameOnShadeSelect = value;
            RaisePropertyChanged(nameof(HitRenameOnShadeSelect));
        }
    }

    private string selectedDigitalSystem = "";
    public string SelectedDigitalSystem
    {
        get => selectedDigitalSystem;
        set
        {
            selectedDigitalSystem = value;
            RaisePropertyChanged(nameof(SelectedDigitalSystem));
            FocusOnRenameButton();
        }
    }

    private string originalOrderID = "";
    public string OriginalOrderID
    {
        get => originalOrderID!;
        set
        {
            originalOrderID = value;
            RaisePropertyChanged(nameof(OriginalOrderID));
        }
    }

    private string selectedShade = "";
    public string SelectedShade
    {
        get => selectedShade;
        set
        {
            selectedShade = value;
            RaisePropertyChanged(nameof(SelectedShade));
        }
    }
    
    private string orderNamePreview = "";
    public string OrderNamePreview
    {
        get => orderNamePreview;
        set
        {
            orderNamePreview = value;
            RaisePropertyChanged(nameof(OrderNamePreview));
        }
    }
    
    private ThreeShapeOrdersModel? selectedOrder;
    public ThreeShapeOrdersModel? SelectedOrder
    {
        get => selectedOrder;
        set
        {
            if (value is not null)
            {
                selectedOrder = value;
                
                RaisePropertyChanged(nameof(SelectedOrder));
                PreviouslySelectedOrder = value;
                FocusOnPanNumberBox();
                
                PanNumber = "";
                CarestreamID = "";
                IsScrewRetained = false;
                SelectedDigitalSystem = "None";
                SelectedShade = "";
                OrderNamePreview = string.Empty;
                CustomerSuggestionsList = [];
            }
        }
    }
      

    private ThreeShapeOrdersModel? previouslySelectedOrder;
    public ThreeShapeOrdersModel? PreviouslySelectedOrder
    {
        get => previouslySelectedOrder;
        set
        {
            previouslySelectedOrder = value;
            RaisePropertyChanged(nameof(PreviouslySelectedOrder));
        }
    }

    private bool firstOrderSelected = false;
    public bool FirstOrderSelected
    {
        get => firstOrderSelected!;
        set
        {
            firstOrderSelected = value;
            RaisePropertyChanged(nameof(FirstOrderSelected));
        }
    }
    
    private bool jumpToSmartOrderNamesTabWhenNewOrder = false;
    public bool JumpToSmartOrderNamesTabWhenNewOrder
    {
        get => jumpToSmartOrderNamesTabWhenNewOrder!;
        set
        {
            jumpToSmartOrderNamesTabWhenNewOrder = value;
            RaisePropertyChanged(nameof(JumpToSmartOrderNamesTabWhenNewOrder));
        }
    }
    
    private string threeShapeDirectoryHelper = "";
    public string ThreeShapeDirectoryHelper
    {
        get => threeShapeDirectoryHelper!;
        set
        {
            threeShapeDirectoryHelper = value;
            RaisePropertyChanged(nameof(ThreeShapeDirectoryHelper));
        }
    }

    private string toothNumbersString = "";
    public string ToothNumbersString
    {
        get => toothNumbersString!;
        set
        {
            toothNumbersString = value;
            RaisePropertyChanged(nameof(ToothNumbersString));
        }
    }

    private bool controlsEnabled = true;
    public bool ControlsEnabled
    {
        get => controlsEnabled!;
        set
        {
            controlsEnabled = value;
            RaisePropertyChanged(nameof(ControlsEnabled));
        }
    }

    private bool orderIDIsValid = true;
    public bool OrderIDIsValid
    {
        get => orderIDIsValid!;
        set
        {
            orderIDIsValid = value;
            RaisePropertyChanged(nameof(OrderIDIsValid));
        }
    }

    public RelayCommand CloseWindowCommand { get; set; }
    public RelayCommand RefreshCommand { get; set; }
    public RelayCommand BuildNameCommand { get; set; }
    public RelayCommand ShadeButtonClickedCommand { get; set; }
    public RelayCommand RenameOrderCommand { get; set; }
    public RelayCommand AddCustomerSuggestionCommand { get; set; }
    public RelayCommand FocusOnPanNumberCommand { get; set; }

    public System.Timers.Timer _timer;

    public SmartOrderNamesViewModel()
    {
        StaticInstance = this;
        //MainViewModel.Instance.SmartOrderNamesViewModel = this;

        CustomerSuggestionsList = [];

        _timer = new System.Timers.Timer(10000);
        _timer.Elapsed += Timer_Elapsed;
        _timer.Start();

        RefreshCommand = new RelayCommand(o => Refresh());
        BuildNameCommand = new RelayCommand(o => BuildName());
        ShadeButtonClickedCommand = new RelayCommand(o => ShadeButtonClicked(o));
        RenameOrderCommand = new RelayCommand(o => RenameOrder());
        AddCustomerSuggestionCommand = new RelayCommand(o => AddCustomerSuggestion());
        CloseWindowCommand = new RelayCommand(o => CloseWindow());
        FocusOnPanNumberCommand = new RelayCommand(o => FocusOnPanNumberBox());

        ThreeShapeDirectoryHelper = GetServerFileDirectory();

        _ = bool.TryParse(ReadLocalSetting("ModuleSmartOrderNames"), out bool moduleSmartOrderNames);
        SmartOrderNamesModuleIsActive = moduleSmartOrderNames;
    }

    private void CloseWindow()
    {
        Debug.WriteLine("hit");
        MainMenuViewModel.StaticInstance.ShowSmartRenameMenuItem();
        MainViewModel.Instance.SmartOrderNamesWindow.Hide();
    }

    private async void SelectFirstOrder()
    {
        await Task.Delay(1000);
        if (NewOrdersByMe.Count > 0)
        {
            if (PreviouslySelectedOrder != NewOrdersByMe[0])
            {
                FirstOrderSelected = true;
                SelectedOrder = NewOrdersByMe[0];
            }
        }
    }

    private void FocusOnPanNumberBox()
    {
        Application.Current.Dispatcher.Invoke(() => {
            SmartOrderNamesPage.StaticInstance!.panNumberBox.Focus();
        });
    }
    
    private void FocusOnRenameButton()
    {
        Application.Current.Dispatcher.Invoke(() => { 
            SmartOrderNamesPage.StaticInstance!.renameButton.Focus();
        });
    }

    private async void AddCustomerSuggestion()
    {
        if (SelectedOrder is null)
        {
            AddCustomerSuggestionsWindow addCustomerSuggestionWindow = new()
            {
                Owner = MainWindow.Instance
            };
            addCustomerSuggestionWindow.ShowDialog();
        }
        else
        {
            AddCustomerSuggestionsWindow addCustomerSuggestionWindow = new(SelectedOrder.Customer)
            {
                Owner = MainWindow.Instance
            };
            addCustomerSuggestionWindow.ShowDialog();
        }

        
        
        if (SelectedOrder is not null)
        {
            CustomerSuggestionsList = await CustomerHasSuggestedName(SelectedOrder.Customer!);
        }
       
    }

    private void ShadeButtonClicked(object obj)
    {
        SelectedShade = (string)obj;
        BuildName("shade");
        SmartOrderNamesPage.StaticInstance!.renameButton.Focus();
    }

    private async void ResetNameForm()
    {
        Application.Current.Dispatcher.Invoke(() => {
            PanNumber = "";
            CarestreamID = "";
            IsScrewRetained = false;
            SelectedDigitalSystem = "None";
            SelectedShade = "";
            OrderNamePreview = string.Empty;
            PreviouslySelectedOrder = null;
            CustomerSuggestionsList = [];   
        });

        await Refresh();
    }

    private async void BuildName(string obj = "")
    {
        if (SelectedOrder is null || string.IsNullOrEmpty(PanNumber))
            return;

        string finalName = "";
        string screwRetained = "";
        string patientName = $"-{SelectedOrder.Patient_LastName!}";
        string customer = SelectedOrder.Customer!;
        string shade = $"-{SelectedShade}";
        string digiSystem = $"-{SelectedDigitalSystem}";
        string carestreamDexisId = CarestreamID.Trim();

        if (await ValidateCarestreamID(carestreamDexisId))
        {
            carestreamDexisId = "-" + carestreamDexisId;
        }
        else
        {
            carestreamDexisId = "";
        }

        patientName = patientName.Replace(" ", "_")
                                .Replace(",", "")
                                .Replace("'", "_")
                                .Replace("\"", "_")
                                .Replace("+", "_")
                                .Replace("\\", "_")
                                .Replace("/", "_")
                                .Replace(":", "_")
                                .Replace("*", "_")
                                .Replace("?", "_")
                                .Replace("<", "_")
                                .Replace(">", "_")
                                .Replace("&", "-")
                                .Replace("|", "_")
                                .Trim();

        if (patientName == "-" || patientName == "--")
        {
            patientName = $"-{SelectedOrder.Patient_FirstName!}";
            patientName = patientName.Replace(" ", "_")
                                .Replace(",", "")
                                .Replace("'", "_")
                                .Replace("\"", "_")
                                .Replace("+", "_")
                                .Replace("\\", "_")
                                .Replace("/", "_")
                                .Replace(":", "_")
                                .Replace("*", "_")
                                .Replace("?", "_")
                                .Replace("<", "_")
                                .Replace(">", "_")
                                .Replace("&", "-")
                                .Replace("|", "_")
                                .Trim();

            if (patientName == "-" || patientName == "--")
                patientName = "-NONAME";
        }

        List<string> customerSuggestions = await CustomerHasSuggestedName(customer);
        if (customerSuggestions.Count > 0)
        {
            if (SelectedCustomerName is null)
            {
                customer = customerSuggestions[0];
                if (customerSuggestions.Count > 1)
                {
                    if (string.IsNullOrEmpty(SelectedCustomerName))
                        SelectedCustomerName = customerSuggestions[0];

                    CustomerSuggestionsList = customerSuggestions;
                }
            }
            else
            {
                customer = SelectedCustomerName;
            }
        }


        customer = $"-{CleanUpCustomerName(customer)}";

        ToothNumbersString = await GetToothNumbersString(SelectedOrder!.IntOrderID!);

        if (!string.IsNullOrEmpty(ToothNumbersString))
            ToothNumbersString = $"-{ToothNumbersString}";

        if (SelectedDigitalSystem.Equals("None"))
            digiSystem = "";
        if (shade.Equals("-"))
            shade = "";

        // check if pan number is valid or not..!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!



        if (IsScrewRetained)
            screwRetained = "-SCR";

        if (NamingCustomerFirst)
            finalName = $"{PanNumber}{customer}{patientName}{ToothNumbersString}{shade}{digiSystem}{screwRetained}";

        if (NamingCustomerLast)
            finalName = $"{PanNumber}{ToothNumbersString}{shade}{carestreamDexisId}{patientName}{customer}{digiSystem}{screwRetained}";

        finalName = finalName.Replace(" ", "_")
                             .Replace("'","")
                             .Replace("%","")
                             .Replace("*","")
                             .Replace(",","")
                             .Replace(".","")
                             .Replace("&","")
                             .Replace("@","")
                             .Replace("$","")
                             .Replace("+","")
                             .ToUpper();

        OrderNamePreview = finalName;

        if (obj == "shade" && HitRenameOnShadeSelect)
            RenameOrder();
    }

    private async Task<bool> ValidateCarestreamID(string carestreamDexisId)
    {
        return CSIdRegex().IsMatch(carestreamDexisId);
    }

    

    [GeneratedRegex(@"[A-Za-z][A-Za-z][A-Za-z]-\d\d\d\d", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex CSIdRegex();

    private async Task Refresh()
    {
        NewOrdersByMe = await GetNewOrdersCreatedByMe(ShowCasesWithoutNumber);

        if (AutoSelectFirstOrder && NewOrdersByMe.Count > 0 && !FirstOrderSelected)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MainMenuViewModel.StaticInstance.HideSmartRenameMenuItem();
                SelectFirstOrder();
                _timer.Stop();
                MainViewModel.Instance.SmartOrderNamesWindow.ShowDialog();
                _timer.Start();
            });
        }
    }

    private async void Timer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        _ = bool.TryParse(ReadLocalSetting("ModuleSmartOrderNames"), out bool moduleSmartOrderNames);

        SmartOrderNamesModuleIsActive = moduleSmartOrderNames;

        if (SmartOrderNamesModuleIsActive)
        {
            NewOrdersByMe = await GetNewOrdersCreatedByMe(ShowCasesWithoutNumber);

            if (AutoSelectFirstOrder && NewOrdersByMe.Count > 0 && !FirstOrderSelected)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MainMenuViewModel.StaticInstance.HideSmartRenameMenuItem();
                    SelectFirstOrder();
                    _timer.Stop();
                    MainViewModel.Instance.SmartOrderNamesWindow.ShowDialog();
                    _timer.Start();
                });
            }
        }
    }


    private async void RenameOrder()
    {
        OriginalOrderID = PreviouslySelectedOrder!.IntOrderID!;
        if (!CheckIfOrderIDIsUnique(OrderNamePreview))
        {
            ShowMessageBox("OrderID conflict", $"It's not possible to rename the order.\nAn another order in 3Shape has the same name already.\n\nPlease ensure that the order number is unique.", SMessageBoxButtons.Ok, NotificationIcon.Error, 15, MainWindow.Instance);
            return;
        }


        ControlsEnabled = false;
        OrderIDIsValid = false;
        await RenamingProcess();
        ResetNameForm();
        OrderNamePreview = string.Empty;
        FirstOrderSelected = false;

        if (AutoSelectFirstOrder && NewOrdersByMe.Count > 0)
        {
            SelectFirstOrder();
        }
        
        if (NewOrdersByMe.Count < 1)
            MainViewModel.Instance.SmartOrderNamesWindow.Hide();
    }


    public async Task RenamingProcess()
    {

        //ThreeShapeOrderInspectionModel inspectedOrder = InspectThreeShapeOrder(PreviouslySelectedOrder!.IntOrderID!);
        bool error = false;
        string NewFileName = OrderNamePreview;
        string NewFolderName = NewFileName;

        await LockOrderIn3Shape(NewFileName);
        //
        // starting renaming process 
        //

        try
        {

            // renaming Order's folder to the new name
            try
            {
                Directory.Move($"{ThreeShapeDirectoryHelper}{OriginalOrderID}", $"{ThreeShapeDirectoryHelper}{NewFileName}");
            }
            catch (Exception ex)
            {
                MainViewModel.Instance.AddDebugLine(ex);
                LogMessage = $"Couldn't rename the order's folder! (some app might still use it or 3Shape directory has a folder named already same as the order's new desired name)";
                ControlsEnabled = true;
                OrderIDIsValid = true;
                
                return;
            }

            // renaming the XML file to the new name
            File.Move(@$"{ThreeShapeDirectoryHelper}{NewFileName}\{OriginalOrderID}.xml", @$"{ThreeShapeDirectoryHelper}{NewFolderName}\{NewFileName}.xml");

            //
            // renaming the 3ML file if exists (designed orders only)
            //
            try
            {
                if (File.Exists(@$"{ThreeShapeDirectoryHelper}{NewFileName}\{OriginalOrderID}_3pl.3ml"))
                    File.Move(@$"{ThreeShapeDirectoryHelper}{NewFileName}\{OriginalOrderID}_3pl.3ml", @$"{ThreeShapeDirectoryHelper}{NewFolderName}\{NewFileName}_3pl.3ml");
            }
            catch (Exception ex)
            {
                MainViewModel.Instance.AddDebugLine(ex);
            }
            //
            // END
            //



            // 
            // dealing with the XML file
            //
            string XMLFileContent = "";

            try
            {
                // opening up the XML file
                XMLFileContent = File.ReadAllText(@$"{ThreeShapeDirectoryHelper}{NewFolderName}\{NewFileName}.xml");

                // replacing all the entry in the text where the original filename presented to the new name
                XMLFileContent = XMLFileContent.Replace(OriginalOrderID, NewFileName);

                // saving the XML file
                File.WriteAllText(@$"{ThreeShapeDirectoryHelper}{NewFolderName}\{NewFileName}.xml", XMLFileContent);



                //
                // Renaming in the database
                //

                try
                {


                    ///
                    /// renaming
                    /// 
                    /// [PrintJobItem] [OrderID]
                    /// [OrderHistory] [OrderID]
                    /// [OrderExchangeElement] [OrderID]
                    /// [ImageOverlay] [OrderID]
                    /// [CustomData] [OrderID]
                    ///

                    string connectionString = DatabaseConnection.ConnectionStrFor3Shape();

                    string queryCopyLine = @$"INSERT INTO Orders ( 
                                             [IntOrderID] 
                                            ,[ExtOrderID] 
                                            ,[ClientID] 
                                            ,[ClientOrderNo] 
                                            ,[OrderDate] 
                                            ,[OrderImportanceID] 
                                            ,[Patient_RefNo] 
                                            ,[Patient_FirstName]
                                            ,[Patient_LastName] 
                                            ,[DeliveryAddress1] 
                                            ,[DeliveryAddress2] 
                                            ,[DeliveryZip] 
                                            ,[DeliveryCity] 
                                            ,[DeliveryState] 
                                            ,[DeliveryCountryID] 
                                            ,[DeliveryType] 
                                            ,[ShipToDeliveryAddress] 
                                            ,[ClientContactPerson] 
                                            ,[LabID] 
                                            ,[LabOperator] 
                                            ,[OrderComments] 
                                            ,[CreatedFromApp] 
                                            ,[RelativePos] 
                                            ,[OperatorID] 
                                            ,[DisplayOrderID] 
                                            ,[NumOrderID] 
                                            ,[DesignModuleID] 
                                            ,[ScanModuleID] 
                                            ,[FaceScanModuleID] 
                                            ,[Items] 
                                            ,[OperatorName] 
                                            ,[Customer] 
                                            ,[ManufName] 
                                            ,[OrderRelativePositionClass] 
                                            ,[ShipToERPCustNo] 
                                            ,[ERPCustomerNo] 
                                            ,[ShipToID] 
                                            ,[ModelManufacturingID] 
                                            ,[CacheMaterialName] 
                                            ,[ScanSource] 
                                            ,[ImprovementProgramSendDate] 
                                            ,[GroupFolder] 
                                            ,[CacheColor] 
                                            ,[OriginalOrderID] 
                                            ,[ImportOrderID] 
                                            ,[CacheMaxScanDate] 
                                            ,[TraySystemType]
                                            ,[ExternalLabID] 
                                            ,[ShipToDifferentAddress] 
                                            ,[PatientGuid]) 

                                        SELECT '{NewFileName}'
                                            ,[ExtOrderID] 
                                            ,[ClientID] 
                                            ,[ClientOrderNo] 
                                            ,[OrderDate] 
                                            ,[OrderImportanceID] 
                                            ,[Patient_RefNo] 
                                            ,[Patient_FirstName] 
                                            ,[Patient_LastName] 
                                            ,[DeliveryAddress1] 
                                            ,[DeliveryAddress2] 
                                            ,[DeliveryZip] 
                                            ,[DeliveryCity] 
                                            ,[DeliveryState] 
                                            ,[DeliveryCountryID] 
                                            ,[DeliveryType] 
                                            ,[ShipToDeliveryAddress] 
                                            ,[ClientContactPerson] 
                                            ,[LabID] 
                                            ,[LabOperator] 
                                            ,[OrderComments] 
                                            ,[CreatedFromApp] 
                                            ,[RelativePos] 
                                            ,[OperatorID] 
                                            ,[DisplayOrderID] 
                                            ,[NumOrderID] 
                                            ,[DesignModuleID] 
                                            ,[ScanModuleID] 
                                            ,[FaceScanModuleID] 
                                            ,[Items] 
                                            ,[OperatorName] 
                                            ,[Customer] 
                                            ,[ManufName] 
                                            ,[OrderRelativePositionClass] 
                                            ,[ShipToERPCustNo] 
                                            ,[ERPCustomerNo] 
                                            ,[ShipToID] 
                                            ,[ModelManufacturingID] 
                                            ,[CacheMaterialName] 
                                            ,[ScanSource] 
                                            ,[ImprovementProgramSendDate] 
                                            ,[GroupFolder] 
                                            ,[CacheColor] 
                                            ,[OriginalOrderID] 
                                            ,[ImportOrderID] 
                                            ,[CacheMaxScanDate] 
                                            ,[TraySystemType] 
                                            ,[ExternalLabID] 
                                            ,[ShipToDifferentAddress] 
                                            ,[PatientGuid] FROM Orders WHERE IntOrderID = '{OriginalOrderID}'";

                    await RunCommandAsynchronouslyWithLogging(queryCopyLine, connectionString);




                    string query6 = $"UPDATE ModelJob SET OrderID = '{NewFileName}' WHERE OrderID = '{OriginalOrderID}'";
                    await RunCommandAsynchronouslyWithLogging(query6, connectionString);

                    string query2 = $"UPDATE OrderHistory SET OrderID = '{NewFileName}' WHERE OrderID = '{OriginalOrderID}'";
                    await RunCommandAsynchronouslyWithLogging(query2, connectionString);

                    string query5 = $"UPDATE CustomData SET OrderID = '{NewFileName}' WHERE OrderID = '{OriginalOrderID}'";
                    await RunCommandAsynchronouslyWithLogging(query5, connectionString);

                    string query1 = $"UPDATE PrintJobItem SET OrderID = '{NewFileName}' WHERE OrderID = '{OriginalOrderID}'";
                    await RunCommandAsynchronouslyWithLogging(query1, connectionString);

                    string query7 = $"UPDATE CommunicateOrders SET OrderID = '{NewFileName}' WHERE OrderID = '{OriginalOrderID}'";
                    await RunCommandAsynchronouslyWithLogging(query7, connectionString);

                    string query3 = $"UPDATE OrderExchangeElement SET OrderID = '{NewFileName}' WHERE OrderID = '{OriginalOrderID}'";
                    await RunCommandAsynchronouslyWithLogging(query3, connectionString);

                    string query4 = $"UPDATE ImageOverlay SET OrderID = '{NewFileName}' WHERE OrderID = '{OriginalOrderID}'";
                    await RunCommandAsynchronouslyWithLogging(query4, connectionString);




                    UpdateLastModifyDateinDatabase(NewFileName);




                    string queryRemoveOriginalLine = $"DELETE FROM Orders WHERE IntOrderID = '{OriginalOrderID}'";
                    await RunCommandAsynchronouslyWithLogging(queryRemoveOriginalLine, connectionString);
                }
                catch (Exception ex)
                {
                    MainViewModel.Instance.AddDebugLine(ex);
                    LogMessage = $"Error ({ex.LineNumber()}): [{ex.Message}]";
                    LogMessages.Add(LogMessage);
                    error = true;
                }

                //
                // END
                //





            }
            catch (Exception ex)
            {
                MainViewModel.Instance.AddDebugLine(ex);
                error = true;
                LogMessage = $"Error ({ex.LineNumber()}): [{ex.Message}]";
                LogMessages.Add(LogMessage);
                ShowMessageBox("Error", $"{ex.LineNumber()} - {ex.Message}", SMessageBoxButtons.Ok, NotificationIcon.Error, 15, MainWindow.Instance);
            }
            //
            // END
            //


        }
        catch (Exception e)
        {
            MainViewModel.Instance.AddDebugLine(e);
            error = true;
            LogMessage = $"Error ({e.LineNumber()}): [{e.Message}]";
            LogMessages.Add(LogMessage);
        }


        //
        // returning every form control to original stage
        //

        if (!error)
        {
            LogMessage = $"\nRenaming finised with no issues.";
            LogMessages.Add(LogMessage);

            if (LogMessages.Count > 0)
            {
                string message = "";
                foreach (string line in LogMessages)
                    message += line + "\n";
                try
                {
                    File.WriteAllText(@$"{ThreeShapeDirectoryHelper}{NewFolderName}\OrderRename.log", message);
                }
                catch (Exception ex)
                {
                    MainViewModel.Instance.AddDebugLine(ex);
                }
            }

            //openOrderIdHelper = NewFileName;
            await UnLockOrderIn3Shape(NewFileName);
            ResetNameForm();
        }
        else
        {
            LogMessage = $"\nEncountered some issues during renaming..";
            LogMessages.Add(LogMessage);

            if (LogMessages.Count > 0)
            {
                string message = "";
                foreach (string line in LogMessages)
                    message += line + "\n";
                try
                {
                    File.WriteAllText(@$"{ThreeShapeDirectoryHelper}{NewFolderName}\OrderRename.log", message);
                }
                catch (Exception ex)
                {
                    MainViewModel.Instance.AddDebugLine(ex);
                }
            }
        }


        ControlsEnabled = true;
        OrderIDIsValid = true;
        //
        // END
        //
    }




    private async Task RunCommandAsynchronouslyWithLogging(string commandText, string connectionString)
    {
        using SqlConnection connection = new(connectionString);
        try
        {
            SqlCommand command = new(commandText, connection);
            connection.Open();

            IAsyncResult result = command.BeginExecuteNonQuery();
            while (!result.IsCompleted)
            {
                Thread.Sleep(100);
            }
            LogMessage = $"Command complete. Affected [{command.EndExecuteNonQuery(result)}] rows.";
            LogMessages.Add(LogMessage);
            await Task.Delay(20);
        }
        catch (SqlException ex)
        {
            MainViewModel.Instance.AddDebugLine(ex);
            LogMessage = $"Error Exception ({ex.LineNumber()}): [{ex.Message}]";
            LogMessages.Add(LogMessage);
            await Task.Delay(300);
        }
        catch (InvalidOperationException ex)
        {
            MainViewModel.Instance.AddDebugLine(ex);
            LogMessage = $"Error ({ex.LineNumber()}): [{ex.Message}]";
            LogMessages.Add(LogMessage);
            await Task.Delay(300);
        }
        catch (Exception ex)
        {
            MainViewModel.Instance.AddDebugLine(ex);
            LogMessage = $"Error General ({ex.LineNumber()}): [{ex.Message}]";
            LogMessages.Add(LogMessage);
            await Task.Delay(300);
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

        return MainViewModel.Instance.SMessageBoxxResult;
    }

    
}
