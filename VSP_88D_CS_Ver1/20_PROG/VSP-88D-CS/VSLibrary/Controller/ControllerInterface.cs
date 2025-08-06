using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSLibrary.Controller
{
    /// <summary>
    /// Interface for analog I/O control.
    /// Defines basic functions for reading and writing analog I/O device data.
    /// </summary>
    public interface IAIOBase
    {
        /// <summary>
        /// Releases resources related to analog I/O control.
        /// </summary>
        void AnalogIOCtrlDispose();

        /// <summary>
        /// Retrieves the dictionary of analog I/O data objects.
        /// </summary>
        Dictionary<string, IAnalogIOData> GetAnalogIODataDictionary();

        /// <summary>
        /// Reads and returns the value of the specified analog I/O channel.
        /// </summary>
        /// <param name="AioData">The analog I/O data object to read.</param>
        /// <returns>The read analog channel value.</returns>
        double ReadChannelValue(IAnalogIOData AioData);

        /// <summary>
        /// Writes a value to the specified analog I/O channel.
        /// </summary>
        /// <param name="AioData">The analog I/O data object to write to.</param>
        /// <param name="value">The analog value to set.</param>
        /// <returns>True if the write operation succeeded; otherwise, false.</returns>
        bool WriteChannelValue(IAnalogIOData AioData, double value);

        /// <summary>
        /// Updates values of all analog I/O channels.
        /// </summary>
        void UpdateAllChannelValues();
    }

    public interface IIOSettinglist
    {
        // ───────────────────────────────────────────────
        // Setting parameters
        // ───────────────────────────────────────────────

        /// <summary>
        /// Gets or sets the I/O type (Input or Output).
        /// </summary>
        IOType IOType { get; set; }

        /// <summary>
        /// Gets or sets the wire name used as the identifier for an analog I/O channel.
        /// </summary>
        string WireName { get; set; }

        /// <summary>
        /// Gets or sets the equipment mapping name (internal logical name).
        /// </summary>
        string EmName { get; set; }

        /// <summary>
        /// Gets or sets the display name shown in UI or code for readability.
        /// </summary>
        string StrdataName { get; set; }
    }

    /// <summary>
    /// Common data interface for all analog I/O controllers (Ajin, Comizoa).
    /// Provides properties for device settings and status information.
    /// </summary>
    public interface IAnalogIOData : IIOSettinglist
    {
        /// <summary>
        /// Gets or sets the controller instance associated with this analog I/O.
        /// </summary>
        IAIOBase Controller { get; set; }

        /// <summary>
        /// Gets or sets the manufacturer of the analog I/O controller (e.g., Ajin, Adlink).
        /// </summary>
        ControllerType ControllerType { get; set; }

        /// <summary>
        /// Gets or sets the module number, typically identifying a specific module.
        /// </summary>
        short ModuleNumber { get; set; }

        /// <summary>
        /// Gets or sets the module name (e.g., SIO_DO32P).
        /// </summary>
        string ModuleName { get; set; }

        /// <summary>
        /// Gets or sets the channel number (offset) of the I/O.
        /// </summary>
        int Channel { get; set; }

        /// <summary>
        /// Gets or sets the measurement range (e.g., input voltage range).
        /// </summary>
        int Range { get; set; }

        /// <summary>
        /// Gets or sets the analog value (actual measurement or setting).
        /// </summary>
        double AValue { get; set; }

        /// <summary>
        /// Reads the current channel value, optionally forcing a hardware read.
        /// </summary>
        /// <param name="forceRead">If true, forces a fresh hardware read.</param>
        /// <returns>The current analog value.</returns>
        double ReadValue(bool forceRead = false);

        /// <summary>
        /// Writes the specified value to the channel.
        /// </summary>
        /// <param name="value">The value to write.</param>
        void WriteValue(double value);

    }

    /// <summary>
    /// Interface for digital I/O control on multi-axis motion controllers.
    /// Defines basic commands for digital I/O operations.
    /// </summary>
    public interface IDIOBase
    {
        /// <summary>
        /// Releases all resources used by the digital I/O controller.
        /// </summary>
        void DigitalIOCtrlDispose();

        /// <summary>
        /// Retrieves a dictionary mapping I/O identifiers to their data objects.
        /// </summary>
        Dictionary<string, IDigitalIOData> GetDigitalIODataDictionary();

        /// <summary>
        /// Reads the current value of the specified digital I/O bit.
        /// </summary>
        /// <param name="dioData">The digital I/O data object containing bit information.</param>
        /// <returns>True if the bit is high; otherwise, false.</returns>
        bool ReadBit(IDigitalIOData dioData);

        /// <summary>
        /// Writes the specified value to a digital I/O bit.
        /// </summary>
        /// <param name="dioData">The digital I/O data object containing bit information.</param>
        /// <param name="value">The value to set (true for high, false for low).</param>
        /// <returns>True if the write succeeded; otherwise, false.</returns>
        bool WriteBit(IDigitalIOData dioData, bool value);

        /// <summary>
        /// Reads a 32-bit data word from the specified I/O module.
        /// </summary>
        /// <param name="dioDataDict">
        /// A dictionary mapping module keys to their corresponding digital I/O data objects.
        /// </param>
        /// <param name="key">The key identifying which module to read.</param>
        void ReadDword(Dictionary<string, IDigitalIOData> dioDataDict, string key);

        /// <summary>
        /// Writes a 32-bit data word to the specified I/O module.
        /// </summary>
        /// <param name="dioDataDict">
        /// A dictionary mapping module keys to their corresponding digital I/O data objects.
        /// </param>
        /// <param name="key">The key identifying which module to write.</param>
        /// <param name="value">The 32-bit value to write.</param>
        void WriteDword(Dictionary<string, IDigitalIOData> dioDataDict, string key, uint value);

        /// <summary>
        /// Reads the status of all input/output modules in bulk and updates the dictionary.
        /// </summary>
        void UpdateAllIOStates();
    }

    /// <summary>
    /// Common data interface for all digital I/O controllers (Ajin, Comizoa).
    /// Provides properties to describe I/O settings and status.
    /// </summary>
    public interface IDigitalIOData : IIOSettinglist
    {
        /// <summary>
        /// Gets or sets the controller instance associated with this digital I/O.
        /// </summary>
        IDIOBase Controller { get; set; }

        /// <summary>
        /// Gets or sets the axis number (for motion-related I/O).
        /// </summary>
        short AxisNo { get; set; }

        /// <summary>
        /// Gets or sets the type of I/O controller (e.g., I/O board or motion I/O).
        /// </summary>
        ControllerType ControllerType { get; set; }

        /// <summary>
        /// Gets or sets the module name (e.g., SIO_DO32P).
        /// </summary>
        string ModuleName { get; set; }

        /// <summary>
        /// Gets or sets the module index identifier.
        /// </summary>
        int ModuleIndex { get; set; }

        /// <summary>
        /// Gets or sets the current I/O state value.
        /// </summary>
        bool Value { get; set; }

        /// <summary>
        /// Gets or sets the polling state after edge detection.
        /// </summary>
        bool PollingState { get; set; }

        /// <summary>
        /// Gets or sets whether to apply state inversion.
        /// </summary>
        bool StateReversal { get; set; }

        /// <summary>
        /// Gets or sets the bit offset within the module.
        /// </summary>
        int Offset { get; set; }

        /// <summary>
        /// Gets or sets whether edge detection (rising or falling) is enabled.
        /// </summary>
        bool Edge { get; set; }

        /// <summary>
        /// Gets or sets the edge detection duration.
        /// </summary>
        int DetectionTime { get; set; }

        // ───────────────────────────────────────────────
        // controll commands
        // ───────────────────────────────────────────────

        /// <summary>
        /// Checks if the I/O is on, optionally forcing a hardware read.
        /// </summary>
        bool IsOn(bool forceRead = false);

        /// <summary>
        /// Checks if the I/O is off, optionally forcing a hardware read.
        /// </summary>
        bool IsOff(bool forceRead = false);

        /// <summary>
        /// Turns the I/O on.
        /// </summary>
        void On();

        /// <summary>
        /// Turns the I/O off.
        /// </summary>
        void Off();
    }

    /// <summary>
    /// Interface for multi-axis motion control commands.
    /// Defines operations such as move, stop, home, and status checks.
    /// </summary>
    public interface IMotionBase
    {
        /// <summary>
        /// Releases resources related to motion control.
        /// </summary>
        void MotionCtrlDispose();

        /// <summary>
        /// Retrieves the dictionary of axis data objects.
        /// </summary>
        Dictionary<int, IAxisData> GetMotionDataDictionary();

        /// <summary>
        /// Moves the specified axis by a relative distance with given velocity and acceleration.
        /// </summary>
        void MoveToPosition(int axis, double position, double velocity, double acceleration);

        /// <summary>
        /// Moves the specified axis to an absolute position with given velocity and acceleration.
        /// </summary>
        void MoveToPoint(int axis, double position, double velocity, double acceleration);

        /// <summary>
        /// Repeats movement on the specified axis through given positions.
        /// </summary>
        void Repeat(int axis, double[] position, double velocity, double acceleration, int repeatCount);

        /// <summary>
        /// Stops motion on the specified axis.
        /// </summary>
        bool StopMotion(int axis);

        /// <summary>
        /// Clears the alarm on the specified axis asynchronously.
        /// </summary>
        Task ClearAlarm(int axis);

        /// <summary>
        /// Performs a home operation on the specified axis asynchronously.
        /// </summary>
        Task HomeMove(int axis, Motion_HomeConfig initset);

        /// <summary>
        /// Gets the current actual position of the specified axis.
        /// </summary>
        double GetPosition(int axis);

        /// <summary>
        /// Gets the current commanded position of the specified axis.
        /// </summary>
        double GetCmdPosition(int axis);

        /// <summary>
        /// Gets the current velocity of the specified axis.
        /// </summary>
        double GetVelocity(int axis);

        /// <summary>
        /// Enables or disables servo control on the specified axis.
        /// </summary>
        void SetServoOnOff(int axis, bool enabled);

        /// <summary>
        /// Checks whether the specified axis servo is enabled.
        /// </summary>
        bool IsServo(int axis);

        /// <summary>
        /// Checks whether the specified axis has been homed.
        /// </summary>
        bool IsHomed(int axis);

        /// <summary>
        /// Checks whether the specified axis is in an alarm state.
        /// </summary>
        bool IsAlarm(int axis);

        /// <summary>
        /// Checks whether the specified axis is currently moving.
        /// </summary>
        bool IsMoving(int axis);

        /// <summary>
        /// Checks whether the positive limit switch is active for the specified axis.
        /// </summary>
        bool IsPositiveLimit(int axis);

        /// <summary>
        /// Checks whether the negative limit switch is active for the specified axis.
        /// </summary>
        bool IsNegativeLimit(int axis);

        /// <summary>
        /// Sets motion parameters from the given axis data.
        /// </summary>
        bool SetParameter(IAxisData motionData);

        /// <summary>
        /// Retrieves motion parameters into the given axis data.
        /// </summary>
        bool GetParameter(IAxisData motionData);

        /// <summary>
        /// Updates all I/O status for the motion controller.
        /// </summary>
        void UpdateAllIOStatus();

        /// <summary>
        /// Updates all axis positions for the motion controller.
        /// </summary>
        void UpdateAllPosition();
    }

    public interface IAxisSettinglist
    {
        // ───────────────────────────────────────────────
        // Setting parameters
        // ───────────────────────────────────────────────

        //------------------- Basic parameters
        /// <summary>
        /// Gets or sets the controller type for the axis.
        /// </summary>
        ControllerType ControllerType { get; set; }

        /// <summary>
        /// Gets or sets the axis number identifier.
        /// </summary>
        short AxisNo { get; set; }

        /// <summary>
        /// Gets or sets the logical name of the axis.
        /// </summary>
        string AxisName { get; set; }

        /// <summary>
        /// Gets or sets the display name for the axis data.
        /// </summary>
        string StrAxisData { get; set; }


        //------------------- Parameters required to calculate travel distance
        /// <summary>
        /// Gets or sets the lead pitch of the ball screw (mm/rev).
        /// </summary>
        double LeadPitch { get; set; }

        /// <summary>
        /// Gets or sets pulses per revolution (pulse/rev).
        /// </summary>
        double PulsesPerRev { get; set; }

        /// <summary>
        /// Gets or sets the gear ratio (input vs output).
        /// </summary>
        double GearRatio { get; set; }

        /// <summary>
        /// Gets or sets the pulse output mode.
        /// </summary>
        byte PulseOutputMode { get; set; }

        /// <summary>
        /// Gets or sets the encoder input mode.
        /// </summary>
        byte EncInputMode { get; set; }


        //------------------- speed limit
        /// <summary>
        /// Gets or sets the minimum speed limit.
        /// </summary>
        double MinSpeed { get; set; }

        /// <summary>
        /// Gets or sets the maximum speed limit.
        /// </summary>
        double MaxSpeed { get; set; }


        //------------------- Whether the servo motor is activated or not
        /// <summary>
        /// Gets or sets whether servo enable inversion is applied.
        /// </summary>
        bool ServoEnabledReversal { get; set; }


        //------------------- Setting the level
        /// <summary>
        /// Gets or sets whether the positive end limit is set.
        /// </summary>
        bool LvSet_EndLimitP { get; set; }

        /// <summary>
        /// Gets or sets whether the negative end limit is set.
        /// </summary>
        bool LvSet_EndLimitN { get; set; }

        /// <summary>
        /// Gets or sets whether the positive slow limit is set.
        /// </summary>
        bool LvSet_SlowLimitP { get; set; }

        /// <summary>
        /// Gets or sets whether the negative slow limit is set.
        /// </summary>
        bool LvSet_SlowLimitN { get; set; }

        /// <summary>
        /// Gets or sets whether the in-position signal is set.
        /// </summary>
        bool LvSet_InPosition { get; set; }

        /// <summary>
        /// Gets or sets whether the alarm state is set.
        /// </summary>
        bool LvSet_Alarm { get; set; }        
    }

    /// <summary>
    /// Common data interface for all motion controllers (Ajin, Comizoa).
    /// Provides axis settings, status information (position, velocity, limits), and control methods.
    /// </summary>
    public interface IAxisData : IAxisSettinglist
    {
        // ───────────────────────────────────────────────
        // Basic information
        // ───────────────────────────────────────────────

        /// <summary>
        /// Gets or sets the motion controller associated with this axis.
        /// </summary>
        IMotionBase Controller { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the servo is enabled.
        /// </summary>
        bool ServoEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the axis has completed its home operation.
        /// </summary>
        bool HomeState { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an alarm is currently active on the axis.
        /// </summary>
        bool Alarm { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the positive limit switch is engaged.
        /// </summary>
        bool PositiveLimit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the negative limit switch is engaged.
        /// </summary>
        bool NegativeLimit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the axis is in the target position.
        /// </summary>
        bool InPosition { get; set; }

        // ───────────────────────────────────────────────
        // Commanded motion parameters and actual status
        // ───────────────────────────────────────────────

        /// <summary>
        /// Gets or sets the target position command for this axis.
        /// </summary>
        double CurrentPosition { get; set; }

        /// <summary>
        /// Gets or sets the target velocity command for this axis.
        /// </summary>
        double CurrentVelocity { get; set; }

        /// <summary>
        /// Gets or sets the target acceleration command for this axis.
        /// </summary>
        double CurrentAcceleration { get; set; }

        /// <summary>
        /// Gets or sets the actual current position of this axis.
        /// </summary>
        double Position { get; set; }

        /// <summary>
        /// Gets or sets the actual current velocity of this axis.
        /// </summary>
        double Velocity { get; set; }

        // ───────────────────────────────────────────────
        // Control methods
        // ───────────────────────────────────────────────

        /// <summary>
        /// Moves the axis by a relative distance using specified velocity and acceleration.
        /// </summary>
        /// <param name="position">Relative distance to move.</param>
        /// <param name="velocity">Movement velocity.</param>
        /// <param name="acceleration">Movement acceleration.</param>
        void MoveToPosition(double position, double velocity, double acceleration);

        /// <summary>
        /// Moves the axis to an absolute position using specified velocity and acceleration.
        /// </summary>
        /// <param name="position">Absolute target position.</param>
        /// <param name="velocity">Movement velocity.</param>
        /// <param name="acceleration">Movement acceleration.</param>
        void MoveToPoint(double position, double velocity, double acceleration);

        /// <summary>
        /// Repeats movement through an array of positions with specified velocity, acceleration, and repeat count.
        /// </summary>
        /// <param name="position">Array of positions to move through.</param>
        /// <param name="velocity">Movement velocity.</param>
        /// <param name="acceleration">Movement acceleration.</param>
        /// <param name="repeatCount">Number of times to repeat the sequence.</param>
        void Repeat(double[] position, double velocity, double acceleration, int repeatCount);

        /// <summary>
        /// Stops any ongoing motion on this axis.
        /// </summary>
        /// <returns>True if the stop command was accepted; otherwise, false.</returns>
        bool StopMotion();

        /// <summary>
        /// Enables or disables the servo motor on this axis.
        /// </summary>
        /// <param name="enabled">True to turn servo on; false to turn servo off.</param>
        /// <returns>True if the command succeeded; otherwise, false.</returns>
        bool SetServoOnOff(bool enabled);

        /// <summary>
        /// Performs a homing operation on this axis using the given configuration.
        /// </summary>
        /// <param name="initset">Home move configuration parameters.</param>
        /// <returns>A task that completes when homing is finished.</returns>
        Task HomeMove(Motion_HomeConfig initset);

        // ───────────────────────────────────────────────
        // Status queries and I/O updates
        // ───────────────────────────────────────────────

        /// <summary>
        /// Reads and returns the actual position of this axis.
        /// </summary>
        double GetPosition();

        /// <summary>
        /// Reads and returns the commanded (target) position of this axis.
        /// </summary>
        double GetCmdPosition();

        /// <summary>
        /// Reads and returns the actual velocity of this axis.
        /// </summary>
        double GetVelocity();

        /// <summary>
        /// Checks whether the servo is currently enabled.
        /// </summary>
        bool IsServo();

        /// <summary>
        /// Checks whether the axis has been homed.
        /// </summary>
        bool IsHomed();

        /// <summary>
        /// Checks whether an alarm condition is active on the axis.
        /// </summary>
        bool IsAlarm();

        /// <summary>
        /// Checks whether the axis is moving (not yet at target).
        /// </summary>
        bool IsMoving();

        /// <summary>
        /// Checks whether the positive limit switch is engaged.
        /// </summary>
        bool IsPositiveLimit();

        /// <summary>
        /// Checks whether the negative limit switch is engaged.
        /// </summary>
        bool IsNegativeLimit();

        /// <summary>
        /// Clears any alarm condition on the axis.
        /// </summary>
        /// <returns>True if the alarm was cleared successfully; otherwise, false.</returns>
        bool ClearAlarm();

        // ───────────────────────────────────────────────
        // Parameter save and restore
        // ───────────────────────────────────────────────

        /// <summary>
        /// Retrieves and loads saved motion parameters into this axis data.
        /// </summary>
        /// <returns>True if parameters were retrieved successfully; otherwise, false.</returns>
        bool GetParameter();

        /// <summary>
        /// Saves the provided motion parameters into this axis data.
        /// </summary>
        /// <param name="motionData">The axis data containing parameters to save.</param>
        /// <returns>True if parameters were saved successfully; otherwise, false.</returns>
        bool SetParameter(IAxisData motionData);
    }
}
