using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Ranet.Demo.Core.Configuration;
using Ranet.Olap.Core.Extensions;
using Ranet.Olap.Themes;
using Ranet.Olap.UI;
using Ranet.Olap.UI.Controls;
using Ranet.Olap.UI.Extensions;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Utils;
using Zaaml.Theming;
using Zaaml.UI.Windows;

namespace Ranet.Demo.Core
{
	public partial class TreeViewDemo
	{
		private ConfigurationInfo _configInfo;
		private readonly IConfiguration _icoConfiguration;

		public static readonly DependencyProperty CurrentDemoProperty = DependencyProperty.Register(
			"CurrentDemo", typeof(DemoControl), typeof(TreeViewDemo), new PropertyMetadata(null, OnCurrentDemoPropertyChanged));

		public static readonly DependencyProperty ShowTreeProperty = DependencyProperty.Register(
			"ShowTree", typeof(bool), typeof(TreeViewDemo), new PropertyMetadata(true));

		public bool ShowTree
		{
			get => (bool)GetValue(ShowTreeProperty);
			set => SetValue(ShowTreeProperty, value);
		}

		private static void OnCurrentDemoPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((TreeViewDemo)d).OnCurrentDemoChanged();
		}

		private void OnCurrentDemoChanged()
		{
			ContentHost.Content = CurrentDemo;
		}

		public DemoControl CurrentDemo
		{
			get => (DemoControl)GetValue(CurrentDemoProperty);
			set => SetValue(CurrentDemoProperty, value);
		}

		public TreeViewDemo()
		{
			ConnectionServiceProvider.Instance.RegisterService<IConfiguration>(new DemoConfiguration());

			_icoConfiguration = ConnectionServiceProvider.Instance.GetServiceExt<IConfiguration>();
			_configInfo = _icoConfiguration.Load();
			System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(_configInfo.Culture);
			RanetThemeManager.CurrentTheme = GetUserTheme();
			InitializeComponent();
			GetModules();
		}

		private RanetTheme GetUserTheme()
		{
			if (_configInfo.CurrentTheme.Contains(Themes.MetroDark.Name)) 
				return RanetThemes.MetroDark;

			if (_configInfo.CurrentTheme.Contains(Themes.MetroLight.Name)) 
				return RanetThemes.MetroLight;

			if (_configInfo.CurrentTheme.Contains(Themes.MetroOffice.Name))
				return RanetThemes.Office;

			return null;
		}
		private void GetModules()
		{
			var assembly = Assembly.GetEntryAssembly();
			var files = assembly.EnumerateEmbeddedResources().ToList();
			var demoAssemblies = new List<Assembly>(AppDomain.CurrentDomain.GetAssemblies());

			foreach (var file in files)
			{
				if (file.Contains("demomodules"))
				{
					var dllStream = UnzipFromStream(assembly.GetResourceStream(file));

					if (dllStream == null)
						continue;

					dllStream.Seek(0, SeekOrigin.Begin);

					var ms = new MemoryStream();

					dllStream.CopyTo(ms);
					ms.Flush();

					var assemblyDemo = Assembly.Load(ms.ToArray());

					demoAssemblies.Add(assemblyDemo);
				}
			}

			GetModules(demoAssemblies);
		}

		private void GetModules(IEnumerable<Assembly> assemblies)
		{
			foreach (var assemblyDemo in assemblies)
			{
				var demoModuleAttributes = assemblyDemo.GetCustomAttributes(typeof(DemoModuleAttribute), false);

				if (demoModuleAttributes.Length == 0)
					continue;

				var demoModuleAttribute = (DemoModuleAttribute) demoModuleAttributes[0];
				var moduleItem = new TreeViewItem {Header = demoModuleAttribute.ModuleName, Tag = demoModuleAttribute};

				TreeView.Items.Add(moduleItem);

				var categoryDictionary = new Dictionary<DemoCategoryAttribute, TreeViewItem>();
				var categoryNameDictionary = new Dictionary<string, TreeViewItem>(StringComparer.OrdinalIgnoreCase);
				var categoryAttributes = assemblyDemo.GetCustomAttributes(typeof(DemoCategoryAttribute), false);

				// Build categories
				foreach (var categoryAttribute in categoryAttributes.Cast<DemoCategoryAttribute>())
				{
					var categoryItem = new TreeViewItem {Header = categoryAttribute.CategoryName, Tag = categoryAttribute};

					categoryDictionary[categoryAttribute] = categoryItem;
					categoryNameDictionary[categoryAttribute.CategoryName] = categoryItem;
				}

				var sortItemsHashSet = new HashSet<TreeViewItem>();

				// Bind categories
				foreach (var categoryAttribute in categoryAttributes.Cast<DemoCategoryAttribute>())
				{
					var categoryItem = categoryDictionary[categoryAttribute];

					if (categoryAttribute.ParentCategory == null)
						moduleItem.Items.Add(categoryItem);
					else
					{
						var parentItem = categoryNameDictionary.GetValueOrDefault(categoryAttribute.ParentCategory);
						if (parentItem != null)
						{
							parentItem.Items.Add(categoryItem);
							sortItemsHashSet.Add(parentItem);
						}
					}
				}

				// Sort categories
				foreach (var treeViewItem in sortItemsHashSet)
				{
					var items = treeViewItem.Items.Cast<TreeViewItem>().ToList();

					treeViewItem.Items.Clear();
					treeViewItem.Items.AddRange(items.OrderBy(i => ((DemoCategoryAttribute) i.Tag).Index));
				}

				var controlsDictionary = new Dictionary<TreeViewItem, List<DemoControl>>();

				foreach (var demoType in assemblyDemo.GetTypes().Where(p => p.HasAttribute<DemoAttribute>(false)))
				{
					var demoAttribute = (DemoAttribute) demoType.GetCustomAttributes(typeof(DemoAttribute), false)[0];
					var demoParent =
						(demoAttribute.Category != null
							? categoryNameDictionary.GetValueOrDefault(demoAttribute.Category)
							: null) ?? moduleItem;
					var demoControl = (DemoControl) Activator.CreateInstance(demoType);

					controlsDictionary.GetValueOrCreate(demoParent, () => new List<DemoControl>()).Add(demoControl);
					demoControl.IsInDemoCenter = true;
				}

				// Load sorted demo controls
				foreach (var kv in controlsDictionary)
				{
					foreach (var demoControl in kv.Value.OrderBy(c => c.GetType().GetAttribute<DemoAttribute>().Index))
					{
						var demoItem = new TreeViewItem {Header = demoControl.ControlName, Tag = demoControl};
						kv.Key.Items.Add(demoItem);
					}
				}
			}

			// Sort demo modules
			var moduleItems = TreeView.Items.Cast<TreeViewItem>().ToList();

			TreeView.Items.Clear();
			TreeView.Items.AddRange(moduleItems.OrderBy(i => ((DemoModuleAttribute)i.Tag).Index));
		}

