﻿using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace PowerTab.UIElements
{
	public class PowerTabThing : IDrawableTabElement
	{
		private readonly Thing _thing; // Represents a battery, producer, or consumer
		private readonly float _power;
		private readonly float _barFill;
		private readonly float _parentTabWidth;
		private readonly bool _isBattery;

		public float Height => GenUI.ListSpacing + GenUI.GapTiny;

		/// <summary>
		/// Represents the data needed for drawing to the power tab a single item on the power grid,
		/// be it any one battery, workbench, light, or whatever else.
		/// </summary>
		/// <param name="thing">A battery, producer, or consumer on the power grid. As a rule of thumb,
		/// if it was passed from <see cref="PowerNetElements"/>, it should work just fine.</param>
		/// <param name="power">How much power to display on the watt display</param>
		/// <param name="barFill">How much to fill the power bar, between 0 and 1</param>
		/// <param name="parentTabWidth">How wide the power tab page is</param>
		/// <param name="isBattery">If it is a battery, display as Wd instead of W</param>
		public PowerTabThing(Thing thing, float power, float barFill, float parentTabWidth, bool isBattery = false)
		{
			_thing = thing;
			_power = power;
			_barFill = barFill;
			_parentTabWidth = parentTabWidth;
			_isBattery = isBattery;
		}

		public void Draw(float y)
		{
			Rect mainRect = new Rect(0, y, _parentTabWidth - GenUI.GapTiny * 3 - GenUI.ScrollBarWidth, GenUI.ListSpacing);
			Widgets.DrawHighlightIfMouseover(mainRect);
			if (Widgets.ButtonInvisible(mainRect)) CameraJumper.TryJumpAndSelect(new GlobalTargetInfo(_thing));

			Rect iconRect = new Rect(0, y, GenUI.ListSpacing, GenUI.ListSpacing);
			Widgets.ThingIcon(iconRect, _thing);

			Rect labelRect = new Rect(35, y + 3, _parentTabWidth / 2.5f, Text.SmallFontHeight); // Not dynamic width because area to right contains bar and watt info
			Widgets.Label(labelRect, _thing.LabelCap);

			Rect barRect = new Rect(_parentTabWidth / 2.5f + 40, y, _parentTabWidth / 2 - 25, GenUI.ListSpacing);
			Widgets.FillableBar(barRect.ContractedBy(2), Mathf.Clamp(_barFill, 0, 1)); 

			string powerDrawStr = $"{_power} " + (_isBattery ? "Wd" : "W");
			float textWidth = Text.CalcSize(powerDrawStr).x; // Calculate here instead of using cache since the numbers can change fast, and the cache can become outdated, leading to minor graphical issues.
			
			Rect wattBkgRect = new Rect(_parentTabWidth / 2.5f + 40, y, textWidth + 16, GenUI.ListSpacing);
			Widgets.DrawRectFast(wattBkgRect.ContractedBy(GenUI.GapTiny * 1.5f), Color.black);

			Rect wattLabelRect = new Rect(wattBkgRect.x + 6, y + 3, textWidth /*Small buffer to prevent potential overflow*/, GenUI.ListSpacing);
			Widgets.Label(wattLabelRect, powerDrawStr);
			
		}
	}
}