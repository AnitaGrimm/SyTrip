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
using дипломная_работа.Controls;
using дипломная_работа.Helpers;
using дипломная_работа.Model;

namespace дипломная_работа
{
    /// <summary>
    /// Логика взаимодействия для ResultItemViewer.xaml
    /// </summary>
    public partial class ResultItemViewer : Page
    {
        Frame MainFrame;
        ResultViewer ParentPage;
        ResultItemInfo info;
        int numdouble;
        public ResultItemViewer(Frame MainFrame, ResultViewer ParentPage, ResultItemInfo info, Result res, int numdouble)
        {
            InitializeComponent();
            this.MainFrame = MainFrame;
            this.ParentPage = ParentPage;
            this.info = info;
            this.numdouble = numdouble;
            List<HotelInfo> hotelinfos = new List<HotelInfo>();
            foreach (var room in info.Rooms)
                hotelinfos.Add(new HotelInfo(room));
            HotelInfoViewer.ItemsSource = hotelinfos;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(
                ParentPage,
                null
                );
        }
    }
}
