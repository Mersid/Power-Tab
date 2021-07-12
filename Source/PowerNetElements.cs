using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Compilatron
{
	public class PowerNetElements
	{
		public List<CompPowerBattery> Batteries { get; }
		public List<CompPowerPlant> PowerPlants { get; }
		public List<CompPowerTrader> Consumers { get; }
		/// <summary>
		/// A list of all batteries, producers, and consumers on a power net
		/// </summary>
		public PowerNetElements()
		{
			Batteries = new List<CompPowerBattery>();
			PowerPlants = new List<CompPowerPlant>();
			Consumers = new List<CompPowerTrader>();
		}

		public void AddBattery(CompPowerBattery battery)
		{
			Batteries.Add(battery);
		}

		public void AddPowerComponent(CompPowerTrader powerTrader)
		{
			CompPowerPlant compPowerPlant = powerTrader.parent.TryGetComp<CompPowerPlant>();
			if (compPowerPlant is null) // Is consumer, not producer
				Consumers.Add(powerTrader);
			else // Is power plant
				PowerPlants.Add(compPowerPlant);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();

			float netStoredEnergy = Batteries.Sum(t => t.StoredEnergy);
			float netStoredEnergyMax = Batteries.Sum(t => t.Props.storedEnergyMax);
			stringBuilder.Append($"Batteries ({Batteries.Count}; {netStoredEnergy}/{netStoredEnergyMax} Wd):\n");
			foreach (CompPowerBattery battery in Batteries)
				stringBuilder.Append($"{battery.parent.def.LabelCap} at {battery.parent.Position} ({battery.StoredEnergy}/{battery.Props.storedEnergyMax} Wd)\n");

			float powerProducedInstantaneous = PowerPlants.Sum(t => t.PowerOutput);
			stringBuilder.Append($"\nPowerplants ({PowerPlants.Count}; {powerProducedInstantaneous} W):\n");
			foreach (CompPowerPlant powerPlant in PowerPlants)
				stringBuilder.Append($"{powerPlant.parent.def.LabelCap} at {powerPlant.parent.Position} ({powerPlant.PowerOutput}/{-powerPlant.Props.basePowerConsumption} W)\n");

			float powerConsumedInstantaneous = Consumers.Sum(t => t.PowerOutput);
			stringBuilder.Append($"\nConsumers ({Consumers.Count}; {powerConsumedInstantaneous} W):\n");
			foreach (CompPowerTrader consumer in Consumers)
				stringBuilder.Append($"{consumer.parent.def.LabelCap} at {consumer.parent.Position} ({consumer.PowerOutput} W)\n");

			stringBuilder.Append($"Net power: {powerProducedInstantaneous + powerConsumedInstantaneous} W"); // powerConsumedInstantaneous is a negative value, so add instead of subtract

			return stringBuilder.ToString();
		}
	}
}