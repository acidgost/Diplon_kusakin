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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;

namespace Diplon_kusakin.Pages
{
    /// <summary>
    /// Логика взаимодействия для UpdateRequestManager.xaml
    /// </summary>
    public partial class UpdateRequestManager : Page
    {
        private Request selectedRequest;
        MainWindow mainWindow;
        Users users;
        public UpdateRequestManager(Request selectedRequest_, MainWindow mainWindow_)
        {
            this.mainWindow = mainWindow_;

            InitializeComponent();
            selectedRequest = selectedRequest_;
            LoadDataIntoComboBox();



        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                manager manager = new manager(mainWindow, users);
                mainWindow.OpenPages(manager);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (ComboBoxAigree.SelectedItem == null)
                {
                    MessageBox.Show("Выбирите исполнителя!");
                    return;
                }
                if (DatePicker.SelectedDate == null)
                {
                    MessageBox.Show("Выбирите дату!");
                    return;
                }

                string query = "UPDATE Requests SET Assignee = @Assignee, DateEnd = @DateEnd, Status = 'в работе' WHERE Id = @RequestId";
                List<MySqlParameter> parameters = new List<MySqlParameter>
                {
                    new MySqlParameter("@Assignee", ComboBoxAigree.Text),
                    new MySqlParameter("@DateEnd", DatePicker.Text),
                    new MySqlParameter("@RequestId", selectedRequest.Id)
                };

                Connection.SqlConnection(query, parameters);

                MessageBox.Show("Данные успешно обновлены.");
                manager manager = new manager(mainWindow, users);
                mainWindow.OpenPages(manager);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении данных: {ex.Message}");
            }
        }

        private void LoadDataIntoComboBox()
        {
            try
            {
                string query = "SELECT Full_Name FROM Users WHERE Position = 'isp'";
                List<string> users = new List<string>();

                using (MySqlDataReader reader = Connection.SqlConnection(query))
                {
                    while (reader.Read())
                    {
                        string userName = reader["Full_Name"].ToString();
                        users.Add(userName);
                    }
                }

                ComboBoxAigree.ItemsSource = users;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных из базы данных: {ex.Message}");
            }
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation fadeIn = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.5)));
            MainContainer.BeginAnimation(OpacityProperty, fadeIn);
        }

    }
}
