using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StarCorpBot.Models;

namespace StarCorpBot.Services
{
    public class RedmineServices
    {
        public static IConfiguration configuration;
        public RedmineServices(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        public async Task<RedmineTaskModel> GetDailyTasks(DateTime date)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"{configuration.GetSection("Redmine-API-Url").Value}time_entries.json?limit=100&spent_on={date.ToString("yyyy-MM-dd")}");
            request.Headers.Add("X-Redmine-API-Key", configuration.GetSection("Redmine-API-Key").Value);
            var conn = await client.SendAsync(request);
            conn.EnsureSuccessStatusCode();
            var response = JsonConvert.DeserializeObject<RedmineTaskModel>(await conn.Content.ReadAsStringAsync());

            return response;
        }
        public async Task<RedmineTaskModel> GetDailyTasks(DateTime date, string username)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"{configuration.GetSection("Redmine-API-Url").Value}time_entries.json?limit=100&spent_on={date.ToString("yyyy-MM-dd")}&name={username}");
            request.Headers.Add("X-Redmine-API-Key", configuration.GetSection("Redmine-API-Key").Value);
            var conn = await client.SendAsync(request);
            conn.EnsureSuccessStatusCode();
            var response = JsonConvert.DeserializeObject<RedmineTaskModel>(await conn.Content.ReadAsStringAsync());

            return response;
        }
        public async Task<RedmineUserModel> GetUser(string username)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"{configuration.GetSection("Redmine-API-Url").Value}users.json?limit=100&group_id=11&status=1&name={username}");
            request.Headers.Add("X-Redmine-API-Key", configuration.GetSection("Redmine-API-Key").Value);
            var conn = await client.SendAsync(request);
            conn.EnsureSuccessStatusCode();
            var response = JsonConvert.DeserializeObject<RedmineUserModel>(await conn.Content.ReadAsStringAsync());

            return response;
        }
        public async Task<RedmineUserModel> GetUsers()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"{configuration.GetSection("Redmine-API-Url").Value}users.json?limit=100&group_id=11&status=1");
            request.Headers.Add("X-Redmine-API-Key", configuration.GetSection("Redmine-API-Key").Value);
            var conn = await client.SendAsync(request);
            conn.EnsureSuccessStatusCode();
            var response = JsonConvert.DeserializeObject<RedmineUserModel>(await conn.Content.ReadAsStringAsync());

            return response;
        }
        public async Task<RedmineIssueModel> GetByTasks(List<long> taskIds)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"{configuration.GetSection("Redmine-API-Url").Value}issues.json?issue_id={string.Join(",",taskIds)}");
            request.Headers.Add("X-Redmine-API-Key", configuration.GetSection("Redmine-API-Key").Value);
            var conn = await client.SendAsync(request);
            conn.EnsureSuccessStatusCode();
            var response = JsonConvert.DeserializeObject<RedmineIssueModel>(await conn.Content.ReadAsStringAsync());

            return response;
        }
    }
}
