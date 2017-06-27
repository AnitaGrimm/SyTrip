using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using дипломная_работа.Model;

namespace дипломная_работа
{
    /// <summary>
    /// Логика взаимодействия для ResultsListViewer.xaml
    /// </summary>
    public partial class ResultsListViewer : Page
    {
        Frame MainFrame;
        List<Result> Results;
        InputPage InputPage;
        List<ResultControl> rc;
        List<ResultControl> current;
        Querry Querry;
        public ResultsListViewer(Frame MainFrame, List<Result> Results, InputPage InputPage, Querry Querry)
        {
            if (Results != null)
            {
                current = rc = new List<ResultControl>();
                foreach (var result in Results)
                    rc.Add(new ResultControl(result));
            }
                InitializeComponent();
            this.MainFrame = MainFrame;
            this.Results = Results;
            this.InputPage = InputPage;
            this.Querry = Querry;
            Result OptDays, OptCost, OptDayCost;
            double MaxDays=-1;
            double MinVal=-1, MinDayCost=-1;
            if (Results == null)
                return;
            try
            {
                MaxDays = Results?.Max(x => x.EndDate.Subtract(x.BeginDate).TotalDays) ?? 0;
                MinVal = Results?.Min(x => x.GetCost()) ?? 0;
                MinDayCost = Results?.Min(x => x.GetCost() / x.EndDate.Subtract(x.BeginDate).TotalDays) ?? 0;
            }
            catch
            {

            }
            foreach (var item in Results)
            {
                string s = item.ToString();
                var days = item.EndDate.Subtract(item.BeginDate).TotalDays;
                var cost = item.GetCost();
            }
            if (Results != null)
            {
                //rc = new ObservableCollection<ResultControl>();
                //foreach (var result in Results)
                //    rc.Add(new ResultControl(result));
                LBox.ItemsSource = rc;
            }
                tb.Text = "Бюджет: " + Querry.Budget + "   Максимальное кол-во дней: " + Querry.MaxDayCount + "     "+
                "Дата начала: " + Querry.DateOfBigiinning1.ToShortDateString() + " - " + Querry.DateOfBigiinning1.ToShortDateString() + "   Город начала: " + (Querry.NativeTown.Name_rus != "" ? Querry.NativeTown.Name_rus : Querry.NativeTown.Name) + Environment.NewLine +
                "Взрослых: " + Querry.AdultsCount + " пенс:" + Querry.CeniorsCount + " детей(2-11):" + Querry.ChildrenCount + " дети(0-1): с местом - " + Querry.InfantSeatCount + ", на руках -" + Querry.InfantLapCount + Environment.NewLine +
                "Комнат: " + Querry.RoomCount + ", " + (Querry.IsOneHotel ? "один отель" : "разные отели");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(
                InputPage,
                null
                );
        }

        private void ListBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            
        }

