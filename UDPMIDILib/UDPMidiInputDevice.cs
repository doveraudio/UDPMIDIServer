using System;
using Sanford.Multimedia.Midi;
namespace UDPMIDILib
{
    public class UDPMidiInputDevice
    {

        public UDPMidiInputDevice()
        {

            try
            {
                this.Initialize(0);
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        private void Initialize(int i)
        {
            active = false;
            index = i;
            caps = InputDevice.GetDeviceCapabilities(index);
            try
            {
            device = new InputDevice(index);

            }
            catch (Exception ex)
            {

                //throw;
            }



        }

        public UDPMidiInputDevice(int DeviceIndex)
        {
            Initialize(DeviceIndex);
        }

        bool active;
        int index;
        MidiInCaps caps;
        InputDevice device;
        public MidiInCaps Caps { get => caps; set => caps = value; }
        public bool Active { get => active; set => active = value; }
        public InputDevice Device { get => device; set => device = value; }
        public int Index { get => index; set => index = value; }
    }
}
