using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using дипломная_работа.Controls;
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
            List<FlightInfoMini> fl = new List<FlightInfoMini>();
            foreach (var segment in info.resItem.ArrivalInfo.Segment)
                try
                {
                    FlightInfoMini fim = new FlightInfoMini();
                    fim.SetData(segment);
                    fl.Add(fim);
                }
                catch { }
            foreach (var segment in info.resItem.DepartureInfo.Segment)
                try
                {
                    FlightInfoMini fim = new FlightInfoMini();
                    fim.SetData(segment);
                    fl.Add(fim);
                }
                catch { }
            lb.ItemsSource = fl;
            List<HotelInfo> hotelinfos = new List<HotelInfo>();
            foreach (var room in info.Rooms)
                hotelinfos.Add(new HotelInfo(room));
            HotelInfoViewer.ItemsSource = hotelinfos;
            var hot = info.Rooms.FirstOrDefault().hotel;
            wb.Address = @"http://maps.google.com/?q=" + hot.address.city + "+" + hot.address.line1 +"+" + hot.property_name;
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
                    wb.Address=@"http://maps.google.com/?q=" + hot.address.city + "+" + hot.address.line1 +"+"+ hot.property_name;
                }
                catch
                {

                }
            }
        }
    }
}
