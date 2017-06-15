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
            i.SetData(info);
            List<string> vals = new List<string>();
            if (!info.IsNativeTown)
            {
                CurrencyConverter cc = new CurrencyConverter();
                foreach (var item in info.resItem.rooms)
                {
                    vals.Add(String.Format("{0:f2}rub", item.Sum(x => cc.getRub(x.total_amount).amount)));
                }
                cbox.ItemsSource = vals;
                cbox.SelectedIndex = info.SelectedRoomsnum;
            }
            else
            {
                cbox.Visibility = Visibility.Collapsed;
                HotelInfoViewer.Visibility = Visibility.Collapsed;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ParentPage.lb.SelectedIndex = -1;
            try
            {
                if (cbox.SelectedIndex != -1)
                    ParentPage.HotelCosts[numdouble] = Double.Parse(((string)cbox.SelectedValue).Replace("rub", ""));
            }
            catch { }
            CurrencyConverter cc = new CurrencyConverter();
            ParentPage.MakeCost();
            MainFrame.Navigate(
                ParentPage,
                null
                );
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            info.SelectedRoomsnum = ((ComboBox)sender).SelectedIndex;
            if(info.SelectedRooms!=null)
            {
                List<HotelInfo> hotelinfos = new List<HotelInfo>();
                foreach (var room in info.SelectedRooms)
                    hotelinfos.Add(new HotelInfo(room));
                HotelInfoViewer.ItemsSource = hotelinfos;
            }     
        }
    }
}
