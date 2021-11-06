using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class DireccionEntrega
    {

        public int ID_DIRECCION_ENTREGA { get; set; }

        public string COD_DIRECCION_ENTREGA { get; set; }

        public string COD_CLIENTE { get; set; }

        public string DIRECCION { get; set; }

        public string COD_PROVINCIA { get; set; }

        public string LOCALIDAD { get; set; }

        public string HABITUAL { get; set; }

        public string CODIGO_POSTAL { get; set; }

        public string TELEFONO1 { get; set; }

        public string TELEFONO2 { get; set; }

        public string TOMA_IMPUESTO_HABITUAL { get; set; }

        public string FILLER { get; set; }

        public string OBSERVACIONES { get; set; }

        public int AL_FIJ_IB3 { get; set; }

        public string ALI_ADI_IB { get; set; }

        public string ALI_FIJ_IB { get; set; }

        public bool IB_L { get; set; }

        public bool IB_L3 { get; set; }

        public bool II_IB3 { get; set; }

        public string LIB { get; set; }

        public double PORC_L { get; set; }

        public string HABILITADO { get; set; }

        public string HORARIO_ENTREGA { get; set; }

        public string ENTREGA_LUNES { get; set; }

        public string ENTREGA_MARTES { get; set; }

        public string ENTREGA_MIERCOLES { get; set; }

        public string ENTREGA_JUEVES { get; set; }

        public string ENTREGA_VIERNES { get; set; }

        public string ENTREGA_SABADO { get; set; }

        public string ENTREGA_DOMINGO { get; set; }

        public string CONSIDERA_IVA_BASE_CALCULO_IIBB { get; set; }

        public string CONSIDERA_IVA_BASE_CALCULO_IIBB_ADIC { get; set; }

        public int WEB_ADDRESS_ID { get; set; }

    }
}
