﻿using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Navigation;
using ibcdatacsharp.DeviceList.TreeClasses;
using System.Windows.Media.Imaging;
using DirectShowLib;
using System.Collections.Generic;
using OpenCvSharp;
using System.Threading.Tasks;
using ibcdatacsharp.UI.ToolBar;
using ibcdatacsharp.UI.Timer;
using ibcdatacsharp.UI.ToolBar.Enums;
using System.Threading;

using GraphWindowClass = ibcdatacsharp.UI.GraphWindow.GraphWindow;
using AngleGraphClass = ibcdatacsharp.UI.AngleGraph.AngleGraph;

using WisewalkSDK;
using static WisewalkSDK.Protocol_v3;


namespace ibcdatacsharp.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {

        private const int CAPTURE_MS = 10;
        private const int RENDER_MS = 100;

        public Device.Device device;
        public VirtualToolBar virtualToolBar;

        private System.Timers.Timer timerCapture;
        private System.Timers.Timer timerRender;
        private FileSaver.FileSaver fileSaver;

        //Wiseware
        // WiseWare
        private const string pathDir = @"c:\Wiseware\Wisewalk-API\";

        private const int ColumnBatteryIndex = 2;
        private const int ColumnNPacketsIndex = 5;
        private const int ColumnTimespanIndex = 6;
        private const int ColumnAccIndex = 7;

        private Wisewalk api;
        private string portName = "";
        private int baudRate = 921600;
        private List<Wisewalk.ComPort> ports;
        private bool isConnected = false;
        private bool startStream = false;
        private bool startRecord = false;
        private Dictionary<string, WisewalkSDK.Device> devices_list = new Dictionary<string, WisewalkSDK.Device>();

        private byte counterUI = 0;

        private readonly ushort[] SampleRate = { 25, 50, 100, 200 };

        private string version = "";

        private List<int> counter = new List<int>();

        private List<Wisewalk.Dev> scanDevices = null;

        private bool devConnected = false;

        private int indexDev = -1;

        private int indexSelected = -1;
        private short handlerSelected = -1;
        float[] acc = new float[3];

        

        string error = "";
        public MainWindow()
        {
            InitializeComponent();
            setGraphLibraries();
            graphWindow.Source = new Uri("pack://application:,,,/UI/GraphWindow/GraphWindow.xaml");
            

            virtualToolBar = new VirtualToolBar();
            device = new Device.Device();
            fileSaver = new FileSaver.FileSaver();
            initIcon();
            initToolBarHandlers();
            initMenuHandlers();
            //loadAllGraphs();

            //WiseWalk
;       
            ports = new List<Wisewalk.ComPort>();

            devices_list = new Dictionary<string, WisewalkSDK.Device>();

            counter = new List<int>();
            api = new Wisewalk();


            version = api.GetApiVersion();
            api.scanFinished += Api_scanFinished;
            
            

           

        }

        

        private void Api_scanFinished(List<Wisewalk.Dev> devices)
        {
            scanDevices = devices;
            Trace.WriteLine(devices.Count);
            ShowScanList(scanDevices);
           
        }

        private void ShowScanList(List<Wisewalk.Dev> devices)
        {
            
            for (int idx = 0; idx < devices.Count; idx++)
            {
                string macAddress = devices[idx].mac[5].ToString("X2") + ":" + devices[idx].mac[4].ToString("X2") + ":" + devices[idx].mac[3].ToString("X2") + ":" +
                                    devices[idx].mac[2].ToString("X2") + ":" + devices[idx].mac[1].ToString("X2") + ":" + devices[idx].mac[0].ToString("X2");


                Trace.WriteLine("MacAddress: ", " * " + macAddress);
            }

        }

