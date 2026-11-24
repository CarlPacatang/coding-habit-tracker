using System;

class CodingHabit
{
    private string _habitName = string.Empty;

    public int Id { get; set; }
    public string HabitDate { get; set; } = string.Empty;
    public string Status { get; set; } = "Completed";

    public string HabitName
    {
        get => _habitName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Habit name cannot be empty.");
            _habitName = value.Trim();
        }
    }

    public CodingHabit(int id, string habitDate, string habitName, string status)
    {
        Id = id;
        HabitDate = habitDate;
        HabitName = habitName;
        Status = status;
    }

    public CodingHabit(string habitDate, string habitName, string status = "Completed")
    {
        HabitDate = habitDate;
        HabitName = habitName;
        Status = status;
    }

    public override string ToString() => $"{HabitDate} - {HabitName} [{Status}]";
}