using System;

namespace INA_Generations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class DisplayInGridViewAttribute : Attribute
{
	public string Header;

	public DisplayInGridViewAttribute(string header)
	{
		Header = header;
	}
}