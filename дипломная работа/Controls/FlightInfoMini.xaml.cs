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
        public void SetData(SegmentInfo Segment)
        {
            var or = CommonData.Airports.Where(x => x.Code == Segment.Leg[0].Origin).FirstOrDefault();
            Origin.Text = or==null?Segment.Leg[0].Origin:(or.Name_rus==""? or.Name: or.Name_rus);
            var dest = CommonData.Airports.Where(x => x.Code == Segment.Leg[0].Destination).FirstOrDefault();
            Destination.Text = dest == null ? Segment.Leg[0].Destination : (dest.Name_rus == "" ? dest.Name : dest.Name_rus);
            var depdate = Computer.ParseDateTime(Segment.Leg[0].DepartureTime);
            var ardate = Computer.ParseDateTime(Segment.Leg[0].ArrivalTime);
            DepDate.Text = string.Format("{0}-{1}-{2} {3}:{4}", (depdate.Day < 10 ? "0" : "") + depdate.Day, (depdate.Month < 10 ? "0" : "") + depdate.Month, depdate.Year, (depdate.Hour < 10 ? "0" : "") + depdate.Hour, (depdate.Minute < 10 ? "0" : "") + depdate.Minute);
            ArDate.Text = string.Format("{0}-{1}-{2} {3}:{4}", (ardate.Day < 10 ? "0" : "") + ardate.Day, (ardate.Month < 10 ? "0" : "") + ardate.Month, ardate.Year, (ardate.Hour < 10 ? "0" : "") + ardate.Hour, (ardate.Minute < 10 ? "0" : "") + ardate.Minute);
            Carrier.Source = DataLoader.getLogoAirport(200, 200, Segment.Flight.Carrier);
            Cabin.Text = TranslateCabin(Segment.Cabin);
        }

        private string TranslateCabin(string cabin)
        {
            switch(cabin)
            {
                case "COACH":return "эконом";
                case "PREMIUM_COACH": return "премиум эконом";
                case "BUSINESS": return "бизнес";
                case "FIRST": return "первый";
                default: return "";
            }
        }

        private double ParseRub(string saleTotal)
        {
            Tools.SetNumberDecimalSeparator();
            var rx = new Regex("[^0-9]*(?<val>[0-9]*(.[0-9])?)");
            var match = rx.Match(saleTotal);
            return double.Parse(match.Groups["val"].Value);
        }
    }
}
