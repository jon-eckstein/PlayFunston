using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Linq.Mapping;
using System.ComponentModel.DataAnnotations;
using ShouldITakeMyDogToFortFunstonNow.Framework;

namespace ShouldITakeMyDogToFortFunstonNow.Models
{
    [Serializable]    
    public class CurrentObservation
    {
        //[Column(IsPrimaryKey = true, IsDbGenerated = true)]
        [Key]
        public object Id { get; set; }        
        //[Column(CanBeNull=true)]
        public DateTime Day { get; set; }
        //[Column(CanBeNull=true)]
        private string condition;
        public string Condition 
        {
            get { return condition; }
            set
            {
                condition = value;
                if(!String.IsNullOrEmpty(condition))
                    ConditionCode = GetConditionCode(condition);
            } 
        }
        //[Column(CanBeNull = false)]
        public int ConditionCode { get; set; }
        //[Column(CanBeNull = false)]
        public double WindMph { get; set; }
        //[Column]
        public double WindGustMph { get; set; }
        //[Column(CanBeNull = false)]
        public double Temp {get;set;}
        //[Column]
        public double WindChill { get; set; }
        //[Column(CanBeNull=true)]
        public DateTime NextLowTide { get; set; }        
        public int WeatherScore { get; set; }        
        public int TideScore { get; set; }
        //[Column(CanBeNull = false)]
        public int GoFunston { get; set; }

        public CurrentObservation() { }

        public CurrentObservation(int code, double temp, double windChill, double windMph, double windGustMph, int go)
        {
            ConditionCode = code;
            WindMph = windMph;
            WindGustMph = windGustMph;
            Temp = temp;
            WindChill = windChill;
            GoFunston = go;
        }

        public CurrentObservation(string weather, double temp, double windChill, double windMph, double windGustMph, int go)
            :this(GetConditionCode(weather),temp,windChill,windMph, windGustMph, go)
        {            
        }



        public double[] ToDoubleArray()
        {
            return new[] { ConditionCode, Temp, WindChill, WindMph, WindGustMph, GoFunston };
        }

        private static int GetConditionCode(string weather)
        {

            if (weather.Contains(new[] { "Clear", "Partly Cloudy", "Scattered Clouds", "Haze" }))
                return (int)Conditions.Good;

            if (weather.Contains(new[] { "Drizzle", "Mostly Cloudy", "Overcast", "Fog", "Mist" }))
                return (int)Conditions.Ok;

            return (int)Conditions.Bad;
        }        
        


    }
}