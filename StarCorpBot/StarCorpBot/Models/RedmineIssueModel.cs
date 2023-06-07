using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarCorpBot
{
    public partial class RedmineIssueModel
    {
        [JsonProperty("issues")]
        public List<Issue> Issues { get; set; }

        [JsonProperty("total_count")]
        public long TotalCount { get; set; }

        [JsonProperty("offset")]
        public long Offset { get; set; }

        [JsonProperty("limit")]
        public long Limit { get; set; }
    }

    public partial class Issue
    {

        [JsonProperty("project")]
        public Author Project { get; set; }

        [JsonProperty("tracker")]
        public Author Tracker { get; set; }

        [JsonProperty("status")]
        public Author Status { get; set; }

        [JsonProperty("priority")]
        public Author Priority { get; set; }

        [JsonProperty("author")]
        public Author Author { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("start_date")]
        public DateTimeOffset StartDate { get; set; }

        [JsonProperty("due_date")]
        public DateTime? DueDate { get; set; }

        [JsonProperty("done_ratio")]
        public long DoneRatio { get; set; }

        [JsonProperty("is_private")]
        public bool IsPrivate { get; set; }

        [JsonProperty("estimated_hours")]
        public string? EstimatedHours { get; set; }

        [JsonProperty("custom_fields")]
        public List<CustomField> CustomFields { get; set; }

        [JsonProperty("created_on")]
        public DateTimeOffset CreatedOn { get; set; }

        [JsonProperty("updated_on")]
        public DateTimeOffset UpdatedOn { get; set; }

        [JsonProperty("closed_on")]
        public object ClosedOn { get; set; }
    }

    public partial class Author
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