        private void Api_deviceConnected(byte handler, WisewalkSDK.Device dev)
        {
            if (!devices_list.ContainsKey(handler.ToString()))
            {
                // Add new device to list
                WisewalkSDK.Device device = new WisewalkSDK.Device();

                devices_list.Add(handler.ToString(), device);

                // Update values
                devices_list[handler.ToString()].Id = dev.Id;
                devices_list[handler.ToString()].Name = dev.Name;
                devices_list[handler.ToString()].Connected = dev.Connected;
                devices_list[handler.ToString()].HeaderInfo = dev.HeaderInfo;
                devices_list[handler.ToString()].NPackets = 0;
                devices_list[handler.ToString()].sampleRate = dev.sampleRate;
                devices_list[handler.ToString()].offsetTime = dev.offsetTime;
                devices_list[handler.ToString()].Rtc = dev.Rtc;
                devices_list[handler.ToString()].Stream = false;
                devices_list[handler.ToString()].Record = false;

                counter.Add(0);

                Trace.WriteLine("Dev: " + devices_list["0"].Id);
            }
            else
            {
                // Update info device
                devices_list[handler.ToString()].HeaderInfo = dev.HeaderInfo;
            }
        
            //ShowDevices(devices_list);
       
        }

    
        private void setGraphLibraries()
        {
            //graphWindow.Source = new Uri("pack://application:,,,/UI/GraphWindow/GraphWindow.xaml");
            angleGraph.Source = new Uri("pack://application:,,,/UI/AngleGraph/AngleGraph.xaml");
        }
        // Configura el timer capture
        private void initTimerCapture()
        {
            

            void onPause(object sender, PauseState pauseState)
            {
                if (pauseState == PauseState.Pause)
                {
                    timerCapture.Stop();
                    timerRender.Stop();
                }
                else if (pauseState == PauseState.Play)
                {
                    timerCapture.Start();
                    timerRender.Start();
                }
            }
            void onStop(object sender)
            {
                virtualToolBar.pauseEvent -= onPause;
                virtualToolBar.stopEvent -= onStop;
                timerCapture.Dispose();
                timerCapture = null;
                timerRender.Dispose();
                timerRender = null;
            }
            if (timerCapture == null)
            {
                timerCapture = new System.Timers.Timer(CAPTURE_MS);
                timerCapture.AutoReset = true;
                timerRender = new System.Timers.Timer(RENDER_MS);
                timerRender.AutoReset = true;
                
                if (graphWindow.Content == null)
                {
                   

                    graphWindow.Navigated += delegate (object sender, NavigationEventArgs e)
                    {
                        
                        GraphWindowClass graphWindowClass = graphWindow.Content as GraphWindowClass;
                        graphWindowClass.clearData();

                        api.dataReceived += graphWindowClass.Api_dataReceived;
                      

                    };
                }
                else
                {
                    

                    GraphWindowClass graphWindowClass = graphWindow.Content as GraphWindowClass;
                    graphWindowClass.clearData();
                    api.dataReceived += graphWindowClass.Api_dataReceived;
                    //timerCapture.Elapsed += graphWindowClass.onTick;
                    //timerRender.Elapsed += graphWindowClass.onRender;

                }
                if (angleGraph.Content == null)
                {
                    angleGraph.Navigated += delegate (object sender, NavigationEventArgs e)
                    {
                        AngleGraphClass angleGraphClass = angleGraph.Content as AngleGraphClass;
                        angleGraphClass.clearData();
                        //timerCapture.Elapsed += angleGraphClass.onTick;
                        //timerRender.Elapsed += angleGraphClass.onRender;
                        
                    };
                }
                else
                {
                    AngleGraphClass angleGraphClass = angleGraph.Content as AngleGraphClass;
                    angleGraphClass.clearData();
                    //timerCapture.Elapsed += angleGraphClass.onTick;
                    //timerRender.Elapsed += angleGraphClass.onRender;
                }
                virtualToolBar.pauseEvent += onPause; //funcion local
                virtualToolBar.stopEvent += onStop; //funcion local
                if (virtualToolBar.pauseState == PauseState.Play)
                {
                    timerCapture.Start();
                    timerRender.Start();
                }
                device.initTimer();
                api.StartStream(out error);
            }
            
        }
        // Cambia el icono de la ventana
        private void initIcon()
        {
            Uri iconUri = new Uri("pack://application:,,,/UI/MenuBar/Icons/ibc-logo.png", UriKind.RelativeOrAbsolute);
            Icon = BitmapFrame.Create(iconUri);
        }
        // Conecta los botones de la ToolBar
        private void initToolBarHandlers()
        {
            toolBar.Navigated += delegate (object sender, NavigationEventArgs e)
            {
                ToolBar.ToolBar toolBarClass = toolBar.Content as ToolBar.ToolBar;
                toolBarClass.scan.Click += new RoutedEventHandler(onScan);
                toolBarClass.connect.Click += new RoutedEventHandler(onConnect);
                toolBarClass.disconnect.Click += new RoutedEventHandler(onDisconnect);
                toolBarClass.openCamera.Click += new RoutedEventHandler(onOpenCamera);
                toolBarClass.capture.Click += new RoutedEventHandler(onCapture);
                toolBarClass.pause.Click += new RoutedEventHandler(onPause);
                toolBarClass.stop.Click += new RoutedEventHandler(onStop);
                toolBarClass.record.Click += new RoutedEventHandler(onRecord);
                toolBarClass.capturedFiles.Click += new RoutedEventHandler(onCapturedFiles); 
            };
        }
        // Conecta los botones del Menu
        private void initMenuHandlers()
        {
            menuBar.Navigated += delegate (object sender, NavigationEventArgs e)
            {
                MenuBar.MenuBar menuBarClass = menuBar.Content as MenuBar.MenuBar;
                menuBarClass.scan.Click += new RoutedEventHandler(onScan);
                menuBarClass.connect.Click += new RoutedEventHandler(onConnect);
                menuBarClass.disconnect.Click += new RoutedEventHandler(onDisconnect);
                menuBarClass.openCamera.Click += new RoutedEventHandler(onOpenCamera);
                menuBarClass.capture.Click += new RoutedEventHandler(onCapture);
                menuBarClass.pause.Click += new RoutedEventHandler(onPause);
                menuBarClass.stop.Click += new RoutedEventHandler(onStop);
                menuBarClass.record.Click += new RoutedEventHandler(onRecord);
                menuBarClass.capturedFiles.Click += new RoutedEventHandler(onCapturedFiles);
                menuBarClass.exit.Click += new RoutedEventHandler(onExit);
            };
        }
        // Funcion que llaman todos los handlers del ToolBar. Por si acaso el device list no se ha cargado.
        private void deviceListLoadedCheck(Action func)
        {
            if (deviceList.Content == null)
            {
                deviceList.Navigated += delegate (object sender, NavigationEventArgs e)
                {
                    func();
                };
            }
            else
            {
                func();
            }
        }
        // Conecta el boton scan
        private void onScan(object sender, EventArgs e)
        {
            
            // Funcion que se ejecuta al clicar el boton scan
            void onScanFunction()
            {
                // Añade las camaras al TreeView
                async void addCameras(DeviceList.DeviceList deviceListClass)
                {
                    // Devuelve el nombre de todas las camaras conectadas
                    List<string> cameraNames()
                    {
                        List<DsDevice> devices = new List<DsDevice>(DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice));
                        List<string> cameraNames = new List<string>();
                        foreach (DsDevice device in devices)
                        {
                            cameraNames.Add(device.Name);
                        }
                        return cameraNames;
                    }
                    // Devuelve una lista de indice OpenCV de las camaras disponibles
                     List<int> cameraIndices(int maxIndex = 10)
                    {
                        List<int> indices = new List<int>();
                        VideoCapture capture = new VideoCapture();
                        for(int index = 0; index < maxIndex; index++)
                        {
                            capture.Open(index, VideoCaptureAPIs.DSHOW);
                            if (capture.IsOpened())
                            {
                                indices.Add(index);
                                capture.Release();
                            }
                        }
                        return indices;
                    }
                    List<string> names = await Task.Run(() => cameraNames());
                    //names.ForEach(n => Trace.WriteLine(n));
                    List<int> indices = await Task.Run(() => cameraIndices(names.Count));
                    //indices.ForEach(i => Trace.WriteLine(i));
                    for (int i = 0; i < names.Count; i++)
                    {
                        if (indices.Contains(i))
                        {
                            deviceListClass.addCamera(new CameraInfo(i, names[i]));
                        }
                    }

                    api.Open("COM6", out error);
                    api.ScanDevices(out error);
                   

                }
                DeviceList.DeviceList deviceListClass = deviceList.Content as DeviceList.DeviceList;
                deviceListClass.clearAll();
                addCameras(deviceListClass);
                deviceListClass.hideIMUs();
                deviceListClass.showCameras();
                deviceListClass.hideInsoles(); //Por defecto estan escondidos pero si los muestras una vez los tienes que volver a esconder
                
                deviceListClass.showIMUs();
                deviceListClass.addIMU(new IMUInfo("IMU", "AD:DS"));
                /*
                deviceListClass.showInsoles();
                deviceListClass.addInsole(new InsolesInfo("Insole", "Left"));
                */
            }
            deviceListLoadedCheck(onScanFunction);
        }
        // Conecta el boton connect
        private void onConnect(object sender, EventArgs e)
        {
            // Funcion que se ejecuta al clicar el boton connect
            void onConnectFunction()
            {
                DeviceList.DeviceList deviceListClass = deviceList.Content as DeviceList.DeviceList;
                object selected = deviceListClass.treeView.SelectedItem;
                if (selected != null)
                {
                    if (selected is IMUInfo)
                    {
                        TreeViewItem treeViewItem = (TreeViewItem)deviceListClass.IMUs.ItemContainerGenerator.ContainerFromItem(selected);
                        deviceListClass.connectIMU(treeViewItem);
                    }
                    else if (selected is CameraInfo)
                    {
                        TreeViewItem treeViewItem = (TreeViewItem)deviceListClass.cameras.ItemContainerGenerator.ContainerFromItem(selected);
                        deviceListClass.connectCamera(treeViewItem);
                    }
                }
            }

            handlerSelected = 0;
            api.Connect(scanDevices, out error);
            Thread.Sleep(1000);
            api.SetDeviceConfiguration(0, 100, 3, out error);
            Thread.Sleep(1000);

            deviceListLoadedCheck(onConnectFunction);
        }
        // Conecta el boton disconnect
        private void onDisconnect(object sender, EventArgs e)
        {
            // Funcion que se ejecuta al clicar el boton disconnect
            void onDisconnectFunction()
            {
                DeviceList.DeviceList deviceListClass = deviceList.Content as DeviceList.DeviceList;
                object selected = deviceListClass.treeView.SelectedItem;
                if (selected != null && selected is IMUInfo)
                {
                    TreeViewItem treeViewItem = (TreeViewItem)deviceListClass.IMUs.ItemContainerGenerator.ContainerFromItem(selected);
                    deviceListClass.disconnectIMU(treeViewItem);
                }
            }
            deviceListLoadedCheck(onDisconnectFunction);
        }
        // Conecta el boton Open Camera
        private void onOpenCamera(object sender, EventArgs e)
        {
            // Funcion que se ejecuta al clicar el boton Open Camera
            void onOpenCameraFunction()
            {
                DeviceList.DeviceList deviceListClass = deviceList.Content as DeviceList.DeviceList;
                object selected = deviceListClass.treeView.SelectedItem;
                if (selected != null && selected is CameraInfo)
                {
                    CameraInfo cameraInfo = (CameraInfo)selected;
                    int id = cameraInfo.number; //Id de la camara
                    CamaraViewport.CamaraViewport camaraViewportClass = camaraViewport.Content as CamaraViewport.CamaraViewport;
                    if (!camaraViewportClass.someCameraOpened())
                    {
                        camaraViewportClass.Title = cameraInfo.name + " CAM " + id;
                        camaraViewportClass.initializeCamara(id);
                    }
                }
            }
            deviceListLoadedCheck(onOpenCameraFunction);
        }
        // Funcion que se ejecuta al clicar el boton Capture
        private void onCapture(object sender, EventArgs e)
        {

            initTimerCapture();
            
        }
        // Funcion que se ejecuta al clicar el boton Pause
        private void onPause(object sender, EventArgs e)
        {
            virtualToolBar.pauseClick();
        }
        // Funcion que se ejecuta al clicar el boton Stop
        private void onStop(object sender, EventArgs e)
        {
            virtualToolBar.stopClick();
        }
        // Funcion que se ejecuta al clicar el boton Record
        private void onRecord(object sender, EventArgs e)
        {
            virtualToolBar.recordClick();
        }
        // Funcion que se ejecuta al clicar el boton Show Captured Files
        private void onCapturedFiles(object sender, EventArgs e)
        {
            Trace.WriteLine("Show Captured Files");
        }
        // Funcion que se ejecuta al clicar el menú Exit
        private void onExit(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
        // Funcion que se ejecuta al cerrar la ventana
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            CamaraViewport.CamaraViewport camaraViewportClass = camaraViewport.Content as CamaraViewport.CamaraViewport;
            camaraViewportClass.onCloseApplication();
            fileSaver.onCloseApplication();
            base.OnClosing(e);
        }
    }
}
