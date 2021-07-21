using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerTab
{
	/// <summary>
	/// Tracks power for an entire category (as defined in <see cref="PowerType"/>)
	/// </summary>
	public class PowerTrackerCategory
	{
		private readonly List<PowerTrackerGroup> _children;
		
		/// <summary>
		/// The <see cref="PowerType"/> of the items tracked in this category
		/// </summary>
		public PowerType PowerType { get; }
		
		public PowerTrackerCategory(PowerType powerType)
		{
			_children = new List<PowerTrackerGroup>();
			PowerType = powerType;
		}

		/// <summary>
		/// Gets the current power output. Negative values means that the tracked category is consuming power.
		/// If the tracked category is the battery category or otherwise has no power type (a glitch), this always returns 0
		/// </summary>
		public float CurrentPowerOutput => _children.Sum(t => t.CurrentPowerOutput);

		/// <summary>
		/// Gets the desired power output. Negative values means that the tracked category is consuming power.
		/// If the tracked category is the battery category or otherwise has no power type (a glitch), this always returns 0.
		/// Solar panels in the children list will contribute 1 * the number of panels to this output.
		/// </summary>
		public float DesiredPowerOutput => _children.Sum(t => t.DesiredPowerOutput);

		/// <summary>
		/// Add a <see cref="PowerTrackerGroup"/> to the category. No validation will be performed in this method.
		/// If the group already exists, it will not be added again
		/// </summary>
		/// <param name="powerTrackerGroup"></param>
		public void AddTrackerGroup(PowerTrackerGroup powerTrackerGroup)
		{
			if (!_children.Contains(powerTrackerGroup))
				_children.Add(powerTrackerGroup);
		}
	}
}