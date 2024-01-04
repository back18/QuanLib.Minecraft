using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Mod.Fabric
{
    public static class JsonFormatter
    {
        public static string Format(string json)
        {
            JObject jObject = JObject.Parse(json);

            FormatPersons(jObject, "authors");
            FormatPersons(jObject, "contributors");
            FormatDependencies(jObject, "depends");
            FormatDependencies(jObject, "recommends");
            FormatDependencies(jObject, "suggests");
            FormatDependencies(jObject, "conflicts");
            FormatDependencies(jObject, "breaks");
            FormatMixin(jObject, "mixins");

            return jObject.ToString();
        }

        private static void FormatPersons(JObject jObject, string propertyName)
        {
            JArray? persons = jObject.GetValueAs<JArray>(propertyName);
            if (persons is not null)
            {
                List<(JToken oldValue, JObject newValue)> objs = [];
                foreach (var jToken in persons)
                {
                    if (jToken is JValue jValue && jValue.Value is string value)
                    {
                        JObject person = new();
                        person.Add("name", value);
                        person.Add("contact", new JObject());
                        objs.Add((jToken, person));
                    }
                }

                foreach (var obj in objs)
                {
                    persons.Remove(obj.oldValue);
                    persons.Add(obj.newValue);
                }
            }
        }

        private static void FormatDependencies(JObject jObject, string propertyName)
        {
            JObject? dependencies = jObject.GetValueAs<JObject>(propertyName);
            if (dependencies is not null)
            {
                Dictionary<string, JArray> arrays = [];
                foreach (var item in dependencies)
                {
                    if (item.Value is JValue jValue && jValue.Value is string value)
                        arrays.Add(item.Key, new JArray(value));
                }

                foreach (var array in arrays)
                {
                    dependencies[array.Key] = array.Value;
                }
            }
        }

        private static void FormatMixin(JObject jObject, string propertyName)
        {
            JArray? mixins = jObject.GetValueAs<JArray>(propertyName);
            if (mixins is not null)
            {
                List<(JToken oldValue, JObject newValue)> objs = [];
                foreach (var jToken in mixins)
                {
                    if (jToken is JValue jValue && jValue.Value is string value)
                    {
                        JObject mixin = new();
                        mixin.Add("contact", new JValue(value));
                        mixin.Add("environment", "*");
                        objs.Add((jToken, mixin));
                    }
                }

                foreach (var obj in objs)
                {
                    mixins.Remove(obj.oldValue);
                    mixins.Add(obj.newValue);
                }
            }
        }

        private static T? GetValueAs<T>(this JObject source, string? propertyName) where T : JToken
        {
            if (propertyName is not null && source.TryGetValue(propertyName, out var jToken) && jToken is T value)
                return value;
            else
                return null;
        }
    }
}
