using StatsClient.MVVM.Core;
using StatsClient.MVVM.Model;
using StatsClient.MVVM.View;
using System.IO;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using static StatsClient.MVVM.Core.DatabaseOperations;
using static StatsClient.MVVM.Core.MessageBoxes;
using static StatsClient.MVVM.Core.Functions;
using TaskDialog = Ookii.Dialogs.Wpf.TaskDialog;
using TaskDialogIcon = Ookii.Dialogs.Wpf.TaskDialogIcon;
using TaskDialogButton = Ookii.Dialogs.Wpf.TaskDialogButton;
using System.Diagnostics;


namespace StatsClient.MVVM.ViewModel;

public class OrderInfoViewModel : ObservableObject
{
    #region Properties
    private static OrderInfoViewModel? instance;
    public static OrderInfoViewModel Instance
    {
        get => instance!;
        set
        {
            instance = value;
            RaisePropertyChangedStatic(nameof(Instance));
        }
    }

    private ThreeShapeOrdersModel? threeShapeObject;
    public ThreeShapeOrdersModel? ThreeShapeObject
    {
        get => threeShapeObject;
        set
        {
            threeShapeObject = value;
            RaisePropertyChanged(nameof(ThreeShapeObject));
        }
    }

    private OrderInfoWindow? _infoWindow;
    public OrderInfoWindow _InfoWindow
    {
        get => _infoWindow!;
        set
        {
            _infoWindow = value;
            RaisePropertyChanged(nameof(_InfoWindow));
        }
    }

    private List<LastTouchedByModel> lastTouchedByList = [];
    public List<LastTouchedByModel> LastTouchedByList
    {
        get => lastTouchedByList;
        set
        {
            lastTouchedByList = value;
            RaisePropertyChanged(nameof(LastTouchedByList));
        }
    }
    
    private List<DesignerModel> designers = [];
    public List<DesignerModel> Designers
    {
        get => designers;
        set
        {
            designers = value;
            RaisePropertyChanged(nameof(Designers));
        }
    }
    
    private List<DesignedByModel> lastDesignedByList = [];
    public List<DesignedByModel> LastDesignedByList
    {
        get => lastDesignedByList;
        set
        {
            lastDesignedByList = value;
            RaisePropertyChanged(nameof(LastDesignedByList));
        }
    }
    
    private List<XMLItemsDataModel> tDM_ItemsList = [];
    public List<XMLItemsDataModel> TDM_ItemsList
    {
        get => tDM_ItemsList;
        set
        {
            tDM_ItemsList = value;
            RaisePropertyChanged(nameof(TDM_ItemsList));
        }
    }

    private List<ImageListModel> imageList = [];
    public List<ImageListModel> ImageList
    {
        get => imageList;
        set
        {
            imageList = value;
            RaisePropertyChanged(nameof(ImageList));
        }
    }

    private string orderID = "";
    public string OrderID
    {
        get => orderID;
        set
        {
            orderID = value;
            RaisePropertyChanged(nameof(OrderID));
        }
    }

    private string firstName = "";
    public string FirstName
    {
        get => firstName;
        set
        {
            firstName = value;
            RaisePropertyChanged(nameof(FirstName));
        }
    }

    private string lastName = "";
    public string LastName
    {
        get => lastName;
        set
        {
            lastName = value;
            RaisePropertyChanged(nameof(LastName));
        }
    }

    private string customer = "";
    public string Customer
    {
        get => customer;
        set
        {
            customer = value;
            RaisePropertyChanged(nameof(Customer));
        }
    }

    private string status = "";
    public string Status
    {
        get => status;
        set
        {
            status = value;
            RaisePropertyChanged(nameof(Status));
        }
    }

    private string scanner = "";
    public string Scanner
    {
        get => scanner;
        set
        {
            scanner = value;
            RaisePropertyChanged(nameof(Scanner));
        }
    }

    private string digitalCase = "";
    public string DigitalCase
    {
        get => digitalCase;
        set
        {
            digitalCase = value;
            RaisePropertyChanged(nameof(DigitalCase));
        }
    }

    private string created = "";
    public string Created
    {
        get => created;
        set
        {
            created = value;
            RaisePropertyChanged(nameof(Created));
        }
    }

    private string scanned = "";
    public string Scanned
    {
        get => scanned;
        set
        {
            scanned = value;
            RaisePropertyChanged(nameof(Scanned));
        }
    }

