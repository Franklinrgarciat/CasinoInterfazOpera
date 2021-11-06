using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades.ArchivosTxt
{
    public class CabeceraArchivo
    {
        public string TipoComprobante { get; set; }
        public string NumeroComprobante { get; set; }
        public string FechaEmision { get; set; }
        public string RazonSocial { get; set; }
        public string CategoriaIva { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string CodClienteOpera { get; set; }
        public string Domicilio { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
        public string CodigoPostal { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string Observaciones { get; set; }
        public string FormaDePago { get; set; }
        public string Moneda { get; set; }
        public Decimal CotizMonedaExtranjera { get; set; }
        public Decimal SubTotalGrav { get; set; }
        public Decimal SubTotalNOGrav { get; set; }
        public Decimal ImpTotalIva21 { get; set; }
        public Decimal ImpTotalIva10_5 { get; set; }
        public Decimal ImpTotalPercepciones { get; set; }
        public Decimal ImpTotalMonedaArg { get; set; }
        public Decimal ImpTotalMonedaExt { get; set; }
        public string FechaVencimiento { get; set; }
    }
}