        private void LBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems == null || e.AddedItems.Count == 0)
                return;
            Result res;
            try
            {
                res = ((ResultControl)((ListBox)sender).SelectedItem).Result;
            }
            catch { return; }
            MainFrame.Navigate(
                new ResultViewer(MainFrame, this, res, Querry),
                null
                );
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var arr = current.ToArray();
            Array.Sort(arr.Select(x=>x.Result.GetCost()).ToArray(), arr);
            LBox.ItemsSource = arr;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var arr = current.ToArray();
            Array.Sort(arr.Select(x => x.Result.GetCost()).ToArray(), arr);
            LBox.ItemsSource = arr.Reverse();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var arr = current.ToArray();
            Array.Sort(arr.Select(x => x.Result.FlightCostTotal).ToArray(), arr);
            LBox.ItemsSource = arr;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            var arr = current.ToArray();
            Array.Sort(arr.Select(x => x.Result.FlightCostTotal).ToArray(), arr);
            LBox.ItemsSource = arr.Reverse();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            var arr = current.ToArray();
            Array.Sort(arr.Select(x => x.Result.HotelCost).ToArray(), arr);
            LBox.ItemsSource = arr;
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            var arr = current.ToArray();
            Array.Sort(arr.Select(x => x.Result.HotelCost).ToArray(), arr);
            LBox.ItemsSource = arr.Reverse();
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            var arr = current.ToArray();
            var vals = new List<double>();
            foreach (var res in arr)
            {
                try
                {
                    vals.Add(res.Result.Route.Average(ri => ri.rooms.Average(r => r.hotel.rating)));
                }
                catch
                {
                    vals.Add(0);
                }
            }
            Array.Sort(vals.ToArray(), arr);
            LBox.ItemsSource = arr;
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            var arr = current.ToArray();
            var vals = new List<double>();
            foreach(var res in arr)
            {
                try
                {
                    vals.Add(res.Result.Route.Average(ri => ri.rooms.Average(r => r.hotel.rating)));
                }
                catch
                {
                    vals.Add(0);
                }
            }
            Array.Sort(vals.ToArray(), arr);
            LBox.ItemsSource = arr.Reverse();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                var list = rc.ToList();
                var res1 = list.Where(rc => Ifrating(rc,r1.IsChecked,r2.IsChecked, r3.IsChecked, r4.IsChecked, r5.IsChecked)).ToList();
                var res3 = list.Where(rc => Ifdatebeg(rc, ismodep.IsChecked, isdadep.IsChecked, isevdep.IsChecked, isnidep.IsChecked)).ToList();
                var res4 = list.Where(rc => Ifdatend(rc, ismoar.IsChecked, isdaar.IsChecked, isavar.IsChecked, isniar.IsChecked)).ToList();
                LBox.ItemsSource = current = res1.Where(x => res3.IndexOf(x) != -1 && res4.IndexOf(x) != -1).ToList();
            }
            catch
            {

            }
        }
        bool Ifrating( ResultControl rc, bool? r1, bool? r2, bool? r3, bool? r4, bool? r5)
        {
            bool res = false;
            if (r1.HasValue && r1.Value)
                res |= Math.Ceiling(rc.Result.AverageHotelRating) <= 1;
            if (r2.HasValue && r2.Value)
                res |= Math.Ceiling(rc.Result.AverageHotelRating) == 2;
            if (r3.HasValue && r3.Value)
                res |= Math.Ceiling(rc.Result.AverageHotelRating) == 3;
            if (r4.HasValue && r4.Value)
                res |= Math.Ceiling(rc.Result.AverageHotelRating) == 4;
            if (r5.HasValue && r5.Value)
                res |= Math.Ceiling(rc.Result.AverageHotelRating) == 5;
            return res; 
        }
        bool Ifdatebeg(ResultControl rc, bool? ismodep, bool? isdadep, bool? isevdep, bool? isnidep)
        {
            bool res = false;
            if (ismodep.HasValue && ismodep.Value)
                res |= rc.Result.BeginDate.Hour >= 6 && rc.Result.BeginDate.Hour < 12;
            if (isdadep.HasValue && isdadep.Value)
                res |= rc.Result.BeginDate.Hour >= 12 && rc.Result.BeginDate.Hour < 18;
            if (isevdep.HasValue && isevdep.Value)
                res |= rc.Result.BeginDate.Hour >= 18 && rc.Result.BeginDate.Hour < 24;
            if (isnidep.HasValue && isnidep.Value)
                res |= rc.Result.BeginDate.Hour >= 0 && rc.Result.BeginDate.Hour < 6;
            return res;
        }
        bool Ifdatend(ResultControl rc, bool? ismoar, bool? isdaar, bool? isevar, bool? isniar)
        {
            bool res = false;
            if (ismoar.HasValue && ismoar.Value)
                res |= rc.Result.EndDate.Hour >= 6 && rc.Result.EndDate.Hour < 12;
            if (isdaar.HasValue && isdaar.Value)
                res |= rc.Result.EndDate.Hour >= 12 && rc.Result.EndDate.Hour < 18;
            if (isevar.HasValue && isevar.Value)
                res |= rc.Result.EndDate.Hour >= 18 && rc.Result.EndDate.Hour < 24;
            if (isniar.HasValue && isniar.Value)
                res |= rc.Result.EndDate.Hour >= 0 && rc.Result.EndDate.Hour < 6;
            return res;
        }
    }
}
