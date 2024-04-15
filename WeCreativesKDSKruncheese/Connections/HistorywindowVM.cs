using Back_Office.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using WeCreatives_KDSPJ;
using WeCreatives_KDSPJ.Connections;
using WeCreativesKDSKruncheese.Views;

namespace WeCreativesKDSKruncheese.Connections
{
    public class HistorywindowVM:ViewModelBase
    {
        string kdcLocValue = App.KdcLoc;
        private readonly IServiceProvider _serviceProvider;
        private bool _isStrikethrough;

        public bool IsStrikethrough
        {
            get => _isStrikethrough;
            set
            {
                if (_isStrikethrough != value)
                {
                    _isStrikethrough = value;
                    OnPropertyChanged(nameof(IsStrikethrough));
                }
            }
        }
        public static int KDSLOC { get; set; }
        private string _kdsnamee = App.KdcName;
        public string KDSNAMEE
        {
            get => _kdsnamee;
            set
            {
                if (_kdsnamee != value)
                {
                    _kdsnamee = value;
                    OnPropertyChanged(nameof(KDSNAMEE));
                    // If other properties' visibility depends on KDSNAMEE, trigger their change notification too
                    OnPropertyChanged(nameof(IsFryer));
                    OnPropertyChanged(nameof(IsMakeTimeVisible));
                    OnPropertyChanged(nameof(IsRackTimeVisible));
                    UpdateStatusVisibility();
                    // Continue with other dependent properties as needed
                }
            }
        }

        private readonly StatusQuery _querystrings;
        private DateTime _kdsdateTime;
        public string KDSNAME
        {
            get => _kDSNAME;
            set
            {
                if (_kDSNAME != value)
                {
                    _kDSNAME = value;
                    OnPropertyChanged(nameof(KDSNAME));
                    // Trigger the OnPropertyChanged for dependent properties
                    OnPropertyChanged(nameof(IsFryer));
                    OnPropertyChanged(nameof(IsMakeTimeVisible));
                    OnPropertyChanged(nameof(IsRackTimeVisible));
                    // Add more as needed for each status
                }
            }
        }

        public bool IsFryer => KDSNAME.Equals("KDS FRYER", StringComparison.OrdinalIgnoreCase);
        public bool IsASSEMBLY => KDSNAME.Equals("ASSEMBLY STATION", StringComparison.OrdinalIgnoreCase);

        // Controls visibility of Make Time based on KDSNAME
        public bool IsMakeTimeVisible => IsFryer;
        public bool IsAssemblyVisible => IsASSEMBLY;

        // Controls visibility of Rack Time based on KDSNAME
        public bool IsRackTimeVisible => !IsFryer && !IsASSEMBLY;

        public DateTime KdsdateTime
        {
            get { return _kdsdateTime; }
            set
            {
                if (_kdsdateTime != value)
                {
                    _kdsdateTime = value;
                    OnPropertyChanged(nameof(KdsdateTime)); // Use nameof to avoid hard-coded strings
                }
            }
        }
        private int _totalItemCount;
        public int TotalItemCount
        {
            get => _totalItemCount;
            set { _totalItemCount = value; OnPropertyChanged(nameof(TotalItemCount)); }
        }
        private string _averagemaketime;
        public string AverageMakeTime
        {
            get => _averagemaketime;
            set { _averagemaketime = value; OnPropertyChanged(nameof(AverageMakeTime)); }
        }
        private string _averageracktime;
        public string AverageRackTime
        {
            get => _averageracktime;
            set { _averageracktime = value; OnPropertyChanged(nameof(AverageRackTime)); }
        }
        private string _averageotdtime;
        public string AverageotdTime
        {
            get => _averageotdtime;
            set { _averageotdtime = value; OnPropertyChanged(nameof(AverageotdTime)); }
        }
        private string _averagettdttime;
        public string AverageottdtTime
        {
            get => _averagettdttime;
            set { _averagettdttime = value; OnPropertyChanged(nameof(AverageottdtTime)); }
        }
        private string _averageCSCtime;
        public string AverageoCSCTime
        {
            get => _averageCSCtime;
            set { _averageCSCtime = value; OnPropertyChanged(nameof(AverageoCSCTime)); }
        }
        private string _kDSNAME;
        public string KDSAPPNAME
        {
            get => _kDSNAME;
            set { _kDSNAME = value; OnPropertyChanged(nameof(KDSAPPNAME)); }
        }

