﻿using System;
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
    /// Логика взаимодействия для ResultViewer.xaml
    /// </summary>
    public partial class ResultViewer : Page
    {
        public static readonly DependencyProperty InfoProp = DependencyProperty.Register("Info", typeof(Result), typeof(ResultViewer));
        public Result Info
        {
            get { return (Result)GetValue(InfoProp); }
            set { SetValue(InfoProp, value); }
        }
        Frame MainFrame;
        ResultsListViewer ParentPage;
        Result Result;
        List<ResultItemInfo> infos;
        public List<double> HotelCosts;
        double flightcost;
        Querry Querry;
        public ResultViewer(Frame MainFrame, ResultsListViewer ParentPage, Result Result, Querry Querry)
        {
            InitializeComponent();
            this.MainFrame = MainFrame;
            this.ParentPage = ParentPage;
            this.Result = Result;
            HotelCosts = new List<double>();
            this.Querry = Querry;
            infos = new List<ResultItemInfo>();
            flightcost = Result.FlightCostTotal;
            foreach (var item in Result.Route)
            {
                var info = new ResultItemInfo(item);
                infos.Add(info);
            }
            lb.ItemsSource = infos; foreach (var info in infos)
                if (info.resItem.ArrivalDate == new DateTime() || info.resItem.DepatureDate == new DateTime())
                    HotelCosts.Add(info.HotelCost);
                else HotelCosts.Add(0);
            tb.Text = "Даты поездки: " + Result.BeginDate.ToShortDateString() +" - "+ Result.EndDate.ToShortDateString() + Environment.NewLine
                +"Взрослых: " + Querry.AdultsCount+ " пенс:" + Querry.CeniorsCount + " детей(2-11):" + Querry.ChildrenCount + " дети(0-1): с местом - " + Querry.InfantSeatCount + ", на руках -" + Querry.InfantLapCount + Environment.NewLine +
                "Комнат: " + Querry.RoomCount + ", " + (Querry.IsOneHotel ? "поиск в одном отеле" : "поиск в разных отелях");
            MakeCost();
        }
        string GetInfo(Result res)
        {
            string info = "";
            var nl = Environment.NewLine;
            foreach (var item in res.Route)
            {
                info += item.ToString()+nl;
            }
            return info;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ParentPage.LBox.SelectedIndex = -1;
            MainFrame.Navigate(
                ParentPage,
                null
                );
        }

        private void lb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems == null || e.AddedItems.Count == 0 )
                return;
            ResultItemInfo res = (ResultItemInfo)e.AddedItems[0];
            if (res.IsNativeTown)
                return;
            MainFrame.Navigate(
                new ResultItemViewer(MainFrame, this, res, Result, lb.SelectedIndex),
                null
                );
        }

        public void MakeCost()
        {
            if (HotelCosts.IndexOf(-1) != -1)
                return;
            cost.Text = String.Format("{0:f2}", Result.GetCost());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Importer im = new Importer();
            im.MakeReport(Result, Querry, infos);
        }
    }
}
