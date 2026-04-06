using System;
using WIS.Domain.DataModel;
using WIS.Domain.General;

namespace WIS.Domain.Services.Interfaces
{
    public interface IAlmacenamientoService
    {
        SugerenciaAlmacenamiento SugerirUbicacionParaProducto(IUnitOfWork uow, string predio, int nuEtiqueta, string agrupador, string producto, decimal? faixa, string lote, decimal cantidadSeparar, decimal cantidadClasificada, decimal cantidadOriginal, DateTime? vencimiento);
        SugerenciaAlmacenamiento SugerirUbicacionParaEtiqueta(IUnitOfWork uow, string predio, int nuEtiqueta, string ubicacionInicio);
        void CancelarSugerenciaParaProducto(IUnitOfWork uow, SugerenciaAlmacenamiento sugerencia, int nuEtiqueta, string producto, decimal? faixa, string lote, decimal cantidad);
        void AprobarSugerenciaParaProducto(IUnitOfWork uow, SugerenciaAlmacenamiento sugerencia, int nuEtiqueta, string producto, decimal? faixa, string lote, decimal cantidad, DateTime? vencimiento);
        void RechazarSugerenciaParaEtiqueta(IUnitOfWork uow, SugerenciaAlmacenamiento sugerencia, string cdMotivoRechazo);
        void AprobarSugerenciaParaEtiqueta(IUnitOfWork uow, SugerenciaAlmacenamiento sugerencia);
		SugerenciaAlmacenamiento SugerirUbicacionParaReabastecer(IUnitOfWork uow, string predio, int nuEtiqueta, string producto, int cdEmpresa, decimal? faixa, string lote, DateTime? vencimiento, bool ignorarStock, decimal cantidadSeparar, decimal cantidadClasificada, decimal cantidadOriginal);
		void CancelarSugerenciaParaReabastecimiento(IUnitOfWork uow, SugerenciaAlmacenamiento sugerencia, int nuEtiqueta, string producto, int IdEmpresa, decimal? faixa, string lote, decimal cantidad);
        void AprobarSugerenciaParaReabastecimiento(IUnitOfWork uow, SugerenciaAlmacenamiento sugerencia, int numero, string producto, int IdEmpresa, decimal? faixa, DateTime? vencimiento, string lote, decimal cantidad);

    }
}
