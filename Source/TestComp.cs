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
		/// If the tracked item is a battery or otherwise has no power type (a glitch), this always returns 0
		/// </summary>
		public float CurrentPowerOutput
		{
			get
			{
				if (PowerType is PowerType.Consumer or PowerType.Producer)
					return parent.GetComp<CompPowerTrader>().PowerOutput;
				return 0;
			}
		}

		// Max observed power output. This is to mitigate an issue wherein some items like solar panels are stated to produce 1 W.
		// We fix this in the CompTick method above. Use MinValue or this breaks with things that consume energy (because default for float is 0)
		private float _maxObservedPowerOutput = float.MinValue;
		
		/// <summary>
		/// Gets the desired power output. Negative values means that the tracked item is consuming power.
		/// If the tracked item is a battery or otherwise has no power type (a glitch), this always returns 0.
		/// </summary>
		public float DesiredPowerOutput
		{
			get
			{
				if (PowerType is PowerType.Consumer or PowerType.Producer)
					return Mathf.Max(-parent.GetComp<CompPowerTrader>().Props.basePowerConsumption, _maxObservedPowerOutput);
				return 0;
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