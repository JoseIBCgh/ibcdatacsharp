using ibcdatacsharp.UI.ToolBar.Enums;
using ibcdatacsharp.UI.ToolBar;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ibcdatacsharp.UI.DeviceList;
using ibcdatacsharp.DeviceList.TreeClasses;
using System.Diagnostics;
using ibcdatacsharp.UI.Graphs.Sagital;
using ibcdatacsharp.UI.Graphs.TwoIMU;
using ibcdatacsharp.UI.Common;

namespace ibcdatacsharp.UI.Angles
{
    public class Angles
    {
        private MainWindow mainWindow;
        private VirtualToolBar virtualToolBar;
        private DeviceList.DeviceList deviceList;

        private Dictionary<byte, int> indices;
        private int NUM_PACK = 4;
        private int IMUsReceived = 0;

        private int TOTAL_SENSORS = 2;
        private Quaternion[] mQ_sensors_raw;
        private Quaternion[] mQ_sensors_ref;
        private Quaternion[,] mQ_sensors_raw_list;

        private Quaternion[] mQ_delta;
        private Quaternion[] mQ_local;

        public Quaternion q_v { get; set; }

        private float fakets = 0f;
        private int frame = 0;

        AngleGraphX angleX;
        AngleGraphY angleY;
        AngleGraphZ angleZ;

