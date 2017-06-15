using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace дипломная_работа
{
    /// <summary>
    /// Логика взаимодействия для InitialPage.xaml
    /// </summary>
    public sealed partial class InitialPage : Page
    {
        Frame MainFrame;
        List<Country> Countries;
        MainWindow mw;
        public InitialPage(Frame MainFrame, MainWindow mw)
        {
            InitializeComponent();
            CultureInfo curCulture = Thread.CurrentThread.CurrentCulture;
            CultureInfo newCulture = new CultureInfo(curCulture.Name);
            newCulture.NumberFormat.NumberDecimalSeparator = ".";
            Thread.CurrentThread.CurrentCulture = newCulture;
            this.MainFrame = MainFrame;
            this.mw = mw;
            Loading();
        }
        void Loading()
        {
            Task t = new Task(LoadData);
            Task imageviewer = new Task(imageViewer);
            t.ContinueWith((antecedent) => { EndofLoading(); });
            t.Start();
            imageviewer.Start();
        }
        void imageViewer()
        {
            Random rnd = new Random();
            int i = rnd.Next(1, 24);
            bool prov = true;
            while (prov)
            {
                backgroundImage.Dispatcher.BeginInvoke(new Action(() => { backgroundImage.Source = new BitmapImage(new Uri("../Resources/Sights/" + i + ".jpg", UriKind.Relative)); }));
                i = i % 23 + 1;
                progressBar.Dispatcher.BeginInvoke(new Action(() => { prov = progressBar.Value != 100; }));
                Thread.Sleep(10000);
            }
        }
        void LoadData()
        {
            Countries = DataLoader.Load(ref progressBar);
        }
        void EndofLoading()
        {
            if (Countries == null)
            {
                var Result = MessageBox.Show("Ошибка! Нет соеденения с интернетом. Попробовать еще раз?","Ошибка загрузки приложения", MessageBoxButton.YesNo);
                if (Result == MessageBoxResult.Yes)
                    Loading();
                else
                    mw.Close();
            }
            else
                progressBar.Dispatcher.Invoke(new Action(() => { progressBar.Value = 100; }));
        }
        private void progressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var progBar = (ProgressBar)sender;
            if (progBar.Value == progBar.Maximum)
            {
                CommonData.Countries = new ObservableCollection<Country>(Countries);
                CommonData.Towns = CommonData.Countries?.SelectMany(country => country.Towns)?.ToList();
                CommonData.Airports = CommonData.Towns?.SelectMany(town => town.Airports)?.ToList();
                CommonData.NameOfTowns = CommonData.Towns.Where(town => town.Name != null)?.Select(town => town.Name)?.ToList() ?? new List<string>();
                CommonData.NameOfTowns_rus = CommonData.Towns.Where(town => town.Name_rus != null)?.Select(town => town.Name_rus)?.ToList() ?? new List<string>();
                CommonData.UserTown = WebTools.GetUserLocation();
                MainFrame.Navigate(
                    new WelcomePage(MainFrame),
                    null
                    );
            }
        }
    }
}
