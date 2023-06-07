using Newtonsoft.Json;

namespace StarCorpBot
{
    public partial class RedmineTaskModel
    {
        [JsonProperty("time_entries")]
        public List<TimeEntry> TimeEntries { get; set; }

        [JsonProperty("total_count")]
        public long TotalCount { get; set; }

        [JsonProperty("offset")]
        public long Offset { get; set; }

        [JsonProperty("limit")]
        public long Limit { get; set; }
    }

    public partial class TimeEntry
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("project")]
        public Activity Project { get; set; }

        [JsonProperty("issue")]
        public Issue Issue { get; set; }

        [JsonProperty("user")]
        public Activity User { get; set; }

        [JsonProperty("activity")]
        public Activity Activity { get; set; }

        [JsonProperty("hours")]
        public double Hours { get; set; }

        [JsonProperty("comments")]
        public string Comments { get; set; }

        [JsonProperty("spent_on")]
        public DateTimeOffset SpentOn { get; set; }

        [JsonProperty("created_on")]
        public DateTimeOffset CreatedOn { get; set; }

        [JsonProperty("updated_on")]
        public DateTimeOffset UpdatedOn { get; set; }

        [JsonProperty("custom_fields")]
        public List<CustomField> CustomFields { get; set; }
    }

    public partial class Activity
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public partial class CustomField
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public partial class Issue
    {
        [JsonProperty("id")]
        public long Id { get; set; }
    }
}
