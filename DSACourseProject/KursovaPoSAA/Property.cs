using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

using System.Threading.Tasks;

namespace KursovaPoSAA
{
    public class Property
    {
        public List<object> Values { get; set; }
        public string Name { get; set; }

        public Type Type { get; set; }

        public object? DefaultValue { get; set; }


        public Property(string Name, string StrType)
        {
            this.Name = Name;

            switch (StrType.ToLower())
            {
                case "int": { Type = typeof(int); break; }
                case "string": Type = typeof(string); break;
                case "date": Type = typeof(DateOnly); break;
            }
            Values = new List<object>();
        }
        public Property(string Name)
        {
            this.Name = Name;
            Values = new List<object>();
        }
        public Property()
        {

        }

        public static Property CreateProperty(string s)
        {
            bool defaultValue = s.Contains("default") && (s = s.Replace("default ", string.Empty)) != null;

            string[] data = s.Split(new[] { ':', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string name = data[0];
            string type = data[1];
            object? defaultV = defaultValue ? data[2] : null;

            Property property = new Property(name, type);
            if (defaultV is not null)
            {
                if (defaultV is string date)
                    property.DefaultValue = date.Trim('\"');
            }

            //\"01.01.2022\"
            return property;
        }
        public void InsertData(object data)
        {
            switch (Type.ToString())
            {
                case "System.Int32":
                    {
                        if (data is not int)
                            throw new InvalidDataException($"Property {Name} expects an integer!");
                        break;
                    }
                case "System.String":
                    {
                        if (data is not string)
                            throw new InvalidDataException($"Property {Name} expects a string!");
                        break;
                    }
                case "System.DateOnly":
                    {
                        if (data is not DateOnly)
                            if (!DateTime.TryParseExact((string)data, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out _))
                                throw new InvalidDataException($"Property {Name} expects a date!");
                        break;
                    }

                default:
                    {
                        throw new InvalidDataException("How? You are only allowed 3 data types");
                    }
            }
            Values.Add(data);
        }

        // Serialize to a custom format using StreamWriter
        public void Serialize(StreamWriter sw)
        {
            // Concatenate values with commas
            string valuesString = string.Join(",", Values);

            // Write the entire line with name, type, values, defaultValue, and hasIndex
            sw.WriteLine($"{Name},{valuesString},{DefaultValue ?? "null"}");
        }

        public void Deserialize(StreamReader sr)
        {
            string[] values = sr.ReadLine().Split(',');

            Name = values[0];
            Values = ConvertStringListToTypeList(values.Skip(1).Take(values.Length - 2).ToList());

            // Parse and assign DefaultValue (handling null case)
            DefaultValue = values[values.Length - 1] == "null" ? null : values[values.Length - 1];

            // Parse and assign HasIndex
            
        }

        private List<object> ConvertStringListToTypeList(List<string> stringValues)
        {
            List<object> result = new List<object>();

            foreach (string stringValue in stringValues)
            {
                // Adjust this part based on the expected types of your values
                // Here, I'm assuming all values are strings, you should adapt it to your actual types
                result.Add(Form1.ParseValue(stringValue));
            }

            return result;
        }
               
    }
}
