using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace PowerTab.UIElements
{
	/// <summary>
	/// Represents a group of items in the Power tab. Could be every sun lamp on the grid, or every chemfuel generator, or whatever
	/// </summary>
	public class PowerTabGroup : IDrawableTabElement
	{
		private readonly string _label;
		private readonly int _count;
		public float Power { get; }
		private readonly float _barFill;
		private readonly float _parentTabWidth;
		public IEnumerable<PowerTabThing> Children { get; }
		private bool _expanded;
		private readonly bool _isBattery;
		private readonly Action<PowerTabGroup> _ifButtonPressed;
		private static float SelfHeight => Text.SmallFontHeight + GenUI.GapTiny * 2 + 2;
		public float Height => SelfHeight + (_expanded ? Children.Sum(t => t.Height) : 1); // Note: No width since we're only really drawing down; the width is fixed.

		/// <summary>
		/// Represents the data needed for drawing to the power tab an entire group of the same thing,
		/// such as every light, cooler, or machining table in a grid.
		/// </summary>
		/// <param name="label">Name of the item in the group</param>
		/// <param name="count">Number of items. This is prepended to the label</param>
		/// <param name="power">How much power to display on the watt display</param>
		/// <param name="barFill">How much to fill the power bar, between 0 and 1</param>
		/// <param name="children">A list of <see cref="PowerTabThing"/>s to draw below this group if the expand button is pressed</param>
		/// <param name="parentTabWidth">How wide the power tab page is</param>
		/// <param name="expanded">Whether to draw this group with its children or not</param>
		/// <param name="isBattery">If it is a battery, display as Wd instead of W</param>
		/// <param name="ifButtonPressed">A callback that runs if the expand button on the side is pressed. Returns the <see cref="PowerTabGroup"/> that it is called from</param>
		public PowerTabGroup(string label, int count, float power, float barFill, IEnumerable<PowerTabThing> children, float parentTabWidth, bool expanded, bool isBattery = false,  Action<PowerTabGroup> ifButtonPressed = null)
		{
			_label = label;
			_count = count;
			Power = power;
			_barFill = barFill;
			_parentTabWidth = parentTabWidth;
			_expanded = expanded;
			_isBattery = isBattery;
			_ifButtonPressed = ifButtonPressed;
			Children = children;
		}
		
		public void Draw(float y)
		{
			Rect mainRect = new Rect(0, y, _parentTabWidth - GenUI.GapTiny * 3 - GenUI.ScrollBarWidth, Text.SmallFontHeight + GenUI.GapTiny * 2);
			Widgets.DrawOptionSelected(mainRect);
			
			Rect buttonRect = new Rect(2, y + 1, GenUI.ListSpacing, GenUI.ListSpacing);
			if (Widgets.ButtonText(buttonRect.ContractedBy(2), _expanded ? "-" : "+"))
				_ifButtonPressed.Invoke(this);

			Rect labelRect = new Rect(35, y + 4, _parentTabWidth / 2.5f, Text.SmallFontHeight);
			Widgets.Label(labelRect, $"{_count} {_label}");
			
			Rect barRect = new Rect(_parentTabWidth / 2.5f + 40, y + 1, _parentTabWidth / 2 - 25, GenUI.ListSpacing);
			Widgets.FillableBar(barRect.ContractedBy(2), Mathf.Clamp(_barFill, 0, 1)); 
			
			string powerDrawStr = $"{Power} " + (_isBattery ? "Wd" : "W");
			float textWidth = Text.CalcSize(powerDrawStr).x; // Calculate here instead of using cache since the numbers can change fast, and the cache can become outdated, leading to minor graphical issues.
			
			Rect wattBkgRect = new Rect(_parentTabWidth / 2.5f + 40, y + 1, textWidth + 16, GenUI.ListSpacing);
			Widgets.DrawRectFast(wattBkgRect.ContractedBy(GenUI.GapTiny * 1.5f), Color.black);

			Rect wattLabelRect = new Rect(wattBkgRect.x + 6, y + 5, textWidth /*Small buffer to prevent potential overflow*/, GenUI.ListSpacing);
			Widgets.Label(wattLabelRect, powerDrawStr);

			y += SelfHeight;

			if (!_expanded) return;
			foreach (PowerTabThing powerTabThing in Children)
			{
				powerTabThing.Draw(y);
				y += powerTabThing.Height;
			}
			
		}
	}
}