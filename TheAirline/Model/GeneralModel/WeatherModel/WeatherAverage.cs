﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheAirline.Model.GeneralModel.CountryModel.TownModel;
using TheAirline.Model.AirportModel;
using System.Runtime.Serialization;
using System.Reflection;


namespace TheAirline.Model.GeneralModel.WeatherModel
{
    //the class for the averages for a month for a region/state/country/town
    [Serializable]
    public class WeatherAverage : ISerializable
    {
        [Versioning("airport")]
        public Airport Airport { get; set; }
        [Versioning("country")]
        public Country Country { get; set; }
        [Versioning("town")]
        public Town Town { get; set; }
        [Versioning("month")]
        public int Month { get; set; }
        //in mm
        [Versioning("precip")]
        public int Precipitation { get; set; }
        //in celcius
        [Versioning("max")]
        public double TemperatureMax { get; set; }
        [Versioning("min")]
        public double TemperatureMin { get; set; }
        [Versioning("windmax")]
        public Weather.eWindSpeed WindSpeedMax { get; set; }
        [Versioning("windmin")]
        public Weather.eWindSpeed WindSpeedMin { get; set; }
        public WeatherAverage(int month, double temperatureMin, double temperatureMax,int precipitation, Weather.eWindSpeed windspeedMin, Weather.eWindSpeed windspeedMax, Country country, Town town,Airport airport)
        {
            this.Month = month;
            this.Airport = airport;
            this.Country = country;
            this.Town = town;
            this.TemperatureMin = temperatureMin;
            this.TemperatureMax = temperatureMax;
            this.WindSpeedMax = windspeedMax;
            this.WindSpeedMin = windspeedMin;
            this.Precipitation = precipitation;
        }
        public WeatherAverage(int month, double temperatureMin, double temperatureMax, int precipitation, Weather.eWindSpeed windspeedMin, Weather.eWindSpeed windspeedMax, Town town)
            : this(month, temperatureMin, temperatureMax, precipitation, windspeedMin, windspeedMax, town.Country, town,null)
        {
        }
        public WeatherAverage(int month, double temperatureMin, double temperatureMax, int precipitation, Weather.eWindSpeed windspeedMin, Weather.eWindSpeed windspeedMax, Country country)
            : this(month, temperatureMin, temperatureMax, precipitation, windspeedMin, windspeedMax, country, null,null)
        {
        }
        public WeatherAverage(int month, double temperatureMin, double temperatureMax, int precipitation, Weather.eWindSpeed windspeedMin, Weather.eWindSpeed windspeedMax, Airport airport)
            : this(month, temperatureMin, temperatureMax, precipitation, windspeedMin, windspeedMax,airport.Profile.Town.Country, airport.Profile.Town,airport)
        {
        }
          private WeatherAverage(SerializationInfo info, StreamingContext ctxt)
        {
            int version = info.GetInt16("version");

            IList<PropertyInfo> props = new List<PropertyInfo>(this.GetType().GetProperties().Where(p => p.GetCustomAttribute(typeof(Versioning)) != null && ((Versioning)p.GetCustomAttribute(typeof(Versioning))).AutoGenerated));

            foreach (SerializationEntry entry in info)
            {
                PropertyInfo prop = props.FirstOrDefault(p => ((Versioning)p.GetCustomAttribute(typeof(Versioning))).Name == entry.Name);


                if (prop != null)
                    prop.SetValue(this, entry.Value);
            }

            var notSetProps = props.Where(p => ((Versioning)p.GetCustomAttribute(typeof(Versioning))).Version > version);

            foreach (PropertyInfo prop in notSetProps)
            {
                Versioning ver = (Versioning)prop.GetCustomAttribute(typeof(Versioning));

                if (ver.AutoGenerated)
                    prop.SetValue(this, ver.DefaultValue);

            }




        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("version", 1);

            Type myType = this.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties().Where(p => p.GetCustomAttribute(typeof(Versioning)) != null));

            foreach (PropertyInfo prop in props)
            {
                object propValue = prop.GetValue(this, null);

                Versioning att = (Versioning)prop.GetCustomAttribute(typeof(Versioning));

                info.AddValue(att.Name, propValue);
            }

        }
       

    }
    //the list of weather averages
    public class WeatherAverages
    {
        private static List<WeatherAverage> averages = new List<WeatherAverage>();
        //adds a weather average to the list
        public static void AddWeatherAverage(WeatherAverage average)
        {
            averages.Add(average);
        }
        //returns the weather average for a specific airport and specific month
        public static WeatherAverage GetWeatherAverage(int month, Airport airport)
        {
            WeatherAverage airportAverage = averages.Find(w => w.Airport == airport && w.Month == month);
            WeatherAverage townAverage = averages.Find(w => w.Town == airport.Profile.Town && w.Month == month);
            WeatherAverage countryAverage = averages.Find(w => w.Country == airport.Profile.Town.Country && w.Month == month);

            if (airportAverage != null)
                return airportAverage;

            if (townAverage != null)
                return townAverage ;

            if (countryAverage != null)
                return countryAverage;
        
            return null;
        }
        //returns all weather averages with a specific match
        public static List<WeatherAverage> GetWeatherAverages(Predicate<WeatherAverage> match)
        {
            return averages.FindAll(match);
        }
        //clears the list of weather averages
        public static void Clear()
        {
            averages.Clear();
        }
    }
}
