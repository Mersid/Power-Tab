using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace Compilatron
{
	[StaticConstructorOnStartup]
	public class Mod
	{
		static Mod() // Mod entrypoint
		{
			Harmony harmony = new Harmony("net.mersid.powertab");
			harmony.PatchAll();
		}
	}

	//[HarmonyPatch(typeof(Thing))]
	//[HarmonyPatch("GetInspectTabs")]
	
	[HarmonyPatch(typeof(MainTabWindow_Inspect))]
	[HarmonyPatch(MethodType.Getter)]
	[HarmonyPatch("CurTabs")]
	public class Patch
	{
		private static PowerTab powerTab = new PowerTab();
		private static void Postfix(InspectTabBase __instance, ref IEnumerable<InspectTabBase> __result)
		{
			Thing selectedThing = Find.Selector.SingleSelectedThing;

			CompPower compPower = (selectedThing as ThingWithComps)?.TryGetComp<CompPower>();
			if (compPower is null)
				return;

			StringBuilder stringBuilder = new StringBuilder();
			Log.Warning($"Connectors ({compPower.PowerNet.connectors.Count})");
			foreach (CompPower x in compPower.PowerNet.connectors)
			{
				stringBuilder.Append(x + "\n");
			}
			Log.Message(stringBuilder.ToString());
			stringBuilder.Clear();

			Log.Warning($"Transmitters ({compPower.PowerNet.transmitters.Count})");
			foreach (CompPower x in compPower.PowerNet.transmitters)
			{
				stringBuilder.Append(x + "\n");
			}
			Log.Message(stringBuilder.ToString());
			stringBuilder.Clear();

			Log.Warning($"Battery Comps ({compPower.PowerNet.batteryComps.Count})");
			foreach (CompPowerBattery x in compPower.PowerNet.batteryComps)
			{
				stringBuilder.Append(x.parent.def.LabelCap + $" ({x.StoredEnergy}Wd)\n");
			}
			Log.Message(stringBuilder.ToString());
			stringBuilder.Clear();
			
			Log.Warning($"Power Comps ({compPower.PowerNet.powerComps.Count})");
			foreach (CompPowerTrader x in compPower.PowerNet.powerComps)
			{
				CompPowerPlant compPowerPlant = x.parent.TryGetComp<CompPowerPlant>();
				if (compPowerPlant is null)
					stringBuilder.Append(x.parent.def.LabelCap + $" ({x.GetType().BaseType}/{x.GetType().BaseType.BaseType}, {x.PowerOutput}W)\n");
				else
					stringBuilder.Append(x.parent.def.LabelCap + $" ({x.GetType().BaseType}/{x.GetType().BaseType.BaseType}, {x.PowerOutput}W/{-compPowerPlant.Props.basePowerConsumption}W)\n");
			}
			Log.Message(stringBuilder.ToString());
			stringBuilder.Clear();

			__result = __result.AddItem(powerTab);
		}
	}
}