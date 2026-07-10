namespace WeatherFeature.Domain
{
    public sealed class WeatherData
    {
        public string DayName { get; }
        public int Temperature { get; }
        public string TemperatureUnit { get; }

        public WeatherData(string dayName, int temperature, string temperatureUnit)
        {
            DayName = dayName;
            Temperature = temperature;
            TemperatureUnit = temperatureUnit;
        }

        public string ToDisplayString()
        {
            return $"{DayName} - {Temperature}{TemperatureUnit}";
        }
    }
}