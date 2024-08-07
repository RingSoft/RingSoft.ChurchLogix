﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;

namespace RingSoft.ChurchLogix.DataAccess.Model.Financial_Management
{
    public class Fund
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Description { get; set; }

        [DefaultValue(0)]
        public double Goal { get; set; }

        [DefaultValue(0)]
        public double TotalCollected { get; set; }

        [DefaultValue(0)]
        public double TotalSpent { get; set; }

        public string? Notes { get; set; }

        public virtual ICollection<BudgetItem> Budgets { get; set; }
        public virtual ICollection<FundHistory> History { get; set; }
        public virtual ICollection<FundPeriodTotals> PeriodTotals { get; set; }
        public virtual ICollection<MemberGivingHistory> MemberGivingHistory { get; set; }
        public virtual ICollection<MemberGivingDetails> GivingDetails { get; set; }

        public Fund()
        {
            Budgets  = new HashSet<BudgetItem>();
            History = new HashSet<FundHistory>();
            PeriodTotals = new HashSet<FundPeriodTotals>();
            MemberGivingHistory = new HashSet<MemberGivingHistory>();
            GivingDetails = new HashSet<MemberGivingDetails>();
        }
    }
}
