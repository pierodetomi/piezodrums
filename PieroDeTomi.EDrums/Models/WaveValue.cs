namespace PieroDeTomi.EDrums.Models
{
    public struct WaveValue
    {
        public float UnderlyingValue { get; private set; }

        public float NormalizedValue => UnderlyingValue * 10;

        public bool IsAudible => Math.Round(UnderlyingValue, 2) > 0;

        public WaveValue(float underlyingValue)
        {
            UnderlyingValue = underlyingValue;
        }

        public int ToVelocity(float maxNormalizedValue)
        {
            var value = NormalizedValue;

            if (value > maxNormalizedValue)
            {
                // System.Console.WriteLine(value);
                value = maxNormalizedValue;
            }

            // peakValue : velocity = maxPeakValue : maxVelocity
            // velocity = (peakValue * maxVelocity) / maxPeakValue
            return (int)Math.Floor(value * 127f / maxNormalizedValue);
        }
    }
}