using MySql.Data.MySqlClient;
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
using System.IO;


namespace Diplon_kusakin.Pages
{
    /// <summary>
    /// Логика взаимодействия для login.xaml
    /// </summary>
    public partial class login : Page
    {
        MainWindow mainWindow;
        private Users currentUser;
        Users user;
        Request request = new Request();

        public login(MainWindow mainWindow_, Users user)
        {
            this.mainWindow = mainWindow_;
            InitializeComponent();
            this.user = user;

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.OpenPages(new Pages.reg(mainWindow, currentUser));
        }

        private void butlog_Click_1(object sender, RoutedEventArgs e)
        {
            if (Login.Text.Length > 0 && Password.Password.Length > 0)
            {
                string login = Login.Text;
                string password = Password.Password;
                string role = GetUserRole(login, password);

                if (role == "user" || role == "admin" || role == "isp")
                {
                    Users user = TransferData(login);

                    if (role == "user")
                    {
                        mainWindow.OpenPages(new Pages.main(mainWindow, user));
                        MessageBox.Show("Успешный вход!");
                    }
                    else if (role == "admin")
                    {
                        mainWindow.OpenPages(new Pages.manager(mainWindow, user));
                        MessageBox.Show("Успешный вход!");
                    }
                    else if (role == "isp")
                    {
                        //mainWindow.OpenPages(new Pages.ispolnitel(mainWindow, user));

                        MessageBox.Show("Успешный вход!");

                    }
                    else
                    {
                        MessageBox.Show("Ошибка при получении данных пользователя.");
                    }
                }
                else
                {
                    MessageBox.Show("Пользователь не найден.");
                }
            }
            else
            {
                MessageBox.Show("Введите логин и пароль.");
            }
        }

        public Users TransferData(string login)
        {
            try
            {
                string query = "SELECT * FROM Users WHERE Username = @login";
                List<MySqlParameter> parameters = new List<MySqlParameter>
                {
            new MySqlParameter("@login", login)
                };

                using (MySqlDataReader reader = Connection.SqlConnection(query, parameters))
                {
                    Users user = null;

                    while (reader.Read())
                    {
                        user = new Users
                        {
                            Id = reader.GetInt32("Id"),
                            Username = reader.GetString("Username"),
                            Contact_Information = reader.GetString("Contact_Information"),
                            Full_Name = reader.GetString("Full_Name"),
                            Position = reader.GetString("Position")
                        };
                    }

                    return user; // Возвращаем объект user
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка данных: {ex.Message}");
                return null;
            }
        }

        private string GetUserRole(string login, string password)
        {
            try
            {
                string query = "SELECT Position FROM Users WHERE Username = @login AND Password = @password";
                List<MySqlParameter> parameters = new List<MySqlParameter>
                {
                    new MySqlParameter("@login", login),
                    new MySqlParameter("@password", password),
                };

                using (MySqlDataReader reader = Connection.SqlConnection(query, parameters))
                {
                    if (reader.Read())
                    {
                        return reader.GetString("Position");
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при проверке учетных данных: {ex.Message}");
                return null;
            }
        }
        private void showPasswordCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            string password = Password.Password;

            Password.Visibility = Visibility.Collapsed;
            pasVisible.Visibility = Visibility.Visible;
            pasVisible.Text = password;
        }

        private void showPasswordCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            string password = pasVisible.Text;
            Password.Visibility = Visibility.Visible;
            pasVisible.Visibility = Visibility.Collapsed;
            Password.Password = password;

        }
    }
}
