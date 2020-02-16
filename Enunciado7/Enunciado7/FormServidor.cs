using ClasesSocket;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Enunciado7
{
    public delegate void EnviarConcatenacion(ListBox lb);
    public delegate void NuevaLinea(ListBox lb, int pos);
    public partial class FormServidor : Form
    {
        Servidor srv;
        public FormServidor()
        {
            InitializeComponent();
        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            srv = new Servidor(txtHost.Text, int.Parse(txtSocket.Text));
            srv.Start();
            srv.OnNuevaConexion += Srv_OnNuevaConexion;
            srv.OnDatosRecibidos += Srv_OnDatosRecibidos;
        }

        private void Srv_OnDatosRecibidos(string datos, int pos, DateTime fechaDatoRecibido)
        {
            lbConexiones.Items[pos] = String.Format("[{0}] en ({1},{2})", srv.GetClientIP(pos), datos, pos);
            if (srv.Procesos[pos].fechaReciboPrimerDato == null)
            {
                srv.Procesos[pos].fechaReciboPrimerDato = DateTime.Now;
            }
            else
            {
                srv.Procesos[pos].fechaReciboUltimoDato = DateTime.Now;
            }
            
        }

        private void Srv_OnNuevaConexion(string idTerminal, int pos)
        {
            NuevaLinea nl = nuevaLinea;
            lbConexiones.BeginInvoke(nl, new object[] { lbConexiones, pos });
            srv.Procesos[pos].fechaConexion = DateTime.Now;
        }
        public void nuevaLinea(ListBox lb, int pos)
        {
            lbConexiones.Items.Add(String.Format("[{0}] en (0,0)", srv.GetClientIP(pos)));
        }
        public void enviarConcatenacion(ListBox lb)
        {
            String cadena = "";
            for(int i = 0; i < lb.Items.Count; i++)
            {
                String.Concat(cadena, lb.Items[i].ToString() + ";");
            }
            srv.EnviarDatos(cadena);
        }
    }
}
