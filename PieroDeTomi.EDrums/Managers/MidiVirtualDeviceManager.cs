using NAudio.Midi;
using PieroDeTomi.EDrums.Models;

namespace PieroDeTomi.EDrums.Managers
{
    public class MidiVirtualDeviceManager : IDisposable
    {
        private readonly int _channel;

        private int _deviceIndex = -1;

        private MidiOut _midiOut = null;

        private NoteOnEvent _noteEvent = null;

        public MidiVirtualDeviceManager(string deviceName, int channel)
        {
            _channel = channel;
            _noteEvent = new NoteOnEvent(0, _channel, 0, 0, 0);

            InitializeMidiDevice(deviceName);
        }

        public void SendNote(MidiNote note, int velocity)
        {
            _noteEvent.NoteNumber = (int)note;
            _noteEvent.Velocity = velocity;

            _midiOut.Send(_noteEvent.GetAsShortMessage());
        }

        public void Dispose()
        {
            try { _midiOut.Dispose(); }
            catch { }
        }

        private void InitializeMidiDevice(string deviceName)
        {
            for (int device = 0; device < MidiOut.NumberOfDevices; device++)
            {
                if (MidiOut.DeviceInfo(device).ProductName == deviceName)
                {
                    _deviceIndex = device;
                    _midiOut = new MidiOut(_deviceIndex);

                    break;
                }
            }
        }
    }
}
