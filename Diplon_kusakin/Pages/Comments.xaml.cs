using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Логика взаимодействия для Comments.xaml
    /// </summary>
    public partial class Comments : Page
    {
        private Request selectedRequest;
        MainWindow mainWindow;
        Users user;
        public Comments(Request selectedRequest_, MainWindow _mainWindow, Users user_)
        {
            InitializeComponent();
            selectedRequest = selectedRequest_;
            mainWindow = _mainWindow;
            user = user_;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                mainWindow.OpenPages(new Pages.ispolnitel(mainWindow, user));
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtDescription.Text.Length > 0)
                {
                    string description = txtDescription.Text;
                    if (!IsValidDescription(description))
                    {
                        MessageBox.Show("Поле комментарий было записано неверно.\nЗаполните без вводных знаков и на русском!");
                        return;
                    }

                    string query = $"INSERT INTO Comments (Comment, IdRequest, Assignee) VALUES (@Comment, @IdRequest, @Assignee)";
                    List<MySqlParameter> parameters = new List<MySqlParameter>
                {
                    new MySqlParameter("@Comment", description),
                    new MySqlParameter("@IdRequest",selectedRequest.Id ),
                    new MySqlParameter("@Assignee",selectedRequest.Assignee )

                };

                    Connection.SqlConnection(query, parameters);

                    MessageBox.Show("Комментарий успешно добавлен!");
                    mainWindow.OpenPages(new Pages.ispolnitel(mainWindow, user));
                }
                else
                {
                    MessageBox.Show("Заполните поле");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении комментария: {ex.Message}");
            }
        }
        public bool IsValidDescription(string input)
        {
            var regex = new Regex(@"^[а-яА-ЯёЁ\s,-]+$");
            return regex.IsMatch(input);
        }
    }
}
