using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Ranet.Demo.Core;
using Ranet.Olap.Core.Data;
using Zaaml.PresentationCore.Utils;

namespace MdxDynamic
{
	[Demo(1)]
	public partial class MdxDynamicByTuple
	{
		public MdxDynamicByTuple()
		{
			InitializeComponent();
			ControlName = DemoResources.InitByTuple_Name;
			About = DemoResources.InitByTuple_Caption;
			using (var reader = new StreamReader(GetType().Assembly.GetResourceStream("Resources/InitByTupleDescription.txt")))
				Description = reader.ReadToEnd();
		}

		private void ConfigureByDefault()
		{
			if (string.IsNullOrEmpty(CurrentConnection)) return;
			var initByDefaultTuples = new List<List<ShortMemberInfo>>();
			var tuples = new List<ShortMemberInfo>();
			//tuple 1
			var smi = new ShortMemberInfo
			{
				HierarchyUniqueName = "[Product].[Product Categories]",
				UniqueName = "[Product].[Product Categories].[Category].&[1]"
			};
			tuples.Add(smi);
			smi = new ShortMemberInfo
			{
				HierarchyUniqueName = "[Date].[Calendar]",
				UniqueName = "[Date].[Calendar].[Calendar Year].&[2007]"
			};
			tuples.Add(smi);
			smi = new ShortMemberInfo
			{
				HierarchyUniqueName = "[Measures]",
				UniqueName = "[Measures].[Internet Sales Amount]"
			};
			tuples.Add(smi);
			initByDefaultTuples.Add(tuples);

			//tuple 2
			smi = new ShortMemberInfo
			{
				HierarchyUniqueName = "[Product].[Product Categories]",
				UniqueName = "[Product].[Product Categories].[Category].&[4]"
			};
			tuples.Add(smi);
			smi = new ShortMemberInfo
			{
				HierarchyUniqueName = "[Date].[Calendar]",
				UniqueName = "[Date].[Calendar].[Calendar Year].&[2008]"
			};
			tuples.Add(smi);
			smi = new ShortMemberInfo
			{
				HierarchyUniqueName = "[Measures]",
				UniqueName = "[Measures].[Internet Sales Amount]"
			};
			tuples.Add(smi);
			initByDefaultTuples.Add(tuples);

			pivotMdxDynamicControl.Connection = CurrentConnection;
			pivotMdxDynamicControl.OlapDataLoader = GetDataLoader();
			pivotMdxDynamicControl.AutoExecuteQuery = true;
			pivotMdxDynamicControl.CubeName = DefaultCubeName;
			pivotMdxDynamicControl.UseCellConditionsDesigner = true;

			pivotMdxDynamicControl.ClearMetadataFilterList();

			pivotMdxDynamicControl.Initialize(initByDefaultTuples);
		}

		private void MdxDynamicByTuple_OnLoaded(object s, RoutedEventArgs e)
		{
			ConfigureByDefault();
			CurrentConnectionChanged -= OnCurrentConnectionChanged;
			DefaultCubeNameChanged -= OnDefaultCubeNameChanged;
			CurrentConnectionChanged += OnCurrentConnectionChanged;
			DefaultCubeNameChanged += OnDefaultCubeNameChanged;
		}

		private void OnDefaultCubeNameChanged(object sender, EventArgs eventArgs)
		{
			ConfigureByDefault();
		}

		private void OnCurrentConnectionChanged(object sender, EventArgs eventArgs)
		{
			ConfigureByDefault();
		}
	}
}