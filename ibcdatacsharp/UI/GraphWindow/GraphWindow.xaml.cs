using DirectShowLib.DES;
using ibcdatacsharp.UI.Device;
using ibcdatacsharp.UI.DeviceList;
using OpenCvSharp.Flann;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using WisewalkSDK;
using Quaternion = System.Numerics.Quaternion;


using ibcdatacsharp.UI.AngleGraph;
using DirectShowLib.BDA;

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
        public FileSaver.FileSaver v;
        public string frame2;
        public int sr;
        int timespan;
        string ts;
        int frame = 0;

        Quaternion q1 = new Quaternion();
        Quaternion refq = new Quaternion();
        Quaternion q_lower = new Quaternion();
        Quaternion q_upper = new Quaternion();

        int anglequat = 0;
        int mac1 = 0;
        int mac2 = 0;

        double alpha = 0.0d;
        double delta = 0.0d;
        double phi = 0.0d;

        float a1 = 0.0f;
        float a2 = 0.0f;
        float a3 = 0.0f;

        MainWindow mainWindow;
        AngleGraph.AngleGraph angleGraph;

        public GraphWindow()
        {
            InitializeComponent();
            angleGraph = new AngleGraph.AngleGraph();
            initModels();
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            v = mainWindow.fileSaver;
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

        public static Vector3 ToEulerAngles(Quaternion q)
        {
            Vector3 angles = new();

            // roll / x
            double sinr_cosp = 2 * (q.W * q.X + q.Y * q.Z);
            double cosr_cosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
            angles.X = (float)Math.Atan2(sinr_cosp, cosr_cosp);

            // pitch / y
            double sinp = 2 * (q.W * q.Y - q.Z * q.X);
            if (Math.Abs(sinp) >= 1)
            {
                angles.Y = (float)Math.CopySign(Math.PI / 2, sinp);
            }
            else
            {
                angles.Y = (float)Math.Asin(sinp);
            }

            // yaw / z
            double siny_cosp = 2 * (q.W * q.Z + q.X * q.Y);
            double cosy_cosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
            angles.Z = (float)Math.Atan2(siny_cosp, cosy_cosp);

            return angles;
        }

        /// <summary>
        /// Convert radians to degrees.
        /// </summary>
        public static float ToDegrees(float radians)
        {
            return radians * 180f / (float)Math.PI;
        }

        //Callback para recoger datas del IMU
        public async void Api_dataReceived( byte deviceHandler, WisewalkSDK.WisewalkData data)
        {
            
            devices_list[deviceHandler.ToString()].NPackets++;
            devices_list[deviceHandler.ToString()].Stream = true;
            
            // N. Packets + N. Frames received from device handler
            frame2 = counter[0].ToString() + " / " + (devices_list[deviceHandler.ToString()].NPackets * devices_list[deviceHandler.ToString()].HeaderInfo.sampleFrame).ToString();


            int sr = devices_list[deviceHandler.ToString()].sampleRate;
            int timespan = (int)((devices_list[deviceHandler.ToString()].NPackets * 4) * ((1 / (float)devices_list[deviceHandler.ToString()].sampleRate) * 1000));
            
            float tsA = ((float)(devices_list[deviceHandler.ToString()].NPackets * 4) * ((1 / (float)devices_list[deviceHandler.ToString()].sampleRate) * 1000) / 1000) ;

            ts = timespan.ToString();
            frame += 1;

            // refq = 0.823125, 0.000423, 0.009129, -0.567773


            /**
             * 
             *  
                X1 = Q1*X*conj(Q1);
                Y1 = Q1*Y*conj(Q1);
                Z1 = Q1*Z*conj(Q1);

                X2 = Q2*X*conj(Q2);
                Y2 = Q2*Y*conj(Q2);
                Z2 = Q2*Z*conj(Q2);


                DiffAngleX = acos(dot(X1,X2));
                DiffAngleY = acos(dot(Y1,Y2));
                DiffAngleZ = acos(dot(Z1,Z2));
             */

            /*
            
            Q1 = C0:97:3C:F2:DA:40
            Q2 = D8:D3:A5:0A:4F:BC

             */


            //ref_quaternion:
            //0.823125, 0.000423, 0.009129, -0.567773 Z para arriba
            // 0.176144, -0.189621, 0.693031, -0.672846 Y para arriba
            // 0.516224, 0.4542, 0.55528, -0.467841 X para arriba

            refq.W = 0.176144f;
            refq.X = -0.189621f;
            refq.Y = 0.693031f;
            refq.Z = -0.672846f;


            Matrix4x4 refmat = Matrix4x4.CreateFromQuaternion(refq);


            if (data.Imu.Count > 0 ) {
            //    Trace.WriteLine("Data: " + " " + devices_list[deviceHandler.ToString()].Id.ToString() + " " + tsA.ToString("F3") + " " + devices_list[deviceHandler.ToString()].NPackets.ToString() + " "
            //+ data.Quat[0].W.ToString() + ", " + data.Quat[0].X.ToString() + ", " + data.Quat[0].Y.ToString() + ", " + data.Quat[0].Z.ToString());

                if (devices_list[deviceHandler.ToString()].Id.ToString() == "C0:97:3C:F2:DA:40"  )  
                {
                    q_lower.W = (float)data.Quat[0].W;
                    q_lower.X = (float)data.Quat[0].X;
                    q_lower.Y = (float)data.Quat[0].Y;
                    q_lower.Z = (float)data.Quat[0].Z;
                    anglequat++;
                    
                }
                
                else if ( devices_list[deviceHandler.ToString()].Id.ToString() == "D8:D3:A5:0A:4F:BC" )
                {
                    q_upper.W = (float)data.Quat[0].W;
                    q_upper.X = (float)data.Quat[0].X;
                    q_upper.Y = (float)data.Quat[0].Y;
                    q_upper.Z = (float)data.Quat[0].Z;
                    anglequat++;
                }
                


               if (anglequat % 2 == 0)
                {
                    Vector3 angle_low = new();
                    Vector3 angle_up = new();
                    Vector3 angle_ref = new();
                    angle_low = ToEulerAngles(q_lower);
                    angle_up = ToEulerAngles(q_upper);
                    angle_ref = ToEulerAngles(refq);
                    a1 = angle_low.X - angle_up.X + angle_ref.X;
                    a2 = angle_low.Y - angle_up.Y + angle_ref.Y;
                    a3 = angle_low.Z - angle_up.Z + angle_ref.Z;
                    a1 = ToDegrees(a1);
                    a2 = ToDegrees(a2);
                    a3 = ToDegrees(a3);

                    Trace.WriteLine(":::::: ANGLE JOINT: " + a1.ToString() + " " + a2.ToString() + " " + a3.ToString());

                    Matrix4x4 m_lower = Matrix4x4.CreateFromQuaternion(q_lower);
                    Matrix4x4 m_upper = Matrix4x4.CreateFromQuaternion(q_upper);

                    Matrix4x4 R = Matrix4x4.Multiply(m_lower, m_upper);

                    double beta = Math.Atan2(R.M32 , Math.Sqrt( Math.Pow(R.M12,2) * Math.Pow(R.M22, 2) ) );
                    double delta = Math.Atan2(-(R.M12 / Math.Cos(beta)), R.M22 / Math.Cos(beta));
                    double phi = Math.Atan2(-(R.M31 / Math.Cos(beta)), R.M33 / Math.Cos(beta));

                    if (beta >= 90.0 && beta < 91.0)
                    {
                        beta = 90.0d;
                        delta = 0.0d;
                        phi = Math.Atan2(R.M13, R.M23);
                        
                    }

                    //Trace.WriteLine("Beta: " + ToDegrees((float) beta).ToString() + " Delta:" + ToDegrees((float) delta).ToString() + 
                    //    " Phi: " + ToDegrees((float) phi).ToString());                   
                }
            }

            // Only a IMU

            if (devices_list.Count == 1)
            {
                await Task.WhenAll(new Task[] {
                    updateAccelerometer(frame, data.Imu[0].acc_x, data.Imu[0].acc_y, data.Imu[0].acc_z),
                    updateMagnetometer(frame, data.Imu[0].gyro_x, data.Imu[0].gyro_y, data.Imu[0].gyro_z),
                    updateGyroscope(frame, data.Imu[0].mag_x, data.Imu[0].mag_y, data.Imu[0].mag_z),
                    renderAcceletometer(),
                    renderGyroscope(),
                    renderMagnetometer(),
                     v.appendCSV(frame.ToString(), tsA.ToString("F3"), data.Imu[0].acc_x.ToString("F3"), data.Imu[0].acc_y.ToString("F3"), data.Imu[0].acc_z.ToString("F3"),
                    data.Imu[0].gyro_x.ToString("F3") ,  data.Imu[0].gyro_y.ToString("F3") , data.Imu[0].gyro_z.ToString("F3") ,
                    data.Imu[0].mag_x.ToString("F3") , data.Imu[0].mag_y.ToString("F3") , data.Imu[0].mag_z.ToString("F3")
                ),

            });
            }
            else if (devices_list.Count == 2)
            {
                await Task.WhenAll(new Task[] { 
                
                    angleGraph.updateX(frame, a1),
                
                   
                    angleGraph.renderX()
                  
                
                });





            }
       
            
           
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
