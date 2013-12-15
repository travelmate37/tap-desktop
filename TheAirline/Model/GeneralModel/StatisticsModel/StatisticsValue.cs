﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;


namespace TheAirline.Model.GeneralModel.StatisticsModel
{
    [Serializable]
    public class StatisticsValue : ISerializable
    {
        [Versioning("stat")]
        public StatisticsType Stat { get; set; }
        [Versioning("year")]
        public int Year { get; set; }
        [Versioning("value")]
        public double Value { get; set; }
        public StatisticsValue(int year,StatisticsType stat, double value)
        {
            this.Value = value;
            this.Stat = stat;
            this.Year = year;
        }
             protected StatisticsValue(SerializationInfo info, StreamingContext ctxt)
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
