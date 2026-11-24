using System;
using System.Drawing;
using System.Windows.Forms;

class Program
{
    const string ConnStr = "server=127.0.0.1;port=3306;database=coding_habit_db;uid=CarlFran;pwd=1234;";

    // UI change so all butons and its design matches
    static void StyleButton(Button b, Color color)
    {
        b.FlatStyle = FlatStyle.Flat;
        b.FlatAppearance.BorderSize = 0;
        b.BackColor = color;
        b.ForeColor = Color.White;
        b.Font = new Font("Segoe UI Semibold", 10);
        b.Height = 40;
        b.TextAlign = ContentAlignment.MiddleLeft;
        b.Padding = new Padding(12, 0, 0, 0);
        b.Cursor = Cursors.Hand;
        b.FlatAppearance.MouseOverBackColor = ControlPaint.Light(color, 0.2f);
        b.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(color, 0.1f);
    }

    // UI small white bars i replaced the old single dashboard text
    static Panel StatCard(string caption, Color accent, Point location, out Label value)
    {
        var card = new Panel { Location = location, Size = new Size(220, 70), BackColor = Color.White };
        var strip = new Panel { Dock = DockStyle.Left, Width = 5, BackColor = accent };
        var cap = new Label
        {
            Text = caption, ForeColor = Color.Gray, Font = new Font("Segoe UI", 8.5f),
            Location = new Point(18, 8), AutoSize = true
        };
        value = new Label
        {
            Text = "0", Font = new Font("Segoe UI", 18, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 41, 59), Location = new Point(16, 27), AutoSize = true
        };
        card.Controls.AddRange(new Control[] { strip, cap, value });
        return card;
    }

