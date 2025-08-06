using VSP_88D_CS.Models.Device;
namespace VSP_88D_CS.Common.Device
{

    public interface IVSIOController
    {
        public bool IsSensorOn(eDI _input, int nTimer = 0);
        public bool IsSensorOff(eDI _input, int nTimer = 0);

        public bool IsSolenoidOn(eDO _output, int nTimer = 0);
        public bool IsSolenoidOff(eDO _output, int nTimer = 0);

        public void SingleSolOnOff(eDO _output, bool bOn);
        public void ToggleSingleSol(eDO _output, bool bOn);

        public void ResetSolenoidTimer(eDO _output);
    }

    public class VSIOController: IVSIOController
    {
        //private readonly DigitalIOCtrlManager _digitalIOController;
        //private readonly AnalogIOCtrlManager _analogIOController;
        //private readonly List<VSIOSettingItem> _iODatas;

        //public VSIOController(DigitalIOCtrlManager digitalIOController, List<VSIOSettingItem> IOData, AnalogIOCtrlManager analogIOController)
        //{
        //    _digitalIOController = digitalIOController;
        //    _analogIOController = analogIOController;
        //    _iODatas= IOData;
        //}
        
        private string _getWireNameIn(eDI _input)
        {
            //var item = _iODatas.FirstOrDefault(x => x.EmName == _input.ToString());
            //return item != null ? item.WireName : "";
            return string.Empty;
        }
        private string _getWireNameOut(eDO _output)
        {
            //var item = _iODatas.FirstOrDefault(x => x.EmName == _output.ToString());
            //return item != null ? item.WireName : "";
            return string.Empty;
        }

        public bool IsSensorOn(eDI _input, int nTimer = 0)
        {
            //return _digitalIOController.DigitalIODataDictionary[_getWireNameIn(_input)].Value == true; 
            return true;
        }
        public bool IsSensorOff(eDI _input, int nTimer = 0)
        {
            //return _digitalIOController.DigitalIODataDictionary[_getWireNameIn(_input)].Value == false;
            return true;
        }

        public bool IsSolenoidOn(eDO _output, int nTimer = 0)
        {
            //return _digitalIOController.DigitalIODataDictionary[_getWireNameOut(_output)].Value == true;
            return true;
        }
        public bool IsSolenoidOff(eDO _output, int nTimer = 0)
        {
            //return _digitalIOController.DigitalIODataDictionary[_getWireNameOut(_output)].Value == false;
            return true;
        }

        public void SingleSolOnOff(eDO _output, bool bOn)
        {
            //_digitalIOController.SetOutput(_getWireNameOut(_output), bOn);
        }
        public void ToggleSingleSol(eDO _output, bool bOn)
        {
            throw new NotImplementedException();
        }

        public void ResetSolenoidTimer(eDO _output)
        {
            throw new NotImplementedException();
        }

    }
}
