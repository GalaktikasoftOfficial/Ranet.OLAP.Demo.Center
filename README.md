# Ranet.OLAP.Demo.Center
Ranet OLAP Browser and other Visual Controls for XMLA datasources


Ranet.Demo.Core\RanetDemoApp.cs

```csharp
protected virtual void OnApplicationStartup()
{
	Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
	RanetThemeManager.CurrentTheme = RanetThemes.Office;
}
```


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


Ranet.Demo.Core\TreeViewDemo.xaml.cs

private void GetModules()


+		assemblyDemo	{KPIViewer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null}	System.Reflection.Assembly {System.Reflection.RuntimeAssembly}
+		assemblyDemo	{MdxDesigner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null}	System.Reflection.Assembly {System.Reflection.RuntimeAssembly}
+		assemblyDemo	{MdxDynamic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null}	System.Reflection.Assembly {System.Reflection.RuntimeAssembly}
+		assemblyDemo	{Choices, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null}	System.Reflection.Assembly {System.Reflection.RuntimeAssembly}
+		assemblyDemo	{PivotGrid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null}	System.Reflection.Assembly {System.Reflection.RuntimeAssembly}
+		assemblyDemo	{Gauges, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null}	System.Reflection.Assembly {System.Reflection.RuntimeAssembly}
+		assemblyDemo	{Integrations, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null}	System.Reflection.Assembly {System.Reflection.RuntimeAssembly}
+		assemblyDemo	{ServerExplorer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null}	System.Reflection.Assembly {System.Reflection.RuntimeAssembly}
+		assemblyDemo	{ServerExplorer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null}	System.Reflection.Assembly {System.Reflection.RuntimeAssembly}