        private int _currentItemCount;
        public int CurrentItemCount
        {
            get => _currentItemCount;
            set { _currentItemCount = value; OnPropertyChanged(nameof(CurrentItemCount)); }
        }

        private readonly DispatcherTimer _orderCheckTimer;
        private ObservableCollection<KDSModel> _allOrders;
        private int _selectedIndex;
        private string _currentTime;
        public string CurrentTime
        {
            get => _currentTime;
            set { _currentTime = value; OnPropertyChanged(nameof(CurrentTime)); }
        }

        public ICommand KeyDownCommand { get; }

        public ObservableCollection<KDSModel> AllOrders
        {
            get { return _allOrders; }
            set { _allOrders = value; OnPropertyChanged(nameof(AllOrders)); }
        }

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set { _selectedIndex = value; OnPropertyChanged(nameof(SelectedIndex)); }
        }
        public ICommand SelectNextItemCommand { get; }
        public ICommand SelectPreviousItemCommand { get; }
        public HistorywindowVM(StatusQuery statusQuery, IServiceProvider serviceProvider)
        {
            _querystrings = statusQuery;
            KDSNAME = KDSNAMEE;
            IsStrikethrough = true;
            KDSAPPNAME = KDSNAME;

            KDSLOC = Convert.ToInt32(kdcLocValue);
            AllOrders = new ObservableCollection<KDSModel>();

            // Initialize and start the timer for real-time updates
            DispatcherTimer realTimeUpdateTimer = new DispatcherTimer();
            realTimeUpdateTimer.Interval = TimeSpan.FromSeconds(1);
            realTimeUpdateTimer.Tick += (sender, e) =>
            {
                foreach (var order in AllOrders)
                {
                    order.UpdateDisplayTime();
                }
            };
            realTimeUpdateTimer.Start();

            // Initialize the order check timer
            _orderCheckTimer = new DispatcherTimer();
            _orderCheckTimer.Interval = TimeSpan.FromSeconds(10);
            _orderCheckTimer.Tick += OrderCheckTimer_Tick;

            // Call CheckForOrder method immediately to update the orders without waiting for the first tick
            CheckForOrder();

            // After the initial call, start the timer for subsequent checks
            _orderCheckTimer.Start();

            KeyDownCommand = new RelayCommand(KeyDownHandler);
            FocusGrid();

            _serviceProvider = serviceProvider;

            // Timer to update the current time every second
            DispatcherTimer currentTimeUpdateTimer = new DispatcherTimer();
            currentTimeUpdateTimer.Interval = TimeSpan.FromSeconds(1);
            currentTimeUpdateTimer.Tick += (sender, e) =>
            {
                CurrentTime = DateTime.Now.ToString("hh:mm:ss tt");
            };
            currentTimeUpdateTimer.Start();

            SelectedIndex =0;
        }

        private void UpdateStatusVisibility()
        {
            OnPropertyChanged(nameof(IsFryer));
            OnPropertyChanged(nameof(IsMakeTimeVisible));
            OnPropertyChanged(nameof(IsRackTimeVisible));
            // Add OnPropertyChanged for each status visibility property you add
        }
        private void KeyDownHandler(object key)
        {
            // Check if the object passed as 'key' is actually a Key type
            if (key is Key pressedKey)
            {
                // Switch based on the pressed key
                switch (pressedKey)
                {
                    case Key.D2:
                    case Key.NumPad2:
                    case Key.Down:
                        if (SelectedIndex < AllOrders.Count - 1)
                        {
                            SelectedIndex++;

                        }
                        break;

                    case Key.D8:
                    case Key.NumPad8:
                    case Key.Up:
                        if (SelectedIndex > 0)
                        {
                            SelectedIndex--;
                        }
                        break;
                    case Key.Clear: // For the '5' key above letters
                    case Key.NumPad5: // For the '5' key on the numeric keypad
                        OpenMainWindow();
                       // CloseCurrentWindow();
                        break;
                }
            }
        }

