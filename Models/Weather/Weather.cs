using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Portfolio.Models.Weather
{


    public class Weather
    {
        public string SunriseUTC;
        public string SunsetUTC;
        public string City;
        public List<DayForecast> Forecasts;
        public int days;
        public DateTime Sunrise;
        public DateTime Sunset;


        public Weather(String URL)
        {
            XDocument WeatherXml = XDocument.Load(URL);
            City = WeatherXml.Root.Element("location").Element("name").Value.ToString();
            SunriseUTC = WeatherXml.Root.Element("sun").Attribute("rise").Value.ToString();
            SunriseUTC = SunriseUTC.Substring(SunriseUTC.IndexOf("T") + 1);
            SunsetUTC = WeatherXml.Root.Element("sun").Attribute("set").Value.ToString();
            SunsetUTC = SunsetUTC.Substring(SunsetUTC.IndexOf("T") + 1);
            Sunrise = Convert.ToDateTime(SunriseUTC);
            Sunrise = TimeZone.CurrentTimeZone.ToLocalTime(Sunrise);
            Sunset = Convert.ToDateTime(SunsetUTC);
            Sunset = TimeZone.CurrentTimeZone.ToLocalTime(Sunset); List<XElement> ForecastXml = WeatherXml.Root.Element("forecast").Elements().ToList();
            Forecasts = new List<DayForecast>();
            foreach (XElement day in ForecastXml)
            {
                DayForecast forecast = new DayForecast();
                forecast.Day = DateTime.Parse(day.Attribute("day").Value);
                forecast.Forecast = GetValue(day, "symbol", "name");
                forecast.WindDirection = GetValue(day, "windDirection", "name");
                forecast.precipitation = GetNumber(day, "precipitation", "value");
                forecast.WindSpeed = GetValue(day, "windSpeed", "name");
                forecast.Pressure = GetNumber(day, "pressure", "value");
                forecast.Humidity = GetNumber(day, "humidity", "value");
                forecast.Clouds = GetValue(day, "clouds", "value");
                forecast.Temperature = new Temperature();
                forecast.Temperature.day = GetNumber(day, "temperature", "day");
                forecast.Temperature.Min = GetNumber(day, "temperature", "min");
                forecast.Temperature.Max = GetNumber(day, "temperature", "max");
                forecast.Temperature.Night = GetNumber(day, "temperature", "night");
                forecast.Temperature.Eve = GetNumber(day, "temperature", "eve");
                forecast.Temperature.Morn = GetNumber(day, "temperature", "morn");
                Forecasts.Add(forecast);
            }
            days = Forecasts.Count;
        }
        private decimal GetNumber(XElement root, string element, string Value)
        {
            if (root.Element(element) == null ||
                root.Element(element).Attribute(Value) == null)
                return 0;
            else
                return decimal.Parse(root.Element(element).Attribute(Value).Value);
        }
        private string GetValue(XElement root, string element, string Value)
        {
            if (root.Element(element) == null ||
                root.Element(element).Attribute(Value) == null)
                return string.Empty;
            else
                return root.Element(element).Attribute(Value).Value;
        }

    }
}