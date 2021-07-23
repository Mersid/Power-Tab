using HarmonyLib;
using JetBrains.Annotations;
using Verse;

namespace PowerTab
{
	[StaticConstructorOnStartup]
	[UsedImplicitly] // by Rimworld. All classes with StaticConstructorOnStartup will be run on game load.
	public class Mod
	{
		public static PowerTracker PowerTracker;
		public static readonly PowerTab PowerTab;
		static Mod() // Mod entrypoint
		{
			PowerTracker = new PowerTracker();
			PowerTab = new PowerTab();
			Harmony harmony = new Harmony("net.mersid.powertab");
			harmony.PatchAll();
		}
	}
}