    private string manufacturer = "";
    public string Manufacturer
    {
        get => manufacturer;
        set
        {
            manufacturer = value;
            RaisePropertyChanged(nameof(Manufacturer));
        }
    }

    private string comments = "";
    public string Comments
    {
        get => comments;
        set
        {
            comments = value;
            RaisePropertyChanged(nameof(Comments));
        }
    }

    private string traySystem = "";
    public string TraySystem
    {
        get => traySystem;
        set
        {
            traySystem = value;
            RaisePropertyChanged(nameof(TraySystem));
        }
    }

    private string material = "";
    public string Material
    {
        get => material;
        set
        {
            material = value;
            RaisePropertyChanged(nameof(Material));
        }
    }

    private string items = "";
    public string Items
    {
        get => items;
        set
        {
            items = value;
            RaisePropertyChanged(nameof(Items));
        }
    }

    private string dentalSystemVersion  = "";
    public string DentalSystemVersion
    {
        get => dentalSystemVersion;
        set
        {
            dentalSystemVersion = value;
            RaisePropertyChanged(nameof(DentalSystemVersion));
        }
    }


    private string designModuleActual = "";
    public string DesignModuleActual
    {
        get => designModuleActual;
        set
        {
            designModuleActual = value;
            RaisePropertyChanged(nameof(DesignModuleActual));
        }
    }
    
    private string designModuleOriginal = "";
    public string DesignModuleOriginal
    {
        get => designModuleOriginal;
        set
        {
            designModuleOriginal = value;
            RaisePropertyChanged(nameof(DesignModuleOriginal));
        }
    }

    private string originalOrderID = "";
    public string OriginalOrderID
    {
        get => originalOrderID;
        set
        {
            originalOrderID = value;
            RaisePropertyChanged(nameof(OriginalOrderID));
        }
    }


    #endregion Properties

    public RelayCommand CloseWindowCommand { get; set; }
    public RelayCommand ImageClickedCommand { get; set; }
    public RelayCommand DesignedByDesignerClickCommand { get; set; }
    public RelayCommand OpenUpOrderSourceFolderCommand { get; set; }
    public RelayCommand SearchCommand { get; set; }
    

    public OrderInfoViewModel()
    {
        Instance = this;

        CloseWindowCommand = new RelayCommand(o => CloseWindow());
        ImageClickedCommand = new RelayCommand(o => ImageClicked(o));
        DesignedByDesignerClickCommand = new RelayCommand(o => DesignedByDesignerMenuItemClicked(o));
        OpenUpOrderSourceFolderCommand = new RelayCommand(o => OpenUpOrderSourceFolder());
        SearchCommand = new RelayCommand(o => Search(o));
        
    }

    private void Search(object obj)
    {
        if (obj is not string || ((string)obj).Length < 2)
            return;

        string searchStr = ((string)obj).Trim();

        MainViewModel.Instance.SearchOnlyInFileNames = false;

        bool isNumeric = int.TryParse(searchStr, out _);
        if (isNumeric)
        {
            MainViewModel.Instance.SearchOnlyInFileNames = true;
            searchStr += "-";
        }

        MainViewModel.Instance.SearchString = searchStr;
        MainViewModel.Instance.SearchFieldKeyDownCommand.Execute(null);
        CloseWindow();
    }

    private void OpenUpOrderSourceFolder()
    {
        string ThreeShapeDirectoryHelper = DatabaseOperations.GetServerFileDirectory();
        string folder = $"{ThreeShapeDirectoryHelper}{ThreeShapeObject!.IntOrderID}";
        
        try
        {
            if (!string.IsNullOrEmpty(folder) && Directory.Exists(folder))
                Process.Start("explorer.exe", "\"" + folder + "\"");
        }
        catch (Exception)
        {
        }
    }

