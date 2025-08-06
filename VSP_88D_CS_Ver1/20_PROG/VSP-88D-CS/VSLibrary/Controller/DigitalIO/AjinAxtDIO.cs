using System;
using System.Collections.Generic;
using System.Linq;

namespace VSLibrary.Controller.DigitalIO
{
    /// <summary>
    /// Controller class for managing and performing read/write operations
    /// on an Ajin digital I/O board.
    /// Inherits from DIOBase and provides initialization, data handling,
    /// and module control for Ajin digital I/O modules.
    /// </summary>
    public class AjinAxtDIO : DIOBase
    {
        /// <summary>
        /// Dictionary storing digital I/O data objects keyed by wire name.
        /// </summary>
        private Dictionary<string, IDigitalIOData> _digitalIOData = new Dictionary<string, IDigitalIOData>();

        private Dictionary<string, int> _iocount;

        private bool _isInitialized = false;  // Indicates whether the board has been initialized

        /// <summary>
        /// Constructs the Ajin digital I/O controller with provided channel counts.
        /// Performs board initialization and device setup.
        /// </summary>
        /// <param name="count">Dictionary containing channel counts (e.g., "DInput", "DOutput").</param>
        public AjinAxtDIO(Dictionary<string, int> count)
        {
            _iocount = count;

            if (IsInitialized())
            {
                OpenDevice();
                _isInitialized = true;
            }

            // Populate IoType data for debugging (disable in production)
            test();
        }

