using NAudio.Midi;
using PiezoDrums.Models.Configuration;
using Sanford.Multimedia.Midi;

namespace PiezoDrums.Managers
{
    public class MidiDeviceManager : ManagerBase
    {
        private readonly DrumModuleConfiguration _configuration;

        private int _deviceIndex = -1;

        private OutputDevice _midiOut = null;

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

            _midiOut.SendShort(_noteEvent.GetAsShortMessage());
        }

        public override void Dispose()
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
                    _midiOut = new OutputDevice(_deviceIndex);

                    break;
                }
            }
        }
    }
}
