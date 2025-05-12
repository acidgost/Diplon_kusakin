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
    /// Логика взаимодействия для CommentsAdd.xaml
    /// </summary>
    public partial class CommentsAdd : Page
    {
        MainWindow mainWindow;
        private Request selectedRequest;
        private Users currentUser;
        public ObservableCollection<CommentsClass> comments;
        private ObservableCollection<CommentsClass> originalComments;

        public CommentsAdd(Request selectedRequest_, MainWindow mainWindow_, Users user)
        {
            mainWindow = mainWindow_;
            selectedRequest = selectedRequest_;
            currentUser = user;
            InitializeComponent();
            LoadUserRequests();
        }

        public void LoadUserRequests()
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
                MessageBox.Show($"Ошибка при загрузке комменатрия: {ex.Message}");
            }
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                ispolnitel ispolnitelPage = new ispolnitel(mainWindow, currentUser);
                mainWindow.OpenPages(ispolnitelPage);
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            Comments commentsWindow = new Comments(selectedRequest, mainWindow, currentUser);
            mainWindow.OpenPages(commentsWindow);
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchTerm = searchTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                requestsListView.ItemsSource = originalComments;
                MessageBox.Show("Введите номер или параметры для поиска.", "Поиск", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            SearchComments(searchTerm);
        }

        private void SearchComments(string searchTerm)
        {
            var searchResults = originalComments.Where(comment =>
                comment.Id.ToString().Contains(searchTerm) ||
                comment.Comment.ToString().Contains(searchTerm)).ToList();
            if (searchResults.Count == 0)
            {
                requestsListView.ItemsSource = originalComments;
                MessageBox.Show("Ничего не найдено. Показаны все записи.", "Поиск", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                requestsListView.ItemsSource = searchResults;
            }
        }
    }
}
