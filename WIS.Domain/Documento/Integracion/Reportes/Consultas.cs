using NLog;
using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Documento;
using WIS.Domain.Documento.Serializables;
using WIS.Domain.Documento.Serializables.Entrada;

namespace WIS.Domain.Documento.Integracion.Reportes
{
    public class Consultas
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public Consultas(IUnitOfWorkFactory uowFactory)
        {
            this._uowFactory = uowFactory;
        }

        public virtual ConsultarCantidadDocumentoResponse ConsultaCantidadDocumento(ConsultarCantidadDocumentoRequest request)
        {
            ConsultarCantidadDocumentoResponse result = new ConsultarCantidadDocumentoResponse();
            try
            {
                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    var query = new GetCantidadesDocumentoQuery(request.cdEmp, request.cdProd);

                    uow.HandleQuery(query);

                    result.totalDisponibleDocumental = query.GetCantidadDcoumentos();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                result.success = false;
                result.errorMsg = ex.Message;
                throw ex;
            }

            return result;
        }

        public virtual decimal ConsultaValidaSaldoDUA(ConsultaValidaSaldoDUARequest request)
        {
            ConsultaValidaSaldoDUAResponse result = new ConsultaValidaSaldoDUAResponse();

            try
            {
                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    var query = new DocumentosConSaldoByProducto(request.produto, request.identificador, request.faixa, request.empresa);

                    uow.HandleQuery(query);

                    return query.GetTotalSaldo();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                result.success = false;
                result.errorMsg = ex.Message;
                throw ex;
            }
        }

        public virtual DocumentoTipoResponse GetDocumentoTiposHabilitados(DocumentoTipoRequest request)
        {
            DocumentoTipoResponse response = new DocumentoTipoResponse();
            try
            {
                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    List<DocumentoTipo> listaDocumentos = uow.DocumentoTipoRepository.GetDocumentosHabilitados();
                    response.TiposDocumento = new List<DocumentoTipoFilaResponse>();
                    
                    listaDocumentos.ForEach(tipo =>
                    {
                        response.AddFila(tipo.TipoDocumento, tipo.TipoOperacion, tipo.DescripcionTipoDocumento, tipo.NumeroAutogenerado, tipo.IngresoManual);
                    });
                }
                response.Success = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                response.Success = false;
                response.ErrorMsg = ex.Message;
                throw ex;
            }

            return response;
        }

    }
}
