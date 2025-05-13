using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для AddRequest.xaml
    /// </summary>
    public partial class AddRequest : Page
    {
        MainWindow mainWindow;
        private Users currentUser;

        public AddRequest(MainWindow mainWindow_, Users user)
        {
            InitializeComponent();
            this.mainWindow = mainWindow_;
            this.currentUser = user;
        }

        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtEquipment.Text.Length > 0 && txtKabinet.Text.Length > 0 && txtFaultType.Text.Length > 0 && txtDescription.Text.Length > 0)
                {
                    if (!IsValidEquipment(txtEquipment.Text))
                    {
                        MessageBox.Show("Недопустимое название оборудования.\nЗаполните без вводных знаков и на русском!");
                        return;
                    }
                    if (!IsValidKabinet(txtKabinet.Text))
                    {
                        MessageBox.Show("Заполните поле кабинет правильно!\nПример: A209");
                        return;
                    }
                    if (!IsValidDescription(txtDescription.Text))
                    {
                        MessageBox.Show("Недопустимое описание оборудования.\nЗаполните без вводных знаков и на русском!");
                        return;
                    }

                    DateTime dateAdded = DateTime.Now;
                    string formattedDate = dateAdded.ToString("dd/MM/yyyy HH:mm");
                    string equipment = txtEquipment.Text;
                    string faultType = txtFaultType.Text;
                    string description = txtDescription.Text;
                    string kabinet = txtKabinet.Text;

                    string query = $"INSERT INTO Requests (Registration_Date, Equipment_Type, Equipment, ProblemDescription, Contact_Information," +
                        $"Status, Assignee, Priority, Idclient, Full_name, DateEnd, Kabinet)" +
                        $" VALUES (@Registration_Date, @Equipment_Type, @Equipment, @ProblemDescription," +
                        $"'{currentUser.Contact_Information}', 'в ожидании', 'Не назначен', 'стандартный', @Idclient, @Full_name, 'Не назначено', @Kabinet)";

                    List<MySqlParameter> parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@Registration_Date", formattedDate),
                new MySqlParameter("@Equipment_Type", faultType),
                new MySqlParameter("@Equipment", equipment),
                new MySqlParameter("@ProblemDescription", description),
                new MySqlParameter("@Kabinet", kabinet),
                new MySqlParameter("@Full_name", currentUser.Full_Name),
                new MySqlParameter("@Idclient", currentUser.Id)
            };

                    Connection.SqlConnection(query, parameters);

                    MessageBox.Show("Заявка успешно добавлена!");

                    // Отправка сообщения в Telegram
                    string telegramMessage = $"✅ Новая заявка от {currentUser.Full_Name}!\n" +
                                             $"📅 Дата: {formattedDate}\n" +
                                             $"📍 Кабинет: {kabinet}\n" +
                                             $"🔧 Оборудование: {equipment}\n" +
                                             $"📋 Проблема: {description}";
                    await SendMessageToTelegram(telegramMessage);

                    // Переход на главную страницу
                    main mainPage = new main(mainWindow, currentUser);
                    mainWindow.OpenPages(mainPage);
                }
                else
                {
                    MessageBox.Show("Заполните все поля!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении заявки: {ex.Message}");
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
                    Console.WriteLine($"Ошибка при отправке в Telegram: {ex.Message}");
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                main mainPage = new main(mainWindow, currentUser); // передаем оба аргумента
                mainWindow.OpenPages(mainPage);
            }
        }
        public bool IsValidEquipment(string input)
        {
            var regex = new Regex(@"^[а-яА-ЯёЁ]+$");
            return regex.IsMatch(input);
        }
        public bool IsValidKabinet(string input)
        {
            var regex = new Regex(@"^[а-яА-ЯёЁA0-9]+$");
            return regex.IsMatch(input);
        }
        public bool IsValidDescription(string input)
        {
            var regex = new Regex(@"^[а-яА-ЯёЁ\s,-]+$");
            return regex.IsMatch(input);
        }
    }
}
