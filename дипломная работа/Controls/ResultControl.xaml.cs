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
using дипломная_работа.Model;

namespace дипломная_работа.Controls
{
    /// <summary>
    /// Логика взаимодействия для ResultControl.xaml
    /// </summary>
    public partial class ResultControl : UserControl
    {
        public Result Result { get; set; }
        public ResultControl(Result Result)
        {
            InitializeComponent();
            this.Result = Result;
            StartTownName.Text = Result.Route[0].Town.Name_rus == "" ? Result.Route[0].Town.Name : Result.Route[0].Town.Name_rus;
            StartDate.Text = String.Format("{0}-{1}-{2} {3}:{4}", (Result.BeginDate.Day<10?"0":"")+ Result.BeginDate.Day, (Result.BeginDate.Month < 10 ? "0" : "")+ Result.BeginDate.Month, Result.BeginDate.Year, (Result.BeginDate.Hour < 10 ? "0" : "") +Result.BeginDate.Hour, (Result.BeginDate.Minute < 10 ? "0" : "") +Result.BeginDate.Minute);
            TotalPrice.Text = "итого: " + String.Format("{0:f2}руб", Result.GetCost());
            FlightPrice.Text = String.Format("{0:f2}руб", Result.FlightCostTotal);
            HotelPrice.Text = String.Format("{0:f2}руб", Result.HotelCost);
            List<HotelsInTown> Hotels = new List<HotelsInTown>();
            List<TownInfo> Towns = new List<TownInfo>();
            List<FlightInfoMini> Flights = new List<FlightInfoMini>();
            for(int i=0; i<Result.Route.Count; i++)
            {
                var nd = new DateTime();
                if (Result.Route[i].ArrivalDate == nd || Result.Route[i].DepatureDate == nd)
                    continue;
                Hotels.Add(new HotelsInTown(Result.Route[i]));
                Towns.Add(new TownInfo(Result.Route[i], i));
            }
            foreach(var slice in Result.currenttrip.Slice)
            {
                foreach (var segment in slice.Segment)
                {
                    var finfo = new FlightInfoMini();
                    finfo.SetData(segment);
                    Flights.Add(finfo);
                }
            }
            FlightInfoList.ItemsSource = Flights;
            TownInfoList.ItemsSource = Towns;
            HotelInfoList.ItemsSource = Hotels;
        }
    }
}
