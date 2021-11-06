using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades.ArchivosTxt
{
    public class DetalleArchivo
    {
        public string TipoComprobante { get; set; }
        public string NumeroComprobante { get; set; }
        public string CodigoArticulo { get; set; }
        public Decimal Cantidad { get; set; }
        public Decimal Precio { get; set; }
        public Decimal Iva { get; set; }
    }
}
