using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Impresiones
{
    public class EquipoImpresionStrategy : IImpresionDetalleBuildingStrategy
    {
        protected readonly IEstiloTemplate _estilo;
        protected readonly List<Equipo> _equipos;
        protected readonly IUnitOfWork _uow;
        protected readonly IBarcodeService _barcodeService;
        protected readonly IPrintingService _printingService;

        public EquipoImpresionStrategy(IEstiloTemplate estilo,
            List<Equipo> equipos,
            IUnitOfWork uow,
            IPrintingService printingService,
            IBarcodeService barcodeService)
        {
            this._estilo = estilo;
            this._equipos = equipos;
            this._uow = uow;
            this._printingService = printingService;
            this._barcodeService = barcodeService;
        }

        public virtual List<DetalleImpresion> Generar(Impresora impresora)
        {
            var template = this._estilo.GetTemplate(impresora);
            var detalles = new List<DetalleImpresion>();

            foreach (var equipo in this._equipos.OrderBy(s => s.Id))
            {
                var claves = this.GetDiccionarioInformacion(equipo, template.EstiloEtiqueta);

                detalles.Add(new DetalleImpresion
                {
                    Contenido = template.Parse(claves),
                    Estado = _printingService.GetEstadoInicial(),
                    FechaProcesado = DateTime.Now,
                });
            }

            return detalles;
        }

        public virtual Dictionary<string, string> GetDiccionarioInformacion(Equipo equipo, string estiloEtiqueta)
        {
            var autoAsignado = (equipo.Herramienta?.Autoasignado ?? false) ? "S" : "N";

            var claves = new Dictionary<string, string>()
                {
                    {"T_ENDERECO_ESTOQUE.CD_ENDERECO", equipo.Ubicacion?.Id },
                    {"T_ENDERECO_ESTOQUE.NU_PREDIO", equipo.Ubicacion?.NumeroPredio},
                    {"T_ENDERECO_ESTOQUE.CD_EMPRESA", equipo.Ubicacion?.IdEmpresa.ToString() },
                    {"T_EQUIPO.CD_EQUIPO", equipo.Id.ToString() },
                    {"T_EQUIPO.CD_FERRAMENTA", equipo.CodigoHerramienta.ToString() },
                    {"T_EQUIPO.DS_EQUIPO", equipo.Descripcion },
                    {"T_EQUIPO.CD_FUNCIONARIO", equipo.CodigoFuncionario.ToString() },
                    {"T_EQUIPO.DT_UPDROW", equipo.FechaModificacion.ToString() },
                    {"T_EQUIPO.DT_ADDROW", equipo.FechaInsercion.ToString() },
                    {"T_EQUIPO.CD_APLICACAO", equipo.Aplicacion },
                    {"T_FERRAMENTA.DS_FERRAMENTA", equipo.Herramienta?.Descripcion },
                    {"T_FERRAMENTA.ID_AUTOASIGNADO", autoAsignado },
                    {"T_FERRAMENTA.CD_SITUACAO", equipo.Herramienta?.Estado.ToString() },

                };

            claves.Add("WIS.P_EQUIPO", equipo.Id.ToString());
            if (!string.IsNullOrEmpty(equipo.PosicionEquipo))
            {
                var posicion = equipo.PosicionEquipo.Replace(GeneralDb.SeparadorEquipoPosicion, "");
                claves.Add("WIS.POSICION_EQUIPO", posicion);
            }

            claves.Add("WIS.CD_BARRAS_ETIQUETA", _barcodeService.GenerateBarcode(equipo.Id.ToString(), "EQ", anexoEtiqueta: equipo.PosicionEquipo));

            return claves;
        }
    }
}
