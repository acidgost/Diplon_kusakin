using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
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
using System.Windows.Media.Animation;

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

        public async Task LoadUserRequests()
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
                            Priority = reader.GetString("Priority"),
                            Prosmotr = reader.IsDBNull(reader.GetOrdinal("Prosmotr"))
                                ? null
                                : reader.GetString("Prosmotr")
                        };

                        userRequests.Add(request);

                        if (request.Status == "готово" && request.Prosmotr != "Просмотренно")
                        {
                            string message = $"Ваша заявка №{request.Id} готова! Скоро с вами свяжутся.";
                            MessageBox.Show(message);
                            await SendMessageToTelegram(message);

                            string updateQuery = $"UPDATE Requests SET Prosmotr = 'Просмотренно' WHERE id = {request.Id}";
                            await Task.Run(() => Connection.SqlConnection(updateQuery, parameters));
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
                    Console.WriteLine($"Ошибка при отправке запроса в Telegram: {ex.Message}");
                }
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
            AddRequest addRequestPage = new AddRequest(mainWindow, currentUser);
            mainWindow.OpenPages(addRequestPage);
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

            UpdateRequest updateRequestPage = new UpdateRequest(selectedRequest, mainWindow, currentUser);
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

        private bool isChatOpen = false;

        private void ChatToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isChatOpen)
            {
                ChatPanel.Visibility = Visibility.Visible;
                Storyboard showChat = (Storyboard)this.FindResource("ShowChatStoryboard");
                showChat.Begin();
            }
            else
            {
                Storyboard hideChat = (Storyboard)this.FindResource("HideChatStoryboard");
                hideChat.Completed += (s, _) => ChatPanel.Visibility = Visibility.Collapsed;
                hideChat.Begin();
            }
            isChatOpen = !isChatOpen;
        }

        private async void SendMessageToAI_Click(object sender, RoutedEventArgs e)
        {
            string userMessage = UserMessageTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(userMessage)) return;

            AddChatMessage("Вы: " + userMessage, HorizontalAlignment.Right);
            UserMessageTextBox.Text = "";

            // Здесь можно использовать API OpenAI, если он подключен
            string aiResponse = await GetFakeAIResponse(userMessage); // временно

            AddChatMessage("ИИ: " + aiResponse, HorizontalAlignment.Left);
        }

        private void AddChatMessage(string text, HorizontalAlignment alignment)
        {
            TextBlock message = new TextBlock
            {
                Text = text,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                Background = alignment == HorizontalAlignment.Right ? Brushes.LightBlue : Brushes.LightGray,
                Foreground = Brushes.Black,
                Padding = new Thickness(8),
                HorizontalAlignment = alignment,
                MaxWidth = 220,
                
            };

            ChatHistoryPanel.Children.Add(message);
            ChatHistoryViewer.ScrollToEnd();
        }

        // Заглушка вместо реального ИИ-запроса
        private async Task<string> GetFakeAIResponse(string userMessage)
        {
            await Task.Delay(500); // эмуляция ответа
            return "Это тестовый ответ ИИ.";
        }

    }
}
