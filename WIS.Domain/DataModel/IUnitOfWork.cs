using System;
using WIS.Data;
using WIS.Domain.DataModel.Repositories;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel
{
    public interface IUnitOfWork : ITransactionalUnit, IQueryObjectHandler<WISDB>
    {
        public void BeginTransaction(System.Data.IsolationLevel isolationLevel);
        public long GetTransactionNumber();
        public void CreateTransactionNumber(string descripcion);
        public void CreateTransactionNumber(string descripcion, string application, int userId);
        public bool IsSnapshotException(Exception ex);
        public bool AnyTransaction();
        public void EndTransaction();

        AgendaRepository AgendaRepository { get; }
        AlmacenamientoRepository AlmacenamientoRepository { get; }
        AgenteRepository AgenteRepository { get; }
        ArchivoRepository ArchivoRepository { get; }
        CrossDockingRepository CrossDockingRepository { get; }
        CargaRepository CargaRepository { get; }
        ControlDeCalidadRepository ControlDeCalidadRepository { get; }
        ConfiguracionIdiomaRepository ConfiguracionIdiomaRepository { get; }
        InventarioRepository InventarioRepository { get; }
        EmpresaRepository EmpresaRepository { get; }
        EstacionDeTrabajoRepository EstacionDeTrabajoRepository { get; }
        EtiquetaLoteRepository EtiquetaLoteRepository { get; }
        FacturacionRepository FacturacionRepository { get; }
        DespachanteRepository DespachanteRepository { get; }
        DominioRepository DominioRepository { get; }
        GridConfigRepository GridConfigRepository { get; }
        GrupoConsultaRepository GrupoConsultaRepository { get; }
        InterfazEjecDataRepository InterfazEjecDataRepository { get; }
        MonedaRepository MonedaRepository { get; }
        EtiquetaTransferenciaRepository EtiquetaTransferenciaRepository { get; }
        NcmRepository NcmRepository { get; }
        PorteriaRepository PorteriaRepository { get; }
        ProduccionRepository ProduccionRepository { get; }
        IdentificadorConsumirRepository IdentificadorConsumirRepository { get; }
        IdentificadorProducirRepository IdentificadorProducirRepository { get; }
        LineaRepository LineaRepository { get; }
        FormulaRepository FormulaRepository { get; }
        FormulaAccionRepository FormulaAccionRepository { get; }
        AccionRepository AccionRepository { get; }
        MovimientoStockBBRepository MovimientoStockBBRepository { get; }
        DocumentoRepository DocumentoRepository { get; }
        DocumentoTipoRepository DocumentoTipoRepository { get; }
        DocumentoEntradaProduccionReservaRepository DocumentoEntradaProduccionReservaRepository { get; }
        DocumentoAnulacionPreparacionReservaRepository DocumentoAnulacionPreparacionReservaRepository { get; }
        TipoDuaRepository TipoDuaRepository { get; }
        TipoReferenciaExternaRepository TipoReferenciaExternaRepository { get; }
        RegimenAduaneroRepository RegimenAduaneroRepository { get; }
        PorteriaCamionRepository PorteriaCamionRepository { get; }
        PuertaEmbarqueRepository PuertaEmbarqueRepository { get; }
        ProductoRepository ProductoRepository { get; }
        ProductoRotatividadRepository ProductoRotatividadRepository { get; }
        ProductoFamiliaRepository ProductoFamiliaRepository { get; }
        UbicacionAreaRepository UbicacionAreaRepository { get; }
        UbicacionPickingProductoRepository UbicacionPickingProductoRepository { get; }
        FuncionarioRepository FuncionarioRepository { get; }
        FileRepository FileRepository { get; }
        EquipoRepository EquipoRepository { get; }
        OndaRepository OndaRepository { get; }
        ParametroRepository ParametroRepository { get; }
        LocalizacionRepository LocalizationRepository { get; }
        PaisRepository PaisRepository { get; }
        PaisSubdivisionRepository PaisSubdivisionRepository { get; }
        PaisSubdivisionLocalidadRepository PaisSubdivisionLocalidadRepository { get; }
        ReferenciaRecepcionRepository ReferenciaRecepcionRepository { get; }
        RecepcionTipoRepository RecepcionTipoRepository { get; }
        RutaRepository RutaRepository { get; }
        StockRepository StockRepository { get; }
        StockTraceRepository StockTraceRepository { get; }
        AjusteRepository AjusteRepository { get; }
        SecurityRepository SecurityRepository { get; }
        TipoAlmacenajeSeguroRepository TipoAlmacenajeSeguroRepository { get; }
        ImpresionRepository ImpresionRepository { get; }
        TemplateImpresionRepository TemplateImpresionRepository { get; }
        EstiloEtiquetaRepository EstiloEtiquetaRepository { get; }
        EstiloContenedorRepository EstiloContenedorRepository { get; }
        CodigoBarrasRepository CodigoBarrasRepository { get; }
        TransportistaRepository TransportistaRepository { get; }
        UbicacionRepository UbicacionRepository { get; }
        UbicacionTipoRepository UbicacionTipoRepository { get; }
        UnidadMedidaRepository UnidadMedidaRepository { get; }
        ViaRepository ViaRepository { get; }
        CamionRepository CamionRepository { get; }
        CargaCamionRepository CargaCamionRepository { get; }
        PreparacionRepository PreparacionRepository { get; }
        PedidoRepository PedidoRepository { get; }
        TransaccionRepository TransaccionRepository { get; }
        ContenedorRepository ContenedorRepository { get; }
        LiberacionRepository LiberacionRepository { get; }
        LenguajeImpresionRepository LenguajeImpresionRepository { get; }
        PredioRepository PredioRepository { get; }
        EventoRepository EventoRepository { get; }
        NotificacionRepository NotificacionRepository { get; }
        DestinatarioRepository DestinatarioRepository { get; }
        InterfazRepository InterfazRepository { get; }
        ImpresoraRepository ImpresoraRepository { get; }
        ReporteRepository ReporteRepository { get; }
        EnvaseRepository EnvaseRepository { get; }
        SituacionRepository SituacionRepository { get; }
        ProductoCodigoBarraRepository ProductoCodigoBarraRepository { get; }
        VehiculoRepository VehiculoRepository { get; }
        ValidezRepository ValidezRepository { get; }
        TipoVehiculoRepository TipoVehiculoRepository { get; }
        AnulacionRepository AnulacionRepository { get; }
        ZonaUbicacionRepository ZonaUbicacionRepository { get; }
        ZonaRepository ZonaRepository { get; }
        TrackingRepository TrackingRepository { get; }
        TemplateEtiquetaRepository TemplateEtiquetaRepository { get; }
        ProductoRamoRepository ProductoRamoRepository { get; }
        CuentaContableRepository CuentaContableRepository { get; }
        CotizacionListasRepository CotizacionListasRepository { get; }
        ListaPrecioRepository ListaPrecioRepository { get; }
        EjecucionRepository EjecucionRepository { get; }
        TareaRepository TareaRepository { get; }
        InsumoManipuleoRepository InsumoManipuleoRepository { get; }
        OrdenRepository OrdenRepository { get; }
        EstrategiaRepository EstrategiaRepository { get; }
        ManejoLpnRepository ManejoLpnRepository { get; }
        AtributoRepository AtributoRepository { get; }
        UnidadTransporteRepository UnidadTransporteRepository { get; }
        EmpaquetadoPickingRepository EmpaquetadoPickingRepository { get; }
        ClaseRepository ClaseRepository { get; }
        GrupoRepository GrupoRepository { get; }
        MesaDeClasificacionRepository MesaDeClasificacionRepository { get; }
        AutomatismoRepository AutomatismoRepository { get; }
        AutomatismoEjecucionRepository AutomatismoEjecucionRepository { get; }
        AutomatismoDataRepository AutomatismoDataRepository { get; }
        AutomatismoCaracteristicaRepository AutomatismoCaracteristicaRepository { get; }
        AutomatismoInterfazRepository AutomatismoInterfazRepository { get; }
        AutomatismoPosicionRepository AutomatismoPosicionRepository { get; }
        AutomatismoProductosAsociadosRepository AutomatismoProductosAsociadosRepository { get; }
        AutomatismoPuestoRepository AutomatismoPuestoRepository { get; }
        IntegracionServicioRepository IntegracionServicioRepository { get; }
        PtlRepository PtlRepository { get; }
        ContenedorPtlRepository ContenedorPtlRepository { get; }
        EspacioProduccionRepository EspacioProduccionRepository { get; }
        IngresoProduccionRepository IngresoProduccionRepository { get; }
        CodigoMultidatoRepository CodigoMultidatoRepository { get; }
        RecorridoRepository RecorridoRepository { get; }
        ColaDeTrabajoRepository ColaDeTrabajoRepository { get; }
        PreferenciaRepository PreferenciaRepository { get; }
        FacturaRepository FacturaRepository { get; }
        TraspasoEmpresasRepository TraspasoEmpresasRepository { get; }
    }
}
