using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace PowerTab
{
	/// <summary>
	/// Patches a getter that runs when anything is selected (because it then checks for what tabs to apply) 
	/// </summary>
	[HarmonyPatch(typeof(MainTabWindow_Inspect))]
	[HarmonyPatch(MethodType.Getter)]
	[HarmonyPatch("CurTabs")]
	[UsedImplicitly] // by Harmony
	public class CurTabsPatch
	{
		private static readonly PowerTab PowerTab = new PowerTab();
		private static void Postfix(InspectTabBase __instance, ref IEnumerable<InspectTabBase> __result)
		{
			Thing selectedThing = Find.Selector.SingleSelectedThing; // Is null if more than one item was selected.
			
			//Note that sometimes, this patch runs a few times even after deselecting an item. selectedThing will be null, which will end the patch with no harm done.

			CompPower compPower = (selectedThing as ThingWithComps)?.TryGetComp<CompPower>();
			if (compPower is null)
				return;

			// Code past this point runs only if the aforementioned compPower element actually contains a CompPower
			
			PowerNetElements powerNetElements = new PowerNetElements();

			// Add batteries
			foreach (CompPowerBattery x in compPower.PowerNet.batteryComps)
				powerNetElements.AddBattery(x);
			
			// Add power consumers and producers
			foreach (CompPowerTrader x in compPower.PowerNet.powerComps)
				powerNetElements.AddPowerComponent(x);
			

			PowerTab.UpdatePowerNetInfo(powerNetElements);

			// Inject the power tab into the display. It is done here instead of in XML
			// because we need to be able to dynamically resolve items on the power grid at runtime.
			// Some mods have their own defs for Things that provide or consume power, so we can't patch them in XML
			// unless we want to hunt down every mod that does this.
			__result = __result.AddItem(PowerTab);
		}
	}
}