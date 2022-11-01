using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace PowerTab
{
	/// <summary>
	/// Component to monitor its parent's power statistics. Remember to assign the parent field!
	/// </summary>
	public class CompPowerTracker : ThingComp
	{
		public override void CompTick()
		{
			// For things like solar panels and other dynamically-adjusted power generators, retrieving from Props.basePowerConsumption
			// may not return the proper value (ex: solar panels return -1). This helps alleviate that over time.
			if (CurrentPowerOutput > _maxObservedPowerOutput && PowerType == PowerType.Producer) // It should only apply to producers like solar panels
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
				switch (PowerType)
				{
					case PowerType.Consumer or PowerType.Producer:
					{
						// PowerOutput returns the power use or draw regardless of whether it is on or off.
						// Obviously, things that are flicked off draw no power, so we must account for that.
						CompFlickable compFlickable = parent.TryGetComp<CompFlickable>();
						if (compFlickable is not null && !compFlickable.SwitchIsOn)
							return 0;
						return parent.GetComp<CompPowerTrader>().PowerOutput;
					}
					case PowerType.Battery:
						return parent.GetComp<CompPowerBattery>().StoredEnergy;
					default:
						return 0;
				}
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
		public float DesiredPowerOutput =>
			parent.TryGetComp<CompPower>() is CompPowerBattery ? parent.GetComp<CompPowerBattery>().Props.storedEnergyMax : 
			Mathf.Max(-parent.GetComp<CompPowerTrader>().Props.PowerConsumption, _maxObservedPowerOutput);

		public PowerType PowerType =>
			parent.TryGetComp<CompPower>() is CompPowerBattery ? PowerType.Battery :
			DesiredPowerOutput >= 0 ? PowerType.Producer :
			PowerType.Consumer;
	}
}