        /// <summary>
        /// Verifies that the specified wire name exists in the dictionary.
        /// Prints an error if not found.
        /// </summary>
        /// <param name="wireName">Wire name to check.</param>
        /// <returns>True if the wire exists; otherwise false.</returns>
        private bool CheckDic(string wireName)
        {
            if (!_digitalIOData.ContainsKey(wireName))
            {
                Console.WriteLine($"[Error] No digital IO data found for {wireName}.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Creates 15 dummy digital I/O entries for testing and debugging.
        /// </summary>
        private void test()
        {
            for (int i = 0; i < 15; i++)
            {
                DIOData dummy = new DIOData
                {
                    Controller = this,
                    ControllerType = ControllerType.DIO_AjinAXT,
                    IOType = IOType.OUTPut,
                    WireName = $"Y{i + _iocount["DOutput"]:X3}",  // e.g.: Y000, Y001, ...
                    StrdataName = string.Empty,
                    ModuleName = string.Empty,
                    ModuleIndex = i,
                    Value = false,
                    PollingState = false,
                    StateReversal = false,
                    Offset = 0,
                    Edge = false,
                    DetectionTime = 0
                };
                _digitalIOData.Add(dummy.WireName, dummy);
            }
            _iocount["DOutput"] += 15;
        }

        /// <summary>
        /// Initializes the Ajin digital I/O library.
        /// </summary>
        /// <returns>True if initialization is successful; otherwise false.</returns>
        private bool IsInitialized()
        {
            // CAxtDIO.InitializeDIO returns non-zero on success
            return CAxtDIO.InitializeDIO() != 0;
        }

        /// <summary>
        /// Opens all modules and populates input/output data entries.
        /// </summary>
        public void OpenDevice()
        {
            int moduleCount = CAxtDIO.DIOget_module_count();
            int inputCount = 0;
            int outputCount = 0;

            for (int i = 0; i < moduleCount; i++)
            {
                int moduleID = CAxtDIO.DIOget_module_id((short)i);
                AXT_FUNC_MODULE moduleType = (AXT_FUNC_MODULE)moduleID;

                // Output-only modules
                if (moduleType == AXT_FUNC_MODULE.AXT_SIO_DO32P || moduleType == AXT_FUNC_MODULE.AXT_SIO_DO32T)
                {
                    AddOutputModule(moduleType, i, ref outputCount);
                }
                // Dual DI/DO modules
                else if (moduleType == AXT_FUNC_MODULE.AXT_SIO_DB32P || moduleType == AXT_FUNC_MODULE.AXT_SIO_DB32T)
                {
                    AddInputModule(moduleType, i, ref inputCount);
                    AddOutputModule(moduleType, i, ref outputCount);
                }
                // Input-only modules
                else if (moduleType == AXT_FUNC_MODULE.AXT_SIO_DI32)
                {
                    AddInputModule(moduleType, i, ref inputCount);
                }
            }
        }

        /// <summary>
        /// Handles input module data creation and adds entries to the dictionary.
        /// </summary>
        /// <param name="moduleType">Type of module.</param>
        /// <param name="moduleIndex">Index of module.</param>
        /// <param name="inputCount">Current number of input channels (ref parameter).</param>
        private void AddInputModule(AXT_FUNC_MODULE moduleType, int moduleIndex, ref int inputCount)
        {
            int count = CAxtDIO.DIOget_input_number((short)moduleIndex);
            string moduleName = moduleType == AXT_FUNC_MODULE.AXT_SIO_DB32P ? "SIO_DB32P" : "SIO_DB32T";

            for (int i = 0; i < count; i++)
            {
                DIOData data = new DIOData
                {
                    Controller = this,
                    ControllerType = ControllerType.DIO_AjinAXT,
                    IOType = IOType.InPut,
                    WireName = $"X{inputCount + _iocount["DInput"]:X3}",  // e.g.: X000, X001, ...
                    StrdataName = string.Empty,
                    ModuleName = moduleType == AXT_FUNC_MODULE.AXT_SIO_DI32 ? "SIO_DI32" : moduleName,
                    ModuleIndex = moduleIndex,
                    Value = false,
                    PollingState = false,
                    StateReversal = false,
                    Offset = i,
                    Edge = false,
                    DetectionTime = 0
                };
                _digitalIOData.Add(data.WireName, data);
                inputCount++;
            }
            _iocount["DInput"] += inputCount;
        }

        /// <summary>
        /// Handles output module data creation and adds entries to the dictionary.
        /// </summary>
        /// <param name="moduleType">Type of module.</param>
        /// <param name="moduleIndex">Index of module.</param>
        /// <param name="outputCount">Current number of output channels (ref parameter).</param>
        private void AddOutputModule(AXT_FUNC_MODULE moduleType, int moduleIndex, ref int outputCount)
        {
            int count = CAxtDIO.DIOget_output_number((short)moduleIndex);
            string moduleName = moduleType == AXT_FUNC_MODULE.AXT_SIO_DO32P ? "SIO_DO32P" : "SIO_DO32T";

            for (int i = 0; i < count; i++)
            {
                DIOData data = new DIOData
                {
                    Controller = this,
                    ControllerType = ControllerType.DIO_AjinAXT,
                    IOType = IOType.OUTPut,
                    WireName = $"Y{outputCount + _iocount["DOutput"]:X3}",  // e.g.: Y000, Y001, ...
                    StrdataName = string.Empty,
                    ModuleName = moduleName,
                    ModuleIndex = moduleIndex,
                    Value = false,
                    PollingState = false,
                    StateReversal = false,
                    Offset = i,
                    Edge = false,
                    DetectionTime = 0
                };
                _digitalIOData.Add(data.WireName, data);
                outputCount++;
            }
            _iocount["DOutput"] += outputCount;
        }

        /// <summary>
        /// Returns the dictionary of digital I/O data objects.
        /// </summary>
        public override Dictionary<string, IDigitalIOData> GetDigitalIODataDictionary()
        {
            return _digitalIOData.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        /// <summary>
        /// Releases resources on application shutdown.
        /// Implement board-specific cleanup here if needed.
        /// </summary>
        public override void DigitalIOCtrlDispose()
        {
            // TODO: Add cleanup calls for Ajin DIO board
        }

        /// <summary>
        /// Reads the state of a single bit for the specified I/O data.
        /// </summary>
        /// <param name="dioData">Digital I/O data object.</param>
        /// <returns>Current bit value.</returns>
        public override bool ReadBit(IDigitalIOData dioData)
        {
            if (dioData.IOType == IOType.InPut)
            {
                dioData.Value = CAxtDIO.DIOread_inport_bit((short)dioData.ModuleIndex, (ushort)dioData.Offset) == 1;
            }
            else
            {
                dioData.Value = CAxtDIO.DIOread_outport_bit((short)dioData.ModuleIndex, (ushort)dioData.Offset) == 1;
            }
            return dioData.Value;
        }

        /// <summary>
        /// Writes a bit to the specified output I/O data and returns its new state.
        /// </summary>
        /// <param name="dioData">Digital I/O data object.</param>
        /// <param name="value">Value to write (true/false).</param>
        /// <returns>Bit value after write.</returns>
        public override bool WriteBit(IDigitalIOData dioData, bool value)
        {
            if (dioData.IOType != IOType.OUTPut)
                throw new InvalidOperationException("Cannot write to an input channel.");

            CAxtDIO.DIOwrite_outport_bit((short)dioData.ModuleIndex, (ushort)dioData.Offset, value ? 1 : 0);
            return ReadBit(dioData);
        }

        /// <summary>
        /// Reads a 32-bit value from the specified module and updates each bit's state.
        /// </summary>
        /// <param name="dioDataDict">Dictionary of digital I/O data.</param>
        /// <param name="key">Key identifying the module to read.</param>
        public override void ReadDword(Dictionary<string, IDigitalIOData> dioDataDict, string key)
        {
            var target = dioDataDict[key];
            bool isInput = target.IOType == IOType.InPut;
            int idx = target.ModuleIndex;
            uint data = isInput
                ? CAxtDIO.DIOread_inport_dword((short)idx, 0)
                : CAxtDIO.DIOread_outport_dword((short)idx, 0);

            var sameModule = dioDataDict.Values.Where(x => x.ModuleIndex == idx && x.IOType == target.IOType);
            foreach (var item in sameModule)
            {
                item.Value = (data & (1u << item.Offset)) != 0;
            }
        }

        /// <summary>
        /// Writes a 32-bit value to the specified module and updates its bit states.
        /// </summary>
        /// <param name="dioDataDict">Dictionary of digital I/O data.</param>
        /// <param name="key">Key identifying the module to write.</param>
        /// <param name="value">32-bit value to write.</param>
        public override void WriteDword(Dictionary<string, IDigitalIOData> dioDataDict, string key, uint value)
        {
            CAxtDIO.DIOwrite_outport_dword((short)dioDataDict[key].ModuleIndex, 0, value);
            ReadDword(dioDataDict, key);
        }

        /// <summary>
        /// Updates all I/O module states by reading their current 32-bit values.
        /// </summary>
        public override void UpdateAllIOStates()
        {
            if (!_isInitialized) return;

            // Update input modules
            var inputGroups = _digitalIOData.Values.Where(d => d.IOType == IOType.InPut).GroupBy(d => d.ModuleIndex);
            foreach (var grp in inputGroups)
            {
                ReadDword(_digitalIOData, grp.First().WireName);
            }

            // Update output modules
            var outputGroups = _digitalIOData.Values.Where(d => d.IOType == IOType.OUTPut).GroupBy(d => d.ModuleIndex);
            foreach (var grp in outputGroups)
            {
                ReadDword(_digitalIOData, grp.First().WireName);
            }
        }
    }
}