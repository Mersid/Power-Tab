using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Compilatron
{
	public class PowerNetElements
	{
		private List<CompPowerBattery> _batteries;
		private List<CompPowerPlant> _powerPlants;
		private List<CompPowerTrader> _consumers;
		
		public PowerNetElements()
		{
			_batteries = new List<CompPowerBattery>();
			_powerPlants = new List<CompPowerPlant>();
			_consumers = new List<CompPowerTrader>();
		}

		public void AddBattery(CompPowerBattery battery)
		{
			_batteries.Add(battery);
		}

		public void AddPowerComponent(CompPowerTrader powerTrader)
		{
			CompPowerPlant compPowerPlant = powerTrader.parent.TryGetComp<CompPowerPlant>();
			if (compPowerPlant is null) // Is consumer, not producer
				_consumers.Add(powerTrader);
			else // Is power plant
				_powerPlants.Add(compPowerPlant);
		}

		public List<CompPowerBattery> Batteries => _batteries;

		public List<CompPowerPlant> PowerPlants => _powerPlants;

		public List<CompPowerTrader> Consumers => _consumers;
	}
}