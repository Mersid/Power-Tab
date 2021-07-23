using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace PowerTab.UIElements
{
	/// <summary>
	/// Generates UI elements from trackers
	/// </summary>
	public class UIFactory
	{
		/// <summary>
		/// Creates a <see cref="PowerTabCategory"/> UI element from a <see cref="PowerTrackerCategory"/>
		/// </summary>
		/// <param name="powerTrackerCategory"></param>
		/// <returns></returns>
		public static PowerTabCategory MakeCategory(PowerTrackerCategory powerTrackerCategory)
		{
			IEnumerable<PowerTabGroup> powerTabGroups = powerTrackerCategory.Children.Select(MakeGroup).Where(t => t != null);

			PowerTabCategory powerTabCategory = new PowerTabCategory(
			powerTrackerCategory.Label,
			powerTrackerCategory.CurrentPowerOutput,
			powerTrackerCategory.CurrentPowerOutput / powerTrackerCategory.DesiredPowerOutput,
			powerTabGroups,
			Mod.PowerTab.InnerSize.x,
			powerTrackerCategory.PowerType == PowerType.Battery);

			return powerTabCategory;
		}

		/// <summary>
		/// Creates a <see cref="PowerTabGroup"/> UI element from a <see cref="PowerTrackerGroup"/>.
		/// If powerTrackerGroup contains no child elements, this returns null
		/// </summary>
		/// <param name="powerTrackerGroup"></param>
		/// <returns></returns>
		[CanBeNull]
		public static PowerTabGroup MakeGroup(PowerTrackerGroup powerTrackerGroup)
		{
			if (powerTrackerGroup.Children.Count == 0)
				return null;

			IEnumerable<PowerTabThing> powerTabThings = powerTrackerGroup.Children.Select(MakeThing);

			PowerTabGroup powerTabGroup = new PowerTabGroup(
				powerTrackerGroup.Label,
				powerTrackerGroup.Children.Count,
				powerTrackerGroup.CurrentPowerOutput,
				powerTrackerGroup.CurrentPowerOutput / powerTrackerGroup.DesiredPowerOutput,
				powerTabThings,
				Mod.PowerTab.InnerSize.x,
				powerTrackerGroup.Expanded,
				powerTrackerGroup.PowerType == PowerType.Battery,
				_ => powerTrackerGroup.Expanded = !powerTrackerGroup.Expanded);

			return powerTabGroup;
		}

		/// <summary>
		/// Creates a <see cref="PowerTabThing"/> UI element from a <see cref="PowerTrackerThing"/>
		/// </summary>
		/// <param name="powerTrackerThing"></param>
		/// <returns></returns>
		public static PowerTabThing MakeThing(PowerTrackerThing powerTrackerThing)
		{
			PowerTabThing powerTabThing = new PowerTabThing(
				powerTrackerThing.CompPower.parent, 
				powerTrackerThing.CurrentPowerOutput,
				powerTrackerThing.CurrentPowerOutput / powerTrackerThing.DesiredPowerOutput,
				Mod.PowerTab.InnerSize.x, 
				powerTrackerThing.PowerType == PowerType.Battery);

			return powerTabThing;
		}
	}
}
