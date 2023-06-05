using PieroDeTomi.EDrums.Managers;
using PieroDeTomi.EDrums.Models.Configuration;

namespace PieroDeTomi.EDrums.Console
{
    public class EDrums : IDisposable
    {
        private readonly DrumModuleConfiguration _configuration;

        private readonly MidiDeviceManager _midiDevice;

        private readonly AudioDeviceManager _audioDevice;

        public EDrums(DrumModuleConfiguration configuration)
        {
            _configuration = configuration;

            _midiDevice = new MidiDeviceManager(configuration);
            _audioDevice = new AudioDeviceManager(configuration);

            BindInputChannels();
        }

        public void Dispose()
        {
            _audioDevice.Dispose();
            _midiDevice.Dispose();
        }

        private void BindInputChannels()
        {
            _configuration.ChannelMappings.ForEach(mapping => BindInputChannel(mapping));
        }

        private void BindInputChannel(InputChannelMappingConfiguration mapping)
        {
            _audioDevice.BindInputChannel(mapping.Channel, velocity =>
            {
                _midiDevice.SendNote(mapping.MidiNote, velocity);
                System.Console.WriteLine($"Note {mapping.MidiNote} > {velocity}");
            });
        }
    }
}