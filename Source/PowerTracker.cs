using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace PowerTab
{
	public class PowerTracker
	{
		// Putting the tracker here instead of as a Comp because we're injecting everything at runtime for mod compatibility
	
		private readonly Dictionary<CompPower, PowerTrackerThing> _trackers;
		private readonly Dictionary<ThingDef, PowerTrackerGroup> _groups;
		private readonly Dictionary<PowerType, PowerTrackerCategory> _categories;

		public PowerTracker()
		{
			_trackers = new Dictionary<CompPower, PowerTrackerThing>();
			_groups = new Dictionary<ThingDef, PowerTrackerGroup>();
			_categories = new Dictionary<PowerType, PowerTrackerCategory>();
		}

		/// <summary>
		/// Add a tracker to the internal database for the given <see cref="CompPower"/>. This method will do nothing if there is already a tracker for this <see cref="CompPower"/>.
		/// </summary>
		/// <param name="compPower">The <see cref="CompPower"/> to add a tracker for</param>
		public void AddTracker(CompPower compPower)
		{
			if (_trackers.ContainsKey(compPower)) return;
			
			
			PowerTrackerThing thing = new PowerTrackerThing(compPower);
			_trackers[compPower] = thing;


			ThingDef def = compPower.parent.def;
			// Add group if it doesn't exist, then we'll add the thing to the group
			if (!_groups.ContainsKey(def))
				_groups[def] = new PowerTrackerGroup(thing.PowerType);
			_groups[def].AddTrackerThing(thing);
			
			
			// Add group to category if necessary. If it exists already,
			// it will already have a reference to the groups, so will be updated automatically
			if (!_categories.ContainsKey(thing.PowerType))
				_categories[thing.PowerType] = new PowerTrackerCategory(thing.PowerType);
			
			_categories[thing.PowerType].AddTrackerGroup(_groups[def]); // If the group already exists, the method will automatically refuse to re-add it.
		}

		/// <summary>
		/// Removes the given <see cref="CompPower"/> from the database. This method will do nothing if that item does not exist, or is already deleted.
		/// </summary>
		/// <param name="compPower">The <see cref="CompPower"/> to remove from the database</param>
		public void RemoveTracker(CompPower compPower)
		{
			if (!_trackers.ContainsKey(compPower) || !_groups.ContainsKey(compPower.parent.def)) return; // If either doesn't exist, the target doesn't exist, so we should exit
			
			_groups[compPower.parent.def].RemoveTrackerThing(_trackers[compPower]); // Remove from group tracker
			_trackers.Remove(compPower); // Remove from the database's tracker. These two references are the only ones to a PowerTrackerThing, so it's effectively wiped.
		}

		/// <summary>
		/// Removes all trackers EXCEPT for those that track items listed in compPowers
		/// </summary>
		/// <param name="compPowers">List of <see cref="CompPower"/> trackers to not delete</param>
		public void RemoveTrackersExcept(List<CompPower> compPowers)
		{
			List<CompPower> excepts = _trackers.Keys.Except(compPowers).ToList(); // Obtain all keys that are in _trackers but not compPowers. We will delete these.
			foreach (CompPower compPower in excepts)
				RemoveTracker(compPower);
		}

		/// <summary>
		/// Attempt to retrieve the <see cref="PowerTrackerThing"/> that tracks the <see cref="CompPower"/>. Returns null if there is no tracker for it.
		/// </summary>
		/// <param name="compPower">The <see cref="PowerTrackerThing"/> that tracks the given <see cref="CompPower"/></param>
		/// <returns></returns>
		public PowerTrackerThing GetTracker(CompPower compPower)
		{
			_trackers.TryGetValue(compPower, out PowerTrackerThing thing);
			return thing;
		}
	}
}