        const int Xindex = 0;
        const int Yindex = 1;
        const int Zindex = 2;
        public Angles()
        {
            mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.initialized += (sender, args) =>
            {
                virtualToolBar = mainWindow.virtualToolBar;
            };
            if (mainWindow.deviceList.Content == null)
            {
                mainWindow.deviceList.Navigated += (sender, args) =>
                {
                    deviceList = mainWindow.deviceList.Content as DeviceList.DeviceList;
                };
            }
            else
            {
               deviceList = mainWindow.deviceList.Content as DeviceList.DeviceList;
            }
            if (mainWindow.angleX.Content == null)
            {
                mainWindow.angleX.Navigated += (s, e) =>
                {
                    angleX = mainWindow.angleX.Content as AngleGraphX;
                };
            }
            else
            {
                angleX = mainWindow.angleX.Content as AngleGraphX; ;
            }
            if (mainWindow.angleY.Content == null)
            {
                mainWindow.angleY.Navigated += (s, e) =>
                {
                    angleY = mainWindow.angleY.Content as AngleGraphY;
                };
            }
            else
            {
                angleY = mainWindow.angleY.Content as AngleGraphY;
            }
            if (mainWindow.angleZ.Content == null)
            {
                mainWindow.angleZ.Navigated += (s, e) =>
                {
                    angleZ = mainWindow.angleZ.Content as AngleGraphZ;
                };
            }
            else
            {
                angleZ = mainWindow.angleZ.Content as AngleGraphZ;
            }
            quaternionCalcsConnect();
        }
        public void quaternionCalcsConnect()
        {
            Quaternion idenQuat = Quaternion.Identity;
            mQ_sensors_raw_list = new Quaternion[NUM_PACK, TOTAL_SENSORS];

            mQ_sensors_raw = new Quaternion[TOTAL_SENSORS];
            mQ_sensors_ref = new Quaternion[TOTAL_SENSORS];
            mQ_delta = new Quaternion[TOTAL_SENSORS];
            mQ_local = new Quaternion[TOTAL_SENSORS];

            for (int i = 0; i < TOTAL_SENSORS; i++)
            {
                for (int j = 0; j < NUM_PACK; j++)
                {
                    mQ_sensors_raw_list[j, i] = idenQuat;
                }
                mQ_sensors_raw[i] = idenQuat;
                mQ_sensors_ref[i] = idenQuat;
                mQ_delta[i] = idenQuat;
                mQ_local[i] = idenQuat;
            }
        }
        public void initIMUs()
        {

            byte handlerFromMAC(string mac)
            {
                string handler = mainWindow.devices_list.Where(z => z.Value.Id == mac).FirstOrDefault().Key;
                return byte.Parse(handler);
            }

            indices = new Dictionary<byte, int>();
            List<IMUInfo> imus = deviceList.IMUsUsed;
            Trace.WriteLine("Lista UI");
            foreach (IMUInfo imu in imus)
            {
                Trace.WriteLine("A = " + imu.A + ", MAC = " + imu.address);
            }
            Trace.WriteLine("Funcion de sagital angles");
            for (int i = 0; i < TOTAL_SENSORS; i++)
            {
                string i_str = i.ToString();
                IMUInfo? imu = imus.Where(imu => imu.A == i_str).FirstOrDefault();
                if (imu != null)
                {
                    byte handler = handlerFromMAC(imu.address);
                    indices[handler] = i;
                    Trace.WriteLine("A = " + i + " MAC = " + imu.address + " , handler = " + handler);
                }
                else
                {
                    Trace.WriteLine("A = " + i + " no encontrado");
                }
            }
            angleX.initCapture();
            angleY.initCapture();
            angleZ.initCapture();
            quaternionCalcsConnect();
        }
        private int handlerToIndex(byte deviceHandler)
        {
            return indices[deviceHandler];
        }
        public void processSerialData(byte deviceHandler, WisewalkSDK.WisewalkData data)
        {
            if (indices == null)
            {
                return;
            }
            int index = handlerToIndex(deviceHandler);
            for (int i = 0; i < NUM_PACK; i++)
            {
                mQ_sensors_raw_list[i, index] = new Quaternion((float)data.Quat[i].X, (float)data.Quat[i].Y, (float)data.Quat[i].Z, (float)data.Quat[i].W);
            }
            IMUsReceived++;
            if (IMUsReceived % TOTAL_SENSORS == 0)
            {
                float[] angleXData = new float[NUM_PACK];
                float[] angleYData = new float[NUM_PACK];
                float[] angleZData = new float[NUM_PACK];
                for (int i = 0; i < NUM_PACK; i++)
                {
                    for (int s = 0; s < TOTAL_SENSORS; s++)
                    {
                        mQ_sensors_raw[s] = mQ_sensors_raw_list[i, s];
                    }
                    float[] eulerAngles = new float[3];
                    updateSegmentsAndJoints(mQ_sensors_raw, ref eulerAngles);
                    angleXData[i] = (float)eulerAngles[Xindex];
                    angleYData[i] = (float)eulerAngles[Yindex];
                    angleZData[i] = (float)eulerAngles[Zindex];
                }
                angleX.drawData(angleXData);
                angleY.drawData(angleYData);
                angleZ.drawData(angleZData);
                if (virtualToolBar.recordState == RecordState.Recording)
                {
                    float offsetX = (float)angleX.model.offset;
                    float offsetY = (float)angleY.model.offset;
                    float offsetZ = (float)angleZ.model.offset;
                    string dataline = "";
                    for (int i = 0; i < NUM_PACK; i++)
                    {
                        dataline += "1 " + (fakets + 0.01 * i).ToString("F2", CultureInfo.InvariantCulture) + " " + (frame + i).ToString() + " " +
                            (angleXData[i] + offsetX).ToString("F2", CultureInfo.InvariantCulture) + " " + (angleYData[i] + offsetY).ToString("F2", CultureInfo.InvariantCulture) + " " +
                            (angleZData[i] + offsetZ).ToString("F2", CultureInfo.InvariantCulture) + "\n";
                    }
                    frame += NUM_PACK;
                    fakets += NUM_PACK * 0.01f;
                    mainWindow.fileSaver.appendCSVManual(dataline);
                }
            }
        }
        public void calculateMounting()
        {
            Array.Copy(mQ_sensors_raw, mQ_sensors_ref, TOTAL_SENSORS);
            for (int iSen = 0; iSen < mQ_sensors_ref.Length; ++iSen)
            {
                mQ_sensors_ref[iSen] = Quaternion.Normalize(mQ_sensors_ref[iSen]);
            }
            Task.Run(() =>
            {
                MessageBox.Show("Sensor Mounting Done", "Info", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.Yes);
            });
        }
        public void updateSegmentsAndJoints(Quaternion[] mQ_sensors_raw, ref float[] eulerAngles)
        {
            float sqrt_2 = (float)Math.Sqrt(2);
            Quaternion q_isb = new Quaternion(sqrt_2, sqrt_2, 0, 0);
            Quaternion[] mQ_delta = new Quaternion[TOTAL_SENSORS];
            for (int i = 0; i < TOTAL_SENSORS; i++)
            {
                mQ_delta[i] = mQ_sensors_raw[i] * Quaternion.Conjugate(mQ_sensors_ref[i]);
            }
            Quaternion[] mQ_locals = new Quaternion[TOTAL_SENSORS];
            for(int i = 0; i < TOTAL_SENSORS; i++)
            {
                Quaternion q_v = mQ_sensors_raw[i];
                mQ_locals[i] = Quaternion.Conjugate(q_v) * (mQ_delta[i] * q_v);
            }
            Quaternion[] mQ_segments = new Quaternion[TOTAL_SENSORS];
            for(int i = 0; i < TOTAL_SENSORS; ++i)
            {
                mQ_segments[i] = Quaternion.Conjugate(q_isb) * (mQ_locals[i] * q_isb);
            }
            Quaternion Q_proximal, Q_distal;
            Q_proximal = mQ_segments[0];
            Q_distal = mQ_segments[1];
            Quaternion Qjoint = Quaternion.Conjugate(Q_proximal) * Q_distal;
            float w = Qjoint.W;
            float x = Qjoint.X;
            float y = Qjoint.Y;
            float z = Qjoint.Z;
            float w2 = w * w;
            float x2 = x * x;
            float y2 = y * y;
            float z2 = z * z;
            float alpha = (float)Math.Atan(2 * (x * y + w * z) / (w2 - x2 + y2 - z2));
            float beta = (float)Math.Asin(-2 * (y * z - w * x));
            float gamma = (float)Math.Atan(2 * (x * z + w * y) / (w2 - x2 - y2 + z2));
            eulerAngles[0] = Helpers.ToDegrees(alpha);
            eulerAngles[1] = Helpers.ToDegrees(beta);
            eulerAngles[2] = Helpers.ToDegrees(gamma);
        }
    }
}
