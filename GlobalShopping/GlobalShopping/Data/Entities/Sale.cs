using GlobalShopping.Enums;
using System.ComponentModel.DataAnnotations;

namespace GlobalShopping.Data.Entities
{
	public class Sale
	{
        public int Id { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm tt}")]
        [Display(Name = "Fecha")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public DateTime Date { get; set; }

        public User User { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Comentarios")]
        public string? Remarks { get; set; }

        [Display(Name = "Estatus")]
        public OrderStatus OrderStatus { get; set; }

        public ICollection<SaleDetail> SaleDetails { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        [Display(Name = "Líneas")]
        public int Lines => SaleDetails == null ? 0 : SaleDetails.Count;

        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Cantidad")]
        public float Quantity => SaleDetails == null ? 0 : SaleDetails.Sum(sd => sd.Quantity);

        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "SubTotal:")]
        public decimal SubTotal => SaleDetails == null ? 0 : SaleDetails.Sum(sd => sd.Value);

        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Itbis  (1.18%):")]
        public decimal Itbis => SaleDetails == null ? 0 : SaleDetails.Sum(sd => sd.Value) * 0.18M;

        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Total:")]
        public decimal Total => SubTotal + Itbis;

    }
}
