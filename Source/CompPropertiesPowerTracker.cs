using System.Collections.Generic;
using Verse;

// ReSharper disable once CheckNamespace
namespace Compilatron
{
	// ReSharper disable once UnusedType.Global
	// This class is used in Harmony
	public class CompPropertiesPowerTracker : CompProperties
	{
		public CompPropertiesPowerTracker()
		{
			compClass = typeof(CompPowerTracker);
		}

		public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
		{
			yield break;
		}
	}
}