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
using дипломная_работа.Resources;

namespace дипломная_работа
{
    /// <summary>
    /// Логика взаимодействия для InputPage.xaml
    /// </summary>
    public sealed partial class InputPage : Page
    {
        Frame MainFrame;
        WelcomePage WelcomePage;
        Querry Querry = new Querry();

        public InputPage(WelcomePage WelcomePage,Frame MainFrame)
        {
            InitializeComponent();
            this.MainFrame = MainFrame;
            this.WelcomePage = WelcomePage;
            Querry.Add(new Stop());
            DataContext = Querry;
            NativeTownTB.Text = CommonData.UserTown?.Name_rus ?? CommonData.UserTown?.Name ?? "";
            Querry.Budget = 100000;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(
                WelcomePage,
                null
                );
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Querry.Add(new Stop());
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (QuerryList.SelectedIndex != -1)
            {
                Querry.Remove((Stop)QuerryList.SelectedItem);
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            double i;
            if (tb.Text != "" && double.TryParse(tb.Text, out i))
                Querry.Budget = Double.Parse(tb.Text);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (date1.SelectedDate.HasValue && date2.SelectedDate.HasValue && IsProperPeople(Querry) && CombinationCount(Querry) <= 10 && Querry.Count<=8)
            {
                Querry.DateOfBigiinning1 = date1.SelectedDate.Value;
                Querry.DateOfBigiinning2 = date2.SelectedDate.Value;
                MainFrame.Navigate(
                    new ResultLoader(MainFrame, Querry, this),
                    null);
            }
        }
        private bool IsProperPeople(Querry querry)
        {
            if (querry.AdultsCount + querry.CeniorsCount == 0)
                return false;
            return true;
        }
        private int CombinationCount(Querry querry)
        {
            int count = querry.Count>0?1:0;
            for (int i = 1; i <= querry.Count; i++)
                count *= i*Math.Abs(querry[i-1].Period.From - querry[i-1].Period.To);
            return count*querry.DateOfBigiinning2.Subtract(querry.DateOfBigiinning1).Days;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (CommonData.NameOfTowns.IndexOf(tb.Text) == -1 && CommonData.NameOfTowns_rus.IndexOf(tb.Text) == -1)
                tb.Background = Brushes.LightCoral;
            else
            {
                tb.Background = Brushes.LightGreen;
                Querry.NativeTown = CommonData.Countries.SelectMany(country => country.Towns).Where(town => town.Name == tb.Text || town.Name_rus == tb.Text)?.First();
            }
        }

        private void DatePicker_LostFocus(object sender, RoutedEventArgs e)
        {
            DatePicker dp = (DatePicker)sender;
            if (dp.SelectedDate.HasValue && dp.SelectedDate.Value.Day >= DateTime.Now.Day && dp.SelectedDate.Value.Month >= DateTime.Now.Month && dp.SelectedDate.Value.Year >= DateTime.Now.Year)
                Querry.DateOfBigiinning1 = dp.SelectedDate.Value;
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            int i;
            if (tb.Text != null && int.TryParse(tb.Text, out i))
                Querry.ChildrenCount = int.Parse(tb.Text);
        }

        private void TextBox_TextChanged_2(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            int i;
            if (tb.Text != null && int.TryParse(tb.Text, out i))
                Querry.AdultsCount = int.Parse(tb.Text);
        }

        private void TextBox_TextChanged_3(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            int i;
            if (tb.Text != null && int.TryParse(tb.Text, out i))
                Querry.MaxDayCount = int.Parse(tb.Text);
        }

        private void TextBox_TextChanged_4(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            int i;
            if (tb.Text != null && int.TryParse(tb.Text, out i))
                Querry.InfantLapCount = int.Parse(tb.Text);
        }

        private void TextBox_TextChanged_5(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            int i;
            if (tb.Text != null && int.TryParse(tb.Text, out i))
                Querry.InfantSeatCount = int.Parse(tb.Text);
        }

        private void TextBox_TextChanged_6(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            int i;
            if (tb.Text != null && int.TryParse(tb.Text, out i))
                Querry.CeniorsCount = int.Parse(tb.Text);
        }

        private void TextBox_TextChanged_7(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            int i;
            if (tb.Text != null && int.TryParse(tb.Text, out i))
                Querry.RoomCount = int.Parse(tb.Text);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Querry.IsOneHotel = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Querry.IsOneHotel = false;
        }

        private void date2_LostFocus(object sender, RoutedEventArgs e)
        {
            DatePicker dp = (DatePicker)sender;
            if (dp.SelectedDate.HasValue && dp.SelectedDate.Value.Day >= DateTime.Now.Day && dp.SelectedDate.Value.Month >= DateTime.Now.Month && dp.SelectedDate.Value.Year >= DateTime.Now.Year)
                Querry.DateOfBigiinning2 = dp.SelectedDate.Value;
        }
    }
}
