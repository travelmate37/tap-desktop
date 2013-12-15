﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using TheAirline.Model.AirlinerModel.RouteModel;
using TheAirline.Model.GeneralModel;

namespace TheAirline.Model.AirlineModel.SubsidiaryModel
{
    //the class for a subsidiary airline for an airline
    [Serializable]
    public class SubsidiaryAirline :  Airline
    {
        [Versioning("airline")]
        public Airline Airline { get; set; }
        public SubsidiaryAirline(Airline airline,AirlineProfile profile, AirlineMentality mentality, AirlineFocus market, AirlineLicense license,Route.RouteType routefocus)
            : base(profile, mentality, market,license,routefocus)
        {
            this.Airline = airline;

            foreach (AirlineLogo logo in this.Airline.Profile.Logos)
                this.Profile.addLogo(logo);
        }
        public override bool isHuman()
        {
            return this.Airline != null && this.Airline.isHuman();
        }
        public override bool isSubsidiaryAirline()
        {
            return this.Airline != null;
        }


        private SubsidiaryAirline(SerializationInfo info, StreamingContext ctxt) : base(info,ctxt)
        {
            int version = info.GetInt16("version");

            IList<PropertyInfo> props = new List<PropertyInfo>(this.GetType().GetProperties().Where(p => p.GetCustomAttribute(typeof(Versioning)) != null && ((Versioning)p.GetCustomAttribute(typeof(Versioning))).AutoGenerated));

            foreach (SerializationEntry entry in info)
            {
                PropertyInfo prop = props.FirstOrDefault(p => ((Versioning)p.GetCustomAttribute(typeof(Versioning))).Name == entry.Name);


                if (prop != null)
                    prop.SetValue(this,entry.Value);
            }

            var notSetProps = props.Where(p => ((Versioning)p.GetCustomAttribute(typeof(Versioning))).Version > version);

            foreach (PropertyInfo prop in notSetProps)
            {
                Versioning ver = (Versioning)prop.GetCustomAttribute(typeof(Versioning));

                if (ver.AutoGenerated)
                    prop.SetValue(this, ver.DefaultValue);
               
            }
           
            
          
          
        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context)
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

            base.GetObjectData(info, context);

        }
    }
}
