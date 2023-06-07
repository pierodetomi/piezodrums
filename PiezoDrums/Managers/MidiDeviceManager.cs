using NAudio.Midi;
using PiezoDrums.Base;
using PiezoDrums.Models.Configuration;

namespace PiezoDrums.Managers
{
    public class MidiDeviceManager : LoggingComponentBase, IDisposable
    {
        private readonly DrumModuleConfiguration _configuration;

        private int _deviceIndex = -1;

        private MidiOut _midiOut = null;

        private NoteOnEvent _noteEvent = null;

        public MidiDeviceManager(DrumModuleConfiguration configuration)
        {
            _configuration = configuration;
            _noteEvent = new NoteOnEvent(0, _configuration.Midi.Channel, 0, 0, 0);

            InitializeMidiDevice(_configuration.Midi.DeviceName);
        }

        public void SendNote(int note, int velocity)
        {
            _noteEvent.NoteNumber = note;
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
                if (MidiOut.DeviceInfo(device).ProductName.Contains(deviceName))
                {
                    _deviceIndex = device;
                    _midiOut = new MidiOut(_deviceIndex);

                    break;
                }
            }
        }
    }
}
