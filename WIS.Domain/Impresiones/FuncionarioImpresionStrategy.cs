using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Impresiones
{
    public class FuncionarioImpresionStrategy : IImpresionDetalleBuildingStrategy
    {
        protected readonly IEstiloTemplate _estilo;
        protected readonly List<Funcionario> _funcionarios;
        protected readonly IPrintingService _printingService;

        public FuncionarioImpresionStrategy(IEstiloTemplate estilo, 
            IPrintingService printingService,
            List<Funcionario> funcionarios)
        {
            this._estilo = estilo;
            this._funcionarios = funcionarios;
            //this._uow = uow;
            this._printingService = printingService;
        }

        public virtual List<DetalleImpresion> Generar(Impresora impresora)
        {
            TemplateImpresion template = this._estilo.GetTemplate(impresora);

            var detalles = new List<DetalleImpresion>();

            foreach (var funcionario in this._funcionarios.OrderBy(s => s.Id))
            {

                Dictionary<string, string> claves = this.GetDiccionarioInformacion(funcionario, template.EstiloEtiqueta);

                detalles.Add(new DetalleImpresion
                {
                    Contenido = template.Parse(claves),
                    Estado = _printingService.GetEstadoInicial(),
                    FechaProcesado = DateTime.Now,
                });
            }

            return detalles;
        }


        public virtual Dictionary<string, string> GetDiccionarioInformacion(Funcionario funcionario, string estiloEtiqueta)
        {

            Dictionary<string, string> claves = new Dictionary<string, string>()
                {

                    {"T_FUNCIONARIO.CD_ATIVIDADE",funcionario.Actividad?.ToString() },
                    {"T_FUNCIONARIO.CD_EQUIPO",funcionario.Equipo.ToString() },
                    {"T_FUNCIONARIO.CD_FUNCIONARIO",funcionario.Id.ToString() },
                    {"T_FUNCIONARIO.CD_IDIOMA",funcionario.Idioma },
                    {"T_FUNCIONARIO.CD_OPID",funcionario.OperadorId },
                    {"T_FUNCIONARIO.CD_SITUACAO",funcionario.Estado.ToString() },
                    {"T_FUNCIONARIO.CD_USER_ORCLE",funcionario.UsuarioOracle },
                    {"T_FUNCIONARIO.CD_USER_UNIX",funcionario.UsuarioUnix },
                    {"T_FUNCIONARIO.DS_DIR_ARCHIVOS_EXCEL",funcionario.DireccionArchivosExcel },
                    {"T_FUNCIONARIO.DS_EMAIL",funcionario.Email },
                    {"T_FUNCIONARIO.DS_FUNCAO",funcionario.Descripcion },
                    {"T_FUNCIONARIO.DT_ADDROW",funcionario.FechaInsercion.ToString() },
                    {"T_FUNCIONARIO.DT_UPDROW",funcionario.FechaModificacion.ToString() },
                    {"T_FUNCIONARIO.NM_ABREV_FUNC",funcionario.NombreAbreviado },
                    {"T_FUNCIONARIO.NM_FUNCIONARIO",funcionario.Nombre },
                    {"T_FUNCIONARIO.NU_IP_COLECTOR",funcionario.IpColector },
                    {"T_FUNCIONARIO.NU_ORDEN_TRABAJO",funcionario.OrdenTrabajo.ToString() },
                    {"T_FUNCIONARIO.NU_PTS",funcionario.Puntos?.ToString() },
                    {"T_FUNCIONARIO.QT_CARGA_HORARIA",funcionario.CargaHoraria.ToString() },

                };



            claves.Add("WIS.CD_BARRAS_FUNCIONARIO", funcionario.NombreLogin);

            return claves;
        }

    }
}
