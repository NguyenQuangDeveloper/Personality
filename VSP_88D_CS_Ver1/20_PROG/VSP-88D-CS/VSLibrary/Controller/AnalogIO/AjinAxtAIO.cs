using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Threading.Channels;

namespace VSLibrary.Controller.AnalogIO
{
    /// <summary>
    /// Base class for managing data and controlling read/write operations
    /// on an Ajin analog I/O board.
    /// Utilizes external libraries (e.g., CAxtLib, CAxtAIO) for board initialization,
    /// channel configuration, range settings, and data I/O.
    /// </summary>
    public class AjinAxtAIO : AIOBase
    {
        /// <summary>
        /// Dictionary storing analog I/O data objects keyed by wire name.
        /// </summary>
        private Dictionary<string, IAnalogIOData> _analogIOData = new Dictionary<string, IAnalogIOData>();

        private bool _isInitialized = false;

        /// <summary>
        /// (Optional) List of analog I/O data loaded from DB or other sources.
        /// </summary>
        //private readonly List<IAnalogIOData> _dBdata;

        private Dictionary<string, int> _iocount;

        /// <summary>
        /// Constructor for Ajin AIO controller.
        /// - Sets manufacturer to Ajin and stores provided channel counts in _iocount.
        /// - Registers DLL paths, initializes the library, and configures board channels.
        /// </summary>
        /// <param name="count">Dictionary containing channel counts (e.g., keys "input", "output").</param>
        public AjinAxtAIO(Dictionary<string, int> count)
        {
            _iocount = count;
            // Initialize board and open device if successful
            if (IsInitialized())
            {
                OpenDevice();
                _isInitialized = true;
            }
            // Generate IoType data (for debugging; disable in production)
            test();
        }

