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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Diplon_kusakin
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Users user;
        public MainWindow()
        {

            InitializeComponent();
            OpenPages(new Pages.login(this, user));
        }
        public void OpenPages(Page _pages)
        {
            DoubleAnimation opgrid = new DoubleAnimation();
            opgrid.From = 1;
            opgrid.To = 0;
            opgrid.Duration = TimeSpan.FromSeconds(0.1);
            opgrid.Completed += delegate
            {
                frame.Navigate(_pages);
                DoubleAnimation opgrid2 = new DoubleAnimation();
                opgrid2.From = 0;
                opgrid2.To = 1;
                opgrid2.Duration = TimeSpan.FromSeconds(0.1);
                frame.BeginAnimation(Frame.OpacityProperty, opgrid2);
            };
            frame.BeginAnimation(Frame.OpacityProperty, opgrid);
        }
    }
}
