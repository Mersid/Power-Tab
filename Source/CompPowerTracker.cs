using System;
using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace PowerTab
{
	public class CompPowerTracker : ThingComp
	{
		public static readonly string[] PowerTypeString = { "None", "Producers", "Consumers", "Storage" };
		
		private Dictionary<int, float> _historicalPowerUsage = new Dictionary<int, float>();
		private Dictionary<int, bool> _historicalUptime = new Dictionary<int, bool>();
		private Dictionary<int, bool> _historicalUsetime = new Dictionary<int, bool>();
		
		private float _intMaxPowerUsage = 1;

		public float MaxPowerUsage
		{
			get
			{
				if (Math.Abs(PowerUsage) > _intMaxPowerUsage) _intMaxPowerUsage = Math.Abs(PowerUsage);
				return _intMaxPowerUsage;
			}
		}

		public float PowerUsage
		{
			get
			{
				CompPowerBattery powerBattery = parent.GetComp<CompPowerBattery>();
				if (powerBattery != null)
					return powerBattery.StoredEnergy;

				CompPowerPlant powerPlant = parent.GetComp<CompPowerPlant>();
				if (powerPlant != null)
					return powerPlant.PowerOn ? powerPlant.PowerOutput : 0;
                
				CompPowerTrader powerTrader = parent.GetComp<CompPowerTrader>();
				if (powerTrader != null)
					return powerTrader.PowerOn ? powerTrader.PowerOutput : 0;

				return 0;
			}
		}

		public PowerNet PowerNetwork
		{
			get
			{
				CompPowerBattery powerBattery = parent.GetComp<CompPowerBattery>(); 
				if (powerBattery != null)
					return powerBattery.PowerNet;

				CompPowerPlant powerPlant = parent.GetComp<CompPowerPlant>();
				if (powerPlant != null)
					return powerPlant.PowerNet;
				

				CompPowerTrader powerTrader = parent.GetComp<CompPowerTrader>();
				if (powerTrader != null)
					return powerTrader.PowerNet;

				CompPower power = parent.GetComp<CompPower>();
				if (power != null)
					return power.PowerNet;

				return null;
			}
		}

		public override void PostExposeData()
		{
			Scribe_Collections.Look(ref _historicalPowerUsage, "HistoricalPowerUsage");
			Scribe_Collections.Look(ref _historicalUptime, "HistoricalUptime");
			Scribe_Collections.Look(ref _historicalUsetime, "HistoricalUsetime");
			base.PostExposeData();
		}

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			if (_historicalPowerUsage == null) _historicalPowerUsage = new Dictionary<int, float>();
			if (_historicalUptime == null) _historicalUptime = new Dictionary<int, bool>();
			if (_historicalUsetime == null) _historicalUsetime = new Dictionary<int, bool>();
		}

		
		public static PowerType PowerTypeFor(ThingDef def)
		{
			CompProperties_Battery powerBattery = def.GetCompProperties<CompProperties_Battery>();
			if (powerBattery != null)
				return PowerType.Storage;

			CompProperties_Power power = def.GetCompProperties<CompProperties_Power>();
			if (power != null)
				return power.basePowerConsumption > 0 ? PowerType.Consumer : PowerType.Producer;

			return PowerType.None;
		}

		public override void Initialize(CompProperties properties)
		{
			CompPowerBattery powerBattery = parent.GetComp<CompPowerBattery>();
			if (powerBattery != null)
				_intMaxPowerUsage = Math.Max(Math.Abs(powerBattery.Props.storedEnergyMax), MaxPowerUsage);

			CompPowerPlant powerPlant = parent.GetComp<CompPowerPlant>();
			if (powerPlant != null)
				_intMaxPowerUsage = Math.Max(Math.Abs(powerPlant.Props.basePowerConsumption), MaxPowerUsage);

			CompPowerTrader powerTrader = parent.GetComp<CompPowerTrader>();
			if (powerTrader != null)
				_intMaxPowerUsage = Math.Max(Math.Abs(powerTrader.Props.basePowerConsumption), MaxPowerUsage);
		}

		
		/// <summary>
		/// Draws a group of items (batteries, specific generators, etc.) Drawn when pressing the + button on the power tab.
		/// </summary>
		/// <param name="yref"></param>
		/// <param name="width"></param>
		/// <param name="maxPowerUsage"></param>
		public void DrawGUI(ref float yref, float width, float maxPowerUsage)
		{
			GUI.BeginGroup(new Rect(GenUI.GapWide, yref, width - GenUI.Gap - GenUI.GapTiny * 2, GenUI.ListSpacing));
			Rect rect = new Rect(0, 0, width - GenUI.Gap * 2 - GenUI.ScrollBarWidth, GenUI.ListSpacing);
			Widgets.DrawHighlightIfMouseover(rect);
			Widgets.ThingIcon(rect.LeftPartPixels(rect.height), parent);
			if (Widgets.ButtonInvisible(rect)) CameraJumper.TryJumpAndSelect(new GlobalTargetInfo(parent));
			rect.xMin += rect.height;
			Widgets.Label(rect.LeftPartPixels(150 - GenUI.GapWide).ContractedBy(GenUI.GapTiny), parent.LabelShortCap);
			rect.xMin += 150 - GenUI.GapWide;
			Widgets.FillableBarLabeled(rect.ContractedBy(GenUI.GapTiny), Math.Abs(PowerUsage) / maxPowerUsage, 50, "Power");
			string label = "{0}W".Formatted(PowerUsage.ToString("0"));
			float w = label.GetWidthCached();
			rect = new RectOffset(-58, 0, -4, -4).Add(rect).LeftPartPixels(w + 4);
			Widgets.DrawRectFast(new RectOffset(0, 4, -4, -4).Add(rect), Color.black);
			Widgets.Label(new RectOffset(-4, 4, 0, 0).Add(rect), label);
			yref += GenUI.ListSpacing + GenUI.GapTiny;
			GUI.EndGroup();
		}
		
		public enum PowerType
		{
			None = 0,
			Producer = 1,
			Consumer = 2,
			Storage = 3
		}
	}
}