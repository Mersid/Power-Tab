using System.Collections.Generic;
using System.Text;
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

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			
			stringBuilder.Append($"Batteries ({_batteries.Count}):\n");
			foreach (CompPowerBattery battery in _batteries)
				stringBuilder.Append($"{battery.parent.def.LabelCap} at {battery.parent.Position} ({battery.StoredEnergy}/{battery.Props.storedEnergyMax} Wd)\n");

			stringBuilder.Append($"\nPowerplants ({_powerPlants.Count}):\n");
			foreach (CompPowerPlant powerPlant in _powerPlants)
				stringBuilder.Append($"{powerPlant.parent.def.LabelCap} at {powerPlant.parent.Position} ({powerPlant.PowerOutput}/{-powerPlant.Props.basePowerConsumption} W)\n");

			stringBuilder.Append($"\nConsumers ({_consumers.Count}):\n");
			foreach (CompPowerTrader consumer in _consumers)
				stringBuilder.Append($"{consumer.parent.def.LabelCap} at {consumer.parent.Position} ({consumer.PowerOutput} W)\n");

			return stringBuilder.ToString();
		}
	}
}