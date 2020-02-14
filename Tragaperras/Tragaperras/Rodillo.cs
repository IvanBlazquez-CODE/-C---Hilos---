using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tragaperras
{
    public delegate void CambiarPicture(PictureBox pb, Bitmap bm);

    public delegate void OnDetener(object sender, EventArgs ev);
    
    public enum Estados
    {
        canceled,
        running,
        canceling
    };
    class Rodillo
    {
        public event OnDetener AlDetener;
        Bitmap[] imagenes;
        public int index { get; set; }
        
        public Estados _current;
        public Estados current
        {
            get
            { return _current; }
        }
        PictureBox pb;
       public Rodillo(Bitmap []imgs,int inicio,PictureBox px)
        {
            AlDetener += (x, y) => {};
            pb = px;
            index = inicio;
            imagenes = imgs;
        }

        private void Girar()
        { DateTime ahora = DateTime.Now;
            float velo=100;
            Random r = new Random(DateTime.Now.Millisecond);
            int vueltas=0,vueltasmax=r.Next(0,60);
            while (current == Estados.running&&vueltas<=vueltasmax)
            {
                if ((DateTime.Now - ahora).Milliseconds >= velo )
                {
                    CambiarPicture f = CambiarPictureBox;
                    pb.BeginInvoke(f, new object[] { pb, imagenes[index] });
                    index = ++index % imagenes.Length;
                    ahora = DateTime.Now;
                    velo *= 1.01f;
                    vueltas++;
                }
            }
            _current = Estados.canceled;
            AlDetener(this, new EventArgs());
        }

        public void Start()
        {
            
            _current = Estados.running;
            Thread hilo=new Thread(Girar);
            hilo.Start();

        }
        public void Stop()
        {
            _current = Estados.canceling;
        }
        private void CambiarPictureBox(PictureBox pb, Bitmap bmp)
        {
            pb.Image = bmp;
        }
    }
}
