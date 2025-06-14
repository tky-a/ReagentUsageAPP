using System;
using System.ComponentModel;

namespace WpfApp2.Models
{
    public class Chemical
    {
        public int ChemicalId { get; set; }
        public string Name { get; set; }
        public string Class { get; set; }
        public decimal CurrentMass { get; set; }
        public string UseStatus { get; set; }
        public int StorageLocationId { get; set; }
        public string LocationName { get; set; }
        public int? LastUserId { get; set; }
        public string LastUserName { get; set; }
        public DateTime? LastUseDate { get; set; }
        public DateTime FirstDate { get; set; }
    }

    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
    }

    public class StorageLocation
    {
        public int LocationId { get; set; }
        public string LocationName { get; set; }
    }
    public class UsageRecord
    {
        public DateTime ActionDate { get; set; }
        public string ActionType { get; set; }
        public decimal MassBefore { get; set; }
        public decimal MassAfter { get; set; }
        public decimal UsageAmount { get; set; }
        public string Notes { get; set; }
        public string UserName { get; set; }
        public string ChemicalName { get; set; }

    }
}