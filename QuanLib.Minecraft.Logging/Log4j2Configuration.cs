using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace QuanLib.Minecraft.Logging
{
    public class Log4j2Configuration
    {
        private Log4j2Configuration(XmlDocument xmlDocument)
        {
            ArgumentNullException.ThrowIfNull(xmlDocument, nameof(xmlDocument));

            XmlDocument = xmlDocument;
        }

        public XmlDocument XmlDocument { get; }

        public void AddRegexFilter(string regex, MatchBehavior onMatch, MatchBehavior onMismatch)
        {
            XmlElement regexFilter = XmlDocument.CreateElement("RegexFilter");
            regexFilter.SetAttribute("regex", regex);
            regexFilter.SetAttribute("onMatch", onMatch.ToString());
            regexFilter.SetAttribute("onMismatch", onMismatch.ToString());

            XmlNode? filtersNode = XmlDocument.SelectSingleNode("//filters");
            filtersNode?.AppendChild(regexFilter);
        }

        public void DisableFileOutput()
        {
            CommentNode("//RollingRandomAccessFile");
            CommentNode("//AppenderRef[@ref='File']");
        }

        private void CommentNode(string xpath)
        {
            XmlNode? node = XmlDocument.SelectSingleNode(xpath);
            if (node is not null)
            {
                XmlComment comment = XmlDocument.CreateComment(node.OuterXml);
                node.ParentNode?.ReplaceChild(comment, node);
            }
        }

        public void Save(Stream output)
        {
            using XmlWriter xmlWriter = XmlWriter.Create(output, CreateXmlWriterSettings());
            XmlDocument.Save(xmlWriter);
        }

        public void Save(string outputFileName)
        {
            using XmlWriter xmlWriter = XmlWriter.Create(outputFileName, CreateXmlWriterSettings());
            XmlDocument.Save(xmlWriter);
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new();
            using XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, CreateXmlWriterSettings());
            XmlDocument.Save(xmlWriter);
            return stringBuilder.ToString();
        }

        private static XmlWriterSettings CreateXmlWriterSettings()
        {
            return new XmlWriterSettings()
            {
                Encoding = Encoding.UTF8,
                Indent = true,
                IndentChars = "    ",
                NewLineChars = "\n",
                NewLineHandling = NewLineHandling.Replace,
                NewLineOnAttributes = false
            };
        }

        public static Log4j2Configuration Load()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using Stream stream = assembly.GetManifestResourceStream(assembly.GetName().Name + ".SystemResource.log4j2.xml") ?? throw new InvalidOperationException();
            using StreamReader reader = new(stream, Encoding.UTF8);

            XmlDocument xmlDocument = new();
            xmlDocument.Load(reader);
            return new(xmlDocument);
        }

        public static string LoadXml()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using Stream stream = assembly.GetManifestResourceStream(assembly.GetName().Name + ".SystemResource.log4j2.xml") ?? throw new InvalidOperationException();
            using StreamReader reader = new(stream, Encoding.UTF8);
            return reader.ReadToEnd();
        }
    }
}