    [STAThread]
    static void Main()
    {
        var repository = new HabitRepository(ConnStr);

        // UI colors 
        var sidebarBg = Color.FromArgb(30, 41, 59);   // dark slate
        var accent    = Color.FromArgb(99, 102, 241); // indigo
        var green     = Color.FromArgb(22, 163, 74);
        var amber     = Color.FromArgb(217, 119, 6);
        var red       = Color.FromArgb(220, 38, 38);
        var gray      = Color.FromArgb(100, 116, 139);
        var pageBg    = Color.FromArgb(241, 245, 249);

        var form = new Form
        {
            Text = "Coding Habit Tracker (MySQL)",
            Size = new Size(1000, 640),
            MinimumSize = new Size(900, 560),
            BackColor = pageBg,
            Font = new Font("Segoe UI", 10),
            StartPosition = FormStartPosition.CenterScreen
        };

        // LEFT SIDEBAR (title + input + buttons) 
        var sidebar = new Panel { Dock = DockStyle.Left, Width = 260, BackColor = sidebarBg };

        var title = new Label
        {
            Text = "Habit Records",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(25, 25),
            AutoSize = true
        };

        var subtitle = new Label
        {
            Text = "Coding Habit Tracker",
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.FromArgb(148, 163, 184),
            Location = new Point(27, 58),
            AutoSize = true
        };

        var inputLabel = new Label
        {
            Text = "HABIT NAME",
            Font = new Font("Segoe UI Semibold", 8.5f),
            ForeColor = Color.FromArgb(148, 163, 184),
            Location = new Point(27, 115),
            AutoSize = true
        };

        var box = new TextBox
        {
            Location = new Point(25, 140),
            Width = 210,
            Font = new Font("Segoe UI", 11),
            BorderStyle = BorderStyle.FixedSingle,
            PlaceholderText = "e.g., Java OOP, SQL JOINs..."
        };

        var btnCreate = new Button { Text = "Save",   Location = new Point(25, 195), Width = 210 };
        var btnUpdate = new Button { Text = "Update", Location = new Point(25, 243), Width = 210 };
        var btnDelete = new Button { Text = "Delete", Location = new Point(25, 291), Width = 210 };
        var btnClear  = new Button { Text = "Clear",  Location = new Point(25, 339), Width = 210 };

        StyleButton(btnCreate, green);
        StyleButton(btnUpdate, amber);
        StyleButton(btnDelete, red);
        StyleButton(btnClear, gray);

        var footer = new Label
        {
            Text = "MySQL  •  coding_habit_db",
            Font = new Font("Segoe UI", 8.5f),
            ForeColor = Color.FromArgb(100, 116, 139),
            Location = new Point(27, 565),
            AutoSize = true,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left
        };

        sidebar.Controls.AddRange(new Control[] { title, subtitle, inputLabel, box, btnCreate, btnUpdate, btnDelete, btnClear, footer });

        //  RIGHT SIDE (stat bars or textbox + records table) 
        var content = new Panel { Dock = DockStyle.Fill, Padding = new Padding(25), BackColor = pageBg };

        var topbar = new Panel { Dock = DockStyle.Top, Height = 150, BackColor = pageBg };

        var pageTitle = new Label
        {
            Text = "All Habit Records",
            Font = new Font("Segoe UI", 18, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 41, 59),
            Location = new Point(0, 0),
            AutoSize = true
        };

        var pageHint = new Label
        {
            Text = "Click a row to edit it using the panel on the left.",
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.Gray,
            Location = new Point(2, 36),
            AutoSize = true
        };

        var cardTotal = StatCard("TOTAL SAVED",  accent, new Point(0, 70),   out Label lblTotal);
        var cardToday = StatCard("LOGGED TODAY", green,  new Point(235, 70), out Label lblToday);
        topbar.Controls.AddRange(new Control[] { pageTitle, pageHint, cardTotal, cardToday });

        // UI changed the old Listbox (DataGridView) so it looks more like a record system
        var grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToResizeRows = false,
            MultiSelect = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            RowHeadersVisible = false,
            BorderStyle = BorderStyle.None,
            BackgroundColor = Color.White,
            GridColor = Color.FromArgb(226, 232, 240),
            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
            EnableHeadersVisualStyles = false,
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
            ColumnHeadersHeight = 42,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };
        grid.RowTemplate.Height = 38;
        grid.ColumnHeadersDefaultCellStyle.BackColor = sidebarBg;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = sidebarBg;
        grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 10);
        grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 0, 0);
        grid.DefaultCellStyle.Padding = new Padding(8, 0, 0, 0);
        grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(224, 231, 255);
        grid.DefaultCellStyle.SelectionForeColor = Color.Black;
        grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);

        grid.Columns.Add("id", "ID");
        grid.Columns.Add("date", "Date");
        grid.Columns.Add("name", "Habit");
        grid.Columns.Add("status", "Status");
        grid.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
        grid.Columns[0].Width = 60;
        grid.Columns[2].FillWeight = 200;

        // UI color the status text green means finished or done, orangeish is anything else
        grid.CellFormatting += (s, e) =>
        {
            if (e.ColumnIndex == 3 && e.Value is string st)
                e.CellStyle.ForeColor = st == "Completed" ? green : amber;
        };

        // Fill control is added first, then the Top one WinForms docking order
        content.Controls.Add(grid);
        content.Controls.Add(topbar);

        void RefreshDashboard()
        {
            grid.Rows.Clear();
            var habits = repository.FetchAll();
            int todayCount = 0;
            string today = DateTime.Now.ToString("yyyy-MM-dd");

            foreach (var h in habits)
            {
                int rowIndex = grid.Rows.Add(h.Id, h.HabitDate, h.HabitName, h.Status);
                grid.Rows[rowIndex].Tag = h;   // keeps the CodingHabit object inside the row
                if (h.HabitDate == today) todayCount++;
            }

            grid.CurrentCell = null;           // no row auto-selected after loading
            lblTotal.Text = habits.Count.ToString();
            lblToday.Text = todayCount.ToString();
        }

        // Creates the habit entry to save in the database
        btnCreate.Click += (s, e) =>
        {
            if (string.IsNullOrWhiteSpace(box.Text)) return;
            try
            {
                var habit = new CodingHabit(DateTime.Now.ToString("yyyy-MM-dd"), box.Text.Trim(), "Completed");
                repository.Insert(habit);
                box.Clear();
                RefreshDashboard();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database error on Save:\n{ex.Message}", "Error");
            }
        };

        // reads selection into TextBox for editing
        grid.SelectionChanged += (s, e) =>
        {
            if (grid.CurrentRow?.Tag is CodingHabit item)
                box.Text = item.HabitName;
        };

        // Updates the entry via text box in the dashboard along inside the SQL Database
        btnUpdate.Click += (s, e) =>
        {
            if (grid.CurrentRow?.Tag is not CodingHabit item)
            {
                MessageBox.Show("Select a habit from the table first.", "Update");
                return;
            }
            if (string.IsNullOrWhiteSpace(box.Text))
            {
                MessageBox.Show("Enter the updated habit name first.", "Update");
                return;
            }
            try
            {
                repository.Update(item.Id, box.Text.Trim());
                box.Clear();
                RefreshDashboard();
                MessageBox.Show("Habit updated in database.", "Updated");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database error on Update:\n{ex.Message}", "Error");
            }
        };

        // Easily deletes the habit logs both in dashboard and in database
        btnDelete.Click += (s, e) =>
        {
            if (grid.CurrentRow?.Tag is not CodingHabit item)
            {
                MessageBox.Show("Select a habit from the table first.", "Delete");
                return;
            }
            try
            {
                repository.Delete(item.Id);
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
            grid.ClearSelection();
            grid.CurrentCell = null;
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

        // Fill control first, then the Left one
        form.Controls.Add(content);
        form.Controls.Add(sidebar);
        Application.Run(form);
    }
}