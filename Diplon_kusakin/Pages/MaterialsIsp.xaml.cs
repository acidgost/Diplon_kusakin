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
    /// Логика взаимодействия для MaterialsIsp.xaml
    /// </summary>
    public partial class MaterialsIsp : Page
    {
        private Request selectedRequest;
        public int cost;
        public int totalCost;
        private Users currentUser; // Добавлено для хранения текущего пользователя

        MainWindow mainWindow;

        public MaterialsIsp(Request selectedRequest_, MainWindow mainWindow_, Users user)
        {
            this.mainWindow = mainWindow_;
            this.currentUser = user; // Инициализируем текущего пользователя
            InitializeComponent();
            selectedRequest = selectedRequest_;
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                ispolnitel ispolnitel = new ispolnitel(mainWindow, currentUser);
                mainWindow.OpenPages(ispolnitel);
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            string pattern = @"^[а-яА-Яa-zA-Z0-9\s\-]+$";
            try
            {
                if (txtEquipment.Text.Length > 0 && txtFaultType.Text.Length > 0 && txtcost.Text.Length > 0)
                {
                    string equipment = txtEquipment.Text;
                    string kol = txtFaultType.Text;
                    string cost = txtcost.Text;
                    if (!Regex.IsMatch(equipment, pattern))
                    {
                        MessageBox.Show("Ввод детали некорректен!");
                        return;
                    }
                    else if (!IsNumeric(kol))
                    {
                        MessageBox.Show("Ошибка: В поле количество введите только цифры!");
                        return;
                    }
                    else if (!IsNumeric(cost))
                    {
                        MessageBox.Show("Ошибка: В поле количество введите только цифры!");
                        return;
                    }


                    string query = $"INSERT INTO isp (Materials, Cost,Type,Help, IdClient) VALUES (@Materials, @Cost, 'Null', 'Null', @IdClient)";
                    List<MySqlParameter> parameters = new List<MySqlParameter>
                        {
                            new MySqlParameter("@Cost", cost),
                            new MySqlParameter("@Materials", equipment),
                            new MySqlParameter("@Idclient", selectedRequest.Id)
                        };

                    Connection.SqlConnection(query, parameters);

                    MessageBox.Show("Заказ успешно добавлен!");
                    ispolnitel ispolnitel = new ispolnitel(mainWindow, currentUser);
                    mainWindow.OpenPages(ispolnitel);
                }
                else
                {
                    MessageBox.Show("Заполните все поля");
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при заказе деталей: {ex.Message}");
            }
        }
        private bool IsNumeric(string text)
        {
            foreach (char c in text)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
