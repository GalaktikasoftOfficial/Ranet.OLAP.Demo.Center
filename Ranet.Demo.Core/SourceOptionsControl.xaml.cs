using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Xml.Linq;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Zaaml.PresentationCore.Utils;
using Microsoft.Win32;

namespace Ranet.Demo.Core
{
	public partial class SourceOptionsControl
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty SourceCodeListProperty = DependencyProperty.Register(
			"SourceCodeList", typeof(List<SourceCodeEntry>), typeof(SourceOptionsControl), new PropertyMetadata(default(List<SourceCodeEntry>)));

		public static readonly DependencyProperty SelectedSourceCodeEntryProperty = DependencyProperty.Register(
			"SelectedSourceCodeEntry", typeof(SourceCodeEntry), typeof(SourceOptionsControl), new PropertyMetadata(default(SourceCodeEntry)));

		public static readonly DependencyProperty AssemblyNameProperty = DependencyProperty.Register(
			"AssemblyName", typeof(string), typeof(SourceOptionsControl), new PropertyMetadata(default(string)));

		#endregion

		#region Ctors

		public SourceOptionsControl()
		{
			InitializeComponent();
		}

		#endregion

		#region Properties

		public string AssemblyName
		{
			get { return (string) GetValue(AssemblyNameProperty); }
			set { SetValue(AssemblyNameProperty, value); }
		}

		public SourceCodeEntry SelectedSourceCodeEntry
		{
			get { return (SourceCodeEntry) GetValue(SelectedSourceCodeEntryProperty); }
			set { SetValue(SelectedSourceCodeEntryProperty, value); }
		}

		public List<SourceCodeEntry> SourceCodeList
		{
			get { return (List<SourceCodeEntry>) GetValue(SourceCodeListProperty); }
			set { SetValue(SourceCodeListProperty, value); }
		}

		#endregion

		#region  Methods

		private void DownloadZipFile_OnClick(object sender, RoutedEventArgs e)
		{
			var saveDialog = new SaveFileDialog
			{
				DefaultExt = ".zip",
				FileName = $"{AssemblyName}.zip",
				Filter = "Zip archives (*.zip)|*.zip"
			};

			if (saveDialog.ShowDialog() != true) 
				return;

			using (var fs = saveDialog.OpenFile())
			{
				var projectZip = new ModuleProjectZip(fs, saveDialog.SafeFileName, AssemblyName);

				projectZip.DownloadProject();
			}
		}

