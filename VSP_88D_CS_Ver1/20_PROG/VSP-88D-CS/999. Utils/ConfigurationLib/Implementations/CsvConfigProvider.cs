using ConfigurationLib.Interfaces;
using ConfigurationLib.Shared;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;

namespace ConfigurationLib.Implementations
{
    public class CsvConfigProvider<T> : IConfigProvider<T> where T : class, new()
    {
        private readonly string _filePath;

        public ConfigStorageType ProviderType => ConfigStorageType.Csv;

        public CsvConfigProvider(string filePath)
        {
            _filePath = filePath;
        }

        public async Task<T?> LoadAsync()
        {
            if (!File.Exists(_filePath))
                return new T();

            var lines = await File.ReadAllLinesAsync(_filePath);
            if (lines.Length < 2)
                return default;

            var headers = lines[0].Split(',').Select(h => h.Trim()).ToList();
            var dataLines = lines.Skip(1).ToList();

            var tType = typeof(T);
            if (tType.IsGenericType && tType.GetGenericTypeDefinition() == typeof(List<>))
            {
                return (T?)LoadObjectCollection(tType, headers, dataLines);
            }
            else
            {
                return (T?)LoadObject(tType, headers, dataLines.First());
            }
        }

        private object? LoadObjectCollection(Type listType, List<string> headers, List<string> dataLines)
        {
            var itemType = listType.GetGenericArguments()[0];
            var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType))!;
            var properties = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var line in dataLines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var obj = LoadObject(itemType, headers, line);
                if (obj != null)
                    list.Add(obj);
            }

            return list;
        }

        private object? LoadObject(Type objectType, List<string> headers, string line)
        {
            var values = line.Split(',');
            var properties = objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var obj = Activator.CreateInstance(objectType);
            if (obj == null) return null;

            for (int j = 0; j < headers.Count && j < values.Length; j++)
            {
                var header = headers[j];
                var value = values[j].Trim();

                var prop = properties.FirstOrDefault(p =>
                    string.Equals(p.Name, header, StringComparison.OrdinalIgnoreCase));

                if (prop != null && prop.CanWrite)
                {
                    try
                    {
                        object converted = Convert.ChangeType(value, prop.PropertyType);
                        prop.SetValue(obj, converted);
                    }
                    catch
                    {
                        Debug.WriteLine($"Error mapping: '{value}' -> {prop.Name}");
                    }
                }
            }

            return obj;
        }

        public async Task SaveAsync(T config)
        {
            var tType = typeof(T);

            if (tType.IsGenericType && (tType.GetGenericTypeDefinition() == typeof(List<>) || tType.GetGenericTypeDefinition() == typeof(ObservableCollection<>)))
            {
                var csvLines = SaveObjectCollection(config);
                try
                {
                    await File.WriteAllLinesAsync(_filePath, csvLines);
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                var csvLines = SaveObject(config!);
                await File.WriteAllLinesAsync(_filePath, csvLines);
            }
        }

        private List<string> SaveObjectCollection(object listObject)
        {
            var listType = listObject.GetType();
            var itemType = listType.GetGenericArguments()[0];
            var properties = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var lines = new List<string>();

            var headerLine = string.Join(",", properties.Select(p => p.Name));
            lines.Add(headerLine);

            foreach (var item in (IEnumerable<object>)listObject)
            {
                var values = properties.Select(p =>
                {
                    var val = p.GetValue(item);
                    return EscapeCsv(val);
                });

                lines.Add(string.Join(",", values));
            }

            return lines;
        }

        private List<string> SaveObject(object obj)
        {
            var tType = obj.GetType();
            var properties = tType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var lines = new List<string>();

            var headerLine = string.Join(",", properties.Select(p => p.Name));
            var valueLine = string.Join(",", properties.Select(p => EscapeCsv(p.GetValue(obj))));

            lines.Add(headerLine);
            lines.Add(valueLine);

            return lines;
        }

        private string EscapeCsv(object? value)
        {
            if (value == null) return "";

            var str = value.ToString() ?? "";

            if (str.Contains(",") || str.Contains("\""))
            {
                str = str.Replace("\"", "\"\"");
                return $"\"{str}\"";
            }

            return str;
        }
    }
}
