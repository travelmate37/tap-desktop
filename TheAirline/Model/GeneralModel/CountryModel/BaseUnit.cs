﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace TheAirline.Model.GeneralModel.CountryModel
{
    //the base unit for countries and union members
    [Serializable]
    public class BaseUnit : ISerializable
    {

        [Versioning("uid")]
        public string Uid { get; set; }

        [Versioning("shortname")]
        public string ShortName { get; set; }

        [Versioning("flag")]
        public string Flag { get; set; }
        public BaseUnit(string uid, string shortname)
        {
            this.Uid = uid;
            this.ShortName = shortname;
        }
        public virtual string Name
        {
            get { return Translator.GetInstance().GetString(Country.Section, this.Uid); }
        }
        public static bool operator ==(BaseUnit a, BaseUnit b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }
            // if one is union and the other is not
            if ((a is Union && b is Country) || (a is Country && b is Union))
                return false;

            if (a is Union && b is Union)
                return a.Uid == b.Uid;
                      
            // Return true if the fields match:
            if (a is TerritoryCountry && b is TerritoryCountry)
            {
                return a.Uid == b.Uid || ((TerritoryCountry)a).MainCountry.Uid == b.Uid || a.Uid == ((TerritoryCountry)b).MainCountry.Uid || ((TerritoryCountry)a).MainCountry.Uid == ((TerritoryCountry)b).MainCountry.Uid;
            }
            if (a is TerritoryCountry)
            {
                return a.Uid == b.Uid || ((TerritoryCountry)a).MainCountry.Uid == b.Uid;
            }
            if (b is TerritoryCountry)
            {
                return a.Uid == b.Uid || a.Uid == ((TerritoryCountry)b).MainCountry.Uid;
            }

            return a.Uid == b.Uid;//a.x == b.x && a.y == b.y && a.z == b.z;
        }

        public static bool operator !=(BaseUnit a, BaseUnit b)
        {
            return !(a == b);
        }
        public override int GetHashCode()
        {
            return this.Uid.GetHashCode() ^ this.ShortName.GetHashCode();
        }
        public override bool Equals(object u)
        {
            // If parameter is null return false:
            if ((object)u == null || !(u is BaseUnit))
            {
                return false;
            }
           
            // Return true if the fields match:
            return (this.Uid == ((BaseUnit)u).Uid);
        }
             protected BaseUnit(SerializationInfo info, StreamingContext ctxt)
        {
            int version = info.GetInt16("version");

            var fields = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Where(p => p.GetCustomAttribute(typeof(Versioning)) != null);

            IList<PropertyInfo> props = new List<PropertyInfo>(this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Where(p => p.GetCustomAttribute(typeof(Versioning)) != null));

            var propsAndFields = props.Cast<MemberInfo>().Union(fields.Cast<MemberInfo>());

            foreach (SerializationEntry entry in info)
            {
                MemberInfo prop = propsAndFields.FirstOrDefault(p => ((Versioning)p.GetCustomAttribute(typeof(Versioning))).Name == entry.Name);


                if (prop != null)
                {
                    if (prop is FieldInfo)
                        ((FieldInfo)prop).SetValue(this, entry.Value);
                    else
                        ((PropertyInfo)prop).SetValue(this, entry.Value);
                }
            }

            var notSetProps = propsAndFields.Where(p => ((Versioning)p.GetCustomAttribute(typeof(Versioning))).Version > version);

            foreach (MemberInfo notSet in notSetProps)
            {
                Versioning ver = (Versioning)notSet.GetCustomAttribute(typeof(Versioning));

                if (ver.AutoGenerated)
                {
                    if (notSet is FieldInfo)
                        ((FieldInfo)notSet).SetValue(this, ver.DefaultValue);
                    else
                        ((PropertyInfo)notSet).SetValue(this, ver.DefaultValue);

                }

            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("version", 1);

            Type myType = this.GetType();

            var fields = myType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Where(p => p.GetCustomAttribute(typeof(Versioning)) != null);

            IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Where(p => p.GetCustomAttribute(typeof(Versioning)) != null));

            var propsAndFields = props.Cast<MemberInfo>().Union(fields.Cast<MemberInfo>());

            foreach (MemberInfo member in propsAndFields)
            {
                object propValue;

                if (member is FieldInfo)
                    propValue = ((FieldInfo)member).GetValue(this);
                else
                    propValue = ((PropertyInfo)member).GetValue(this, null);

                Versioning att = (Versioning)member.GetCustomAttribute(typeof(Versioning));

                info.AddValue(att.Name, propValue);
            }
        }
    }
}
