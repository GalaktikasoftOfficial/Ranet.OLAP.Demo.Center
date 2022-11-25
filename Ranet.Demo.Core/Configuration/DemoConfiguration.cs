using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Windows;
using Ranet.Olap.Core.Storage;

namespace Ranet.Demo.Core.Configuration
{
	public class DemoConfiguration : IConfiguration
	{
		#region Fields

		private ConfigurationInfo _config;

		#endregion

		#region Properties

		public ConfigurationInfo ConfigurationData
		{
			get { return _config; }
			set
			{
				_config = value;
				OnConfigurationDataChanged();
			}
		}

		#endregion

		#region  Methods

		private string GetConnectionFilePath()
		{
			var path = Path.GetDirectoryName(Application.Current.GetType().Assembly.Location);
			if (string.IsNullOrEmpty(path)) return string.Empty;
			return Path.Combine(path, "connectionData.json");
		}


		protected virtual void OnConfigurationDataChanged()
		{
			var handler = ConfigurationDataChanged;
			if (handler != null) handler(this, EventArgs.Empty);
		}

		private ConfigurationInfo SetDefaulSettings()
		{
			var storage = new ConnectionStringStorage();
			storage.Add("Default Connection", "Provider=MSOLAP.4; Data Source=https://bi.galaktika-soft.com/olap/2012/msmdpump.dll; Catalog=AdventureWorksDW2012 MD-EE;");
			storage.Add("AdventureWorks TM 2012", "Provider=MSOLAP.4; Data Source=https://bi.galaktika-soft.com/tm/2012/msmdpump.dll; Catalog=AdventureWorks Tabular Model SQL 2012;");
			var configInfo = new ConfigurationInfo {SelectedIndex = 0, Storage = storage, CubeName = "Adventure Works"};
			return configInfo;
		}

		#endregion

		#region Interface Implementations

		#region IConfiguration

		public void Save(ConfigurationInfo data)
		{
			var path = GetConnectionFilePath();
			if (string.IsNullOrEmpty(path)) return;
			using (var stream = new FileStream(path, FileMode.OpenOrCreate))
			{
				stream.SetLength(0);
				data.SerializeData(stream);
				ConfigurationData = data;
			}
		}

		public ConfigurationInfo Load()
		{
			var filePath = GetConnectionFilePath();
			if (!File.Exists(filePath)) return SetDefaulSettings();

			if (ConfigurationData != null) return ConfigurationData;
			using (var stream = new FileStream(filePath, FileMode.Open))
				return SerializationHelper.DeserializeData<ConfigurationInfo>(stream);
		}

		public event EventHandler ConfigurationDataChanged;

		#endregion

		#endregion
	}

	public static class SerializationHelper
	{
		#region  Methods

		public static T DeserializeData<T>(Stream streamObject)
		{
			if (streamObject == null)
				return default(T);

			var ser = new DataContractJsonSerializer(typeof(T));
			return (T) ser.ReadObject(streamObject);
		}

		public static void SerializeData<T>(this T obj, Stream streamObject)
		{
			if (obj == null || streamObject == null)
				return;

			var ser = new DataContractJsonSerializer(typeof(T));
			ser.WriteObject(streamObject, obj);
		}

		#endregion
	}
}