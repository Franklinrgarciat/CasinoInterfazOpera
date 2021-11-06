using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows.Forms;
using System.Globalization;
using System.Configuration;
using Entidades.Json;
using Newtonsoft.Json;

namespace Negocios
{
    public class NegGuardarDatos
    {
        public string rutaJsonFormaDePago = ConfigurationManager.AppSettings.Get("RutaJsonFormaDePago").ToString();
        public string rutaJsonProvincias = ConfigurationManager.AppSettings.Get("RutaJsonProvincias").ToString();
        public List<FormasDePago> DevolverFormasDePago()
        {
            var mapearAchivoJson = "";
            using (var reader = new StreamReader(rutaJsonFormaDePago))
            {
                mapearAchivoJson = reader.ReadToEnd();
            }
            var ListaJson = JsonConvert.DeserializeObject<List<FormasDePago>>(mapearAchivoJson);

            return ListaJson;
        }
        public List<Provincias> DevolverProvincias()
        {
            var mapearArchivoJson = "";
            using(var reader = new StreamReader(rutaJsonProvincias))
            {
                mapearArchivoJson = reader.ReadToEnd();
            }
            var ListaJson = JsonConvert.DeserializeObject<List<Provincias>>(mapearArchivoJson);
            return ListaJson;
        }


        public List<Entidades.ArchivosTxt.CabeceraArchivo> LeerCabeceraArchivo(string ruta1)
        {
            try
            {
                List<Entidades.ArchivosTxt.CabeceraArchivo> LCabecera = new List<Entidades.ArchivosTxt.CabeceraArchivo>();
                using (var reader = new StreamReader(ruta1))
                {
                    while (!reader.EndOfStream)
                    {
                        var linea = reader.ReadLine();
                        var dato = linea.Split(';');

                        Entidades.ArchivosTxt.CabeceraArchivo cabecera = new Entidades.ArchivosTxt.CabeceraArchivo();
                        cabecera.TipoComprobante = dato[0].ToString();
                        cabecera.NumeroComprobante = dato[1].ToString();
                        cabecera.FechaEmision = dato[2].ToString();
                        cabecera.RazonSocial = dato[3].ToString();
                        cabecera.CategoriaIva = dato[4].ToString();
                        cabecera.TipoDocumento = dato[5].ToString();
                        cabecera.NumeroDocumento = dato[6].ToString();
                        cabecera.CodClienteOpera = dato[7].ToString();
                        cabecera.Domicilio = dato[8].ToString();
                        cabecera.Localidad = dato[9].ToString();
                        cabecera.Provincia = dato[10].ToString();
                        cabecera.CodigoPostal = dato[11].ToString();
                        cabecera.Telefono = dato[12].ToString();
                        cabecera.Email = dato[13].ToString();
                        cabecera.Observaciones = dato[14].ToString();
                        cabecera.FormaDePago = dato[15].ToString();
                        cabecera.Moneda = dato[16].ToString();
                        cabecera.CotizMonedaExtranjera = Convert.ToDecimal(dato[17].ToString());
                        cabecera.SubTotalGrav = Convert.ToDecimal(dato[18].ToString());
                        cabecera.SubTotalNOGrav = Convert.ToDecimal(dato[19].ToString());
                        cabecera.ImpTotalIva21 = Convert.ToDecimal(dato[20].ToString());
                        cabecera.ImpTotalIva10_5 = Convert.ToDecimal(dato[21].ToString());
                        cabecera.ImpTotalPercepciones = Convert.ToDecimal(dato[22].ToString());
                        cabecera.ImpTotalMonedaArg = Convert.ToDecimal(dato[23].ToString());
                        cabecera.ImpTotalMonedaExt = Convert.ToDecimal(dato[24].ToString());
                        cabecera.FechaVencimiento = dato[25].ToString();

                        LCabecera.Add(cabecera);
                    }
                    return LCabecera;
                }

            }
            catch (Exception ex)
            {
                //MessageBox.Show("Cierre los archivos antes de exportar", MessageBoxIcon.Error.ToString(), MessageBoxButtons.OK);
                throw ex;
            }
        }

