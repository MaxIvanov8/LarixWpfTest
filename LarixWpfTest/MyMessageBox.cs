using System.Windows;

namespace LarixWpfTest;

public static class MyMessageBox
{
	private const string ProgramMessage = "Программное сообщение";
	public static void Show(string text) => MessageBox.Show(text, ProgramMessage, MessageBoxButton.OK, MessageBoxImage.Information);
	public static void ShowError(string text) => MessageBox.Show(text, ProgramMessage, MessageBoxButton.OK, MessageBoxImage.Error);
	public static bool ShowYesNo(string text) => MessageBox.Show(text, ProgramMessage, MessageBoxButton.YesNo, MessageBoxImage.Question) ==
	                                             MessageBoxResult.Yes;
}