﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Eto.Forms;

namespace INA_Generations;

public static class GridViewExtensions
{
	/// <summary>
	/// Adds columns to the grid view based on the properties and fields of the given type.
	/// </summary>
	public static void AddColumns<T>(this GridView gridView) where T : class
	{
		gridView.ClearData<T>();
		
		List<(MemberInfo member, DisplayInGridViewAttribute displayAttribute)> membersToDisplay = new();

		foreach (MemberInfo member in typeof(T).GetMembers()
			         .Where(x => x.MemberType is MemberTypes.Field or MemberTypes.Property))
		{
			if (member.GetCustomAttribute<DisplayInGridViewAttribute>() is not DisplayInGridViewAttribute displayInGridView) continue;
			membersToDisplay.Add((member, displayInGridView));
		}

		foreach ((MemberInfo member, DisplayInGridViewAttribute displayInGridView) in membersToDisplay.OrderByDescending(x => x.displayAttribute.Priority))
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

	/// <summary>
	/// Sets the data store of the grid view to the given data.
	/// </summary>
	/// <param name="data">Data to display in the grid view</param>
	public static void SetData<T>(this GridView gridView, IEnumerable<T> data) where T : class
	{
		gridView.DataStore = new ObservableCollection<T>(data);
	}
	
	/// <summary>
	/// Clears the data store of the grid view and sets it to an empty collection.
	/// </summary>
	public static void ClearData<T>(this GridView gridView) where T : class
	{
		gridView.DataStore = new ObservableCollection<T>();
	}
}