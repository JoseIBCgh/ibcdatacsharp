//# define REAL_TIME_GRAPH_X //Usar RealTimeGraphX para los graficos

using System;
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
using WisewalkSDK;

using GraphWindowClass = ibcdatacsharp.UI.GraphWindow.GraphWindow;
using AngleGraphClass = ibcdatacsharp.UI.AngleGraph.AngleGraph;
using System.IO.Ports;

namespace ibcdatacsharp.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    

    public partial class MainWindow : System.Windows.Window
    {

        private const int CAPTURE_MS = 10;
        private const int RENDER_MS = 100;

        public Device.Device device;
        public VirtualToolBar virtualToolBar;

        private System.Timers.Timer timerCapture;
        private FileSaver.FileSaver fileSaver;


        // WiseWare vars
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
        string macaddress = "";


        public MainWindow()
        {
            InitializeComponent();
            setGraphLibraries();
            virtualToolBar = new VirtualToolBar();
            device = new Device.Device();
            fileSaver = new FileSaver.FileSaver();
            initIcon();
            initToolBarHandlers();
            initMenuHandlers();
            loadAllGraphs();

            // WiseWalk
            string error = "";
            api = new Wisewalk();
            version = api.GetApiVersion();

            devices_list = new Dictionary<string, WisewalkSDK.Device>();

            counter = new List<int>();
            api.scanFinished += Api_scanFinished;


            string[] ports = SerialPort.GetPortNames();


            // Display each port name to the console.
            foreach (string port in ports)
            {
                Trace.WriteLine(port);

            }
            if (api.Open("COM6", out error))
            {
                Trace.WriteLine("Opened");
            }

            if (api.ScanDevices(out error))
            {
                Trace.WriteLine("Scanned :");
            }




        }

        private void Api_scanFinished(List<Wisewalk.Dev> devices)
        {
            scanDevices = devices;
            ShowScanList(scanDevices);
           
        }

        private void ShowScanList(List<Wisewalk.Dev> devices)
        {
            

            for (int idx = 0; idx < devices.Count; idx++)
            {
                string macAddress = devices[idx].mac[5].ToString("X2") + ":" + devices[idx].mac[4].ToString("X2") + ":" + devices[idx].mac[3].ToString("X2") + ":" +
                                    devices[idx].mac[2].ToString("X2") + ":" + devices[idx].mac[1].ToString("X2") + ":" + devices[idx].mac[0].ToString("X2");


                Trace.WriteLine("", " * " + macAddress);
            }

            
        }



        private void setGraphLibraries()
        {
            graphWindow.Source = new Uri("pack://application:,,,/UI/GraphWindow/GraphWindow.xaml");
            angleGraph.Source = new Uri("pack://application:,,,/UI/AngleGraph/AngleGraph.xaml");
        }
        // Si se captura antes de visitar una pestaña sale un error (RealTimeGraphX)
        private void loadAllGraphs()
        {
            void changeToAngle()
            {
                graphsTabControl.SelectedIndex = 1;
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(15);
                timer.Tick += (sender, e) =>
                {
                    graphsTabControl.SelectedIndex = 0;
                    timer.Stop();
                };
                timer.Start();
            }
            if (angleGraph.Content == null)
            {
                angleGraph.Navigated += delegate (object sender, NavigationEventArgs e)
                {
                    changeToAngle();
                };
            }
            else
            {
                changeToAngle();
            }
        }
        // Configura el timer capture
        private void initTimerCapture()
        {
            void onPause(object sender, PauseState pauseState)
            {
                if (pauseState == PauseState.Pause)
                {
                    timerCapture.Stop();
                }
                else if (pauseState == PauseState.Play)
                {
                    timerCapture.Start();
                }
            }
            void onStop(object sender)
            {
                virtualToolBar.pauseEvent -= onPause;
                virtualToolBar.stopEvent -= onStop;
                timerCapture.Dispose();
                timerCapture = null;
            }
            if (timerCapture == null)
            {
                timerCapture = new System.Timers.Timer(CAPTURE_MS);
                timerCapture.AutoReset = true;
                if (graphWindow.Content == null)
                {
                    graphWindow.Navigated += delegate (object sender, NavigationEventArgs e)
                    {
                        GraphWindowClass graphWindowClass = graphWindow.Content as GraphWindowClass;
                        graphWindowClass.clearData();
                        timerCapture.Elapsed += graphWindowClass.onTick;
                    };
                }
                else
                {
                    GraphWindowClass graphWindowClass = graphWindow.Content as GraphWindowClass;
                    graphWindowClass.clearData();
                    timerCapture.Elapsed += graphWindowClass.onTick;
                }
                if (angleGraph.Content == null)
                {
                    angleGraph.Navigated += delegate (object sender, NavigationEventArgs e)
                    {
                        AngleGraphClass angleGraphClass = angleGraph.Content as AngleGraphClass;
                        angleGraphClass.clearData();
                        timerCapture.Elapsed += angleGraphClass.onTick;
                    };
                }
                else
                {
                    AngleGraphClass angleGraphClass = angleGraph.Content as AngleGraphClass;
                    angleGraphClass.clearData();
                    timerCapture.Elapsed += angleGraphClass.onTick;
                }
                virtualToolBar.pauseEvent += onPause; //funcion local
                virtualToolBar.stopEvent += onStop; //funcion local
                if (virtualToolBar.pauseState == PauseState.Play)
                {
                    timerCapture.Start();
                }
                device.initTimer();
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

                    string mac = await Task.Run(() =>
                    {
                        while (macaddress == "")
                        {
                            continue;
                        }
                        return macaddress;
                    }
                           
                       );
                    //names.ForEach(n => Trace.WriteLine(n));
                    List<int> indices = await Task.Run(() => cameraIndices(names.Count));
                    //indices.ForEach(i => Trace.WriteLine(i));

                    Trace.WriteLine("This is mac: ", mac);
                   

                    deviceListClass.addIMU(new IMUInfo("IMU", mac));


                    for (int i = 0; i < names.Count; i++)
                    {
                        if (indices.Contains(i))
                        {
                            deviceListClass.addCamera(new CameraInfo(i, names[i]));
                        }
                    }

                    
                    
                }
               
                DeviceList.DeviceList deviceListClass = deviceList.Content as DeviceList.DeviceList;
                deviceListClass.clearAll();
                addCameras(deviceListClass);
                
                deviceListClass.hideIMUs();
                deviceListClass.showCameras();
                deviceListClass.hideInsoles(); //Por defecto estan escondidos pero si los muestras una vez los tienes que volver a esconder
                
                deviceListClass.showIMUs();
                deviceListClass.showInsoles();
                //deviceListClass.addIMU(new IMUInfo("IMU", "AD:DS"));
                deviceListClass.addInsole(new InsolesInfo("Insole", "Left"));
              



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
       

        private void toolBar_Navigated(object sender, NavigationEventArgs e)
        {

        }
    }
}