		#endregion
	}

	public class ProjectInfo
	{
		#region Fields

		public string ModuleName;
		public string WebName;

		#endregion
	}

	public class ModuleProjectZip
	{
		#region Static Fields and Constants

		private static string ProjectSectionTemplate =
			@"Project(""{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"") = ""${ProjectName}"", ""${ProjectDirectory}\${ProjectName}.csproj"", ""${ProjectGuid}""
EndProject";

		private static string SolutionTemplate =
			@"Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio 2013
VisualStudioVersion = 12.0.21005.1
MinimumVisualStudioVersion = 10.0.40219.1

$(ProjectSection1)

$(ProjectSection2)

Global
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
EndGlobal";

		#endregion

		#region Fields

		private string _assemblyName;
		private string _outputFileName;
		private Stream _outputStream;

		#endregion

		#region Ctors

		public ModuleProjectZip(Stream outputStream, string outputFileName, string assemblyName)
		{
			_outputStream = outputStream;
			_outputFileName = outputFileName;
			_assemblyName = assemblyName;
		}

		#endregion

		#region  Methods

		private MemoryStream ChangeProperties(XElement root, ProjectInfo projectInfo, ZipFileMode zipFileType)
		{
			switch (zipFileType)
			{
				case ZipFileMode.WebProject:
					var ns = XNamespace.Get("http://schemas.microsoft.com/developer/msbuild/2003");
					var iisUrl = root.Descendants(ns.GetName("IISUrl")).SingleOrDefault();

					iisUrl?.SetValue("");

					break;
			}

			var memory = new MemoryStream();

			root.Save(memory);

			return memory;
		}

		private void CopyZip(ZipOutputStream zipOutput, string name, string rootFolder, ProjectInfo projectInfo, ZipFileMode zipFileType)
		{
			var fileStream = GetStreamZipFile(name, zipFileType);
			var zipInputStream = new ZipInputStream(fileStream);
			var zipEntry = zipInputStream.GetNextEntry();
			var buffer = new byte[4096];

			while (zipEntry != null)
			{
				var memory = new MemoryStream();

				StreamUtils.Copy(zipInputStream, memory, buffer);

				memory.Seek(0, SeekOrigin.Begin);

				if (IsCsprojFile(zipEntry.Name))
				{
					var root = XElement.Load(new StreamReader(memory));
					var projectName = GetProjectName(zipEntry.Name);

					if (zipFileType == ZipFileMode.WebProject)
						projectInfo.WebName = projectName;
					else if (zipFileType == ZipFileMode.ModuleProject) 
						projectInfo.ModuleName = projectName;

					var temp = ChangeProperties(root, projectInfo, zipFileType);

					temp.Seek(0, SeekOrigin.Begin);
					memory = new MemoryStream();
					StreamUtils.Copy(temp, memory, buffer);
				}

				memory.Seek(0, SeekOrigin.Begin);

				var newEntry = new ZipEntry(rootFolder + name + "\\" + zipEntry.Name)
				{
					Size = memory.Length
				};

				zipOutput.PutNextEntry(newEntry);
				StreamUtils.Copy(memory, zipOutput, buffer);

				zipEntry = zipInputStream.GetNextEntry();
			}


			zipInputStream.IsStreamOwner = true; // Makes the Close also Close the underlying stream
			zipInputStream.Close();
		}

		public void DownloadProject()
		{
			var rootFolder = _outputFileName.Replace(".zip", "\\");
			var output = new ZipOutputStream(_outputStream);

			var projectInfo = new ProjectInfo();
			var moduleName = _assemblyName;

			CopyZip(output, moduleName, rootFolder, projectInfo, ZipFileMode.ModuleProject); // copy module zip file			

			var newEntry = new ZipEntry(rootFolder + "Solution.sln");
			var slnStream = GetSolutionFile(projectInfo);

			newEntry.Size = slnStream.Length;
			output.PutNextEntry(newEntry);
			StreamUtils.Copy(slnStream, output, new byte[4096]);
			output.IsStreamOwner = true;
			output.Close();
		}


		private static Assembly GetAssemblyByName(string name)
		{
			return AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(assembly => assembly.FullName.Contains(name));
		}

		private static string GetProjectName(string fileName)
		{
			return Path.GetFileNameWithoutExtension(fileName);
		}

		private string GetProjectSectionNet(string projectName)
		{
			var result = ProjectSectionTemplate;

			result = result.Replace("${ProjectName}", projectName);
			result = result.Replace("${ProjectDirectory}", projectName);

			return result;
		}

		private Stream GetSolutionFile(ProjectInfo projectInfo)
		{
			var result = SolutionTemplate;
			var moduleSection = GetProjectSectionNet(projectInfo.ModuleName);

			result = result.Replace("$(ProjectSection1)", moduleSection);
			result = result.Replace("$(ProjectSection2)", "");

			return new MemoryStream(Encoding.UTF8.GetBytes(result));
		}

		private Stream GetStreamZipFile(string zipFileName, ZipFileMode zipFileType)
		{
			zipFileName = zipFileName.ToLower();
			var assembly = GetAssemblyByName("Ranet.DemoCenter");
			var files = assembly.EnumerateEmbeddedResources().ToList();

			foreach (var file in files)
			{
				if (file.Contains(".zip") && file.Contains(zipFileName))
					return assembly.GetResourceStream(file);
			}
			return Stream.Null;
		}

		private static bool IsCsprojFile(string fileName)
		{
			return string.Equals(Path.GetExtension(fileName), ".csproj", StringComparison.OrdinalIgnoreCase);
		}

		#endregion
	}

	public enum ZipFileMode
	{
		ModuleProject,
		WebProject,
		Properties
	}
}