		private static Stream UnzipFromStream(Stream zipStream)
		{
			var zipInputStream = new ZipInputStream(zipStream);
			var zipEntry = zipInputStream.GetNextEntry();

			while (zipEntry != null)
			{
				var entryFileName = zipEntry.Name;
				if (entryFileName.Contains("Source"))
				{
					if (entryFileName.Contains(".dll") || entryFileName.Contains(".exe"))
					{
						var memory = new MemoryStream();
						var buffer = new byte[4096];

						StreamUtils.Copy(zipInputStream, memory, buffer);
						return memory;
					}
				}
				zipEntry = zipInputStream.GetNextEntry();
			}

			return null;
		}

		public void ShowConfigurationWindow()
		{
			_configureWindow = new FloatingDialog
			{
				Title = "Configuration",
				Buttons = FloatingDialogButtons.OkCancel,
				WindowStartupLocation = WindowStartupLocation.CenterScreen,
				MinHeight = 250,
				MinWidth = 350,
				Height = 300,
				Width = 500,
				Content = new ConfigurationWindow(_configInfo.Clone())
			}.InitWindow(this);
			_configureWindow.Closing += ConfigurationWindow_Closing;
			_configureWindow.ShowDialog();
		}

		private void ConfigurationWindow_Closing(object sender, CancelEventArgs e)
		{
			if (_configureWindow.DialogResult != true) 
				return;

			e.Cancel = true;

			var configurationControl = (ConfigurationWindow)_configureWindow.Content;
			var cloneInfo = configurationControl.CloneConfigInfo;

			if (string.IsNullOrEmpty(cloneInfo.CubeName))
			{
				MessageWindow.Show("Cube can't be empty.", Ranet.Olap.UI.Localization.Error, MessageWindowButtons.OK);

				return;
			}

			Action closeAction = OnEditorDialogOk;
			_configureWindow.Dispatcher.BeginInvoke(closeAction);
		}

		private void OnEditorDialogOk()
		{
			var configurationControl = (ConfigurationWindow)_configureWindow.Content;
			var cloneInfo = configurationControl.CloneConfigInfo;

			if (!_configInfo.Equals(cloneInfo))
			{
				_configInfo = cloneInfo;
				_icoConfiguration.Save(cloneInfo);
			}

			CloseConfigurationWindow();
		}

		private void CloseConfigurationWindow()
		{
			_configureWindow.Closing -= ConfigurationWindow_Closing;
			_configureWindow.Close();

			if (_configureWindow != null)
				_configureWindow.Content = null;

			_configureWindow = null;
		}

		private FloatingDialog _configureWindow;

		private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			var selectedItem = (TreeViewItem)TreeView.SelectedItem;

			if (selectedItem != null) 
				LoadContent(selectedItem);
		}

		private void TreeView_Loaded(object sender, RoutedEventArgs e)
		{
			if (TreeView.Items.Count <= 0) 
				return;
			var fistTreeItem = GetFirstTreeViewItem(TreeView.Items[0] as TreeViewItem);

			if (fistTreeItem == null) 
				return;

			fistTreeItem.IsSelected = true;

			var parent = (TreeViewItem)fistTreeItem.Parent;

			while (parent != null)
			{
				parent.IsExpanded = true;
				parent = parent.Parent as TreeViewItem;
			}

			LoadContent(fistTreeItem);
		}

		public TreeViewItem GetFirstTreeViewItem(TreeViewItem oParentNode)
		{
			foreach (TreeViewItem oSubNode in oParentNode.Items)
				return GetFirstTreeViewItem(oSubNode);

			return oParentNode;
		}

		private void LoadContent(TreeViewItem item)
		{
			if (item.Tag is DemoControl demoControl)
				CurrentDemo = demoControl;
		}
	}
}