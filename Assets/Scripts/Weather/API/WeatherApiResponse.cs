using System;
using System.Collections.Generic;

namespace WeatherFeature.API
{
    [Serializable]
    public class WeatherApiResponse
    {
        public WeatherProperties properties;
    }

    [Serializable]
    public class WeatherProperties
    {
        public List<WeatherPeriodDto> periods;
    }

    [Serializable]
    public class WeatherPeriodDto
    {
        public int number;
        public string name;
        public int temperature;
        public string temperatureUnit;
        public bool isDaytime;
        public string shortForecast;
    }
}