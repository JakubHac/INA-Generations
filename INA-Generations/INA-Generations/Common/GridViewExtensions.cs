using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Eto.Forms;

namespace INA_Generations;

public static class GridViewExtensions
{
	public static void AddColumns<T>(this GridView gridView) where T : class
	{
		gridView.ClearData<T>();
		
		foreach (MemberInfo member in typeof(T).GetMembers()
			         .Where(x => x.MemberType is MemberTypes.Field or MemberTypes.Property))
		{
			if (member.GetCustomAttribute<DisplayInGridViewAttribute>() is DisplayInGridViewAttribute displayInGridView)
			{
				gridView.Columns.Add(new()
				{
					HeaderText = displayInGridView.Header,
					DataCell = new TextBoxCell
					{
						Binding = Binding.Property<T, string>(x =>
							(member.MemberType == MemberTypes.Field
								? ((FieldInfo)member).GetValue(x)
								: ((PropertyInfo)member).GetValue(x)).ToString())
					}
				});
			}
		}
	}

	public static void SetData<T>(this GridView gridView, IEnumerable<T> data) where T : class
	{
		gridView.DataStore = new ObservableCollection<T>(data);
	}
	
	public static void ClearData<T>(this GridView gridView) where T : class
	{
		gridView.DataStore = new ObservableCollection<T>();
	}
}