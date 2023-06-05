namespace PiezoDrums.Models
{
    public struct AudioSamplePeakValue
    {
        public float UnderlyingValue { get; private set; }

        public float NormalizedValue => UnderlyingValue * 10;

        public bool IsAudible => Math.Round(UnderlyingValue, 2) > 0;

        public AudioSamplePeakValue(float underlyingValue)
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