    private async void DesignedByDesignerMenuItemClicked(object obj)
    {
        string designerID = (string)obj;
        string orderID = ThreeShapeObject!.IntOrderID!;

        if (await AddLastDesignedByToOrder(orderID, designerID))
        {
            if (Designers is not null)
            {
                string designerName = Designers.FirstOrDefault(x => x.DesignerID == designerID)!.FriendlyName!;
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    SMessageBox sMessageBox = new("Success", $"Designer set to {designerName}", Enums.SMessageBoxButtons.Ok, MainViewModel.NotificationIcon.Success, 4)
                    {
                        Owner = MainWindow.Instance
                    };

                    sMessageBox.ShowDialog();
                }));
            }
            UpdateDesignedByList();
        }
        else
        {
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                SMessageBox sMessageBox = new("Error", "Some of the tasks could not be finished..", Enums.SMessageBoxButtons.Ok, MainViewModel.NotificationIcon.Warning, 7)
                {
                    Owner = MainWindow.Instance
                };

                sMessageBox.ShowDialog();                
            }));
        }
    }

    private void ImageClicked(object obj)
    {
        string imageSource = (string)obj;
        ImagePreviewWindow imagePreviewWindow = new ();
        imagePreviewWindow.ShowDialog(OrderInfoWindow.StaticInstance, imageSource);
    }

    private void CloseWindow()
    {
        OrderInfoWindow.StaticInstance.Close();
    }

    public async void UpdateForm()
    {
        OrderID = ThreeShapeObject!.IntOrderID!;
        FirstName = ThreeShapeObject.Patient_FirstName!;
        LastName = ThreeShapeObject.Patient_LastName!;
        Customer = ThreeShapeObject.Customer!;
        Status = GetStatus(ThreeShapeObject.ProcessStatusID!);
        Scanner = ThreeShapeObject.ScanSourceFriendlyName!;

        if (IsDigitalCase(ThreeShapeObject.ScanSource!))
            DigitalCase = "Yes";
        else
            DigitalCase = "No";

        if(DateTime.TryParse(ThreeShapeObject.MaxCreateDate, out DateTime MaxCreateDate))
            Created = MaxCreateDate.ToString("yyyy-MM-dd h:mm:ss tt");

        if (DateTime.TryParse(ThreeShapeObject.CacheMaxScanDate, out DateTime CacheMaxScanDate))
            Scanned = CacheMaxScanDate.ToString("yyyy-MM-dd h:mm:ss tt");



        Manufacturer = ThreeShapeObject.ManufName!;
        if (Manufacturer == "")
            Manufacturer = "This Lab";
        Comments = ThreeShapeObject.OrderComments!.Replace("&#xA;", "\n").Replace("&", "");

        if (!IsDigitalCase(ThreeShapeObject.ScanSource!))
            TraySystem = GetTraySystem(ThreeShapeObject.TraySystemType!, ThreeShapeObject.ProcessStatusID!);
        else
            TraySystem = "";

        Material = ThreeShapeObject.CacheMaterialName!;
        Items = RemoveChineseCharacters(ThreeShapeObject.Items!);

        ReadXMLInfo(ThreeShapeObject.IntOrderID!);


        var bc = new BrushConverter();

        LastTouchedByList = GetLastTouchedByListData(ThreeShapeObject.IntOrderID!);
        _InfoWindow!.panelLastTouchedBy.Children.Clear();

        if (LastTouchedByList.Count > 0)
        {
            foreach (LastTouchedByModel item in LastTouchedByList)
            {
                string computerName = item.ComputerName;
                string dateTime = item.DateTimeStr;

                _ = DateTime.TryParse(dateTime, out DateTime dtm);
                dateTime = dtm.ToString("dddd - M/d/yyyy h:mm tt");

                StackPanel stckPanel = new()
                {
                    Margin = new Thickness(0, 0, 0, 8)
                };

                TextBlock dtbCompName = new()
                {
                    FontWeight = FontWeights.SemiBold,
                    FontSize = 12,
                    Foreground = Brushes.Black,
                    Text = computerName
                };
                stckPanel.Children.Add(dtbCompName);

                TextBlock dtbDateTime = new()
                {
                    FontSize = 9
                };

                if (dtm.ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd"))
                    dtbDateTime.Foreground = Brushes.Green;
                else if (dtm.ToString("yyyy-MM-dd") == DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"))
                    dtbDateTime.Foreground = Brushes.SteelBlue;
                else
                    dtbDateTime.Foreground = (Brush)bc.ConvertFrom("#FF666666")!;

                dtbDateTime.Text = dateTime;
                stckPanel.Children.Add(dtbDateTime);

                _InfoWindow.panelLastTouchedBy.Children.Add(stckPanel);
            }
        }
        else
            _InfoWindow.borderLastTouchedByPanel.Visibility = Visibility.Collapsed;


        UpdateDesignedByList();

        if (ThreeShapeObject.PanNumber != "" && ThreeShapeObject.PanColor != "#FFFFFF" && ThreeShapeObject.PanColor != "Transparent")
        {
            try
            {
                _InfoWindow.brPanColor.Background = (Brush)bc.ConvertFrom("#FF" + ThreeShapeObject.PanColor!.Replace("#", ""))!;
            }
            catch (Exception ex)
            {
                MainViewModel.Instance.AddDebugLine(ex);
            }

        }
        else
        {
            _InfoWindow.brPanColor.Visibility = Visibility.Collapsed;
        }


        List<DesignerModel> designersList = await GetDesignersListAtStartAsync();
        ContextMenu menu = OrderInfoWindow.StaticInstance.designerContextMenu;

        if (ThreeShapeObject.MaxProcessStatusID == "psModelled" && ThreeShapeObject.IsCaseWereDesigned)
        {
            MenuItem mitem = new()
            {
                Header = $"Set last designer",
                IsEnabled = false
            };

            menu.Items.Add(mitem);

            Separator separator = new();
            menu.Items.Add(separator);

            foreach (var designer in designersList)
            {
                Designers.Add(designer);
                MenuItem item = new () 
                {
                    Header = $"To {designer.FriendlyName}",
                    Tag = designer.DesignerID,
                    Command = OrderInfoViewModel.Instance.DesignedByDesignerClickCommand,
                    CommandParameter = designer.DesignerID
                };

                menu.Items.Add(item);
            }
        }
        else
        {
            MenuItem mitem = new()
            {
                Header = $"Order is not designed!",
                IsEnabled = false
            };

            menu.Items.Add(mitem);
        }

        ReloadImages(false);




        //tbOrderUpdated.Visibility = Visibility.Visible;
    }

    private void UpdateDesignedByList()
    {
        var bc = new BrushConverter();

        LastDesignedByList = GetLastDesignedByListData(ThreeShapeObject.IntOrderID!);
        _InfoWindow!.panelLastDesignedBy.Children.Clear();

        if (LastDesignedByList.Count > 0)
        {
            foreach (DesignedByModel item in LastDesignedByList)
            {
                string designerName = item.Designer!;
                string dateTime = item.DateTimeStr!;

                _ = DateTime.TryParse(dateTime, out DateTime dtm);
                dateTime = dtm.ToString("dddd - M/d/yyyy h:mm tt");

                StackPanel stckPanel = new()
                {
                    Margin = new Thickness(0, 0, 0, 8)
                };

                TextBlock dtbCompName = new()
                {
                    FontWeight = FontWeights.SemiBold,
                    FontSize = 12,
                    Foreground = Brushes.Black,
                    Text = designerName
                };
                stckPanel.Children.Add(dtbCompName);

                TextBlock dtbDateTime = new()
                {
                    FontSize = 9
                };

                if (dtm.ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd"))
                    dtbDateTime.Foreground = Brushes.Green;
                else if (dtm.ToString("yyyy-MM-dd") == DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"))
                    dtbDateTime.Foreground = Brushes.SteelBlue;
                else
                    dtbDateTime.Foreground = (Brush)bc.ConvertFrom("#FF666666")!;

                dtbDateTime.Text = dateTime;
                stckPanel.Children.Add(dtbDateTime);

                _InfoWindow.panelLastDesignedBy.Children.Add(stckPanel);
            }
        }
        else
            _InfoWindow.borderLastDesignedByPanel.Visibility = Visibility.Collapsed;
    }

    public void WindowClosing()
    {
        ImageList.Clear();
        GC.Collect();
    }

    public void ReloadImages(bool ReleaseFileLocks)
    {
        ImageList.Clear();

        bool isThereAnyPicture = false;
        string orderFolder;

        if (ReleaseFileLocks)
            orderFolder = LocalSettingsDB.DataBaseFolder;
        else
            orderFolder = GetServerFileDirectory() + ThreeShapeObject!.IntOrderID + "\\Images";
        if (Directory.Exists(orderFolder))
        {
            DirectoryInfo folder = new (orderFolder);
            FileInfo[] images = [.. folder.GetFiles("*.jpg"), .. folder.GetFiles("*.png")];
            if (images.Length > 0)
                isThereAnyPicture = true;
            foreach (FileInfo img in images)
            {
                ImageList.Add(new ImageListModel(img.FullName));
            }
        }



        if (ReleaseFileLocks)
            GC.Collect();

        if (!ReleaseFileLocks)
            orderFolder = GetServerFileDirectory() + ThreeShapeObject!.IntOrderID + "\\3SCom\\Screenshots";
        if (Directory.Exists(orderFolder))
        {
            DirectoryInfo folder = new (orderFolder);
            FileInfo[] images = [.. folder.GetFiles("*.jpg"), .. folder.GetFiles("*.png")];
            if (images.Length > 0)
                isThereAnyPicture = true;
            foreach (FileInfo img in images)
            {
                ImageList.Add(new ImageListModel(img.FullName));
            }
        }

        if (!ReleaseFileLocks)
            orderFolder = GetServerFileDirectory() + ThreeShapeObject!.IntOrderID;
        if (Directory.Exists(orderFolder))
        {
            DirectoryInfo folder = new (orderFolder);
            FileInfo[] images = [.. folder.GetFiles("*.jpg"), .. folder.GetFiles("*.png")];
            if (images.Length > 0)
                isThereAnyPicture = true;
            foreach (FileInfo img in images)
            {
                ImageList.Add(new ImageListModel(img.FullName));
            }
        }

        if (!isThereAnyPicture && !ReleaseFileLocks)
            _InfoWindow.tabPageImages.Visibility = Visibility.Collapsed;
        else
            _InfoWindow.tabPageImages.Visibility = Visibility.Visible;

        _InfoWindow.Thumbnails.Items.Refresh();
    }

    public bool ReadXMLInfo(string OrderID)
    {
        string XMLFile = DatabaseConnection.GetServerFileDirectory() + OrderID + "\\" + OrderID + ".xml";
        string stCopyFile = DatabaseConnection.GetServerFileDirectory() + OrderID + "\\" + OrderID + ".stCopy";

        bool stCopyExists = File.Exists(stCopyFile);

        if (File.Exists(XMLFile))
        {
            List<string> TDM_Item_ModelElement = new List<string>();
            bool NeedToCopyModelElement = false;
            
            foreach (string line in File.ReadLines(XMLFile))
            {
                if (line.Contains("<DentalContainer version="))
                    DentalSystemVersion = line.Replace("<DentalContainer version=\"", "").Replace("\"", "").Replace("/>", "").Replace(">", "").Trim();

                if (line.Contains("<Property name=\"DesignModuleID\" value=\""))
                    DesignModuleActual = line.Replace("<Property name=\"DesignModuleID\" value=\"", "").Replace("\"", "").Replace("/>", "").Trim();

                if (line.Contains("<Property name=\"OriginalOrderID\" value=\""))
                    OriginalOrderID = line.Replace("<Property name=\"OriginalOrderID\" value=\"", "").Replace("\"", "").Replace("/>", "").Trim();

                if (line.Contains("<Object type=\"TDM_Item_ModelElement\">"))
                    NeedToCopyModelElement = true;

                if (line.Contains("</List>"))
                    NeedToCopyModelElement = false;

                if (NeedToCopyModelElement)
                    TDM_Item_ModelElement.Add(line);
            }

            string ItemChunk = "";
            string CacheMaterialChunk = "";
            string ModelJobIDChunk = "";
            string ManufacturingProcessIDChunk = "";
            string ManufacturerIDChunk = "";
            string ManufNameChunk = "";
            string ModelElementID = "";




            foreach (string line in TDM_Item_ModelElement)
            {
                if (line.Contains("<Property name=\"ModelElementID\" value=\""))
                {
                    ModelElementID = line.Replace("<Property name=\"ModelElementID\" value=\"", "").Replace("\"", "").Replace("/>", "").Replace("'", "_").Trim();
                }

                if (line.Contains("<Property name=\"Items\" value=\""))
                {
                    ItemChunk = line.Replace("<Property name=\"Items\" value=\"", "").Replace("\"", "").Replace("/>", "").Replace("'", "_").Trim();
                }

                if (line.Contains("<Property name=\"CacheMaterialName\" value=\""))
                {
                    CacheMaterialChunk = line.Replace("<Property name=\"CacheMaterialName\" value=\"", "").Replace("\"", "").Replace("/>", "").Replace("'", "_").Trim();
                }

                if (line.Contains("<Property name=\"ModelJobID\" value=\""))
                {
                    ModelJobIDChunk = line.Replace("<Property name=\"ModelJobID\" value=\"", "").Replace("\"", "").Replace("/>", "").Replace("'", "_").Trim();
                }

                if (line.Contains("<Property name=\"ManufacturingProcessID\" value=\""))
                {
                    ManufacturingProcessIDChunk = line.Replace("<Property name=\"ManufacturingProcessID\" value=\"", "").Replace("\"", "").Replace("/>", "").Replace("'", "_").Trim();
                }

                if (line.Contains("<Property name=\"ManufacturerID\" value=\""))
                {
                    ManufacturerIDChunk = line.Replace("<Property name=\"ManufacturerID\" value=\"", "").Replace("\"", "").Replace("/>", "").Replace("'", "_").Trim();
                }

                if (line.Contains("<Property name=\"ManufName\" value=\""))
                {
                    ManufNameChunk = line.Replace("<Property name=\"ManufName\" value=\"", "").Replace("\"", "").Replace("/>", "").Replace("'", "_").Trim();
                }




                // checking ModelElementID to avoid fake entries which appears during redesign
                if (CacheMaterialChunk != "" && !ModelElementID.Contains("_"))
                {
                    if (stCopyExists)
                        TDM_ItemsList.Add(new XMLItemsDataModel(
                                ItemChunk,
                                CacheMaterialChunk,
                                ModelJobIDChunk,
                                ManufacturingProcessIDChunk,
                                ManufacturerIDChunk,
                                ManufNameChunk,
                                null,
                                null,
                                null,
                                Brushes.Blue,
                                null
                        ));
                    else
                        TDM_ItemsList.Add(new XMLItemsDataModel(
                                ItemChunk,
                                CacheMaterialChunk,
                                ModelJobIDChunk,
                                ManufacturingProcessIDChunk,
                                ManufacturerIDChunk,
                                ManufNameChunk,
                                ManufacturingProcessIDChunk,
                                ManufacturerIDChunk,
                                ManufNameChunk,
                                Brushes.DimGray,
                                "FailedToValidate"
                        ));


                    CacheMaterialChunk = "";
                }
            }





            #region Parse stCopy                
            if (stCopyExists)
            {
                List<string> TDM_Item_ModelElementST = [];
                bool NeedToCopyModelElementST = false;

                foreach (string line in File.ReadLines(stCopyFile))
                {
                    if (line.Contains("<Property name=\"DesignModuleID\" value=\""))
                        DesignModuleOriginal = line.Replace("<Property name=\"DesignModuleID\" value=\"", "").Replace("\"", "").Replace("/>", "").Trim();

                    if (line.Contains("<Object type=\"TDM_Item_ModelElement\">"))
                        NeedToCopyModelElementST = true;

                    if (line.Contains("</List>"))
                        NeedToCopyModelElementST = false;

                    if (NeedToCopyModelElementST)
                        TDM_Item_ModelElementST.Add(line);
                }

                ItemChunk = "";
                CacheMaterialChunk = "";
                ModelJobIDChunk = "";
                ManufacturingProcessIDChunk = "";
                ManufacturerIDChunk = "";
                ManufNameChunk = "";


                foreach (string line in TDM_Item_ModelElementST)
                {
                    if (line.Contains("<Property name=\"Items\" value=\""))
                    {
                        ItemChunk = line.Replace("<Property name=\"Items\" value=\"", "").Replace("\"", "").Replace("/>", "").Replace("'", "_").Trim();
                    }



                    if (line.Contains("<Property name=\"ModelJobID\" value=\""))
                    {
                        ModelJobIDChunk = line.Replace("<Property name=\"ModelJobID\" value=\"", "").Replace("\"", "").Replace("/>", "").Replace("'", "_").Trim();
                    }

                    if (line.Contains("<Property name=\"ManufacturingProcessID\" value=\""))
                    {
                        ManufacturingProcessIDChunk = line.Replace("<Property name=\"ManufacturingProcessID\" value=\"", "").Replace("\"", "").Replace("/>", "").Replace("'", "_").Trim();
                    }

                    if (line.Contains("<Property name=\"ManufacturerID\" value=\""))
                    {
                        ManufacturerIDChunk = line.Replace("<Property name=\"ManufacturerID\" value=\"", "").Replace("\"", "").Replace("/>", "").Replace("'", "_").Trim();
                    }

                    if (line.Contains("<Property name=\"ManufName\" value=\""))
                    {
                        ManufNameChunk = line.Replace("<Property name=\"ManufName\" value=\"", "").Replace("\"", "").Replace("/>", "").Replace("'", "_").Trim();
                    }

                    if (line.Contains("<Property name=\"CacheMaterialName\" value=\""))
                    {
                        CacheMaterialChunk = line.Replace("<Property name=\"CacheMaterialName\" value=\"", "").Replace("\"", "").Replace("/>", "").Replace("'", "_").Trim();
                    }



                    if (line.Contains("</Object>"))
                    {
                        try
                        {
                            if (TDM_ItemsList.FirstOrDefault(x => x.Item == ItemChunk && x.ModelJobID == ModelJobIDChunk) != null)
                            {
                                TDM_ItemsList.FirstOrDefault(x => x.Item == ItemChunk && x.ModelJobID == ModelJobIDChunk).ManufacturerID_stCopy = ManufacturerIDChunk;
                                TDM_ItemsList.FirstOrDefault(x => x.Item == ItemChunk && x.ModelJobID == ModelJobIDChunk).ManufacturingProcessID_stCopy = ManufacturingProcessIDChunk;
                                TDM_ItemsList.FirstOrDefault(x => x.Item == ItemChunk && x.ModelJobID == ModelJobIDChunk).ManufName_stCopy = ManufNameChunk;

                                if (TDM_ItemsList.FirstOrDefault(x => x.Item == ItemChunk && x.ModelJobID == ModelJobIDChunk).ManufacturerID !=
                                    TDM_ItemsList.FirstOrDefault(x => x.Item == ItemChunk && x.ModelJobID == ModelJobIDChunk).ManufacturerID_stCopy ||
                                    TDM_ItemsList.FirstOrDefault(x => x.Item == ItemChunk && x.ModelJobID == ModelJobIDChunk).ManufacturingProcessID !=
                                    TDM_ItemsList.FirstOrDefault(x => x.Item == ItemChunk && x.ModelJobID == ModelJobIDChunk).ManufacturingProcessID_stCopy ||
                                    TDM_ItemsList.FirstOrDefault(x => x.Item == ItemChunk && x.ModelJobID == ModelJobIDChunk).ManufName !=
                                    TDM_ItemsList.FirstOrDefault(x => x.Item == ItemChunk && x.ModelJobID == ModelJobIDChunk).ManufName_stCopy)
                                {
                                    TDM_ItemsList.FirstOrDefault(x => x.Item == ItemChunk && x.ModelJobID == ModelJobIDChunk).ValidateItemIntegrity = "Invalid";
                                }
                                else
                                {
                                    TDM_ItemsList.FirstOrDefault(x => x.Item == ItemChunk && x.ModelJobID == ModelJobIDChunk).ValidateItemIntegrity = "Valid";
                                }
                            }
                            // when this process failed, checking if the numbering converted from FDI to Univerzal Notation would pass the comparison..
                            // if it does, then at the end of this scope overwriting the FDI notation with Universal Numeric Notation..
                            else if (TDM_ItemsList.FirstOrDefault(x => FDIConverter.ConvertFDIinString(x.Item) == ItemChunk && x.ModelJobID == ModelJobIDChunk) != null)
                            {
                                TDM_ItemsList.FirstOrDefault(x => FDIConverter.ConvertFDIinString(x.Item) == ItemChunk && x.ModelJobID == ModelJobIDChunk).ManufacturerID_stCopy = ManufacturerIDChunk;
                                TDM_ItemsList.FirstOrDefault(x => FDIConverter.ConvertFDIinString(x.Item) == ItemChunk && x.ModelJobID == ModelJobIDChunk).ManufacturingProcessID_stCopy = ManufacturingProcessIDChunk;
                                TDM_ItemsList.FirstOrDefault(x => FDIConverter.ConvertFDIinString(x.Item) == ItemChunk && x.ModelJobID == ModelJobIDChunk).ManufName_stCopy = ManufNameChunk;

                                if (TDM_ItemsList.FirstOrDefault(x => FDIConverter.ConvertFDIinString(x.Item) == ItemChunk && x.ModelJobID == ModelJobIDChunk).ManufacturerID !=
                                    TDM_ItemsList.FirstOrDefault(x => FDIConverter.ConvertFDIinString(x.Item) == ItemChunk && x.ModelJobID == ModelJobIDChunk).ManufacturerID_stCopy ||
                                    TDM_ItemsList.FirstOrDefault(x => FDIConverter.ConvertFDIinString(x.Item) == ItemChunk && x.ModelJobID == ModelJobIDChunk).ManufacturingProcessID !=
                                    TDM_ItemsList.FirstOrDefault(x => FDIConverter.ConvertFDIinString(x.Item) == ItemChunk && x.ModelJobID == ModelJobIDChunk).ManufacturingProcessID_stCopy ||
                                    TDM_ItemsList.FirstOrDefault(x => FDIConverter.ConvertFDIinString(x.Item) == ItemChunk && x.ModelJobID == ModelJobIDChunk).ManufName !=
                                    TDM_ItemsList.FirstOrDefault(x => FDIConverter.ConvertFDIinString(x.Item) == ItemChunk && x.ModelJobID == ModelJobIDChunk).ManufName_stCopy)
                                {
                                    TDM_ItemsList.FirstOrDefault(x => FDIConverter.ConvertFDIinString(x.Item) == ItemChunk && x.ModelJobID == ModelJobIDChunk).ValidateItemIntegrity = "Invalid";
                                }
                                else
                                {
                                    TDM_ItemsList.FirstOrDefault(x => FDIConverter.ConvertFDIinString(x.Item) == ItemChunk && x.ModelJobID == ModelJobIDChunk).ValidateItemIntegrity = "Valid";
                                }

                                // overwriting the FDI numbering..
                                TDM_ItemsList.FirstOrDefault(x => FDIConverter.ConvertFDIinString(x.Item) == ItemChunk && x.ModelJobID == ModelJobIDChunk).Item = ItemChunk;
                            }
                            else
                            {
                                foreach (var x in TDM_ItemsList)
                                {
                                    if (x.Item != ItemChunk && x.ModelJobID != ModelJobIDChunk)
                                    {
                                        x.ValidateItemIntegrity = "ChangedItem";
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MainViewModel.Instance.AddDebugLine(ex);
                        }
                    }
                }
            }
            #endregion END Parse stCopy


            
            

            if (DesignModuleActual == "DentalDesigner")
            {
                DesignModuleActual = "DentalDesigner " + DentalSystemVersion.Substring(0, 4);
            }
            

            if (DesignModuleOriginal == "DentalDesigner" && DesignModuleActual.StartsWith("DD"))
            {
                DesignModuleOriginal = "DentalDesigner " + DesignModuleActual.Substring(2);
            }
            
            return true;
        }
        return false;
    }

    public async void SaveImagesInto3ShapeOrder(string[] files)
    {
        var target = new DirectoryInfo(GetServerFileDirectory() + ThreeShapeObject!.IntOrderID + "\\Images");

        try
        {
            Directory.CreateDirectory(target.FullName);
        }
        catch (Exception ex)
        {
            MainViewModel.Instance.AddDebugLine(ex);
            ShowMessage(_InfoWindow.Instance, "Could not prepare the order to receive images!\n\n" + ex.Message, TaskDialogIcon.Error, Buttons.Ok);
            return;
        }

        foreach (string file in files)
        {
            FileInfo fileInfo = new(file);
            if (file.ToLower().EndsWith(".jpg", StringComparison.CurrentCultureIgnoreCase) || file.ToLower().EndsWith(".png", StringComparison.CurrentCultureIgnoreCase))
            {
                try
                {
                    await Task.Run(() => fileInfo.CopyTo(Path.Combine(target.FullName, fileInfo.Name)));
                }
                catch (Exception ex)
                {
                    MainViewModel.Instance.AddDebugLine(ex);
                    using TaskDialog dialog = new ();
                    dialog.WindowTitle = "Stats Client";
                    dialog.Content = fileInfo.Name + " already exists.";
                    dialog.CenterParent = true;
                    dialog.MainIcon = TaskDialogIcon.Warning;

                    TaskDialogButton copyButton = new("Copy in new name");
                    TaskDialogButton skipButton = new("Skip");

                    dialog.Buttons.Add(copyButton);
                    dialog.Buttons.Add(skipButton);

                    TaskDialogButton buttonResult = dialog.ShowDialog(_InfoWindow.Instance);
                    if (buttonResult == copyButton)
                    {
                        string newFileName = fileInfo.Name.Replace(fileInfo.Extension, DateTime.Now.ToString("-HHmmss") + fileInfo.Extension);
                        await Task.Run(() => fileInfo.CopyTo(Path.Combine(target.FullName, newFileName), true));
                    }
                }
            }
        }

        ReloadImages(false);
    }
}
