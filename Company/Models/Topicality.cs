using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Company.Models
{
    [Table("Topicalities")]
    public class Topicality
    {
        [Key]
        public int ID { get; set; }
        [StringLength(500, ErrorMessage = "Topcicality can not be longer than 500 characters.")]
        [Display(Name = "Informacje: ")]
        public string TripDescription { get; set; }
    }
}