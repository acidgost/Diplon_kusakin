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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;

namespace Diplon_kusakin.Pages
{
    /// <summary>
    /// Логика взаимодействия для ReadyIsp.xaml
    /// </summary>
    public partial class ReadyIsp : Page
    {
        private Request selectedRequest;
        private Users currentUser; // Добавлено для хранения текущего пользователя
        MainWindow mainWindow;
        public ReadyIsp(Request selectedRequest_, MainWindow mainWindow_, Users user)
        {
            this.mainWindow = mainWindow_;
            this.currentUser = user; // Инициализируем текущего пользователя
            InitializeComponent();
            selectedRequest = selectedRequest_;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                ispolnitel ispolnitel = new ispolnitel(mainWindow, currentUser); // Передаем currentUser
                mainWindow.OpenPages(ispolnitel);
            }
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation fadeIn = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.5)));
            MainContainer.BeginAnimation(OpacityProperty, fadeIn);
        }


        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                string pattern = @"^[а-яА-Яa-zA-Z0-9\s\-]+$";

                if (txtEquipment.Text.Length > 0 && txtHelp.Text.Length > 0)
                {


                    if (!Regex.IsMatch(txtEquipment.Text, pattern))
                    {
                        MessageBox.Show("Поле причины неисправности некорректен!");
                        return;
                    }
                    if (!Regex.IsMatch(txtHelp.Text, pattern))
                    {
                        MessageBox.Show("Поле оказание помощи некорректен!");
                        return;
                    }

                    string type = txtEquipment.Text;
                    string help = txtHelp.Text;

                    string query = "UPDATE isp SET Type = @Type, Help = @Help WHERE IdClient = @IdClient";

                    List<MySqlParameter> parameters = new List<MySqlParameter>
                        {

                            new MySqlParameter("@Type", type ),
                            new MySqlParameter("@Help", help),


                            new MySqlParameter("@Idclient", selectedRequest.Id)
                        };
                    Connection.SqlConnection(query, parameters);
                    string query1 = "UPDATE Requests SET Status = 'готово' WHERE Id = @IdClient";

                    List<MySqlParameter> parameters1 = new List<MySqlParameter>
                    {

                        new MySqlParameter("@Idclient", selectedRequest.Id)
                    };

                    Connection.SqlConnection(query1, parameters1);
                    MessageBox.Show("Заказ успешно отправлен!");
                    ispolnitel ispolnitel = new ispolnitel(mainWindow, currentUser);
                    mainWindow.OpenPages(ispolnitel);
                }
                else
                {
                    MessageBox.Show("Заполните все поля!");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при заказе деталей: {ex.Message}");
            }
        }
    }
}
