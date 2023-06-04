using PieroDeTomi.EDrums.Managers;
using PieroDeTomi.EDrums.Models;

namespace PieroDeTomi.EDrums.Console
{
    public class EDrums : IDisposable
    {
        private const int MIDI_DRUMS_CHANNEL = 10;

        private readonly MidiVirtualDeviceManager _midiDevice;

        private readonly AudioDeviceManager _audioDevice;

        public EDrums(string midiVirtualDeviceName, string audioDeviceSearchKey, int audioDeviceSampleRate, float maxWaveImpulseValue)
        {
            _midiDevice = new MidiVirtualDeviceManager(midiVirtualDeviceName, channel: MIDI_DRUMS_CHANNEL);

            _audioDevice = new AudioDeviceManager(
                deviceSearchKey: audioDeviceSearchKey,
                sampleRate: audioDeviceSampleRate,
                maxWaveImpulseValue);

            BindInputChannels();
        }

        public void Dispose()
        {
            _audioDevice.Dispose();
            _midiDevice.Dispose();
        }

        private void BindInputChannels()
        {
            _audioDevice.BindInputChannel(1, velocity =>
            {
                _midiDevice.SendNote(MidiNote.Snare, velocity);
                System.Console.WriteLine($"Snare {velocity}");
            });
        }
    }
}