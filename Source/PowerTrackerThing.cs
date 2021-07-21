using System;
using PowerTab.UIElements;
using RimWorld;

namespace PowerTab
{
	/// <summary>
	/// Tracks power data for a specific thing (some form of <see cref="CompPower"/>)
	/// </summary>
	public class PowerTrackerThing
	{
		private readonly CompPower _compPower; // The CompPower that this class tracks; the backing field, so to speak
		
		public PowerTrackerThing(CompPower compPower)
		{
			_compPower = compPower;
		}

		public PowerType PowerType
		{
			get
			{
				return _compPower switch
				{
					CompPowerBattery => PowerType.Battery,
					CompPowerPlant => PowerType.Producer,
					CompPowerTrader => PowerType.Consumer,
					_ => throw new ArgumentOutOfRangeException($"Could not deduce power type for {_compPower.parent.LabelCap}")
				};
			}
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
					return ((CompPowerTrader) _compPower).PowerOutput;
				return 0;
			}
		}

		/// <summary>
		/// Gets the desired power output. Negative values means that the tracked item is consuming power.
		/// If the tracked item is a battery or otherwise has no power type (a glitch), this always returns 0.
		/// If the tracked item is a solar panel, this always returns -1.
		/// </summary>
		public float DesiredPowerOutput
		{
			get
			{
				if (PowerType is PowerType.Consumer or PowerType.Producer)
					return -((CompPowerTrader) _compPower).Props.basePowerConsumption;
				return 0;
			}
		}

	}
}