using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;

namespace WIS.Domain.Eventos
{

    public partial class Evento
    {
        public Evento()
        {
            //T_EVENTO_BANDEJA = new List<T_EVENTO_BANDEJA>();
            Instancias = new List<Instancia>();
            Parametros = new List<ParametroEvento>();
        }

        public int Id { get; set; }

        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        public bool EsProgramado { get; set; }

        //public ICollection<T_EVENTO_BANDEJA> T_EVENTO_BANDEJA { get; set; }

        public List<Instancia> Instancias { get; set; }

        public List<ParametroEvento> Parametros { get; set; }

        public virtual void CrearBandeja(IUnitOfWork uow, string vlSerializado)
        {
            List<Instancia> intanciasHabilitadas = this.Instancias.Where(w => w.EsHabilitado).ToList();

            if (intanciasHabilitadas.Any())
            {
                Bandeja bandeja = new Bandeja
                {
                    NU_EVENTO = this.Id,
                    VL_SEREALIZADO = vlSerializado,
                    ND_ESTADO = EstadoBandeja.EST_PEND,
                };

                uow.EventoRepository.AddBandeja(bandeja);

                foreach (var inst in intanciasHabilitadas)
                {
                    uow.EventoRepository.AddBandejaInstancia(new InstanciaBandeja()
                    {
                        NU_EVENTO_BANDEJA = bandeja.NU_EVENTO_BANDEJA,
                        NU_EVENTO_INSTANCIA = inst.Id,
                        ND_ESTADO = EstadoBandeja.EST_PEND,
                    });
                }

            }
        }
    }
}