        private void OpenMainWindow()
        {
            // Get the current HistoryWindow instance
            var historyWindow = System.Windows.Application.Current.Windows.OfType<HistoryWindow>().FirstOrDefault();

            // Create an instance of the MainWindow and set its DataContext
            MainWindow mainWindow = new MainWindow();
            mainWindow.DataContext = _serviceProvider.GetRequiredService<MainWindowVM>();

            // Set the application's MainWindow property
            System.Windows.Application.Current.MainWindow = mainWindow;

            // Close the HistoryWindow if it exists
            historyWindow?.Close();

            // Show the MainWindow
            mainWindow.Show();

            // No need for dummy window as we've set the MainWindow before closing the HistoryWindow
        }





        //public static int GetTotal()
        //{

        //    using (OracleHelper.GetCon())
        //        return Convert.ToInt32(OracleHelper.SelectRec("select count(*) total from kds WHERE OPENDATE = (SELECT MAX(OPENDATE) FROM KDS) AND KDS_LOC =  " + KDSLOC).Rows[0][0]);
        //}
        public static int GetTotal()
        {

            using (OracleHelper.GetCon())
                if (KDSLOC == 24)
                    return Convert.ToInt32(OracleHelper.SelectRec("select count(*) total from kds WHERE OPENDATE = (SELECT MAX(OPENDATE) FROM KDS) AND KDS_LOC IN (  " + KDSLOC + ", 25 )").Rows[0][0]);
                else
                    return Convert.ToInt32(OracleHelper.SelectRec("select count(*) total from kds WHERE OPENDATE = (SELECT MAX(OPENDATE) FROM KDS) AND KDS_LOC   = " + KDSLOC).Rows[0][0]);

        }
        public string GetAverageMakeTime()
        {
            string query = _querystrings.average_make_time;
            string queryparameter = query + "and KDS_LOC=" + KDSLOC;

            using (OracleHelper.GetCon())
                return (OracleHelper.SelectRec(queryparameter).Rows[0][0]).ToString();
        }
        public string GetAverageRackTime()
        {
            string query = _querystrings.average_rack_time;

            using (OracleHelper.GetCon())
                return (OracleHelper.SelectRec(query).Rows[0][0]).ToString();
        }
        public string GetAverageOTDTime()
        {
            string query = _querystrings.average_otd_time;

            using (OracleHelper.GetCon())
                return (OracleHelper.SelectRec(query).Rows[0][0]).ToString();
        }
        public string GetAverageTTDTTime()
        {
            string query = _querystrings.average_ttdt_time;

            using (OracleHelper.GetCon())
                return (OracleHelper.SelectRec(query).Rows[0][0]).ToString();
        }
        public string GetAverageCSCPERCENTAGE()
        {
            string query = _querystrings.average_CSC_percentage;

            using (OracleHelper.GetCon())
                return (OracleHelper.SelectRec(query).Rows[0][0]).ToString();
        }
        //public static int GetCurrentTotal()
        //{
        //    using (OracleHelper.GetCon())
        //        return Convert.ToInt32(OracleHelper.SelectRec("select count(*) total from kds where bumped <>1 and opendate = (select max(opendate) from kds) AND KDS_LOC= " + KDSLOC).Rows[0][0]);
        //}
        public static int GetCurrentTotal()
        {
            using (OracleHelper.GetCon())

                if (KDSLOC == 24)
                    return Convert.ToInt32(OracleHelper.SelectRec("select count(*) total from kds where bumped <>1 and opendate = (select max(opendate) from kds) AND KDS_LOC IN ( " + KDSLOC + ",25)").Rows[0][0]);
                else if (KDSLOC == 25)
                {
                    return Convert.ToInt32(OracleHelper.SelectRec("select count(*) total from kds where BRGR_STS <>1 and opendate = (select max(opendate) from kds) AND KDS_LOC = " + KDSLOC).Rows[0][0]);
                }
                else
                    return Convert.ToInt32(OracleHelper.SelectRec("select count(*) total from kds where bumped <>1 and opendate = (select max(opendate) from kds) AND KDS_LOC =  " + KDSLOC).Rows[0][0]);

        }

