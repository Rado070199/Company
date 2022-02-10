using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Company.DAL;

namespace Company.Models
{
    [Table("Orders")]
    public class Orders
    {
        [Key]
        public int ID { get; set; }
        public int ProfileID { get; set; }
        public int DeviceID { get; set; }
        [Range(0, 50)]
        [Display(Name = "zamówiono sztuk: ")]
        public int NumberOfPieces { get; set; }
        [Display(Name = "Koszt (PLN): ")]
        public double Price { get; set; }
        public Status status { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data Transakcji: ")]
        public string TransactionDate { get; set; }
        [ForeignKey("DeviceID")]
        public virtual Device device { get; set; }
        [ForeignKey("ProfileID")]
        public virtual Profile profile { get; set; }
        public enum Status
        {
            nieopłacone,
            opłacone
        }
        private CompanyContext db = new CompanyContext();
        public int SearchID(string userName)
        {
            Profile user = db.Profiles.Single(o => o.UserName.Equals(userName));
            return user.ID;
        }
    }
}