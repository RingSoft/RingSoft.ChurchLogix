﻿using System.ComponentModel.DataAnnotations;

namespace RingSoft.ChurchLogix.DataAccess.Model.ChurchLife
{
    public class Event
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        public DateTime BeginDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public double? MemberCost { get; set; }

        public double? TotalCost { get; set; }

        public double? TotalPaid { get; set; }

        public string? Notes { get; set; }

        public ICollection<EventMember> Members { get; set; }

        public Event()
        {
            Members = new HashSet<EventMember>();
        }
    }
}
