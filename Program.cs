using System;
using System.IO;

class Program
{
    static void Main()
    {
        //Our Core Feature
        Console.WriteLine("=== CODING HABIT TRACKER ===");
        // this section ask the user what coding habit they practiced today example, Java, Python, C#
        Console.Write("Enter today's coding task: ");
        string task = Console.ReadLine();
        //this is the date
        string log = $"{DateTime.Now:yyyy-MM-dd} - {task} [Completed]";
        // and this is the file where we will save the log.
        File.AppendAllText("habits.txt", log + Environment.NewLine);

        Console.WriteLine("\nSaved! Current Habit Log:");
        Console.WriteLine(File.ReadAllText("habits.txt"));
    }
}