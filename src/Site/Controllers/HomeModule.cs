using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using ShouldITakeMyDogToFortFunstonNow.Models;
using ShouldITakeMyDogToFortFunstonNow.Services;
using ShouldITakeMyDogToFortFunstonNow.Framework;

namespace ShouldITakeMyDogToFortFunstonNow.Controllers
{
    public class HomeModule : NancyModule
    {

        private readonly string WEATHER_URI = "http://api.wunderground.com/api/f88d918861288deb/conditions/tide/q/pws:KCASANFR69.json";

        public HomeModule(DecisionService ds)
        {               
            Get["/observation"] = (ctx) =>
            {
                var currentObservation = new CurrentObservation();

                var obs = GetWeatherObservation();
                if (obs == null)                
                    return Response.AsError(Nancy.HttpStatusCode.ServiceUnavailable, "Unable to get weather data.");
                                   
                currentObservation.Condition = obs.weather.Value;
                currentObservation.WindMph = obs.wind_mph.Value;
                currentObservation.Temp = obs.temp_f.Value;
                double windGust;
                if(Double.TryParse(obs.wind_gust_mph.Value,out windGust))
                    currentObservation.WindGustMph = windGust;
                else currentObservation.WindGustMph = 0;
                double windchill;
                if (Double.TryParse(obs.windchill_f.Value, out windchill))
                    currentObservation.WindChill = windchill;
                else currentObservation.WindChill = 0;

                currentObservation.GoFunston = (int)ds.GetDecision(currentObservation);
                return Response.AsJson(currentObservation);
            };

            Get["/"] = (ctx) =>
            {
                return View["Home"];
            };

        }

        private dynamic GetWeatherObservation()
        {
            try
            {
                var httpClient = HttpWebRequest.Create(WEATHER_URI);
                var response = httpClient.GetResponse() as HttpWebResponse;
                var stream = response.GetResponseStream();
                var respReader = new StreamReader(stream);
                string txtResponse = respReader.ReadToEnd();
                if (String.IsNullOrEmpty(txtResponse))
                    return null;

                dynamic json = JObject.Parse(txtResponse);
                return (dynamic)json.current_observation;
            }
            catch
            {
                return null;
            }
        }

    }
}