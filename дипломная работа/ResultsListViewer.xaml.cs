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
        Querry Querry;
        public ResultsListViewer(Frame MainFrame, List<Result> Results, InputPage InputPage, Querry Querry)
        {
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
                item.IsMinDays = days == MaxDays;
                item.IsMinCost = cost == MinVal;
                item.IsMinDaysCost = cost / days == MinDayCost;
            }
            if (Results != null)
            {
                List<ResultControl> r1 = new List<ResultControl>();
                foreach (var result in Results)
                    r1.Add(new ResultControl(result));
                LBox.ItemsSource = r1;
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
            Result res = ((ResultControl)((ListBox)sender).SelectedItem).Result;
            MainFrame.Navigate(
                new ResultViewer(MainFrame, this, res, Querry),
                null
                );
        }
    }
}
