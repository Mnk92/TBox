using System;
using System.Drawing;
using System.Reflection;
using System.Resources;

namespace Mnk.TBox.Core.Contracts
{
    [AttributeUsage(AttributeTargets.Class)]
	public sealed class PluginInfoAttribute : Attribute
	{
		private readonly ResourceManager textsResMan;
		private readonly ResourceManager iconsResMan;

		public PluginInfoAttribute(Type textResourceManagerType, Type iconsResourceManagerType, PluginGroup group):this(textResourceManagerType, -1, group)
		{
			textsResMan = GetResourceManager(textResourceManagerType);
			iconsResMan = GetResourceManager(iconsResourceManagerType);
		}

		public PluginInfoAttribute(Type textResourceManagerType, int systemIconId, PluginGroup group)
		{
			PluginGroup = group;
			textsResMan = GetResourceManager(textResourceManagerType);
			SystemIconId = systemIconId;
		}

		private static ResourceManager GetResourceManager(Type type)
		{
			var p = type.GetProperty("ResourceManager", BindingFlags.Static | BindingFlags.NonPublic);
			if (p == null) p = type.GetProperty("ResourceManager", BindingFlags.Static | BindingFlags.Public);
			if (p == null) throw new ArgumentException("Invalid resouce manager " + type);
			return (ResourceManager) p.GetValue(p.DeclaringType, null);
		}

		public string Name
		{
			get { return textsResMan.GetString("PluginName"); }
		}

		public string Description
		{
			get { return textsResMan.GetString("PluginDescription"); } }

		public Icon Icon
		{
			get { return (Icon) iconsResMan.GetObject("Icon");}
		}

		public bool IsIconSystem
		{
			get { return SystemIconId > -1; }
		}

		public int SystemIconId
		{
			get; private set;
		}

		public PluginGroup PluginGroup { get; private set; }
	}
}
