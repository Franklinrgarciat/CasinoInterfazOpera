using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;
using Entidades;

namespace Datos
{
    public class GuardarDB
    {
        private static string conexion = ConfigurationManager.AppSettings.Get("Conexion").ToString();

        public bool ValidarComprobante(string tipo, string n_comp)
        {
            SqlConnection cn = new SqlConnection(conexion);
            SqlCommand cm = new SqlCommand();
            bool Existe = false;
            try
            {
                using (cn)
                {
                    cn.Open();
                    cm.Connection = cn;
                    cm.CommandText = "SELECT * FROM GVA12 WHERE T_COMP='" + tipo + "'AND N_COMP='" + n_comp + "'";
                    cm.ExecuteNonQuery();
                    SqlDataReader dr = cm.ExecuteReader();
                    if (dr.Read())
                    {
                        Existe = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Existe;
        }

        public string TraerInterno()
        {
            SqlConnection oConn = new SqlConnection(conexion);
            SqlCommand oComm = new SqlCommand();
            string proximo = "";
            try
            {
                using (oConn)
                {
                    oConn.Open();
                    oComm.Connection = oConn;

                    oComm.CommandText = "SELECT isnull(CONVERT(VARCHAR, CAST(MAX(N_INTERNO) AS INT) + 1),0) AS PROXIMO from sba04";
                    oComm.Parameters.Clear();
                    oComm.ExecuteNonQuery();
                    SqlDataReader reader = oComm.ExecuteReader();
                    if (reader.Read())
                    {
                        proximo = reader["PROXIMO"].ToString();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return proximo;
        }


        public string TraerDatos(string sTabla, string sCampoAtraer, bool bTextoCampoAtraer, string sCampoABuscar, bool bTexto, string sValorAComparar)
        {
            SqlConnection cn = new SqlConnection(conexion);

            string sSQL;
            SqlCommand cm = new SqlCommand();
            SqlDataReader dr;
            string sValor;


            try
            {
                cn.Open();
                if (bTexto == false)
                {
                    sSQL = "SELECT " + sCampoAtraer + " FROM " + sTabla + " WHERE " + sCampoABuscar + "=" + sValorAComparar;
                }
                else
                {
                    sSQL = "SELECT " + sCampoAtraer + " FROM " + sTabla + " WHERE " + sCampoABuscar + "='" + sValorAComparar + "'";
                }
                cm.Connection = cn;
                cm.CommandText = sSQL;
                dr = cm.ExecuteReader();

                if (dr.Read())
                {
                    sValor = dr[0].ToString();
                }
                else if (bTextoCampoAtraer == true)
                {
                    sValor = "";
                }
                else
                {
                    sValor = "";
                }
            }
            catch
            {
                if (bTextoCampoAtraer == true)
                    sValor = "";
                else
                    sValor = "";
            }
            if (cn.State == ConnectionState.Open)
                cn.Close();
            return sValor;
        }

        public bool GuardarEnBaseDeDatos(CabeceraGVA12 comprobante)
        {
            SqlConnection cn = new SqlConnection(conexion);
            SqlCommand cm = new SqlCommand();
            SqlTransaction tr = null;
            try
            {
                cn.Open();
                tr = cn.BeginTransaction();
                cm = GuardarCabecera(comprobante);
                cm.Connection = cn;
                cm.Transaction = tr;
                cm.ExecuteNonQuery();

                foreach (DetallesGVA53 dt in comprobante.Detalle)
                {
                    cm = GuardarDetalle(dt);
                    cm.Connection = cn;
                    cm.Transaction = tr;
                    cm.ExecuteNonQuery();
                }
                foreach (CuotasGVA46 cuota in comprobante.Cuotas)
                {
                    cm = GuardarCuotas(cuota);
                    cm.Connection = cn;
                    cm.Transaction = tr;
                    cm.ExecuteNonQuery();
                }
                foreach (ImpuestosGVA42 iva in comprobante.Impuesto)
                {
                    cm = GuardarImpuestos(iva);
                    cm.Connection = cn;
                    cm.Transaction = tr;
                    cm.ExecuteNonQuery();
                }

                cm = GuardarEncabezadoMovimientos(comprobante.EncabezadoMov);
                cm.Connection = cn;
                cm.Transaction = tr;
                cm.ExecuteNonQuery();

                foreach (DetalleMovimientoSBA05 movimiento in comprobante.EncabezadoMov.MovDetalle)
                {
                    cm = GuardarMovimientos(movimiento);
                    cm.Connection = cn;
                    cm.Transaction = tr;
                    cm.ExecuteNonQuery();
                }
                foreach (CuponesSBA20 cupon in comprobante.EncabezadoMov.Cupones)
                {
                    if (comprobante.EncabezadoMov.Cupones.Count > 0)
                    {
                        cm = GuardarCupones(cupon);
                        cm.Connection = cn;
                        cm.Transaction = tr;
                        cm.ExecuteNonQuery();
                    }
                    
                }
                cm = InsertarAsientoComprobante();
                cm.Connection = cn;
                cm.Transaction = tr;
                cm.ExecuteNonQuery();

                cm = GuardarCliente(comprobante.Cliente);
                cm.Connection = cn;
                cm.Transaction = tr;
                cm.ExecuteNonQuery();

                cm = GuardarDireccionEntrega(comprobante.Cliente.DireccionEntrega);
                cm.Connection = cn;
                cm.Transaction = tr;
                cm.ExecuteNonQuery();

                tr.Commit();
                return true;
            }
            catch (Exception ex)
            {
                tr.Rollback();
                return false;
                throw ex;
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
            }
        }


        public SqlCommand GuardarCabecera(Entidades.CabeceraGVA12 cabeceraGVA12)
        {
            try
            {
                SqlCommand cm = new SqlCommand();

                cm.CommandText =
                    @" 
                        DECLARE @NCOMP_IN_V INT = (SELECT ISNULL(MAX(NCOMP_IN_V), 0) FROM GVA12) + 1;
                        INSERT INTO GVA12
                        (
                        FILLER,	
                        AFEC_STK,	
                        CANC_COMP,	
                        CANT_HOJAS,	
                        CAT_IVA,
                        CENT_STK,	
                        CENT_COB,	
                        CITI_OPERA,	
                        CITI_TIPO,	
                        COD_CAJA,
                        COD_CLIENT,	
                        COD_SUCURS,	
                        COD_TRANSP,	
                        COD_VENDED,	
                        COND_VTA,	
                        CONTABILIZ,	
                        CONTFISCAL,	
                        COTIZ,	
                        DESC_PANT,	
                        ESTADO,	
                        ESTADO_STK,	
                        EXPORTADO,	
                        FECHA_ANU,	
                        FECHA_EMIS,	
                        ID_CIERRE,	
                        IMPORTE,	
                        IMPORTE_BO,	
                        IMPORTE_EX,
                        IMPORTE_FL,
                        IMPORTE_GR,
                        IMPORTE_IN,	
                        IMPORTE_IV,	
                        IMP_TICK_N,	
                        IMP_TICK_P,	
                        LEYENDA,	
                        LOTE,	
                        MON_CTE,	
                        MOTI_ANU,	
                        NRO_DE_LIS,	
                        NRO_SUCURS,	
                        N_COMP,
                        ORIGEN,
                        PORC_BONIF,	
                        PORC_PRO,	
                        PORC_REC,	
                        PORC_TICK,	
                        PROPINA,	
                        PROPINA_EX,	
                        PTO_VTA,
                        REC_PANT,	
                        TALONARIO,	
                        TCOMP_IN_V,	
                        TICKET,	
                        TIPO_ASIEN,	
                        TIPO_EXPOR,	
                        TIPO_VEND,	
                        T_COMP,	
                        T_FORM,	
                        UNIDADES,	
                        LOTE_ANU,	
                        PORC_INT,	
                        PORC_FLE,	
                        ESTADO_UNI,	
                        ID_CFISCAL,	
                        NUMERO_Z,	
                        HORA_COMP,	
                        SENIA,	
                        ID_TURNO,	
                        ID_TURNOX,	
                        HORA_ANU,	
                        CCONTROL,	
                        ID_A_RENTA,	
                        COD_CLASIF,	
                        AFEC_CIERR,	
                        CAICAE,	
                        CAICAE_VTO,	
                        DOC_ELECTR,	
                        SERV_DESDE,	
                        SERV_HASTA,	
                        CANT_IMP,	
                        CANT_MAIL,	
                        ULT_IMP,	
                        ULT_MAIL,	
                        MORA_SOBRE,	
                        ESTADO_ANT,	
                        T_DOC_DTE,	
                        DTE_ANU,	
                        FOLIO_ANU,	
                        REBAJA_DEB,	
                        SUCURS_SII,	
                        EXENTA,
                        MOTIVO_DTE,
                        IMPOR_EXT,	
                        CERRADO,	
                        IMP_BO_EXT,	
                        IMP_EX_EXT,	
                        IMP_FL_EXT,	
                        IMP_GR_EXT,	
                        IMP_IN_EXT,	
                        IMP_IV_EXT,
                        IM_TIC_N_E,	
                        IM_TIC_P_E,	
                        UNIDAD_EXT,	
                        PROPIN_EXT,	
                        PRO_EX_EXT,	
                        REC_PAN_EX,	
                        DES_PAN_EX,	
                        T_DTO_COMP,	
                        RECARGO_PV,	
                        NCOMP_IN_V,	
                        ID_ASIENTO_MODELO_GV,	
                        GENERA_ASIENTO,	
                        FECHA_INGRESO,	
                        HORA_INGRESO,
                        USUARIO_INGRESO,
                        TERMINAL_INGRESO,
                        FECHA_ULTIMA_MODIFICACION,	
                        HORA_ULTIMA_MODIFICACION,
                        USUA_ULTIMA_MODIFICACION,
                        TERM_ULTIMA_MODIFICACION,
                        ID_PUESTO_CAJA,
                        NCOMP_IN_ORIGEN,
                        OBS_COMERC,
                        OBSERVAC,
                        LEYENDA_1,
                        LEYENDA_2,	
                        LEYENDA_3,
                        LEYENDA_4,
                        LEYENDA_5,
                        IMP_CIGARRILLOS,	
                        POR_CIGARRILLOS,	
                        ID_MOTIVO_NOTA_CREDITO,
                        FECHA_DESCARGA_PDF,	
                        HORA_DESCARGA_PDF,
                        USUARIO_DESCARGA_PDF,
                        ID_DIRECCION_ENTREGA,
                        ID_HISTORIAL_RENDICION,
                        IMPUTACION_MODIFICADA,
                        PUBLICADO_WEB_CLIENTES,
                        RG_3572_TIPO_OPERACION_HABITUAL_VENTAS,
                        RG_3685_TIPO_OPERACION_VENTAS,
                        DESCRIPCION_FACTURA,
                        ID_NEXO_COBRANZAS_PAGO,
                        TIPO_TRANSACCION_VENTA,
                        TIPO_TRANSACCION_COMPRA,
                        COMPROBANTE_CREDITO
                        )
                        VALUES
                        (
                        @FILLER,	
                        @AFEC_STK,	
                        @CANC_COMP,	
                        @CANT_HOJAS,	
                        @CAT_IVA,	
                        @CENT_STK,	
                        @CENT_COB,	
                        @CITI_OPERA,	
                        @CITI_TIPO,	
                        @COD_CAJA,
                        @COD_CLIENT,	
                        @COD_SUCURS,	
                        @COD_TRANSP,	
                        @COD_VENDED,	
                        @COND_VTA,	
                        @CONTABILIZ,	
                        @CONTFISCAL,	
                        @COTIZ,	
                        @DESC_PANT,	
                        @ESTADO,	
                        @ESTADO_STK,	
                        @EXPORTADO,	
                        @FECHA_ANU,	
                        @FECHA_EMIS,	
                        @ID_CIERRE,	
                        @IMPORTE,	
                        @IMPORTE_BO,	
                        @IMPORTE_EX,
                        @IMPORTE_FL,	
                        @IMPORTE_GR,	
                        @IMPORTE_IN,	
                        @IMPORTE_IV,	
                        @IMP_TICK_N,	
                        @IMP_TICK_P,	
                        @LEYENDA,	
                        @LOTE,	
                        @MON_CTE,	
                        @MOTI_ANU,	
                        @NRO_DE_LIS,	
                        @NRO_SUCURS,	
                        @N_COMP,
                        @ORIGEN,	
                        @PORC_BONIF,	
                        @PORC_PRO,	
                        @PORC_REC,	
                        @PORC_TICK,	
                        @PROPINA,	
                        @PROPINA_EX,	
                        @PTO_VTA,
                        @REC_PANT,	
                        @TALONARIO,	
                        @TCOMP_IN_V,	
                        @TICKET,	
                        @TIPO_ASIEN,	
                        @TIPO_EXPOR,	
                        @TIPO_VEND,	
                        @T_COMP,	
                        @T_FORM,	
                        @UNIDADES,	
                        @LOTE_ANU,	
                        @PORC_INT,	
                        @PORC_FLE,	
                        @ESTADO_UNI,	
                        @ID_CFISCAL,	
                        @NUMERO_Z,	
                        @HORA_COMP,	
                        @SENIA,	
                        @ID_TURNO,	
                        @ID_TURNOX,
                        @HORA_ANU,	
                        @CCONTROL,	
                        @ID_A_RENTA,	
                        @COD_CLASIF,	
                        @AFEC_CIERR,	
                        @CAICAE,	
                        @CAICAE_VTO,	
                        @DOC_ELECTR,	
                        @SERV_DESDE,	
                        @SERV_HASTA,	
                        @CANT_IMP,	
                        @CANT_MAIL,	
                        @ULT_IMP,	
                        @ULT_MAIL,	
                        @MORA_SOBRE,	
                        @ESTADO_ANT,	
                        @T_DOC_DTE,	
                        @DTE_ANU,	
                        @FOLIO_ANU,	
                        @REBAJA_DEB,	
                        @SUCURS_SII,	
                        @EXENTA,
                        @MOTIVO_DTE,
                        @IMPOR_EXT,	
                        @CERRADO,	
                        @IMP_BO_EXT,	
                        @IMP_EX_EXT,
                        @IMP_FL_EXT,	
                        @IMP_GR_EXT,	
                        @IMP_IN_EXT,	
                        @IMP_IV_EXT,
                        @IM_TIC_N_E,	
                        @IM_TIC_P_E,	
                        @UNIDAD_EXT,	
                        @PROPIN_EXT,	
                        @PRO_EX_EXT,	
                        @REC_PAN_EX,	
                        @DES_PAN_EX,	
                        @T_DTO_COMP,	
                        @RECARGO_PV,
                        @NCOMP_IN_V,	
                        @ID_ASIENTO_MODELO_GV,	
                        @GENERA_ASIENTO,	
                        @FECHA_INGRESO,	
                        @HORA_INGRESO,
                        @USUARIO_INGRESO,
                        @TERMINAL_INGRESO,
                        @FECHA_ULTIMA_MODIFICACION,	
                        @HORA_ULTIMA_MODIFICACION,
                        @USUA_ULTIMA_MODIFICACION,
                        @TERM_ULTIMA_MODIFICACION,
                        @ID_PUESTO_CAJA,
                        @NCOMP_IN_ORIGEN,
                        @OBS_COMERC,
                        @OBSERVAC,
                        @LEYENDA_1,
                        @LEYENDA_2,
                        @LEYENDA_3,
                        @LEYENDA_4,
                        @LEYENDA_5,
                        @IMP_CIGARRILLOS,	
                        @POR_CIGARRILLOS,	
                        @ID_MOTIVO_NOTA_CREDITO,
                        @FECHA_DESCARGA_PDF,	
                        @HORA_DESCARGA_PDF,
                        @USUARIO_DESCARGA_PDF,
                        @ID_DIRECCION_ENTREGA,
                        @ID_HISTORIAL_RENDICION,
                        @IMPUTACION_MODIFICADA,
                        @PUBLICADO_WEB_CLIENTES,
                        @RG_3572_TIPO_OPERACION_HABITUAL_VENTAS,
                        @RG_3685_TIPO_OPERACION_VENTAS,
                        @DESCRIPCION_FACTURA,
                        @ID_NEXO_COBRANZAS_PAGO,
                        @TIPO_TRANSACCION_VENTA,
                        @TIPO_TRANSACCION_COMPRA,
                        @COMPROBANTE_CREDITO
                        )
                    ";
                cm.Parameters.Clear();
                cm.Parameters.Add("@FILLER", SqlDbType.VarChar).Value = cabeceraGVA12.FILLER;
                cm.Parameters.Add("@AFEC_STK", SqlDbType.Bit).Value = cabeceraGVA12.AFEC_STK;
                cm.Parameters.Add("@CANC_COMP", SqlDbType.VarChar).Value = cabeceraGVA12.CANC_COMP;
                cm.Parameters.Add("@CANT_HOJAS", SqlDbType.Int).Value = cabeceraGVA12.CANT_HOJAS;
                cm.Parameters.Add("@CAT_IVA", SqlDbType.VarChar).Value = cabeceraGVA12.CAT_IVA;
                cm.Parameters.Add("@CENT_STK", SqlDbType.VarChar).Value = cabeceraGVA12.CENT_STK;
                cm.Parameters.Add("@CENT_COB", SqlDbType.VarChar).Value = cabeceraGVA12.CENT_COB;
                cm.Parameters.Add("@CITI_OPERA", SqlDbType.VarChar).Value = cabeceraGVA12.CITI_OPERA;
                cm.Parameters.Add("@CITI_TIPO", SqlDbType.VarChar).Value = cabeceraGVA12.CITI_TIPO;
                cm.Parameters.Add("@COD_CAJA", SqlDbType.Int).Value = cabeceraGVA12.COD_CAJA;
                cm.Parameters.Add("@COD_CLIENT", SqlDbType.VarChar).Value = cabeceraGVA12.COD_CLIENT;
                cm.Parameters.Add("@COD_SUCURS", SqlDbType.VarChar).Value = cabeceraGVA12.COD_SUCURS;
                cm.Parameters.Add("@COD_TRANSP", SqlDbType.VarChar).Value = cabeceraGVA12.COD_TRANSP;
                cm.Parameters.Add("@COD_VENDED", SqlDbType.VarChar).Value = cabeceraGVA12.COD_VENDED;
                cm.Parameters.Add("@COND_VTA", SqlDbType.Int).Value = cabeceraGVA12.COND_VTA;
                cm.Parameters.Add("@CONTABILIZ", SqlDbType.Bit).Value = cabeceraGVA12.CONTABILIZ;
                cm.Parameters.Add("@CONTFISCAL", SqlDbType.Bit).Value = cabeceraGVA12.CONTFISCAL;
                cm.Parameters.Add("@COTIZ", SqlDbType.Decimal).Value = cabeceraGVA12.COTIZ;
                cm.Parameters.Add("@DESC_PANT", SqlDbType.Decimal).Value = cabeceraGVA12.DESC_PANT;
                cm.Parameters.Add("@ESTADO", SqlDbType.VarChar).Value = cabeceraGVA12.ESTADO;
                cm.Parameters.Add("@ESTADO_STK", SqlDbType.VarChar).Value = cabeceraGVA12.ESTADO_STK;
                cm.Parameters.Add("@EXPORTADO", SqlDbType.Bit).Value = cabeceraGVA12.EXPORTADO;
                cm.Parameters.Add("@FECHA_ANU", SqlDbType.DateTime).Value = cabeceraGVA12.FECHA_ANU;
                cm.Parameters.Add("@FECHA_EMIS", SqlDbType.DateTime).Value = cabeceraGVA12.FECHA_EMIS;
                cm.Parameters.Add("@ID_CIERRE", SqlDbType.Float).Value = cabeceraGVA12.ID_CIERRE;
                cm.Parameters.Add("@IMPORTE", SqlDbType.Decimal).Value = cabeceraGVA12.IMPORTE;
                cm.Parameters.Add("@IMPORTE_BO", SqlDbType.Decimal).Value = cabeceraGVA12.IMPORTE_BO;
                cm.Parameters.Add("@IMPORTE_EX", SqlDbType.Decimal).Value = cabeceraGVA12.IMPORTE_EX;
                cm.Parameters.Add("@IMPORTE_FL", SqlDbType.Decimal).Value = cabeceraGVA12.IMPORTE_FL;
                cm.Parameters.Add("@IMPORTE_GR", SqlDbType.Decimal).Value = cabeceraGVA12.IMPORTE_GR;
                cm.Parameters.Add("@IMPORTE_IN", SqlDbType.Decimal).Value = cabeceraGVA12.IMPORTE_IN;
                cm.Parameters.Add("@IMPORTE_IV", SqlDbType.Decimal).Value = cabeceraGVA12.IMPORTE_IV;
                cm.Parameters.Add("@IMP_TICK_N", SqlDbType.Decimal).Value = cabeceraGVA12.IMP_TICK_N;
                cm.Parameters.Add("@IMP_TICK_P", SqlDbType.Decimal).Value = cabeceraGVA12.IMP_TICK_P;
                cm.Parameters.Add("@LEYENDA", SqlDbType.VarChar).Value = cabeceraGVA12.LEYENDA;
                cm.Parameters.Add("@LOTE", SqlDbType.Float).Value = cabeceraGVA12.LOTE;
                cm.Parameters.Add("@MON_CTE", SqlDbType.Bit).Value = cabeceraGVA12.MON_CTE;
                cm.Parameters.Add("@MOTI_ANU", SqlDbType.VarChar).Value = cabeceraGVA12.MOTI_ANU;
                cm.Parameters.Add("@NRO_DE_LIS", SqlDbType.Int).Value = cabeceraGVA12.NRO_DE_LIS;
                cm.Parameters.Add("@NRO_SUCURS", SqlDbType.Int).Value = cabeceraGVA12.NRO_SUCURS;
                cm.Parameters.Add("@N_COMP", SqlDbType.VarChar).Value = cabeceraGVA12.N_COMP;
                cm.Parameters.Add("@ORIGEN", SqlDbType.VarChar).Value = cabeceraGVA12.ORIGEN;
                cm.Parameters.Add("@PORC_BONIF", SqlDbType.Decimal).Value = cabeceraGVA12.PORC_BONIF;
                cm.Parameters.Add("@PORC_PRO", SqlDbType.Decimal).Value = cabeceraGVA12.PORC_PRO;
                cm.Parameters.Add("@PORC_REC", SqlDbType.Decimal).Value = cabeceraGVA12.PORC_REC;
                cm.Parameters.Add("@PORC_TICK", SqlDbType.Decimal).Value = cabeceraGVA12.PORC_TICK;
                cm.Parameters.Add("@PROPINA", SqlDbType.Decimal).Value = cabeceraGVA12.PROPINA;
                cm.Parameters.Add("@PROPINA_EX", SqlDbType.Decimal).Value = cabeceraGVA12.PROPINA_EX;
                cm.Parameters.Add("@PTO_VTA", SqlDbType.Bit).Value = cabeceraGVA12.PTO_VTA;
                cm.Parameters.Add("@REC_PANT", SqlDbType.Decimal).Value = cabeceraGVA12.REC_PANT;
                cm.Parameters.Add("@TALONARIO", SqlDbType.Int).Value = cabeceraGVA12.TALONARIO;
                cm.Parameters.Add("@TCOMP_IN_V", SqlDbType.VarChar).Value = cabeceraGVA12.TCOMP_IN_V;
                cm.Parameters.Add("@TICKET", SqlDbType.VarChar).Value = cabeceraGVA12.TICKET;
                cm.Parameters.Add("@TIPO_ASIEN", SqlDbType.VarChar).Value = cabeceraGVA12.TIPO_ASIEN;
                cm.Parameters.Add("@TIPO_EXPOR", SqlDbType.VarChar).Value = cabeceraGVA12.TIPO_EXPOR;
                cm.Parameters.Add("@TIPO_VEND", SqlDbType.VarChar).Value = cabeceraGVA12.TIPO_VEND;
                cm.Parameters.Add("@T_COMP", SqlDbType.VarChar).Value = cabeceraGVA12.T_COMP;
                cm.Parameters.Add("@T_FORM", SqlDbType.VarChar).Value = cabeceraGVA12.T_FORM;
                cm.Parameters.Add("@UNIDADES", SqlDbType.Decimal).Value = cabeceraGVA12.UNIDADES;
                cm.Parameters.Add("@LOTE_ANU", SqlDbType.Float).Value = cabeceraGVA12.LOTE_ANU;
                cm.Parameters.Add("@PORC_INT", SqlDbType.Decimal).Value = cabeceraGVA12.PORC_INT;
                cm.Parameters.Add("@PORC_FLE", SqlDbType.Decimal).Value = cabeceraGVA12.PORC_FLE;
                cm.Parameters.Add("@ESTADO_UNI", SqlDbType.VarChar).Value = cabeceraGVA12.ESTADO_UNI;
                cm.Parameters.Add("@ID_CFISCAL", SqlDbType.VarChar).Value = cabeceraGVA12.ID_CFISCAL;
                cm.Parameters.Add("@NUMERO_Z", SqlDbType.Float).Value = cabeceraGVA12.NUMERO_Z;
                cm.Parameters.Add("@HORA_COMP", SqlDbType.VarChar).Value = cabeceraGVA12.HORA_COMP;
                cm.Parameters.Add("@SENIA", SqlDbType.Bit).Value = cabeceraGVA12.SENIA;
                cm.Parameters.Add("@ID_TURNO", SqlDbType.Float).Value = cabeceraGVA12.ID_TURNO;
                cm.Parameters.Add("@ID_TURNOX", SqlDbType.Float).Value = cabeceraGVA12.ID_TURNOX;
                cm.Parameters.Add("@HORA_ANU", SqlDbType.VarChar).Value = cabeceraGVA12.HORA_ANU;
                cm.Parameters.Add("@CCONTROL", SqlDbType.VarChar).Value = cabeceraGVA12.CCONTROL;
                cm.Parameters.Add("@ID_A_RENTA", SqlDbType.Float).Value = cabeceraGVA12.ID_A_RENTA;
                cm.Parameters.Add("@COD_CLASIF", SqlDbType.VarChar).Value = cabeceraGVA12.COD_CLASIF;
                cm.Parameters.Add("@AFEC_CIERR", SqlDbType.VarChar).Value = cabeceraGVA12.AFEC_CIERR;
                cm.Parameters.Add("@CAICAE", SqlDbType.VarChar).Value = cabeceraGVA12.CAICAE;
                cm.Parameters.Add("@CAICAE_VTO", SqlDbType.DateTime).Value = cabeceraGVA12.CAICAE_VTO;
                cm.Parameters.Add("@DOC_ELECTR", SqlDbType.Bit).Value = cabeceraGVA12.DOC_ELECTR;
                cm.Parameters.Add("@SERV_DESDE", SqlDbType.DateTime).Value = cabeceraGVA12.SERV_DESDE;
                cm.Parameters.Add("@SERV_HASTA", SqlDbType.DateTime).Value = cabeceraGVA12.SERV_HASTA;
                cm.Parameters.Add("@CANT_IMP", SqlDbType.Int).Value = cabeceraGVA12.CANT_IMP;
                cm.Parameters.Add("@CANT_MAIL", SqlDbType.Int).Value = cabeceraGVA12.CANT_MAIL;
                cm.Parameters.Add("@ULT_IMP", SqlDbType.DateTime).Value = cabeceraGVA12.ULT_IMP;
                cm.Parameters.Add("@ULT_MAIL", SqlDbType.DateTime).Value = cabeceraGVA12.ULT_MAIL;
                cm.Parameters.Add("@MORA_SOBRE", SqlDbType.VarChar).Value = cabeceraGVA12.MORA_SOBRE;
                cm.Parameters.Add("@ESTADO_ANT", SqlDbType.VarChar).Value = cabeceraGVA12.ESTADO_ANT;
                cm.Parameters.Add("@T_DOC_DTE", SqlDbType.VarChar).Value = cabeceraGVA12.T_DOC_DTE;
                cm.Parameters.Add("@DTE_ANU", SqlDbType.VarChar).Value = cabeceraGVA12.DTE_ANU;
                cm.Parameters.Add("@FOLIO_ANU", SqlDbType.VarChar).Value = cabeceraGVA12.FOLIO_ANU;
                cm.Parameters.Add("@REBAJA_DEB", SqlDbType.Bit).Value = cabeceraGVA12.REBAJA_DEB;
                cm.Parameters.Add("@SUCURS_SII", SqlDbType.Float).Value = cabeceraGVA12.SUCURS_SII;
                cm.Parameters.Add("@EXENTA", SqlDbType.Bit).Value = cabeceraGVA12.EXENTA;
                cm.Parameters.Add("@MOTIVO_DTE", SqlDbType.Int).Value = cabeceraGVA12.MOTIVO_DTE;
                cm.Parameters.Add("@IMPOR_EXT", SqlDbType.Decimal).Value = cabeceraGVA12.IMPOR_EXT;
                cm.Parameters.Add("@CERRADO", SqlDbType.Bit).Value = cabeceraGVA12.CERRADO;
                cm.Parameters.Add("@IMP_BO_EXT", SqlDbType.Decimal).Value = cabeceraGVA12.IMP_BO_EXT;
                cm.Parameters.Add("@IMP_EX_EXT", SqlDbType.Decimal).Value = cabeceraGVA12.IMP_EX_EXT;
                cm.Parameters.Add("@IMP_FL_EXT", SqlDbType.Decimal).Value = cabeceraGVA12.IMP_FL_EXT;
                cm.Parameters.Add("@IMP_GR_EXT", SqlDbType.Decimal).Value = cabeceraGVA12.IMP_GR_EXT;
                cm.Parameters.Add("@IMP_IN_EXT", SqlDbType.Decimal).Value = cabeceraGVA12.IMP_IN_EXT;
                cm.Parameters.Add("@IMP_IV_EXT", SqlDbType.Decimal).Value = cabeceraGVA12.IMP_IV_EXT;
                cm.Parameters.Add("@IM_TIC_N_E", SqlDbType.Decimal).Value = cabeceraGVA12.IM_TIC_N_E;
                cm.Parameters.Add("@IM_TIC_P_E", SqlDbType.Decimal).Value = cabeceraGVA12.IM_TIC_P_E;
                cm.Parameters.Add("@UNIDAD_EXT", SqlDbType.Decimal).Value = cabeceraGVA12.UNIDAD_EXT;
                cm.Parameters.Add("@PROPIN_EXT", SqlDbType.Decimal).Value = cabeceraGVA12.PROPIN_EXT;
                cm.Parameters.Add("@PRO_EX_EXT", SqlDbType.Decimal).Value = cabeceraGVA12.PRO_EX_EXT;
                cm.Parameters.Add("@REC_PAN_EX", SqlDbType.Decimal).Value = cabeceraGVA12.REC_PAN_EX;
                cm.Parameters.Add("@DES_PAN_EX", SqlDbType.Decimal).Value = cabeceraGVA12.DES_PAN_EX;
                cm.Parameters.Add("@T_DTO_COMP", SqlDbType.VarChar).Value = cabeceraGVA12.T_DTO_COMP;
                cm.Parameters.Add("@RECARGO_PV", SqlDbType.VarChar).Value = cabeceraGVA12.RECARGO_PV;
                cm.Parameters.Add("@ID_ASIENTO_MODELO_GV", SqlDbType.Int).Value = cabeceraGVA12.ID_ASIENTO_MODELO_GV;
                cm.Parameters.Add("@GENERA_ASIENTO", SqlDbType.Char).Value = cabeceraGVA12.GENERA_ASIENTO;
                cm.Parameters.Add("@FECHA_INGRESO", SqlDbType.DateTime).Value = cabeceraGVA12.FECHA_INGRESO;
                cm.Parameters.Add("@HORA_INGRESO", SqlDbType.VarChar).Value = cabeceraGVA12.HORA_INGRESO;
                cm.Parameters.Add("@USUARIO_INGRESO", SqlDbType.VarChar).Value = cabeceraGVA12.USUARIO_INGRESO;
                cm.Parameters.Add("@TERMINAL_INGRESO", SqlDbType.VarChar).Value = cabeceraGVA12.TERMINAL_INGRESO;
                cm.Parameters.Add("@FECHA_ULTIMA_MODIFICACION", SqlDbType.DateTime).Value = cabeceraGVA12.FECHA_ULTIMA_MODIFICACION;
                cm.Parameters.Add("@HORA_ULTIMA_MODIFICACION", SqlDbType.VarChar).Value = cabeceraGVA12.HORA_ULTIMA_MODIFICACION;
                cm.Parameters.Add("@USUA_ULTIMA_MODIFICACION", SqlDbType.VarChar).Value = cabeceraGVA12.USUA_ULTIMA_MODIFICACION;
                cm.Parameters.Add("@TERM_ULTIMA_MODIFICACION", SqlDbType.VarChar).Value = cabeceraGVA12.TERM_ULTIMA_MODIFICACION;
                cm.Parameters.Add("@ID_PUESTO_CAJA", SqlDbType.Int).Value = cabeceraGVA12.ID_PUESTO_CAJA;
                cm.Parameters.Add("@NCOMP_IN_ORIGEN", SqlDbType.Float).Value = cabeceraGVA12.NCOMP_IN_ORIGEN;
                cm.Parameters.Add("@OBS_COMERC", SqlDbType.Text).Value = cabeceraGVA12.OBS_COMERC;
                cm.Parameters.Add("@OBSERVAC", SqlDbType.Text).Value = cabeceraGVA12.OBSERVAC;
                cm.Parameters.Add("@LEYENDA_1", SqlDbType.VarChar).Value = cabeceraGVA12.LEYENDA_1;
                cm.Parameters.Add("@LEYENDA_2", SqlDbType.VarChar).Value = cabeceraGVA12.LEYENDA_2;
                cm.Parameters.Add("@LEYENDA_3", SqlDbType.VarChar).Value = cabeceraGVA12.LEYENDA_3;
                cm.Parameters.Add("@LEYENDA_4", SqlDbType.VarChar).Value = cabeceraGVA12.LEYENDA_4;
                cm.Parameters.Add("@LEYENDA_5", SqlDbType.VarChar).Value = cabeceraGVA12.LEYENDA_5;
                cm.Parameters.Add("@IMP_CIGARRILLOS", SqlDbType.Decimal).Value = cabeceraGVA12.IMP_CIGARRILLOS;
                cm.Parameters.Add("@POR_CIGARRILLOS", SqlDbType.Decimal).Value = cabeceraGVA12.POR_CIGARRILLOS;
                cm.Parameters.Add("@ID_MOTIVO_NOTA_CREDITO", SqlDbType.Int).Value = DBNull.Value;
                cm.Parameters.Add("@FECHA_DESCARGA_PDF", SqlDbType.DateTime).Value = cabeceraGVA12.FECHA_DESCARGA_PDF;
                cm.Parameters.Add("@HORA_DESCARGA_PDF", SqlDbType.DateTime).Value = cabeceraGVA12.HORA_DESCARGA_PDF;
                cm.Parameters.Add("@USUARIO_DESCARGA_PDF", SqlDbType.VarChar).Value = cabeceraGVA12.USUARIO_DESCARGA_PDF;
                cm.Parameters.Add("@ID_DIRECCION_ENTREGA", SqlDbType.Int).Value = cabeceraGVA12.ID_DIRECCION_ENTREGA;
                cm.Parameters.Add("@ID_HISTORIAL_RENDICION", SqlDbType.Int).Value = cabeceraGVA12.ID_HISTORIAL_RENDICION;
                cm.Parameters.Add("@IMPUTACION_MODIFICADA", SqlDbType.VarChar).Value = cabeceraGVA12.IMPUTACION_MODIFICADA;
                cm.Parameters.Add("@PUBLICADO_WEB_CLIENTES", SqlDbType.VarChar).Value = cabeceraGVA12.PUBLICADO_WEB_CLIENTES;
                cm.Parameters.Add("@RG_3572_TIPO_OPERACION_HABITUAL_VENTAS", SqlDbType.VarChar).Value = DBNull.Value;
                cm.Parameters.Add("@RG_3685_TIPO_OPERACION_VENTAS", SqlDbType.VarChar).Value = cabeceraGVA12.RG_3685_TIPO_OPERACION_VENTAS;
                cm.Parameters.Add("@DESCRIPCION_FACTURA", SqlDbType.VarChar).Value = cabeceraGVA12.DESCRIPCION_FACTURA;
                cm.Parameters.Add("@ID_NEXO_COBRANZAS_PAGO", SqlDbType.Int).Value = DBNull.Value;
                cm.Parameters.Add("@TIPO_TRANSACCION_VENTA", SqlDbType.Int).Value = cabeceraGVA12.TIPO_TRANSACCION_VENTA;
                cm.Parameters.Add("@TIPO_TRANSACCION_COMPRA", SqlDbType.VarChar).Value = cabeceraGVA12.TIPO_TRANSACCION_COMPRA;
                cm.Parameters.Add("@COMPROBANTE_CREDITO", SqlDbType.Char).Value = cabeceraGVA12.COMPROBANTE_CREDITO;

                return cm;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public SqlCommand GuardarDetalle(Entidades.DetallesGVA53 detallesGVA53)
        {
            try
            {
                SqlCommand cm = new SqlCommand();

                cm.CommandText =
                    @" INSERT INTO GVA53
                        (
                        FILLER,
                        CANC_CRE,
                        CANTIDAD,
                        CAN_EQUI_V,
                        CENT_STK,
                        COD_ARTICU,
                        COD_DEPOSI,
                        FALTAN_REM,
                        FECHA_MOV,
                        IMP_NETO_P,
                        IMP_RE_PAN,
                        N_COMP,
                        N_PARTIDA,
                        N_RENGL_V,
                        PORC_DTO,
                        PORC_IVA,
                        PPP_EX,
                        PPP_LO,
                        PRECIO_NET,
                        PRECIO_PAN,
                        PREC_ULC_E,
                        PREC_ULC_L,
                        PROMOCION,
                        T_COMP,
                        TCOMP_IN_V,
                        COD_CLASIF,
                        IM_NET_P_E,
                        IM_RE_PA_E,
                        PREC_NET_E,
                        PREC_PAN_E,
                        PR_ULC_E_E,
                        PR_ULC_L_E,
                        PRECSINDTO,
                        IMPORTE_EXENTO,
                        IMPORTE_GRAVADO,	
                        CANTIDAD_2,
                        FALTAN_REM_2,
                        ID_MEDIDA_VENTAS,
                        ID_MEDIDA_STOCK_2,
                        ID_MEDIDA_STOCK,
                        UNIDAD_MEDIDA_SELECCIONADA,
                        RENGL_PADR,
                        COD_ARTICU_KIT,
                        INSUMO_KIT_SEPARADO,
                        PRECIO_FECHA,
                        PRECIO_LISTA,
                        PRECIO_BONIF,
                        PORC_DTO_PARAM,
                        FECHA_MODIFICACION_PRECIO,
                        USUARIO_MODIFICACION_PRECIO,
                        TERMINAL_MODIFICACION_PRECIO,
                        ITEM_ESPECTACULO
                        )
                        VALUES
                        (
                        @FILLER,
                        @CANC_CRE,
                        @CANTIDAD,
                        @CAN_EQUI_V,
                        @CENT_STK,
                        @COD_ARTICU,
                        @COD_DEPOSI,
                        @FALTAN_REM,
                        @FECHA_MOV,
                        @IMP_NETO_P,
                        @IMP_RE_PAN,
                        @N_COMP,
                        @N_PARTIDA,
                        @N_RENGL_V,
                        @PORC_DTO,
                        @PORC_IVA,
                        @PPP_EX,
                        @PPP_LO,
                        @PRECIO_NET,
                        @PRECIO_PAN,
                        @PREC_ULC_E,
                        @PREC_ULC_L,
                        @PROMOCION,
                        @T_COMP,
                        @TCOMP_IN_V,
                        @COD_CLASIF,
                        @IM_NET_P_E,
                        @IM_RE_PA_E,
                        @PREC_NET_E,
                        @PREC_PAN_E,
                        @PR_ULC_E_E,
                        @PR_ULC_L_E,
                        @PRECSINDTO,
                        @IMPORTE_EXENTO,
                        @IMPORTE_GRAVADO,
                        @CANTIDAD_2,	
                        @FALTAN_REM_2,
                        @ID_MEDIDA_VENTAS,
                        @ID_MEDIDA_STOCK_2,
                        @ID_MEDIDA_STOCK,
                        @UNIDAD_MEDIDA_SELECCIONADA,
                        @RENGL_PADR,
                        @COD_ARTICU_KIT,
                        @INSUMO_KIT_SEPARADO,
                        @PRECIO_FECHA,
                        @PRECIO_LISTA,
                        @PRECIO_BONIF,
                        @PORC_DTO_PARAM,
                        @FECHA_MODIFICACION_PRECIO,
                        @USUARIO_MODIFICACION_PRECIO,
                        @TERMINAL_MODIFICACION_PRECIO,
                        @ITEM_ESPECTACULO
                        )
                    ";
                cm.Parameters.Clear();
                cm.Parameters.Add("@FILLER", SqlDbType.VarChar).Value = detallesGVA53.FILLER;
                cm.Parameters.Add("@CANC_CRE", SqlDbType.Decimal).Value = detallesGVA53.CANC_CRE;
                cm.Parameters.Add("@CANTIDAD", SqlDbType.Decimal).Value = detallesGVA53.CANTIDAD;
                cm.Parameters.Add("@CAN_EQUI_V", SqlDbType.Decimal).Value = detallesGVA53.CAN_EQUI_V;
                cm.Parameters.Add("@CENT_STK", SqlDbType.VarChar).Value = detallesGVA53.CENT_STK;
                cm.Parameters.Add("@COD_ARTICU", SqlDbType.VarChar).Value = detallesGVA53.COD_ARTICU;
                cm.Parameters.Add("@COD_DEPOSI", SqlDbType.VarChar).Value = detallesGVA53.COD_DEPOSI;
                cm.Parameters.Add("@FALTAN_REM", SqlDbType.Decimal).Value = detallesGVA53.FALTAN_REM;
                cm.Parameters.Add("@FECHA_MOV", SqlDbType.DateTime).Value = detallesGVA53.FECHA_MOV;
                cm.Parameters.Add("@IMP_NETO_P", SqlDbType.Decimal).Value = detallesGVA53.IMP_NETO_P;
                cm.Parameters.Add("@IMP_RE_PAN", SqlDbType.Decimal).Value = detallesGVA53.IMP_RE_PAN;
                cm.Parameters.Add("@N_COMP", SqlDbType.VarChar).Value = detallesGVA53.N_COMP;
                cm.Parameters.Add("@N_PARTIDA", SqlDbType.VarChar).Value = detallesGVA53.N_PARTIDA;
                cm.Parameters.Add("@N_RENGL_V", SqlDbType.Int).Value = detallesGVA53.N_RENGL_V;
                cm.Parameters.Add("@PORC_DTO", SqlDbType.Decimal).Value = detallesGVA53.PORC_DTO;
                cm.Parameters.Add("@PORC_IVA", SqlDbType.Decimal).Value = detallesGVA53.PORC_IVA;
                cm.Parameters.Add("@PPP_EX", SqlDbType.Decimal).Value = detallesGVA53.PPP_EX;
                cm.Parameters.Add("@PPP_LO", SqlDbType.Decimal).Value = detallesGVA53.PPP_LO;
                cm.Parameters.Add("@PRECIO_NET", SqlDbType.Decimal).Value = detallesGVA53.PRECIO_NET;
                cm.Parameters.Add("@PRECIO_PAN", SqlDbType.Decimal).Value = detallesGVA53.PRECIO_PAN;
                cm.Parameters.Add("@PREC_ULC_E", SqlDbType.Decimal).Value = detallesGVA53.PREC_ULC_E;
                cm.Parameters.Add("@PREC_ULC_L", SqlDbType.Decimal).Value = detallesGVA53.PREC_ULC_L;
                cm.Parameters.Add("@PROMOCION", SqlDbType.Bit).Value = detallesGVA53.PROMOCION;
                cm.Parameters.Add("@T_COMP", SqlDbType.VarChar).Value = detallesGVA53.T_COMP;
                cm.Parameters.Add("@TCOMP_IN_V", SqlDbType.VarChar).Value = detallesGVA53.TCOMP_IN_V;
                cm.Parameters.Add("@COD_CLASIF", SqlDbType.VarChar).Value = detallesGVA53.COD_CLASIF;
                cm.Parameters.Add("@IM_NET_P_E", SqlDbType.Decimal).Value = detallesGVA53.IM_NET_P_E;
                cm.Parameters.Add("@IM_RE_PA_E", SqlDbType.Decimal).Value = detallesGVA53.IM_RE_PA_E;
                cm.Parameters.Add("@PREC_NET_E", SqlDbType.Decimal).Value = detallesGVA53.PREC_NET_E;
                cm.Parameters.Add("@PREC_PAN_E", SqlDbType.Decimal).Value = detallesGVA53.PREC_PAN_E;
                cm.Parameters.Add("@PR_ULC_E_E", SqlDbType.Decimal).Value = detallesGVA53.PR_ULC_E_E;
                cm.Parameters.Add("@PR_ULC_L_E", SqlDbType.Decimal).Value = detallesGVA53.PR_ULC_L_E;
                cm.Parameters.Add("@PRECSINDTO", SqlDbType.Decimal).Value = detallesGVA53.PRECSINDTO;
                cm.Parameters.Add("@IMPORTE_EXENTO", SqlDbType.Decimal).Value = detallesGVA53.IMPORTE_EXENTO;
                cm.Parameters.Add("@IMPORTE_GRAVADO", SqlDbType.Decimal).Value = detallesGVA53.IMPORTE_GRAVADO;
                cm.Parameters.Add("@CANTIDAD_2", SqlDbType.Decimal).Value = detallesGVA53.CANTIDAD_2;
                cm.Parameters.Add("@FALTAN_REM_2", SqlDbType.Decimal).Value = detallesGVA53.FALTAN_REM_2;
                cm.Parameters.Add("@ID_MEDIDA_VENTAS", SqlDbType.Int).Value = DBNull.Value;
                cm.Parameters.Add("@ID_MEDIDA_STOCK_2", SqlDbType.Int).Value = DBNull.Value;
                cm.Parameters.Add("@ID_MEDIDA_STOCK", SqlDbType.Int).Value = DBNull.Value;
                cm.Parameters.Add("@UNIDAD_MEDIDA_SELECCIONADA", SqlDbType.Char).Value = detallesGVA53.UNIDAD_MEDIDA_SELECCIONADA;
                cm.Parameters.Add("@RENGL_PADR", SqlDbType.Int).Value = detallesGVA53.RENGL_PADR;
                cm.Parameters.Add("@COD_ARTICU_KIT", SqlDbType.VarChar).Value = detallesGVA53.COD_ARTICU_KIT;
                cm.Parameters.Add("@INSUMO_KIT_SEPARADO", SqlDbType.Bit).Value = detallesGVA53.INSUMO_KIT_SEPARADO;
                cm.Parameters.Add("@PRECIO_FECHA", SqlDbType.DateTime).Value = detallesGVA53.PRECIO_FECHA;
                cm.Parameters.Add("@PRECIO_LISTA", SqlDbType.Decimal).Value = detallesGVA53.PRECIO_LISTA;
                cm.Parameters.Add("@PRECIO_BONIF", SqlDbType.Decimal).Value = detallesGVA53.PRECIO_BONIF;
                cm.Parameters.Add("@PORC_DTO_PARAM", SqlDbType.Decimal).Value = detallesGVA53.PORC_DTO_PARAM;
                cm.Parameters.Add("@FECHA_MODIFICACION_PRECIO", SqlDbType.DateTime).Value = detallesGVA53.FECHA_MODIFICACION_PRECIO;
                cm.Parameters.Add("@USUARIO_MODIFICACION_PRECIO", SqlDbType.VarChar).Value = DBNull.Value;
                cm.Parameters.Add("@TERMINAL_MODIFICACION_PRECIO", SqlDbType.VarChar).Value = DBNull.Value;
                cm.Parameters.Add("@ITEM_ESPECTACULO", SqlDbType.VarChar).Value = detallesGVA53.ITEM_ESPECTACULO;

                return cm;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public SqlCommand GuardarCuotas(Entidades.CuotasGVA46 cuotasGVA46)
        {
            try
            {
                SqlCommand cm = new SqlCommand();

                cm.CommandText =
                    @" INSERT INTO GVA46
                        (
                        FILLER,
                        ESTADO_VTO,
                        FECHA_VTO,
                        IMPORTE_VT,
                        N_COMP,
                        T_COMP,
                        ESTADO_UNI,
                        IMP_VT_UNI,
                        IMP_VT_EXT,
                        IM_VT_UN_E,
                        ALTERNATIVA_1,
                        IMPORTE_TOTAL_1,
                        ALTERNATIVA_2,
                        IMPORTE_TOTAL_2,
                        AJUSTA_COBRO_FECHA_ALTERNATIVA,
                        UNIDADES_TOTAL_1,
                        UNIDADES_TOTAL_2
                        )   
                        VALUES
                        (
                        @FILLER,
                        @ESTADO_VTO,
                        @FECHA_VTO,
                        @IMPORTE_VT,
                        @N_COMP,
                        @T_COMP,
                        @ESTADO_UNI,
                        @IMP_VT_UNI,
                        @IMP_VT_EXT,
                        @IM_VT_UN_E,
                        @ALTERNATIVA_1,
                        @IMPORTE_TOTAL_1,
                        @ALTERNATIVA_2,
                        @IMPORTE_TOTAL_2,
                        @AJUSTA_COBRO_FECHA_ALTERNATIVA,
                        @UNIDADES_TOTAL_1,
                        @UNIDADES_TOTAL_2
                        )
                    ";
                cm.Parameters.Clear();
                cm.Parameters.Add("@FILLER", SqlDbType.VarChar).Value = cuotasGVA46.FILLER;
                cm.Parameters.Add("@ESTADO_VTO", SqlDbType.VarChar).Value = cuotasGVA46.ESTADO_VTO;
                cm.Parameters.Add("@FECHA_VTO", SqlDbType.DateTime).Value = cuotasGVA46.FECHA_VTO;
                cm.Parameters.Add("@IMPORTE_VT", SqlDbType.Decimal).Value = cuotasGVA46.IMPORTE_VT;
                cm.Parameters.Add("@N_COMP", SqlDbType.VarChar).Value = cuotasGVA46.N_COMP;
                cm.Parameters.Add("@T_COMP", SqlDbType.VarChar).Value = cuotasGVA46.T_COMP;
                cm.Parameters.Add("@ESTADO_UNI", SqlDbType.VarChar).Value = cuotasGVA46.ESTADO_UNI;
                cm.Parameters.Add("@IMP_VT_UNI", SqlDbType.Decimal).Value = cuotasGVA46.IMP_VT_UNI;
                cm.Parameters.Add("@IMP_VT_EXT", SqlDbType.Decimal).Value = cuotasGVA46.IMP_VT_EXT;
                cm.Parameters.Add("@IM_VT_UN_E", SqlDbType.Decimal).Value = cuotasGVA46.IM_VT_UN_E;
                cm.Parameters.Add("@ALTERNATIVA_1", SqlDbType.DateTime).Value = cuotasGVA46.ALTERNATIVA_1;
                cm.Parameters.Add("@IMPORTE_TOTAL_1", SqlDbType.Decimal).Value = cuotasGVA46.IMPORTE_TOTAL_1;
                cm.Parameters.Add("@ALTERNATIVA_2", SqlDbType.DateTime).Value = cuotasGVA46.ALTERNATIVA_2;
                cm.Parameters.Add("@IMPORTE_TOTAL_2", SqlDbType.Decimal).Value = cuotasGVA46.IMPORTE_TOTAL_2;
                cm.Parameters.Add("@AJUSTA_COBRO_FECHA_ALTERNATIVA", SqlDbType.VarChar).Value = cuotasGVA46.AJUSTA_COBRO_FECHA_ALTERNATIVA;
                cm.Parameters.Add("@UNIDADES_TOTAL_1", SqlDbType.Decimal).Value = cuotasGVA46.UNIDADES_TOTAL_1;
                cm.Parameters.Add("@UNIDADES_TOTAL_2", SqlDbType.Decimal).Value = cuotasGVA46.UNIDADES_TOTAL_2;


                return cm;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public SqlCommand GuardarImpuestos(Entidades.ImpuestosGVA42 impuestosGVA42)
        {
            try
            {
                SqlCommand cm = new SqlCommand();

                cm.CommandText =
                    @"  INSERT INTO GVA42
                        (
                        FILLER,
                        COD_ALICUO,
                        IMPORTE,
                        N_COMP,
                        NETO_GRAV,
                        PERCEP,
                        PORCENTAJE,
                        T_COMP,
                        COD_IMPUES,
                        COD_SII,
                        IMP_EXT,
                        NE_GRAV_EX,
                        PERCEP_EXT
                        )
                        VALUES
                        (
                        @FILLER,
                        @COD_ALICUO,
                        @IMPORTE,
                        @N_COMP,
                        @NETO_GRAV,
                        @PERCEP,
                        @PORCENTAJE,
                        @T_COMP,
                        @COD_IMPUES,
                        @COD_SII,
                        @IMP_EXT,
                        @NE_GRAV_EX,
                        @PERCEP_EXT
                        )
                    ";
                cm.Parameters.Clear();
                cm.Parameters.Add("@FILLER", SqlDbType.VarChar).Value = impuestosGVA42.FILLER;
                cm.Parameters.Add("@COD_ALICUO", SqlDbType.Int).Value = impuestosGVA42.COD_ALICUO;
                cm.Parameters.Add("@IMPORTE", SqlDbType.Decimal).Value = impuestosGVA42.IMPORTE;
                cm.Parameters.Add("@N_COMP", SqlDbType.VarChar).Value = impuestosGVA42.N_COMP;
                cm.Parameters.Add("@NETO_GRAV", SqlDbType.Decimal).Value = impuestosGVA42.NETO_GRAV;
                cm.Parameters.Add("@PERCEP", SqlDbType.Decimal).Value = impuestosGVA42.PERCEP;
                cm.Parameters.Add("@PORCENTAJE", SqlDbType.Decimal).Value = impuestosGVA42.PORCENTAJE;
                cm.Parameters.Add("@T_COMP", SqlDbType.VarChar).Value = impuestosGVA42.T_COMP;
                cm.Parameters.Add("@COD_IMPUES", SqlDbType.VarChar).Value = impuestosGVA42.COD_IMPUES;
                cm.Parameters.Add("@COD_SII", SqlDbType.VarChar).Value = impuestosGVA42.COD_SII;
                cm.Parameters.Add("@IMP_EXT", SqlDbType.Decimal).Value = impuestosGVA42.IMP_EXT;
                cm.Parameters.Add("@NE_GRAV_EX", SqlDbType.Decimal).Value = impuestosGVA42.NE_GRAV_EX;
                cm.Parameters.Add("@PERCEP_EXT", SqlDbType.Decimal).Value = impuestosGVA42.PERCEP_EXT;

                return cm;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public SqlCommand GuardarEncabezadoMovimientos(Entidades.EncabezadoMovimientosSBA04 encabMovimiento)
        {
            try
            {
                SqlCommand cm = new SqlCommand();
                cm.CommandText =
                    @"
                        INSERT INTO(
                        FILLER,
                        BARRA,
                        CERRADO,
                        CLASE,
                        COD_COMP,
                        CONCEPTO,
                        COTIZACION,
                        EXPORTADO,
                        EXTERNO,
                        FECHA,
                        FECHA_ING,
                        HORA_ING,
                        N_COMP,
                        N_INTERNO,
                        PASE,
                        SITUACION,
                        TERMINAL,
                        USUARIO,
                        LOTE,
                        LOTE_ANU,
                        SUCUR_ORI,
                        FECHA_ORI,
                        C_COMP_ORI,
                        N_COMP_ORI,
                        BARRA_ORI,
                        FECHA_EMIS,
                        GENERA_ASIENTO,
                        FECHA_ULTIMA_MODIFICACION,
                        HORA_ULTIMA_MODIFICACION,
                        USUA_ULTIMA_MODIFICACION,
                        TERM_ULTIMA_MODIFICACION,
                        ID_PUESTO_CAJA,
                        ID_GVA81,
                        ID_SBA02,
                        ID_SBA02_C_COMP_ORI,
                        COD_GVA14,
                        COD_CPA01,
                        ID_CODIGO_RELACION,
                        ID_LEGAJO,
                        OBSERVACIONES,
                        TIPO_COD_RELACIONADO,
                        CN_ASTOR,
                        ID_MODELO_INGRESO_SB,
                        TOTAL_IMPORTE_CTE,
                        TOTAL_IMPORTE_EXT,
                        TRANSFERENCIA_DEVOLUCION_CUPONES
                        )
                        VALUES
                        (
                        @FILLER,
                        @BARRA,
                        @CERRADO,
                        @CLASE,
                        @COD_COMP,
                        @CONCEPTO,
                        @COTIZACION,
                        @EXPORTADO,
                        @EXTERNO,
                        @FECHA,
                        @FECHA_ING,
                        @HORA_ING,
                        @N_COMP,
                        @N_INTERNO,
                        @PASE,
                        @SITUACION,
                        @TERMINAL,
                        @USUARIO,
                        @LOTE,
                        @LOTE_ANU,
                        @SUCUR_ORI,
                        @FECHA_ORI,
                        @C_COMP_ORI,
                        @N_COMP_ORI,
                        @BARRA_ORI,
                        @FECHA_EMIS,
                        @GENERA_ASIENTO,
                        @FECHA_ULTIMA_MODIFICACION,
                        @HORA_ULTIMA_MODIFICACION,
                        @USUA_ULTIMA_MODIFICACION,
                        @TERM_ULTIMA_MODIFICACION,
                        @ID_PUESTO_CAJA,
                        @ID_GVA81,
                        @ID_SBA02,
                        @ID_SBA02_C_COMP_ORI,
                        @COD_GVA14,
                        @COD_CPA01,
                        @ID_CODIGO_RELACION,
                        @ID_LEGAJO,
                        @OBSERVACIONES,
                        @TIPO_COD_RELACIONADO,
                        @CN_ASTOR,
                        @ID_MODELO_INGRESO_SB,
                        @TOTAL_IMPORTE_CTE,
                        @TOTAL_IMPORTE_EXT,
                        @TRANSFERENCIA_DEVOLUCION_CUPONES
                        )
                    ";
                cm.Parameters.Clear();
                cm.Parameters.Add("@FILLER", SqlDbType.VarChar).Value = encabMovimiento.FILLER;
                cm.Parameters.Add("@BARRA", SqlDbType.Int).Value = encabMovimiento.BARRA;
                cm.Parameters.Add("@CERRADO", SqlDbType.Bit).Value = encabMovimiento.CERRADO;
                cm.Parameters.Add("@CLASE", SqlDbType.Int).Value = encabMovimiento.CLASE;
                cm.Parameters.Add("@COD_COMP", SqlDbType.VarChar).Value = encabMovimiento.COD_COMP;
                cm.Parameters.Add("@CONCEPTO", SqlDbType.VarChar).Value = encabMovimiento.CONCEPTO;
                cm.Parameters.Add("@COTIZACION", SqlDbType.Decimal).Value = encabMovimiento.COTIZACION;
                cm.Parameters.Add("@EXPORTADO", SqlDbType.Bit).Value = encabMovimiento.EXPORTADO;
                cm.Parameters.Add("@EXTERNO", SqlDbType.Bit).Value = encabMovimiento.EXTERNO;
                cm.Parameters.Add("@FECHA", SqlDbType.DateTime).Value = encabMovimiento.FECHA;
                cm.Parameters.Add("@FECHA_ING", SqlDbType.DateTime).Value = encabMovimiento.FECHA_ING;
                cm.Parameters.Add("@HORA_ING", SqlDbType.VarChar).Value = encabMovimiento.HORA_ING;
                cm.Parameters.Add("@N_COMP", SqlDbType.VarChar).Value = encabMovimiento.N_COMP;
                cm.Parameters.Add("@N_INTERNO", SqlDbType.Decimal).Value = encabMovimiento.N_INTERNO;
                cm.Parameters.Add("@PASE", SqlDbType.Bit).Value = encabMovimiento.PASE;
                cm.Parameters.Add("@SITUACION", SqlDbType.VarChar).Value = encabMovimiento.SITUACION;
                cm.Parameters.Add("@TERMINAL", SqlDbType.VarChar).Value = encabMovimiento.TERMINAL;
                cm.Parameters.Add("@USUARIO", SqlDbType.VarChar).Value = encabMovimiento.USUARIO;
                cm.Parameters.Add("@LOTE", SqlDbType.Decimal).Value = encabMovimiento.LOTE;
                cm.Parameters.Add("@LOTE_ANU", SqlDbType.Decimal).Value = encabMovimiento.LOTE_ANU;
                cm.Parameters.Add("@SUCUR_ORI", SqlDbType.Int).Value = encabMovimiento.SUCUR_ORI;
                cm.Parameters.Add("@FECHA_ORI", SqlDbType.DateTime).Value = encabMovimiento.FECHA_ORI;
                cm.Parameters.Add("@C_COMP_ORI", SqlDbType.VarChar).Value = encabMovimiento.C_COMP_ORI;
                cm.Parameters.Add("@N_COMP_ORI", SqlDbType.VarChar).Value = encabMovimiento.N_COMP_ORI;
                cm.Parameters.Add("@BARRA_ORI", SqlDbType.Int).Value = encabMovimiento.BARRA_ORI;
                cm.Parameters.Add("@FECHA_EMIS", SqlDbType.DateTime).Value = encabMovimiento.FECHA_EMIS;
                cm.Parameters.Add("@GENERA_ASIENTO", SqlDbType.VarChar).Value = encabMovimiento.GENERA_ASIENTO;
                cm.Parameters.Add("@FECHA_ULTIMA_MODIFICACION", SqlDbType.DateTime).Value = encabMovimiento.FECHA_ULTIMA_MODIFICACION;
                cm.Parameters.Add("@HORA_ULTIMA_MODIFICACION", SqlDbType.VarChar).Value = encabMovimiento.HORA_ULTIMA_MODIFICACION;
                cm.Parameters.Add("@USUA_ULTIMA_MODIFICACION", SqlDbType.VarChar).Value = encabMovimiento.USUA_ULTIMA_MODIFICACION;
                cm.Parameters.Add("@TERM_ULTIMA_MODIFICACION", SqlDbType.VarChar).Value = encabMovimiento.TERM_ULTIMA_MODIFICACION;
                cm.Parameters.Add("@ID_PUESTO_CAJA", SqlDbType.Int).Value = encabMovimiento.ID_PUESTO_CAJA;
                cm.Parameters.Add("@ID_GVA81", SqlDbType.Int).Value = encabMovimiento.ID_GVA81;
                cm.Parameters.Add("@ID_SBA02", SqlDbType.Int).Value = encabMovimiento.ID_SBA02;
                cm.Parameters.Add("@ID_SBA02_C_COMP_ORI", SqlDbType.Int).Value = encabMovimiento.ID_SBA02_C_COMP_ORI;
                cm.Parameters.Add("@COD_GVA14", SqlDbType.VarChar).Value = encabMovimiento.COD_GVA14;
                cm.Parameters.Add("@COD_CPA01", SqlDbType.VarChar).Value = encabMovimiento.COD_CPA01;
                cm.Parameters.Add("@ID_CODIGO_RELACION", SqlDbType.Int).Value = encabMovimiento.ID_CODIGO_RELACION;
                cm.Parameters.Add("@ID_LEGAJO", SqlDbType.Int).Value = encabMovimiento.ID_LEGAJO;
                cm.Parameters.Add("@OBSERVACIONES", SqlDbType.VarChar).Value = encabMovimiento.OBSERVACIONES;
                cm.Parameters.Add("@TIPO_COD_RELACIONADO", SqlDbType.VarChar).Value = encabMovimiento.TIPO_COD_RELACIONADO;
                cm.Parameters.Add("@CN_ASTOR", SqlDbType.VarChar).Value = encabMovimiento.CN_ASTOR;
                cm.Parameters.Add("@ID_MODELO_INGRESO_SB", SqlDbType.Int).Value = encabMovimiento.ID_MODELO_INGRESO_SB;
                cm.Parameters.Add("@TOTAL_IMPORTE_CTE", SqlDbType.Decimal).Value = encabMovimiento.TOTAL_IMPORTE_CTE;
                cm.Parameters.Add("@TOTAL_IMPORTE_EXT", SqlDbType.Decimal).Value = encabMovimiento.TOTAL_IMPORTE_EXT;
                cm.Parameters.Add("@TRANSFERENCIA_DEVOLUCION_CUPONES", SqlDbType.VarChar).Value = encabMovimiento.TRANSFERENCIA_DEVOLUCION_CUPONES;
                return cm;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public SqlCommand GuardarMovimientos(Entidades.DetalleMovimientoSBA05 movimientosSBA05)
        {
            try
            {
                SqlCommand cm = new SqlCommand();
                cm.CommandText =
                    @"
                        INSERT INTO
                        (
                        FILLER,
                        BARRA,
                        CANT_MONE,
                        CHEQUES,
                        CLASE,
                        COD_COMP,
                        COD_CTA,
                        COD_OPERAC,
                        CONCILIADO,
                        COTIZ_MONE,
                        D_H,
                        EFECTIVO,
                        FECHA,
                        FECHA_CONC,
                        LEYENDA,
                        MONTO,
                        N_COMP,
                        RENGLON,
                        UNIDADES,
                        VA_DIRECTO,
                        ID_SBA02,
                        ID_GVA81,
                        CONC_EFTV,
                        F_CONC_EFT,
                        COMENTARIO,
                        COMENTARIO_EFT,
                        COD_GVA14,
                        COD_CPA01,
                        ID_CODIGO_RELACION,
                        ID_LEGAJO,
                        TIPO_COD_RELACIONADO,
                        ID_TIPO_COTIZACION,
                        ID_SBA11
                        )
                        VALUES
                        (
                        @FILLER,
                        @BARRA,
                        @CANT_MONE,
                        @CHEQUES,
                        @CLASE,
                        @COD_COMP,
                        @COD_CTA,
                        @COD_OPERAC,
                        @CONCILIADO,
                        @COTIZ_MONE,
                        @D_H,
                        @EFECTIVO,
                        @FECHA,
                        @FECHA_CONC,
                        @LEYENDA,
                        @MONTO,
                        @N_COMP,
                        @RENGLON,
                        @UNIDADES,
                        @VA_DIRECTO,
                        @ID_SBA02,
                        @ID_GVA81,
                        @CONC_EFTV,
                        @F_CONC_EFT,
                        @COMENTARIO,
                        @COMENTARIO_EFT,
                        @COD_GVA14,
                        @COD_CPA01,
                        @ID_CODIGO_RELACION,
                        @ID_LEGAJO,
                        @TIPO_COD_RELACIONADO,
                        @ID_TIPO_COTIZACION,
                        @ID_SBA11
                        )
                    ";
                cm.Parameters.Clear();
                cm.Parameters.Add("@FILLER", SqlDbType.VarChar).Value = movimientosSBA05.FILLER;
                cm.Parameters.Add("@BARRA", SqlDbType.Int).Value = movimientosSBA05.BARRA;
                cm.Parameters.Add("@CANT_MONE", SqlDbType.Decimal).Value = movimientosSBA05.CANT_MONE;
                cm.Parameters.Add("@CHEQUES", SqlDbType.Decimal).Value = movimientosSBA05.CHEQUES;
                cm.Parameters.Add("@CLASE", SqlDbType.Int).Value = movimientosSBA05.CLASE;
                cm.Parameters.Add("@COD_COMP", SqlDbType.VarChar).Value = movimientosSBA05.COD_COMP;
                cm.Parameters.Add("@COD_CTA", SqlDbType.Decimal).Value = movimientosSBA05.COD_CTA;
                cm.Parameters.Add("@COD_OPERAC", SqlDbType.VarChar).Value = movimientosSBA05.COD_OPERAC;
                cm.Parameters.Add("@CONCILIADO", SqlDbType.Bit).Value = movimientosSBA05.CONCILIADO;
                cm.Parameters.Add("@COTIZ_MONE", SqlDbType.Decimal).Value = movimientosSBA05.COTIZ_MONE;
                cm.Parameters.Add("@D_H", SqlDbType.VarChar).Value = movimientosSBA05.D_H;
                cm.Parameters.Add("@EFECTIVO", SqlDbType.Decimal).Value = movimientosSBA05.EFECTIVO;
                cm.Parameters.Add("@FECHA", SqlDbType.DateTime).Value = movimientosSBA05.FECHA;
                cm.Parameters.Add("@FECHA_CONC", SqlDbType.DateTime).Value = movimientosSBA05.FECHA_CONC;
                cm.Parameters.Add("@LEYENDA", SqlDbType.VarChar).Value = movimientosSBA05.LEYENDA;
                cm.Parameters.Add("@MONTO", SqlDbType.Decimal).Value = movimientosSBA05.MONTO;
                cm.Parameters.Add("@N_COMP", SqlDbType.VarChar).Value = movimientosSBA05.N_COMP;
                cm.Parameters.Add("@RENGLON", SqlDbType.Int).Value = movimientosSBA05.RENGLON;
                cm.Parameters.Add("@UNIDADES", SqlDbType.Decimal).Value = movimientosSBA05.UNIDADES;
                cm.Parameters.Add("@VA_DIRECTO", SqlDbType.VarChar).Value = movimientosSBA05.VA_DIRECTO;
                cm.Parameters.Add("@ID_SBA02", SqlDbType.Int).Value = movimientosSBA05.ID_SBA02;
                cm.Parameters.Add("@ID_GVA81", SqlDbType.Int).Value = movimientosSBA05.ID_GVA81;
                cm.Parameters.Add("@CONC_EFTV", SqlDbType.Bit).Value = movimientosSBA05.CONC_EFTV;
                cm.Parameters.Add("@F_CONC_EFT", SqlDbType.DateTime).Value = movimientosSBA05.F_CONC_EFT;
                cm.Parameters.Add("@COMENTARIO", SqlDbType.VarChar).Value = movimientosSBA05.COMENTARIO;
                cm.Parameters.Add("@COMENTARIO_EFT", SqlDbType.VarChar).Value = movimientosSBA05.COMENTARIO_EFT;
                cm.Parameters.Add("@COD_GVA14", SqlDbType.VarChar).Value = movimientosSBA05.COD_GVA14;
                cm.Parameters.Add("@COD_CPA01", SqlDbType.VarChar).Value = movimientosSBA05.COD_CPA01;
                cm.Parameters.Add("@ID_CODIGO_RELACION", SqlDbType.Int).Value = movimientosSBA05.ID_CODIGO_RELACION;
                cm.Parameters.Add("@ID_LEGAJO", SqlDbType.Int).Value = movimientosSBA05.ID_LEGAJO;
                cm.Parameters.Add("@TIPO_COD_RELACIONADO", SqlDbType.VarChar).Value = movimientosSBA05.TIPO_COD_RELACIONADO;
                cm.Parameters.Add("@ID_TIPO_COTIZACION", SqlDbType.Int).Value = movimientosSBA05.ID_TIPO_COTIZACION;
                cm.Parameters.Add("@ID_SBA11", SqlDbType.Int).Value = movimientosSBA05.ID_SBA11;

                return cm;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public SqlCommand GuardarCupones(Entidades.CuponesSBA20 cuponesSBA20)
        {
            try
            {
                SqlCommand cm = new SqlCommand();
                cm.CommandText =
                    @"
                        INSERT INTO
                        (
                        FILLER,
                        BARRA_REC,
                        CANT_CUOTA,
                        COD_CTA,
                        COTIZ,
                        CUOTA,
                        ESTADO,
                        EXPORTADO,
                        F_VTO_TARJ,
                        FECHA_CUPO,
                        FECHA_DEP,
                        FECHA_REC,
                        IMPORTE_TO,
                        MONEDA_EX,
                        N_AUTORIZA,
                        N_COMP_DEP,
                        N_COMP_REC,
                        N_CUPON,
                        N_DOC,
                        N_SOCIO,
                        NOMBRE_SOC,
                        T_COMP_DEP,
                        T_COMP_REC,
                        T_DOC,
                        TELEFONO,
                        FECHA_SAL,
                        T_COMP_SAL,
                        N_COMP_SAL,
                        BARRA_SAL,
                        NRO_SUCURS,
                        TERM_ID,
                        LOTE,
                        HORA_REC,
                        TERM_ID_2,
                        ID_HOST,
                        ID_SBA22,
                        ID_PLAN_TARJETA,
                        COEF_VENTA,
                        COEF_ACRED,
                        PORC_DESC,
                        COMISION,
                        ID_PROMOCION_TARJETA,
                        RENGLON_REC,
                        NETO_TOT,
                        ORIGEN,
                        TIPO_CUPON,
                        ID_SUCURSAL,
                        ID_TERMINAL_POS,
                        BARRA_DEP,
                        OBSERVACIONES,
                        RENGLON_DEP,
                        RENGLON_SAL,
                        VERSION_ANT,
                        ID_SBA02_REC,
                        ID_SBA02_DEP,
                        ID_SBA02_SAL
                        )
                        VALUES
                        (
                        @FILLER,
                        @BARRA_REC,
                        @CANT_CUOTA,
                        @COD_CTA,
                        @COTIZ,
                        @CUOTA,
                        @ESTADO,
                        @EXPORTADO,
                        @F_VTO_TARJ,
                        @FECHA_CUPO,
                        @FECHA_DEP,
                        @FECHA_REC,
                        @IMPORTE_TO,
                        @MONEDA_EX,
                        @N_AUTORIZA,
                        @N_COMP_DEP,
                        @N_COMP_REC,
                        @N_CUPON,
                        @N_DOC,
                        @N_SOCIO,
                        @NOMBRE_SOC,
                        @T_COMP_DEP,
                        @T_COMP_REC,
                        @T_DOC,
                        @TELEFONO,
                        @FECHA_SAL,
                        @T_COMP_SAL,
                        @N_COMP_SAL,
                        @BARRA_SAL,
                        @NRO_SUCURS,
                        @TERM_ID,
                        @LOTE,
                        @HORA_REC,
                        @TERM_ID_2,
                        @ID_HOST,
                        @ID_SBA22,
                        @ID_PLAN_TARJETA,
                        @COEF_VENTA,
                        @COEF_ACRED,
                        @PORC_DESC,
                        @COMISION,
                        @ID_PROMOCION_TARJETA,
                        @RENGLON_REC,
                        @NETO_TOT,
                        @ORIGEN,
                        @TIPO_CUPON,
                        @ID_SUCURSAL,
                        @ID_TERMINAL_POS,
                        @BARRA_DEP,
                        @OBSERVACIONES,
                        @RENGLON_DEP,
                        @RENGLON_SAL,
                        @VERSION_ANT,
                        @ID_SBA02_REC,
                        @ID_SBA02_DEP,
                        @ID_SBA02_SAL
                        )
                    ";
                cm.Parameters.Clear();
                cm.Parameters.Add("@FILLER", SqlDbType.VarChar).Value = cuponesSBA20.FILLER;
                cm.Parameters.Add("@BARRA_REC", SqlDbType.Int).Value = cuponesSBA20.BARRA_REC;
                cm.Parameters.Add("@CANT_CUOTA", SqlDbType.Int).Value = cuponesSBA20.CANT_CUOTA;
                cm.Parameters.Add("@COD_CTA", SqlDbType.Decimal).Value = cuponesSBA20.COD_CTA;
                cm.Parameters.Add("@COTIZ", SqlDbType.Decimal).Value = cuponesSBA20.COTIZ;
                cm.Parameters.Add("@CUOTA", SqlDbType.VarChar).Value = cuponesSBA20.CUOTA;
                cm.Parameters.Add("@ESTADO", SqlDbType.VarChar).Value = cuponesSBA20.ESTADO;
                cm.Parameters.Add("@EXPORTADO", SqlDbType.Bit).Value = cuponesSBA20.EXPORTADO;
                cm.Parameters.Add("@F_VTO_TARJ", SqlDbType.VarChar).Value = cuponesSBA20.F_VTO_TARJ;
                cm.Parameters.Add("@FECHA_CUPO", SqlDbType.DateTime).Value = cuponesSBA20.FECHA_CUPO;
                cm.Parameters.Add("@FECHA_DEP", SqlDbType.DateTime).Value = cuponesSBA20.FECHA_DEP;
                cm.Parameters.Add("@FECHA_REC", SqlDbType.DateTime).Value = cuponesSBA20.FECHA_REC;
                cm.Parameters.Add("@IMPORTE_TO", SqlDbType.Decimal).Value = cuponesSBA20.IMPORTE_TO;
                cm.Parameters.Add("@MONEDA_EX", SqlDbType.Bit).Value = cuponesSBA20.MONEDA_EX;
                cm.Parameters.Add("@N_AUTORIZA", SqlDbType.VarChar).Value = cuponesSBA20.N_AUTORIZA;
                cm.Parameters.Add("@N_COMP_DEP", SqlDbType.VarChar).Value = cuponesSBA20.N_COMP_DEP;
                cm.Parameters.Add("@N_COMP_REC", SqlDbType.VarChar).Value = cuponesSBA20.N_COMP_REC;
                cm.Parameters.Add("@N_CUPON", SqlDbType.Decimal).Value = cuponesSBA20.N_CUPON;
                cm.Parameters.Add("@N_DOC,", SqlDbType.VarChar).Value = cuponesSBA20.N_DOC;
                cm.Parameters.Add("@N_SOCIO", SqlDbType.VarChar).Value = cuponesSBA20.N_SOCIO;
                cm.Parameters.Add("@NOMBRE_SOC", SqlDbType.VarChar).Value = cuponesSBA20.NOMBRE_SOC;
                cm.Parameters.Add("@T_COMP_DEP", SqlDbType.VarChar).Value = cuponesSBA20.T_COMP_DEP;
                cm.Parameters.Add("@T_COMP_REC", SqlDbType.VarChar).Value = cuponesSBA20.T_COMP_REC;
                cm.Parameters.Add("@T_DOC", SqlDbType.VarChar).Value = cuponesSBA20.T_DOC;
                cm.Parameters.Add("@TELEFONO", SqlDbType.VarChar).Value = cuponesSBA20.TELEFONO;
                cm.Parameters.Add("@FECHA_SAL", SqlDbType.DateTime).Value = cuponesSBA20.FECHA_SAL;
                cm.Parameters.Add("@T_COMP_SAL", SqlDbType.VarChar).Value = cuponesSBA20.T_COMP_SAL;
                cm.Parameters.Add("@N_COMP_SAL", SqlDbType.VarChar).Value = cuponesSBA20.N_COMP_SAL;
                cm.Parameters.Add("@BARRA_SAL", SqlDbType.Int).Value = cuponesSBA20.BARRA_SAL;
                cm.Parameters.Add("@NRO_SUCURS", SqlDbType.Int).Value = cuponesSBA20.NRO_SUCURS;
                cm.Parameters.Add("@TERM_ID", SqlDbType.VarChar).Value = cuponesSBA20.TERM_ID;
                cm.Parameters.Add("@LOTE", SqlDbType.VarChar).Value = cuponesSBA20.LOTE;
                cm.Parameters.Add("@HORA_REC", SqlDbType.VarChar).Value = cuponesSBA20.HORA_REC;
                cm.Parameters.Add("@TERM_ID_2", SqlDbType.VarChar).Value = cuponesSBA20.TERM_ID_2;
                cm.Parameters.Add("@ID_HOST", SqlDbType.Int).Value = cuponesSBA20.ID_HOST;
                cm.Parameters.Add("@ID_SBA22", SqlDbType.Int).Value = cuponesSBA20.ID_SBA22;
                cm.Parameters.Add("@ID_PLAN_TARJETA", SqlDbType.Int).Value = cuponesSBA20.ID_PLAN_TARJETA;
                cm.Parameters.Add("@COEF_VENTA", SqlDbType.Decimal).Value = cuponesSBA20.COEF_VENTA;
                cm.Parameters.Add("@COEF_ACRED", SqlDbType.Decimal).Value = cuponesSBA20.COEF_ACRED;
                cm.Parameters.Add("@PORC_DESC", SqlDbType.Decimal).Value = cuponesSBA20.PORC_DESC;
                cm.Parameters.Add("@COMISION", SqlDbType.Decimal).Value = cuponesSBA20.COMISION;
                cm.Parameters.Add("@ID_PROMOCION_TARJETA", SqlDbType.Int).Value = cuponesSBA20.ID_PROMOCION_TARJETA;
                cm.Parameters.Add("@RENGLON_REC", SqlDbType.Int).Value = cuponesSBA20.RENGLON_REC;
                cm.Parameters.Add("@NETO_TOT", SqlDbType.Decimal).Value = cuponesSBA20.NETO_TOT;
                cm.Parameters.Add("@ORIGEN", SqlDbType.VarChar).Value = cuponesSBA20.ORIGEN;
                cm.Parameters.Add("@TIPO_CUPON", SqlDbType.VarChar).Value = cuponesSBA20.TIPO_CUPON;
                cm.Parameters.Add("@ID_SUCURSAL", SqlDbType.Int).Value = cuponesSBA20.ID_SUCURSAL;
                cm.Parameters.Add("@ID_TERMINAL_POS", SqlDbType.Int).Value = cuponesSBA20.ID_TERMINAL_POS;
                cm.Parameters.Add("@BARRA_DEP", SqlDbType.Int).Value = cuponesSBA20.BARRA_DEP;
                cm.Parameters.Add("@OBSERVACIONES", SqlDbType.VarChar).Value = cuponesSBA20.OBSERVACIONES;
                cm.Parameters.Add("@RENGLON_DEP", SqlDbType.Int).Value = cuponesSBA20.RENGLON_DEP;
                cm.Parameters.Add("@RENGLON_SAL", SqlDbType.Int).Value = cuponesSBA20.RENGLON_SAL;
                cm.Parameters.Add("@VERSION_ANT", SqlDbType.Bit).Value = cuponesSBA20.VERSION_ANT;
                cm.Parameters.Add("@ID_SBA02_REC", SqlDbType.Int).Value = cuponesSBA20.ID_SBA02_REC;
                cm.Parameters.Add("@ID_SBA02_DEP", SqlDbType.Int).Value = cuponesSBA20.ID_SBA02_DEP;
                cm.Parameters.Add("@ID_SBA02_SAL", SqlDbType.Int).Value = cuponesSBA20.ID_SBA02_SAL;

                return cm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public SqlCommand GuardarCliente(Entidades.ClienteGVA14 clientesGVA14)
        {
            try
            {
                SqlCommand cm = new SqlCommand();
                cm.CommandText =
                    @"
                        INSERT INTO
                        (
                        FILLER,
                        ADJUNTO,
                        ALI_NO_CAT,
                        BMP,
                        C_POSTAL,
                        CALLE,
                        CALLE2_ENV,
                        CLAUSULA,
                        CLAVE_IS,
                        COD_CLIENT,
                        COD_PROVIN,
                        COD_TRANSP,
                        COD_VENDED,
                        COD_ZONA,
                        COND_VTA,
                        CUIT,
                        CUMPLEANIO,
                        CUPO_CREDI,
                        DIR_COM,
                        DOMICILIO,
                        DTO_ENVIO,
                        DTO_LEGAL,
                        E_MAIL,
                        ENV_DOMIC,
                        ENV_LOCAL,
                        ENV_POSTAL,
                        ENV_PROV,
                        EXPORTA,
                        FECHA_ALTA,
                        FECHA_ANT,
                        FECHA_DESD,
                        FECHA_HAST,
                        FECHA_INHA,
                        FECHA_VTO,
                        GRUPO_EMPR,
                        ID_EXTERNO,
                        ID_INTERNO,
                        II_D,
                        II_L,
                        IVA_D,
                        IVA_L,
                        LOCALIDAD,
                        N_IMPUESTO,
                        N_ING_BRUT,
                        NOM_COM,
                        NRO_ENVIO,
                        NRO_LEGAL,
                        NRO_LISTA,
                        OBSERVACIO,
                        PARTIDOENV,
                        PERMITE_IS,
                        PISO_ENVIO,
                        PISO_LEGAL, 
                        PORC_DESC,
                        PORC_EXCL,
                        PORC_RECAR,
                        PUNTAJE,
                        RAZON_SOCI,
                        SALDO_ANT,
                        SALDO_CC,
                        SALDO_DOC,
                        SALDO_D_UN,
                        SOBRE_II,
                        SOBRE_IVA,
                        TELEFONO_1,
                        TELEFONO_2,
                        TIPO,
                        TIPO_DOC,
                        ZONA_ENVIO,
                        FECHA_MODI,
                        EXP_SALDO,
                        N_PAGOELEC,
                        MON_CTE,
                        SAL_AN_UN,
                        SALDO_CC_U,
                        SUCUR_ORI,
                        LIMCRE_EN,
                        RG_1361,
                        CAL_DEB_IN,
                        PORCE_INT,
                        MON_MI_IN,
                        DIAS_MI_IN,
                        DESTINO_DE,
                        CLA_IMP_CL,
                        RECIBE_DE,
                        AUT_DE,
                        MAIL_DE,
                        WEB,
                        COD_RUBRO,
                        CTA_CLI,
                        CTO_CLI,
                        COD_GVA14,
                        CBU,
                        IDENTIF_AFIP,
                        IDIOMA_CTE,
                        DET_ARTIC,
                        INC_COMENT,
                        ID_GVA44_FEX,
                        ID_GVA44_NCEX,
                        ID_GVA44_NDEX,
                        CIUDAD,
                        CIUDAD_ENVIO,
                        APLICA_MORA,
                        ID_INTERES_POR_MORA,
                        PUBLICA_WEB_CLIENTES,
                        MAIL_NEXO,
                        AUTORIZADO_WEB_CLIENTES,
                        OBSERVACIONES,
                        COD_GVA18,
                        COD_GVA24,
                        COD_GVA23,
                        COD_GVA05,
                        COD_GVA62,
                        COD_GVA151,
                        COBRA_LUNES,
                        COBRA_MARTES,
                        COBRA_MIERCOLES,
                        COBRA_JUEVES,
                        COBRA_VIERNES,
                        COBRA_SABADO,
                        COBRA_DOMINGO,
                        HORARIO_COBRANZA,
                        COMENTARIO_TYP_FAC,
                        COMENTARIO_TYP_ND,
                        COMENTARIO_TYP_NC,
                        TELEFONO_MOVIL,
                        ID_CATEGORIA_IVA,
                        ID_GVA14,
                        COD_GVA150,
                        TYP_FEX,
                        TYP_NCEX,
                        TYP_NDEX,
                        COD_ACT_CNA25,
                        COD_GVA05_ENV,
                        COD_GVA18_ENV,
                        RG_3572_EMPRESA_VINCULADA_CLIENTE,
                        RG_3572_TIPO_OPERACION_HABITUAL_VENTAS,
                        INHABILITADO_NEXO_PEDIDOS,
                        ID_TIPO_DOCUMENTO_EXTERIOR,
                        NUMERO_DOCUMENTO_EXTERIOR,
                        WEB_CLIENT_ID,
                        RG_3685_TIPO_OPERACION_VENTAS,
                        REQUIERE_INFORMACION_ADICIONAL,
                        NRO_INSCR_RG1817,
                        INHABILITADO_NEXO_COBRANZAS,
                        CODIGO_AFINIDAD,
                        ID_TRA_ORIGEN_INFORMACION,
                        SEXO
                        )
                        VALUES
                        (
                        @FILLER,
                        @ADJUNTO,
                        @ALI_NO_CAT,
                        @BMP,
                        @C_POSTAL,
                        @CALLE,
                        @CALLE2_ENV,
                        @CLAUSULA,
                        @CLAVE_IS,
                        @COD_CLIENT,
                        @COD_PROVIN,
                        @COD_TRANSP,
                        @COD_VENDED,
                        @COD_ZONA,
                        @COND_VTA,
                        @CUIT,
                        @CUMPLEANIO,
                        @CUPO_CREDI,
                        @DIR_COM,
                        @DOMICILIO,
                        @DTO_ENVIO,
                        @DTO_LEGAL,
                        @E_MAIL,
                        @ENV_DOMIC,
                        @ENV_LOCAL,
                        @ENV_POSTAL,
                        @ENV_PROV,
                        @EXPORTA,
                        @FECHA_ALTA,
                        @FECHA_ANT,
                        @FECHA_DESD,
                        @FECHA_HAST,
                        @FECHA_INHA,
                        @FECHA_VTO,
                        @GRUPO_EMPR,
                        @ID_EXTERNO,
                        @ID_INTERNO,
                        @II_D,
                        @II_L,
                        @IVA_D,
                        @IVA_L,
                        @LOCALIDAD,
                        @N_IMPUESTO,
                        @N_ING_BRUT,
                        @NOM_COM,
                        @NRO_ENVIO,
                        @NRO_LEGAL,
                        @NRO_LISTA,
                        @OBSERVACIO,
                        @PARTIDOENV,
                        @PERMITE_IS,
                        @PISO_ENVIO,
                        @PISO_LEGAL, 
                        @PORC_DESC,
                        @PORC_EXCL,
                        @PORC_RECAR,
                        @PUNTAJE,
                        @RAZON_SOCI,
                        @SALDO_ANT,
                        @SALDO_CC,
                        @SALDO_DOC,
                        @SALDO_D_UN,
                        @SOBRE_II,
                        @SOBRE_IVA,
                        @TELEFONO_1,
                        @TELEFONO_2,
                        @TIPO,
                        @TIPO_DOC,
                        @ZONA_ENVIO,
                        @FECHA_MODI,
                        @EXP_SALDO,
                        @N_PAGOELEC,
                        @MON_CTE,
                        @SAL_AN_UN,
                        @SALDO_CC_U,
                        @SUCUR_ORI,
                        @LIMCRE_EN,
                        @RG_1361,
                        @CAL_DEB_IN,
                        @PORCE_INT,
                        @MON_MI_IN,
                        @DIAS_MI_IN,
                        @DESTINO_DE,
                        @CLA_IMP_CL,
                        @RECIBE_DE,
                        @AUT_DE,
                        @MAIL_DE,
                        @WEB,
                        @COD_RUBRO,
                        @CTA_CLI,
                        @CTO_CLI,
                        @COD_GVA14,
                        @CBU,
                        @IDENTIF_AFIP,
                        @IDIOMA_CTE,
                        @DET_ARTIC,
                        @INC_COMENT,
                        @ID_GVA44_FEX,
                        @ID_GVA44_NCEX,
                        @ID_GVA44_NDEX,
                        @CIUDAD,
                        @CIUDAD_ENVIO,
                        @APLICA_MORA,
                        @ID_INTERES_POR_MORA,
                        @PUBLICA_WEB_CLIENTES,
                        @MAIL_NEXO,
                        @AUTORIZADO_WEB_CLIENTES,
                        @OBSERVACIONES,
                        @COD_GVA18,
                        @COD_GVA24,
                        @COD_GVA23,
                        @COD_GVA05,
                        @COD_GVA62,
                        @COD_GVA151,
                        @COBRA_LUNES,
                        @COBRA_MARTES,
                        @COBRA_MIERCOLES,
                        @COBRA_JUEVES,
                        @COBRA_VIERNES,
                        @COBRA_SABADO,
                        @COBRA_DOMINGO,
                        @HORARIO_COBRANZA,
                        @COMENTARIO_TYP_FAC,
                        @COMENTARIO_TYP_ND,
                        @COMENTARIO_TYP_NC,
                        @TELEFONO_MOVIL,
                        @ID_CATEGORIA_IVA,
                        @ID_GVA14,
                        @COD_GVA150,
                        @TYP_FEX,
                        @TYP_NCEX,
                        @TYP_NDEX,
                        @COD_ACT_CNA25,
                        @COD_GVA05_ENV,
                        @COD_GVA18_ENV,
                        @RG_3572_EMPRESA_VINCULADA_CLIENTE,
                        @RG_3572_TIPO_OPERACION_HABITUAL_VENTAS,
                        @INHABILITADO_NEXO_PEDIDOS,
                        @ID_TIPO_DOCUMENTO_EXTERIOR,
                        @NUMERO_DOCUMENTO_EXTERIOR,
                        @WEB_CLIENT_ID,
                        @RG_3685_TIPO_OPERACION_VENTAS,
                        @REQUIERE_INFORMACION_ADICIONAL,
                        @NRO_INSCR_RG1817,
                        @INHABILITADO_NEXO_COBRANZAS,
                        @CODIGO_AFINIDAD,
                        @ID_TRA_ORIGEN_INFORMACION,
                        @SEXO
                        )
                    ";
                cm.Parameters.Clear();
                cm.Parameters.Add("FILLER", SqlDbType.VarChar).Value = clientesGVA14.FILLER;
                cm.Parameters.Add("@ADJUNTO", SqlDbType.VarChar).Value = clientesGVA14.ADJUNTO;
                cm.Parameters.Add("@ALI_NO_CAT", SqlDbType.Int).Value = clientesGVA14.ALI_NO_CAT;
                cm.Parameters.Add("@BMP", SqlDbType.VarChar).Value = clientesGVA14.BMP;
                cm.Parameters.Add("@C_POSTAL", SqlDbType.VarChar).Value = clientesGVA14.C_POSTAL;
                cm.Parameters.Add("@CALLE", SqlDbType.VarChar).Value = clientesGVA14.CALLE;
                cm.Parameters.Add("@CALLE2_ENV", SqlDbType.VarChar).Value = clientesGVA14.CALLE2_ENV;
                cm.Parameters.Add("@CLAUSULA", SqlDbType.Bit).Value = clientesGVA14.CLAUSULA;
                cm.Parameters.Add("@CLAVE_IS", SqlDbType.VarChar).Value = clientesGVA14.CLAVE_IS;
                cm.Parameters.Add("@COD_CLIENT", SqlDbType.VarChar).Value = clientesGVA14.COD_CLIENT;
                cm.Parameters.Add("@COD_PROVIN", SqlDbType.VarChar).Value = clientesGVA14.COD_PROVIN;
                cm.Parameters.Add("@COD_TRANSP", SqlDbType.VarChar).Value = clientesGVA14.COD_TRANSP;
                cm.Parameters.Add("@COD_VENDED", SqlDbType.VarChar).Value = clientesGVA14.COD_VENDED;
                cm.Parameters.Add("@COD_ZONA", SqlDbType.VarChar).Value = clientesGVA14.COD_ZONA;
                cm.Parameters.Add("@COND_VTA", SqlDbType.Int).Value = clientesGVA14.COND_VTA;
                cm.Parameters.Add("@CUIT", SqlDbType.VarChar).Value = clientesGVA14.CUIT;
                cm.Parameters.Add("@CUMPLEANIO", SqlDbType.DateTime).Value = clientesGVA14.CUMPLEANIO;
                cm.Parameters.Add("@CUPO_CREDI", SqlDbType.Decimal).Value = clientesGVA14.CUPO_CREDI;
                cm.Parameters.Add("@DIR_COM", SqlDbType.VarChar).Value = clientesGVA14.DIR_COM;
                cm.Parameters.Add("@DOMICILIO", SqlDbType.VarChar).Value = clientesGVA14.DOMICILIO;
                cm.Parameters.Add("@DTO_ENVIO", SqlDbType.VarChar).Value = clientesGVA14.DTO_ENVIO;
                cm.Parameters.Add("@DTO_LEGAL", SqlDbType.VarChar).Value = clientesGVA14.DTO_LEGAL;
                cm.Parameters.Add("@E_MAIL", SqlDbType.VarChar).Value = clientesGVA14.E_MAIL;
                cm.Parameters.Add("@ENV_DOMIC", SqlDbType.VarChar).Value = clientesGVA14.ENV_DOMIC;
                cm.Parameters.Add("@ENV_LOCAL", SqlDbType.VarChar).Value = clientesGVA14.ENV_LOCAL;
                cm.Parameters.Add("@ENV_POSTAL", SqlDbType.VarChar).Value = clientesGVA14.ENV_POSTAL;
                cm.Parameters.Add("@ENV_PROV", SqlDbType.VarChar).Value = clientesGVA14.ENV_PROV;
                cm.Parameters.Add("@EXPORTA", SqlDbType.Bit).Value = clientesGVA14.EXPORTA;
                cm.Parameters.Add("@FECHA_ALTA", SqlDbType.DateTime).Value = clientesGVA14.FECHA_ALTA;
                cm.Parameters.Add("@FECHA_ANT", SqlDbType.DateTime).Value = clientesGVA14.FECHA_ANT;
                cm.Parameters.Add("@FECHA_DESD", SqlDbType.DateTime).Value = clientesGVA14.FECHA_DESD;
                cm.Parameters.Add("@FECHA_HAST", SqlDbType.DateTime).Value = clientesGVA14.FECHA_HAST;
                cm.Parameters.Add("@FECHA_INHA", SqlDbType.DateTime).Value = clientesGVA14.FECHA_INHA;
                cm.Parameters.Add("@FECHA_VTO", SqlDbType.DateTime).Value = clientesGVA14.FECHA_VTO;
                cm.Parameters.Add("@GRUPO_EMPR", SqlDbType.VarChar).Value = clientesGVA14.GRUPO_EMPR;
                cm.Parameters.Add("@ID_EXTERNO", SqlDbType.VarChar).Value = clientesGVA14.ID_EXTERNO;
                cm.Parameters.Add("@ID_INTERNO", SqlDbType.VarChar).Value = clientesGVA14.ID_INTERNO;
                cm.Parameters.Add("@II_D", SqlDbType.VarChar).Value = clientesGVA14.II_D;
                cm.Parameters.Add("@II_L", SqlDbType.VarChar).Value = clientesGVA14.II_L;
                cm.Parameters.Add("@IVA_D", SqlDbType.VarChar).Value = clientesGVA14.IVA_D;
                cm.Parameters.Add("@IVA_L", SqlDbType.VarChar).Value = clientesGVA14.IVA_L;
                cm.Parameters.Add("@LOCALIDAD", SqlDbType.VarChar).Value = clientesGVA14.LOCALIDAD;
                cm.Parameters.Add("@N_IMPUESTO", SqlDbType.VarChar).Value = clientesGVA14.N_IMPUESTO;
                cm.Parameters.Add("@N_ING_BRUT", SqlDbType.VarChar).Value = clientesGVA14.N_ING_BRUT;
                cm.Parameters.Add("@NOM_COM", SqlDbType.VarChar).Value = clientesGVA14.NOM_COM;
                cm.Parameters.Add("@NRO_ENVIO", SqlDbType.VarChar).Value = clientesGVA14.NRO_ENVIO;
                cm.Parameters.Add("@NRO_LEGAL", SqlDbType.VarChar).Value = clientesGVA14.NRO_LEGAL;
                cm.Parameters.Add("@NRO_LISTA", SqlDbType.Int).Value = clientesGVA14.NRO_LISTA;
                cm.Parameters.Add("@OBSERVACIO", SqlDbType.VarChar).Value = clientesGVA14.OBSERVACIO;
                cm.Parameters.Add("@PARTIDOENV", SqlDbType.VarChar).Value = clientesGVA14.PARTIDOENV;
                cm.Parameters.Add("@PERMITE_IS", SqlDbType.Bit).Value = clientesGVA14.PERMITE_IS;
                cm.Parameters.Add("@PISO_ENVIO", SqlDbType.VarChar).Value = clientesGVA14.PISO_ENVIO;
                cm.Parameters.Add("@PISO_LEGAL", SqlDbType.VarChar).Value = clientesGVA14.PISO_LEGAL;
                cm.Parameters.Add("@PORC_DESC", SqlDbType.Decimal).Value = clientesGVA14.PORC_DESC;
                cm.Parameters.Add("@PORC_EXCL", SqlDbType.Decimal).Value = clientesGVA14.PORC_EXCL;
                cm.Parameters.Add("@PORC_RECAR", SqlDbType.Decimal).Value = clientesGVA14.PORC_RECAR;
                cm.Parameters.Add("@PUNTAJE", SqlDbType.Decimal).Value = clientesGVA14.PUNTAJE;
                cm.Parameters.Add("@RAZON_SOCI", SqlDbType.VarChar).Value = clientesGVA14.RAZON_SOCI;
                cm.Parameters.Add("@SALDO_ANT", SqlDbType.Decimal).Value = clientesGVA14.SALDO_ANT;
                cm.Parameters.Add("@SALDO_CC", SqlDbType.Decimal).Value = clientesGVA14.SALDO_CC;
                cm.Parameters.Add("@SALDO_DOC", SqlDbType.Decimal).Value = clientesGVA14.SALDO_DOC;
                cm.Parameters.Add("@SALDO_D_UN", SqlDbType.Decimal).Value = clientesGVA14.SALDO_D_UN;
                cm.Parameters.Add("@SOBRE_II", SqlDbType.VarChar).Value = clientesGVA14.SOBRE_II;
                cm.Parameters.Add("@SOBRE_IVA", SqlDbType.VarChar).Value = clientesGVA14.SOBRE_IVA;
                cm.Parameters.Add("@TELEFONO_1", SqlDbType.VarChar).Value = clientesGVA14.TELEFONO_1;
                cm.Parameters.Add("@TELEFONO_2", SqlDbType.VarChar).Value = clientesGVA14.TELEFONO_2;
                cm.Parameters.Add("@TIPO", SqlDbType.VarChar).Value = clientesGVA14.TIPO;
                cm.Parameters.Add("@TIPO_DOC", SqlDbType.Int).Value = clientesGVA14.TIPO_DOC;
                cm.Parameters.Add("@ZONA_ENVIO", SqlDbType.VarChar).Value = clientesGVA14.ZONA_ENVIO;
                cm.Parameters.Add("@FECHA_MODI", SqlDbType.DateTime).Value = clientesGVA14.FECHA_MODI;
                cm.Parameters.Add("@EXP_SALDO", SqlDbType.Bit).Value = clientesGVA14.EXP_SALDO;
                cm.Parameters.Add("@N_PAGOELEC", SqlDbType.VarChar).Value = clientesGVA14.N_PAGOELEC;
                cm.Parameters.Add("@MON_CTE", SqlDbType.Bit).Value = clientesGVA14.MON_CTE;
                cm.Parameters.Add("@SAL_AN_UN", SqlDbType.Decimal).Value = clientesGVA14.SAL_AN_UN;
                cm.Parameters.Add("@SALDO_CC_U", SqlDbType.Decimal).Value = clientesGVA14.SALDO_CC_U;
                cm.Parameters.Add("@SUCUR_ORI", SqlDbType.Int).Value = clientesGVA14.SUCUR_ORI;
                cm.Parameters.Add("@LIMCRE_EN", SqlDbType.VarChar).Value = clientesGVA14.LIMCRE_EN;
                cm.Parameters.Add("@RG_1361", SqlDbType.Bit).Value = clientesGVA14.RG_1361;
                cm.Parameters.Add("@CAL_DEB_IN", SqlDbType.Bit).Value = clientesGVA14.CAL_DEB_IN;
                cm.Parameters.Add("@PORCE_INT", SqlDbType.Decimal).Value = clientesGVA14.PORCE_INT;
                cm.Parameters.Add("@MON_MI_IN", SqlDbType.Decimal).Value = clientesGVA14.MON_MI_IN;
                cm.Parameters.Add("@DIAS_MI_IN", SqlDbType.Int).Value = clientesGVA14.DIAS_MI_IN;
                cm.Parameters.Add("@DESTINO_DE", SqlDbType.VarChar).Value = clientesGVA14.DESTINO_DE;
                cm.Parameters.Add("@CLA_IMP_CL", SqlDbType.VarChar).Value = clientesGVA14.CLA_IMP_CL;
                cm.Parameters.Add("@RECIBE_DE", SqlDbType.Bit).Value = clientesGVA14.RECIBE_DE;
                cm.Parameters.Add("@AUT_DE", SqlDbType.Bit).Value = clientesGVA14.AUT_DE;
                cm.Parameters.Add("@MAIL_DE", SqlDbType.VarChar).Value = clientesGVA14.MAIL_DE;
                cm.Parameters.Add("@WEB", SqlDbType.VarChar).Value = clientesGVA14.WEB;
                cm.Parameters.Add("@COD_RUBRO", SqlDbType.VarChar).Value = clientesGVA14.COD_RUBRO;
                cm.Parameters.Add("@CTA_CLI", SqlDbType.Decimal).Value = clientesGVA14.CTA_CLI;
                cm.Parameters.Add("@CTO_CLI", SqlDbType.VarChar).Value = clientesGVA14.CTO_CLI;
                cm.Parameters.Add("@COD_GVA14", SqlDbType.VarChar).Value = clientesGVA14.COD_GVA14;
                cm.Parameters.Add("@CBU", SqlDbType.VarChar).Value = clientesGVA14.CBU;
                cm.Parameters.Add("@IDENTIF_AFIP", SqlDbType.VarChar).Value = clientesGVA14.IDENTIF_AFIP;
                cm.Parameters.Add("@IDIOMA_CTE", SqlDbType.VarChar).Value = clientesGVA14.IDIOMA_CTE;
                cm.Parameters.Add("@DET_ARTIC", SqlDbType.VarChar).Value = clientesGVA14.DET_ARTIC;
                cm.Parameters.Add("@INC_COMENT", SqlDbType.VarChar).Value = clientesGVA14.INC_COMENT;
                cm.Parameters.Add("@ID_GVA44_FEX", SqlDbType.Int).Value = clientesGVA14.ID_GVA44_FEX;
                cm.Parameters.Add("@ID_GVA44_NCEX", SqlDbType.Int).Value = clientesGVA14.ID_GVA44_NCEX;
                cm.Parameters.Add("@ID_GVA44_NDEX", SqlDbType.Int).Value = clientesGVA14.ID_GVA44_NDEX;
                cm.Parameters.Add("@CIUDAD", SqlDbType.VarChar).Value = clientesGVA14.CIUDAD;
                cm.Parameters.Add("@CIUDAD_ENVIO", SqlDbType.VarChar).Value = clientesGVA14.CIUDAD_ENVIO;
                cm.Parameters.Add("@APLICA_MORA", SqlDbType.VarChar).Value = clientesGVA14.APLICA_MORA;
                cm.Parameters.Add("@ID_INTERES_POR_MORA", SqlDbType.Int).Value = clientesGVA14.ID_INTERES_POR_MORA;
                cm.Parameters.Add("@PUBLICA_WEB_CLIENTES", SqlDbType.VarChar).Value = clientesGVA14.PUBLICA_WEB_CLIENTES;
                cm.Parameters.Add("@MAIL_NEXO", SqlDbType.VarChar).Value = clientesGVA14.MAIL_NEXO;
                cm.Parameters.Add("@AUTORIZADO_WEB_CLIENTES", SqlDbType.VarChar).Value = clientesGVA14.AUTORIZADO_WEB_CLIENTES;
                cm.Parameters.Add("@OBSERVACIONES", SqlDbType.VarChar).Value = clientesGVA14.OBSERVACIONES;
                cm.Parameters.Add("@COD_GVA18", SqlDbType.VarChar).Value = clientesGVA14.COD_GVA18;
                cm.Parameters.Add("@COD_GVA24", SqlDbType.VarChar).Value = clientesGVA14.COD_GVA24;
                cm.Parameters.Add("@COD_GVA23", SqlDbType.VarChar).Value = clientesGVA14.COD_GVA23;
                cm.Parameters.Add("@COD_GVA05", SqlDbType.VarChar).Value = clientesGVA14.COD_GVA05;
                cm.Parameters.Add("@COD_GVA62", SqlDbType.VarChar).Value = clientesGVA14.COD_GVA62;
                cm.Parameters.Add("@COD_GVA151", SqlDbType.VarChar).Value = clientesGVA14.COD_GVA151;
                cm.Parameters.Add("@COBRA_LUNES", SqlDbType.VarChar).Value = clientesGVA14.COBRA_LUNES;
                cm.Parameters.Add("@COBRA_MARTES", SqlDbType.VarChar).Value = clientesGVA14.COBRA_MARTES;
                cm.Parameters.Add("@COBRA_MIERCOLES", SqlDbType.VarChar).Value = clientesGVA14.COBRA_MIERCOLES;
                cm.Parameters.Add("@COBRA_JUEVES", SqlDbType.VarChar).Value = clientesGVA14.COBRA_JUEVES;
                cm.Parameters.Add("@COBRA_VIERNES", SqlDbType.VarChar).Value = clientesGVA14.COBRA_VIERNES;
                cm.Parameters.Add("@COBRA_SABADO", SqlDbType.VarChar).Value = clientesGVA14.COBRA_SABADO;
                cm.Parameters.Add("@COBRA_DOMINGO", SqlDbType.VarChar).Value = clientesGVA14.COBRA_DOMINGO;
                cm.Parameters.Add("@HORARIO_COBRANZA", SqlDbType.VarChar).Value = clientesGVA14.HORARIO_COBRANZA;
                cm.Parameters.Add("@COMENTARIO_TYP_FAC", SqlDbType.VarChar).Value = clientesGVA14.COMENTARIO_TYP_FAC;
                cm.Parameters.Add("@COMENTARIO_TYP_ND", SqlDbType.VarChar).Value = clientesGVA14.COMENTARIO_TYP_ND;
                cm.Parameters.Add("@COMENTARIO_TYP_NC", SqlDbType.VarChar).Value = clientesGVA14.COMENTARIO_TYP_NC;
                cm.Parameters.Add("@TELEFONO_MOVIL", SqlDbType.VarChar).Value = clientesGVA14.TELEFONO_MOVIL;
                cm.Parameters.Add("@ID_CATEGORIA_IVA", SqlDbType.Int).Value = clientesGVA14.ID_CATEGORIA_IVA;
                cm.Parameters.Add("@ID_GVA14", SqlDbType.Int).Value = clientesGVA14.ID_GVA14;
                cm.Parameters.Add("@COD_GVA150", SqlDbType.VarChar).Value = clientesGVA14.COD_GVA150;
                cm.Parameters.Add("@TYP_FEX", SqlDbType.VarChar).Value = clientesGVA14.TYP_FEX;
                cm.Parameters.Add("@TYP_NCEX", SqlDbType.VarChar).Value = clientesGVA14.TYP_NCEX;
                cm.Parameters.Add("@TYP_NDEX", SqlDbType.VarChar).Value = clientesGVA14.TYP_NDEX;
                cm.Parameters.Add("@COD_ACT_CNA25", SqlDbType.VarChar).Value = clientesGVA14.COD_ACT_CNA25;
                cm.Parameters.Add("@COD_GVA05_ENV", SqlDbType.VarChar).Value = clientesGVA14.COD_GVA05_ENV;
                cm.Parameters.Add("@COD_GVA18_ENV", SqlDbType.VarChar).Value = clientesGVA14.COD_GVA18_ENV;
                cm.Parameters.Add("@RG_3572_EMPRESA_VINCULADA_CLIENTE", SqlDbType.Bit).Value = clientesGVA14.RG_3572_EMPRESA_VINCULADA_CLIENTE;
                cm.Parameters.Add("@RG_3572_TIPO_OPERACION_HABITUAL_VENTAS", SqlDbType.VarChar).Value = clientesGVA14.RG_3572_TIPO_OPERACION_HABITUAL_VENTAS;
                cm.Parameters.Add("@INHABILITADO_NEXO_PEDIDOS", SqlDbType.VarChar).Value = clientesGVA14.INHABILITADO_NEXO_PEDIDOS;
                cm.Parameters.Add("@ID_TIPO_DOCUMENTO_EXTERIOR", SqlDbType.Int).Value = clientesGVA14.ID_TIPO_DOCUMENTO_EXTERIOR;
                cm.Parameters.Add("@NUMERO_DOCUMENTO_EXTERIOR", SqlDbType.VarChar).Value = clientesGVA14.NUMERO_DOCUMENTO_EXTERIOR;
                cm.Parameters.Add("@WEB_CLIENT_ID", SqlDbType.Int).Value = clientesGVA14.WEB_CLIENT_ID;
                cm.Parameters.Add("@RG_3685_TIPO_OPERACION_VENTAS", SqlDbType.VarChar).Value = clientesGVA14.RG_3685_TIPO_OPERACION_VENTAS;
                cm.Parameters.Add("@REQUIERE_INFORMACION_ADICIONAL", SqlDbType.VarChar).Value = clientesGVA14.REQUIERE_INFORMACION_ADICIONAL;
                cm.Parameters.Add("@NRO_INSCR_RG1817", SqlDbType.VarChar).Value = clientesGVA14.NRO_INSCR_RG1817;
                cm.Parameters.Add("@INHABILITADO_NEXO_COBRANZAS", SqlDbType.VarChar).Value = clientesGVA14.INHABILITADO_NEXO_COBRANZAS;
                cm.Parameters.Add("@CODIGO_AFINIDAD", SqlDbType.VarChar).Value = clientesGVA14.CODIGO_AFINIDAD;
                cm.Parameters.Add("@ID_TRA_ORIGEN_INFORMACION", SqlDbType.Int).Value = clientesGVA14.ID_TRA_ORIGEN_INFORMACION;
                cm.Parameters.Add("@SEXO", SqlDbType.VarChar).Value = clientesGVA14.SEXO;

                return cm;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public SqlCommand GuardarDireccionEntrega(Entidades.DireccionEntrega direccion)
        {
            try
            {
                SqlCommand cm = new SqlCommand();

                cm.CommandText = 
                                @"INSERT INTO DIRECCION_ENTREGA
                                    (
                                    COD_DIRECCION_ENTREGA,
                                    COD_CLIENTE,
                                    DIRECCION,
                                    COD_PROVINCIA,
                                    LOCALIDAD,
                                    HABITUAL,
                                    CODIGO_POSTAL,
                                    TELEFONO1,
                                    TELEFONO2,
                                    TOMA_IMPUESTO_HABITUAL,
                                    FILLER,
                                    OBSERVACIONES,
                                    AL_FIJ_IB3,
                                    ALI_ADI_IB,
                                    ALI_FIJ_IB,
                                    IB_L,
                                    IB_L3,
                                    II_IB3,
                                    LIB,
                                    PORC_L,
                                    HABILITADO,
                                    HORARIO_ENTREGA,
                                    ENTREGA_LUNES,
                                    ENTREGA_MARTES,
                                    ENTREGA_MIERCOLES,
                                    ENTREGA_JUEVES,
                                    ENTREGA_VIERNES,
                                    ENTREGA_SABADO,
                                    ENTREGA_DOMINGO,
                                    CONSIDERA_IVA_BASE_CALCULO_IIBB,
                                    CONSIDERA_IVA_BASE_CALCULO_IIBB_ADIC,
                                    WEB_ADDRESS_ID
                                    )
                                    VALUES
                                    (
                                    @COD_DIRECCION_ENTREGA,
                                    @COD_CLIENTE,
                                    @DIRECCION,
                                    @COD_PROVINCIA,
                                    @LOCALIDAD,
                                    @HABITUAL,
                                    @CODIGO_POSTAL,
                                    @TELEFONO1,
                                    @TELEFONO2,
                                    @TOMA_IMPUESTO_HABITUAL,
                                    @FILLER,
                                    @OBSERVACIONES,
                                    @AL_FIJ_IB3,
                                    @ALI_ADI_IB,
                                    @ALI_FIJ_IB,
                                    @IB_L,
                                    @IB_L3,
                                    @II_IB3,
                                    @LIB,
                                    @PORC_L,
                                    @HABILITADO,
                                    @HORARIO_ENTREGA,
                                    @ENTREGA_LUNES,
                                    @ENTREGA_MARTES,
                                    @ENTREGA_MIERCOLES,
                                    @ENTREGA_JUEVES,
                                    @ENTREGA_VIERNES,
                                    @ENTREGA_SABADO,
                                    @ENTREGA_DOMINGO,
                                    @CONSIDERA_IVA_BASE_CALCULO_IIBB,
                                    @CONSIDERA_IVA_BASE_CALCULO_IIBB_ADIC,
                                    @WEB_ADDRESS_ID
                                    )
                                ";

                cm.Parameters.Clear();
                cm.Parameters.Add("COD_DIRECCION_ENTREGA", SqlDbType.VarChar).Value = direccion.COD_DIRECCION_ENTREGA;
                cm.Parameters.Add("@COD_CLIENTE", SqlDbType.VarChar).Value = direccion.COD_CLIENTE;
                cm.Parameters.Add("@DIRECCION", SqlDbType.VarChar).Value = direccion.DIRECCION;
                cm.Parameters.Add("@COD_PROVINCIA", SqlDbType.VarChar).Value = direccion.COD_PROVINCIA;
                cm.Parameters.Add("@LOCALIDAD", SqlDbType.VarChar).Value = direccion.LOCALIDAD;
                cm.Parameters.Add("@HABITUAL", SqlDbType.VarChar).Value = direccion.HABITUAL;
                cm.Parameters.Add("@CODIGO_POSTAL", SqlDbType.VarChar).Value = direccion.CODIGO_POSTAL;
                cm.Parameters.Add("@TELEFONO1", SqlDbType.VarChar).Value = direccion.TELEFONO1;
                cm.Parameters.Add("@TELEFONO2", SqlDbType.VarChar).Value = direccion.TELEFONO2;
                cm.Parameters.Add("@TOMA_IMPUESTO_HABITUAL", SqlDbType.VarChar).Value = direccion.TOMA_IMPUESTO_HABITUAL;
                cm.Parameters.Add("@FILLER,", SqlDbType.VarChar).Value = direccion.FILLER;
                cm.Parameters.Add("@OBSERVACIONES", SqlDbType.VarChar).Value = direccion.OBSERVACIONES;
                cm.Parameters.Add("@AL_FIJ_IB3", SqlDbType.VarChar).Value = direccion.AL_FIJ_IB3;
                cm.Parameters.Add("@ALI_ADI_IB", SqlDbType.VarChar).Value = direccion.ALI_ADI_IB;
                cm.Parameters.Add("@ALI_FIJ_IB", SqlDbType.VarChar).Value = direccion.ALI_FIJ_IB;
                cm.Parameters.Add("@IB_L", SqlDbType.VarChar).Value = direccion.IB_L;
                cm.Parameters.Add("@IB_L3", SqlDbType.VarChar).Value = direccion.IB_L3;
                cm.Parameters.Add("@II_IB3", SqlDbType.VarChar).Value = direccion.II_IB3;
                cm.Parameters.Add("@LIB", SqlDbType.VarChar).Value = direccion.LIB;
                cm.Parameters.Add("@PORC_L", SqlDbType.VarChar).Value = direccion.PORC_L;
                cm.Parameters.Add("@HABILITADO", SqlDbType.VarChar).Value = direccion.HABILITADO;
                cm.Parameters.Add("@HORARIO_ENTREGA", SqlDbType.VarChar).Value = direccion.HORARIO_ENTREGA;
                cm.Parameters.Add("@ENTREGA_LUNES", SqlDbType.VarChar).Value = direccion.ENTREGA_LUNES;
                cm.Parameters.Add("@ENTREGA_MARTES", SqlDbType.VarChar).Value = direccion.ENTREGA_MARTES;
                cm.Parameters.Add("@ENTREGA_MIERCOLES", SqlDbType.VarChar).Value = direccion.ENTREGA_MIERCOLES;
                cm.Parameters.Add("@ENTREGA_JUEVES", SqlDbType.VarChar).Value = direccion.ENTREGA_JUEVES;
                cm.Parameters.Add("@ENTREGA_VIERNES", SqlDbType.VarChar).Value = direccion.ENTREGA_VIERNES;
                cm.Parameters.Add("@ENTREGA_SABADO", SqlDbType.VarChar).Value = direccion.ENTREGA_SABADO;
                cm.Parameters.Add("@ENTREGA_DOMINGO", SqlDbType.VarChar).Value = direccion.ENTREGA_DOMINGO;
                cm.Parameters.Add("@CONSIDERA_IVA_BASE_CALCULO_IIBB", SqlDbType.VarChar).Value = direccion.CONSIDERA_IVA_BASE_CALCULO_IIBB;
                cm.Parameters.Add("@CONSIDERA_IVA_BASE_CALCULO_IIBB_ADIC", SqlDbType.VarChar).Value = direccion.CONSIDERA_IVA_BASE_CALCULO_IIBB_ADIC;
                cm.Parameters.Add("@WEB_ADDRESS_ID", SqlDbType.VarChar).Value = direccion.WEB_ADDRESS_ID;

                return cm;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public SqlCommand InsertarAsientoComprobante()
        {
            try
            {
                SqlCommand cm = new SqlCommand();
                cm.CommandText = "insert into  asiento_comprobante_gv " +
                    " ( id_asiento_comprobante_gv,ncomp_in_v, " +
                    " asiento_anulacion,contabilizado,usuario_contabilizacion,fecha_contabilizacion,transferido_cn) " +
                    " select (SELECT MAX(id_asiento_comprobante_gv) from " +
                    " ASIENTO_COMPROBANTE_GV) + ROW_NUMBER() over (order by  id_gva12),NCOMP_IN_V ,'N','N',NULL,NULL,'N' " +
                    " from gva12 " +
                    " where t_comp<>'REC' and ncomp_in_v not in (select ncomp_in_v from asiento_comprobante_gv) AND ESTADO<>'ANU'";
                return cm;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void Reset_ID()
        {
            try
            {
                SqlConnection cn = new SqlConnection(ConfigurationManager.AppSettings.Get("Conexion").ToString());
                SqlCommand cm = new SqlCommand();
                cn.Open();
                cm.CommandText = "IF EXISTS (SELECT * FROM SYS.SEQUENCES WHERE NAME = 'GVA12')EXECUTE P_UPDATESEQUENCEBYTABLENAME 'GVA12'" +
                                 "IF EXISTS (SELECT * FROM SYS.SEQUENCES WHERE NAME = 'GVA42')EXECUTE P_UPDATESEQUENCEBYTABLENAME 'GVA42'" +
                                 "IF EXISTS (SELECT * FROM SYS.SEQUENCES WHERE NAME = 'GVA53')EXECUTE P_UPDATESEQUENCEBYTABLENAME 'GVA53'" +
                                 "IF EXISTS (SELECT * FROM SYS.SEQUENCES WHERE NAME = 'GVA46')EXECUTE P_UPDATESEQUENCEBYTABLENAME 'GVA46'" +
                                 "IF EXISTS (SELECT * FROM SYS.SEQUENCES WHERE NAME = 'GVA46')EXECUTE P_UPDATESEQUENCEBYTABLENAME 'GVA14'" +
                                 "IF EXISTS (SELECT * FROM SYS.SEQUENCES WHERE NAME = 'GVA46')EXECUTE P_UPDATESEQUENCEBYTABLENAME 'SBA04'" +
                                 "IF EXISTS (SELECT * FROM SYS.SEQUENCES WHERE NAME = 'GVA46')EXECUTE P_UPDATESEQUENCEBYTABLENAME 'SBA05'" +
                                 "IF EXISTS (SELECT * FROM SYS.SEQUENCES WHERE NAME = 'GVA46')EXECUTE P_UPDATESEQUENCEBYTABLENAME 'SBA20'" +
                                 "IF EXISTS (SELECT * FROM SYS.SEQUENCES WHERE NAME = 'ASIENTO_COMPROBANTE_GV')EXECUTE P_UPDATESEQUENCEBYTABLENAME 'ASIENTO_COMPROBANTE_GV'";

                cm.Connection = cn;
                cm.ExecuteNonQuery();
                cn.Close();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
