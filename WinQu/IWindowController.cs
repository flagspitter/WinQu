namespace WinQu
{
	public interface IWindowController
	{
		int Width    { get; set; }
		int Height   { get; set; }
		int Left     { get; set; }
		int Top      { get; set; }
		bool Visible { get; set; }

		void Show();
		void Hide();
		void Activate();
	}
}
