using System;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace Chuave.Scholarship.ClientWinForms
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            var cfgPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.client.json");
            var baseUrl = "http://localhost:5145";

            if (File.Exists(cfgPath))
            {
                using var fs = File.OpenRead(cfgPath);
                var cfg = JsonSerializer.Deserialize<JsonElement>(fs);

                // rename 'api' -> 'apiSection' to avoid shadowing the ApiClient variable below
                if (cfg.TryGetProperty("Api", out var apiSection) &&
                    apiSection.TryGetProperty("BaseUrl", out var urlEl))
                {
                    baseUrl = urlEl.GetString() ?? baseUrl;
                }
            }

            var apiClient = new Services.ApiClient(baseUrl);
            using var login = new UI.LoginForm(apiClient);

            if (login.ShowDialog() == DialogResult.OK)
            {
                Application.Run(new UI.MainForm(apiClient));
            }
        }
    }
}
