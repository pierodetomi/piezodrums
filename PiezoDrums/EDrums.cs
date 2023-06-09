﻿using PiezoDrums.Base;
using PiezoDrums.Managers;
using PiezoDrums.Models.Configuration;

namespace PiezoDrums.Console
{
    public class EDrums : LoggingComponentBase, IDisposable
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

#if DEBUG
                Log($"NOTE: {mapping.MidiNote} - VELOCITY: {velocity}", clearPreviousContent: false);
#endif
            });
        }
    }
}