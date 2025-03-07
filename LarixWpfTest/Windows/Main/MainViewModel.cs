using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LarixWpfTest.Properties;
using LarixWpfTest.Windows.AddEditWindow;
using Microsoft.Win32;

namespace LarixWpfTest.Windows.Main;

internal partial class MainViewModel : AbstractViewModel
{
	private const string FileFilter = "json files (*.json)|*.json";
	private CancellationTokenSource _cancelTokenSource;

	[ObservableProperty]
	private int _progressValue;

	[ObservableProperty]
	private bool _isProgressActive;

	[ObservableProperty] private ObservableCollection<Employee> _employees;

	[ObservableProperty] private bool _isSalaryShown;

	[ObservableProperty] private bool _isAgeShown;

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(DeleteEmployeeCommand), nameof(OpenEditWindowCommand))]
	private Employee? _selectedEmployee;
	public bool HasItems => Employees.Count > 0;

	private bool SelectedEmployeeNotNull => SelectedEmployee != null;

	public MainViewModel()
	{
		Title = "LarixWpfTest";
		Employees = [];
		Employees.CollectionChanged += (sender, args) => UpdateIsEnable();
	}

	private void UpdateIsEnable()
	{
		OnPropertyChanged(nameof(HasItems));
		SaveCommand.NotifyCanExecuteChanged();
	}

	[RelayCommand]
	public void OpenAddWindow()
	{
		var viewModel = new AddEditViewModel();
		var view = new AddEditWindow.AddEditWindow { DataContext = viewModel };
		viewModel.OnRequestClose += (_, _) => view.Close();
		view.ShowDialog();
		if (viewModel.IsEntered)
			Employees.Add(new Employee(viewModel.Name, viewModel.Surname, viewModel.Age, viewModel.Salary));
	}

	[RelayCommand(CanExecute = nameof(SelectedEmployeeNotNull))]
	public void OpenEditWindow()
	{
		var viewModel = new AddEditViewModel(SelectedEmployee);
		var view = new AddEditWindow.AddEditWindow { DataContext = viewModel };
		viewModel.OnRequestClose += (_, _) => view.Close();
		view.ShowDialog();
	}

	[RelayCommand(CanExecute = nameof(SelectedEmployeeNotNull))]
	private void DeleteEmployee()
	{
		if (MyMessageBox.ShowYesNo($"Вы уверены, что хотите удалить {SelectedEmployee.NameSurname}?"))
		{
			Employees.Remove(SelectedEmployee);
			if (!HasItems)
			{
				IsAgeShown = false;
				IsSalaryShown = false;
			}
		}
	}


	[RelayCommand(CanExecute = nameof(HasItems))]
	private async Task Save()
	{
		var dialog = new SaveFileDialog { Title = Resources.Save, Filter = FileFilter, FileName = "data" };
		if (dialog.ShowDialog() == true)
		{
			IsProgressActive = true;
			_cancelTokenSource = new CancellationTokenSource();
			await Task.Run(async () =>
			{
				try
				{
					HardWork(_cancelTokenSource.Token);
					await File.WriteAllTextAsync(dialog.FileName, JsonSerializer.Serialize(Employees), _cancelTokenSource.Token);
					MyMessageBox.Show($"Данные сохранены в файл {dialog.FileName}.");
				}
				catch (IOException)
				{
					MyMessageBox.ShowError("Произошла ошибка доступа к файлу.");
				}
				catch (Exception)
				{
					MyMessageBox.ShowError("Произошла ошибка при сохранении файла.");
				}
				finally
				{
					ProgressValue = 0;
					IsProgressActive = false;
				}
			}, _cancelTokenSource.Token);
		}
	}

	[RelayCommand]
	private async Task Open()
	{
		var dialog = new OpenFileDialog { Title = Resources.Download, Filter = FileFilter };
		if (dialog.ShowDialog() == true)
		{
			var isSuccess = false;
			IsProgressActive = true;
			_cancelTokenSource = new CancellationTokenSource();
			ObservableCollection<Employee> result = [];
			await Task.Run(async () =>
			{
				try
				{
					var res = HardWork(_cancelTokenSource.Token);
					if (!res)
					{
						MyMessageBox.Show("Загрузка данных отменена");
						ClearProgress();
						return;
					}

					await using var openStream = File.OpenRead(dialog.FileName);
					result = JsonSerializer.Deserialize<ObservableCollection<Employee>>(openStream) ??
					         throw new InvalidOperationException();
					isSuccess = true;
				}
				catch (IOException)
				{
					MyMessageBox.ShowError("Произошла ошибка доступа к файлу.");
				}
				catch (JsonException)
				{
					MyMessageBox.ShowError("Выбранный файл имеет неверный формат.");
				}
				catch (Exception)
				{
					MyMessageBox.ShowError("Произошла ошибка при загрузке файла.");
				}
				finally
				{
					ClearProgress();
				}
			}, _cancelTokenSource.Token);
			if (isSuccess)
			{
				Employees.Clear();
				foreach (var employee in result)
					Employees.Add(employee);
				MyMessageBox.Show($"Данные загружены из файла {dialog.FileName}.");
			}
		}
	}

[RelayCommand]
	private void Cancel() => _cancelTokenSource.Cancel();

	private void ClearProgress()
	{
		ProgressValue = 0;
		IsProgressActive = false;
	}

	private bool HardWork(CancellationToken token)
	{
		for (var i = 0; i < 100; i++)
		{
			if(token.IsCancellationRequested) return false;
			ProgressValue++;
			Thread.Sleep(50);
		}
		return true;
	}
}