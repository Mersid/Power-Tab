using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Compilatron
{
	public class PowerTabThing : IDrawableTabElement
	{
		private readonly Thing _thing; // Represents a battery, producer, or consumer
		private readonly float _powerDraw;
		private readonly float _barFill;
		private readonly float _parentTabWidth;
		
		public float Height => GenUI.ListSpacing + GenUI.GapTiny;

		/// <summary>
		/// Represents the data needed for drawing to the power tab a single item on the power grid,
		/// be it any one battery, workbench, light, or whatever else.
		/// </summary>
		/// <param name="thing">A battery, producer, or consumer on the power grid. As a rule of thumb,
		/// if it was passed from <see cref="PowerNetElements"/>, it should work just fine.</param>
		/// <param name="powerDraw"></param>
		/// <param name="barFill">How much to fill the power bar, between 0 and 1</param>
		/// <param name="parentTabWidth">How wide the power tab page is</param>
		public PowerTabThing(Thing thing, float powerDraw, float barFill, float parentTabWidth)
		{
			_thing = thing;
			_powerDraw = powerDraw;
			_barFill = barFill;
			_parentTabWidth = parentTabWidth;
		}

		public void Draw(float y)
		{
			Rect mainRect = new Rect(0, y, _parentTabWidth - GenUI.GapTiny * 3 - GenUI.ScrollBarWidth, GenUI.ListSpacing);
			Widgets.DrawHighlightIfMouseover(mainRect);
			if (Widgets.ButtonInvisible(mainRect)) CameraJumper.TryJumpAndSelect(new GlobalTargetInfo(_thing));

			Rect iconRect = new Rect(30, y, GenUI.ListSpacing, GenUI.ListSpacing);
			Widgets.ThingIcon(iconRect, _thing);

			Rect labelRect = new Rect(70, y + 3, _parentTabWidth / 2.5f, Text.SmallFontHeight);
			//Widgets.Label(labelRect, $"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ: {_powerDraw} W");
			Widgets.Label(labelRect, _thing.LabelCap);

			Rect barRect = new Rect(_parentTabWidth / 2.5f + 120, y, _parentTabWidth / 4, GenUI.ListSpacing);
			Widgets.FillableBar(barRect.ContractedBy(GenUI.GapTiny), _barFill / 1.32f);
		}
	}
}