namespace Compilatron
{
	/// <summary>
	/// Represents an element that can be drawn on the power tab page
	/// </summary>
	public interface IDrawableTabElement
	{
		float Height { get; }
		void Draw(float y);
	}
}