using CommunityToolkit.Mvvm.ComponentModel;

namespace LarixWpfTest.Windows;

public class AbstractViewModel:ObservableObject
{
	public string Title { get; protected set; }
}