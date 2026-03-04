using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;

namespace StaticClass.Extensions
{
    public static class StringExtensions
    {
        private readonly static Encoding latinizeEncoding = Encoding.GetEncoding("ISO-8859-8");

        public static string ToPrettyXml(this string xml)
        {
            try
            {
                if (!string.IsNullOrEmpty(xml))
                {
                    var stringBuilder = new StringBuilder();

                    var element = XElement.Parse(xml);

                    var settings = new XmlWriterSettings();
                    settings.OmitXmlDeclaration = false;
                    settings.Indent = true;
                    settings.NewLineOnAttributes = true;

                    using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
                    {
                        element.Save(xmlWriter);
                    }

                    return stringBuilder.ToString();
                }
                else
                {
                    return xml;
                }
            }
            catch (System.Xml.XmlException)
            {
                return xml;
            }
        }
        public static string ToPrettyJson(this string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return json;

            json = json.Trim();
            if ((json.StartsWith("{") && json.EndsWith("}")) || //For object
                (json.StartsWith("[") && json.EndsWith("]"))) //For array
            {
                try
                {
                    using var jDoc = JsonDocument.Parse(json);
                    return JsonSerializer.Serialize(jDoc, new JsonSerializerOptions { WriteIndented = true });
                }
                catch
                {
                    return json;
                }
            }
            else
            {
                return json;
            }
        }

        public static string Latinize(this string value)
        {
            var strBytes = latinizeEncoding.GetBytes(value);
            return latinizeEncoding.GetString(strBytes);
        }
    }

}
