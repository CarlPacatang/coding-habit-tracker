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

    static void UpdateHabit(int id, string habitName)
    {
        using var conn = new MySqlConnection(ConnStr);
        conn.Open();

        const string sql = "UPDATE habits SET habit_name = @name WHERE id = @id;";
        using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@name", habitName);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    static void DeleteHabit(int id)
    {
        using var conn = new MySqlConnection(ConnStr);
        conn.Open();

        const string sql = "DELETE FROM habits WHERE id = @id;";
        using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    [STAThread]
    static void Main()
    {
        var form = new Form
        {
            Text = "Coding Habit Tracker (MySQL)",
            Size = new Size(460, 340),
            BackColor = Color.White,
            StartPosition = FormStartPosition.CenterScreen
        };

        var title = new Label
        {
            Text = "Coding Habit Tracker",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Location = new Point(20, 15),
            AutoSize = true
        };

        var dash = new Label
        {
            Location = new Point(20, 45),
            ForeColor = Color.DimGray,
            AutoSize = true
        };

        var box = new TextBox
        {
            Location = new Point(20, 75),
            Width = 400,
            PlaceholderText = "Enter coding habit (e.g., Java OOP, SQL JOINs, C# CRUD)..."
        };

        var btnCreate = new Button { Text = "Save", Location = new Point(20, 108), Width = 92 };
        var btnUpdate = new Button { Text = "Update", Location = new Point(122, 108), Width = 92 };
        var btnDelete = new Button { Text = "Delete", Location = new Point(224, 108), Width = 92 };
        var btnClear  = new Button { Text = "Clear", Location = new Point(326, 108), Width = 94 };

        var list = new ListBox { Location = new Point(20, 148), Width = 400, Height = 135 };

        void RefreshDashboard()
        {
            list.Items.Clear();
            var habits = FetchAllHabits();
            int todayCount = 0;
            string today = DateTime.Now.ToString("yyyy-MM-dd");

            foreach (var h in habits)
            {
                list.Items.Add(h);
                if (h.HabitDate == today) todayCount++;
            }

            dash.Text = $"Dashboard — Total Saved: {habits.Count}   |   Logged Today: {todayCount}";
        }

        // Creates the habit entry to save in the database
        btnCreate.Click += (s, e) =>
        {
            if (string.IsNullOrWhiteSpace(box.Text)) return;
            try
            {
                InsertHabit(DateTime.Now.ToString("yyyy-MM-dd"), box.Text.Trim(), "Completed");
                box.Clear();
                RefreshDashboard();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database error on Save:\n{ex.Message}", "Error");
            }
        };

        // READ selection into TextBox for editing
        list.SelectedIndexChanged += (s, e) =>
        {
            if (list.SelectedItem is HabitItem item)
                box.Text = item.HabitName;
        };

        RefreshDashboard();
        form.Controls.AddRange(new Control[] { title, dash, box, btnCreate, btnUpdate, btnDelete, btnClear, list });
        Application.Run(form);
    }
}