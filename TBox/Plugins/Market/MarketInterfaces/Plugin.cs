using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Mnk.TBox.Plugins.Market.Interfaces
{
	[DataContract]
	public class Plugin
	{
		[DataMember, XmlAttribute]public string Name;
		[DataMember, XmlAttribute]public string Author;
		[DataMember, XmlAttribute]public DateTime Date;
		[DataMember, XmlAttribute]public string Description;
		[DataMember, XmlAttribute]public long Size;
		[DataMember, XmlAttribute]public long[] Dependenses;
		[DataMember, XmlAttribute]public long Downloads;
		[DataMember, XmlAttribute]public long Uploads;
		[DataMember, XmlAttribute]public string Type;
		[DataMember, XmlAttribute]public bool IsPlugin;
	}
}
