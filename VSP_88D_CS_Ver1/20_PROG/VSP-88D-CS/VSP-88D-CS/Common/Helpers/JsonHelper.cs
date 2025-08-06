using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSP_88D_CS.Common.Helpers
{
    public static class JsonHelper
    {
        public static T DeepCloneObject<T>(this T input) where T : new()
        {
            try
            {
                var jsonString = JsonConvert.SerializeObject(input, Formatting.Indented);
                return JsonConvert.DeserializeObject<T>(jsonString) ?? new();
            }
            catch (Exception)
            {
                return new();
            }
        }

        public static T1 DeepCloneObject<T1, T2>(this T2 input) where T1 : new() where T2 : new()
        {
            try
            {
                if (input == null)
                    return new T1();

                var jsonString = JsonConvert.SerializeObject(input, Formatting.Indented);
                return JsonConvert.DeserializeObject<T1>(jsonString) ?? new T1();
            }
            catch (Exception)
            {
                return new T1();
            }
        }

        public static T SafeDeserializeJSON<T>(string jsonString) where T : new()
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(jsonString) ?? new();
            }
            catch (Exception)
            {
                return new();
            }
        }

        public static string SafeSerializeJSON<T>(this T input) where T : new()
        {
            string jsonString;
            try
            {
                jsonString = JsonConvert.SerializeObject(input, Formatting.Indented);
            }
            catch (Exception)
            {
                jsonString = JsonConvert.SerializeObject(new(), Formatting.Indented);
            }
            return jsonString;
        }

        public static bool CompareJson<T>(this T input, T other) where T : new()
        {
            try
            {
                return JsonConvert.SerializeObject(input, Formatting.None) == JsonConvert.SerializeObject(other, Formatting.None);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
