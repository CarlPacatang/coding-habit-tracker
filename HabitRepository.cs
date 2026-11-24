using System.Collections.Generic;
using MySql.Data.MySqlClient;

class HabitRepository
{
    private readonly string _connStr;

    public HabitRepository(string connStr)
    {
        _connStr = connStr;
    }

    public List<CodingHabit> FetchAll()
    {
        var items = new List<CodingHabit>();
        using var conn = new MySqlConnection(_connStr);
        conn.Open();

        const string sql = "SELECT id, habit_date, habit_name, status FROM habits ORDER BY id DESC;";
        using var cmd = new MySqlCommand(sql, conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            items.Add(new CodingHabit(
                reader.GetInt32("id"),
                reader.GetString("habit_date"),
                reader.GetString("habit_name"),
                reader.GetString("status")
            ));
        }
        return items;
    }

    // This method inserts the logs to SQL database
    public void Insert(CodingHabit habit)
    {
        using var conn = new MySqlConnection(_connStr);
        conn.Open();

        const string sql = "INSERT INTO habits (habit_date, habit_name, status) VALUES (@date, @name, @status);";
        using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@date", habit.HabitDate);
        cmd.Parameters.AddWithValue("@name", habit.HabitName);
        cmd.Parameters.AddWithValue("@status", habit.Status);
        cmd.ExecuteNonQuery();
    }

    // Updates the habit logs both in app dashboard and in the database
    public void Update(int id, string habitName)
    {
        var validated = new CodingHabit(id, "2000-01-01", habitName, "Completed");

        using var conn = new MySqlConnection(_connStr);
        conn.Open();

        const string sql = "UPDATE habits SET habit_name = @name WHERE id = @id;";
        using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@name", validated.HabitName);
        cmd.Parameters.AddWithValue("@id", validated.Id);
        cmd.ExecuteNonQuery();
    }

    // Deletes the habit log from the database by ID
    public void Delete(int id)
    {
        using var conn = new MySqlConnection(_connStr);
        conn.Open();

        const string sql = "DELETE FROM habits WHERE id = @id;";
        using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }
}