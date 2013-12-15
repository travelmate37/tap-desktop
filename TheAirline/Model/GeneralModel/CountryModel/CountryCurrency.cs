﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace TheAirline.Model.GeneralModel.CountryModel
{
    //the class for the currency for a country
    [Serializable]
    public class CountryCurrency : ISerializable
    {
        public enum CurrencyPosition {Left, Right}
        [Versioning("symbol")]   
        public string CurrencySymbol { get; set; }
        [Versioning("position")]
        public CurrencyPosition Position { get; set; }
        //dollars to currency
        [Versioning("rate")]
        public double Rate { get; set; }
        [Versioning("from")]
        public DateTime DateFrom { get; set; }
        [Versioning("to")]
        public DateTime DateTo { get; set; }
        public CountryCurrency(DateTime datefrom, DateTime dateto, string currencysymbol,CurrencyPosition position,  double rate)
        {
            this.DateFrom = datefrom;
            this.DateTo = dateto;
            this.CurrencySymbol = currencysymbol;
            this.Rate = rate;
            this.Position = position;
        }
             private CountryCurrency(SerializationInfo info, StreamingContext ctxt)
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
}
