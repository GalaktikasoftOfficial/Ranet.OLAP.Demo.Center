using System;

namespace Ranet.Demo.Core.Configuration
{
    public interface IConfiguration
    {
        void Save(ConfigurationInfo data);
        ConfigurationInfo Load();

        event EventHandler ConfigurationDataChanged;
    }
}
