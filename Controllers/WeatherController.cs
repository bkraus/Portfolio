using Portfolio.Models.Weather;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portfolio.Controllers
{
    public class WeatherController : Controller
    {
        //
        // GET: /Weather/

        public ActionResult Index()
        {
            const string API_Call = "http://api.openweathermap.org/data/2.5/forecast/daily?q={0}&mode=xml&units=imperial&cnt={1}&APPID=27e9627a91300eec2c39fa9ebd4e7cb3";
            String CityWeather = String.Format(API_Call, "Newark", "16");

                Weather weather = new Weather(CityWeather);

            return View(weather);
        }
        public ActionResult CitySearch(string city)
        {
            const string API_Call = "http://api.openweathermap.org/data/2.5/forecast/daily?q={0}&mode=xml&units=imperial&cnt={1}&APPID=27e9627a91300eec2c39fa9ebd4e7cb3";
            String CityWeather = String.Format(API_Call, city, "16");

            Weather weather = new Weather(CityWeather);

            return View("Index",weather);
        }

    }
}
