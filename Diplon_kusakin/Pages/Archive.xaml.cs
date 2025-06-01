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
using System.Collections.ObjectModel;
using System.Windows.Shapes;

namespace Diplon_kusakin.Pages
{
    


    /// <summary>
    /// Логика взаимодействия для Archive.xaml
    /// </summary>
    public partial class Archive : Page
    {
        public Archive(MainWindow mainWindow, Users currentUser)
        {
            InitializeComponent();
            LoadArchive();
        }
        private void LoadArchive()
        {
            manager managerPage = new manager(null, null);
            var archivedRequests = managerPage.GetArchivedRequests();
            archiveListView.ItemsSource = archivedRequests;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.OpenPages(new Pages.Archive(mainWindow, currentUser))
        }
    }
}
