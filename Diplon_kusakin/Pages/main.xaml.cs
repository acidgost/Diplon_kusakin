using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using MySql.Data.MySqlClient;

namespace Diplon_kusakin.Pages
{
    /// <summary>
    /// Логика взаимодействия для main.xaml
    /// </summary>
    public partial class main : Page
    {
        MainWindow mainWindow;
        private Users currentUser;
        private ObservableCollection<Request> originalRequests;
        public ObservableCollection<Request> userRequests;

        public main(MainWindow mainWindow_, Users user)
        {
            InitializeComponent();
            mainWindow = mainWindow_;
            currentUser = user;
            LoadUserRequests();
            name.Content = currentUser.Full_Name;
            originalRequests = userRequests;
        }

        public void LoadUserRequests()
        {
            try
            {
                userRequests = new ObservableCollection<Request>();
                List<MySqlParameter> parameters = new List<MySqlParameter>();

                string query = $"SELECT * FROM Requests WHERE Idclient = {currentUser.Id}";
                using (MySqlDataReader reader = Connection.SqlConnection(query, parameters))
                {
                    while (reader.Read())
                    {
                        Request request = new Request
                        {
                            Id = reader.GetInt32("id"),
                            Registration_Date = reader.GetString("Registration_Date"),
                            Equipment_Type = reader.GetString("Equipment_Type"),
                            Equipment = reader.GetString("Equipment"),
                            Kabinet = reader.GetString("Kabinet"),
                            ProblemDescription = reader.GetString("ProblemDescription"),
                            Contact_Information = reader.GetString("Contact_Information"),
                            Status = reader.GetString("Status"),
                            Assignee = reader.GetString("Assignee"),
                            Priority = reader.GetString("Priority")
                        };


                        if (!reader.IsDBNull(reader.GetOrdinal("Prosmotr")))
                        {
                            request.Prosmotr = reader.GetString("Prosmotr");
                        }
                        else
                        {
                            request.Prosmotr = null;
                        }

                        userRequests.Add(request);

                        if (request.Status == "готово" && request.Prosmotr != "Просмотренно")
                        {
                            MessageBox.Show("Ваша заявка: " + request.Id + " готова!\nСкоро с вами свяжутся.");
                            string sqlquery = $"UPDATE Requests SET Prosmotr = 'Просмотренно' WHERE id = {request.Id}";
                            Connection.SqlConnection(sqlquery, parameters);
                        }
                    }
                }

                requestsListView.ItemsSource = userRequests;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке заявок пользователя: {ex.Message}");
            }
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                login loginPage = new login(mainWindow, currentUser);
                mainWindow.OpenPages(loginPage);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //AddRequest addRequestPage = new AddRequest(mainWindow, currentUser);
            //mainWindow.OpenPages(addRequestPage);
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            Request selectedRequest = (Request)requestsListView.SelectedItem;
            if (selectedRequest == null)
            {
                MessageBox.Show("Пожалуйста, выберите заявку для редактирования.");
                return;
            }
            if (selectedRequest.Status == "готово")
            {
                MessageBox.Show("Заявка выполнена.");
                return;
            }

            //UpdateRequest updateRequestPage = new UpdateRequest(selectedRequest, mainWindow, currentUser);
            //mainWindow.OpenPages(updateRequestPage);
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchTerm = searchTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                requestsListView.ItemsSource = originalRequests;
                MessageBox.Show("Введите номер или параметры для поиска.", "Поиск", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            SearchRequests(searchTerm);
        }

        private void SearchRequests(string searchTerm)
        {
            var searchResults = userRequests.Where(request =>
                request.Id.ToString().Contains(searchTerm) ||
                request.Equipment.Contains(searchTerm) ||
                request.Kabinet.Contains(searchTerm) ||
                request.ProblemDescription.Contains(searchTerm) ||
                request.Registration_Date.ToString().Contains(searchTerm) ||
                request.Equipment_Type.Contains(searchTerm) ||
                request.Status.Contains(searchTerm)).ToList();

            if (searchResults.Count == 0)
            {
                requestsListView.ItemsSource = originalRequests;
                MessageBox.Show("Ничего не найдено. Показаны все записи.", "Поиск", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                requestsListView.ItemsSource = new ObservableCollection<Request>(searchResults);
            }
        }
    }
}
