using System;

namespace INA_Generations;

/// <summary>
/// Marks a property or field to be displayed in the grid view
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class DisplayInGridViewAttribute : Attribute
{
	public string Header;
	public int Priority;

	public DisplayInGridViewAttribute(string header, int priority = 0)
	{
		Header = header;
		Priority = priority;
	}
}