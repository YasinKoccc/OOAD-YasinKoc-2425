using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfEllipsen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int MaxEllipsen = 25;
        private const int MinSize = 20;
        private const int MaxSize = 100;
        private DispatcherTimer timer;
        private Random rand = new Random();
        private int ellipsCount = 0;
        public MainWindow()
        {
            InitializeComponent();
            Ellipse newEllipse = new Ellipse()
            {
                Width = 150,
                Height = 60,
                Fill = new SolidColorBrush(Color.FromRgb(122, 78, 200))
            };
            double xPos = 50;
            double yPos = 85;
            newEllipse.SetValue(Canvas.LeftProperty, xPos);
            newEllipse.SetValue(Canvas.TopProperty, yPos);
            canvas1.Children.Add(newEllipse);

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(50);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (ellipsCount >= MaxEllipsen)
            {
                timer.Stop();
                return;
            }

            Ellipse newEllipse = new Ellipse()
            {
                Width = rand.Next(MinSize, MaxSize),
                Height = rand.Next(MinSize, MaxSize),
                Fill = new SolidColorBrush(Color.FromRgb(
                    (byte)rand.Next(256),
                    (byte)rand.Next(256),
                    (byte)rand.Next(256)))
            };

            double xPos = rand.Next((int)(canvas1.Width - newEllipse.Width));
            double yPos = rand.Next((int)(canvas1.Height - newEllipse.Height));

            newEllipse.SetValue(Canvas.LeftProperty, xPos);
            newEllipse.SetValue(Canvas.TopProperty, yPos);

            canvas1.Children.Add(newEllipse);
            ellipsCount++;
        }

        private void btnTekenen_Click(object sender, RoutedEventArgs e)
        {
            ellipsCount = 0;
            canvas1.Children.Clear();
            timer.Start();
        }
    }
}