        /// <summary>
        /// Verifies existence of specified wire name in the dictionary.
        /// Prints an error message if not found.
        /// </summary>
        /// <param name="wireName">Wire name to check.</param>
        /// <returns>True if exists and initialized; otherwise false.</returns>
        private bool CheckDic(string wireName)
        {
            if (!_analogIOData.ContainsKey(wireName))
            {
                Console.WriteLine($"[Error] Analog IO data for {wireName} does not exist.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Generates 2 dummy entries in _analogIOData for testing purposes.
        /// Used mainly for debugging or initial verification.
        /// </summary>
        private void test()
        {
            for (int i = 0; i < 2; i++)
            {
                AIOData dummy = new AIOData
                {
                    Controller = this,
                    ControllerType = ControllerType.AIO_AjinAXT,
                    IOType = IOType.OUTPut,
                    WireName = $"AO{i + _iocount["AOutput"]:X3}",  // e.g.: AO000, AO001
                    StrdataName = "Analog Test",
                    ModuleName = string.Empty,
                    ModuleNumber = (short)i,
                    Channel = i,
                    Range = i  // Placeholder range for IoType
                };
                _analogIOData.Add(dummy.WireName, dummy);
            }
            // Increments output count by 8 (note: only 2 entries created)
            _iocount["AOutput"] += 8;
        }

        /// <summary>
        /// Initializes the Ajin AIO library and board.
        /// Calls OpenDevice upon successful initialization.
        /// </summary>
        /// <returns>True if initialization succeeded; otherwise false.</returns>
        private bool IsInitialized()
        {
            // CAxtAIO.InitializeAIO returns non-zero on success
            if (CAxtAIO.InitializeAIO() == 0)
                return false;

            // Configure channels immediately after successful init
            OpenDevice();
            return true;
        }

        /// <summary>
        /// Configures analog output and input channels on the board,
        /// adds each channel's data to _analogIOData, and updates _iocount.
        /// </summary>
        public void OpenDevice()
        {
            // Configure analog output channels
            short outputCount = CAxtAIO.AIOget_channel_number_dac();
            for (short i = 0; i < outputCount; i++)
            {
                short modNo = CAxtAIO.AIOchannelno_2_moduleno_dac(i);
                ushort modId = CAxtAIO.AIOget_moduleid(modNo);
                AIOData outData = new AIOData
                {
                    Controller = this,
                    ControllerType = ControllerType.AIO_AjinAXT,
                    IOType = IOType.OUTPut,
                    WireName = $"AO{i + _iocount["AOutput"]:X3}",
                    StrdataName = "Analog Output Test",
                    ModuleName = (AXT_FUNC_MODULE)modId == AXT_FUNC_MODULE.AXT_SIO_AO4RB ? "SIO_AO4RB" : "SIO_AO8H",
                    ModuleNumber = modNo,
                    Channel = i,
                    Range = 0
                };
                _analogIOData.Add(outData.WireName, outData);
            }

            // Configure analog input channels
            short inputCount = CAxtAIO.AIOget_channel_number_adc();
            for (short i = 0; i < inputCount; i++)
            {
                short modNo = CAxtAIO.AIOchannelno_2_moduleno_adc(i);
                ushort modId = CAxtAIO.AIOget_moduleid(modNo);
                AIOData inData = new AIOData
                {
                    Controller = this,
                    ControllerType = ControllerType.AIO_AjinAXT,
                    IOType = IOType.InPut,
                    WireName = $"AI{i + _iocount["AInput"]:X3}",
                    StrdataName = "Analog Input Test",
                    ModuleName = (AXT_FUNC_MODULE)modId == AXT_FUNC_MODULE.AXT_SIO_AI4RB ? "SIO_AI4RB" : "SIO_AI16H",
                    ModuleNumber = modNo,
                    Channel = i,
                    Range = 0
                };
                _analogIOData.Add(inData.WireName, inData);
            }

            _iocount["AInput"] += inputCount;
            _iocount["AOutput"] += outputCount;
        }

        /// <summary>
        /// Sets the voltage range for a specified channel via the Ajin library.
        /// </summary>
        /// <param name="channel">Channel index</param>
        /// <param name="min">Minimum voltage</param>
        /// <param name="max">Maximum voltage</param>
        /// <param name="isOutput">True for output channel, false for input</param>
        private void SetRange(short channel, int min, int max, bool isOutput)
        {
            try
            {
                if (isOutput)
                    CAxtAIO.AIOset_range_dac(channel, min, max);
                else
                    CAxtAIO.AIOset_range_adc(channel, min, max);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to set range for channel {channel}.", ex);
            }
        }

        /// <summary>
        /// Sets channel range based on a shared AioRange code (0-3):
        /// 0=0-10V, 1=0-5V, 2=-10-10V, 3=-5-5V.
        /// </summary>
        /// <param name="channel">Channel index</param>
        /// <param name="aioRange">Range code 0-3</param>
        /// <param name="isOutput">True for output, false for input</param>
        private void SetRangeByAioRange(short channel, int aioRange, bool isOutput)
        {
            if (aioRange < 0 || aioRange > 3)
                throw new ArgumentOutOfRangeException(nameof(aioRange), "AioRange must be between 0 and 3.");

            switch (aioRange)
            {
                case 0: SetRange(channel, 0, 10, isOutput); break;
                case 1: SetRange(channel, 0, 5, isOutput); break;
                case 2: SetRange(channel, -10, 10, isOutput); break;
                case 3: SetRange(channel, -5, 5, isOutput); break;
            }
        }

        /// <summary>
        /// Configures range for a specific AIO data entry by key.
        /// </summary>
        /// <param name="unusedWire">Wire name (currently unused)</param>
        /// <param name="key">Key in _analogIOData</param>
        /// <param name="isOutput">True for output, false for input</param>
        public void SetAioConfig(string unusedWire, string key, bool isOutput)
        {
            if (!_analogIOData.ContainsKey(key))
                throw new ArgumentException("Specified key does not exist in AIO data.");

            int channel = _analogIOData[key].Channel;
            SetRangeByAioRange((short)channel, _analogIOData[key].Range, isOutput);
        }

        /// <summary>
        /// Configures range for all channels in the dictionary.
        /// </summary>
        /// <param name="unusedWire">Wire name (currently unused)</param>
        /// <param name="isOutput">True for output, false for input</param>
        public void SetAllAioConfig(int unusedWire, bool isOutput)
        {
            foreach (var data in _analogIOData.Values)
                SetRangeByAioRange((short)data.Channel, data.Range, isOutput);
        }

        /// <summary>
        /// Reads the value of a specified analog channel:
        /// uses AIOread_dac for outputs and AIOread_one_volt_adc for inputs.
        /// </summary>
        /// <param name="aioData">IAnalogIOData instance</param>
        /// <returns>Measured analog value</returns>
        public override double ReadChannelValue(IAnalogIOData aioData)
        {
            if (!_analogIOData.ContainsKey(aioData.WireName))
                throw new ArgumentException("Specified key does not exist in AIO data.");

            int channel = _analogIOData[aioData.WireName].Channel;
            return aioData.IOType == IOType.OUTPut
                ? CAxtAIO.AIOread_dac((short)channel)
                : CAxtAIO.AIOread_one_volt_adc((short)channel);
        }

        /// <summary>
        /// Reads all channels and updates internal values for both inputs and outputs.
        /// </summary>
        public override void UpdateAllChannelValues()
        {
            if (!_isInitialized) return;

            foreach (var data in _analogIOData.Values)
            {
                short ch = (short)data.Channel;
                data.AValue = data.IOType == IOType.InPut
                    ? CAxtAIO.AIOread_one_volt_adc(ch)
                    : CAxtAIO.AIOread_dac(ch);
            }
        }

        /// <summary>
        /// Writes a value to a specified analog output channel,
        /// validating against configured range and throwing if out of bounds.
        /// </summary>
        /// <param name="aioData">IAnalogIOData instance</param>
        /// <param name="value">Value to write</param>
        /// <returns>True if write succeeded; otherwise false</returns>
        public override bool WriteChannelValue(IAnalogIOData aioData, double value)
        {
            if (!_analogIOData.ContainsKey(aioData.WireName))
                throw new ArgumentException("Specified key does not exist in AIO data.");

            int channel = _analogIOData[aioData.WireName].Channel;
            int rangeCode = _analogIOData[aioData.WireName].Range;
            int min = 0, max = 10;
            switch (rangeCode)
            {
                case 0: min = 0; max = 10; break;
                case 1: min = 0; max = 5; break;
                case 2: min = -10; max = 10; break;
                case 3: min = -5; max = 5; break;
            }
            if (value < min || value > max)
                throw new ArgumentOutOfRangeException(nameof(value), $"Value must be between {min} and {max}.");

            return CAxtAIO.AIOwrite_dac((short)channel, value) == 1;
        }

        /// <summary>
        /// Returns a copy of the internal analog I/O data dictionary.
        /// </summary>
        public override Dictionary<string, IAnalogIOData> GetAnalogIODataDictionary()
        {
            return _analogIOData.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        /// <summary>
        /// Called on application shutdown to release board resources.
        /// Implement disposal logic as needed.
        /// </summary>
        public override void AnalogIOCtrlDispose()
        {
            // TODO: Add any necessary cleanup for Ajin board
        }
    }
}
