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

			// Code past this point runs only if the aforementioned compPower element actually contains a CompPower
			
			PowerNetElements powerNetElements = new PowerNetElements();
			List<TestComp> testComps = new List<TestComp>();
			
			// If a single workbench or something that does not have a powernet (i.e. it only connects to one)
			// attempts to call any method under compPower.PowerNet will throw a NullReferenceException.
			// Incidentally, in such a situation, the selected object is the only item it its own "net" (RimWorld does not define it thusly).
			if (compPower.PowerNet == null)
			{
				// We could optionally add the lone power item here, but since it's alone with no means of power generation,
				// we can skip this without much ill effect.
			}
			else
			{
				// Add batteries
				foreach (CompPowerBattery x in compPower.PowerNet.batteryComps)
				{
					powerNetElements.AddBattery(x);
					Mod.PowerTracker.AddTracker(x);
				
					if (x.parent.TryGetComp<TestComp>() == null)
					{
						TestComp testComp = new TestComp {parent = x.parent};
						x.parent.AllComps.Add(testComp);
					}
					testComps.Add(x.parent.TryGetComp<TestComp>());
				}
				
			
				// Add power consumers and producers
				foreach (CompPowerTrader x in compPower.PowerNet.powerComps)
				{
					powerNetElements.AddPowerComponent(x);
					Mod.PowerTracker.AddTracker(x);
				
					if (x.parent.TryGetComp<TestComp>() == null)
					{
						TestComp testComp = new TestComp {parent = x.parent};
						x.parent.AllComps.Add(testComp);
					}
					testComps.Add(x.parent.TryGetComp<TestComp>());
				}
				
			}
			

			Mod.PowerTab.UpdatePowerNetInfo(powerNetElements);
			Mod.PowerTab.TestComps = testComps;
			
			// Inject the power tab into the display. It is done here instead of in XML
			// because we need to be able to dynamically resolve items on the power grid at runtime.
			// Some mods have their own defs for Things that provide or consume power, so we can't patch them in XML
			// unless we want to hunt down every mod that does this.
			__result = __result.AddItem(Mod.PowerTab);
		}
	}
}