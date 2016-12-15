using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace BrowserComparsion
{
    /// <summary>
    /// Interaction logic for PerformanceWindow.xaml
    /// </summary>
    public partial class PerformanceWindow : Window
    {
        private PerformanceCounter Counter { get; set; }
        private List<PerformanceEntry> Entries { get; set; }

        public PerformanceWindow(List<IBrowserController> controllers)
        {
            InitializeComponent();
            Counter = new PerformanceCounter(controllers);
            Counter.DataUpdated += UpdateView;
            LoadEntries();
            UpdateView();
        }

        private void UpdateView()
        {
            Dispatcher.Invoke(() =>
            {
                ResultGrid.IsReadOnly = Counter.Running;

                ResultGrid.ItemsSource = null;
                ResultGrid.ItemsSource = Entries;                

                StartBtn.IsEnabled = !Counter.Running;
                StopBtn.IsEnabled = Counter.Running;

                if(!Counter.Running)
                    SaveEntries();
            });
        }

        private void SaveEntries()
        {
            if(File.Exists("performance.xml")) File.Delete("performance.xml");
            using (FileStream fs = File.OpenWrite("performance.xml"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<PerformanceEntry>));
                serializer.Serialize(fs, Entries);
            }
        }

        private void LoadEntries()
        {
            try
            {
                using (FileStream fs = File.OpenRead("performance.xml"))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<PerformanceEntry>));
                    Entries = (List<PerformanceEntry>)serializer.Deserialize(fs);
                }
            }
            catch
            {
                Entries = new List<PerformanceEntry>()
                {
                    new PerformanceEntry() {Url = "http://youtube.com"},
                    new PerformanceEntry() {Url = "http://teamdev.com"},
                    new PerformanceEntry() {Url = "http://teamdev.com/dotnetbrowser"}
                };

                SaveEntries();
            }
        }

        private void StartBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!Counter.Running)
            {
                Counter.Measure(Entries);
            }
        }

        private void StopBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (Counter.Running)
            {
                Counter.Abort();
            }
        }
    }
}
