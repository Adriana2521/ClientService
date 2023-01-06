using ClientService.Models;
using ClientService.Service;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClientService.ViewModels
{
    public enum Vistas
    {
        Encuesta,resultados
    }


    public class ClientViewModel : INotifyPropertyChanged
    {

        public Preguntas? Preguntas { get; set; }
        public Vistas vista { get; set; }
        public int V1 { get; set; }

        public int V2 { get; set; }

        public int V3 { get; set; }

         public int V4 { get; set; }

        public int V5 { get; set; }

        public int V6 { get; set; }
        public string error { get; set; } = "";
        public int Total=> V1+V2+V3+V4+V5+V6;

        public ICommand iniciarCommand { get; set; }

        ClienteService clienteService = new();

        public ClientViewModel()
        {
            vista = Vistas.Encuesta;
            iniciarCommand = new RelayCommand(iniciar);
            clienteService.recibirVotos += ClienteService_recibirVotos;
            CargarEncuesta();
        }

        private void ClienteService_recibirVotos(int obj)
        {
            switch (obj)
            {
                case 1:V1++; break;case 2:V2++; break;case 3:V3++; break; case 4:V4++; break;case 5:V5++; break;
                case 6:V6++; break;
                
            }
            Enviar();
        }

        public void CargarEncuesta()
        {
            if (File.Exists("pregunta.json"))
            {
                var json = File.ReadAllText("pregunta.json");
                Preguntas= JsonConvert.DeserializeObject<Preguntas>(json);
            }
            else
            {
                Preguntas = new Preguntas();
            }
            Enviar(nameof(Preguntas));
        }
        private void iniciar()
        {
            if (Preguntas !=null)
            {
                error = "";
               
                if (string.IsNullOrEmpty(Preguntas.opcion1)|| string.IsNullOrEmpty(Preguntas.opcion2)|| string.IsNullOrEmpty(Preguntas.opcion3)
                    || string.IsNullOrEmpty(Preguntas.opcion4)|| string.IsNullOrEmpty(Preguntas.opcion5)|| string.IsNullOrEmpty(Preguntas.opcion6))
                {
                    error = "no deje espacios vacios" + Environment.NewLine;
                }
                if (error =="")
                {
                    clienteService.EstablecerEncuesta(Preguntas);
                    clienteService.iniciar();   
                    var json= JsonConvert.SerializeObject(Preguntas);   
                    File.WriteAllText("preguntas.json",json);
                    vista = Vistas.resultados;





                }
            }
            Enviar();
        }
        void Enviar(string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));  
        }












        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
