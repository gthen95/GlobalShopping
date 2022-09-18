using GlobalShopping.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace GlobalShopping.Models
{
	public class ShowCartViewModel
	{
        public User User { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Comentarios")]
        public string Remarks { get; set; }

        public ICollection<TemporalSale> TemporalSales { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Cantidad")]
        public float Quantity => TemporalSales == null ? 0 : TemporalSales.Sum(ts => ts.Quantity);

        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "SubTotal:")]
        public decimal SubTotal => TemporalSales == null ? 0 : TemporalSales.Sum(ts => ts.Value);

        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Itbis  (1.18%):")]
        public decimal Itbis => TemporalSales == null ? 0 : TemporalSales.Sum(ts => ts.Value) * 0.18M;

        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Total:")]
        public decimal Total => SubTotal + Itbis;


    }
}
