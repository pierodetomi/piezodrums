namespace PieroDeTomi.EDrums.Models.Configuration
{
    public class DrumModuleConfiguration
    {
        public int SampleRate { get; set; }

        public MidiConfiguration Midi { get; set; }
        
        public string AudioDeviceSearchKey { get; set; }
        
        public float MaxWaveImpulseValue { get; set; }

        public List<InputChannelMappingConfiguration> ChannelMappings { get; set; } = new();
    }
}
