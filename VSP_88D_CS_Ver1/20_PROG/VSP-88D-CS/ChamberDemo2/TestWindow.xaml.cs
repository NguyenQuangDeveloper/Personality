using ChamberDemo2.ViewModels;
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
using System.Windows.Shapes;

namespace ChamberDemo2
{
    /// <summary>
    /// Interaction logic for TestWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        public TestWindow(TestViewModel testViewModel)
        {
            InitializeComponent();
            this.DataContext = testViewModel;
            txtShowLog.TextChanged += (object sender, TextChangedEventArgs e) => { txtShowLog.ScrollToEnd(); };
        }
    }
}



