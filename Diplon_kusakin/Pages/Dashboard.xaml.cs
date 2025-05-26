using System;
using System.Collections.Generic;
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

        private void LoadDashboardData()
        {
            // Пример данных — замени своими из БД
            OpenRequestCount = GetOpenRequestCount();
            AverageCloseTime = GetAverageCloseTimeFormatted();

            var executorData = GetExecutorLoad(); // словарь Имя -> Кол-во заявок

            ExecutorLabels = executorData.Keys.ToList();
            ExecutorLoadSeries = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Заявки",
                    Values = new ChartValues<int>(executorData.Values)
                }
            };
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

        private void RefreshData_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            LoadDashboardData();
            DataContext = null;
            DataContext = this;
        }

        private void BackToRequests_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService?.Navigate(new manager(mainWindow, currentUser));
        }

        private void Settings_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Навигация в настройки
        }
    }
}

