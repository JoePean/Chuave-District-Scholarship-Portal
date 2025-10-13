
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Chuave.Scholarship.ClientWinForms.Services
{
    public class ApiClient
    {
        private readonly HttpClient _http;
        public string? Token { get; private set; }
        public string Role { get; private set; } = "";
        public int? ApplicantId { get; private set; }

        public ApiClient(string baseUrl)
        {
            _http = new HttpClient { BaseAddress = new Uri(baseUrl) };
        }

        private void ApplyAuth()
        {
            _http.DefaultRequestHeaders.Authorization = Token != null
                ? new AuthenticationHeaderValue("Bearer", Token)
                : null;
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            var res = await _http.PostAsJsonAsync("/api/auth/login", new { email, password });
            if (!res.IsSuccessStatusCode) return false;
            var doc = await res.Content.ReadFromJsonAsync<JsonElement>();
            Token = doc.GetProperty("token").GetString();
            Role = doc.GetProperty("role").GetString() ?? "";
            ApplyAuth();

            if (Role == "Applicant")
            {
                try
                {
                    var me = await GetMeAsync();
                    if (me.HasValue && me.Value.TryGetProperty("applicantId", out var idEl))
                        ApplicantId = idEl.GetInt32();
                } catch {}
            }
            return true;
        }

        public async Task<bool> RegisterApplicantAsync(string name, string email, string password)
        {
            var res = await _http.PostAsJsonAsync("/api/auth/register", new { email, password, role = "Applicant", name });
            return res.IsSuccessStatusCode;
        }

        public async Task<JsonElement?> GetMeAsync()
        {
            ApplyAuth();
            var res = await _http.GetAsync("/api/applicants/me");
            if (!res.IsSuccessStatusCode) return null;
            return await res.Content.ReadFromJsonAsync<JsonElement>();
        }

        public async Task<JsonElement> GetScholarshipsAsync()
        {
            var res = await _http.GetAsync("/api/scholarships");
            res.EnsureSuccessStatusCode();
            return await res.Content.ReadFromJsonAsync<JsonElement>();
        }

        public async Task<bool> ApplyAsync(int scholarshipId)
        {
            ApplyAuth();
            var res = await _http.PostAsync($"/api/applications/apply/{scholarshipId}", null);
            return res.IsSuccessStatusCode;
        }

        public async Task<JsonElement> MyApplicationsAsync()
        {
            ApplyAuth();
            var res = await _http.GetAsync("/api/applications/mine");
            res.EnsureSuccessStatusCode();
            return await res.Content.ReadFromJsonAsync<JsonElement>();
        }

        public async Task<JsonElement> Admin_ListApplicantsAsync()
        {
            ApplyAuth();
            var res = await _http.GetAsync("/api/applicants");
            res.EnsureSuccessStatusCode();
            return await res.Content.ReadFromJsonAsync<JsonElement>();
        }

        public async Task<JsonElement> Admin_ListApplicationsAsync()
        {
            ApplyAuth();
            var res = await _http.GetAsync("/api/applications");
            res.EnsureSuccessStatusCode();
            return await res.Content.ReadFromJsonAsync<JsonElement>();
        }

        public async Task<bool> Admin_SetStatusAsync(int appId, string status)
        {
            ApplyAuth();
            var res = await _http.PutAsync($"/api/applications/{appId}/status/{status}", null);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> Admin_CreateScholarshipAsync(object scholarship)
        {
            ApplyAuth();
            var res = await _http.PostAsJsonAsync("/api/scholarships", scholarship);
            return res.IsSuccessStatusCode;
        }
    }
}
