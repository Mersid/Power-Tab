using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace PowerTab
{
	/// <summary>
	/// Component to monitor its parent's power statistics. Remember to assign the parent field!
	/// </summary>
	public class TestComp : ThingComp
	{
		
		public override string CompInspectStringExtra()
		{
			return $"This is a {PowerType}\nPower: {CurrentPowerOutput}/{DesiredPowerOutput}";
		}

		public override string TransformLabel(string label)
		{
			return $"{label} (patched)";
		}

		public override void CompTick()
		{
			// For things like solar panels and other dynamically-adjusted power generators, retrieving from Props.basePowerConsumption
			// may not return the proper value (ex: solar panels return -1). This helps alleviate that over time.
			if (CurrentPowerOutput > _maxObservedPowerOutput)
				_maxObservedPowerOutput = CurrentPowerOutput;
		}

		/// <summary>
		/// Gets the current power output. Negative values means that the tracked item is consuming power.
		/// If the tracked item is a battery, this returns the Wd of power in the battery.
		/// It's not technically correct, but it reduces the amount of code needed further down
		/// to discern and call a separate function for that.
		/// If the tracked item has no power type (a glitch), this returns 0
		/// </summary>
		public float CurrentPowerOutput
		{
			get
			{
				return PowerType switch
				{
					PowerType.Consumer or PowerType.Producer => parent.GetComp<CompPowerTrader>().PowerOutput,
					PowerType.Battery => parent.GetComp<CompPowerBattery>().StoredEnergy,
					_ => 0
				};
			}
		}

		// Max observed power output. This is to mitigate an issue wherein some items like solar panels are stated to produce 1 W.
		// We fix this in the CompTick method above. Use MinValue or this breaks with things that consume energy (because default for float is 0)
		private float _maxObservedPowerOutput = float.MinValue;
		
		/// <summary>
		/// Gets the desired power output. Negative values means that the tracked item is consuming power.
		/// If the tracked item is a battery, this returns the maximum Wd of power the battery can store.
		/// If the tracked item has no power type (a glitch), this returns 0.
		/// </summary>
		public float DesiredPowerOutput
		{
			get
			{
				return PowerType switch
				{
					PowerType.Consumer or PowerType.Producer => Mathf.Max(-parent.GetComp<CompPowerTrader>().Props.basePowerConsumption, _maxObservedPowerOutput),
					PowerType.Battery => parent.GetComp<CompPowerBattery>().Props.storedEnergyMax,
					_ => 0
				};
			}
		}

		public PowerType PowerType
		{
			get
			{
				return parent.TryGetComp<CompPower>() switch
				{
					CompPowerBattery => PowerType.Battery,
					CompPowerPlant => PowerType.Producer,
					CompPowerTrader => PowerType.Consumer,
					_ => throw new ArgumentOutOfRangeException($"Could not deduce power type for {parent.TryGetComp<CompPower>().parent.LabelCap}")
				};
			}
		}
	}
}