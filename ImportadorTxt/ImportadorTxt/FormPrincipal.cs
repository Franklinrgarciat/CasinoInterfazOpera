using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Negocios;

namespace ImportadorTxt
{
    public partial class FormPrincipal : Form
    {
        private string SeleccioneArchivo = "Seleccione un archivo";
        public FormPrincipal()
        {
            InitializeComponent();
            txt_ruta_archivo1.Text = SeleccioneArchivo;
            txt_ruta_archivo2.Text = SeleccioneArchivo;
            txt_ruta_archivo3.Text = SeleccioneArchivo;
            txt_ruta_archivo4.Text = SeleccioneArchivo;
            btn_guardarDatos.Enabled = false;
            btn_buscar1.Select();
        }

        public void LogErrores(Exception ex)
        {
            using (StreamWriter oLog = new System.IO.StreamWriter(Application.StartupPath + "\\Errores.log", true))
            {
                oLog.WriteLine(DateTime.Now.ToString("dd-MM-yyy HH:mm") + " - " + ex.Message);
            }
        }
        private string BusquedaDeArchivos()
        {
            try
            {
                OpenFileDialog dialg = new OpenFileDialog();
                dialg.Filter = "Txt File | *.txt;";
                dialg.Title = "Importar Archivo";
                string ruta = "";
                if (dialg.ShowDialog() == DialogResult.OK)
                {
                    ruta = dialg.FileName;
                }
                return ruta;
            }
            catch (Exception ex)
            {
                LogErrores(ex);
                throw;
            }
        }



        private void btn_buscar1_Click(object sender, EventArgs e)
        {
            string ruta = BusquedaDeArchivos();
            txt_ruta_archivo1.Text = ruta;
            btn_buscar2.Select();
        }

        private void btn_buscar2_Click(object sender, EventArgs e)
        {
            string ruta = BusquedaDeArchivos();
            txt_ruta_archivo2.Text = ruta;
            btn_buscar3.Select();
        }

        private void btn_buscar3_Click(object sender, EventArgs e)
        {
            string ruta = BusquedaDeArchivos();
            txt_ruta_archivo3.Text = ruta;
            btn_guardarDatos.Enabled = true;
            btn_buscar4.Select();
        }

        private void btn_buscar4_Click(object sender, EventArgs e)
        {
            string ruta = BusquedaDeArchivos();
            txt_ruta_archivo4.Text = ruta;
        }

        private void btn_guardarDatos_Click(object sender, EventArgs e)
        {
            try
            {
                bool GuardoConExito = false;
                if (File.Exists(txt_ruta_archivo1.Text) && File.Exists(txt_ruta_archivo2.Text)
                    && File.Exists(txt_ruta_archivo3.Text))
                {
                    Negocios.NegGuardarDatos save = new Negocios.NegGuardarDatos();
                    if (File.Exists(txt_ruta_archivo4.Text))
                    {
                        GuardoConExito = save.GuardarEnBD(txt_ruta_archivo1.Text, txt_ruta_archivo2.Text, txt_ruta_archivo3.Text, txt_ruta_archivo4.Text);
                    }
                    else
                    {
                        GuardoConExito = save.GuardarEnBD(txt_ruta_archivo1.Text, txt_ruta_archivo2.Text, txt_ruta_archivo3.Text, "");
                    }
                    if (GuardoConExito)
                    {
                        MessageBox.Show("El registro se completo corectamente!!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("ERROR!! No se pudo completar el registro", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("ERROR!! El archivo seleccionado no existe", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
