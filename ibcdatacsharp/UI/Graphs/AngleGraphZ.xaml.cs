﻿using ibcdatacsharp.UI.Device;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ibcdatacsharp.UI.Graphs
{
    /// <summary>
    /// Lógica de interacción para AngleGraphX.xaml
    /// </summary>
    public partial class AngleGraphZ : Page, GraphInterface
    {
        private const DispatcherPriority UPDATE_PRIORITY = DispatcherPriority.Render;
        private const DispatcherPriority CLEAR_PRIORITY = DispatcherPriority.Render;
        protected Device.Device device;
        public Model1S model { get; private set; }
        public AngleGraphZ()
        {
            InitializeComponent();
            model = new Model1S(plot);
            model.valueEvent += onUpdateAngle;
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            device = mainWindow.device;
            DataContext = this;

            this.plot.Plot.XLabel("Frames");
            this.plot.Plot.YLabel("grados");
        }
        private void onUpdateAngle(object sender, double value)
        {
            angle.Text = value.ToString("0.#");
        }
        public void initCapture()
        {
            model.initCapture();
        }
        public async void drawData(GraphData data)
        {
            double[] angleZ = new double[data.length];
            for (int i = 0; i < data.length; i++)
            {
                // Provisional hay que cambiarlo por los angulos
                angleZ[i] = ((FrameData2IMUs)data[i]).angleZ;
            }
            await update(angleZ);
        }
        public async void onUpdateTimeLine(object sender, int frame)
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                model.updateIndex(frame);
            });
        }

        // Funcion para actualizar la grafica de golpe (para replay)
        public async Task update(double[] data)
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                model.updateData(data);
            });
        }
        // Actualiza los datos de los grafos
        public async void onTick(object sender, EventArgs e)
        {
            AngleArgs angleArgs = device.angleData;
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                model.updateData(angleArgs.angle[0]);
            });
        }
        // Borra el contenido de los graficos
        public async void clearData()
        {
            await Dispatcher.BeginInvoke(CLEAR_PRIORITY, () =>
            {
                model.clear();
            });
        }
        // Actualiza el render de los grafos
        public async void onRender(object sender, EventArgs e)
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                model.render();
            });
        }

        public async void drawRealTimeData(double angle)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                model.updateData(angle);
            });
        }

        private void onSetOffset(object sender, RoutedEventArgs e)
        {
            string offset_point = offset.Text.Replace(",", ".");
            model.offset = float.Parse(offset_point, CultureInfo.InvariantCulture);
        }
    }
}
