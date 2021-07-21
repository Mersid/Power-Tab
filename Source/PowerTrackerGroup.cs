using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace PowerTab
{
	/// <summary>
	/// Tracks power for an entire group of some item in the power net
	/// </summary>
	public class PowerTrackerGroup
	{
		private readonly List<PowerTrackerThing> _children;
		
		/// <summary>
		/// The <see cref="PowerType"/> of the items tracked in this group. Please recall that all items in the
		/// group should be instances of the same <see cref="ThingDef"/>.
		/// </summary>
		public PowerType PowerType { get; }

		public PowerTrackerGroup(PowerType powerType)
		{
			_children = new List<PowerTrackerThing>();
			PowerType = powerType;
		}
		
		/// <summary>
		/// Gets the current power output. Negative values means that the tracked group is consuming power.
		/// If the tracked group are batteries or otherwise has no power type (a glitch), this always returns 0
		/// </summary>
		public float CurrentPowerOutput => _children.Sum(t => t.CurrentPowerOutput);

		/// <summary>
		/// Gets the desired power output. Negative values means that the tracked group is consuming power.
		/// If the tracked group are batteries or otherwise has no power type (a glitch), this always returns 0.
		/// If the tracked group are solar panels, this always returns 1 * the number of panels.
		/// </summary>
		public float DesiredPowerOutput => _children.Sum(t => t.DesiredPowerOutput);

		/// <summary>
		/// Add a <see cref="PowerTrackerThing"/> to the group. Please recall that all items in the
		/// group should be instances of the same <see cref="ThingDef"/>. No validation will be performed in this method.
		/// If the added <see cref="PowerTrackerThing"/> already exists in the group, this method will exit without adding it again
		/// </summary>
		/// <param name="powerTrackerThing">The <see cref="PowerTrackerThing"/> to add</param>
		public void AddTrackerThing(PowerTrackerThing powerTrackerThing)
		{
			if (!_children.Contains(powerTrackerThing))
				_children.Add(powerTrackerThing);
		}

		public void RemoveTrackerThing(PowerTrackerThing powerTrackerThing)
		{
			_children.Remove(powerTrackerThing);
		}
	}
}