using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class ArchivoTxt
    {
        public string Clientes { get; set; }
        public string CodigoCliente { get; set; }
        public string RazonSocial { get; set; }
        public string CondicionIva { get; set; }
        public string NumeroCuit { get; set; }
        public string Domicilio { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string DetallesComprobVentas { get; set; }
        public DateTime FechaEmision { get; set; }
        public string TipoComprobante { get; set; }
        public string NumeroComprobante { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string CondicionVenta { get; set; }
        public decimal ImporteReglon { get; set; }
        public decimal SubTotalBrutoGravado { get; set; }
        public decimal SubTotalBrutoNoGravado { get; set; }
        public decimal IvaAlicuota { get; set; }
        public decimal PercepIngresoBrutoDetallado { get; set; }
        public decimal ImporteTotal { get; set; }
        public string DetalleCobroFacturaContado { get; set; }
        public decimal ImporteCobroNumeroCuenta { get; set; }
        public decimal ImporteTarjetaDetalle { get; set; }
        public string TarjetaTipo { get; set; }
        public int NumeroCupon { get; set; }
        public string NombreUsuarioTarjeta { get; set; }
        public decimal ImporteTarjeta { get; set; }
        public int NumeroLote { get; set; }
        public int Cuotas { get; set; }

    }
}
