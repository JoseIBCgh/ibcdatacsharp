using ibcdatacsharp.UI.Device;
using System;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ibcdatacsharp.UI.AngleGraph
{
    /// <summary>
    /// Version de angle graph con la libreria RealTimeGraphX
    /// </summary>
    public partial class AngleGraph : Page
    {
        private const DispatcherPriority UPDATE_PRIORITY = DispatcherPriority.Render;
        private const DispatcherPriority CLEAR_PRIORITY = DispatcherPriority.Render;
        private Device.Device device;
        private Chart chartX;
        private Chart chartY;
        private Chart chartZ;
        public AngleGraph()
        {
            InitializeComponent();
            initCharts();
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            device = mainWindow.device;
            DataContext = this;
        }
        // Funcion para incializar los graficos
        private void initCharts()
        {
            chartX = new Chart("X");
            grid.Children.Add(chartX.chart);
            Grid.SetRow(chartX.chart, 0);
            Grid.SetColumn(chartX.chart, 0);

            chartY = new Chart("Y");
            grid.Children.Add(chartY.chart);
            Grid.SetRow(chartY.chart, 1);
            Grid.SetColumn(chartY.chart, 0);

            chartZ = new Chart("Z");
            grid.Children.Add(chartZ.chart);
            Grid.SetRow(chartZ.chart, 2);
            Grid.SetColumn(chartZ.chart, 0);
        }
        // Funcion para actualizar la grafica del acelerometro
        public async Task updateX(int frame, double data) 
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                chartX.update((float)data);
            });
        }
        // Funcion para actualizar la grafica del giroscopio
        public async Task updateY(int frame, double data)
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                chartY.update((float)data); ;
            });
        }
        // Funcion para actualizar la grafica del magnetometro
        public async Task updateZ(int frame, double data)
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                chartZ.update((float)data);
            });
        }
        // Recive los datos del IMU inventado
        public async void onTick(object sender, ElapsedEventArgs e)
        {
            AngleArgs angleArgs = device.angleData;
            int frame = device.frame;
            await Task.WhenAll(new Task[]
            {
                updateX(frame, angleArgs.angle[0]),
                updateY(frame, angleArgs.angle[1]),
                updateZ(frame, angleArgs.angle[2])
            });
        }
        // Recive los datos del IMU inventado media timer
        public async void onTick(object sender, EventArgs e)
        {
            AngleArgs angleArgs = device.angleData;
            int frame = device.frame;
            await Task.WhenAll(new Task[]
            {
                updateX(frame, angleArgs.angle[0]),
                updateY(frame, angleArgs.angle[1]),
                updateZ(frame, angleArgs.angle[2])
            });
        }
    }
}
