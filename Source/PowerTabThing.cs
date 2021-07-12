using Verse;

namespace Compilatron
{
	public class PowerTabThing : IDrawableTabElement
	{
		private readonly Thing _thing; // Represents a battery, producer, or consumer
		private readonly float _parentTabWidth;
		
		public float Height => GenUI.ListSpacing + GenUI.GapTiny;

		/// <summary>
		/// Represents the data needed for drawing to the power tab a single item on the power grid,
		/// be it any one battery, workbench, light, or whatever else.
		/// </summary>
		/// <param name="thing">A battery, producer, or consumer on the power grid. As a rule of thumb,
		/// if it was passed from <see cref="PowerNetElements"/>, it should work just fine.</param>
		/// <param name="parentTabWidth">How wide the power tab page is</param>
		public PowerTabThing(Thing thing, float parentTabWidth)
		{
			_thing = thing;
			_parentTabWidth = parentTabWidth;
		}

		public void Draw(float y)
		{
			throw new System.NotImplementedException();
		}
	}
}