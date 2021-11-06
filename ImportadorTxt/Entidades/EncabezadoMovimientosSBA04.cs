using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class EncabezadoMovimientosSBA04
    {
        public int ID_SBA04 { get; set; }

        public string FILLER { get; set; }

        public int BARRA { get; set; }

        public bool CERRADO { get; set; }

        public int CLASE { get; set; }

        public string COD_COMP { get; set; }

        public string CONCEPTO { get; set; }

        public double COTIZACION { get; set; }

        public bool EXPORTADO { get; set; }

        public bool EXTERNO { get; set; }

        public DateTime FECHA { get; set; }

        public DateTime FECHA_ING { get; set; }

        public string HORA_ING { get; set; }

        public string N_COMP { get; set; }

        public double N_INTERNO { get; set; }

        public bool PASE { get; set; }

        public string SITUACION { get; set; }

        public string TERMINAL { get; set; }

        public string USUARIO { get; set; }

        public double LOTE { get; set; }

        public double LOTE_ANU { get; set; }

        public int SUCUR_ORI { get; set; }

        public DateTime FECHA_ORI { get; set; }

        public string C_COMP_ORI { get; set; }

        public string N_COMP_ORI { get; set; }

        public int BARRA_ORI { get; set; }

        public DateTime FECHA_EMIS { get; set; }

        public string GENERA_ASIENTO { get; set; }

        public DateTime FECHA_ULTIMA_MODIFICACION { get; set; }

        public string HORA_ULTIMA_MODIFICACION { get; set; }

        public string USUA_ULTIMA_MODIFICACION { get; set; }

        public string TERM_ULTIMA_MODIFICACION { get; set; }

        public int ID_PUESTO_CAJA { get; set; }

        public int ID_GVA81 { get; set; }

        public int ID_SBA02 { get; set; }

        public int ID_SBA02_C_COMP_ORI { get; set; }

        public string COD_GVA14 { get; set; }

        public string COD_CPA01 { get; set; }

        public int ID_CODIGO_RELACION { get; set; }

        public int ID_LEGAJO { get; set; }

        public string OBSERVACIONES { get; set; }

        public string TIPO_COD_RELACIONADO { get; set; }

        public string CN_ASTOR { get; set; }

        public int ID_MODELO_INGRESO_SB { get; set; }

        public double TOTAL_IMPORTE_CTE { get; set; }

        public double TOTAL_IMPORTE_EXT { get; set; }

        public string TRANSFERENCIA_DEVOLUCION_CUPONES { get; set; }

        public List<DetalleMovimientoSBA05> MovDetalle { get; set; }
        public List<CuponesSBA20> Cupones { get; set; }
    }
}
