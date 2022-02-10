using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Company.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace Company.DAL
{
    public class Initializer : DropCreateDatabaseIfModelChanges<CompanyContext>
    {
        protected override void Seed(CompanyContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

            roleManager.Create(new IdentityRole("Admin"));

            var user = new ApplicationUser { UserName = "qwerty@wp.pl" };
            string password = "Qwerty2022!";

            userManager.Create(user, password);

            userManager.AddToRole(user.Id, "Admin");

            var profiles = new List<Profile>
            {
                new Profile
                {
                    UserName = "bartek98@int.pl",
                    Password = "Bartek2022!",
                    Name = "Jan",
                    Surname = "Nowak",
                    Address = "Armii krajowej 35 Poznań"
                }

            };
            profiles.ForEach(o => context.Profiles.Add(o));
            context.SaveChanges();

            var device = new List<Device>
            {
                new Device
                {
                    NameElectronicDevice = "HUAWEI P40 Lite 6/128GB Czarny",
                    deviceCategory= Device.DeviceCategory.telefon,
                    PricePerPieces=850,
                    quantity=20,
                    DeviceDescription ="Smartfon Huawei z ekranem 6,4 cala, wyświetlacz IPS TFT. Aparat 48 Mpix, pamięć 6 GB RAM",
                    Image = "huawei1.jpg"
                },
                new Device
                {
                    NameElectronicDevice = "LENOVO THINKPAD E14 Czarny",
                    deviceCategory= Device.DeviceCategory.notebook,
                    PricePerPieces=4399,
                    quantity=20,
                    DeviceDescription ="Ekran 14' 1920 x 1080px, 60Hz , Procesor Intel Core i5-10210U , Wielkość pamięci RAM [GB] 16, Dysk 256 GB SSD, Karta graficzna Intel UHD Graphics, System operacyjny Windows 10 Professional",
                    Image = "lenovo1.jpg"
                }
            };
            device.ForEach(o => context.Devices.Add(o));
            context.SaveChanges();
        }
    }
}