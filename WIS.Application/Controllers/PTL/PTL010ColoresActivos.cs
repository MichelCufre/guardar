using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules.Ptl;
using WIS.Domain.Automatismo.Logic;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.General.Auxiliares;
using WIS.Domain.General.Enums;
using WIS.Domain.Integracion.Client;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Extension;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Session;
using WIS.Sorting;
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.PTL
{
    public class PTL010ColoresActivos : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly IAutomatismoPtlClientService _automatismoPtlClientService;
        protected readonly ISessionAccessor _session;
        protected readonly IBarcodeService _barcodeService;
        protected readonly AutomatismoPtlClientProxy _automatismoPtlClientProxy;
        protected readonly string _entityLock;
        protected readonly PtlLogic _logicPtl;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> Sorts { get; }

        public PTL010ColoresActivos(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridValidationService gridValidationService,
            IGridService gridService,
            IAutomatismoPtlClientService automatismoPtlClientService,
            IGridExcelService excelService,
            ITrafficOfficerService concurrencyControl,
            ISessionAccessor session,
            IBarcodeService barcodeService)
        {
            GridKeys = new List<string>
            {
                "Usuario", "Color"
            };

            Sorts = new List<SortCommand> {
                new SortCommand("Usuario", SortDirection.Ascending),
            };

            _uowFactory = uowFactory;
            _identity = identity;
            _gridValidationService = gridValidationService;
            _gridService = gridService;
            _concurrencyControl = concurrencyControl;
            _excelService = excelService;
            _session = session;
            _automatismoPtlClientService = automatismoPtlClientService;
            _automatismoPtlClientProxy = new AutomatismoPtlClientProxy(automatismoPtlClientService);
            _logicPtl = new PtlLogic(barcodeService,identity);
            _entityLock = "PTL010NotificarPTL_";
            _barcodeService = barcodeService;
        }

        #region GRID EVENTS

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = true;
            context.IsRemoveEnabled = false;
            context.IsAddEnabled = false;
            context.IsCommitEnabled = true;

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton> {
                new GridButton("btnFinalizarOperacion", "PTL010_grid1_btn_FinalizarOperacion", "far fa-caret-square-down")
            }));

            return GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            var nuAutomatismo = context.GetParameter("NU_AUTOMATISMO");

            if ((string.IsNullOrEmpty(nuAutomatismo) || !int.TryParse(nuAutomatismo, out int numeroAutomatismo)))
                return grid;

            using var uow = this._uowFactory.GetUnitOfWork();
            var editableCells = new List<string> { "Contenedor" };

            var automatismo = uow.AutomatismoRepository.GetAutomatismoById(numeroAutomatismo);
            var records = _automatismoPtlClientProxy.GetColoresActivosByPtl(automatismo.ZonaUbicacion, uow);

            GridInMemoryUtils.LoadGrid(_gridService, uow, grid, context, records, Sorts, GridKeys, editableCells);

            grid.Rows.ForEach(x =>
            {
                string css = automatismo.Caracteristicas.FirstOrDefault(w => w.Codigo == "COLOR" && w.Valor == x.GetCell("Color").Value)?.ValorAuxiliar;

                x.CssClass = "ptl_" + css.Replace("#", "");

                if (x.GetCell("Preparacion").Value == "0")
                {
                    x.SetEditableCells(new List<string>());
                }
            });

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nuAutomatismo = context.GetParameter("NU_AUTOMATISMO");

            if (string.IsNullOrEmpty(nuAutomatismo) || !int.TryParse(nuAutomatismo, out int numeroAutomatismo))
                return new GridStats { Count = 0 };

            var automatismo = uow.AutomatismoRepository.GetAutomatismoById(numeroAutomatismo);
            var records = _automatismoPtlClientProxy.GetColoresActivosByPtl(automatismo.ZonaUbicacion, uow);

            return GridInMemoryUtils.FetchStats(context, uow, records);
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nuAutomatismo = context.GetParameter("NU_AUTOMATISMO");

            if (string.IsNullOrEmpty(nuAutomatismo) || !int.TryParse(nuAutomatismo, out int numeroAutomatismo))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            var automatismo = uow.AutomatismoRepository.GetAutomatismoById(numeroAutomatismo);

            var records = _automatismoPtlClientProxy.GetColoresActivosByPtl(automatismo.ZonaUbicacion, uow);

            return GridInMemoryUtils.CreateExcel(_excelService, uow, grid, context, records, Sorts, _identity.Application);
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new ColoresActivosGridValidationModule(uow, _identity, _session, _concurrencyControl, _barcodeService), grid, row, context);
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber("Grid commit");

            try
            {
                uow.BeginTransaction();

                foreach (var row in grid.Rows)
                {
                    UpdateRow(row, uow);
                }

                uow.SaveChanges();
                uow.Commit();
                context.AddSuccessNotification("PTL010ColoresActivos_Sec0_Success_ContenedorModificado");
            }
            catch (Exception e)
            {
                uow.Rollback();
                context.AddErrorNotification(e.Message);
            }

            return grid;
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext data)
        {
            if (data.ButtonId == "btnFinalizarOperacion")
            {
                var nuAutomatismo = data.GetParameter("NU_AUTOMATISMO");

                if (string.IsNullOrEmpty(nuAutomatismo) || !int.TryParse(nuAutomatismo, out int numeroAutomatismo))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                using var uow = this._uowFactory.GetUnitOfWork();

                int userId = data.Row.GetCell("UsuarioId").Value.ToNumber<int>();

                var automatismo = uow.AutomatismoRepository.GetAutomatismoById(numeroAutomatismo);
                var color = data.Row.GetCell("Color").Value;
                (ValidationsResult validationResult, bool result) = _automatismoPtlClientService.FinishOperation(automatismo.ZonaUbicacion, color, userId);

                if (validationResult != null && validationResult.Errors.Count() > 0)
                    validationResult.Errors.ForEach(x => { data.AddErrorNotification(string.Join('.', x.Messages)); });
                else
                    data.AddSuccessNotification("PTL010_form1_Info_Success");
            }

            return data;
        }

        #endregion

        #region UTILS
        public virtual void UpdateRow(GridRow row, IUnitOfWork uow)
        {
            var usuario = row.GetCell("Usuario").Value;
            var color = row.GetCell("Color").Value;
            var preparacion = int.Parse(row.GetCell("Preparacion").Value);
            var codigoBarras = row.GetCell("Contenedor").Value;
            var nuContenedorAnterior = row.GetCell("Contenedor").Old;
            var IdPtl = row.GetCell("IdPtl").Value;
            var automatismo = uow.AutomatismoRepository.GetAutomatismoById(int.Parse(IdPtl));

            (bool existeContenedor, Contenedor contenedor) = _logicPtl.ExisteContenedor(uow, codigoBarras);

            if (!existeContenedor) contenedor = CreateContenedor(uow, codigoBarras, preparacion, nuContenedorAnterior);

            else ValidatePreviousUpdate(uow, contenedor, nuContenedorAnterior, preparacion);

            var colorActivo = _automatismoPtlClientProxy.GetColoresActivosByPtl(automatismo.ZonaUbicacion, uow).FirstOrDefault(i => i.Color == color && i.Usuario == usuario);

            colorActivo.Contenedor = contenedor.Numero.ToString();
            colorActivo.IdPtl = automatismo.ZonaUbicacion;

            _automatismoPtlClientProxy.UpdateLuzByPtlColor(colorActivo);
        }

        public virtual Contenedor CreateContenedor(IUnitOfWork uow, string codigoBarras, int preparacion, string nuContenedorAnterior)
        {
            var equipo = uow.FuncionarioRepository.GetFuncionario(_identity.UserId).Equipo;

            string ubicacion = equipo != null ? uow.EquipoRepository.GetEquipo((short)equipo)?.CodigoUbicacion ?? string.Empty : string.Empty;

            _barcodeService.ValidarEtiquetaContenedor(codigoBarras, _identity.UserId, out AuxContenedor datosContenedor, out int cantidadEmpresa);

            (bool tieneContenedorPrevio, Contenedor previousContenedor) = TieneContenedorPrevio(uow, nuContenedorAnterior, preparacion);

            var contenedor = new Contenedor
            {
                Numero = datosContenedor.NuContenedor,
                NumeroPreparacion = preparacion,
                Estado = EstadoContenedor.EnPreparacion,
                SegundaFase = "N",
                Ubicacion = ubicacion,
                TipoContenedor = datosContenedor.TipoContenedor,
                NumeroTransaccion = uow.GetTransactionNumber(),
                FechaAgregado = DateTime.Now,
                CodigoSubClase = tieneContenedorPrevio ? previousContenedor.CodigoSubClase : string.Empty
            };

            uow.ContenedorPtlRepository.AddContenedor(contenedor);

            uow.SaveChanges();

            return contenedor;
        }

        public virtual void ValidatePreviousUpdate(IUnitOfWork uow, Contenedor contenedor, string nuContenedorAnterior, int preparacion)
        {
            if (contenedor.NumeroPreparacion != preparacion)
                throw new ValidationFailedException("PTL010ColoresActivos_Sec0_msg_ContenedorEnOtraPreparacion");

            if (_logicPtl.TieneMasDeUnConjuntoDeAgrupacionContenedor(uow, contenedor.NumeroPreparacion, contenedor.Numero))
                throw new ValidationFailedException("PTL010ColoresActivos_Sec0_msg_TieneMasDeUnConjuntoDeAgrupacionContenedor");

            (bool tieneContenedorPrevio, Contenedor previousContenedor) = TieneContenedorPrevio(uow, nuContenedorAnterior, preparacion);

            if (tieneContenedorPrevio) ValidarAgrupacionesYaExistentes(uow, contenedor, previousContenedor);
        }

        public virtual void ValidarAgrupacionesYaExistentes(IUnitOfWork uow, Contenedor contenedor, Contenedor previousContenedor)
        {
            var agrupacion = _logicPtl.GetAgrupacionContenedor(uow, previousContenedor.NumeroPreparacion, previousContenedor.Numero);

            if (!_logicPtl.EsCompatibleContenedorConAgrupaciones(uow, contenedor.NumeroPreparacion, contenedor.Numero, agrupacion.ComparteContenedorPicking, agrupacion.SubClase, agrupacion.Cliente))
                throw new ValidationFailedException("PTL010ColoresActivos_Sec0_msg_NoEsCompatibleContenedorConAgrupaciones");

            if (agrupacion.SubClase != contenedor.CodigoSubClase)
                throw new ValidationFailedException("PTL010ColoresActivos_Sec0_msg_TieneDiferenteSubclase");
        }

        public virtual (bool, Contenedor) TieneContenedorPrevio(IUnitOfWork uow, string nuContenedorAnterior, int preparacion)
        {
            if (int.TryParse(nuContenedorAnterior, out int numeroContenedorAnterior))
            {
                var previousContenedor = uow.ContenedorRepository.GetContenedor(preparacion, numeroContenedorAnterior);
                if (previousContenedor != null)
                    return (true, previousContenedor);
            }

            return (false, null);
        }

        #endregion

    }
}
