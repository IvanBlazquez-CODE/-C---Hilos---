﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClasesSocket
{
    public delegate void NuevaConexion(String idTerminal,int pos);
    public delegate void DatosRecibidos(String datos, int pos);
    public delegate void ConexionTerminada(String idTerminal);
    public delegate void ServidorOcupado();
    public class Servidor
    {   //VARIABLES PRIVADAS
        System.Threading.Thread ProcesoServidor;
        Dictionary<int,Conversacion> Procesos;
        TcpListener _servidor;
        int sck;
        String HostIp;
        Boolean cerrado;
        //EVENTOS
        public event NuevaConexion OnNuevaConexion;
        public event DatosRecibidos OnDatosRecibidos;
        public event ConexionTerminada OnConexionTerminada;
        public event ServidorOcupado OnServidorOcupado;
        //PROPIEDADES PÚBLICAS
        public int PuertoDeEscucha
        {
            get
            {
                return sck;
            }
            set
            {
                sck = value;
            }
        }
        public String Host
        {
            get
            {
                return HostIp;
            }
            set
            {
                HostIp = value;
            }
        }

        //CONSTRUCTOR
        public Servidor(int s, String ip)
        {
            sck = s;
            HostIp = ip;
            //Instancio el servidor
            _servidor = new TcpListener(IPAddress.Parse(HostIp), s);
            //Instancio el diccionario
            Procesos = new Dictionary<int, Conversacion>();
            //Instancio el hilo principal;
            ProcesoServidor = new Thread(new ThreadStart(Escuchar));
            //Asignar a eventos funciones por defecto.
            OnNuevaConexion += (x,y) => { };
            OnDatosRecibidos += (x, y) => { };
            OnConexionTerminada += (x) => { };
            OnServidorOcupado += () => { };
        }

      

        //METODOS
        public void Start()
        {
            //Poner a la escucha _servidor
            _servidor.Start();
            //Lanzar el hilo
            ProcesoServidor.Start();
        }
        private void Escuchar()
        { try
            {
                while (!cerrado)
                {
                    
                        Conversacion c = new Conversacion();
                        c.puerto = _servidor.AcceptSocket();
                        c.cerrado = false;
                        c.proceso = new Thread(Hablar);
                    //Configurar conversación
                    if (Procesos.Count == 0)
                    {
                        int key = CalcularClave();
                        c.proceso.Start(key);
                        Procesos.Add(key, c);
                        OnNuevaConexion(c.puerto.RemoteEndPoint.ToString(), key);
                    }
                    else
                    {
                        OnServidorOcupado();
                    }
                                       
                }
            }
            catch(Exception ex)
            {

            }
            finally
            {
                //Fin servidor
            }
        }

        private void Hablar(Object pos)
        {
            Conversacion actual=null;
            try
            {   actual = Procesos[(int)pos];
                while (!actual.cerrado)
                {
                    int tam = actual.puerto.ReceiveBufferSize;
                    byte[] buff = new byte[tam];
                    actual.puerto.Receive(buff, tam, SocketFlags.None);
                    String datos = ProcesarBytes.EncodeBytes(buff);
                    //Evento datos recibidos
                    OnDatosRecibidos(datos, (int)pos);
                }
            }
            catch (Exception ex)
            {

            }
            finally
            { 
                OnConexionTerminada(actual.puerto.RemoteEndPoint.ToString());
                actual.puerto.Close();
                actual.Cerrar();
                Procesos.Remove((int)pos);
                //Conexion Terminada
                
            }
        }

        private int CalcularClave()
        {
            if (Procesos.Count == 0) return 1;
            else return Procesos.Keys.Max()+1;
        }
        public void Cerrar(int pos)
        {
            Procesos[pos].Cerrar();
            Procesos.Remove(pos);
        }

        public void Cerrar()
        {
            for(int i =0;i<Procesos.Count;i++)
                Procesos[i].Cerrar();
        }

        public void EnviarDatos(int pos, String datos)
        { byte[] bytedatos = ProcesarBytes.DecodeByte(datos);
            Procesos[pos].puerto.Send(bytedatos, datos.Length,SocketFlags.None);
        }
        public void EnviarDatos(String datos)
        {
            foreach (int i in Procesos.Keys)
            {
                EnviarDatos(i, datos);
            }

        }
    }
}
