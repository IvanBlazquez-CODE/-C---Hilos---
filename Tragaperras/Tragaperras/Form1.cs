using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tragaperras
{ public delegate void ActivarTrackbar(TrackBar t);
    public partial class Form1 : Form
    {   
        int stopindex=0;
        PictureBox[] pictures = new PictureBox[3];
        Rodillo[] rodillos = new Rodillo[3];
        Bitmap[] imagenes = { Properties.Resources.Campana,Properties.Resources.Cereza, Properties.Resources.Diamante,
                                Properties.Resources.Fresa, Properties.Resources.Limon, Properties.Resources.Naranja,
                                Properties.Resources.Sandia, Properties.Resources.Uva, Properties.Resources._7 };
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CrearPictures();
            CrearRodillos();

        }

        private void CrearPictures()
        {
            pictures[0] = pictureBox1;
            pictures[1] = pictureBox2;
            pictures[2] = pictureBox3;

        }
        private void CrearRodillos()
        {
            Random r = new Random(DateTime.Now.Millisecond);
            for(int i=0;i<3;i++)
            {
                rodillos[i] = new Rodillo(imagenes, r.Next(0, 8), pictures[i]);
                rodillos[i].AlDetener += Rodillo_Detener;
            }

        }

        private void Rodillo_Detener(object sender, EventArgs ev)
        {
            int cont = 0;
            for(int i=0; i<3;i++)
            {
                if (rodillos[i].current==Estados.running)
                {
                    cont++;
                }
            }
            if(cont==0)
            {
                ActivarTrackbar f = EnableTrackBar;
                trackBar1.BeginInvoke(f, new object[] { trackBar1 });
                MessageBox.Show(rodillos[0].index.ToString() + rodillos[1].index.ToString() + rodillos[2].index.ToString());
            }
        }

        private void TrackBar1_ValueChanged(object sender, EventArgs e)
        {
            Random r = new Random(DateTime.Now.Millisecond);
            if (trackBar1.Value==trackBar1.Minimum)
            {
                for(int i=0;i<3;i++)
                {
                    rodillos[i].index = r.Next(0, 8);
                    rodillos[i].Start();
                }
                trackBar1.Value = 10;
                trackBar1.Enabled = false;

            }
            
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            rodillos[stopindex].Stop();
            stopindex = ++stopindex % 3;
        }

        private void EnableTrackBar(TrackBar t)
        {
            t.Enabled = true;

        }
      
    }
}
