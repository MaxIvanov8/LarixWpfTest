using CommunityToolkit.Mvvm.Input;
using LarixWpfTest.Properties;

namespace LarixWpfTest.Windows.AddEditWindow;

public partial class AddEditViewModel: AbstractViewModel
{
	private readonly bool _isEditMode;
	private bool _isChanged;

	private string _name;
	private string _surname;
	private int _age;
	private string _salaryString;
	private readonly Employee _employee;

	public event EventHandler OnRequestClose;
	public string ButtonOkContent { get; }

	public string Name
	{
		get => _name;
		set
		{
			SetProperty(ref _name, value);
			Update();
		}
	}

	public string Surname
	{
		get => _surname;
		set
		{
			SetProperty(ref _surname, value);
			Update();
		}
	}

	public int Age
	{
		get => _age;
		set
		{
			SetProperty(ref _age, value);
			Update();
		}
	}

	public string SalaryString
	{
		get => _salaryString;
		set
		{
			SetProperty(ref _salaryString, value);
			Update();
		}
	}

	public double Salary { get; private set; }
	public bool IsEntered { get; private set; }

	public AddEditViewModel()
	{
		Title = ButtonOkContent = Resources.Create;
		_age = 18;
		_salaryString = "0";
	}

	public AddEditViewModel(Employee employee)
	{
		Title = ButtonOkContent = Resources.Change;

		_isEditMode = true;

		_employee = employee;
		_name = _employee.Name;
		_surname = _employee.Surname;
		_age = _employee.Age;
		_salaryString = _employee.Salary.ToString();
	}

	private void Update()
	{
		if (!_isChanged) _isChanged = true;
		EnterCommand.NotifyCanExecuteChanged();
	}

	private void RaiseEvent() => OnRequestClose.Invoke(this, EventArgs.Empty);

	private bool CanExecuteEnter =>
		!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Surname) &&
		!string.IsNullOrEmpty(SalaryString) &&
		(_isEditMode && _isChanged || !_isEditMode);

	[RelayCommand(CanExecute = nameof(CanExecuteEnter))]
	private void Enter()
	{
		Salary = double.Parse(SalaryString);
		if (_isEditMode)
			_employee.SetValues(Name, Surname, Age, Salary);

		IsEntered = true;
		RaiseEvent();
	}

	[RelayCommand]
	protected virtual void Cancellation() => RaiseEvent();
}