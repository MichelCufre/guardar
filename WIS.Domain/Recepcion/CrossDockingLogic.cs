using System;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;

namespace WIS.Domain.Recepcion
{
    public class CrossDockingLogic
    {
        public static ICrossDocking GetOrCreateCrossDocking(IUnitOfWork uow, Agenda agenda, int userId, string tpCrossDocking)
        {
            ICrossDocking crossDock = uow.CrossDockingRepository.GetCrossDockingActivoByAgenda(agenda.Id);

            if (crossDock == null)
            {
                if (tpCrossDocking == TipoCrossDockingDb.UnaFase)
                {
                    crossDock = new CrossDockingEnUnaFase
                    {
                        Agenda = agenda.Id,
                        FechaAlta = DateTime.Now,
                        Estado = EstadoCrossDockingDb.EnEdicion,
                        Tipo = tpCrossDocking,
                        Usuario = userId
                    };
                }
                else if (tpCrossDocking == TipoCrossDockingDb.SegundaFase)
                {
                    crossDock = new CrossDockingEnDosFases
                    {
                        Agenda = agenda.Id,
                        FechaAlta = DateTime.Now,
                        Estado = EstadoCrossDockingDb.EnEdicion,
                        Tipo = tpCrossDocking,
                        Usuario = userId
                    };
                }

                crossDock.AddPreparacion(uow, agenda.IdEmpresa, agenda.Predio);

                uow.CrossDockingRepository.AddCrossDocking(crossDock);
            }

            return crossDock;
        }
    }
}
