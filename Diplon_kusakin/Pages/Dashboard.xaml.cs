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
        public int OpenRequestCount { get; set; }
        public string AverageCloseTime { get; set; }
        public SeriesCollection ExecutorLoadSeries { get; set; }
        public List<string> ExecutorLabels { get; set; }

        private MainWindow mainWindow;
        private Users currentUser;
        public Dashboard(MainWindow mainWindow_, Users user)
        {
            mainWindow = mainWindow_;
            currentUser = user;
            InitializeComponent();
            LoadDashboardData();
            DataContext = this;
        }

        

        private int GetOpenRequestCount()
        {
            // Возврат количества открытых заявок из базы
            return 12; // пример
        }

        private string GetAverageCloseTimeFormatted()
        {
            // Возврат среднего времени закрытия
            return "2 дн. 4 ч.";
        }

        private Dictionary<string, int> GetExecutorLoad()
        {
            // Пример — загрузка исполнителей
            return new Dictionary<string, int>
            {
                { "Иванов", 5 },
                { "Петров", 8 },
                { "Сидоров", 3 }
            };
        }

        

        private void BackToRequests_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService?.Navigate(new manager(mainWindow, currentUser));
        }

        private void Settings_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Навигация в настройки
        }

        private void LoadDashboardData()
        {
            try
            {
                // 1. Кол-во открытых заявок (в ожидании)
                string openQuery = "SELECT COUNT(*) FROM Requests WHERE Status = 'в ожидании'";
                object openCount = Connection.ExecuteScalar(openQuery);
                OpenRequestsTextBlock.Text = openCount.ToString();

                // 2. Среднее время закрытия (в днях)
                string avgQuery = @"
                    SELECT AVG(DATEDIFF(STR_TO_DATE(DateEnd, '%d.%m.%Y'), STR_TO_DATE(Registration_Date, '%d.%m.%Y %H:%i'))) 
                    FROM Requests 
                    WHERE Status = 'готово' AND DateEnd <> 'Не назначено'";
                object avgDays = Connection.ExecuteScalar(avgQuery);
                AvgCloseTimeTextBlock.Text = avgDays != DBNull.Value ? $"{Math.Round(Convert.ToDouble(avgDays), 1)} дн." : "—";

                // 3. Нагрузка: кол-во заявок на каждого исполнителя
                string loadQuery = @"
                    SELECT Assignee, COUNT(*) AS TaskCount 
                    FROM Requests 
                    WHERE Assignee <> 'Не назначен'
                    GROUP BY Assignee";
                DataTable dt = Connection.ExecuteDataTable(loadQuery);
                LoadChart.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки статистики: " + ex.Message);
            }
        }

        private void RefreshData_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

