
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using Chuave.Scholarship.ClientWinForms.Services;

namespace Chuave.Scholarship.ClientWinForms.UI
{
    public class MainForm : Form
    {
        private readonly ApiClient _api;
        private TabControl tabs = new TabControl();
        private DataGridView gridScholarships = new DataGridView();
        private DataGridView gridMyApps = new DataGridView();
        private Button btnApply = new Button();

        private DataGridView gridApplicants = new DataGridView();
        private DataGridView gridAllApps = new DataGridView();
        private Button btnApprove = new Button();
        private Button btnReject = new Button();
        private GroupBox grpCreateScholarship = new GroupBox();
        private TextBox txtSchName = new TextBox();
        private TextBox txtSchCriteria = new TextBox();
        private TextBox txtSchSponsor = new TextBox();
        private NumericUpDown numFunding = new NumericUpDown();
        private DateTimePicker dtDeadline = new DateTimePicker();
        private Button btnCreateSch = new Button();

        public MainForm(ApiClient api)
        {
            _api = api;
            Text = "Chuave District Scholarship System";
            WindowState = FormWindowState.Maximized;
            BackColor = Color.White;

            tabs.Dock = DockStyle.Fill;
            Controls.Add(tabs);

            BuildApplicantTabs();
            if (_api.Role == "Admin")
            {
                BuildAdminTabs();
            }
        }

        private void BuildApplicantTabs()
        {
            var tab1 = new TabPage("Browse Scholarships");
            var tab2 = new TabPage("My Applications");

            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(12) };
            tab1.Controls.Add(panel);
            var header = new Label { Text = "Available Scholarships", Font = new Font("Segoe UI", 18, FontStyle.Bold), Dock = DockStyle.Top, Height = 42 };
            panel.Controls.Add(header);
            gridScholarships.Dock = DockStyle.Fill; gridScholarships.ReadOnly = true; gridScholarships.AllowUserToAddRows = false;
            gridScholarships.SelectionMode = DataGridViewSelectionMode.FullRowSelect; gridScholarships.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            panel.Controls.Add(gridScholarships);
            btnApply.Text = "Apply to Selected"; btnApply.Height = 38; btnApply.Dock = DockStyle.Bottom;
            btnApply.BackColor = Color.FromArgb(29,78,216); btnApply.ForeColor = Color.White; btnApply.FlatStyle = FlatStyle.Flat;
            btnApply.Click += async (s, e) =>
            {
                if (gridScholarships.SelectedRows.Count == 0) return;
                var id = Convert.ToInt32(gridScholarships.SelectedRows[0].Cells["ScholarshipId"].Value);
                var ok = await _api.ApplyAsync(id);
                if (ok) { MessageBox.Show("Application submitted!", "Success"); await LoadMyAppsAsync(); }
                else MessageBox.Show("Failed to apply.", "Error");
            };
            panel.Controls.Add(btnApply);

            var panel2 = new Panel { Dock = DockStyle.Fill, Padding = new Padding(12) };
            tab2.Controls.Add(panel2);
            var header2 = new Label { Text = "My Applications", Font = new Font("Segoe UI", 18, FontStyle.Bold), Dock = DockStyle.Top, Height = 42 };
            panel2.Controls.Add(header2);
            gridMyApps.Dock = DockStyle.Fill; gridMyApps.ReadOnly = true; gridMyApps.AllowUserToAddRows = false; gridMyApps.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            panel2.Controls.Add(gridMyApps);

            tabs.TabPages.Add(tab1);
            tabs.TabPages.Add(tab2);

            Shown += async (s, e) =>
            {
                await LoadScholarshipsAsync();
                if (_api.Role == "Applicant") await LoadMyAppsAsync();
            };
        }

