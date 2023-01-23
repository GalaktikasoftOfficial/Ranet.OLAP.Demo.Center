# Ranet OLAP Demo Configuration

Ranet.Demo.Core\Configuration\DemoConfiguration.cs 

```csharp
private ConfigurationInfo SetDefaulSettings()
{
	var storage = new ConnectionStringStorage();
	storage.Add("Default Connection", "Provider=MSOLAP.4; Data Source=https://bi.galaktika-soft.com/olap/2012/msmdpump.dll; Catalog=AdventureWorksDW2012 MD-EE;");
	storage.Add("AdventureWorks TM 2012", "Provider=MSOLAP.4; Data Source=https://bi.galaktika-soft.com/tm/2012/msmdpump.dll; Catalog=AdventureWorks Tabular Model SQL 2012;");
	var configInfo = new ConfigurationInfo {SelectedIndex = 0, Storage = storage, CubeName = "Adventure Works"};
	return configInfo;
}
```

D:\GitHub\Ranet.OLAP.Demo.Center\Ranet.DemoCenter\bin\Debug\net48\connectionData.json
