using System;
using Sanford.Multimedia.Midi;
namespace UDPMIDILib
{
    public class UDPMidiOutputDevice
    {

        public UDPMidiOutputDevice()
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
            caps = OutputDevice.GetDeviceCapabilities(index);
            try
            {
                device = new OutputDevice(index);
            }
            catch (Exception ex) {
            }


        }

        public UDPMidiOutputDevice(int DeviceIndex)
        {
            Initialize(DeviceIndex);
        }

        bool active;
        int index;
        MidiOutCaps caps;
        OutputDevice device;
        public MidiOutCaps Caps { get => caps; set => caps = value; }
        public bool Active { get => active; set => active = value; }
        public OutputDevice Device { get => device; set => device = value; }
        public int Index { get => index; set => index = value; }
    }
}
