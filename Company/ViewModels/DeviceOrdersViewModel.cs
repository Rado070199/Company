using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Company.Models;
using System.Collections;

namespace Company.ViewModels
{
    public class DeviceOrdersViewModel
    {
        public IEnumerable<Orders> OrdersVME { get; set; }
        public Device DeviceVM { get; set; }
        public Orders OrdersVM { get; set; }
    }
}