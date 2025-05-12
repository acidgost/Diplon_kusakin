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
    /// Логика взаимодействия для EddIsp.xaml
    /// </summary>
    public partial class EddIsp : Page
    {
        Users users;
        private Users currentUser;
        MainWindow mainWindow;
        public EddIsp(MainWindow _mainWindow, Users user)
        {
            InitializeComponent();
            mainWindow = _mainWindow;
            currentUser = user;
            fio1.Text = user.Full_Name;
            email.Text = user.Contact_Information;
            log.Text = user.Username;
            pas.Text = user.Password;
        }

        private void Edd_Click(object sender, RoutedEventArgs e)
        {
            System.Text.RegularExpressions.Regex regex = null;

            regex = new System.Text.RegularExpressions.Regex("^([а-яА-Яa-zA-Z0-9])*$");
            string pattern = @"^[А-Я][а-я]{1,20}\s[А-Я][а-я]{1,20}\s[А-Я][а-я]{1,20}$";
            try
            {
                if (log.Text.Length > 0 && pas.Text.Length > 0 && fio1.Text.Length > 0 && email.Text.Length > 0)
                {
                    if (!Regex.IsMatch(fio1.Text, pattern))
                    {
                        MessageBox.Show("Поле ФИО в неправильном формате.\nПример: Кузнецов Кирилл Константинович");
                        return;
                    }
                    string ema = email.Text;
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

                    if (!regex.IsMatch(pas.Text))
                    {
                        MessageBox.Show("Неверный пароль. Пароль не может содержать специальные символы.");
                        return;
                    }


                    string query = $"UPDATE Users SET Full_Name = @Full_Name, Contact_Information = @Contact_Information, Password = @Password , Username = @Username1 WHERE Username = @Username";
                    List<MySqlParameter> parameters = new List<MySqlParameter>
                        {
                            new MySqlParameter("@Full_Name", fio1.Text),
                            new MySqlParameter("@Contact_Information", email.Text),
                            new MySqlParameter("@Password", pas.Text),
                            new MySqlParameter("@Username", currentUser.Username),
                            new MySqlParameter("@Username1", log.Text)
                        };
                    Connection.SqlConnection(query, parameters);
                    MessageBox.Show("Данные успешно обновлены");
                    mainWindow.OpenPages(new Pages.Sotrudniki(mainWindow));
                }
                else
                {
                    MessageBox.Show("Заполните поля");
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при изменении данных: {ex.Message}");
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

        private void butback_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.OpenPages(new Pages.Sotrudniki(mainWindow));
        }
    }
}
