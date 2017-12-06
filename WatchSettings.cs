using System;
using System.Collections;
using System.Xml;
using System.Reflection;

namespace SystemsMonitor
{
	/// <summary>
	/// Summary description for WatchSettings.
	/// </summary>
	public class WatchSettings
	{
		private string m_ConfigFile = "";
		private string m_Section = "";
		private Hashtable m_Settings = null;

		public WatchSettings(string Section)
		{
			m_ConfigFile = GetConfigFilename();
			m_Section = Section;

			LoadSettings();
		}

		public WatchSettings(string ConfigFile, string Section)
		{
			m_ConfigFile = ConfigFile;
			m_Section = Section;

			LoadSettings();
		}

		private void LoadSettings()
		{
			// Load the config file into the XML DOM.
			XmlDocument xmlDom = GetXmlDocument();

			// only add nodes
			System.Xml.XmlNodeList nodeList = xmlDom.SelectNodes("configuration/" + m_Section + "/add");
			m_Settings = new Hashtable();
			foreach ( System.Xml.XmlNode node in nodeList )
			{
				m_Settings.Add(node.Attributes.GetNamedItem("key").Value, node.Attributes.GetNamedItem("value").Value);
			}
		}

		public void Add(string Key, string Value)
		{
			// Load the config file into the XML DOM.
			XmlDocument xmlDom = GetXmlDocument();

			// create a new node
			System.Xml.XmlElement elem = xmlDom.CreateElement("add");
			elem.SetAttribute("key", Key);
			elem.SetAttribute("value", Value);
			
			// add the node to the section
			System.Xml.XmlNode node = xmlDom.SelectSingleNode("configuration/" + m_Section);
			node.AppendChild(elem);

			// Save the modified config file.
			xmlDom.Save(m_ConfigFile);

			m_Settings.Add(Key, Value);

			return;
		}

		public void Remove(string Key)
		{
			// Load the config file into the XML DOM.
			XmlDocument xmlDom = GetXmlDocument();

			// get parent & child
			System.Xml.XmlNode node = xmlDom.SelectSingleNode("configuration/" + m_Section + "/add[@key='" + Key + "']");
			System.Xml.XmlNode parentNode = xmlDom.SelectSingleNode("configuration/" + m_Section);

			// delete the node
			parentNode.RemoveChild(node);
			
			// Save the modified config file.
			xmlDom.Save(m_ConfigFile);

			m_Settings.Remove(Key);

			return;
		}

		public int Count()
		{
			return m_Settings.Count;
		}

		public string this[string Key]
		{
			get
			{
				return m_Settings[Key].ToString();
			}
		}

		public string this[int Index]
		{
			get
			{
				string retVal = "";
				int i = 0;

				foreach ( string key in m_Settings.Keys )
				{
					if ( i++ == Index )
					{
						retVal = m_Settings[key].ToString();
						break;
					}
				}

				return retVal;
			}
		}

		public ICollection Keys
		{
			get
			{
				return m_Settings.Keys;
			}
		}

		#region Private Methods
		private string GetConfigFilename()
		{
			// Use reflection to find the location of the config file. 
			Assembly asm = Assembly.GetExecutingAssembly();
			string configLoc = asm.Location;

			return configLoc + ".config";
		}

		private XmlDocument GetXmlDocument()
		{
			XmlDocument myDom = new XmlDocument();
			myDom.Load(m_ConfigFile);

			return myDom;
		}
		#endregion Private Methods
	}
}
