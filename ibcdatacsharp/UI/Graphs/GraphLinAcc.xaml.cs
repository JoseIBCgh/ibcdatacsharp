﻿using ibcdatacsharp.UI.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
using System.Windows.Threading;

namespace ibcdatacsharp.UI.Graphs
{
    /// <summary>
    /// Lógica de interacción para GraphLinAcc.xaml
    /// </summary>
    public partial class GraphLinAcc : Page, GraphInterface
    {
        private const DispatcherPriority UPDATE_PRIORITY = DispatcherPriority.Render;
        private const DispatcherPriority CLEAR_PRIORITY = DispatcherPriority.Render;
        protected Device.Device device;
        public Model3S model { get; private set; }
        public GraphLinAcc()
        {
            InitializeComponent();
            model = new Model3S(plot, -50, 50, title: "Acceleracion Lineal", units: "m/s^2");
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            device = mainWindow.device;
            DataContext = this;
            hasToRender = false;
            this.plot.Plot.XLabel("Frames");
            this.plot.Plot.YLabel("m/s^2"); ;
        }
        public void initCapture()
        {
            model.initCapture();
        }
        public async void drawRealTimeData(double accX, double accY, double accZ)
        {
            double[] acc = new double[3] { accX, accY, accZ };

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                model.updateData(acc);
            });

        }
        public async void drawRealTimeData(Vector3 data)
        {
            double[] array = new double[3] { data.X, data.Y, data.Z };

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                model.updateData(array);
            });

        }
        public async void drawData(GraphData data)
        {
            double[] accX = new double[data.length];
            double[] accY = new double[data.length];
            double[] accZ = new double[data.length];
            for (int i = 0; i < data.length; i++)
            {
                // Cambiar esto
                accX[i] = ((FrameData1IMU)data[i]).accX;
                accY[i] = ((FrameData1IMU)data[i]).accY;
                accZ[i] = ((FrameData1IMU)data[i]).accZ;
            }
            await Application.Current.Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                model.updateData(accX, accY, accZ);
            });
        }
        public async void onUpdateTimeLine(object sender, int frame)
        {
            await Application.Current.Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                model.updateIndex(frame);
            });
        }
        // Devuelve los datos del Acelerometro
        private double[] getData()
        {
            RawArgs rawArgs = device.rawData;
            return rawArgs.accelerometer;
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
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                model.clear();
            });
        }

        //Actualiza el render

        public async void render()
        {
            if (hasToRender)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    model.render();
                });
            }
        }

        // Actualiza el render
        public async void onRender(object sender, EventArgs e)
        {
            if (hasToRender)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    model.render();
                });
            }
        }
        public bool hasToRender { get; set; }
    }
}
