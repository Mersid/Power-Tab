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
		private static PowerTab2 powerTab = new PowerTab2();
		private static void Postfix(InspectTabBase __instance, ref IEnumerable<InspectTabBase> __result)
		{
			Thing selectedThing = Find.Selector.SingleSelectedThing;

			CompPower compPower = (selectedThing as ThingWithComps)?.TryGetComp<CompPower>();
			if (compPower is null)
				return;

			PowerNetElements powerNetElements = new PowerNetElements();

			foreach (CompPowerBattery x in compPower.PowerNet.batteryComps)
				powerNetElements.AddBattery(x);
			

			foreach (CompPowerTrader x in compPower.PowerNet.powerComps)
				powerNetElements.AddPowerComponent(x);
			

			powerTab.UpdatePowerNetInfo(powerNetElements);
			Log.Message(powerNetElements.ToString());

			__result = __result.AddItem(powerTab);
		}
	}
}