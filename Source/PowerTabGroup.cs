using UnityEngine;
using Verse;

namespace Compilatron
{
	/// <summary>
	/// Represents a group of items in the Power tab. Could be every sun lamp on the grid, or every chemfuel generator, or whatever
	/// </summary>
	public class PowerTabGroup : IDrawableTabElement
	{
		private readonly string _label;
		private readonly float _powerDraw;
		private readonly float _parentTabWidth;
		public float Height => Text.SmallFontHeight + GenUI.GapTiny * 2 + 2; // Note: No width since we're only really drawing down; the width is fixed.

		public PowerTabGroup(string label, float powerDraw, float parentTabWidth)
		{
			_label = label;
			_powerDraw = powerDraw;
			_parentTabWidth = parentTabWidth;
		}
		
		public void Draw(float y)
		{
			Rect mainRect = new Rect(0, y, _parentTabWidth - GenUI.GapTiny * 3 - GenUI.ScrollBarWidth, Text.SmallFontHeight + GenUI.GapTiny * 2);
			Widgets.DrawOptionSelected(mainRect);

			Rect labelRect = new Rect(20, y + 4, _parentTabWidth, Text.SmallFontHeight + GenUI.GapTiny * 2);
			Widgets.Label(labelRect, $"{_label}: {_powerDraw} W");
		}
	}
}