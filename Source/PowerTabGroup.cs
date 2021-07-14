using System;
using System.Collections.Generic;
using System.Linq;
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
		private readonly int _count;
		private readonly float _power;
		private readonly float _parentTabWidth;
		private List<PowerTabThing> _children;
		private bool _expanded;
		private readonly Action<PowerTabGroup> _ifButtonPressed;
		public float Height => (Text.SmallFontHeight + GenUI.GapTiny * 2 + 2); // Note: No width since we're only really drawing down; the width is fixed.

		/// <summary>
		/// Represents the data needed for drawing to the power tab an entire group of the same thing,
		/// such as every light, cooler, or machining table in a grid.
		/// </summary>
		/// <param name="label">Name of the item in the group</param>
		/// <param name="count">Number of items. This is prepended to the label</param>
		/// <param name="power">How much power to display on the watt display</param>
		/// <param name="children">A list of <see cref="PowerTabThing"/>s to draw below this group if the expand button is pressed</param>
		/// <param name="parentTabWidth">How wide the power tab page is</param>
		/// <param name="expanded">Whether to draw this group with its children or not</param>
		/// <param name="ifButtonPressed">A callback that runs if the expand button on the side is pressed. Returns the <see cref="PowerTabGroup"/> that it is called from</param>
		public PowerTabGroup(string label, int count, float power, List<PowerTabThing> children, float parentTabWidth, bool expanded, Action<PowerTabGroup> ifButtonPressed = null)
		{
			_label = label;
			_count = count;
			_power = power;
			_parentTabWidth = parentTabWidth;
			_expanded = expanded;
			_ifButtonPressed = ifButtonPressed;
			_children = children;
		}
		
		public void Draw(float y)
		{
			Rect mainRect = new Rect(0, y, _parentTabWidth - GenUI.GapTiny * 3 - GenUI.ScrollBarWidth, Text.SmallFontHeight + GenUI.GapTiny * 2);
			Widgets.DrawOptionSelected(mainRect);
			
			Rect buttonRect = new Rect(2, y + 1, GenUI.ListSpacing, GenUI.ListSpacing);
			if (Widgets.ButtonText(buttonRect.ContractedBy(2), _expanded ? "-" : "+"))
				_ifButtonPressed.Invoke(this);

			Rect labelRect = new Rect(35, y + 4, _parentTabWidth, Text.SmallFontHeight + GenUI.GapTiny * 2);
			Widgets.Label(labelRect, $"{_count} {_label}");
		}
	}
}