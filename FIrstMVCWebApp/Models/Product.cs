using System.ComponentModel.DataAnnotations;

namespace FIrstMVCWebApp.Models
{
    public class Product
    {
        [Key]
        public int Id {get; set;}

        public string ProductName {get; set;} = null!;
        public string Description {get; set;} = null!;
        public decimal price {get; set;} = 0.00m;
        public string color {get; set;} = null!;

    }
}