using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models
{
	public class ProductImage
	{
        public int Id { get; set; }
        [Required]
        public string imageUrl { get; set; }
        public int productId { get; set; }
        [ForeignKey("productId")]
        public Product Product { get; set; }
    }
}
