using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades.ArchivosTxt
{
    public class PagosArchivo
    {
        public string TipoComprobante { get; set; }
        public string NumeroComprobante { get; set; }
        public string Moneda { get; set; }
        public Decimal CotizMonedaExt { get; set; }
        public string FechaPago { get; set; }
        public Decimal ImpMonedaArg { get; set; }
        public Decimal ImpMonedaExt { get; set; }
        public string FormaPago { get; set; }
        public string NumeroTarjeta { get; set; }
        public string NumeroCuponTarjeta { get; set; }
        public string Lote { get; set; }
        public int CantidadCuotas { get; set; }
        public string NumeroAutorizacion { get; set; }
        public string NumeroDocTarj { get; set; }
        public string FechaVencimiento { get; set; }
        public string NombreSocioTarj { get; set; }

    }
}
