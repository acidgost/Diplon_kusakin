using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using LiveCharts;
using LiveCharts.Wpf;
using MySql.Data.MySqlClient;


namespace Diplon_kusakin.Pages
{
    /// <summary>
    /// Логика взаимодействия для Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Page
    {
        public List<string> ExecutorNames { get; set; }
        public int OpenRequestCount { get; set; }
        public string AverageCloseTime { get; set; }
        public SeriesCollection ExecutorLoadSeries { get; set; }
        public List<string> ExecutorLabels { get; set; }

        private MainWindow mainWindow;
        private Users currentUser;

        DispatcherTimer timer = new DispatcherTimer();
        public Dashboard(MainWindow mainWindow_, Users user)
        {
            mainWindow = mainWindow_;
            currentUser = user;
            InitializeComponent();
            LoadDashboardData();
            DataContext = this;

            timer.Interval = TimeSpan.FromSeconds(60);
            timer.Tick += (s, e) =>
            {
                UpdatedTimeTextBlock.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
            };
            timer.Start();

        }



        private void LoadDashboardData()
        {
            try
            {
                // Открытые заявки
                string openQuery = "SELECT COUNT(*) FROM Requests WHERE Status = 'в ожидании'";
                object openCount = Connection.ExecuteScalar(openQuery);
                OpenRequestsTextBlock.Text = openCount.ToString();

                // Среднее время
                string avgQuery = @"
                    SELECT AVG(DATEDIFF(STR_TO_DATE(DateEnd, '%d.%m.%Y'), STR_TO_DATE(Registration_Date, '%d.%m.%Y %H:%i'))) 
                    FROM Requests 
                    WHERE Status = 'готово' AND DateEnd <> 'Не назначено'";
                object avg = Connection.ExecuteScalar(avgQuery);
                AvgCloseTimeTextBlock.Text = avg != DBNull.Value ? $"{Math.Round(Convert.ToDouble(avg), 1)} дн." : "—";

                // Нагрузка исполнителей
                string loadQuery = @"
                    SELECT Assignee, COUNT(*) AS TaskCount 
                    FROM Requests 
                    WHERE Assignee <> 'Не назначен'
                    GROUP BY Assignee";
                DataTable dt = Connection.ExecuteDataTable(loadQuery);

                ExecutorNames = new List<string>();
                ChartValues<int> values = new ChartValues<int>();

                foreach (DataRow row in dt.Rows)
                {
                    ExecutorNames.Add(row["Assignee"].ToString());
                    values.Add(Convert.ToInt32(row["TaskCount"]));
                }

                LoadChart.Series = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Заявки",
                        Values = values
                    }
                };

                LoadChart.AxisX[0].Labels = ExecutorNames;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки статистики: " + ex.Message);
            }
        }

        private void RefreshData_Click(object sender, RoutedEventArgs e) => LoadDashboardData();
        private void BackToRequests_Click(object sender, RoutedEventArgs e) => NavigationService.GoBack();
        private void Settings_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Открытие настроек...");
    }
}


