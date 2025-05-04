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
using Microsoft.Win32;
using MySql.Data.MySqlClient;

namespace Diplon_kusakin.Pages
{
    /// <summary>
    /// Логика взаимодействия для manager.xaml
    /// </summary>
    public partial class manager : Page
    {
        private Users currentUser;
        Users users;
        MainWindow mainWindow;

        private ObservableCollection<Request> originalRequests;
        public ObservableCollection<Request> userRequests;
        Request request1;
        public manager(MainWindow mainWindow_, Users currentUser)
        {
            mainWindow = mainWindow_;
            InitializeComponent();
            LoadUserRequests();
            originalRequests = userRequests;

        }
        public void LoadUserRequests()
        {

            try
            {
                userRequests = new ObservableCollection<Request>();


                List<MySqlParameter> parameters = new List<MySqlParameter>
                {

                };


                string query = $"SELECT * FROM Requests ";
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
                            Full_name = reader.GetString("Full_name"),
                            Priority = reader.GetString("Priority"),
                            DateEnd = reader.GetString("DateEnd")
                        };
                        userRequests.Add(request);
                    }
                }

                requestsListView.ItemsSource = userRequests;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке заявок: {ex.Message}");
            }
        }
        private void Exit(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                login loginPage = new login(mainWindow, users);
                mainWindow.OpenPages(loginPage);
            }

        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {

            Request selectedRequest = (Request)requestsListView.SelectedItem;


            if (selectedRequest == null)
            {
                MessageBox.Show("Пожалуйста, выберите заявку для редактирования.");
                return;
            }
            else if (selectedRequest.Status == "готово")
            {
                MessageBox.Show("Заявка уже выполнена!");
                return;
            }

            UpdateRequestManager updateRequestPage = new UpdateRequestManager(selectedRequest, mainWindow);
            mainWindow.OpenPages(updateRequestPage);
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
                request.Equipment.ToString().Contains(searchTerm) ||
                request.Kabinet.ToString().Contains(searchTerm) ||
                request.ProblemDescription.ToString().Contains(searchTerm) ||
                request.Registration_Date.ToString().Contains(searchTerm) ||
                request.Equipment_Type.ToString().Contains(searchTerm) ||
                request.Full_name.ToString().Contains(searchTerm) ||
                request.DateEnd.ToString().Contains(searchTerm) ||
                request.Assignee.ToString().Contains(searchTerm) ||
                request.Status.ToString().Contains(searchTerm)).ToList();
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

        private void btnComment_Click(object sender, RoutedEventArgs e)
        {
            Request selectedRequest = requestsListView.SelectedItem as Request;

            if (selectedRequest == null)
            {
                MessageBox.Show("Пожалуйста, выберите заявку.");
                return;
            }

            mainWindow.OpenPages(new Pages.CommentMeneger(mainWindow, selectedRequest));
        }

        private void Addisp_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.OpenPages(new Pages.AddIsp(mainWindow));
        }

        private void User_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.OpenPages(new Pages.User(mainWindow, users));
        }

        private void Sotrud_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.OpenPages(new Pages.Sotrudniki(mainWindow));
        }

        private void Koment_Click(object sender, RoutedEventArgs e)
        {

        }
        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Request request)
            {
                MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите удалить заявку {request.Full_name}?", "Подтверждение удаления", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        string query = $"DELETE FROM Requests WHERE Id = @Id";
                        List<MySqlParameter> parameters = new List<MySqlParameter>
                    {
                        new MySqlParameter("@Id", request.Id)
                    };
                        Connection.SqlConnection(query, parameters);
                        userRequests.Remove(request);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении сотрудника: {ex.Message}");
                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                Microsoft.Office.Interop.Excel.Application ObjExcel = null;
                try
                {

                    string path = dialog.FileName;
                    ObjExcel = new Microsoft.Office.Interop.Excel.Application();
                    //Открываем книгу.                                                                                                                                                        
                    Microsoft.Office.Interop.Excel.Workbook ObjWorkBook = ObjExcel.Workbooks.Open(path, 0, false, 5, "", "", false, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
                    Microsoft.Office.Interop.Excel.Worksheet sheet = (Microsoft.Office.Interop.Excel.Worksheet)ObjWorkBook.Sheets[1];

                    for (int i = 2; i <= sheet.UsedRange.Rows.Count; i++)
                    {
                        string name = sheet.Cells[i, 1].Value;
                        string login = Convert.ToString(sheet.Cells[i, 2].Value);
                        string password = Convert.ToString(sheet.Cells[i, 3].Value);
                        string email = Convert.ToString(sheet.Cells[i, 4].Value);
                        string role = sheet.Cells[i, 5].Value;

                        string query = $"INSERT INTO `dip`.`Users`\n(\n`Full_Name`,\n`Position`,\n`Contact_Information`,\n`Username`,\n`Password`)\nVALUES\n('{name}',\n'{role}',\n'{email}',\n'{login}',\n'{password}');";
                        MySqlDataReader reader = Connection.SqlConnection(query);
                        reader.Close();
                    }
                    ObjExcel.Quit();
                    MessageBox.Show("Сохранено");
                }
                catch
                {
                    MessageBox.Show("ошибка");
                    if (ObjExcel != null)
                    {
                        ObjExcel.Quit();
                    }


                }
            }
        }
    }
}
