using ibcdatacsharp.UI.Device;
using System.Threading.Tasks;
using System.Timers;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using SharpDX.Direct3D9;

namespace ibcdatacsharp.UI.GraphWindow
{
    /// <summary>
    /// Lógica de interacción para GraphWindow.xaml
    /// </summary>
    public partial class GraphWindow : Page
    {
        private const DispatcherPriority UPDATE_PRIORITY = DispatcherPriority.Render;
        private Device.Device device;
        private Chart chartAccelerometer;
        private Chart chartGyroscope;
        private Chart chartMagnetometer;
        public GraphWindow()
        {
            InitializeComponent();
            initCharts();
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            device = mainWindow.device;
            DataContext = this;
        }
        // Inicializa los modelos
        private void initCharts()
        {
            chartAccelerometer = new Chart("Accelerometer", -80, 80, "m/s^2");
            grid.Children.Add(chartAccelerometer.chart);
            Grid.SetRow(chartAccelerometer.chart, 0);
            Grid.SetColumn(chartAccelerometer.chart, 0);

            chartGyroscope = new Chart("Gyroscope", -600, 600, "g/s^2");
            grid.Children.Add(chartGyroscope.chart);
            Grid.SetRow(chartGyroscope.chart, 1);
            Grid.SetColumn(chartGyroscope.chart, 0);

            chartMagnetometer = new Chart("Magnetometer", -4, 4, "k(mT)");
            grid.Children.Add(chartMagnetometer.chart);
            Grid.SetRow(chartMagnetometer.chart, 2);
            Grid.SetColumn(chartMagnetometer.chart, 0);
        }
        // Funcion para actualizar la grafica del acelerometro
        public async Task updateAccelerometer(int frame, double x, double y, double z)
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                chartAccelerometer.update(new float[] { (float)x, (float)y, (float)z });
            });
        }
        // Funcion para actualizar la grafica del giroscopio
        public async Task updateGyroscope(int frame, double x, double y, double z)
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                chartGyroscope.update(new float[] { (float)x, (float)y, (float)z });
            });
        }
        // Funcion para actualizar la grafica del magnetometro
        public async Task updateMagnetometer(int frame, double x, double y, double z)
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                chartMagnetometer.update(new float[] { (float)x, (float)y, (float)z });
            });
        }
        // Recive los datos del IMU inventado
        public async void onTick(object sender, ElapsedEventArgs e)
        {
            RawArgs rawArgs = device.rawData;
            int frame = device.frame;
            await Task.WhenAll(new Task[] {
                updateAccelerometer(frame, rawArgs.accelerometer[0], rawArgs.accelerometer[1], rawArgs.accelerometer[2]),
                updateMagnetometer(frame, rawArgs.magnetometer[0], rawArgs.magnetometer[1], rawArgs.magnetometer[2]),
                updateGyroscope(frame, rawArgs.gyroscope[0], rawArgs.gyroscope[1], rawArgs.gyroscope[2])
            });
        }
        // Recive los datos del IMU inventado media timer
        public async void onTick(object sender, EventArgs e)
        {
            RawArgs rawArgs = device.rawData;
            int frame = device.frame;
            await Task.WhenAll(new Task[] {
                updateAccelerometer(frame, rawArgs.accelerometer[0], rawArgs.accelerometer[1], rawArgs.accelerometer[2]),
                updateMagnetometer(frame, rawArgs.magnetometer[0], rawArgs.magnetometer[1], rawArgs.magnetometer[2]),
                updateGyroscope(frame, rawArgs.gyroscope[0], rawArgs.gyroscope[1], rawArgs.gyroscope[2])
            });
        }
        public async void startAll()
        {
            await Task.WhenAll(new Task[] {
                new Task(chartAccelerometer.Start),
                new Task(chartGyroscope.Start),
                new Task(chartMagnetometer.Start)
            });
        }
    }
}
