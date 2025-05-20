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
        MainWindow mainWindow;
        public int OpenRequestCount { get; set; }
        public string AverageCloseTime { get; set; }
        public SeriesCollection LoadSeries { get; set; }
        public Dashboard(MainWindow mainWindow_)
        {
            mainWindow = mainWindow_;
            InitializeComponent();
            LoadStatistics();
            DataContext = this;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Логика при загрузке страницы/окна, если нужна
        }
        private void LoadStatistics()
        {
            try
            {
                List<MySqlParameter> parameters = new List<MySqlParameter>();

                // Открытые заявки
                string queryOpen = "SELECT COUNT(*) FROM Requests WHERE Status != 'готово'";
                using (var reader = Connection.SqlConnection(queryOpen, parameters))
                {
                    if (reader.Read())
                        OpenRequestCount = reader.GetInt32(0);
                }

                // Среднее время закрытия
                string queryAverage = "SELECT AVG(TIMESTAMPDIFF(HOUR, Registration_Date, DateEnd)) FROM Requests WHERE Status = 'готово'";
                using (var reader = Connection.SqlConnection(queryAverage, parameters))
                {
                    if (reader.Read())
                    {
                        double avgHours = reader.IsDBNull(0) ? 0 : reader.GetDouble(0);
                        AverageCloseTime = $"{Math.Round(avgHours, 1)} ч.";
                    }
                }

                // Нагрузка на исполнителей
                Dictionary<string, int> loadData = new Dictionary<string, int>();
                string queryLoad = "SELECT Assignee, COUNT(*) as Count FROM Requests GROUP BY Assignee";
                using (var reader = Connection.SqlConnection(queryLoad, parameters))
                {
                    while (reader.Read())
                    {
                        string name = reader.IsDBNull(0) ? "Не назначен" : reader.GetString(0);
                        int count = reader.GetInt32(1);
                        loadData[name] = count;
                    }
                }

                LoadSeries = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Заявки",
                        Values = new ChartValues<int>(loadData.Values)
                    }
                };

                var chart = this.FindName("ChartControl") as CartesianChart;
                if (chart != null)
                {
                    chart.AxisX.Clear();
                    chart.AxisX.Add(new Axis { Labels = loadData.Keys.ToList() });
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при загрузке статистики: {ex.Message}");
            }
        }
    }

}
