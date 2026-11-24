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
    //This method inserts the logs to SQL database 
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
    // Updates the habit logs both in app dashboard and in the database
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

    // UI: one small helper so all four buttons share the same flat, colored style
    static void StyleButton(Button b, Color color)
    {
        b.FlatStyle = FlatStyle.Flat;
        b.FlatAppearance.BorderSize = 0;
        b.BackColor = color;
        b.ForeColor = Color.White;
        b.Font = new Font("Segoe UI Semibold", 10);
        b.Height = 38;
        b.Cursor = Cursors.Hand;
        b.FlatAppearance.MouseOverBackColor = ControlPaint.Light(color, 0.2f);
        b.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(color, 0.1f);
    }

    [STAThread]
    static void Main()
    {
        // UI: color palette
        var primary = Color.FromArgb(79, 70, 229);   // indigo
        var green   = Color.FromArgb(22, 163, 74);
        var amber   = Color.FromArgb(217, 119, 6);
        var red     = Color.FromArgb(220, 38, 38);
        var gray    = Color.FromArgb(107, 114, 128);
        var pageBg  = Color.FromArgb(243, 244, 246);

        var form = new Form
        {
            Text = "Coding Habit Tracker (MySQL)",
            Size = new Size(520, 560),
            MinimumSize = new Size(520, 560),
            BackColor = pageBg,
            Font = new Font("Segoe UI", 10),
            StartPosition = FormStartPosition.CenterScreen
        };

        // UI: colored header bar that holds the title and the dashboard text
        var header = new Panel
        {
            Dock = DockStyle.Top,
            Height = 80,
            BackColor = primary
        };

        var title = new Label
        {
            Text = "Coding Habit Tracker",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(25, 12),
            AutoSize = true
        };

        var dash = new Label
        {
            Location = new Point(27, 48),
            ForeColor = Color.FromArgb(224, 231, 255),
            Font = new Font("Segoe UI", 9.5f),
            AutoSize = true
        };
        header.Controls.AddRange(new Control[] { title, dash });

        // UI: white "card" behind the input and buttons
        var card = new Panel
        {
            Location = new Point(20, 95),
            Size = new Size(465, 120),
            BackColor = Color.White,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        var box = new TextBox
        {
            Location = new Point(15, 15),
            Width = 435,
            Font = new Font("Segoe UI", 11),
            BorderStyle = BorderStyle.FixedSingle,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            PlaceholderText = "Enter coding habit (e.g., Java OOP, SQL JOINs, C# CRUD)..."
        };

        var btnCreate = new Button { Text = "Save",   Location = new Point(15, 62),  Width = 100 };
        var btnUpdate = new Button { Text = "Update", Location = new Point(125, 62), Width = 100 };
        var btnDelete = new Button { Text = "Delete", Location = new Point(235, 62), Width = 100 };
        var btnClear  = new Button { Text = "Clear",  Location = new Point(345, 62), Width = 105 };

        StyleButton(btnCreate, green);
        StyleButton(btnUpdate, amber);
        StyleButton(btnDelete, red);
        StyleButton(btnClear, gray);
        card.Controls.AddRange(new Control[] { box, btnCreate, btnUpdate, btnDelete, btnClear });

        // UI: list is drawn by hand so each row looks like a card with a colored status
        var list = new ListBox
        {
            Location = new Point(20, 230),
            Size = new Size(465, 275),
            BorderStyle = BorderStyle.None,
            BackColor = Color.White,
            DrawMode = DrawMode.OwnerDrawFixed,
            ItemHeight = 40,
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
        };

        list.DrawItem += (s, e) =>
        {
            if (e.Index < 0) return;
            var item = (HabitItem)list.Items[e.Index];
            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            using var bg = new SolidBrush(selected ? Color.FromArgb(224, 231, 255) : Color.White);
            e.Graphics.FillRectangle(bg, e.Bounds);

            // habit name (left), date (under it), status (right)
            using var nameFont = new Font("Segoe UI Semibold", 10);
            using var dateFont = new Font("Segoe UI", 8.5f);
            e.Graphics.DrawString(item.HabitName, nameFont, Brushes.Black, e.Bounds.X + 12, e.Bounds.Y + 3);
            e.Graphics.DrawString(item.HabitDate, dateFont, Brushes.Gray, e.Bounds.X + 12, e.Bounds.Y + 21);

            var statusSize = e.Graphics.MeasureString(item.Status, dateFont);
            using var statusBrush = new SolidBrush(item.Status == "Completed" ? green : amber);
            e.Graphics.DrawString(item.Status, dateFont, statusBrush,
                e.Bounds.Right - statusSize.Width - 12, e.Bounds.Y + 12);

            // thin divider line between rows
            e.Graphics.DrawLine(Pens.Gainsboro, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
        };

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

        // Updates the entry via text box in the dashboard along inside the SQL Database
        btnUpdate.Click += (s, e) =>
        {
            if (list.SelectedItem is not HabitItem item)
            {
                MessageBox.Show("Select a habit from the list first.", "Update");
                return;
            }
            if (string.IsNullOrWhiteSpace(box.Text))
            {
                MessageBox.Show("Enter the updated habit name first.", "Update");
                return;
            }
            try
            {
                UpdateHabit(item.Id, box.Text.Trim());
                box.Clear();
                RefreshDashboard();
                MessageBox.Show("Habit updated in database.", "Updated");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database error on Update:\n{ex.Message}", "Error");
            }
        };
        //easily deletes the habit logs both in dashboard and in database
        btnDelete.Click += (s, e) =>
        {
            if (list.SelectedItem is not HabitItem item)
            {
                MessageBox.Show("Select a habit from the list first.", "Delete");
                return;
            }
            try
            {
                DeleteHabit(item.Id);
                box.Clear();
                RefreshDashboard();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database error on Delete:\n{ex.Message}", "Error");
            }
        };

        // Clears the text in the text box and deselects them
        btnClear.Click += (s, e) =>
        {
            list.ClearSelected();
            box.Clear();
            box.Focus();
        };

        try
        {
            RefreshDashboard();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Could not connect to MySQL:\n{ex.Message}", "Connection Error");
        }

        form.Controls.AddRange(new Control[] { header, card, list });
        Application.Run(form);
    }
}