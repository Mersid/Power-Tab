using System.Collections.Generic;
using System.Linq;
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

			float netStoredEnergy = _batteries.Sum(t => t.StoredEnergy);
			float netStoredEnergyMax = _batteries.Sum(t => t.Props.storedEnergyMax);
			stringBuilder.Append($"Batteries ({_batteries.Count}; {netStoredEnergy}/{netStoredEnergyMax} Wd):\n");
			foreach (CompPowerBattery battery in _batteries)
				stringBuilder.Append($"{battery.parent.def.LabelCap} at {battery.parent.Position} ({battery.StoredEnergy}/{battery.Props.storedEnergyMax} Wd)\n");

			float powerProducedInstantaneous = _powerPlants.Sum(t => t.PowerOutput);
			stringBuilder.Append($"\nPowerplants ({_powerPlants.Count}; {powerProducedInstantaneous} W):\n");
			foreach (CompPowerPlant powerPlant in _powerPlants)
				stringBuilder.Append($"{powerPlant.parent.def.LabelCap} at {powerPlant.parent.Position} ({powerPlant.PowerOutput}/{-powerPlant.Props.basePowerConsumption} W)\n");

			float powerConsumedInstantaneous = _consumers.Sum(t => t.PowerOutput);
			stringBuilder.Append($"\nConsumers ({_consumers.Count}; {powerConsumedInstantaneous} W):\n");
			foreach (CompPowerTrader consumer in _consumers)
				stringBuilder.Append($"{consumer.parent.def.LabelCap} at {consumer.parent.Position} ({consumer.PowerOutput} W)\n");

			stringBuilder.Append($"Net power: {powerProducedInstantaneous + powerConsumedInstantaneous} W"); // powerConsumedInstantaneous is a negative value, so add instead of subtract

			return stringBuilder.ToString();
		}
	}
}