        private void BuildAdminTabs()
        {
            var tabA = new TabPage("Admin - Applicants");
            var tabB = new TabPage("Admin - Applications");
            var tabC = new TabPage("Admin - Scholarships");

            var paneA = new Panel { Dock = DockStyle.Fill, Padding = new Padding(12) };
            tabA.Controls.Add(paneA);
            var headA = new Label { Text = "All Applicants", Font = new Font("Segoe UI", 18, FontStyle.Bold), Dock = DockStyle.Top, Height = 42 };
            paneA.Controls.Add(headA);
            gridApplicants.Dock = DockStyle.Fill; gridApplicants.ReadOnly = true; gridApplicants.AllowUserToAddRows = false;
            gridApplicants.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            paneA.Controls.Add(gridApplicants);

            var paneB = new Panel { Dock = DockStyle.Fill, Padding = new Padding(12) };
            tabB.Controls.Add(paneB);
            var headB = new Label { Text = "All Applications", Font = new Font("Segoe UI", 18, FontStyle.Bold), Dock = DockStyle.Top, Height = 42 };
            paneB.Controls.Add(headB);
            gridAllApps.Dock = DockStyle.Fill; gridAllApps.ReadOnly = true; gridAllApps.AllowUserToAddRows = false; gridAllApps.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            paneB.Controls.Add(gridAllApps);
            var actionBar = new Panel { Dock = DockStyle.Bottom, Height = 48 };
            btnApprove.Text = "Approve"; btnApprove.Width = 120; btnApprove.Height = 36; btnApprove.Left = 12; btnApprove.Top = 6;
            btnApprove.BackColor = Color.FromArgb(16,185,129); btnApprove.ForeColor = Color.White; btnApprove.FlatStyle = FlatStyle.Flat;
            btnReject.Text = "Reject"; btnReject.Width = 120; btnReject.Height = 36; btnReject.Left = 144; btnReject.Top = 6;
            btnReject.BackColor = Color.FromArgb(239,68,68); btnReject.ForeColor = Color.White; btnReject.FlatStyle = FlatStyle.Flat;
            actionBar.Controls.Add(btnApprove); actionBar.Controls.Add(btnReject);
            paneB.Controls.Add(actionBar);

            var paneC = new Panel { Dock = DockStyle.Fill, Padding = new Padding(12) };
            tabC.Controls.Add(paneC);
            var headC = new Label { Text = "Manage Scholarships", Font = new Font("Segoe UI", 18, FontStyle.Bold), Dock = DockStyle.Top, Height = 42 };
            paneC.Controls.Add(headC);
            grpCreateScholarship.Text = "Create Scholarship";
            grpCreateScholarship.Dock = DockStyle.Top; grpCreateScholarship.Height = 200;
            paneC.Controls.Add(grpCreateScholarship);

            var lblName = new Label { Text = "Name", Left = 12, Top = 24, Width = 120 };
            txtSchName.Left = 12; txtSchName.Top = 44; txtSchName.Width = 360;
            var lblCrit = new Label { Text = "Eligibility", Left = 12, Top = 76, Width = 120 };
            txtSchCriteria.Left = 12; txtSchCriteria.Top = 96; txtSchCriteria.Width = 360;
            var lblSponsor = new Label { Text = "Sponsor", Left = 400, Top = 24, Width = 120 };
            txtSchSponsor.Left = 400; txtSchSponsor.Top = 44; txtSchSponsor.Width = 260;
            var lblFunding = new Label { Text = "Funding Amount", Left = 400, Top = 76, Width = 120 };
            numFunding.Left = 400; numFunding.Top = 96; numFunding.Width = 120; numFunding.Maximum = 100000000; numFunding.DecimalPlaces = 2; numFunding.Value = 2500;
            var lblDeadline = new Label { Text = "Deadline", Left = 540, Top = 76, Width = 120 };
            dtDeadline.Left = 540; dtDeadline.Top = 96; dtDeadline.Width = 200;
            btnCreateSch.Text = "Create";
            btnCreateSch.Left = 12; btnCreateSch.Top = 140; btnCreateSch.Width = 160; btnCreateSch.Height = 34;
            btnCreateSch.BackColor = System.Drawing.Color.FromArgb(29,78,216); btnCreateSch.ForeColor = System.Drawing.Color.White; btnCreateSch.FlatStyle = FlatStyle.Flat;

            grpCreateScholarship.Controls.Add(lblName);
            grpCreateScholarship.Controls.Add(txtSchName);
            grpCreateScholarship.Controls.Add(lblCrit);
            grpCreateScholarship.Controls.Add(txtSchCriteria);
            grpCreateScholarship.Controls.Add(lblSponsor);
            grpCreateScholarship.Controls.Add(txtSchSponsor);
            grpCreateScholarship.Controls.Add(lblFunding);
            grpCreateScholarship.Controls.Add(numFunding);
            grpCreateScholarship.Controls.Add(lblDeadline);
            grpCreateScholarship.Controls.Add(dtDeadline);
            grpCreateScholarship.Controls.Add(btnCreateSch);

            tabs.TabPages.Add(tabA);
            tabs.TabPages.Add(tabB);
            tabs.TabPages.Add(tabC);

            btnCreateSch.Click += async (s, e) =>
            {
                var payload = new
                {
                    scholarshipName = txtSchName.Text,
                    description = "",
                    eligibilityCriteria = txtSchCriteria.Text,
                    fundingAmount = (decimal)numFunding.Value,
                    sponsor = txtSchSponsor.Text,
                    deadline = dtDeadline.Value
                };
                var ok = await _api.Admin_CreateScholarshipAsync(payload);
                if (ok) { MessageBox.Show("Created!", "Success"); await LoadScholarshipsAsync(); }
                else MessageBox.Show("Failed to create.", "Error");
            };

            Shown += async (s, e) =>
            {
                await LoadApplicantsAsync();
                await LoadAllAppsAsync();
            };
        }

