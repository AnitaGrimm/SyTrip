using System;
using System.Collections.Generic;
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

namespace дипломная_работа
{
    /// <summary>
    /// Логика взаимодействия для ResultLoader.xaml
    /// </summary>
    public partial class ResultLoader: Page
    {
        Frame MainFrame;
        List<Result> Results;
        Querry Querry;
        InputPage InputPage;
        public ResultLoader(Frame MainFrame, Querry Querry, InputPage InputPage)
        {
            InitializeComponent();
            this.MainFrame = MainFrame;
            this.Querry = Querry;
            this.InputPage = InputPage;
            Loading();
        }
        void Loading()
        {
            Task t = new Task(new Action(LoadData));
            Task imageviewer = new Task(imageViewer);
            t.ContinueWith((antecedent) => { EndofLoading(); });
            t.Start();
            imageviewer.Start();
        }
        void LoadData()
        {
            Computer cp = new Computer();
            Results = cp.DoCompute(Querry, progressBar);
            progressBar.Dispatcher.Invoke(new Action(() => { progressBar.Value = 100; }));
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
        void EndofLoading()
        {
            if (Results == null)
            {
                var Result = MessageBox.Show("Ошибка! Проверьте входные данные или связь с интернетом. Попробовать снова?", "Ошибка расчета", MessageBoxButton.YesNo);
                if (Result == MessageBoxResult.Yes)
                    Loading();
                else
                    MainFrame.Dispatcher.Invoke(() => MainFrame.Navigate(
                        InputPage,
                        null));
            }
            else
                progressBar.Dispatcher.Invoke(new Action(() => { progressBar.Value = 100; }));
        }
        private void progressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var progBar = (ProgressBar)sender;
            if (progBar.Value == progBar.Maximum)
            {
                MainFrame.Navigate(
                    new ResultsListViewer(MainFrame, Results, InputPage, Querry),
                    null
                    );
            }
        }
    }
}
