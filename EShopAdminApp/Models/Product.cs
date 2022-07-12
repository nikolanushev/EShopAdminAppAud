using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EShopAdminApp.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string ProductName { get; set; }

        public string ProductImage { get; set; }

        public string ProductDesrciption { get; set; }

        public float ProductPrice { get; set; }

        public int Rating { get; set; }
    }
}