        private async System.Threading.Tasks.Task LoadScholarshipsAsync()
        {
            var json = await _api.GetScholarshipsAsync();
            gridScholarships.DataSource = ToTable(json);
        }

        private async System.Threading.Tasks.Task LoadMyAppsAsync()
        {
            var json = await _api.MyApplicationsAsync();
            gridMyApps.DataSource = ToTable(json);
        }

        private async System.Threading.Tasks.Task LoadApplicantsAsync()
        {
            var json = await _api.Admin_ListApplicantsAsync();
            gridApplicants.DataSource = ToTable(json);
        }

        private async System.Threading.Tasks.Task LoadAllAppsAsync()
        {
            var json = await _api.Admin_ListApplicationsAsync();
            gridAllApps.DataSource = ToTable(json);
        }

        private async System.Threading.Tasks.Task UpdateStatusSelectedAsync(string status)
        {
            if (gridAllApps.SelectedRows.Count == 0) return;
            var id = Convert.ToInt32(gridAllApps.SelectedRows[0].Cells["ApplicationModelId"].Value);
            var ok = await _api.Admin_SetStatusAsync(id, status);
            if (ok) { await LoadAllAppsAsync(); MessageBox.Show($"Marked {status}", "Updated"); }
            else MessageBox.Show("Update failed.", "Error");
        }

        private static DataTable ToTable(JsonElement json)
        {
            var dt = new DataTable();
            if (json.ValueKind == JsonValueKind.Array && json.GetArrayLength() > 0)
            {
                var cols = json.EnumerateArray().SelectMany(x => x.EnumerateObject().Select(p => p.Name)).Distinct().ToList();
                foreach (var c in cols) dt.Columns.Add(c);
                foreach (var row in json.EnumerateArray())
                {
                    var dr = dt.NewRow();
                    foreach (var c in cols)
                    {
                        dr[c] = row.TryGetProperty(c, out var val) ? (val.ValueKind == JsonValueKind.Object || val.ValueKind == JsonValueKind.Array ? val.ToString() : (object?)val.ToString() ?? "") : "";
                    }
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }
    }
}
