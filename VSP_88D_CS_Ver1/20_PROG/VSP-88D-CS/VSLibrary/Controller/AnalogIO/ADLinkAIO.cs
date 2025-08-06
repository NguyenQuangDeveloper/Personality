using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace VSLibrary.Controller.AnalogIO
{
    /// <summary>
    /// Analog I/O control class for the ADLink PCI-9112 board.
    /// (The ADLink PCI-9112 typically provides 8 AI channels and 2 AO channels.)
    /// </summary>
    public class ADLinkAIO : AIOBase
    {
        private ushort _cardNumber;
        private Dictionary<string, IAnalogIOData> _analogIOData = new Dictionary<string, IAnalogIOData>();
        private Dictionary<string, int> _iocount;
        private bool _isInitialized = false;  // Indicates whether the board has been initialized

        /// <summary>
        /// Constructor — initializes the board and sets up channels.
        /// </summary>
        public ADLinkAIO(Dictionary<string, int> count)
        {
            _iocount = count;

            if (IsInitialized())
            {
                OpenDevice();
                _isInitialized = true;
            }

            // IoType();
        }

        /// <summary>
        /// Generates 2 dummy entries in _analogIOData for debugging or initial verification.
        /// </summary>
        private void test()
        {
            for (int i = 0; i < 2; i++)
            {
                AIOData strdata = new AIOData
                {
                    Controller = this,
                    ControllerType = ControllerType.AIO_Adlink,
                    IOType = IOType.OUTPut,
                    WireName = $"AO{i + _iocount["AOutput"]:X3}",    // e.g.: AO000, AO001
                    StrdataName = "Analog Test",                     // Display name
                    ModuleName = "",                                  // Module name (if needed)
                    ModuleNumber = (short)i,
                    Channel = i,                                      // Channel index
                    Range = i                                         // Range placeholder
                };

                _analogIOData.Add(strdata.WireName, strdata);
            }

            _iocount["AOutput"] = _iocount["AOutput"] + 8;
        }

        /// <summary>
        /// Registers the ADLink PCI-9112 card. Returns true if successful.
        /// </summary>
        private bool IsInitialized()
        {
            try
            {
                short ret = DASK.Register_Card(DASK.PCI_9112, 0);
                if (ret < 0)
                {
                    Console.WriteLine($"Failed to register ADLink PCI-9112 card, error code: {ret}");
                    return false;
                }
                _cardNumber = (ushort)ret;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Configures analog input (8 channels) and output (2 channels) on the board.
        /// </summary>
        public void OpenDevice()
        {
            // Configure 8 analog input channels
            for (int i = 0; i < 8; i++)
            {
                AIOData inputData = new AIOData
                {
                    Controller = this,
                    ControllerType = ControllerType.AIO_Adlink,
                    IOType = IOType.InPut,
                    WireName = $"AI{i + _iocount["AInput"]:X3}",
                    StrdataName = "Analog Input",
                    ModuleNumber = 0,
                    Channel = i,
                    Range = i == 2 ? DASK.AD_U_10_V : DASK.AD_U_5_V  // Per-channel range setting
                };

                int configRet = DASK.AI_9112_Config(_cardNumber, 0);
                if (configRet != DASK.NoError)
                {
                    throw new Exception($"Failed to configure AI channel, error code: {configRet}");
                }

                _analogIOData.Add(inputData.WireName, inputData);
            }
            _iocount["AInput"] = _iocount["AInput"] + 8;

            // Configure 2 analog output channels
            for (int i = 0; i < 2; i++)
            {
                AIOData outputData = new AIOData
                {
                    Controller = this,
                    ControllerType = ControllerType.AIO_Adlink,
                    IOType = IOType.OUTPut,
                    WireName = $"AO{i + _iocount["AOutput"]:X3}",
                    StrdataName = "Analog Output",
                    ModuleNumber = 0,
                    Channel = i,
                    Range = 0  // 0→0–10V (adjust as needed)
                };

                double refVoltage = 10.0;
                int ret = DASK.AO_9112_Config(_cardNumber, (ushort)i, refVoltage);
                if (ret != DASK.NoError)
                {
                    throw new Exception($"Failed to configure AO channel {i}, error code: {ret}");
                }

                _analogIOData.Add(outputData.WireName, outputData);
            }
            _iocount["AOutput"] = _iocount["AOutput"] + 2;

            Console.WriteLine("ADLink PCI-9112 board configuration completed");
        }

        /// <summary>
        /// Reads the voltage from the specified analog input channel.
        /// </summary>
        /// <param name="aioData">Channel data object</param>
        /// <returns>Measured voltage in volts</returns>
        public override double ReadChannelValue(IAnalogIOData aioData)
        {
            if (!_analogIOData.ContainsKey(aioData.WireName))
            {
                throw new ArgumentException("Specified channel does not exist.");
            }

            int channel = _analogIOData[aioData.WireName].Channel;
            ushort adRange = (ushort)_analogIOData[aioData.WireName].Range;
            ushort rawValue;
            int ret = DASK.AI_ReadChannel(_cardNumber, (ushort)channel, adRange, out rawValue);
            if (ret != DASK.NoError)
            {
                throw new Exception($"Failed to read AI channel {channel}, error code: {ret}");
            }

            double voltage;
            ret = DASK.AI_VoltScale(_cardNumber, adRange, rawValue, out voltage);
            if (ret != DASK.NoError)
            {
                throw new Exception($"Failed to convert raw value to voltage for channel {channel}, error code: {ret}");
            }

            return voltage;
        }

        /// <summary>
        /// Writes the specified voltage to an analog output channel.
        /// Returns true if the underlying AO_WriteChannel call returns 1.
        /// </summary>
        /// <param name="aioData">Channel data object</param>
        /// <param name="value">Voltage to output (V)</param>
        /// <returns>True if successful; otherwise false</returns>
        public override bool WriteChannelValue(IAnalogIOData aioData, double value)
        {
            if (!_analogIOData.ContainsKey(aioData.WireName))
            {
                throw new ArgumentException("Specified channel does not exist.");
            }

            int channel = _analogIOData[aioData.WireName].Channel;
            short outValue = (short)value;  // Simplified scaling

            int ret = DASK.AO_WriteChannel(_cardNumber, (ushort)channel, outValue);
            if (ret == DASK.NoError)
            {
                aioData.AValue = value;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Reads all analog input channels and updates internal values.
        /// Only input channels are processed; outputs are not read.
        /// </summary>
        public override void UpdateAllChannelValues()
        {
            if (!_isInitialized) return;

            foreach (var pair in _analogIOData)
            {
                var data = pair.Value;
                if (data.IOType != IOType.InPut) continue;

                ushort rawValue;
                ushort adRange = (ushort)data.Range;
                int ret = DASK.AI_ReadChannel(_cardNumber, (ushort)data.Channel, adRange, out rawValue);
                if (ret != DASK.NoError)
                {
                    Console.WriteLine($"[ADLinkAIO] Failed to read channel {data.Channel}, code: {ret}");
                    continue;
                }

                double voltage;
                ret = DASK.AI_VoltScale(_cardNumber, adRange, rawValue, out voltage);
                if (ret != DASK.NoError)
                {
                    Console.WriteLine($"[ADLinkAIO] Failed to scale voltage for channel {data.Channel}, code: {ret}");
                    continue;
                }

                data.AValue = voltage;
            }
        }

        /// <summary>
        /// Returns the dictionary of analog I/O data objects.
        /// </summary>
        public override Dictionary<string, IAnalogIOData> GetAnalogIODataDictionary()
        {
            return _analogIOData;
        }

        /// <summary>
        /// Releases card resources when the application exits.
        /// </summary>
        public override void AnalogIOCtrlDispose()
        {
            DASK.Release_Card(_cardNumber);
        }
    }
}
