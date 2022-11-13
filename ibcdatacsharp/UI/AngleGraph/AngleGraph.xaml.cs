﻿using ibcdatacsharp.UI.Device;
using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using AngleGraphClass = ibcdatacsharp.UI.AngleGraph.AngleGraph;

namespace ibcdatacsharp.UI.AngleGraph
{
    /// <summary>
    /// Lógica de interacción para AngleGraph.xaml
    /// </summary>
    public partial class AngleGraph : Page
    {
        private const DispatcherPriority UPDATE_PRIORITY = DispatcherPriority.Render;
        private const DispatcherPriority CLEAR_PRIORITY = DispatcherPriority.Render;
        private Device.Device device;
        public AngleGraph()
        {
            InitializeComponent();
            initModels();
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            device = mainWindow.device;
            DataContext = this;
        }
        public Model modelX { get; private set; }
        public Model modelY { get; private set; }
        public Model modelZ { get; private set; }
        // Funcion para inicializar los graficos
        private void initModels()
        {
            modelX = new Model(angleX, titleY: "X Angle");
            modelY = new Model(angleY, titleY: "Y Angle");
            modelZ = new Model(angleZ, titleY: "Z Angle");
        }
        // Funcion para actualizar la grafica del acelerometro
        public async Task updateX(int frame, double data)
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                modelX.updateData(data);
            });
        }
        // Funcion para borrar los datos del acelerometro
        public async Task clearX()
        {
            await Dispatcher.BeginInvoke(CLEAR_PRIORITY, () =>
            {
                modelX.clear();
            });
        }
        // Funcion para actualizar la grafica del giroscopio
        public async Task updateY(int frame, double data)
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                modelY.updateData(data);
            });
        }
        // Funcion para borrar los datos del giroscopio
        public async Task clearY()
        {
            await Dispatcher.BeginInvoke(CLEAR_PRIORITY, () =>
            {
                modelY.clear();
            });
        }
        // Funcion para actualizar la grafica del magnetometro
        public async Task updateZ(int frame, double data)
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                modelZ.updateData(data);
            });
        }
        // Funcion para borrar los datos del magnetometro
        public async Task clearZ()
        {
            await Dispatcher.BeginInvoke(CLEAR_PRIORITY, () =>
            {
                modelZ.clear();
            });
        }
        // Recive los datos del IMU inventado
        public async void onTick(object sender, ElapsedEventArgs e)
        {
            AngleArgs angleArgs = device.angleData;
            int frame = device.frame;
            //await updateX(frame, angleArgs.angle[0]);
            //await updateY(frame, angleArgs.angle[1]);
            //await updateZ(frame, angleArgs.angle[2]);
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
        // Borra el contenido de los graficos
        public async void clearData()
        {
            //await clearX();
            //await clearY();
            //await clearZ();
            await Task.WhenAll(new Task[] {
                clearX(),
                clearY(),
                clearZ(),
            });
        }
        public async Task renderX()
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                modelX.render();
            });
        }
        public async Task renderY()
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                modelY.render();
            });
        }
        public async Task renderZ()
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                modelZ.render();
            });
        }
        public async void onRender(object sender, EventArgs e)
        {
            await Task.WhenAll(new Task[]
            {
                renderX(),
                renderY(),
                renderZ(),
            });
        }
    }
}
