using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LarixWpfTest;

public partial class Employee:ObservableObject
{
	[ObservableProperty] private string _name;
	[ObservableProperty] private string _surname;
	[ObservableProperty] private int _age;
	[ObservableProperty] private double _salary;

	[JsonIgnore]
	public string NameSurname => $"{Name} {Surname}";

	public Employee(string name, string surname, int age, double salary) => SetValues(name, surname, age, salary);

	public void SetValues(string name, string surname, int age, double salary)
	{
		Name = name;
		Surname = surname;
		Age = age;
		Salary = salary;
	}
}