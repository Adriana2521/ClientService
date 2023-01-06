using ClientService.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ClientService.Service
{
    public class ClienteService
    {
        public event Action<int> recibirVotos;
        private string pregunta = "";
        HttpListener listener = new();

        public ClienteService()
        {
            listener.Prefixes.Add("http://*:3607/Encuesta/");

        }


        public void iniciar()
        {
            if (!listener.IsListening)
            {
                listener.Start();
                listener.BeginGetContext(ContextR, null);
            }
        }



        public void EstablecerEncuesta(Preguntas p)
        {
            pregunta= JsonConvert.SerializeObject(p);
        }
        private void ContextR(IAsyncResult ar)
        {
            var context= listener.EndGetContext(ar);
            listener.BeginGetContext(ContextR, null);

            if (context.Request.Url !=null)
            {
                if (context.Request.Url.LocalPath=="/Encuesta/pregunta")
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(pregunta);
                    context.Response.ContentType = "application/json";
                    context.Response.OutputStream.Write(buffer, 0, buffer.Length);  
                    context.Response.StatusCode= 200;   
                    context.Response.Close();   

                }
                else if (context.Request.HttpMethod=="POST" && context.Request.Url.LocalPath=="/Encuesta/Contestar")
                {
                    var stream= new StreamReader(context.Request.InputStream);
                    var json=stream.ReadToEnd();
                    VotacionClientes? votos= JsonConvert.DeserializeObject<VotacionClientes>(json);
                    context.Response.StatusCode = 200;

                    if (votos != null) recibirVotos?.Invoke(votos.votacion);
                    context.Response.Close();   

                }
                else if (context.Request.Url.LocalPath=="/Encuesta/Contestar")
                {
                    if (context.Request.QueryString["votos"]!= null)
                    {
                        int voto = int.Parse(context.Request.QueryString["votos"]);
                        context.Response.StatusCode = 200;
                        recibirVotos?.Invoke(voto);
                        context.Response.Close();   
                    }
                    else
                    {

                        context.Response.StatusCode = 400;
                        context.Response.Close();
                    }
                }
                else
                {

                    context.Response.StatusCode = 404;
                    context.Response.Close();
                }
            }
        }


       


    }
}
