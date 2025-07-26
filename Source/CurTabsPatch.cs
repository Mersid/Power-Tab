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
		private static void Postfix(InspectTabBase __instance, ref IEnumerable<InspectTabBase> __result)
		{
			Thing selectedThing = Find.Selector.SingleSelectedThing; // Is null if more than one item was selected.

			//Note that sometimes, this patch runs a few times even after deselecting an item. selectedThing will be null, which will end the patch with no harm done.

			CompPower compPower = (selectedThing as ThingWithComps)?.TryGetComp<CompPower>();
			if (compPower is null)
				return;

			// this can be called many times per frame but will only do anything if it gets out of date
			Mod.PowerTab.Tracking = compPower;
			Mod.PowerTab.UpdateTrackers();

			// Inject the power tab into the display. It is done here instead of in XML
			// because we need to be able to dynamically resolve items on the power grid at runtime.
			// Some mods have their own defs for Things that provide or consume power, so we can't patch them in XML
			// unless we want to hunt down every mod that does this.
			__result = __result.AddItem(Mod.PowerTab);
		}
	}
}
