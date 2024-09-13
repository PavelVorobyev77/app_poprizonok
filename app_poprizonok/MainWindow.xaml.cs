using app_poprizonok.Entity;
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

namespace app_poprizonok
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            frame.Content = new Page1(frame);
        }

        private void Frame_OnLoadCompleted(object sender, NavigationEventArgs e)
        {
            try
            {
                Page1 pg = (Page1)e.Content;
                pg.Load();
            }
            catch { };

        }
    }

    public class helper
    {
        public static bool flag = false;
        public static int prioritet = 0;
        public static popit_Entities ent;
        public static popit_Entities GetContext()
        {
            if (ent == null)
            {
                ent = new popit_Entities();
            }
            return ent;
        }
    }
}
