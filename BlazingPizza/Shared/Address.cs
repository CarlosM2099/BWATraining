using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BlazingPizza.Shared
{
    public class Address
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Quien recibiar la orden?"), MaxLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Es necesario especificar la direccion de envio"), MaxLength(100)]
        public string Line1 { get; set; }

        [MaxLength(100, ErrorMessage = "No se pueden exceder 100 caracteres")]
        public string Line2 { get; set; }

        [Required(ErrorMessage = "La ciudad debe ser especificada"), MaxLength(50)]
        public string City { get; set; }

        [Required(ErrorMessage = "El estado debe ser especificado"), MaxLength(20)]
        public string Region { get; set; }

        [Required(ErrorMessage = "El codigo postal debe ser especificado"), MaxLength(20)]
        public string PostalCode { get; set; }
    }
}
