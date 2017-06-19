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
            try
            {
                i1.SetData(res.currenttrip.Slice[numdouble-1]);
                i2.SetData(res.currenttrip.Slice[numdouble]);
            }
            catch
            {

            }
            List<HotelInfo> hotelinfos = new List<HotelInfo>();
            foreach (var room in info.Rooms)
                hotelinfos.Add(new HotelInfo(room));
            HotelInfoViewer.ItemsSource = hotelinfos;
            var hot = info.Rooms.FirstOrDefault().hotel;
            wb.Source = new Uri(@"http://maps.google.com/?q=" + hot.address.city + "+" + hot.address.line1 + hot.property_name,UriKind.Absolute);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ParentPage.lb.SelectedIndex = -1;
            MainFrame.Navigate(
                ParentPage,
                null
                );
        }

        private void HotelInfoViewer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var hi = (HotelInfo)(((ListBox)sender).SelectedItem);
            if (hi != null)
            {
                try
                {
                    var hot = hi.Room.hotel;
                    wb.Source = new Uri(@"http://maps.google.com/?q=" + hot.address.city + "+" + hot.address.line1 + hot.property_name, UriKind.Absolute);
                }
                catch
                {

                }
            }
        }
    }
}
