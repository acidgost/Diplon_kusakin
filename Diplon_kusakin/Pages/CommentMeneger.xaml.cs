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
    /// Логика взаимодействия для CommentMeneger.xaml
    /// </summary>
    public partial class CommentMeneger : Page
    {
        Users user;
        public ObservableCollection<CommentsClass> comments;
        private Request selectedRequest;
        MainWindow mainWindow;
        public CommentMeneger(MainWindow _mainWindow, Request selectedRequest_)
        {
            InitializeComponent();
            mainWindow = _mainWindow;
            selectedRequest = selectedRequest_;
            LouadComment();
        }
        public void LouadComment()
        {
            try
            {
                comments = new ObservableCollection<CommentsClass>();

                List<MySqlParameter> parameters = new List<MySqlParameter>();

                string query = $"SELECT * FROM Comments WHERE IdRequest = {selectedRequest.Id}";
                using (MySqlDataReader reader = Connection.SqlConnection(query, parameters))
                {
                    while (reader.Read())
                    {
                        CommentsClass request = new CommentsClass
                        {
                            Id = reader.GetInt32("Id"),
                            Comment = reader.GetString("Comment"),
                            Assignee = reader.GetString("Assignee"),
                            IdRequest = reader.GetInt32("IdRequest")
                        };
                        comments.Add(request);
                    }
                }

                requestsListView.ItemsSource = comments;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке заявок пользователя: {ex.Message}");
            }
        }
        private void Exit(object sender, RoutedEventArgs e)
        {
            mainWindow.OpenPages(new Pages.manager(mainWindow, user));
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
