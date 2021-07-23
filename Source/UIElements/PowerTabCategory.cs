using System.Collections.Generic;
using System.Linq;
using Verse;

namespace PowerTab.UIElements
{
	public class PowerTabCategory : IDrawableTabElement
	{
		private readonly string _label;
		private readonly float _power;
		private readonly float _barFill;
		private readonly IEnumerable<PowerTabGroup> _children;
		private readonly float _parentTabWidth;
		private readonly bool _isBattery;
		public float Height => 25 /*List separator height*/ + _children.Sum(t => t.Height);

		/// <summary>
		/// Represents the drawing of an entire category on the power tab, such as every battery, every producer, or every power consumer in the grid.
		/// </summary>
		/// <param name="label">Label of the category</param>
		/// <param name="power">How much power to display on the watt display</param>
		/// <param name="barFill">How much to fill the power bar, between 0 and 1</param>
		/// <param name="children">A list of <see cref="PowerTabGroup"/>s to draw in this category</param>
		/// <param name="parentTabWidth">How wide the power tab page is</param>
		/// <param name="isBattery">If it is a battery, display as Wd instead of W</param>
		public PowerTabCategory(string label, float power, float barFill, IEnumerable<PowerTabGroup> children, float parentTabWidth, bool isBattery = false)
		{
			_label = label;
			_power = power;
			_barFill = barFill;
			_children = children;
			_parentTabWidth = parentTabWidth;
			_isBattery = isBattery;
		}
		
		public void Draw(float y)
		{
			Widgets.ListSeparator(ref y, _parentTabWidth, _label); // y += 25 from ref
			foreach (PowerTabGroup child in _children)
			{
				child.Draw(y);
				y += child.Height;
			}
		}
	}
}