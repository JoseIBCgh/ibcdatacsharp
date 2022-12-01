﻿using ibcdatacsharp.UI.Device;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ibcdatacsharp.UI.Graphs
{
    /// <summary>
    /// Lógica de interacción para GraphGyroscope.xaml
    /// </summary>
    public partial class GraphGyroscope : Page, GraphInterface
    {
        private const DispatcherPriority UPDATE_PRIORITY = DispatcherPriority.Render;
        private const DispatcherPriority CLEAR_PRIORITY = DispatcherPriority.Render;
        protected Device.Device device;
        public Model3S model { get; private set; }
        public GraphGyroscope()
        {
            InitializeComponent();
            model = new Model3S(plot, -50, 50, title: "Gyroscope", units: "g/s^2");
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            device = mainWindow.device;
            DataContext = this;

            this.plot.Plot.XLabel("Frames");
            this.plot.Plot.YLabel("grados/s");
        }
        public void initCapture()
        {
            model.initCapture();
        }
        public async void drawData(GraphData data)
        {
            double[] gyrX = new double[data.length];
            double[] gyrY = new double[data.length];
            double[] gyrZ = new double[data.length];
            for (int i = 0; i < data.length; i++)
            {
                gyrX[i] = data[i].gyrX;
                gyrY[i] = data[i].gyrY;
                gyrZ[i] = data[i].gyrZ;
            }
            await Application.Current.Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                model.updateData(gyrX, gyrY, gyrZ);
            });
        }
        public async void onUpdateTimeLine(object sender, int frame)
        {
            await Application.Current.Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                model.updateIndex(frame);
            });
        }
        // Devuelve los datos del Giroscopio
        private double[] getData()
        {
            RawArgs rawArgs = device.rawData;
            return rawArgs.gyroscope;
        }
        // Actualiza los datos
        public async void onTick(object sender, EventArgs e)
        {
            await Application.Current.Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                model.updateData(getData());
            });
        }
        // Borra el contenido de los graficos
        public async void clearData()
        {
            await Application.Current.Dispatcher.BeginInvoke(CLEAR_PRIORITY, () =>
            {
                model.clear();
            });
        }
        // Actualiza el renderizado
        public async void onRender(object sender, EventArgs e)
        {
            await Application.Current.Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                model.render();
            });
        }

        public async void render()
        {
            await Application.Current.Dispatcher.InvokeAsync( () =>
            {
                model.render();
            });
        }
        public async void drawRealTimeData(double accX, double accY, double accZ)
        {
            double[] acc = new double[3] { accX, accY, accZ };

            await Application.Current.Dispatcher.InvokeAsync( () =>
            {
                model.updateData(acc);
            });
        }
    }
}
