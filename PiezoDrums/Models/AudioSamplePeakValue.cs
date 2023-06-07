namespace PiezoDrums.Models
{
    public readonly record struct AudioSamplePeakValue
    {
        public float UnderlyingValue { get; init; }

        public float NormalizedValue { get; init; }

        public bool IsAudible { get; init; }

        public AudioSamplePeakValue(float underlyingValue)
        {
            UnderlyingValue = underlyingValue;
            NormalizedValue = UnderlyingValue * 10;
            IsAudible = Math.Round(UnderlyingValue, 2) > 0;
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