        private void FocusGrid()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                var historyWindow = System.Windows.Application.Current.MainWindow as HistoryWindow;
                if (historyWindow != null)
                {
                    var grid = historyWindow.FindName("Grid") as System.Windows.Controls.Grid;
                    grid?.Focus();
                }
            });
        }

        private void OrderCheckTimer_Tick(object sender, EventArgs e)
        {
            CheckForOrder();
            
            SelectedIndex = 0;
        }
        private void CheckForOrder()
        {
            try
            {
                ObservableCollection<KDSModel> orders = new ObservableCollection<KDSModel>();
                var KDSLOC = Convert.ToInt32(kdcLocValue);
                string query = string.Empty;
                if(KDSLOC == 24)
                {
                    query = @"select * from KDS WHERE opendate =(select max(opendate) from kds) and KDS_LOC IN ( " + KDSLOC + ", 25) order by transact desc";
                }
                else 
                {
                    query = @"select * from KDS WHERE opendate =(select max(opendate) from kds) and KDS_LOC = " + KDSLOC + " order by transact desc";
                }
                string connectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=KRC)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=SCAR)));User Id=KRC;Password=KRC;";
              //  string connectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=182.180.159.89)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=SCAR)));User Id=KRC;Password=KRC;";

                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();
                    using (OracleCommand cmd = new OracleCommand(query, connection))
                    {
                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            foreach (DataRow dr in dt.Rows)
                            {
                                string transaction = dr["TRANSACT"].ToString();
                                string description = dr["PRODUCT_NAME"].ToString();
                                string TYPE = dr["SALETYPE_NAME"].ToString();
                                int trno = Convert.ToInt32(dr["PKCODE"]);
                                int bumped = Convert.ToInt32(dr["BUMPED"]);
                                string VRMRKS = dr["VRMKS"].ToString();
                                string status = string.Empty;
                                if (bumped == 1)
                                {
                                    status = "Bumped";
                                }
                                else
                                {
                                    status = "In Process";
                                }

                                TimeSpan kdsTimeSpan; // This should be a TimeSpan to hold the difference between two DateTime objects.

                                if (dr["STARTTIME"] != DBNull.Value && dr["STARTTIME"] != null)
                                {
                                    KdsdateTime = (DateTime)dr["STARTTIME"];
                                    DateTime startTime = (DateTime)dr["STARTTIME"]; // Get the start time from the DataRow.

                                    KDSModel model = new KDSModel
                                    {
                                        Descript = description,
                                        Transact = transaction,
                                        Status = status, // Store the actual start time in the model.
                                        TRNO = trno,
                                        TYPE = TYPE,
                                        VRMKS=VRMRKS
                                    };
                                    if(VRMRKS== "VOIDED")
                                    {
                                        model.IsStrikethrough= true;
                                    }

                                    // The DisplayTime will be calculated based on the StartTime and the current time.
                                    model.UpdateDisplayTime(); // Ensure this method updates the DisplayTime based on the StartTime.
                                    
                                    orders.Add(model);

                                }


                            }
                            
                            AllOrders = orders;
                        }

                    }
                }
                TotalItemCount = GetTotal();
                CurrentItemCount = GetCurrentTotal();
                AverageMakeTime = GetAverageMakeTime();
                //AverageoCSCTime =GetAverageCSCPERCENTAGE();
                //AverageotdTime =GetAverageOTDTime();
                //AverageottdtTime = GetAverageTTDTTime();
                //AverageRackTime=GetAverageRackTime();
                // AllOrders = orders;
                if (AllOrders.Count > 0)
                {
                    SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine("Error occurred: " + ex.Message);
            }
        }

    }
}
