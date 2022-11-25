using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Resources;

namespace Ranet.Demo.Core
{
	public class SourceFile
	{
		private readonly string _fileName;
		private readonly List<string> _fileNameList;
		private StreamReader _reader;
		private StreamResourceInfo _streamResource;

		public SourceFile(string fileName)
		{
			_fileName = fileName;
			_fileNameList = new List<string>();
		}

		public List<string> GetFileNames()
		{
			_fileNameList.Add(_fileName + ".xaml");
			_fileNameList.Add(_fileName + ".xaml.cs");
			return _fileNameList;
		}

		public string GetSource(string selectedFileName, string assemblyLocation)
		{
			if (!string.IsNullOrEmpty(assemblyLocation))
			{
				var uri = string.Concat(assemblyLocation, ";component/ColorSource/",selectedFileName, ".colorized");

				try
				{
					_streamResource = Application.GetResourceStream(new Uri(uri, UriKind.RelativeOrAbsolute));

					if (_streamResource != null)
					{
						_reader = new StreamReader(_streamResource.Stream);

						return _reader.ReadToEnd();
					}
				}
				catch
				{
					return string.Empty;
				}
			}
			return string.Empty;
		}
	}
}