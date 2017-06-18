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
    /// Логика взаимодействия для HotelsInTown.xaml
    /// </summary>
    public partial class HotelsInTown : UserControl
    {
        public HotelsInTown(ResultItem resitem)
        {
            InitializeComponent();
            TownName.Text = resitem.Town.Name_rus == "" ? resitem.Town.Name : resitem.Town.Name_rus;
            List<HotelInfoMini> him = new List<HotelInfoMini>();
            foreach (var room in resitem.rooms)
                him.Add(new HotelInfoMini(room));
            HotelInfoMiniList.ItemsSource = him;
        }
    }
}
