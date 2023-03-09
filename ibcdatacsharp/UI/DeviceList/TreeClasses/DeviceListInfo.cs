using ibcdatacsharp.Common;
using System.Collections.ObjectModel;

namespace ibcdatacsharp.DeviceList.TreeClasses
{
    // Guarda una lista de IMUs una lista de Camaras y una lista de Insoles
    public class DeviceListInfo: PropertyNotifier
    {
        private ObservableCollection<IMUInfo> _IMUs;
        public ObservableCollection<IMUInfo> IMUs
        {
            get { return _IMUs; }
            set { 
                _IMUs = value;
                OnPropertyChanged();
            }
        }
        public void addIMU(IMUInfo imu)
        {
            _IMUs.Add(imu);
            OnPropertyChanged(nameof(IMUs));
        }
        private ObservableCollection<CameraInfo> _cameras;
        public ObservableCollection<CameraInfo> cameras
        {
            get { return _cameras; }
            set { 
                _cameras = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<InsolesInfo> _insoles;
        public ObservableCollection<InsolesInfo> insoles
        {
            get { return _insoles; }
            set { 
                _insoles = value;
                OnPropertyChanged();
            }
        }
        public void checkJAUpdate()
        {
            foreach (var item in IMUs)
            {
                item.checkJAUpdate();
            }
        }
        public DeviceListInfo()
        {
            _IMUs = new ObservableCollection<IMUInfo>();
            _cameras = new ObservableCollection<CameraInfo>();
            _insoles = new ObservableCollection<InsolesInfo>();
        }
    }
}
