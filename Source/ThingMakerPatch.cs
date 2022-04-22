using System.IO;
using HarmonyLib;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace PowerTab
{
	// I'm not really quite sure what this is doing here...
	//[HarmonyPatch(typeof(ThingMaker),"MakeThing")]
	//[UsedImplicitly] // by Harmony
	public class ThingMakerPatch
	{
		private static void Postfix(ref Thing __result)
		{
			if (__result is not ThingWithComps thingWithComps)
				return;

			Log.Warning($"{thingWithComps}");
			foreach (ThingComp comp in thingWithComps.AllComps)
			{
				Log.Warning($"	{comp}");
			}
			

		}
	}
}