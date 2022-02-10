using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Company.ViewModels
{
    public class StatisticsViewModel
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "| Data Zamówienia | ")]
        public string TransactionDate { get; set; }
        [Display(Name = "Zamówienie |")]
        public int OrdersCount { get; set; }
    }
}