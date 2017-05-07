using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace BDOCalc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr onj);

        private ObservableCollection<BitmapImage> _healthBars = new ObservableCollection<BitmapImage>();
        public ObservableCollection<BitmapImage> HealthBars
        {
            get { return _healthBars; }
            set { _healthBars = value; }
        }

        private ObservableCollection<String> _percents = new ObservableCollection<String>();
        public ObservableCollection<String> Percents
        {
            get { return _percents; }
            set { _percents = value; }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void takeScreenShot_Click(object sender, RoutedEventArgs e)
        {
            double screenWidth = SystemParameters.WorkArea.Width;
            double screenHeight = SystemParameters.WorkArea.Height;

            double barLeft = screenWidth * .4506;
            double barTop = screenHeight * .0362;
            double barWidth = screenWidth * .0995;
            double barHeight = screenHeight * .0085;

            using (Bitmap bmp = new Bitmap((int)barWidth, (int)barHeight))
            {
                Bitmap bitmap;
                bitmap = new Bitmap((int)barWidth,
                    (int)barHeight,
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                using (Graphics g = Graphics.FromImage(bmp))
                {

                    String filename = "ScreenCapture-" + DateTime.Now.ToString("ddMMyyyy-hhmmssfff") + ".png";
                    Opacity = .0;
                    g.CopyFromScreen((int)barLeft, (int)barTop, 0, 0, bmp.Size);
                    bmp.Save("C:\\Screenshots\\" + filename);
                    Opacity = 1;

                    BitmapImage bmpImg = Utils.ToBitmapImage(bmp);
                    _healthBars.Insert(0, bmpImg);
                    String percent = Utils.GetHealthPercent(bmpImg);
                    _percents.Insert(0, percent);
                }
            }

        }

        private void exportToCSV_Click(object sender, RoutedEventArgs e)
        {
            string fileText = "";
            foreach (String percent in Percents)
            {
                fileText += percent + "\n";
            }

            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "CSV Files(*.csv)|*.csv|All(*.*)|*"
            };

            if (dialog.ShowDialog() == true)
            {
                File.WriteAllText(dialog.FileName, fileText);
            }
        }

    }
}
