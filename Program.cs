using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

class Program
{
    [STAThread]
    static void Main()
    {
        var form = new Form { Text = "Habit Dashboard", Size = new Size(420, 260), BackColor = Color.White };
        var box = new TextBox { Location = new Point(20, 20), Width = 250 };
        var btn = new Button { Text = "Save", Location = new Point(280, 18) };
        var dash = new Label { Location = new Point(20, 60), AutoSize = true }; // Single dashboard summary line
        var list = new ListBox { Location = new Point(20, 95), Width = 360, Height = 110 };

        void UpdateDashboard()
        {
            list.Items.Clear();
            if (File.Exists("habits.txt")) list.Items.AddRange(File.ReadAllLines("habits.txt"));
            dash.Text = $"Dashboard — Total Saved Habits: {list.Items.Count}";
        }

        btn.Click += (s, e) =>
        {
            if (!string.IsNullOrWhiteSpace(box.Text))
            {
                File.AppendAllText("habits.txt", $"{DateTime.Now:yyyy-MM-dd} - {box.Text} [Completed]{Environment.NewLine}");
                box.Clear();
                UpdateDashboard();
            }
        };

        UpdateDashboard();
        form.Controls.AddRange(new Control[] { box, btn, dash, list });
        Application.Run(form);
    }
}