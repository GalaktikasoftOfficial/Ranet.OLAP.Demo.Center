using System;

namespace Ranet.Demo.Core
{
	public class DemoAttribute : Attribute
	{
		#region Fields

		#endregion

		#region Ctors

		public DemoAttribute(int index)
		{
			Index = index;
		}

		public DemoAttribute(string category, int index)
		{
			Category = category;
			Index = index;
		}

		public DemoAttribute()
		{
		}

		#endregion

		#region Properties

		public string Category { get; }

		public int Index { get; }

		#endregion
	}

	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class DemoCategoryAttribute : Attribute
	{
		public DemoCategoryAttribute(string categoryName, int index, string parentCategory = null)
		{
			CategoryName = categoryName;
			Index = index;
			ParentCategory = parentCategory;
		}

		public string CategoryName { get; }

		public int Index { get; }

		public string ParentCategory { get; }
	}

	[AttributeUsage(AttributeTargets.Assembly)]
	public sealed class DemoModuleAttribute : Attribute
	{
		#region Ctors

		public DemoModuleAttribute(string name, int index = 0)
		{
			Index = index;
			ModuleName = name;
		}

		#endregion

		public int Index { get; set; }

		#region Properties

		public string ModuleName { get; }

		#endregion

		#region Fields

		#endregion
	}
}