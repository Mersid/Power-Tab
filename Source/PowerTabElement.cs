using UnityEngine;
using Verse;

namespace Compilatron
{
	public class PowerTabElement : IDrawableTabElement
	{
		private readonly string _label;
		private readonly float _powerDraw;
		private readonly float _parentTabWidth;
		public float Height => Text.SmallFontHeight + GenUI.GapTiny * 2 + 2; // Note: No width since we're only really drawing down; the width is fixed.

		public PowerTabElement(string label, float powerDraw, float parentTabWidth)
		{
			_label = label;
			_powerDraw = powerDraw;
			_parentTabWidth = parentTabWidth;
		}
		
		public void Draw(float y)
		{
			Rect r = new Rect(0, y, _parentTabWidth - GenUI.GapTiny * 3 - GenUI.ScrollBarWidth, Text.SmallFontHeight + GenUI.GapTiny * 2);
                 
			Widgets.DrawOptionSelected(r);
			Widgets.Label(r, $"{_label}: {_powerDraw} W");
		}
	}
}