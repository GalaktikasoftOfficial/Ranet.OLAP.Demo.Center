using Ranet.Olap.Themes;
using Ranet.Olap.Core.Storage;

namespace Ranet.Demo.Core.Configuration
{
	public class ConfigurationInfo
	{
		private string _cubeName;

		private string _culture;

		private string _currentTheme;
		public int SelectedIndex;

		public ConnectionStringStorage Storage = new ConnectionStringStorage();

		private ConfigurationInfo(ConnectionStringStorage storage, int selectedIndex, string cubeName)
		{
			Storage = storage;
			SelectedIndex = selectedIndex;
			CubeName = cubeName;
		}

		public ConfigurationInfo()
		{
		}

		public string CubeName
		{
			get { return _cubeName ?? "Adventure Works"; }
			set { _cubeName = value; }
		}

		public string CurrentTheme
		{
			get
			{
				if (_currentTheme == null)
					return RanetThemes.MetroLight.Name;
				return _currentTheme;
			}
			set { _currentTheme = value; }
		}

		public string Culture
		{
			get
			{
				if (_culture == null)
					return "en-US";
				return _culture;
			}
			set { _culture = value; }
		}


		protected bool Equals(ConfigurationInfo other)
		{
			return Equals(Storage, other.Storage) && SelectedIndex == other.SelectedIndex && CubeName == other.CubeName;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((ConfigurationInfo) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (((Storage != null ? Storage.GetHashCode() : 0)*397) ^ SelectedIndex) ^
				       (CubeName != null ? CubeName.GetHashCode() : 0);
			}
		}


		public ConfigurationInfo Clone()
		{
			return CloneConfigurationInfo();
		}

		private ConfigurationInfo CloneConfigurationInfo()
		{
			return new ConfigurationInfo(Storage.Clone(), SelectedIndex, CubeName);
		}
	}
}