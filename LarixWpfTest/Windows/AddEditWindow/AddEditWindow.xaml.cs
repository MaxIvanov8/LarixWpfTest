using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LarixWpfTest.Windows.AddEditWindow;

/// <summary>
/// Interaction logic for AddEditWindow.xaml
/// </summary>
public partial class AddEditWindow
{
	private readonly Regex _regexNumbers;
	private readonly Regex _regexSymbols;
	public AddEditWindow()
	{
		InitializeComponent();
		Owner = Application.Current.MainWindow;
		_regexNumbers = new Regex("[0-9]+");
		_regexSymbols = new Regex("[a-zA-Zа-яА-Я/s-]+");
	}

	private void IntTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e) => e.Handled = !_regexNumbers.IsMatch(e.Text);
	private void SymbolsTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e) => e.Handled = !_regexSymbols.IsMatch(e.Text);

	private void DoubleTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
	{
		if (sender is TextBox textBox)
		{
			var currentText = textBox.Text;
			e.Handled = !(double.TryParse(currentText.Insert(textBox.SelectionStart, e.Text), out var d) && d >= 0d);
		}
	}
}