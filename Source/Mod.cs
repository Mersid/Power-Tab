using HarmonyLib;
using JetBrains.Annotations;
using Verse;

namespace PowerTab
{
	[StaticConstructorOnStartup]
	[UsedImplicitly] // by Rimworld. All classes with StaticConstructorOnStartup will be run on game load.
	public class Mod
	{
		public static readonly PowerTab PowerTab;
		static Mod() // Mod entrypoint
		{
			PowerTab = new PowerTab();
			Harmony harmony = new Harmony("net.mersid.powertab");
			harmony.PatchAll();
		}
	}
}