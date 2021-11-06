using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class DetalleMovimientoSBA05
    {

        public int ID_SBA05 { get; set; }

        public string FILLER { get; set; }

        public int BARRA { get; set; }

        public double CANT_MONE { get; set; }

        public double CHEQUES { get; set; }

        public int CLASE { get; set; }

        public string COD_COMP { get; set; }

        public double COD_CTA { get; set; }

        public string COD_OPERAC { get; set; }

        public bool CONCILIADO { get; set; }

        public double COTIZ_MONE { get; set; }

        public string D_H { get; set; }

        public double EFECTIVO { get; set; }

        public DateTime FECHA { get; set; }

        public DateTime FECHA_CONC { get; set; }

        public string LEYENDA { get; set; }

        public double MONTO { get; set; }

        public string N_COMP { get; set; }

        public int RENGLON { get; set; }

        public double UNIDADES { get; set; }

        public string VA_DIRECTO { get; set; }

        public int ID_SBA02 { get; set; }

        public int ID_GVA81 { get; set; }

        public bool CONC_EFTV { get; set; }

        public DateTime F_CONC_EFT { get; set; }

        public string COMENTARIO { get; set; }

        public string COMENTARIO_EFT { get; set; }

        public string COD_GVA14 { get; set; }

        public string COD_CPA01 { get; set; }

        public int ID_CODIGO_RELACION { get; set; }

        public int ID_LEGAJO { get; set; }

        public string TIPO_COD_RELACIONADO { get; set; }

        public int ID_TIPO_COTIZACION { get; set; }

        public int ID_SBA11 { get; set; }
    }
}
