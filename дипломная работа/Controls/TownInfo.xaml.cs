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
    /// Логика взаимодействия для TownInfo.xaml
    /// </summary>
    public partial class TownInfo : UserControl
    {
        public TownInfo(ResultItem result, int townnum)
        {
            InitializeComponent();
            Num.Text = townnum.ToString();
            Name.Text = string.Format("{0}({1:f2})", result.Town.Name_rus==""? result.Town.Name: result.Town.Name_rus, result.DepatureDate.Subtract(result.ArrivalDate).TotalDays);
            var ar = result.ArrivalDate;
            var dep = result.DepatureDate;
            DateArr.Text = string.Format("{0}-{1}-{2} {3}:{4}", ar.Day, ar.Month, ar.Year, ar.Hour, ar.Minute);
            DateDep.Text = string.Format("{0}-{1}-{2} {3}:{4}", dep.Day, dep.Month, dep.Year, dep.Hour, dep.Minute);
        }
    }
}
