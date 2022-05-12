﻿using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace DeploymentsHistoryBackend.Models
{
    public class Deployment
    {
        public int Id { get; set; }
        public string CommitId { get; set; }
        public int AppId { get; set; }
        [ForeignKey("AppId")]
        public Application Application { get; set; }
        public DateTime Timestamp { get; set; }

        [JsonIgnore]
        public bool IsValid =>
            string.IsNullOrWhiteSpace(CommitId) == false &&
            string.IsNullOrWhiteSpace(Application?.Name) == false &&
            Application.Id > 0 &&
            Timestamp > DateTime.MinValue;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }


        public override bool Equals(object? obj)
        {
            if (obj is not Deployment other) return false;
            return Application.Id == other.Application.Id && CommitId == other.CommitId && Timestamp == other.Timestamp;
        }

        public override int GetHashCode()
        {
            return (Application.Id + CommitId + Timestamp).GetHashCode();
        }
    }
}
