using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades.ArchivosTxt
{
    public class PercepcionesArchivo
    {
        public string TipoComprobante { get; set; }
        public string NumeroComprobante { get; set; }
        public string CodigoJuridiccion { get; set; }
        public Decimal Porcentaje { get; set; }
        public Decimal ImpoPercepcion { get; set; }
    }
}
