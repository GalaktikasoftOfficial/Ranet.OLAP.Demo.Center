# Ranet UICulture and Theme Manager

Ranet.Demo.Core\RanetDemoApp.cs

```csharp
protected virtual void OnApplicationStartup()
{
	Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
	RanetThemeManager.CurrentTheme = RanetThemes.Office;
}
```