        public List<Entidades.ArchivosTxt.DetalleArchivo> LeerDetalleArchivo(string ruta2)
        {
            try
            {
                List<Entidades.ArchivosTxt.DetalleArchivo> LDetalle = new List<Entidades.ArchivosTxt.DetalleArchivo>();
                using (var reader = new StreamReader(ruta2))
                {
                    while (!reader.EndOfStream)
                    {
                        var linea = reader.ReadLine();
                        var dato = linea.Split(';');

                        Entidades.ArchivosTxt.DetalleArchivo detalle = new Entidades.ArchivosTxt.DetalleArchivo();
                        detalle.TipoComprobante = dato[0].ToString();
                        detalle.NumeroComprobante = dato[1].ToString();
                        detalle.CodigoArticulo = dato[2].ToString();
                        detalle.Cantidad = Convert.ToDecimal(dato[3].ToString());
                        detalle.Precio = Convert.ToDecimal(dato[4].ToString());
                        detalle.Iva = Convert.ToDecimal(dato[5].ToString());

                        LDetalle.Add(detalle);
                    }
                }
                return LDetalle;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cierre los archivos antes de exportar", MessageBoxIcon.Error.ToString(), MessageBoxButtons.OK);
                throw ex;
            }

        }
        public List<Entidades.ArchivosTxt.PagosArchivo> LeerPagosArchivo(string ruta3)
        {
            try
            {
                List<Entidades.ArchivosTxt.PagosArchivo> LPagos = new List<Entidades.ArchivosTxt.PagosArchivo>();
                using (var reader = new StreamReader(ruta3))
                {
                    while (!reader.EndOfStream)
                    {
                        var linea = reader.ReadLine();
                        var dato = linea.Split(';');

                        Entidades.ArchivosTxt.PagosArchivo pagos = new Entidades.ArchivosTxt.PagosArchivo();
                        pagos.TipoComprobante = dato[0].ToString();
                        pagos.NumeroComprobante = dato[1].ToString();
                        pagos.Moneda = dato[2].ToString();
                        pagos.CotizMonedaExt = Convert.ToDecimal(dato[3].ToString());
                        pagos.FechaPago = dato[4].ToString();
                        pagos.ImpMonedaArg = Convert.ToDecimal(dato[5].ToString());
                        pagos.ImpMonedaExt = Convert.ToDecimal(dato[6].ToString());
                        pagos.FormaPago = dato[7].ToString();
                        pagos.NumeroTarjeta = dato[8].ToString();
                        pagos.NumeroCuponTarjeta = dato[9].ToString();
                        pagos.Lote = dato[10].ToString();
                        pagos.CantidadCuotas = Convert.ToInt32(dato[11].ToString());
                        pagos.NumeroAutorizacion = dato[12].ToString();
                        pagos.NumeroDocTarj = dato[13].ToString();
                        pagos.FechaVencimiento = dato[14].ToString();
                        pagos.NombreSocioTarj = dato[15].ToString();

                        LPagos.Add(pagos);
                    }
                }
                return LPagos;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cierre los archivos antes de exportar", MessageBoxIcon.Error.ToString(), MessageBoxButtons.OK);
                throw ex;
            }
        }
        public List<Entidades.ArchivosTxt.PercepcionesArchivo> LeerPercepArchivo(string ruta4)
        {
            try
            {
                List<Entidades.ArchivosTxt.PercepcionesArchivo> LPercepciones = new List<Entidades.ArchivosTxt.PercepcionesArchivo>();
                if (!ruta4.Equals(""))
                {
                    using (var reader = new StreamReader(ruta4))
                    {
                        while (!reader.EndOfStream)
                        {
                            var linea = reader.ReadLine();
                            var dato = linea.Split(';');

                            Entidades.ArchivosTxt.PercepcionesArchivo percepciones = new Entidades.ArchivosTxt.PercepcionesArchivo();
                            percepciones.TipoComprobante = dato[0].ToString();
                            percepciones.NumeroComprobante = dato[1].ToString();
                            percepciones.CodigoJuridiccion = dato[2].ToString();
                            percepciones.Porcentaje = Convert.ToDecimal(dato[3].ToString());
                            percepciones.ImpoPercepcion = Convert.ToDecimal(dato[4].ToString());

                            LPercepciones.Add(percepciones);
                        }
                    }
                }
                return LPercepciones;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cierre los archivos antes de exportar", MessageBoxIcon.Error.ToString(), MessageBoxButtons.OK);
                throw ex;
            }
        }
        public bool GuardarEnBD(string ruta1, string ruta2, string ruta3, string ruta4)
        {
            try
            {
                List<Entidades.CabeceraGVA12> ListaCabeceraGVA12 = new List<Entidades.CabeceraGVA12>();
                Datos.GuardarDB dbCasino = new Datos.GuardarDB();
                List<Entidades.ArchivosTxt.CabeceraArchivo> ListaCabeceraArchivo = new List<Entidades.ArchivosTxt.CabeceraArchivo>();
                ListaCabeceraArchivo = LeerCabeceraArchivo(ruta1);
                List<Entidades.ArchivosTxt.DetalleArchivo> ListaDetalleArchivo = new List<Entidades.ArchivosTxt.DetalleArchivo>();
                ListaDetalleArchivo = LeerDetalleArchivo(ruta2);
                List<Entidades.ArchivosTxt.PagosArchivo> ListaPagosArchivo = new List<Entidades.ArchivosTxt.PagosArchivo>();
                ListaPagosArchivo = LeerPagosArchivo(ruta3);
                List<Entidades.ArchivosTxt.PercepcionesArchivo> ListaPercepArchivo = new List<Entidades.ArchivosTxt.PercepcionesArchivo>();
                ListaPercepArchivo = LeerPercepArchivo(ruta4);

                var ListaFormaPagoJson = DevolverFormasDePago();
                var ListaProvinciasJson = DevolverProvincias();
                List<string> CompNoPasados = new List<string>();
                int NumReglon = 0;

                bool GuardoConExito = false;

                foreach (var archivoCabecera in ListaCabeceraArchivo)
                {
                    Entidades.CabeceraGVA12 cabeceraGVA12 = new Entidades.CabeceraGVA12();
                    if (archivoCabecera.TipoComprobante == "FC")
                    {
                        cabeceraGVA12.T_COMP = "FAC";
                        cabeceraGVA12.TCOMP_IN_V = "FC";
                    }
                    else if (archivoCabecera.TipoComprobante == "ND")
                    {
                        cabeceraGVA12.T_COMP = "N/D";
                        cabeceraGVA12.TCOMP_IN_V = "ND";
                    }
                    else if (archivoCabecera.TipoComprobante == "NC")
                    {
                        cabeceraGVA12.T_COMP = "N/C";
                        cabeceraGVA12.TCOMP_IN_V = "CC";
                    }
                    else
                    {
                        throw new Exception($"El Tipo de comprobante: {archivoCabecera.TipoComprobante} no se agrego, tipo de Comp incorrecto");
                    }
                    cabeceraGVA12.N_COMP = archivoCabecera.NumeroComprobante.PadLeft(8, '0');
                    if (!dbCasino.ValidarComprobante(cabeceraGVA12.T_COMP, cabeceraGVA12.N_COMP))
                    {
                        string cod_cliente = dbCasino.TraerDatos("GVA14", "COD_CLIENT", true, "REPLACE(CUIT,'-','')", true, archivoCabecera.NumeroDocumento.Replace("-", ""));
                        int idCategoriaIva = Convert.ToInt32(dbCasino.TraerDatos("CATEGORIA_IVA", "ID_CATEGORIA_IVA", true, "COD_CATEGORIA_IVA", true, archivoCabecera.CategoriaIva));
                        int Talonario = Convert.ToInt32(ConfigurationManager.AppSettings.Get("Talonario")); //------------TRAER DATOS?
                        if (Talonario != 0)
                        {
                            cabeceraGVA12.TALONARIO = Talonario;
                        }
                        cabeceraGVA12.FILLER = "";
                        cabeceraGVA12.AFEC_STK = true;
                        cabeceraGVA12.CANC_COMP = "";
                        cabeceraGVA12.CANT_HOJAS = 1;
                        cabeceraGVA12.CAT_IVA = archivoCabecera.CategoriaIva;
                        cabeceraGVA12.CENT_STK = "N";
                        cabeceraGVA12.CENT_COB = "N";
                        cabeceraGVA12.CITI_OPERA = "0";
                        cabeceraGVA12.CITI_TIPO = "B";
                        cabeceraGVA12.COD_CAJA = 0;
                        cabeceraGVA12.COD_CLIENT = cod_cliente;
                        if (cabeceraGVA12.COD_CLIENT.Equals(""))
                        {
                            Entidades.ClienteGVA14 cliente = new Entidades.ClienteGVA14();
                            cliente.FILLER = "Sistema";
                            cliente.ADJUNTO = "";
                            cliente.ALI_NO_CAT = 0;
                            cliente.BMP = "";
                            cliente.C_POSTAL = archivoCabecera.CodigoPostal;
                            cliente.CALLE = "";
                            cliente.CALLE2_ENV = "";
                            cliente.CLAUSULA = false;
                            cliente.CLAVE_IS = "";
                           
                            string codigoCliente = dbCasino.TraerDatos("GVA16", "dbo.f_proximoCodigoCliente(PREFIJO)", true, "1", false, "1");
                            if (dbCasino.TraerDatos("gva14", "COD_CLIENT", true, "COD_CLIENT", true, codigoCliente) == string.Empty)
                            {
                                int iCodClient; 
                                string prefijo = dbCasino.TraerDatos("GVA16", "PREFIJO", true, "1", false, "1");
                                iCodClient = int.Parse(dbCasino.TraerDatos("gva14", "IsNull(max(cast(REPLACE(subString(COD_CLIENT,2,5),'.',',') as bigint)),0)+1", false, "COD_CLIENT like '" + prefijo + "%' and LEN(COD_CLIENT)=6 AND IsNumeric(REPLACE(subString(COD_CLIENT,2,5),'.',','))", false, "1"));
                                cliente.COD_CLIENT = prefijo + iCodClient.ToString().PadLeft(5, '0');
                            }
                            cliente.COD_PROVIN = ListaProvinciasJson.Where(p => p.CodigoProvinciaOpera.Equals(archivoCabecera.Provincia)).Select(pro => pro.CodigoProvinciaTango).FirstOrDefault();
                            cliente.COD_TRANSP = ConfigurationManager.AppSettings.Get("CodTransporte").ToString();
                            cliente.COD_VENDED = ConfigurationManager.AppSettings.Get("CodVendedor").ToString();
                            cliente.COD_ZONA = ConfigurationManager.AppSettings.Get("CodZona").ToString();
                            cliente.COND_VTA = Convert.ToInt32(ConfigurationManager.AppSettings.Get("CondVenta").ToString());
                            cliente.CUIT = archivoCabecera.NumeroDocumento; 
                            cliente.CUMPLEANIO = new DateTime(1800, 1, 1);
                            cliente.CUPO_CREDI = 0;
                            cliente.DIR_COM = archivoCabecera.Domicilio;
                            cliente.DOMICILIO = archivoCabecera.Domicilio;
                            cliente.DTO_ENVIO = "";
                            cliente.DTO_LEGAL = "";
                            cliente.E_MAIL = archivoCabecera.Email;
                            cliente.ENV_DOMIC = "";
                            cliente.ENV_LOCAL = "";
                            cliente.ENV_POSTAL = "";
                            cliente.ENV_PROV = "";
                            cliente.EXPORTA = true;
                            cliente.FECHA_ALTA = DateTime.Now;
                            cliente.FECHA_ANT = new DateTime(1800, 1, 1);
                            cliente.FECHA_DESD = new DateTime(1800, 1, 1);
                            cliente.FECHA_HAST = new DateTime(1800, 1, 1);
                            cliente.FECHA_INHA = new DateTime(1800, 1, 1);
                            cliente.FECHA_VTO = new DateTime(1800, 1, 1);
                            cliente.GRUPO_EMPR = "";
                            cliente.ID_EXTERNO = "";
                            cliente.ID_INTERNO = "";
                            cliente.II_D = "N";
                            cliente.II_L = "N";
                            switch (archivoCabecera.CategoriaIva)
                            {
                                case "CF":
                                    cliente.IVA_D = "N";
                                    cliente.IVA_L = "S";
                                    break;
                                case "EX":
                                    cliente.IVA_D = "N";
                                    cliente.IVA_L = "E";
                                    break;
                                case "EXE":
                                    cliente.IVA_D = "N";
                                    cliente.IVA_L = "N";
                                    break;
                                case "RI":
                                    cliente.IVA_D = "S";
                                    cliente.IVA_L = "S";
                                    break;
                                case "RS":
                                    cliente.IVA_D = "S";
                                    cliente.IVA_L = "M";
                                    break;
                            }
                            cliente.LOCALIDAD = archivoCabecera.Localidad;
                            cliente.N_IMPUESTO = "";
                            cliente.N_ING_BRUT = "";
                            cliente.NOM_COM = archivoCabecera.RazonSocial;
                            cliente.NRO_ENVIO = "";
                            cliente.NRO_LEGAL = "";
                            cliente.NRO_LISTA = Convert.ToInt32(ConfigurationManager.AppSettings.Get("NroLista").ToString());
                            cliente.OBSERVACIO = "";
                            cliente.PARTIDOENV = "";
                            cliente.PERMITE_IS = false;
                            cliente.PISO_ENVIO = "";
                            cliente.PISO_LEGAL = "";
                            cliente.PORC_DESC = 0;
                            cliente.PORC_EXCL = 0;
                            cliente.PORC_RECAR = 0;
                            cliente.PUNTAJE = 0;
                            cliente.RAZON_SOCI = archivoCabecera.RazonSocial;
                            cliente.SALDO_ANT = 0;
                            cliente.SALDO_CC = 0; 
                            cliente.SALDO_DOC = 0;
                            cliente.SALDO_D_UN = 0;
                            cliente.SOBRE_II = "";
                            cliente.SOBRE_IVA = "";
                            cliente.TELEFONO_1 = archivoCabecera.Telefono;
                            cliente.TELEFONO_2 = "";
                            cliente.TIPO = "";
                            if (archivoCabecera.TipoDocumento.Equals("CUIT") || archivoCabecera.TipoDocumento.Equals("CUIL"))
                            {
                                cliente.TIPO_DOC = 80;
                            }
                            else
                            {
                                cliente.TIPO_DOC = 96;
                            }
                            cliente.ZONA_ENVIO = "";
                            cliente.FECHA_MODI = new DateTime(1800, 1, 1);
                            cliente.EXP_SALDO = false;
                            cliente.N_PAGOELEC = "";
                            cliente.MON_CTE = true;
                            cliente.SAL_AN_UN = 0;
                            cliente.SALDO_CC_U = 0;
                            cliente.SUCUR_ORI = 1;
                            cliente.LIMCRE_EN = "N";
                            cliente.RG_1361 = true;
                            cliente.CAL_DEB_IN = false;
                            cliente.PORCE_INT = 0;
                            cliente.MON_MI_IN = 0;
                            cliente.DIAS_MI_IN = 0;
                            cliente.DESTINO_DE = "";
                            cliente.CLA_IMP_CL = "";
                            cliente.RECIBE_DE = false;
                            cliente.AUT_DE = false;
                            cliente.MAIL_DE = archivoCabecera.Email;
                            cliente.WEB = "";
                            cliente.COD_RUBRO = "";
                            cliente.CTA_CLI = 0;
                            cliente.CTO_CLI = "";
                            cliente.COD_GVA14 = cliente.COD_CLIENT;
                            cliente.CBU = "";
                            cliente.IDENTIF_AFIP = "";
                            cliente.IDIOMA_CTE = "1";
                            cliente.DET_ARTIC = "";
                            cliente.INC_COMENT = "";
                            cliente.ID_GVA44_FEX = 0;
                            cliente.ID_GVA44_NCEX = 0;
                            cliente.ID_GVA44_NDEX = 0;
                            cliente.CIUDAD = "";
                            cliente.CIUDAD_ENVIO = "";
                            cliente.APLICA_MORA = "";
                            cliente.ID_INTERES_POR_MORA = 0;
                            cliente.PUBLICA_WEB_CLIENTES = "";
                            cliente.MAIL_NEXO = "";
                            cliente.AUTORIZADO_WEB_CLIENTES = "";
                            cliente.OBSERVACIONES = archivoCabecera.Observaciones;
                            cliente.COD_GVA18 = cliente.COD_PROVIN; 
                            cliente.COD_GVA24 = cliente.COD_TRANSP;
                            cliente.COD_GVA23 = cliente.COD_VENDED;
                            cliente.COD_GVA05 = cliente.COD_ZONA;
                            cliente.COD_GVA62 = "";
                            cliente.COD_GVA151 = "";
                            cliente.COBRA_LUNES = "N";
                            cliente.COBRA_MARTES = "N";
                            cliente.COBRA_MIERCOLES = "N";
                            cliente.COBRA_JUEVES = "N";
                            cliente.COBRA_VIERNES = "N";
                            cliente.COBRA_SABADO = "N";
                            cliente.COBRA_DOMINGO = "N";
                            cliente.HORARIO_COBRANZA = "";
                            cliente.COMENTARIO_TYP_FAC = "";
                            cliente.COMENTARIO_TYP_ND = "";
                            cliente.COMENTARIO_TYP_NC = "";
                            cliente.TELEFONO_MOVIL = "";
                            cliente.ID_CATEGORIA_IVA = Convert.ToInt32(dbCasino.TraerDatos("CATEGORIA_IVA", "ID_CATEGORIA_IVA", true, "COD_CATEGORIA_IVA", true, archivoCabecera.CategoriaIva));
                            cliente.ID_GVA14 = Convert.ToInt32(dbCasino.TraerDatos("GVA14", "MAX(ID_GVA14)+1", true, "1", true, "1"));
                            cliente.COD_GVA150 = "";
                            cliente.TYP_FEX = "H";
                            cliente.TYP_NCEX = "H";
                            cliente.TYP_NDEX = "H";
                            cliente.COD_ACT_CNA25 = "";
                            cliente.COD_GVA05_ENV = "";
                            cliente.COD_GVA18_ENV = "";
                            cliente.RG_3572_EMPRESA_VINCULADA_CLIENTE = false;
                            cliente.RG_3572_TIPO_OPERACION_HABITUAL_VENTAS = "";
                            cliente.INHABILITADO_NEXO_PEDIDOS = "";
                            cliente.ID_TIPO_DOCUMENTO_EXTERIOR = 0;
                            cliente.NUMERO_DOCUMENTO_EXTERIOR = null;
                            cliente.WEB_CLIENT_ID = 0;
                            cliente.RG_3685_TIPO_OPERACION_VENTAS = "";
                            cliente.REQUIERE_INFORMACION_ADICIONAL = "N";
                            cliente.NRO_INSCR_RG1817 = "";
                            cliente.INHABILITADO_NEXO_COBRANZAS = "N";
                            cliente.CODIGO_AFINIDAD = "";
                            cliente.ID_TRA_ORIGEN_INFORMACION = 0;
                            cliente.SEXO = null;

                            Entidades.DireccionEntrega direccionEntrega = new Entidades.DireccionEntrega();
                            direccionEntrega.COD_DIRECCION_ENTREGA = "PRINCIPAL";
                            direccionEntrega.COD_CLIENTE = cliente.COD_CLIENT;
                            direccionEntrega.DIRECCION = cliente.DOMICILIO;
                            direccionEntrega.COD_PROVINCIA = cliente.COD_PROVIN;
                            direccionEntrega.LOCALIDAD = cliente.LOCALIDAD;
                            direccionEntrega.HABITUAL = "S";
                            direccionEntrega.CODIGO_POSTAL = cliente.C_POSTAL;
                            direccionEntrega.TELEFONO1 = cliente.TELEFONO_1;
                            direccionEntrega.TELEFONO2 = "";
                            direccionEntrega.TOMA_IMPUESTO_HABITUAL = "N";
                            direccionEntrega.FILLER = "";
                            direccionEntrega.OBSERVACIONES = cliente.OBSERVACIONES;
                            direccionEntrega.AL_FIJ_IB3 = 0;
                            direccionEntrega.ALI_ADI_IB = "";
                            direccionEntrega.ALI_FIJ_IB = "";
                            direccionEntrega.IB_L = true;
                            direccionEntrega.IB_L3 = true;
                            direccionEntrega.II_IB3 = true;
                            direccionEntrega.LIB = "N";
                            direccionEntrega.PORC_L = 0;
                            direccionEntrega.HABILITADO = "S";
                            direccionEntrega.HORARIO_ENTREGA = "";
                            direccionEntrega.ENTREGA_LUNES = "N";
                            direccionEntrega.ENTREGA_MARTES = "N";
                            direccionEntrega.ENTREGA_MIERCOLES = "N";
                            direccionEntrega.ENTREGA_JUEVES = "N";
                            direccionEntrega.ENTREGA_VIERNES = "N";
                            direccionEntrega.ENTREGA_SABADO = "N";
                            direccionEntrega.ENTREGA_DOMINGO = "N";
                            direccionEntrega.CONSIDERA_IVA_BASE_CALCULO_IIBB = "N";
                            direccionEntrega.CONSIDERA_IVA_BASE_CALCULO_IIBB_ADIC = "N";
                            direccionEntrega.WEB_ADDRESS_ID = 0;

                            cliente.DireccionEntrega = direccionEntrega;

                            cabeceraGVA12.Cliente = cliente;
                        }
                        cabeceraGVA12.COD_SUCURS = "";
                        // evaluamos = si es vacio codCliente traemos el dato del confi, sino, traer datos, codvend, codventa, codtransp
                        if (cabeceraGVA12.COD_CLIENT == "")
                        {
                            cabeceraGVA12.COD_TRANSP = ConfigurationManager.AppSettings.Get("CodTransporte").ToString();
                            cabeceraGVA12.COD_VENDED = ConfigurationManager.AppSettings.Get("CodVendedor").ToString();
                            cabeceraGVA12.COND_VTA = Convert.ToInt32(ConfigurationManager.AppSettings.Get("CondVenta").ToString());
                        }
                        else
                        {
                            cabeceraGVA12.COD_TRANSP = dbCasino.TraerDatos("GVA14", "COD_TRANSP", true, "COD_CLIENT", true, cabeceraGVA12.COD_CLIENT);
                            cabeceraGVA12.COD_VENDED = dbCasino.TraerDatos("GVA14", "COD_VENDED", true, "COD_CLIENT", true, cabeceraGVA12.COD_CLIENT);
                            cabeceraGVA12.COND_VTA = Convert.ToInt32(dbCasino.TraerDatos("GVA14", "COND_VTA", true, "COD_CLIENT", true, cabeceraGVA12.COD_CLIENT));
                        }
                        cabeceraGVA12.CONTABILIZ = true;
                        cabeceraGVA12.CONTFISCAL = false;
                        cabeceraGVA12.COTIZ = 1;
                        cabeceraGVA12.DESC_PANT = 0;
                        if (archivoCabecera.FormaDePago.Equals("CDO"))
                        {
                            cabeceraGVA12.ESTADO = "***";
                        }
                        else
                        {
                            cabeceraGVA12.ESTADO = "PEN";
                        }
                        cabeceraGVA12.ESTADO_STK = "";
                        cabeceraGVA12.EXPORTADO = false;
                        cabeceraGVA12.FECHA_ANU = new DateTime(1800, 1, 1);
                        cabeceraGVA12.FECHA_EMIS = Convert.ToDateTime(String.Format("{0:yyyy-MM-dd HH:mm:ss}", archivoCabecera.FechaEmision)).Date;
                        cabeceraGVA12.ID_CIERRE = 0;
                        cabeceraGVA12.IMPORTE = Convert.ToDouble(archivoCabecera.ImpTotalMonedaArg);
                        cabeceraGVA12.IMPORTE_BO = 0;
                        cabeceraGVA12.IMPORTE_EX = 0;
                        cabeceraGVA12.IMPORTE_FL = 0;
                        cabeceraGVA12.IMPORTE_GR = Convert.ToDouble(archivoCabecera.ImpTotalMonedaArg) - Convert.ToDouble(archivoCabecera.ImpTotalIva10_5) - 
                            Convert.ToDouble(archivoCabecera.ImpTotalIva21) - Convert.ToDouble(archivoCabecera.ImpTotalPercepciones); // OPERAC = TOTAL - IMPUESTOS
                        cabeceraGVA12.IMPORTE_IN = 0;
                        cabeceraGVA12.IMPORTE_IV = Convert.ToDouble(archivoCabecera.ImpTotalIva10_5) + Convert.ToDouble(archivoCabecera.ImpTotalIva21);// SUMA DE IVA'S
                        cabeceraGVA12.IMP_TICK_N = 0;
                        cabeceraGVA12.IMP_TICK_P = 0;
                        cabeceraGVA12.LEYENDA = "";
                        cabeceraGVA12.LOTE = 0;
                        cabeceraGVA12.MON_CTE = true;
                        cabeceraGVA12.MOTI_ANU = "";
                        //NRO_DE_LIS = si esta vacio el cod_client lo traigo del conf, y si no hago un traer datos de gva14
                        if (cabeceraGVA12.COD_CLIENT == "")
                        {
                            cabeceraGVA12.NRO_DE_LIS = Convert.ToInt32(ConfigurationManager.AppSettings.Get("NroLista").ToString());
                        }
                        else
                        {
                            cabeceraGVA12.NRO_DE_LIS = Convert.ToInt32(dbCasino.TraerDatos("GVA14", "NRO_LISTA", true, "COD_CLIENT", true, cabeceraGVA12.COD_CLIENT));
                        }
                        cabeceraGVA12.NRO_SUCURS = 0;
                        cabeceraGVA12.ORIGEN = "";
                        cabeceraGVA12.PORC_BONIF = 0;
                        cabeceraGVA12.PORC_PRO = 0;
                        cabeceraGVA12.PORC_REC = 0;
                        cabeceraGVA12.PORC_TICK = 0;
                        cabeceraGVA12.PROPINA = 0;
                        cabeceraGVA12.PROPINA_EX = 0;
                        cabeceraGVA12.PTO_VTA = false;
                        cabeceraGVA12.REC_PANT = 0;
                        cabeceraGVA12.TICKET = "N";
                        cabeceraGVA12.TIPO_ASIEN = ConfigurationManager.AppSettings.Get("TipoAsiento").ToString();
                        cabeceraGVA12.TIPO_EXPOR = "";
                        cabeceraGVA12.TIPO_VEND = "V";
                        cabeceraGVA12.T_FORM = "";
                        cabeceraGVA12.UNIDADES = cabeceraGVA12.IMPORTE;
                        cabeceraGVA12.LOTE_ANU = 0;
                        cabeceraGVA12.PORC_INT = 0;
                        cabeceraGVA12.PORC_FLE = 0;
                        cabeceraGVA12.ESTADO_UNI = cabeceraGVA12.ESTADO;
                        cabeceraGVA12.ID_CFISCAL = "";
                        cabeceraGVA12.NUMERO_Z = 0;
                        cabeceraGVA12.HORA_COMP = "0";
                        cabeceraGVA12.SENIA = false;
                        cabeceraGVA12.ID_TURNO = 0;
                        cabeceraGVA12.ID_TURNOX = 0;
                        cabeceraGVA12.HORA_ANU = "0";
                        cabeceraGVA12.CCONTROL = "";
                        cabeceraGVA12.ID_A_RENTA = 0;
                        cabeceraGVA12.COD_CLASIF = "";
                        cabeceraGVA12.AFEC_CIERR = "";
                        cabeceraGVA12.CAICAE = "";
                        cabeceraGVA12.CAICAE_VTO = DateTime.Parse(archivoCabecera.FechaVencimiento); 
                        cabeceraGVA12.DOC_ELECTR = false;
                        cabeceraGVA12.SERV_DESDE = new DateTime(1800, 1, 1);
                        cabeceraGVA12.SERV_HASTA = new DateTime(1800, 1, 1);
                        cabeceraGVA12.CANT_IMP = 0;
                        cabeceraGVA12.CANT_MAIL = 0;
                        cabeceraGVA12.ULT_IMP = new DateTime(1800, 1, 1);
                        cabeceraGVA12.ULT_MAIL = new DateTime(1800, 1, 1);
                        cabeceraGVA12.MORA_SOBRE = "";
                        cabeceraGVA12.ESTADO_ANT = "";
                        cabeceraGVA12.T_DOC_DTE = "";
                        cabeceraGVA12.DTE_ANU = "";
                        cabeceraGVA12.FOLIO_ANU = "";
                        cabeceraGVA12.REBAJA_DEB = false;
                        cabeceraGVA12.SUCURS_SII = 0;
                        cabeceraGVA12.EXENTA = false;
                        cabeceraGVA12.MOTIVO_DTE = 0;
                        cabeceraGVA12.IMPOR_EXT = 0;
                        cabeceraGVA12.CERRADO = false;
                        cabeceraGVA12.IMP_BO_EXT = 0;
                        cabeceraGVA12.IMP_EX_EXT = 0;
                        cabeceraGVA12.IMP_FL_EXT = 0;
                        cabeceraGVA12.IMP_GR_EXT = 0;
                        cabeceraGVA12.IMP_IN_EXT = 0;
                        cabeceraGVA12.IMP_IV_EXT = 0;
                        cabeceraGVA12.IM_TIC_N_E = 0;
                        cabeceraGVA12.IM_TIC_P_E = 0;
                        cabeceraGVA12.UNIDAD_EXT = 0;
                        cabeceraGVA12.PROPIN_EXT = 0;
                        cabeceraGVA12.PRO_EX_EXT = 0;
                        cabeceraGVA12.REC_PAN_EX = 0;
                        cabeceraGVA12.DES_PAN_EX = 0;
                        cabeceraGVA12.T_DTO_COMP = "";
                        cabeceraGVA12.RECARGO_PV = 0;
                        cabeceraGVA12.NCOMP_IN_V = 0;// guardar en el insert
                        if (cabeceraGVA12.T_COMP == "FAC")
                        {
                            cabeceraGVA12.ID_ASIENTO_MODELO_GV = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ID_ASIENTO_MODELO_GV_FAC").ToString());
                        }
                        else
                        {
                            cabeceraGVA12.ID_ASIENTO_MODELO_GV = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ID_ASIENTO_MODELO_GV_NC").ToString());
                        }
                        cabeceraGVA12.GENERA_ASIENTO = "S";
                        cabeceraGVA12.FECHA_INGRESO = new DateTime(1800, 1, 1);
                        cabeceraGVA12.HORA_INGRESO = "";
                        cabeceraGVA12.USUARIO_INGRESO = "Sistema";
                        cabeceraGVA12.TERMINAL_INGRESO = "";
                        cabeceraGVA12.FECHA_ULTIMA_MODIFICACION = new DateTime(1800, 1, 1);
                        cabeceraGVA12.HORA_ULTIMA_MODIFICACION = "";
                        cabeceraGVA12.USUA_ULTIMA_MODIFICACION = "";
                        cabeceraGVA12.TERM_ULTIMA_MODIFICACION = "";
                        cabeceraGVA12.ID_PUESTO_CAJA = 0;
                        cabeceraGVA12.NCOMP_IN_ORIGEN = 0;
                        cabeceraGVA12.OBS_COMERC = "";
                        cabeceraGVA12.OBSERVAC = "";
                        cabeceraGVA12.LEYENDA_1 = "Interfaz Opera";
                        cabeceraGVA12.LEYENDA_2 = "";
                        cabeceraGVA12.LEYENDA_3 = "";
                        cabeceraGVA12.LEYENDA_4 = "";
                        cabeceraGVA12.LEYENDA_5 = "";
                        cabeceraGVA12.IMP_CIGARRILLOS = 0;
                        cabeceraGVA12.POR_CIGARRILLOS = 0;
                        cabeceraGVA12.ID_MOTIVO_NOTA_CREDITO = null;
                        cabeceraGVA12.FECHA_DESCARGA_PDF = new DateTime(1800, 1, 1);
                        cabeceraGVA12.HORA_DESCARGA_PDF = new DateTime(1800, 1, 1);
                        cabeceraGVA12.USUARIO_DESCARGA_PDF = "";
                        cabeceraGVA12.ID_DIRECCION_ENTREGA = Convert.ToInt32(dbCasino.TraerDatos("DIRECCION_ENTREGA", "ID_DIRECCION_ENTREGA", true, "COD_CLIENTE", true, cabeceraGVA12.COD_CLIENT + "'  AND HABILITADO='S"));
                        cabeceraGVA12.ID_HISTORIAL_RENDICION = 0;
                        cabeceraGVA12.IMPUTACION_MODIFICADA = "";
                        cabeceraGVA12.PUBLICADO_WEB_CLIENTES = "";
                        cabeceraGVA12.RG_3572_TIPO_OPERACION_HABITUAL_VENTAS = "E";
                        cabeceraGVA12.RG_3685_TIPO_OPERACION_VENTAS = "0";
                        cabeceraGVA12.DESCRIPCION_FACTURA = "";
                        cabeceraGVA12.ID_NEXO_COBRANZAS_PAGO = 0;
                        cabeceraGVA12.TIPO_TRANSACCION_VENTA = 0;
                        cabeceraGVA12.TIPO_TRANSACCION_COMPRA = 0;
                        cabeceraGVA12.COMPROBANTE_CREDITO = "N";



                        foreach (var detalle in ListaDetalleArchivo.Where(f => f.TipoComprobante.Equals(cabeceraGVA12.T_COMP) && f.NumeroComprobante.Equals(cabeceraGVA12.N_COMP)).ToList())
                        {
                            Entidades.DetallesGVA53 detalleGVA53 = new Entidades.DetallesGVA53();

                            detalleGVA53.FILLER = "";
                            detalleGVA53.CANC_CRE = 0;
                            detalleGVA53.CANTIDAD = Convert.ToDouble(detalle.Cantidad);
                            detalleGVA53.CAN_EQUI_V = 1;
                            detalleGVA53.CENT_STK = "N";
                            detalleGVA53.COD_ARTICU = detalle.CodigoArticulo;
                            detalleGVA53.COD_DEPOSI = "";
                            detalleGVA53.FALTAN_REM = 1;
                            detalleGVA53.FECHA_MOV = DateTime.Parse(archivoCabecera.FechaEmision);
                            detalleGVA53.IMP_NETO_P = Convert.ToDouble(detalleGVA53.CANTIDAD) * Convert.ToDouble(detalle.Precio);
                            detalleGVA53.IMP_RE_PAN = 0;
                            detalleGVA53.N_COMP = cabeceraGVA12.N_COMP;
                            detalleGVA53.N_PARTIDA = "";
                            detalleGVA53.N_RENGL_V = 1;
                            detalleGVA53.PORC_DTO = 0;
                            detalleGVA53.PORC_IVA = 0;
                            detalleGVA53.PPP_EX = 0;
                            detalleGVA53.PPP_LO = 0;
                            detalleGVA53.PRECIO_NET = Convert.ToDouble(detalle.Precio);
                            detalleGVA53.PRECIO_PAN = Convert.ToDouble(detalle.Precio);
                            detalleGVA53.PREC_ULC_E = 0;
                            detalleGVA53.PREC_ULC_L = 0;
                            detalleGVA53.PROMOCION = false;
                            detalleGVA53.T_COMP = cabeceraGVA12.T_COMP;
                            detalleGVA53.TCOMP_IN_V = cabeceraGVA12.TCOMP_IN_V;
                            detalleGVA53.COD_CLASIF = "";
                            detalleGVA53.IM_NET_P_E = 0;
                            detalleGVA53.IM_RE_PA_E = 0;
                            detalleGVA53.PREC_NET_E = 0;
                            detalleGVA53.PREC_PAN_E = 0;
                            detalleGVA53.PR_ULC_E_E = 0;
                            detalleGVA53.PR_ULC_L_E = 0;
                            detalleGVA53.PRECSINDTO = 0;
                            detalleGVA53.IMPORTE_EXENTO = Convert.ToDouble(detalle.Precio);
                            detalleGVA53.IMPORTE_GRAVADO = 0;
                            detalleGVA53.CANTIDAD_2 = 0;
                            detalleGVA53.FALTAN_REM_2 = 0;
                            detalleGVA53.ID_MEDIDA_VENTAS = Convert.ToInt32(dbCasino.TraerDatos("STA11", "ID_MEDIDA_VENTAS", true, "COD_ARTICU", true, detalleGVA53.COD_ARTICU));
                            detalleGVA53.ID_MEDIDA_STOCK_2 = Convert.ToInt32(dbCasino.TraerDatos("STA11", "ID_MEDIDA_STOCK_2", true, "COD_ARTICU", true, detalleGVA53.COD_ARTICU));
                            detalleGVA53.ID_MEDIDA_STOCK = Convert.ToInt32(dbCasino.TraerDatos("STA11", "ID_MEDIDA_STOCK", true, "COD_ARTICU", true, detalleGVA53.COD_ARTICU));
                            detalleGVA53.UNIDAD_MEDIDA_SELECCIONADA = "P";
                            detalleGVA53.RENGL_PADR = 0;
                            detalleGVA53.COD_ARTICU_KIT = "";
                            detalleGVA53.INSUMO_KIT_SEPARADO = false;
                            detalleGVA53.PRECIO_FECHA = new DateTime(1800, 1, 1);
                            detalleGVA53.PRECIO_LISTA = 0;
                            detalleGVA53.PRECIO_BONIF = 0;
                            detalleGVA53.PORC_DTO_PARAM = 0;
                            detalleGVA53.FECHA_MODIFICACION_PRECIO = new DateTime(1800, 1, 1);
                            detalleGVA53.USUARIO_MODIFICACION_PRECIO = null;
                            detalleGVA53.TERMINAL_MODIFICACION_PRECIO = null;
                            detalleGVA53.ITEM_ESPECTACULO = "";

                            cabeceraGVA12.Detalle.Add(detalleGVA53);

                        }

                        Entidades.CuotasGVA46 cuotas = new Entidades.CuotasGVA46();
                        cuotas.FILLER = "";
                        cuotas.ESTADO_VTO = cabeceraGVA12.ESTADO;
                        cuotas.FECHA_VTO = cabeceraGVA12.FECHA_EMIS;
                        cuotas.IMPORTE_VT = cabeceraGVA12.IMPORTE;
                        cuotas.N_COMP = cabeceraGVA12.N_COMP;
                        cuotas.T_COMP = cabeceraGVA12.T_COMP;
                        cuotas.ESTADO_UNI = cabeceraGVA12.ESTADO;
                        cuotas.IMP_VT_UNI = 0;
                        cuotas.IMP_VT_EXT = 0;
                        cuotas.IM_VT_UN_E = 0;
                        cuotas.ALTERNATIVA_1 = new DateTime(1800, 1, 1);
                        cuotas.IMPORTE_TOTAL_1 = 0;
                        cuotas.ALTERNATIVA_2 = new DateTime(1800, 1, 1);
                        cuotas.IMPORTE_TOTAL_2 = 0;
                        cuotas.AJUSTA_COBRO_FECHA_ALTERNATIVA = "N";
                        cuotas.UNIDADES_TOTAL_1 = 0;
                        cuotas.UNIDADES_TOTAL_2 = 0;

                        if (cuotas.T_COMP != "N/C")
                        {
                            cabeceraGVA12.Cuotas.Add(cuotas);
                        }

                        Entidades.ImpuestosGVA42 impuestos1 = new Entidades.ImpuestosGVA42();
                        impuestos1.FILLER = "";
                        impuestos1.COD_ALICUO = Convert.ToInt32(ConfigurationManager.AppSettings.Get("CodAlicuo_iva"));
                        impuestos1.IMPORTE = Convert.ToDouble(archivoCabecera.ImpTotalIva21);
                        impuestos1.N_COMP = cuotas.N_COMP;
                        impuestos1.NETO_GRAV = cabeceraGVA12.IMPORTE_GR;
                        impuestos1.PERCEP = 0;
                        impuestos1.PORCENTAJE = Convert.ToDouble(dbCasino.TraerDatos("GVA41", "PORCENTAJE", true, "COD_ALICUO", true, impuestos1.COD_ALICUO.ToString()));
                        impuestos1.T_COMP = cuotas.T_COMP;
                        impuestos1.COD_IMPUES = "";
                        impuestos1.COD_SII = "";
                        impuestos1.IMP_EXT = 0;
                        impuestos1.NE_GRAV_EX = 0;
                        impuestos1.PERCEP_EXT = 0;

                        if (impuestos1.IMPORTE != 0)
                        {
                            cabeceraGVA12.Impuesto.Add(impuestos1);
                        }

                        Entidades.EncabezadoMovimientosSBA04 encabezadoMov = new Entidades.EncabezadoMovimientosSBA04();
                        encabezadoMov.FILLER = "";
                        encabezadoMov.BARRA = 0;
                        encabezadoMov.CERRADO = true;
                        encabezadoMov.CLASE = 0;
                        encabezadoMov.COD_COMP = cabeceraGVA12.T_COMP;
                        encabezadoMov.CONCEPTO = "Sistema Opera";
                        encabezadoMov.COTIZACION = 1;
                        encabezadoMov.EXPORTADO = true;
                        encabezadoMov.EXTERNO = true;
                        encabezadoMov.FECHA = cabeceraGVA12.FECHA_EMIS;
                        encabezadoMov.FECHA_ING = cabeceraGVA12.FECHA_INGRESO;
                        encabezadoMov.HORA_ING = "";
                        encabezadoMov.N_COMP = cabeceraGVA12.N_COMP;
                        encabezadoMov.N_INTERNO = Convert.ToInt32(dbCasino.TraerInterno()); 
                        encabezadoMov.PASE = true;
                        encabezadoMov.SITUACION = "N";
                        encabezadoMov.TERMINAL = "Interfaz";
                        encabezadoMov.USUARIO = "";
                        encabezadoMov.LOTE = 0;
                        encabezadoMov.LOTE_ANU = 0;
                        encabezadoMov.SUCUR_ORI = 0;
                        encabezadoMov.FECHA_ORI = new DateTime(1800, 1, 1);
                        encabezadoMov.C_COMP_ORI = "";
                        encabezadoMov.N_COMP_ORI = "";
                        encabezadoMov.BARRA_ORI = 0;
                        encabezadoMov.FECHA_EMIS = cabeceraGVA12.FECHA_EMIS;
                        encabezadoMov.GENERA_ASIENTO = "S";
                        encabezadoMov.FECHA_ULTIMA_MODIFICACION = cabeceraGVA12.FECHA_ULTIMA_MODIFICACION;
                        encabezadoMov.HORA_ULTIMA_MODIFICACION = "";
                        encabezadoMov.USUA_ULTIMA_MODIFICACION = "";
                        encabezadoMov.TERM_ULTIMA_MODIFICACION = "";
                        encabezadoMov.ID_PUESTO_CAJA = Convert.ToInt32(DBNull.Value);
                        encabezadoMov.ID_GVA81 = Convert.ToInt32(DBNull.Value);
                        encabezadoMov.ID_SBA02 = Convert.ToInt32(dbCasino.TraerDatos("SBA02 ", "ID_SBA02", true, "COD_COMP", true, cabeceraGVA12.T_COMP));
                        encabezadoMov.ID_SBA02_C_COMP_ORI = Convert.ToInt32(DBNull.Value);
                        encabezadoMov.COD_GVA14 = "";
                        encabezadoMov.COD_CPA01 = "";
                        encabezadoMov.ID_CODIGO_RELACION = 0;
                        encabezadoMov.ID_LEGAJO = 0;
                        encabezadoMov.OBSERVACIONES = archivoCabecera.Observaciones;
                        encabezadoMov.TIPO_COD_RELACIONADO = "";
                        encabezadoMov.CN_ASTOR = "";
                        encabezadoMov.ID_MODELO_INGRESO_SB = 0;
                        encabezadoMov.TOTAL_IMPORTE_CTE = 0;
                        encabezadoMov.TOTAL_IMPORTE_EXT = 0;
                        encabezadoMov.TRANSFERENCIA_DEVOLUCION_CUPONES = "";


                        foreach (var ArchPago in ListaPagosArchivo.Where(f => f.TipoComprobante.Equals(cabeceraGVA12.T_COMP) && f.NumeroComprobante.Equals(cabeceraGVA12.N_COMP)).ToList())
                        {

                            FormasDePago cod_cuenta = ListaFormaPagoJson.Where(p => p.TipoPago.Equals(ArchPago.FormaPago)).FirstOrDefault();
                            // traer dato si existe en sba01.cod_cta
                            Entidades.DetalleMovimientoSBA05 DetalleMov = new Entidades.DetalleMovimientoSBA05();
                            DetalleMov.FILLER = "";
                            DetalleMov.BARRA = 0;
                            DetalleMov.CANT_MONE = Convert.ToDouble(ArchPago.ImpMonedaArg);
                            DetalleMov.CHEQUES = 0;
                            DetalleMov.CLASE = 0;
                            DetalleMov.COD_COMP = cabeceraGVA12.T_COMP;
                            DetalleMov.COD_CTA = Convert.ToDouble(cod_cuenta.CodCuentaTango);
                            DetalleMov.COD_OPERAC = "";
                            DetalleMov.CONCILIADO = false;
                            DetalleMov.COTIZ_MONE = 0;
                            if (DetalleMov.COD_COMP.Equals("FAC"))
                            {
                                DetalleMov.D_H = "D";
                            }
                            else
                            {
                                DetalleMov.D_H = "H";
                            }
                            DetalleMov.EFECTIVO = 0;
                            DetalleMov.FECHA = new DateTime(1800, 1, 1);
                            DetalleMov.FECHA_CONC = new DateTime(1800, 1, 1);
                            DetalleMov.LEYENDA = "";
                            DetalleMov.MONTO = DetalleMov.CANT_MONE;
                            DetalleMov.N_COMP = cabeceraGVA12.N_COMP;
                            DetalleMov.RENGLON = NumReglon;
                            DetalleMov.UNIDADES = 0;
                            DetalleMov.VA_DIRECTO = "";
                            DetalleMov.ID_SBA02 = encabezadoMov.ID_SBA02;
                            DetalleMov.ID_GVA81 = 0;
                            DetalleMov.CONC_EFTV = true;
                            DetalleMov.F_CONC_EFT = new DateTime(1800, 1, 1);
                            DetalleMov.COMENTARIO = "";
                            DetalleMov.COMENTARIO_EFT = "";
                            DetalleMov.COD_GVA14 = "";
                            DetalleMov.COD_CPA01 = "";
                            DetalleMov.ID_CODIGO_RELACION = 0;
                            DetalleMov.ID_LEGAJO = 0;
                            DetalleMov.TIPO_COD_RELACIONADO = "";
                            DetalleMov.ID_TIPO_COTIZACION = 0;
                            DetalleMov.ID_SBA11 = 0;

                            encabezadoMov.MovDetalle.Add(DetalleMov);

                            if (ArchPago.NumeroCuponTarjeta != "")
                            {
                                Entidades.CuponesSBA20 cupones = new Entidades.CuponesSBA20();
                                cupones.FILLER = "";
                                cupones.BARRA_REC = 0;
                                cupones.CANT_CUOTA = ArchPago.CantidadCuotas;
                                cupones.COD_CTA = DetalleMov.COD_CTA;
                                cupones.COTIZ = 1;
                                cupones.CUOTA = 1;
                                cupones.ESTADO = "C";
                                cupones.EXPORTADO = true;
                                cupones.F_VTO_TARJ = Convert.ToString(ArchPago.FechaVencimiento);
                                cupones.FECHA_CUPO = DateTime.Parse(ArchPago.FechaPago);
                                cupones.FECHA_DEP = new DateTime(1800, 1, 1);
                                cupones.FECHA_REC = DateTime.Parse(ArchPago.FechaPago);
                                cupones.IMPORTE_TO = Convert.ToDouble(ArchPago.ImpMonedaArg);
                                cupones.MONEDA_EX = false;
                                cupones.N_AUTORIZA = ArchPago.NumeroAutorizacion;
                                cupones.N_COMP_DEP = "";
                                cupones.N_COMP_REC = "";
                                cupones.N_CUPON = Convert.ToDouble(ArchPago.NumeroCuponTarjeta);
                                cupones.N_DOC = ArchPago.NumeroDocTarj;
                                cupones.N_SOCIO = "";
                                cupones.NOMBRE_SOC = ArchPago.NombreSocioTarj;
                                cupones.T_COMP_DEP = "";
                                cupones.T_COMP_REC = "";
                                cupones.T_DOC = "";
                                cupones.TELEFONO = "";
                                cupones.FECHA_SAL = new DateTime(1800, 1, 1);
                                cupones.T_COMP_SAL = encabezadoMov.COD_COMP;
                                cupones.N_COMP_SAL = encabezadoMov.N_COMP;
                                cupones.BARRA_SAL = 0;
                                cupones.NRO_SUCURS = 0;
                                cupones.TERM_ID = "";
                                cupones.LOTE = ArchPago.Lote;
                                cupones.HORA_REC = "";
                                cupones.TERM_ID_2 = "";
                                cupones.ID_HOST = 0;
                                cupones.ID_SBA22 = 0;
                                cupones.ID_PLAN_TARJETA = 0;
                                cupones.COEF_VENTA = 0;
                                cupones.COEF_ACRED = 0;
                                cupones.PORC_DESC = 0;
                                cupones.COMISION = 0;
                                cupones.ID_PROMOCION_TARJETA = 0;
                                cupones.RENGLON_REC = 0;
                                cupones.NETO_TOT = 0;
                                cupones.ORIGEN = "";
                                cupones.TIPO_CUPON = "";
                                cupones.ID_SUCURSAL = 0;
                                cupones.ID_TERMINAL_POS = 0;
                                cupones.BARRA_DEP = 0;
                                cupones.OBSERVACIONES = "";
                                cupones.RENGLON_DEP = 0;
                                cupones.RENGLON_SAL = 0;
                                cupones.VERSION_ANT = false;
                                cupones.ID_SBA02_REC = 0;
                                cupones.ID_SBA02_DEP = 0;
                                cupones.ID_SBA02_SAL = 0;

                                encabezadoMov.Cupones.Add(cupones);
                            }
                            NumReglon++;
                        }

                        Entidades.DetalleMovimientoSBA05 DetalleMovContraCuenta = new Entidades.DetalleMovimientoSBA05();
                        DetalleMovContraCuenta.FILLER = "";
                        DetalleMovContraCuenta.BARRA = 0;
                        DetalleMovContraCuenta.CANT_MONE = cabeceraGVA12.IMPORTE;
                        DetalleMovContraCuenta.CHEQUES = 0;
                        DetalleMovContraCuenta.CLASE = 0;
                        DetalleMovContraCuenta.COD_COMP = cabeceraGVA12.T_COMP;
                        if (DetalleMovContraCuenta.COD_COMP == "FAC")
                        {
                            DetalleMovContraCuenta.COD_CTA = Convert.ToInt32(ConfigurationManager.AppSettings.Get("cod_contracuenta_FAC"));
                        }
                        else
                        {
                            DetalleMovContraCuenta.COD_CTA = Convert.ToInt32(ConfigurationManager.AppSettings.Get("cod_contracuenta_NC"));
                        }
                        DetalleMovContraCuenta.COD_OPERAC = "";
                        DetalleMovContraCuenta.CONCILIADO = false;
                        DetalleMovContraCuenta.COTIZ_MONE = 0;
                        if (cabeceraGVA12.T_COMP.Equals("FAC"))
                        {
                            DetalleMovContraCuenta.D_H = "H";
                        }
                        else
                        {
                            DetalleMovContraCuenta.D_H = "D";
                        }
                        DetalleMovContraCuenta.EFECTIVO = 0;
                        DetalleMovContraCuenta.FECHA = new DateTime(1800, 1, 1);
                        DetalleMovContraCuenta.FECHA_CONC = new DateTime(1800, 1, 1);
                        DetalleMovContraCuenta.LEYENDA = "";
                        DetalleMovContraCuenta.MONTO = DetalleMovContraCuenta.CANT_MONE;
                        DetalleMovContraCuenta.N_COMP = cabeceraGVA12.N_COMP;
                        DetalleMovContraCuenta.RENGLON = NumReglon + 1;
                        DetalleMovContraCuenta.UNIDADES = 0;
                        DetalleMovContraCuenta.VA_DIRECTO = "";
                        DetalleMovContraCuenta.ID_SBA02 = encabezadoMov.ID_SBA02;
                        DetalleMovContraCuenta.ID_GVA81 = 0;
                        DetalleMovContraCuenta.CONC_EFTV = true;
                        DetalleMovContraCuenta.F_CONC_EFT = new DateTime(1800, 1, 1);
                        DetalleMovContraCuenta.COMENTARIO = "";
                        DetalleMovContraCuenta.COMENTARIO_EFT = "";
                        DetalleMovContraCuenta.COD_GVA14 = "";
                        DetalleMovContraCuenta.COD_CPA01 = "";
                        DetalleMovContraCuenta.ID_CODIGO_RELACION = 0;
                        DetalleMovContraCuenta.ID_LEGAJO = 0;
                        DetalleMovContraCuenta.TIPO_COD_RELACIONADO = "";
                        DetalleMovContraCuenta.ID_TIPO_COTIZACION = 0;
                        DetalleMovContraCuenta.ID_SBA11 = 0;

                        encabezadoMov.MovDetalle.Add(DetalleMovContraCuenta);
                        
                        cabeceraGVA12.EncabezadoMov = encabezadoMov;

                        ListaCabeceraGVA12.Add(cabeceraGVA12);
                    }
                }
                if (ListaCabeceraGVA12.Count > 0)
                {
                    foreach (var item in ListaCabeceraGVA12)
                    {
                        GuardoConExito = dbCasino.GuardarEnBaseDeDatos(item);
                        
                    }
                    dbCasino.Reset_ID();
                }
                return GuardoConExito;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
