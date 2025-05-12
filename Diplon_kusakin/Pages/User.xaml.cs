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
    /// Логика взаимодействия для User.xaml
    /// </summary>
    public partial class User : Page
    {
        private ObservableCollection<Users> originalUsers;
        public ObservableCollection<Users> Users;
        MainWindow mainWindow;
        Users users;
        public User(MainWindow _mainWindow, Users user)
        {
            InitializeComponent();
            mainWindow = _mainWindow;
            LoadUsers();
            originalUsers = Users;
        }

        public void LoadUsers()
        {
            try
            {
                Users = new ObservableCollection<Users>();

                List<MySqlParameter> parameters = new List<MySqlParameter>();

                string query = $"SELECT * FROM Users WHERE Position = 'user'";
                using (MySqlDataReader reader = Connection.SqlConnection(query, parameters))
                {
                    while (reader.Read())
                    {
                        Users user = new Users
                        {
                            Full_Name = reader.GetString("Full_name"),
                            Contact_Information = reader.GetString("Contact_Information"),
                            Username = reader.GetString("Username"),
                            Password = reader.GetString("Password")

                        };
                        Users.Add(user);

                    }
                }

                usersListView.ItemsSource = Users;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке пользователей: {ex.Message}");
            }
        }



        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Users user)
            {
                mainWindow.OpenPages(new Pages.EddUser(mainWindow, user));
            }
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Users user)
            {
                MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите удалить сотрудника {user.Full_Name}?", "Подтверждение удаления", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        string query = $"DELETE FROM Users WHERE Username = @Username";
                        List<MySqlParameter> parameters = new List<MySqlParameter>
                    {
                        new MySqlParameter("@Username", user.Username)
                    };
                        Connection.SqlConnection(query, parameters);
                        Users.Remove(user);
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
            mainWindow.OpenPages(new Pages.manager(mainWindow, users));
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchTerm = searchTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                usersListView.ItemsSource = originalUsers;
                MessageBox.Show("Введите номер или параметры для поиска.", "Поиск", MessageBoxButton.OK, MessageBoxImage.Information);

                return;
            }


            SearchUsers(searchTerm);
        }
        private void SearchUsers(string searchTerm)
        {

            var searchResults = Users.Where(user =>
                user.Full_Name.ToString().Contains(searchTerm) ||
                user.Contact_Information.ToString().Contains(searchTerm) ||
                user.Username.ToString().Contains(searchTerm) ||
                user.Password.ToString().Contains(searchTerm)).ToList();
            if (searchResults.Count == 0)
            {

                usersListView.ItemsSource = originalUsers;
                MessageBox.Show("Ничего не найдено. Показаны все записи.", "Поиск", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {

                usersListView.ItemsSource = new ObservableCollection<Users>(searchResults);
            }

        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.OpenPages(new Pages.AddUser(mainWindow));
        }
    }
}
