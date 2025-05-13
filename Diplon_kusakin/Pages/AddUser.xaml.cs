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
    /// Логика взаимодействия для AddUser.xaml
    /// </summary>
    public partial class AddUser : Page
    {
        MainWindow mainWindow;
        Users users;
        public AddUser(MainWindow _mainWindow)
        {
            InitializeComponent();
            mainWindow = _mainWindow;
        }
        private void butback_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.OpenPages(new Pages.User(mainWindow, users));
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            System.Text.RegularExpressions.Regex regex = null;

            regex = new System.Text.RegularExpressions.Regex("^([а-яА-Яa-zA-Z0-9])*$");
            string pattern = @"^[А-Я][а-я]{1,20}\s[А-Я][а-я]{1,20}\s[А-Я][а-я]{1,20}$";
            string ema = email.Text;
            try
            {
                if (log.Text.Length > 0 && pas.Password.Length > 0 && pas2.Password.Length > 0 && fio1.Text.Length > 0 && email.Text.Length > 0)
                {
                    if (!Regex.IsMatch(fio1.Text, pattern))
                    {
                        MessageBox.Show("Поле ФИО в неправильном формате.\nПример: Кусакин Данил Игоревич");
                        return;
                    }
                    if (!Regex.IsMatch(ema, @"\b[A-Za-z0-9.%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b"))
                    {
                        MessageBox.Show("Email не является допустимым. Пожалуйста, введите действительный адрес электронной почты.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (!regex.IsMatch(log.Text))
                    {
                        MessageBox.Show("Неверный логин. Логин не может содержать специальные символы.");
                        return;
                    }

                    if (!regex.IsMatch(pas.Password))
                    {
                        MessageBox.Show("Неверный пароль. Пароль не может содержать специальные символы.");
                        return;
                    }
                    if (pas.Password.Length > 8)
                    {
                        MessageBox.Show("Пароль не может быть больше 8 символов.");
                        return;
                    }

                    if (pas2.Password == pas.Password)
                    {
                        string login = log.Text;
                        string password = pas.Password;
                        string mail = email.Text;
                        string fio = fio1.Text;

                        Users newUser = new Users
                        {
                            Full_Name = fio,
                            Position = "user",
                            Contact_Information = mail,
                            Username = login,
                            Password = password
                        };

                        if (!CheckIfUserExists(login))
                        {
                            if (RegisterUser(newUser))
                            {
                                TransferData(login);
                                mainWindow.OpenPages(new Pages.User(mainWindow, users));
                                MessageBox.Show("Успешная регистрация!");
                            }
                            else
                            {
                                MessageBox.Show("Ошибка при регистрации.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Пользователь с таким логином уже существует.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Пароли не совпадают.");
                    }
                }
                else
                {
                    MessageBox.Show("Заполните все поля.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении данных: {ex.Message}");
            }
        }
        private bool IsValidFIO(string input)
        {
            string[] words = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length != 3)
                return false;

            foreach (var word in words)
            {
                if (!Regex.IsMatch(word, @"^[А-ЯЁ][а-яё]*$"))
                    return false;
            }

            return true;
        }
        public void TransferData(string login)
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
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка данных: {ex.Message}");
            }
        }
        private bool CheckIfUserExists(string login)
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
                    return reader.HasRows;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при проверке пользователя: {ex.Message}");
                return false;
            }
        }
        private void showPasswordCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            string password = pas.Password;

            pas.Visibility = Visibility.Collapsed;
            pasVisible.Visibility = Visibility.Visible;
            pasVisible.Text = password;
        }
        private void showPasswordCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            string password = pas.Password;
            pas.Visibility = Visibility.Visible;
            pasVisible.Visibility = Visibility.Collapsed;
            pasVisible.Text = password;

        }
        private void Open_Checked(object sender, RoutedEventArgs e)
        {
            string password = pas2.Password;

            pas2.Visibility = Visibility.Collapsed;
            pas2Visible.Visibility = Visibility.Visible;
            pas2Visible.Text = password;
        }
        private void Close_Unchecked(object sender, RoutedEventArgs e)
        {
            string password = pas2Visible.Text;
            pas2.Visibility = Visibility.Visible;
            pas2Visible.Visibility = Visibility.Collapsed;
            pas2.Password = password;

        }
        private bool RegisterUser(Users user)
        {
            try
            {
                string query = "INSERT INTO Users (Username, Password, Contact_Information, Full_Name, Position) VALUES (@Username, @Password, @Contact_Information, @Full_Name, @Position)";
                List<MySqlParameter> parameters = new List<MySqlParameter>
                {
                    new MySqlParameter("@Username", user.Username),
                    new MySqlParameter("@Password", user.Password),
                    new MySqlParameter("@Contact_Information", user.Contact_Information),
                    new MySqlParameter("@Full_Name", user.Full_Name),
                    new MySqlParameter("@Position", user.Position)
                };


                Connection.SqlConnection(query, parameters);
                return true; // Возвращаем true в случае успешного добавления пользователя
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при регистрации пользователя: {ex.Message}");
                return false; // Возвращаем false в случае ошибки
            }
        }
    }
}
