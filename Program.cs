using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

record HabitItem(int Id, string HabitDate, string HabitName, string Status)
{
    public override string ToString() => $"{HabitDate} - {HabitName} [{Status}]";
}

class Program {
    const string ConnStr = "server=127.0.0.1;port=3306;database=coding_habit_db;uid=CarlFran;pwd=1234;";

    static List<HabitItem> FetchAllHabits()
    {
        var items = new List<HabitItem>();
        using var conn = new MySqlConnection(ConnStr);
        conn.Open();

        const string sql = "SELECT id, habit_date, habit_name, status FROM habits ORDER BY id DESC;";
        using var cmd = new MySqlCommand(sql, conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            items.Add(new HabitItem(
                reader.GetInt32("id"),
                reader.GetString("habit_date"),
                reader.GetString("habit_name"),
                reader.GetString("status")
            ));
        }
        return items;
    }

    static void InsertHabit(string habitDate, string habitName, string status)
    {
        using var conn = new MySqlConnection(ConnStr);
        conn.Open();

        const string sql = "INSERT INTO habits (habit_date, habit_name, status) VALUES (@date, @name, @status);";
        using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@date", habitDate);
        cmd.Parameters.AddWithValue("@name", habitName);
        cmd.Parameters.AddWithValue("@status", status);
        cmd.ExecuteNonQuery();
    }
}