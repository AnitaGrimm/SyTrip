using Google.Apis.QPXExpress.v1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using дипломная_работа.Helpers;
using дипломная_работа.Model;
using дипломная_работа.Resources;

namespace дипломная_работа.Controls
{
    /// <summary>
    /// Логика взаимодействия для FlightInfoMini.xaml
    /// </summary>
    public partial class FlightInfoMini : UserControl
    {
        public FlightInfoMini()
        {
            InitializeComponent();
        }
        public void SetData(SliceInfo slice)
        {
            var or = CommonData.Towns.Where(x => x.Code == slice.Segment[0].Leg[0].Origin).FirstOrDefault();
            Origin.Text = or==null?slice.Segment[0].Leg[0].Origin:(or.Name_rus==""? or.Name: or.Name_rus);
            var dest = CommonData.Towns.Where(x => x.Code == slice.Segment[0].Leg[0].Destination).FirstOrDefault();
            Destination.Text = dest == null ? slice.Segment[0].Leg[0].Destination : (dest.Name_rus == "" ? dest.Name : dest.Name_rus);
            var depdate = Computer.ParseDateTime(slice.Segment[0].Leg[0].DepartureTime);
            var ardate = Computer.ParseDateTime(slice.Segment[0].Leg[0].ArrivalTime);
            DepDate.Text = string.Format("{0}-{1}-{2} {3}:{4}", depdate.Day, depdate.Month, depdate.Year, depdate.Hour, depdate.Minute);
            ArDate.Text = string.Format("{0}-{1}-{2} {3}:{4}", ardate.Day, ardate.Month, ardate.Year, ardate.Hour, ardate.Minute);
            Carrier.Source = DataLoader.getLogoAirport(200, 200, slice.Segment[0].Flight.Carrier);
        }
        private double ParseRub(string saleTotal)
        {
            var rx = new Regex("[^0-9]*(?<val>[0-9]*(.[0-9])?)");
            var match = rx.Match(saleTotal);
            return double.Parse(match.Groups["val"].Value);
        }
    }
}
