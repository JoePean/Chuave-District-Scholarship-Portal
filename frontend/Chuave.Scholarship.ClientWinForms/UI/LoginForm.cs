
using System;
using System.Drawing;
using System.Windows.Forms;
using Chuave.Scholarship.ClientWinForms.Services;

namespace Chuave.Scholarship.ClientWinForms.UI
{
    public class LoginForm : Form
    {
        private readonly ApiClient _api;
        private TextBox txtEmail = new TextBox();
        private TextBox txtPassword = new TextBox();
        private Button btnLogin = new Button();
        private Button btnRegister = new Button();
        private Label lblInfo = new Label();

        public LoginForm(ApiClient api)
        {
            _api = api;
            Text = "Chuave District Scholarship - Login";
            Width = 480;
            Height = 360;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;

            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24), BackColor = Color.White };
            Controls.Add(panel);

            var title = new Label
            {
                Text = "Chuave District Scholarship Portal",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 48,
                TextAlign = ContentAlignment.MiddleCenter
            };
            panel.Controls.Add(title);

            var card = new Panel { Width = 380, Height = 180, Left = 30, Top = 80, BackColor = Color.WhiteSmoke, BorderStyle = BorderStyle.FixedSingle };
            panel.Controls.Add(card);

            var lblEmail = new Label { Text = "Email", Left = 16, Top = 16, Width = 100 };
            txtEmail.Left = 16; txtEmail.Top = 36; txtEmail.Width = 340;
            var lblPassword = new Label { Text = "Password", Left = 16, Top = 70, Width = 100 };
            txtPassword.Left = 16; txtPassword.Top = 90; txtPassword.Width = 340; txtPassword.PasswordChar = '•';

            btnLogin.Text = "Login";
            btnLogin.Left = 16; btnLogin.Top = 130; btnLogin.Width = 160; btnLogin.Height = 32;
            btnLogin.BackColor = Color.FromArgb(29, 78, 216); btnLogin.ForeColor = Color.White;
            btnLogin.FlatStyle = FlatStyle.Flat;

            btnRegister.Text = "Register Applicant";
            btnRegister.Left = 196; btnRegister.Top = 130; btnRegister.Width = 160; btnRegister.Height = 32;
            btnRegister.BackColor = Color.FromArgb(16, 185, 129); btnRegister.ForeColor = Color.White;
            btnRegister.FlatStyle = FlatStyle.Flat;

            card.Controls.Add(lblEmail);
            card.Controls.Add(txtEmail);
            card.Controls.Add(lblPassword);
            card.Controls.Add(txtPassword);
            card.Controls.Add(btnLogin);
            card.Controls.Add(btnRegister);

            lblInfo.Dock = DockStyle.Bottom; lblInfo.Height = 28; lblInfo.TextAlign = ContentAlignment.MiddleCenter; lblInfo.ForeColor = Color.DimGray;
            panel.Controls.Add(lblInfo);

            btnLogin.Click += async (s, e) =>
            {
                btnLogin.Enabled = false;
                lblInfo.Text = "Signing in...";
                var ok = await _api.LoginAsync(txtEmail.Text, txtPassword.Text);
                if (ok) { DialogResult = DialogResult.OK; Close(); }
                else { MessageBox.Show("Invalid credentials", "Login failed", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                btnLogin.Enabled = true;
            };

            btnRegister.Click += async (s, e) =>
            {
                using var reg = new RegisterDialog();
                if (reg.ShowDialog() == DialogResult.OK)
                {
                    var ok = await _api.RegisterApplicantAsync(reg.FullName, reg.Email, reg.Password);
                    if (ok) MessageBox.Show("Registered! Now login.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else MessageBox.Show("Registration failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
        }

        private class RegisterDialog : Form
        {
            public string FullName => txtName.Text;
            public string Email => txtEmail.Text;
            public string Password => txtPassword.Text;

            TextBox txtName = new TextBox();
            TextBox txtEmail = new TextBox();
            TextBox txtPassword = new TextBox();

            public RegisterDialog()
            {
                Text = "Register as Applicant";
                Width = 420; Height = 300; StartPosition = FormStartPosition.CenterParent;
                var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(16) };
                Controls.Add(panel);

                var lbl1 = new Label { Text = "Full Name", Left = 8, Top = 8, Width = 120 };
                txtName.Left = 8; txtName.Top = 28; txtName.Width = 360;
                var lbl2 = new Label { Text = "Email", Left = 8, Top = 60, Width = 120 };
                txtEmail.Left = 8; txtEmail.Top = 80; txtEmail.Width = 360;
                var lbl3 = new Label { Text = "Password", Left = 8, Top = 112, Width = 120 };
                txtPassword.Left = 8; txtPassword.Top = 132; txtPassword.Width = 360; txtPassword.PasswordChar = '•';

                var ok = new Button { Text = "Register", DialogResult = DialogResult.OK, Left = 200, Top = 180, Width = 160, Height = 32, BackColor = System.Drawing.Color.FromArgb(16,185,129), ForeColor = System.Drawing.Color.White, FlatStyle = FlatStyle.Flat };
                var cancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Left = 8, Top = 180, Width = 160, Height = 32 };

                panel.Controls.Add(lbl1); panel.Controls.Add(txtName);
                panel.Controls.Add(lbl2); panel.Controls.Add(txtEmail);
                panel.Controls.Add(lbl3); panel.Controls.Add(txtPassword);
                panel.Controls.Add(ok); panel.Controls.Add(cancel);
            }
        }
    }
}
