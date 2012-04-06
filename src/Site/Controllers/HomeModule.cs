using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using Nancy.ModelBinding;
using ShouldITakeMyDogToFortFunstonNow.Models;
using ShouldITakeMyDogToFortFunstonNow.Services;
using ShouldITakeMyDogToFortFunstonNow.Framework;

namespace ShouldITakeMyDogToFortFunstonNow.Controllers
{
    public class HomeModule : NancyModule
    {

        private readonly string WEATHER_URI = "http://api.wunderground.com/api/f88d918861288deb/conditions/tide/q/pws:KCASANFR69.json";

        public HomeModule(IDecisionService ds)
        {               
            Get["/observation"] = (ctx) =>
            {
                var currentObservation = new CurrentObservation();

                var raw = GetRawWeatherData();
                if (raw == null)                
                    return Response.AsError(Nancy.HttpStatusCode.ServiceUnavailable, "Unable to get weather data.");

                var obs = GetWeatherObservation(raw);
                if (obs == null)
                    return Response.AsError(Nancy.HttpStatusCode.ServiceUnavailable, "Unable to get observation data.");

                currentObservation.ObsDateDescription = obs.observation_time.Value;
                currentObservation.Condition = obs.weather.Value;
                currentObservation.WindMph = obs.wind_mph.Value;
                currentObservation.Temp = obs.temp_f.Value;
                double windGust;
                if(Double.TryParse(obs.wind_gust_mph.Value.ToString(),out windGust))
                    currentObservation.WindGustMph = windGust;
                else currentObservation.WindGustMph = 0;
                double windchill;
                if (Double.TryParse(obs.windchill_f.Value.ToString(), out windchill))
                    currentObservation.WindChill = windchill;
                else currentObservation.WindChill = 0;

                var tides = GetTideSet(raw);
                if (tides != null)
                {
                    var nextLowTide = (DateTime?)GetNextLowTide(tides.tideSummary);
                    if (nextLowTide.HasValue)
                    {
                        currentObservation.NextLowTide = nextLowTide.Value;
                        currentObservation.HoursUntilNextLowTide = (currentObservation.NextLowTide - DateTime.UtcNow).TotalHours;
                    }
                }


                currentObservation.GoFunston = (int)ds.GetDecision(currentObservation);
                return Response.AsJson(currentObservation);
            };

            Get["/"] = (ctx) =>
            {
                return View["Home"];
            };

            Post["/observation"] = (postData) =>
            {
                try
                {
                    var userObs = this.Bind<CurrentObservation>();
                    if (userObs != null)
                    {
                        ds.AddUserObservation(userObs);
                        return new Response() { StatusCode = Nancy.HttpStatusCode.Accepted };
                    }
                    else return Response.AsError(Nancy.HttpStatusCode.BadRequest, "Unable to use observation data.");

                }
                catch (Exception ex)
                {
                    return Response.AsError(Nancy.HttpStatusCode.BadRequest, "Unable to use observation data:" + ex.Message);
                }
            };

        }

        private dynamic GetRawWeatherData()
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
                return (dynamic)json;
            }
            catch
            {
                return null;
            }
        }

        private dynamic GetWeatherObservation(dynamic rawWeather)
        {
            return (dynamic)rawWeather.current_observation;
        }

        private dynamic GetTideSet(dynamic rawWeather)
        {
            if (rawWeather.tide.tideSummary.Count > 0)
                return (dynamic)rawWeather.tide;
            else
                return null;
        }

        private DateTime? GetNextLowTide(dynamic tideSummary)
        {
            foreach (var tide in tideSummary)
            {
                if (tide.data.type.Value == "Max Ebb")
                {                    
                    var date = new DateTime(Convert.ToInt32(tide.utcdate.year.Value), 
                                            Convert.ToInt32(tide.utcdate.mon.Value), 
                                            Convert.ToInt32(tide.utcdate.mday.Value),
                                            Convert.ToInt32(tide.utcdate.hour.Value),
                                            Convert.ToInt32(tide.utcdate.min.Value),
                                            0,DateTimeKind.Utc);
                    return date;
                }
            }

            return null;
        }

    }
}