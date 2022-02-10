using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Company.Models
{
    [Table("Devices")]
    public class Device
    {
        [Key]
        public int ID { get; set; }
        [Display(Name = "Nazwa urządzenia: ")]
        public string NameElectronicDevice { get; set; }
        [Display(Name = "Kategoria urządzenia: ")]
        public DeviceCategory deviceCategory { get; set; }

        [Display(Name = "Cena PLN/szt: ")]
        public double PricePerPieces { get; set; }
        [Display(Name = "ilość dostępnych sztuk ")]
        public double quantity { get; set; }
        [StringLength(500, ErrorMessage = "Opis nie moze byc dluzszy niz 500 znakow.")]
        [Display(Name = "Opis sprzętu: ")]
        public string DeviceDescription { get; set; }
        [Display(Name = "Zdjęcie produktu: ")]
        public string Image { get; set; }

        [ForeignKey("DeviceID")]
        public virtual ICollection<Suppliers> suppliers { get; set; }

        [ForeignKey("DeviceID")]
        public virtual ICollection<Orders> orders { get; set; }
        public enum DeviceCategory
        {
            telefon,
            tablet,
            notebook
        }
    }
}