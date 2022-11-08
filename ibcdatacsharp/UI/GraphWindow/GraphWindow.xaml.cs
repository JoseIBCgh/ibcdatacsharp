using ibcdatacsharp.UI.Device;
using ibcdatacsharp.UI.DeviceList;
using OpenCvSharp.Flann;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WisewalkSDK;


namespace ibcdatacsharp.UI.GraphWindow
{
    /// <summary>
    /// Lógica de interacción para GraphWindow.xaml
    /// </summary>
    /// 
    

    public partial class GraphWindow : Page
    {
        private const DispatcherPriority UPDATE_PRIORITY = DispatcherPriority.Render;
        private const DispatcherPriority CLEAR_PRIORITY = DispatcherPriority.Render;
        private Device.Device device;

        double[] acc = new double[9];
        
        Dictionary<string, WisewalkSDK.Device> devices_list = new Dictionary<string, WisewalkSDK.Device>();
        public List<int> counter = new List<int>();
        public GraphWindow()
        {
            InitializeComponent();
            initModels();
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            device = mainWindow.device;
            DataContext = this;
            
        }
        public Model modelAccelerometer { get; private set; }
        public Model modelGyroscope { get; private set; }
        public Model modelMagnetometer { get; private set; }
        // Funcion para inicializar los graficos
        private void initModels()
        {
            modelAccelerometer = new Model(accelerometer ,-100, 100, titleY : "Accelerometer", units : "m/s^2");
            modelGyroscope = new Model(gyroscope ,-5000, 5000, titleY: "Gyroscope", units: "g/s^2");
            modelMagnetometer = new Model(magnetometer ,-20, 20, titleY: "Magnetometer", units: "k(mT)");
        }
        // Funcion para actualizar la grafica del acelerometro
        public async Task updateAccelerometer(int frame, double x, double y, double z)
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                modelAccelerometer.updateData(new double[] { x, y, z });
            });
        }
        // Funcion para borrar los datos del acelerometro
        public async Task clearAccelerometer()
        {
            await Dispatcher.BeginInvoke(CLEAR_PRIORITY, () =>
            {
                modelAccelerometer.clear();
            });
        }
        // Funcion para actualizar la grafica del giroscopio
        public async Task updateGyroscope(int frame, double x, double y, double z)
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                modelGyroscope.updateData(new double[] { x, y, z });
            });
        }
        // Funcion para borrar los datos del giroscopio
        public async Task clearGyroscope()
        {
            await Dispatcher.BeginInvoke(CLEAR_PRIORITY, () =>
            {
                modelGyroscope.clear();
            });
        }
        // Funcion para actualizar la grafica del magnetometro
        public async Task updateMagnetometer(int frame, double x, double y, double z)
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                modelMagnetometer.updateData(new double[] { x, y, z });
            });
        }
        // Funcion para borrar los datos del magnetometro
        public async Task clearMagnetometer()
        {
            await Dispatcher.BeginInvoke(CLEAR_PRIORITY, () =>
            {
                modelMagnetometer.clear();
            });
        }
        // Recive los datos del IMU inventado
        public async void onTick(object sender, ElapsedEventArgs e)
        {
            RawArgs rawArgs = device.rawData;
            int frame = device.frame;
            //await updateAccelerometer(frame, rawArgs.accelerometer[0], rawArgs.accelerometer[1], rawArgs.accelerometer[2]);
            //await updateMagnetometer(frame, rawArgs.magnetometer[0], rawArgs.magnetometer[1], rawArgs.magnetometer[2]);
            //await updateGyroscope(frame, rawArgs.gyroscope[0], rawArgs.gyroscope[1], rawArgs.gyroscope[2]);
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

        // <summary>
        /// Get stream duration (HH:MM:SS).
        /// </summary>
        /// <returns></returns>
        private string GetStreamDuration()
        {
            string streamDuration = "";

            try
            {
                int maxPacket = 0;
                int sampleRate = 0;
                for (int idx = 0; idx < devices_list.Count; idx++)
                {
                    if (devices_list[idx.ToString()].NPackets * devices_list[idx.ToString()].HeaderInfo.sampleFrame > maxPacket)
                    {
                        maxPacket = devices_list[idx.ToString()].NPackets * devices_list[idx.ToString()].HeaderInfo.sampleFrame;
                        sampleRate = devices_list[idx.ToString()].sampleRate;
                    }
                }

                int seconds = maxPacket / sampleRate;

                if (seconds > 0)
                {
                    int nHours = seconds / 3600;
                    int nMinutes = (seconds % 3600) / 60;
                    int nSeconds = (seconds % 3600) % 60;

                    streamDuration = nHours.ToString() + ":" +
                                     (nMinutes > 9 ? nMinutes.ToString() : "0" + nMinutes.ToString()) + ":" +
                                     (nSeconds > 9 ? nSeconds.ToString() : "0" + nSeconds.ToString());
                }
                else
                {
                    streamDuration = " - - : - - : - - ";
                }
            }
            catch (Exception ex)
            {
                streamDuration = " - - : - - : - - ";
            }

            return streamDuration;
        }

        //Callback para recoger datas del IMU
        public async void Api_dataReceived( byte deviceHandler, WisewalkSDK.WisewalkData data)
        {
            
            devices_list[deviceHandler.ToString()].NPackets++;
            devices_list[deviceHandler.ToString()].Stream = true;
            
            // N. Packets + N. Frames received from device handler
            string frame2 = counter[deviceHandler].ToString() + " / " + (devices_list[deviceHandler.ToString()].NPackets * devices_list[deviceHandler.ToString()].HeaderInfo.sampleFrame).ToString();

            int sr = devices_list[deviceHandler.ToString()].sampleRate;
            int timespan = (int)((devices_list[deviceHandler.ToString()].NPackets * devices_list[deviceHandler.ToString()].HeaderInfo.sampleFrame) * ((1 / (float)devices_list[deviceHandler.ToString()].sampleRate) * 1000));

            string ts = timespan.ToString();
            
            Trace.WriteLine("Data: " + " "+ frame2 + " " + data.Imu[0].acc_x.ToString("F3") + " " + data.Imu[0].acc_y.ToString("F3") +" "+ data.Imu[0].acc_z.ToString("F3") 
                + " " + data.Imu[0].gyro_x.ToString("F3") + " " +  data.Imu[0].gyro_y.ToString("F3") +" "+ data.Imu[0].gyro_z.ToString("F3") + " " +
                data.Imu[0].mag_x.ToString("F3") + " " + data.Imu[0].mag_y.ToString("F3") + " " + data.Imu[0].mag_z.ToString("F3"));

        

            int frame = device.frame;
            await Task.WhenAll(new Task[] {
                updateAccelerometer(frame, data.Imu[0].acc_x, data.Imu[0].acc_y, data.Imu[0].acc_z),
                updateMagnetometer(frame, data.Imu[0].gyro_x, data.Imu[0].gyro_y, data.Imu[0].gyro_z),
                updateGyroscope(frame, data.Imu[0].mag_x, data.Imu[0].mag_y, data.Imu[0].mag_z),
                renderAcceletometer(),
                renderGyroscope(),
                renderMagnetometer(),

            });

        }

        public async void devicesList(Dictionary<string, WisewalkSDK.Device> dl, List<int> c)
        {
            devices_list = dl;
            counter = c;
            Trace.WriteLine(c.Count);

        }

        // Borra el contenido de los graficos
        public async void clearData()
        {
            //await clearAccelerometer();
            //await clearGyroscope();
            //await clearMagnetometer();
            await Task.WhenAll(new Task[] {
                clearAccelerometer(),
                clearGyroscope(),
                clearMagnetometer(),
            });
        }
        public async Task renderAcceletometer()
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                modelAccelerometer.render();
            });
        }
        public async Task renderGyroscope()
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                modelGyroscope.render();
            });
        }
        public async Task renderMagnetometer()
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                modelMagnetometer.render();
            });
        }
        public async void onRender(object sender, EventArgs e)
        {
            await Task.WhenAll(new Task[]
            {
                renderAcceletometer(),
                renderGyroscope(),
                renderMagnetometer(),
            });
        }
    }
}
