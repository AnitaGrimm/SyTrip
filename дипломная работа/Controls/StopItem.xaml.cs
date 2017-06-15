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
using дипломная_работа.Model;
using дипломная_работа.Resources;

namespace дипломная_работа.Controls
{
    /// <summary>
    /// Логика взаимодействия для StopItem.xaml
    /// </summary>
    public partial class StopItem : UserControl
    {
        public static readonly DependencyProperty StopProperty = DependencyProperty.Register("Stop", typeof(Stop),   typeof(StopItem));
        public Stop Stop
        {
            get { return (Stop)GetValue(StopProperty); }
            set { SetValue(StopProperty, value); }
        }
        
        public StopItem()
        {
            InitializeComponent();
        }
        #region events
        private void TownsBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (CommonData.NameOfTowns.IndexOf(tb.Text) == -1 && CommonData.NameOfTowns_rus.IndexOf(tb.Text) == -1)
                tb.Background = Brushes.LightCoral;
            else
            {
                tb.Background = Brushes.LightGreen;
                Stop.Town = CommonData.Countries.SelectMany(country => country.Towns).Where(town => town.Name == tb.Text || town.Name_rus == tb.Text)?.First();
            }
        }

        private void ToTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Text != "")
                Stop.Period.To = int.Parse(tb.Text);
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if(tb.Text!="")
                Stop.Period.From = int.Parse(tb.Text);
        }
        #endregion
    }
}
