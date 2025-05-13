using MySql.Data.MySqlClient;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
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
using System.Net.Http;
using System.Threading.Tasks;

namespace Diplon_kusakin.Pages
{
    /// <summary>
    /// Логика взаимодействия для ispolnitel.xaml
    /// </summary>
    public partial class ispolnitel : Page
    {
        MainWindow mainWindow;
        private static bool requestsChecked = false;
        private Users currentUser;
        private ObservableCollection<Request> originalRequests;
        public ObservableCollection<Request> userRequests;

        public ispolnitel(MainWindow mainWindow_, Users user)
        {
            InitializeComponent();
            this.mainWindow = mainWindow_;
            this.currentUser = user;

            LoadUserRequests();
            name.Content = currentUser.Full_Name;
            originalRequests = userRequests;
        }

        public async Task SendMessageToTelegram(string message)
        {
            string token = "7720783710:AAFsO-tlOKcH_wznUyVPlg3yc5omTY63-jU";
            string chatId = "542716186";
            string url = $"https://api.telegram.org/bot{token}/sendMessage?chat_id={chatId}&text={Uri.EscapeDataString(message)}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Ошибка при отправке сообщения: {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при отправке запроса: {ex.Message}");
                }
            }
        }

        public async Task LoadUserRequests()
        {
            try
            {
                userRequests = new ObservableCollection<Request>();

                string query = $"SELECT * FROM Requests";
                using (MySqlDataReader reader = Connection.SqlConnection(query))
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

                        if (request.Status == "в работе" && !requestsChecked)
                        {
                            string message = $"Вам назначенна заявка: {request.Id}";
                            await SendMessageToTelegram(message); // ✅ теперь корректно
                        }
                    }
                }
                requestsChecked = true;
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
                login loginPage = new login(mainWindow, currentUser);
                mainWindow.OpenPages(loginPage);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
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


            ReadyIsp ReadyIsp = new ReadyIsp(selectedRequest, mainWindow, currentUser);
            mainWindow.OpenPages(ReadyIsp);
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

            MaterialsIsp MaterialsIsp = new MaterialsIsp(selectedRequest, mainWindow, currentUser);
            mainWindow.OpenPages(MaterialsIsp);
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

        private void btnOtchhet_Click(object sender, RoutedEventArgs e)
        {
            Request selectedRequest = requestsListView.SelectedItem as Request;

            if (selectedRequest == null)
            {
                MessageBox.Show("Пожалуйста, выберите заявку.");
                return;
            }
            if (selectedRequest.Status != "готово")
            {
                MessageBox.Show("Пожалуйста, выберите готовую заявку.");
                return;
            }

            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Arial", 12);

            string query = $"SELECT * FROM isp WHERE IdClient = @IdClient";
            List<MySqlParameter> parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@IdClient", selectedRequest.Id)
            };

            try
            {
                using (MySqlDataReader reader = Connection.SqlConnection(query, parameters))
                {
                    int row = 0;

                    DateTime registrationDate = DateTime.Parse(selectedRequest.Registration_Date);
                    DateTime dateEnd = DateTime.Parse(selectedRequest.DateEnd);
                    TimeSpan difference = dateEnd - registrationDate;
                    int daysDifference = (int)difference.TotalDays;

                    gfx.DrawString("Отчёт", font, XBrushes.Black, new XPoint(20, 20 + row * 20));
                    row++;

                    gfx.DrawString($"Затраченное время: {daysDifference} дней", font, XBrushes.Black, new XPoint(20, 20 + row * 20));
                    row++;

                    while (reader.Read())
                    {
                        string materials = reader.GetString("Materials");
                        gfx.DrawString($"Материалы: {materials}", font, XBrushes.Black, new XPoint(20, 20 + row * 20));
                        row++;

                        string cost = reader.GetString("Cost");
                        gfx.DrawString($"Стоимость: {cost}", font, XBrushes.Black, new XPoint(20, 20 + row * 20));
                        row++;

                        string type = reader.GetString("Type");
                        gfx.DrawString($"Причина неисправности: {type}", font, XBrushes.Black, new XPoint(20, 20 + row * 20));
                        row++;

                        string help = reader.GetString("Help");
                        gfx.DrawString($"Оказанная помощь: {help}", font, XBrushes.Black, new XPoint(20, 20 + row * 20));
                        row++;
                    }
                }

                string tempFileName = System.IO.Path.GetTempFileName();
                document.Save(tempFileName);
                document.Close();

                System.Diagnostics.Process.Start(tempFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
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

            CommentsAdd commentsAddPage = new CommentsAdd(selectedRequest, mainWindow, currentUser);
            mainWindow.OpenPages(commentsAddPage);
        }
    }
}
