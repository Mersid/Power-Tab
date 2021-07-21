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
		static Mod() // Mod entrypoint
		{
			PowerTracker = new PowerTracker();
			Harmony harmony = new Harmony("net.mersid.powertab");
			harmony.PatchAll();
		}
	}
}