using System.Collections.Generic;
using System.Linq;
using WIS.Domain.Documento;
using WIS.Domain.Documento.Constants;
using WIS.Domain.Documento.Integracion.Egreso;
using WIS.Domain.Documento.Serializables;
using WIS.Domain.Documento.Serializables.Salida;
using WIS.Domain.Expedicion;
using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers.Documento
{
	public class DocumentoMapper : Mapper
    {
        public virtual IDocumentoIngreso MapToIngreso(T_DOCUMENTO documentoEntity, IFactoryService service)
        {
            if (documentoEntity == null)
                return null;

            IDocumentoIngreso documento = service.CreateDocumentoIngreso(documentoEntity.TP_DOCUMENTO);

            SetPropertiesDocumento(documento, documentoEntity);

            documento.Estado = documentoEntity.ID_ESTADO;
            documento.Balanceado = MapStringToBoolean(documentoEntity.FL_BALANCEADO);

            return documento;
        }

        public virtual IDocumentoEgreso MapToEgreso(T_DOCUMENTO documentoEntity, IFactoryService service)
        {
            if (documentoEntity == null)
                return null;

            IDocumentoEgreso documento = service.CreateDocumentoEgreso(documentoEntity.TP_DOCUMENTO);

            SetPropertiesDocumento(documento, documentoEntity);

            foreach (var lineaEgreso in documentoEntity.T_DET_DOCUMENTO_EGRESO)
            {
                documento.OutDetail.Add(MapToDocumentoLineaEgreso(lineaEgreso, service));
            }

            documento.Estado = documentoEntity.ID_ESTADO;

            return documento;
        }

        public virtual IDocumentoActa MapToActa(T_DOCUMENTO documentoEntity, IFactoryService service)
        {
            IDocumentoActa documento = service.CreateDocumentoActa(documentoEntity.TP_DOCUMENTO);

            SetPropertiesDocumento(documento, documentoEntity);

            foreach (var lineaEgreso in documentoEntity.T_DET_DOCUMENTO_EGRESO)
            {
                documento.OutDetail.Add(MapToDocumentoLineaEgreso(lineaEgreso, service));
            }

            foreach (var lineaActa in documentoEntity.T_DET_DOCUMENTO_ACTA)
            {
                documento.ActaDetail.Add(MapToDocumentoLineaActa(lineaActa));
            }

            documento.Estado = documentoEntity.ID_ESTADO;

            return documento;
        }

        public virtual IDocumentoActa MapToActaStock(T_DOCUMENTO documentoEntity, IFactoryService service)
        {
            IDocumentoActa documento = service.CreateDocumentoActa(documentoEntity.TP_DOCUMENTO);

            SetPropertiesDocumento(documento, documentoEntity);

            foreach (var lineaEgreso in documentoEntity.T_DET_DOCUMENTO_EGRESO)
            {
                documento.OutDetail.Add(MapToDocumentoLineaEgreso(lineaEgreso, service));
            }

            foreach (var lineaActa in documentoEntity.T_DET_DOCUMENTO_ACTA)
            {
                documento.ActaDetail.Add(MapToDocumentoLineaActa(lineaActa));
            }

            documento.Estado = documentoEntity.ID_ESTADO;

            return documento;
        }

        public virtual IDocumentoAgrupador MapToDocumentoAgrupador(T_DOCUMENTO_AGRUPADOR agrupadorEntity, IFactoryService service)
        {
            IDocumentoAgrupador agrupador = service.CreateDocumentoAgrupador(agrupadorEntity.T_DOCUMENTO_AGRUPADOR_TIPO.TP_AGRUPADOR);

            SetPropertiesDocumentoAgrupador(agrupador, agrupadorEntity);

            return agrupador;
        }

        public virtual T_DOCUMENTO_AGRUPADOR MapFromDocumentoAgrupador(IDocumentoAgrupador agrupador)
        {
            return CreateEntityDocumentoAgrupador(agrupador);
        }

        public virtual DocumentoAgrupadorTipo MapToDocumentoAgrupadorTipo(T_DOCUMENTO_AGRUPADOR_TIPO agrupadorTipoEntity)
        {
            DocumentoAgrupadorTipo agrupadorTipo = new DocumentoAgrupadorTipo();
            SetPropertiesDocumentoAgrupadorTipo(agrupadorTipo, agrupadorTipoEntity);

            foreach (var grupo in agrupadorTipoEntity.T_DOCUMENTO_AGRUPADOR_GRUPO)
            {
                agrupadorTipo.Grupos.Add(new DocumentoAgrupadorGrupo()
                {
                    TipoDocumento = grupo.TP_DOCUMENTO
                });
            }

            return agrupadorTipo;
        }

        public virtual CuentaCorriente MapEntityToObject(T_PRDC_CUENTA_CORRIENTE_INSUMO en)
        {
            CuentaCorriente cuenta = new CuentaCorriente();
            cuenta.NU_NIVEL = en.NU_NIVEL;
            cuenta.NU_IDENTIFICADOR = en.NU_IDENTIFICADOR?.Trim()?.ToUpper();
            cuenta.CD_FAIXA = en.CD_FAIXA;
            cuenta.CD_PRODUTO = en.CD_PRODUTO;
            cuenta.CD_EMPRESA = en.CD_EMPRESA;
            cuenta.NU_DOCUMENTO_EGRESO = en.NU_DOCUMENTO_EGRESO;
            cuenta.CD_PRODUTO_PRODUCIDO = en.CD_PRODUTO_PRODUCIDO;
            cuenta.NU_DOCUMENTO_EGRESO_PRDC = en.NU_DOCUMENTO_EGRESO_PRDC;
            cuenta.TP_DOCUMENTO_EGRESO_PRDC = en.TP_DOCUMENTO_EGRESO_PRDC;
            cuenta.TP_DOCUMENTO_INGRESO = en.TP_DOCUMENTO_INGRESO;
            cuenta.NU_DOCUMENTO_INGRESO = en.NU_DOCUMENTO_INGRESO;
            cuenta.TP_DOCUMENTO_INGRESO_ORIGINAL = en.TP_DOCUMENTO_INGRESO_ORIGINAL;
            cuenta.NU_DOCUMENTO_INGRESO_ORIGINAL = en.NU_DOCUMENTO_INGRESO_ORIGINAL;
            cuenta.TP_DOCUMENTO_EGRESO = en.TP_DOCUMENTO_EGRESO;
            cuenta.QT_MOVIMIENTO = en.QT_MOVIMIENTO;
            cuenta.QT_DECLARADA_ORIGINAL = en.QT_DECLARADA_ORIGINAL;
            cuenta.NU_DOCUMENTO_CAMBIO = en.NU_DOCUMENTO_CAMBIO;
            cuenta.TP_DOCUMENTO_CAMBIO = en.TP_DOCUMENTO_CAMBIO;
            cuenta.DT_UPDROW = en.DT_UPDROW;
            cuenta.CD_FUNCIONARIO = en.CD_FUNCIONARIO;
            return cuenta;
        }

        public virtual T_PRDC_CUENTA_CORRIENTE_INSUMO MapObjectToEntity(CuentaCorriente en)
        {
            T_PRDC_CUENTA_CORRIENTE_INSUMO cuenta = new T_PRDC_CUENTA_CORRIENTE_INSUMO();
            cuenta.NU_NIVEL = en.NU_NIVEL;
            cuenta.NU_IDENTIFICADOR = en.NU_IDENTIFICADOR?.Trim()?.ToUpper();
            cuenta.CD_FAIXA = en.CD_FAIXA;
            cuenta.CD_PRODUTO = en.CD_PRODUTO;
            cuenta.CD_EMPRESA = en.CD_EMPRESA;
            cuenta.NU_DOCUMENTO_EGRESO = en.NU_DOCUMENTO_EGRESO;
            cuenta.CD_PRODUTO_PRODUCIDO = en.CD_PRODUTO_PRODUCIDO;
            cuenta.NU_DOCUMENTO_EGRESO_PRDC = en.NU_DOCUMENTO_EGRESO_PRDC;
            cuenta.TP_DOCUMENTO_EGRESO_PRDC = en.TP_DOCUMENTO_EGRESO_PRDC;
            cuenta.TP_DOCUMENTO_INGRESO = en.TP_DOCUMENTO_INGRESO;
            cuenta.NU_DOCUMENTO_INGRESO = en.NU_DOCUMENTO_INGRESO;
            cuenta.TP_DOCUMENTO_INGRESO_ORIGINAL = en.TP_DOCUMENTO_INGRESO_ORIGINAL;
            cuenta.NU_DOCUMENTO_INGRESO_ORIGINAL = en.NU_DOCUMENTO_INGRESO_ORIGINAL;
            cuenta.TP_DOCUMENTO_EGRESO = en.TP_DOCUMENTO_EGRESO;
            cuenta.QT_MOVIMIENTO = en.QT_MOVIMIENTO;
            cuenta.QT_DECLARADA_ORIGINAL = en.QT_DECLARADA_ORIGINAL;
            cuenta.NU_DOCUMENTO_CAMBIO = en.NU_DOCUMENTO_CAMBIO;
            cuenta.TP_DOCUMENTO_CAMBIO = en.TP_DOCUMENTO_CAMBIO;
            cuenta.DT_UPDROW = en.DT_UPDROW;
            cuenta.CD_FUNCIONARIO = en.CD_FUNCIONARIO;
            return cuenta;
        }

        public virtual DocumentoPreparacionReserva MapToDocumentoReserva(T_DOCUMENTO_PREPARACION_RESERV reservaEntity, IFactoryService service)
        {
            DocumentoPreparacionReserva documentoReserva = new DocumentoPreparacionReserva();
            SetPropertiesDocumentoReserva(documentoReserva, reservaEntity, service);

            return documentoReserva;
        }

        public virtual DocumentoPreparacionReserva MapToDocumentoReservaDesafectada(LT_DELETE_DOCUMENTO_PREPARACION_RESERV reservaEntity, IFactoryService service)
        {
            DocumentoPreparacionReserva documentoReserva = new DocumentoPreparacionReserva();
            SetPropertiesDocumentoReservaDesafectada(documentoReserva, reservaEntity, service);

            return documentoReserva;
        }

        public virtual curDocProduccion MapEntityToObject(V_PRDC_SALDO_INGRESADO doc)
        {
            curDocProduccion curDocProduccion = new curDocProduccion();
            curDocProduccion.NU_PRDC_INGRESO = doc.NU_PRDC_INGRESO;
            curDocProduccion.NU_DOCUMENTO_EGR = doc.NU_DOCUMENTO_EGR;
            curDocProduccion.TP_DOCUMENTO_EGR = doc.TP_DOCUMENTO_EGR;
            curDocProduccion.QT_INGRESADO = doc.QT_INGRESADO;
            return curDocProduccion;
        }

        public virtual DocumentoPreparacionReserva MapToDocumentoReserva(V_DET_DOCUMENTO_RESERVA reservaEntity)
        {
            DocumentoPreparacionReserva documentoReserva = new DocumentoPreparacionReserva();
            SetPropertiesDocumentoReserva(documentoReserva, reservaEntity);

            return documentoReserva;
        }

        public virtual List<PrdcSaldo> MapEntityToObject(List<V_PRDC_SALDO_CC> listaEnti)
        {
            List<PrdcSaldo> lista = new List<PrdcSaldo>();
            foreach (var curDetEP in listaEnti)
            {
                PrdcSaldo detalle = new PrdcSaldo();
                detalle.TP_DOCUMENTO_INGRESO = curDetEP.TP_DOCUMENTO_INGRESO;
                detalle.NU_DOCUMENTO_INGRESO = curDetEP.NU_DOCUMENTO_INGRESO;
                detalle.NU_IDENTIFICADOR = curDetEP.NU_IDENTIFICADOR;
                detalle.QT_SALDO = curDetEP.QT_SALDO;
                lista.Add(detalle);
            }

            return lista;
        }

        public virtual DatoVistaDoc401 MapEntityToObject(V_CAMBIO_DOC_DOC401 entity)
        {
            DatoVistaDoc401 new_Obj = new DatoVistaDoc401();
            new_Obj.CD_PRODUTO = entity.CD_PRODUTO;
            new_Obj.CD_FAIXA = entity.CD_FAIXA;
            new_Obj.NU_IDENTIFICADOR = entity.NU_IDENTIFICADOR;
            new_Obj.CD_EMPRESA = entity.CD_EMPRESA;
            new_Obj.NU_DOCUMENTO_INGRESO = entity.NU_DOCUMENTO_INGRESO;
            new_Obj.TP_DOCUMENTO_INGRESSO = entity.TP_DOCUMENTO_INGRESSO;
            new_Obj.QT_INGRESADA = entity.QT_INGRESADA;
            new_Obj.QT_RESERVADA = entity.QT_RESERVADA;
            new_Obj.QT_DESAFECTADA = entity.QT_DESAFECTADA;
            new_Obj.QT_SALDO = entity.QT_SALDO;
            new_Obj.NU_DOC = entity.NU_DOC;
            new_Obj.QT_MOVIMIENTO = entity.QT_MOVIMIENTO;
            new_Obj.QT_NACIONALIZADA = entity.QT_NACIONALIZADA;
            new_Obj.QT_EXPEDIDO = entity.QT_EXPEDIDO;
            new_Obj.TP_DOCUMENTO_CAMBIO = entity.TP_DOCUMENTO_CAMBIO;
            return new_Obj;
        }

        public virtual void SetPropertiesDocumentoReserva(DocumentoPreparacionReserva documentoReserva, V_DET_DOCUMENTO_RESERVA documentoReservaEntity)
        {
            documentoReserva.CantidadAnular = 0;
            documentoReserva.CantidadProducto = documentoReservaEntity.QT_RESERVADA;
            documentoReserva.Empresa = documentoReservaEntity.CD_EMPRESA;
            documentoReserva.EspecificaIdentificador = true;
            documentoReserva.Faixa = documentoReservaEntity.CD_FAIXA;
            documentoReserva.Identificador = documentoReservaEntity.NU_IDENTIFICADOR;
            documentoReserva.NroDocumento = documentoReservaEntity.NU_DOCUMENTO;
            documentoReserva.NroIdentificadorPicking = documentoReservaEntity.NU_IDENTIFICADOR;
            documentoReserva.Preparacion = -1;
            documentoReserva.Producto = documentoReservaEntity.CD_PRODUTO;
            documentoReserva.TipoDocumento = documentoReservaEntity.TP_DOCUMENTO;
        }

        public virtual DocumentoProduccion MapToDocumentoProduccion(T_DOCUMENTO_PRODUCCION documentoProduccionEntity, IFactoryService service)
        {
            DocumentoProduccion documentoProduccion = new DocumentoProduccion();
            SetPropertiesDocumentoProduccion(documentoProduccion, documentoProduccionEntity, service);

            return documentoProduccion;
        }

        public virtual DocumentoTransferencia MapToDocumentoTransferencia(T_DOCUMENTO_TRANSFERENCIA documentoTransferenciaEntity, IFactoryService service)
        {
            DocumentoTransferencia documentoTransferencia = new DocumentoTransferencia();
            SetPropertiesDocumentoTransferencia(documentoTransferencia, documentoTransferenciaEntity, service);

            return documentoTransferencia;
        }

        public virtual T_DOCUMENTO MapFromDocumento(IDocumento documento)
        {
            return CreateEntityDocumentoWithDetail(documento);
        }

        public virtual T_DOCUMENTO MapFromIngresoWithDetail(IDocumentoIngreso documento)
        {
            T_DOCUMENTO documentoEntity = CreateEntityDocumentoWithDetail(documento);

            documentoEntity.ID_ESTADO = documento.Estado;

            return documentoEntity;
        }

        public virtual T_DOCUMENTO MapFromIngreso(IDocumentoIngreso documento)
        {
            T_DOCUMENTO documentoEntity = CreateEntityDocumento(documento);

            documentoEntity.ID_ESTADO = documento.Estado;
            documentoEntity.FL_BALANCEADO = MapBooleanToString(documento.Balanceado);
            documentoEntity.VL_VALIDADO = documento.Validado ? "S" : "N";

            return documentoEntity;
        }

        public virtual T_DOCUMENTO MapFromEgreso(IDocumentoEgreso documento)
        {
            T_DOCUMENTO documentoEntity = CreateEntityDocumentoWithDetail(documento);

            if (documento.OutDetail.Any())
                documentoEntity.T_DET_DOCUMENTO_EGRESO = new List<T_DET_DOCUMENTO_EGRESO>();

            foreach (var lineaEgreso in documento.OutDetail)
            {
                documentoEntity.T_DET_DOCUMENTO_EGRESO.Add(MapFromDocumentoLineaEgreso(documento.Numero, documento.Tipo, lineaEgreso));
            }

            documentoEntity.ID_ESTADO = documento.Estado;

            return documentoEntity;
        }

        public virtual T_DOCUMENTO MapDocEgresoSinDetalle(IDocumentoEgreso documento)
        {
            T_DOCUMENTO documentoEntity = CreateEntityDocumentoWithDetail(documento);

            if (documento.OutDetail.Any())
                documentoEntity.T_DET_DOCUMENTO_EGRESO = new List<T_DET_DOCUMENTO_EGRESO>();

            foreach (var lineaEgreso in documento.OutDetail)
            {
                documentoEntity.T_DET_DOCUMENTO_EGRESO.Add(MapFromDocumentoLineaEgreso(documento.Numero, documento.Tipo, lineaEgreso));
            }

            documentoEntity.ID_ESTADO = documento.Estado;

            return documentoEntity;
        }

        public virtual LT_CAMBIO_DOCUMENTO_DET MapFromObjEnt(CambioDocDet doc)
        {
            LT_CAMBIO_DOCUMENTO_DET enty = new LT_CAMBIO_DOCUMENTO_DET();
            enty.CD_PRODUTO = doc.CD_PRODUTO;
            enty.CD_FAIXA = doc.CD_FAIXA;
            enty.NU_IDENTIFICADOR = doc.NU_IDENTIFICADOR?.Trim()?.ToUpper();
            enty.CD_EMPRESA = doc.CD_EMPRESA;
            enty.NU_DOCUMENTO = doc.NU_DOCUMENTO;
            enty.TP_DOCUMENTO = doc.TP_DOCUMENTO;
            enty.NU_DOCUMENTO_CAMBIO = doc.NU_DOCUMENTO_CAMBIO;
            enty.TP_DOCUMENTO_CAMBIO = doc.TP_DOCUMENTO_CAMBIO;
            return enty;
        }

        public virtual T_CAMBIO_DOCUMENTO_DET MapObjectToEntity(CambioDocInt new_insumo, int sec)
        {
            T_CAMBIO_DOCUMENTO_DET new_reg = new T_CAMBIO_DOCUMENTO_DET();
            new_reg.NU_INT = sec;
            new_reg.ID_PROCESADO = "C";
            new_reg.CD_PRODUTO = new_insumo.CD_PRODUTO;
            new_reg.CD_FAIXA = new_insumo.CD_FAIXA;
            new_reg.NU_IDENTIFICADOR = new_insumo.NU_IDENTIFICADOR?.Trim()?.ToUpper();
            new_reg.CD_EMPRESA = new_insumo.CD_EMPRESA;
            new_reg.NU_DOCUMENTO = new_insumo.NU_DOCUMENTO;
            new_reg.TP_DOCUMENTO = new_insumo.TP_DOCUMENTO;
            new_reg.NU_DOCUMENTO_CAMBIO = new_insumo.NU_DOCUMENTO_CAMBIO;
            new_reg.TP_DOCUMENTO_CAMBIO = new_insumo.TP_DOCUMENTO_CAMBIO;
            new_reg.QT_CAMBIO = new_insumo.QT_CAMBIO;
            return new_reg;
        }

        public virtual T_DOCUMENTO MapFromActa(IDocumentoActa documento)
        {
            T_DOCUMENTO documentoEntity = CreateEntityDocumentoWithDetail(documento);

            foreach (var lineaEgreso in documento.OutDetail)
            {
                documentoEntity.T_DET_DOCUMENTO_EGRESO.Add(MapFromDocumentoLineaEgreso(documento.Numero, documento.Tipo, lineaEgreso));
            }

            foreach (var lineaActa in documento.ActaDetail)
            {
                documentoEntity.T_DET_DOCUMENTO_ACTA.Add(MapFromDocumentoActaDetalle(documento.Numero, documento.Tipo, lineaActa));
            }

            documentoEntity.ID_ESTADO = documento.Estado;

            return documentoEntity;
        }

        public virtual T_DOCUMENTO MapFromActaSinDetalle(IDocumentoActa documento)
        {
            T_DOCUMENTO documentoEntity = CreateEntityDocumento(documento);
            documentoEntity.ID_ESTADO = documento.Estado;

            return documentoEntity;
        }

        public virtual T_DOCUMENTO_PREPARACION_RESERV MapFromPreparacionReserva(DocumentoPreparacionReserva preparacionReserva)
        {
            T_DOCUMENTO_PREPARACION_RESERV preparacionReservaEntity = CreateEntityPreparacionReserva(preparacionReserva);

            return preparacionReservaEntity;
        }

        public virtual LT_DELETE_DOCUMENTO_PREPARACION_RESERV MapFromPreparacionReservaDesafectada(DocumentoPreparacionReserva preparacionReserva)
        {
            LT_DELETE_DOCUMENTO_PREPARACION_RESERV preparacionReservaDesafectadaEntity = CreateEntityPreparacionReservaDesafectada(preparacionReserva);

            return preparacionReservaDesafectadaEntity;
        }

        public virtual CuentaCorrienteCambioDoc MapEntityToObject(T_PRDC_CUENTA_CAMBIO_DOC en)
        {
            CuentaCorrienteCambioDoc cuenta = new CuentaCorrienteCambioDoc();
            cuenta.NU_NIVEL = en.NU_NIVEL;
            cuenta.NU_IDENTIFICADOR = en.NU_IDENTIFICADOR?.Trim()?.ToUpper();
            cuenta.CD_FAIXA = en.CD_FAIXA;
            cuenta.CD_PRODUTO = en.CD_PRODUTO;
            cuenta.CD_EMPRESA = en.CD_EMPRESA;
            cuenta.NU_DOCUMENTO_EGRESO = en.NU_DOCUMENTO_EGRESO;
            cuenta.CD_PRODUTO_PRODUCIDO = en.CD_PRODUTO_PRODUCIDO;
            cuenta.NU_DOCUMENTO_EGRESO_PRDC = en.NU_DOCUMENTO_EGRESO_PRDC;
            cuenta.TP_DOCUMENTO_EGRESO_PRDC = en.TP_DOCUMENTO_EGRESO_PRDC;
            cuenta.TP_DOCUMENTO_INGRESO = en.TP_DOCUMENTO_INGRESO;
            cuenta.NU_DOCUMENTO_INGRESO = en.NU_DOCUMENTO_INGRESO;
            cuenta.TP_DOCUMENTO_INGRESO_ORIGINAL = en.TP_DOCUMENTO_INGRESO_ORIGINAL;
            cuenta.NU_DOCUMENTO_INGRESO_ORIGINAL = en.NU_DOCUMENTO_INGRESO_ORIGINAL;
            cuenta.QT_MOVIMIENTO = en.QT_MOVIMIENTO;
            cuenta.QT_DECLARADA_ORIGINAL = en.QT_DECLARADA_ORIGINAL;
            cuenta.NU_DOCUMENTO_CAMBIO = en.NU_DOCUMENTO_CAMBIO;
            cuenta.TP_DOCUMENTO_CAMBIO = en.TP_DOCUMENTO_CAMBIO;
            cuenta.DT_UPDROW = en.DT_UPDROW;
            cuenta.DT_ADDROW = en.DT_ADDROW;
            cuenta.CD_FUNCIONARIO = en.CD_FUNCIONARIO;
            return cuenta;
        }

        public virtual T_PRDC_CUENTA_CAMBIO_DOC MapObjectToEntity(CuentaCorrienteCambioDoc en)
        {
            T_PRDC_CUENTA_CAMBIO_DOC cuenta = new T_PRDC_CUENTA_CAMBIO_DOC();
            cuenta.NU_NIVEL = en.NU_NIVEL;
            cuenta.NU_IDENTIFICADOR = en.NU_IDENTIFICADOR?.Trim()?.ToUpper();
            cuenta.CD_FAIXA = en.CD_FAIXA;
            cuenta.CD_PRODUTO = en.CD_PRODUTO;
            cuenta.CD_EMPRESA = en.CD_EMPRESA;
            cuenta.NU_DOCUMENTO_EGRESO = en.NU_DOCUMENTO_EGRESO;
            cuenta.CD_PRODUTO_PRODUCIDO = en.CD_PRODUTO_PRODUCIDO;
            cuenta.NU_DOCUMENTO_EGRESO_PRDC = en.NU_DOCUMENTO_EGRESO_PRDC;
            cuenta.TP_DOCUMENTO_EGRESO_PRDC = en.TP_DOCUMENTO_EGRESO_PRDC;
            cuenta.TP_DOCUMENTO_INGRESO = en.TP_DOCUMENTO_INGRESO;
            cuenta.NU_DOCUMENTO_INGRESO = en.NU_DOCUMENTO_INGRESO;
            cuenta.TP_DOCUMENTO_INGRESO_ORIGINAL = en.TP_DOCUMENTO_INGRESO_ORIGINAL;
            cuenta.NU_DOCUMENTO_INGRESO_ORIGINAL = en.NU_DOCUMENTO_INGRESO_ORIGINAL;
            cuenta.QT_MOVIMIENTO = en.QT_MOVIMIENTO;
            cuenta.QT_DECLARADA_ORIGINAL = en.QT_DECLARADA_ORIGINAL;
            cuenta.NU_DOCUMENTO_CAMBIO = en.NU_DOCUMENTO_CAMBIO;
            cuenta.TP_DOCUMENTO_CAMBIO = en.TP_DOCUMENTO_CAMBIO;
            cuenta.DT_UPDROW = en.DT_UPDROW;
            cuenta.DT_ADDROW = en.DT_ADDROW;
            cuenta.CD_FUNCIONARIO = en.CD_FUNCIONARIO;
            return cuenta;
        }

        public virtual T_DOCUMENTO_PRODUCCION MapFromDocumentoProduccion(DocumentoProduccion documentoProduccion)
        {
            T_DOCUMENTO_PRODUCCION documentoProduccionEntity = CreateEntityDocumentoProduccion(documentoProduccion);

            return documentoProduccionEntity;
        }

        public virtual T_DOCUMENTO_TRANSFERENCIA MapFromDocumentoTransferencia(DocumentoTransferencia documentoTransferencia)
        {
            T_DOCUMENTO_TRANSFERENCIA documentoTransferenciaEntity = CreateEntityDocumentoTransferencia(documentoTransferencia);

            return documentoTransferenciaEntity;
        }

        public virtual DocumentoLinea MapToDocumentoLinea(T_DET_DOCUMENTO detalleDocumento)
        {
            var linea = new DocumentoLinea
            {
                CantidadDesafectada = detalleDocumento.QT_DESAFECTADA,
                CantidadDescargada = detalleDocumento.QT_DESCARGADA,
                CantidadIngresada = detalleDocumento.QT_INGRESADA,
                CantidadReservada = detalleDocumento.QT_RESERVADA,
                CIF = detalleDocumento.VL_CIF,
                DescripcionProducto = detalleDocumento.DS_PRODUTO_INGRESO,
                Disponible = MapStringToBoolean(detalleDocumento.ID_DISPONIBLE),
                Empresa = detalleDocumento.CD_EMPRESA,
                Faixa = detalleDocumento.CD_FAIXA,
                FechaAlta = detalleDocumento.DT_ADDROW,
                FechaDisponible = detalleDocumento.DT_DISPONIBLE,
                FechaModificacion = detalleDocumento.DT_UPDROW,
                Identificador = detalleDocumento.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                Producto = detalleDocumento.CD_PRODUTO,
                Situacion = detalleDocumento.CD_SITUACAO,
                ValorMercaderia = detalleDocumento.VL_MERCADERIA,
                ValorTributo = detalleDocumento.VL_TRIBUTO,
                IdentificadorIngreso = detalleDocumento.NU_INDENTIFICADOR_INGRESO
            };

            return linea;
        }

        public virtual T_DET_DOCUMENTO MapFromDocumentoLinea(string nroDocumento, string tipoDocumento, DocumentoLinea linea)
        {
            var detalleDocumento = new T_DET_DOCUMENTO
            {
                CD_EMPRESA = linea.Empresa,
                CD_FAIXA = linea.Faixa,
                CD_PRODUTO = linea.Producto,
                CD_SITUACAO = linea.Situacion,
                DS_PRODUTO_INGRESO = linea.DescripcionProducto,
                DT_ADDROW = linea.FechaAlta,
                DT_DISPONIBLE = linea.FechaDisponible,
                DT_UPDROW = linea.FechaModificacion,
                ID_DISPONIBLE = MapBooleanToString(linea.Disponible),
                NU_DOCUMENTO = nroDocumento,
                NU_IDENTIFICADOR = linea.Identificador?.Trim()?.ToUpper(),
                QT_DESAFECTADA = linea.CantidadDesafectada,
                QT_DESCARGADA = linea.CantidadDescargada,
                QT_INGRESADA = linea.CantidadIngresada,
                QT_RESERVADA = linea.CantidadReservada,
                TP_DOCUMENTO = tipoDocumento,
                VL_CIF = linea.CIF,
                VL_MERCADERIA = linea.ValorMercaderia,
                VL_TRIBUTO = linea.ValorTributo,
                NU_INDENTIFICADOR_INGRESO = linea.IdentificadorIngreso?.Trim()?.ToUpper(),
                NU_TRANSACCION_DELETE = linea.NumeroTransaccionDelete,
            };

            return detalleDocumento;
        }

        public virtual DocumentoLineaEgreso MapToDocumentoLineaEgreso(T_DET_DOCUMENTO_EGRESO detalleEgreso, IFactoryService service)
        {
            var linea = new DocumentoLineaEgreso
            {
                CantidadDesafectada = detalleEgreso.QT_DESAFECTADA,
                CantidadDescargada = detalleEgreso.QT_DESCARGADA,
                CIF = detalleEgreso.VL_CIF,
                Empresa = detalleEgreso.CD_EMPRESA,
                Faixa = detalleEgreso.CD_FAIXA,
                FechaAlta = detalleEgreso.DT_ADDROW,
                FechaModificacion = detalleEgreso.DT_UPDROW,
                FOB = detalleEgreso.VL_FOB,
                Identificador = detalleEgreso.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                Numero = detalleEgreso.NU_SECUENCIA,
                Producto = detalleEgreso.CD_PRODUTO,
                Tributo = detalleEgreso.VL_TRIBUTO,
                NumeroProceso = detalleEgreso.NU_PROCESO,
                DocumentoIngreso = MapToIngreso(detalleEgreso.T_DOCUMENTO_INGRESO, service),
                TpDocumentoIngreso = detalleEgreso.T_DOCUMENTO_INGRESO?.TP_DOCUMENTO,
                NumeroDocumentoIngreso = detalleEgreso.T_DOCUMENTO_INGRESO?.NU_DOCUMENTO,
                TpDocumento = detalleEgreso.TP_DOCUMENTO,
                NumeroDocumento = detalleEgreso.NU_DOCUMENTO,
            };

            foreach (var lineaReserva in detalleEgreso.T_DET_DOCU_EGRESO_RESERV)
            {
                linea.Reservas.Add(MapToInformacionReserva(lineaReserva));
            }

            return linea;
        }

        public virtual InformacionReserva MapToInformacionReserva(T_DET_DOCU_EGRESO_RESERV lineaReserva)
        {
            return new InformacionReserva()
            {
                CantidadAfectada = lineaReserva.QT_DESAFECTADA,
                Empresa = lineaReserva.CD_EMPRESA,
                IdentificadorPicking = lineaReserva.NU_IDENTIFICADOR_PICKING_DET,
                Identificador = lineaReserva.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                NumeroDocumentoIngreso = lineaReserva.NU_DOCUMENTO_INGRESO,
                Preparacion = lineaReserva.NU_PREPARACION,
                Producto = lineaReserva.CD_PRODUTO,
                Faixa = lineaReserva.CD_FAIXA,
                TipoDocumentoIngreso = lineaReserva.TP_DOCUMENTO_INGRESO
            };
        }

        public virtual DocumentoLineaEgreso MapToDocumentoLineaEgreso1(T_DET_DOCUMENTO_EGRESO detalleEgreso)
        {
            var linea = new DocumentoLineaEgreso
            {
                NumeroDocumento = detalleEgreso.NU_DOCUMENTO,
                TpDocumento = detalleEgreso.TP_DOCUMENTO,
                Numero = detalleEgreso.NU_SECUENCIA,
                Empresa = detalleEgreso.CD_EMPRESA,
                Faixa = detalleEgreso.CD_FAIXA,
                Producto = detalleEgreso.CD_PRODUTO,
                Identificador = detalleEgreso.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                NumeroDocumentoIngreso = detalleEgreso.NU_DOCUMENTO_INGRESO,
                TpDocumentoIngreso = detalleEgreso.TP_DOCUMENTO_INGRESO,
                CantidadDesafectada = detalleEgreso.QT_DESAFECTADA,
                FechaAlta = detalleEgreso.DT_ADDROW,
                FechaModificacion = detalleEgreso.DT_UPDROW,
                CIF = detalleEgreso.VL_CIF,
                FOB = detalleEgreso.VL_FOB,
                Tributo = detalleEgreso.VL_TRIBUTO,
                NumeroProceso = detalleEgreso.NU_PROCESO
            };

            return linea;
        }

        public virtual T_DET_DOCUMENTO_EGRESO MapToDocumentoLineaEgreso1(DocumentoLineaEgreso detalleEgreso)
        {
            var linea = new T_DET_DOCUMENTO_EGRESO
            {
                NU_DOCUMENTO = detalleEgreso.NumeroDocumento,
                TP_DOCUMENTO = detalleEgreso.TpDocumento,
                NU_SECUENCIA = detalleEgreso.Numero,
                CD_EMPRESA = detalleEgreso.Empresa,
                CD_FAIXA = detalleEgreso.Faixa,
                CD_PRODUTO = detalleEgreso.Producto,
                NU_IDENTIFICADOR = detalleEgreso.Identificador?.Trim()?.ToUpper(),
                NU_DOCUMENTO_INGRESO = detalleEgreso.NumeroDocumentoIngreso,
                TP_DOCUMENTO_INGRESO = detalleEgreso.TpDocumentoIngreso,
                QT_DESAFECTADA = detalleEgreso.CantidadDesafectada,
                DT_ADDROW = detalleEgreso.FechaAlta,
                DT_UPDROW = detalleEgreso.FechaModificacion,
                VL_CIF = detalleEgreso.CIF,
                VL_FOB = detalleEgreso.FOB,
                VL_TRIBUTO = detalleEgreso.Tributo,
                NU_PROCESO = detalleEgreso.NumeroProceso
            };

            return linea;
        }

        public virtual T_DET_DOCU_EGRESO_CAMBIO MapToDocumentoLineaEgresoCambio(DocumentoLineaEgreso detalleEgreso)
        {
            var linea = new T_DET_DOCU_EGRESO_CAMBIO
            {
                NU_DOCUMENTO = detalleEgreso.NumeroDocumento,
                TP_DOCUMENTO = detalleEgreso.TpDocumento,
                NU_SECUENCIA = detalleEgreso.Numero,
                CD_EMPRESA = detalleEgreso.Empresa,
                CD_FAIXA = detalleEgreso.Faixa,
                CD_PRODUTO = detalleEgreso.Producto,
                NU_IDENTIFICADOR = detalleEgreso.Identificador?.Trim()?.ToUpper(),
                NU_DOCUMENTO_INGRESO = detalleEgreso.NumeroDocumentoIngreso,
                TP_DOCUMENTO_INGRESO = detalleEgreso.TpDocumentoIngreso,
                QT_DESAFECTADA = detalleEgreso.CantidadDesafectada,
                DT_ADDROW = detalleEgreso.FechaAlta,
                DT_UPDROW = detalleEgreso.FechaModificacion,
                VL_CIF = detalleEgreso.CIF,
                VL_FOB = detalleEgreso.FOB,
                VL_TRIBUTO = detalleEgreso.Tributo,
                NU_PROCESO = detalleEgreso.NumeroProceso
            };

            return linea;
        }

        public virtual List<DocumentoLineaEgreso> MapToDocumentoLineaEgreso(List<T_DET_DOCUMENTO_EGRESO> detalleEgresos, IFactoryService service)
        {
            List<DocumentoLineaEgreso> lista = new List<DocumentoLineaEgreso>();
            foreach (var detalleEgreso in detalleEgresos)
            {
                var linea = new DocumentoLineaEgreso
                {
                    CantidadDesafectada = detalleEgreso.QT_DESAFECTADA,
                    CantidadDescargada = detalleEgreso.QT_DESCARGADA,
                    CIF = detalleEgreso.VL_CIF,
                    Empresa = detalleEgreso.CD_EMPRESA,
                    Faixa = detalleEgreso.CD_FAIXA,
                    FechaAlta = detalleEgreso.DT_ADDROW,
                    FechaModificacion = detalleEgreso.DT_UPDROW,
                    FOB = detalleEgreso.VL_FOB,
                    Identificador = detalleEgreso.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                    Numero = detalleEgreso.NU_SECUENCIA,
                    Producto = detalleEgreso.CD_PRODUTO,
                    Tributo = detalleEgreso.VL_TRIBUTO,
                    NumeroProceso = detalleEgreso.NU_PROCESO,
                    DocumentoIngreso = MapToIngreso(detalleEgreso.T_DOCUMENTO_INGRESO, service),
                    TpDocumentoIngreso = detalleEgreso.TP_DOCUMENTO_INGRESO
                };
                lista.Add(linea);
            }


            return lista;
        }

        public virtual T_DET_DOCUMENTO_EGRESO MapFromDocumentoLineaEgreso(string nroDocumento, string tipoDocumento, DocumentoLineaEgreso linea)
        {
            var detalleDocumentoEgresoEntity = new T_DET_DOCUMENTO_EGRESO
            {
                CD_EMPRESA = linea.Empresa,
                CD_FAIXA = linea.Faixa,
                CD_PRODUTO = linea.Producto,
                DT_ADDROW = linea.FechaAlta,
                DT_UPDROW = linea.FechaModificacion,
                NU_DOCUMENTO = nroDocumento,
                TP_DOCUMENTO = tipoDocumento,
                NU_DOCUMENTO_INGRESO = linea.DocumentoIngreso.Numero,
                NU_IDENTIFICADOR = linea.Identificador?.Trim()?.ToUpper(),
                NU_SECUENCIA = linea.Numero,
                QT_DESAFECTADA = linea.CantidadDesafectada,
                QT_DESCARGADA = linea.CantidadDescargada,
                TP_DOCUMENTO_INGRESO = linea.DocumentoIngreso.Tipo,
                VL_CIF = linea.CIF,
                VL_FOB = linea.FOB,
                VL_TRIBUTO = linea.Tributo,
                NU_PROCESO = linea.NumeroProceso
            };

            foreach (var lineaReserva in linea.Reservas)
            {
                detalleDocumentoEgresoEntity.T_DET_DOCU_EGRESO_RESERV.Add(MapFromDocumentoLineaEgresoReserva(nroDocumento, tipoDocumento, linea.Numero, lineaReserva));
            }

            return detalleDocumentoEgresoEntity;
        }

        public virtual T_DET_DOCU_EGRESO_RESERV MapFromDocumentoLineaEgresoReserva(string nroDocumento, string tipoDocumento, int nroSecuencia, InformacionReserva lineaReserva)
        {
            return new T_DET_DOCU_EGRESO_RESERV
            {
                CD_EMPRESA = lineaReserva.Empresa,
                CD_FAIXA = lineaReserva.Faixa,
                CD_PRODUTO = lineaReserva.Producto,
                NU_DOCUMENTO = nroDocumento,
                TP_DOCUMENTO = tipoDocumento,
                NU_DOCUMENTO_INGRESO = lineaReserva.NumeroDocumentoIngreso,
                NU_IDENTIFICADOR = lineaReserva.Identificador?.Trim()?.ToUpper(),
                NU_IDENTIFICADOR_PICKING_DET = lineaReserva.IdentificadorPicking,
                NU_PREPARACION = lineaReserva.Preparacion,
                NU_SECUENCIA = nroSecuencia,
                QT_DESAFECTADA = lineaReserva.CantidadAfectada,
                TP_DOCUMENTO_INGRESO = lineaReserva.TipoDocumentoIngreso,
            };
        }

        public virtual DocumentoActaDetalle MapToDocumentoLineaActa(T_DET_DOCUMENTO_ACTA detalleActa)
        {
            var linea = new DocumentoActaDetalle
            {
                NumeroDocumento = detalleActa.NU_DOCUMENTO,
                NumeroDocumentoActa = detalleActa.NU_ACTA,
                TipoDocumento = detalleActa.TP_DOCUMENTO,
                TipoDocumentoActa = detalleActa.TP_ACTA,
            };

            return linea;
        }

        public virtual T_DET_DOCUMENTO_ACTA MapFromDocumentoActaDetalle(string nroDocumento, string tipoDocumento, DocumentoActaDetalle linea)
        {
            var detalleDocumento = new T_DET_DOCUMENTO_ACTA
            {
                NU_ACTA = nroDocumento,
                TP_ACTA = tipoDocumento,
                NU_DOCUMENTO = linea.NumeroDocumento,
                TP_DOCUMENTO = linea.TipoDocumento
            };

            return detalleDocumento;
        }

        public virtual SaldoDetalleDocumento MapToSaldoDetalle(V_DOCUMENTO_SALDO_DETALLE entity)
        {
            return new SaldoDetalleDocumento
            {
                NumeroDocumento = entity.NU_DOCUMENTO,
                TipoDocumento = entity.TP_DOCUMENTO,
                TipoDocumentoIngresoDUA = entity.TP_DOCUMENTO_INGRESO_DUA,
                Empresa = entity.CD_EMPRESA,
                Producto = entity.CD_PRODUTO,
                Identificador = entity.NU_IDENTIFICADOR,
                Faixa = entity.CD_FAIXA,
                CantidadDisponible = entity.QT_DISPONIBLE ?? 0
            };
        }

        public virtual SaldoDetalleDocumento MapToSaldoDetalle(V_DET_DOC_DUA_DOC020 entity)
        {
            return new SaldoDetalleDocumento
            {
                NumeroDocumento = entity.NU_DOCUMENTO,
                TipoDocumento = entity.TP_DOCUMENTO,
                TipoDocumentoIngresoDUA = entity.TP_DOCUMENTO_INGRESO_DUA,
                Empresa = entity.CD_EMPRESA,
                Producto = entity.CD_PRODUTO,
                Identificador = entity.NU_IDENTIFICADOR,
                Faixa = entity.CD_FAIXA,
                CantidadDisponible = entity.QT_DISPONIBLE ?? 0
            };
        }

        public virtual T_DOCUMENTO CreateEntityDocumentoWithDetail(IDocumento documento)
        {
            var documentoEntity = CreateEntityDocumento(documento);

            foreach (var linea in documento.Lineas)
            {
                documentoEntity.T_DET_DOCUMENTO.Add(MapFromDocumentoLinea(documento.Numero, documento.Tipo, linea));
            }

            documentoEntity.T_DET_DOCUMENTO_EGRESO = new List<T_DET_DOCUMENTO_EGRESO>();
            documentoEntity.T_DET_DOCUMENTO_EGRESO_INGRESO = new List<T_DET_DOCUMENTO_EGRESO>();
            documentoEntity.T_DET_DOCUMENTO_ACTA = new List<T_DET_DOCUMENTO_ACTA>();
            documentoEntity.T_DET_DOCUMENTO_ACTA_INGRESO = new List<T_DET_DOCUMENTO_ACTA>();

            return documentoEntity;
        }

        public virtual T_DOCUMENTO CreateEntityDocumento(IDocumento documento)
        {
            T_DOCUMENTO entity = new T_DOCUMENTO();
            entity.NU_AGENDA = documento.Agenda;
            entity.CD_CAMION = documento.Camion;
            entity.CD_CLIENTE = documento.Cliente;
            entity.NU_PREDIO = documento.Predio;
            entity.CD_DESPACHANTE = documento.Despachante;
            entity.CD_EMPRESA = documento.Empresa;
            entity.CD_FORNECEDOR = documento.Proveedor;
            entity.CD_FUNCIONARIO = documento.Usuario;
            entity.CD_MONEDA = NullIfEmpty(documento.Moneda);
            entity.CD_SITUACAO = documento.Situacion;
            entity.CD_TRANSPORTISTA = documento.Transportista;
            entity.CD_UNIDAD_MEDIDA_BULTO = documento.UnidadMedida;
            entity.CD_VIA = documento.Via;
            entity.DS_ANEXO1 = documento.Anexo1;
            entity.DS_ANEXO2 = documento.Anexo2;
            entity.DS_ANEXO3 = documento.Anexo3;
            entity.DS_ANEXO4 = documento.Anexo4;
            entity.DS_ANEXO5 = documento.Anexo5;
            entity.DS_ANEXO6 = documento.Anexo6;
            entity.DS_DOCUMENTO = documento.Descripcion;
            entity.DT_ADDROW = documento.FechaAlta;
            entity.DT_DECLARADO = documento.FechaDeclarado;
            entity.DT_DTI = documento.FechaDTI;
            entity.DT_ENVIADO = documento.FechaEnviado;
            entity.DT_FACTURACION = documento.FechaFacturado;
            entity.DT_FINALIZADO = documento.FechaFinalizado;
            entity.DT_MOVILIZA_CONTENEDOR = documento.FechaMovilizacionContenedor;
            entity.DT_PROGRAMADO = documento.FechaProgramado;
            entity.DT_UPDROW = documento.FechaModificacion;
            entity.ID_AGENDAR_AUTOMATICAMENTE = MapBooleanToString(documento.AgendarAutomaticamente);
            entity.ID_FICTICIO = MapBooleanToString(documento.Ficticio);
            entity.ID_GENERAR_AGENDA = MapBooleanToString(documento.GeneraAgenda);
            entity.ID_MANUAL = documento.IdManual;
            entity.NU_CONOCIMIENTO = documento.Conocimiento;
            entity.NU_CORRELATIVO = documento.NumeroCorrelativo;
            entity.NU_CORRELATIVO_2 = documento.NumeroCorrelativo2;
            entity.NU_DOCUMENTO = documento.Numero;
            entity.NU_DOC_TRANSPORTE = documento.DocumentoTransporte;
            entity.NU_DOC1 = documento.NumeroDocumento1;
            entity.NU_DOC2 = documento.NumeroDocumento2;
            entity.NU_DTI = documento.NumeroDTI;

            if (documento.DocumentoAduana != null)
            {
                entity.DT_DUA = documento.DocumentoAduana.Fecha;
                entity.NU_DUA = documento.DocumentoAduana.Numero;
                entity.TP_DUA = documento.DocumentoAduana.Tipo;
            }

            if (documento.DocumentoReferenciaExterna != null)
            {
                entity.DT_REFERENCIA_EXTERNA = documento.DocumentoReferenciaExterna.Fecha;
                entity.NU_REFERENCIA_EXTERNA = documento.DocumentoReferenciaExterna.Numero;
                entity.TP_REFERENCIA_EXTERNA = NullIfEmpty(documento.DocumentoReferenciaExterna.Tipo);
            }

            entity.NU_EXPORT = documento.NumeroExportacion;
            entity.NU_FACTURA = documento.Factura;
            entity.NU_IMPORT = documento.NumeroImportacion;
            entity.NU_TRANSACCION_DELETE = documento.NumeroTransaccionDelete;
            entity.QT_BULTO = documento.CantidadBulto;
            entity.QT_CONTENEDOR20 = documento.CantidadContenedor20;
            entity.QT_CONTENEDOR40 = documento.CantidadContenedor40;
            entity.QT_PESO = documento.Peso;
            entity.QT_VOLUMEN = documento.Volumen;
            entity.TP_ALMACENAJE_Y_SEGURO = documento.TipoAlmacenajeYSeguro;
            entity.TP_DOCUMENTO = documento.Tipo;
            entity.VL_ARBITRAJE = documento.ValorArbitraje;
            entity.VL_FLETE = documento.ValorFlete;
            entity.VL_OTROS_GASTOS = documento.ValorOtrosGastos;
            entity.VL_SEGURO = documento.ValorSeguro;
            entity.VL_VALIDADO = MapBooleanToString(documento.Validado);
            entity.T_DET_DOCUMENTO = new List<T_DET_DOCUMENTO>();
            entity.NU_AGRUPADOR = documento.NumeroAgrupador;
            entity.TP_AGRUPADOR = documento.TipoAgrupador;
            entity.DS_ANEXO3 = documento.Anexo3;
            entity.DS_ANEXO4 = documento.Anexo4;
            entity.DS_ANEXO5 = documento.Anexo5;
            entity.ICMS = documento.ICMS;
            entity.II = documento.II;
            entity.IPI = documento.IPI;
            entity.IISUSPENSO = documento.IISUSPENSO;
            entity.IPISUSPENSO = documento.IPISUSPENSO;
            entity.PISCONFINS = documento.PISCONFINS;
            entity.CD_REGIMEN_ADUANA = documento.RegimenAduana;
            entity.ID_ESTADO = documento.Estado;

            return entity;
        }

        public virtual T_DOCUMENTO_AGRUPADOR CreateEntityDocumentoAgrupador(IDocumentoAgrupador agrupador)
        {
            return new T_DOCUMENTO_AGRUPADOR()
            {
                DT_ADDROW = agrupador.FechaAlta,
                DT_SAIDA = agrupador.FechaSalida,
                ID_ESTADO = agrupador.Estado,
                NU_AGRUPADOR = agrupador.Numero,
                NU_LACRE = agrupador.NumeroLacre,
                QT_PESO = agrupador.Peso,
                QT_VOLUMEN = agrupador.Cantidad,
                TP_AGRUPADOR = agrupador.Tipo.TipoAgrupador,
                VL_TOTAL = agrupador.ValorTotal,
                DT_UPDATEROW = agrupador.FechaActualizacion,
                CD_TIPO_VEICULO = agrupador.TipoVehiculo.Id,
                CD_TRANSPORTADORA = agrupador.Transportadora.Id,
                QT_PESO_LIQUIDO = agrupador.PesoLiquido,
                ANEXO1 = agrupador.Anexo1,
                ANEXO2 = agrupador.Anexo2,
                ANEXO3 = agrupador.Anexo3,
                ANEXO4 = agrupador.Anexo4,
                CD_EMPRESA = agrupador.Empresa,
                DS_MOTORISTA = agrupador.Motorista,
                DS_PLACA = agrupador.Placa,
                DT_LLEGADA = agrupador.FechaLlegada,
                NU_PREDIO = agrupador.Predio,
                DS_MOTIVO = agrupador.Motivo,
                DT_IMPRESO = agrupador.FechaImpreso
            };
        }

        public virtual void SetPropertiesDocumento(IDocumento documento, T_DOCUMENTO documentoEntity)
        {
            documento.Agenda = documentoEntity.NU_AGENDA;
            documento.AgendarAutomaticamente = MapStringToBoolean(documentoEntity.TP_DOCUMENTO);
            documento.Anexo1 = documentoEntity.DS_ANEXO1;
            documento.Anexo2 = documentoEntity.DS_ANEXO2;
            documento.Anexo3 = documentoEntity.DS_ANEXO3;
            documento.Anexo4 = documentoEntity.DS_ANEXO4;
            documento.Anexo5 = documentoEntity.DS_ANEXO5;
            documento.Anexo6 = documentoEntity.DS_ANEXO6;
            documento.Camion = documentoEntity.CD_CAMION;
            documento.CantidadBulto = documentoEntity.QT_BULTO;
            documento.CantidadContenedor20 = documentoEntity.QT_CONTENEDOR20;
            documento.CantidadContenedor40 = documentoEntity.QT_CONTENEDOR40;
            documento.Cliente = documentoEntity.CD_CLIENTE;
            documento.Configuracion = new DocumentoConfiguracion();
            documento.Conocimiento = documentoEntity.NU_CONOCIMIENTO;
            documento.Predio = documentoEntity.NU_PREDIO;
            documento.Descripcion = documentoEntity.DS_DOCUMENTO;
            documento.Despachante = documentoEntity.CD_DESPACHANTE;
            documento.DocumentoAduana = new DocumentoAduana
            {
                Numero = documentoEntity.NU_DUA,
                Tipo = documentoEntity.TP_DUA,
                Fecha = documentoEntity.DT_DUA
            };
            documento.DocumentoReferenciaExterna = new DocumentoReferenciaExterna
            {
                Numero = documentoEntity.NU_REFERENCIA_EXTERNA,
                Tipo = documentoEntity.TP_REFERENCIA_EXTERNA,
                Fecha = documentoEntity.DT_REFERENCIA_EXTERNA
            };
            documento.DocumentoTransporte = documentoEntity.NU_DOC_TRANSPORTE;
            documento.Empresa = documentoEntity.CD_EMPRESA;
            documento.Factura = documentoEntity.NU_FACTURA;
            documento.FechaAlta = documentoEntity.DT_ADDROW;
            documento.FechaDeclarado = documentoEntity.DT_DECLARADO;
            documento.FechaDTI = documentoEntity.DT_DTI;
            documento.FechaEnviado = documentoEntity.DT_ENVIADO;
            documento.FechaFacturado = documentoEntity.DT_FACTURACION;
            documento.FechaFinalizado = documentoEntity.DT_FINALIZADO;
            documento.FechaModificacion = documentoEntity.DT_UPDROW;
            documento.FechaMovilizacionContenedor = documentoEntity.DT_MOVILIZA_CONTENEDOR;
            documento.FechaProgramado = documentoEntity.DT_PROGRAMADO;
            documento.Ficticio = MapStringToBoolean(documentoEntity.ID_FICTICIO);
            documento.GeneraAgenda = MapStringToBoolean(documentoEntity.ID_GENERAR_AGENDA);
            documento.IdManual = documentoEntity.ID_MANUAL;
            documento.Moneda = documentoEntity.CD_MONEDA;
            documento.Numero = documentoEntity.NU_DOCUMENTO;
            documento.NumeroCorrelativo = documentoEntity.NU_CORRELATIVO;
            documento.NumeroCorrelativo2 = documentoEntity.NU_CORRELATIVO_2;
            documento.NumeroDocumento1 = documentoEntity.NU_DOC1;
            documento.NumeroDocumento2 = documentoEntity.NU_DOC2;
            documento.NumeroDTI = documentoEntity.NU_DTI;
            documento.NumeroExportacion = documentoEntity.NU_EXPORT;
            documento.NumeroImportacion = documentoEntity.NU_IMPORT;
            documento.NumeroTransaccionDelete = documentoEntity.NU_TRANSACCION_DELETE;
            documento.Peso = documentoEntity.QT_PESO;
            documento.Proveedor = documentoEntity.CD_FORNECEDOR;
            documento.Situacion = documentoEntity.CD_SITUACAO;
            documento.Tipo = documentoEntity.TP_DOCUMENTO;
            documento.TipoAlmacenajeYSeguro = documentoEntity.TP_ALMACENAJE_Y_SEGURO;
            documento.Transportista = documentoEntity.CD_TRANSPORTISTA;
            documento.UnidadMedida = documentoEntity.CD_UNIDAD_MEDIDA_BULTO;
            documento.Usuario = documentoEntity.CD_FUNCIONARIO;
            documento.Validado = MapStringToBoolean(documentoEntity.VL_VALIDADO);
            documento.ValorArbitraje = documentoEntity.VL_ARBITRAJE;
            documento.ValorFlete = documentoEntity.VL_FLETE;
            documento.ValorOtrosGastos = documentoEntity.VL_OTROS_GASTOS;
            documento.ValorSeguro = documentoEntity.VL_SEGURO;
            documento.Via = documentoEntity.CD_VIA;
            documento.Volumen = documentoEntity.QT_VOLUMEN;
            documento.NumeroAgrupador = documentoEntity.NU_AGRUPADOR;
            documento.TipoAgrupador = documentoEntity.TP_AGRUPADOR;
            documento.ICMS = documentoEntity.ICMS;
            documento.II = documentoEntity.II;
            documento.IPI = documentoEntity.IPI;
            documento.IISUSPENSO = documentoEntity.IISUSPENSO;
            documento.IPISUSPENSO = documentoEntity.IPISUSPENSO;
            documento.PISCONFINS = documentoEntity.PISCONFINS;
            documento.RegimenAduana = documentoEntity.CD_REGIMEN_ADUANA;

            foreach (var linea in documentoEntity.T_DET_DOCUMENTO)
            {
                documento.Lineas.Add(MapToDocumentoLinea(linea));
            }
        }

        public virtual T_DOCUMENTO_PREPARACION_RESERV CreateEntityPreparacionReserva(DocumentoPreparacionReserva preparacionReserva)
        {
            var preparacionReservaEntity = new T_DOCUMENTO_PREPARACION_RESERV()
            {
                CD_EMPRESA = preparacionReserva.Empresa,
                CD_FAIXA = preparacionReserva.Faixa,
                CD_PRODUTO = preparacionReserva.Producto,
                ID_ESPECIFICA_IDENTIFICADOR = MapBooleanToString(preparacionReserva.EspecificaIdentificador),
                NU_DOCUMENTO = preparacionReserva.NroDocumento,
                NU_IDENTIFICADOR = preparacionReserva.Identificador?.Trim()?.ToUpper(),
                NU_IDENTIFICADOR_PICKING_DET = preparacionReserva.NroIdentificadorPicking,
                NU_PREPARACION = preparacionReserva.Preparacion,
                QT_ANULAR = preparacionReserva.CantidadAnular,
                QT_PRODUTO = preparacionReserva.CantidadProducto,
                TP_DOCUMENTO = preparacionReserva.TipoDocumento,
                QT_PREPARADO = preparacionReserva.CantidadPreparada,
                DT_ADDROW = preparacionReserva.FechaAlta,
                DT_UPDROW = preparacionReserva.FechaModificacion,
                NU_TRANSACCION_DELETE = preparacionReserva.NumeroTransaccionDelete
            };

            return preparacionReservaEntity;
        }
        public virtual DocumentoPreparacionReserva MapEntityToObject(T_DOCUMENTO_PREPARACION_RESERV preparacionReserva)
        {
            var preparacionReservaObject = new DocumentoPreparacionReserva()
            {
                 Empresa = preparacionReserva.CD_EMPRESA ,
                 Faixa = preparacionReserva.CD_FAIXA ,
                 Producto = preparacionReserva.CD_PRODUTO ,
                EspecificaIdentificadorId = preparacionReserva.ID_ESPECIFICA_IDENTIFICADOR ,
                 NroDocumento = preparacionReserva.NU_DOCUMENTO ,
                 Identificador = preparacionReserva.NU_IDENTIFICADOR ,
                 NroIdentificadorPicking = preparacionReserva.NU_IDENTIFICADOR_PICKING_DET ,
                 Preparacion = preparacionReserva.NU_PREPARACION ,
                 CantidadAnular = preparacionReserva.QT_ANULAR ,
                 CantidadProducto = preparacionReserva.QT_PRODUTO ,
                 TipoDocumento = preparacionReserva.TP_DOCUMENTO ,
                 CantidadPreparada = preparacionReserva.QT_PREPARADO ,
                 FechaAlta = preparacionReserva.DT_ADDROW ,
                 FechaModificacion = preparacionReserva.DT_UPDROW ,
                 NumeroTransaccionDelete = preparacionReserva.NU_TRANSACCION_DELETE,
            };

            return preparacionReservaObject;
        }
        public virtual LT_DELETE_DOCUMENTO_PREPARACION_RESERV CreateEntityPreparacionReservaDesafectada(DocumentoPreparacionReserva preparacionReserva)
        {
            var preparacionReservaDesafectadaEntity = new LT_DELETE_DOCUMENTO_PREPARACION_RESERV()
            {
                CD_EMPRESA = preparacionReserva.Empresa,
                CD_FAIXA = preparacionReserva.Faixa,
                CD_PRODUTO = preparacionReserva.Producto,
                ID_ESPECIFICA_IDENTIFICADOR = MapBooleanToString(preparacionReserva.EspecificaIdentificador),
                NU_DOCUMENTO = preparacionReserva.NroDocumento,
                NU_IDENTIFICADOR = preparacionReserva.Identificador?.Trim()?.ToUpper(),
                NU_IDENTIFICADOR_PICKING_DET = preparacionReserva.NroIdentificadorPicking,
                NU_PREPARACION = preparacionReserva.Preparacion,
                QT_ANULAR = preparacionReserva.CantidadAnular,
                TP_DOCUMENTO = preparacionReserva.TipoDocumento,
                DT_ADDROW = preparacionReserva.FechaAlta,
                DT_UPDROW = preparacionReserva.FechaModificacion,
                NU_TRANSACCION = preparacionReserva.NumeroTransaccion,
                NU_TRANSACCION_DELETE = preparacionReserva.NumeroTransaccionDelete
            };

            return preparacionReservaDesafectadaEntity;
        }

        public virtual void SetPropertiesDocumentoReserva(DocumentoPreparacionReserva documentoReserva, T_DOCUMENTO_PREPARACION_RESERV documentoReservaEntity, IFactoryService service)
        {
            documentoReserva.CantidadAnular = documentoReservaEntity.QT_ANULAR;
            documentoReserva.CantidadProducto = documentoReservaEntity.QT_PRODUTO;
            documentoReserva.Empresa = documentoReservaEntity.CD_EMPRESA;
            documentoReserva.EspecificaIdentificador = MapStringToBoolean(documentoReservaEntity.ID_ESPECIFICA_IDENTIFICADOR);
            documentoReserva.Faixa = documentoReservaEntity.CD_FAIXA;
            documentoReserva.Identificador = documentoReservaEntity.NU_IDENTIFICADOR?.Trim()?.ToUpper();
            documentoReserva.NroDocumento = documentoReservaEntity.NU_DOCUMENTO;
            documentoReserva.NroIdentificadorPicking = documentoReservaEntity.NU_IDENTIFICADOR_PICKING_DET;
            documentoReserva.Preparacion = documentoReservaEntity.NU_PREPARACION;
            documentoReserva.Producto = documentoReservaEntity.CD_PRODUTO;
            documentoReserva.TipoDocumento = documentoReservaEntity.TP_DOCUMENTO;
            documentoReserva.CantidadPreparada = documentoReservaEntity.QT_PREPARADO;
            documentoReserva.FechaAlta = documentoReservaEntity.DT_ADDROW;
            documentoReserva.FechaModificacion = documentoReservaEntity.DT_UPDROW;
            documentoReserva.NumeroTransaccionDelete = documentoReservaEntity.NU_TRANSACCION_DELETE;

            if (documentoReservaEntity.T_DOCUMENTO.T_DOCUMENTO_TIPO.TP_OPERACION == TipoDocumentoOperacion.MODIFICACION)
                documentoReserva.DocumentoIngreso = MapToActa(documentoReservaEntity.T_DOCUMENTO, service);
            else
                documentoReserva.DocumentoIngreso = MapToIngreso(documentoReservaEntity.T_DOCUMENTO, service);
        }

        public virtual void SetPropertiesDocumentoReservaDesafectada(DocumentoPreparacionReserva documentoReserva, LT_DELETE_DOCUMENTO_PREPARACION_RESERV documentoReservaEntity, IFactoryService service)
        {
            documentoReserva.CantidadAnular = documentoReservaEntity.QT_ANULAR;
            documentoReserva.CantidadProducto = 0;
            documentoReserva.Empresa = documentoReservaEntity.CD_EMPRESA;
            documentoReserva.EspecificaIdentificador = MapStringToBoolean(documentoReservaEntity.ID_ESPECIFICA_IDENTIFICADOR);
            documentoReserva.Faixa = documentoReservaEntity.CD_FAIXA;
            documentoReserva.Identificador = documentoReservaEntity.NU_IDENTIFICADOR?.Trim()?.ToUpper();
            documentoReserva.NroDocumento = documentoReservaEntity.NU_DOCUMENTO;
            documentoReserva.NroIdentificadorPicking = documentoReservaEntity.NU_IDENTIFICADOR_PICKING_DET;
            documentoReserva.Preparacion = documentoReservaEntity.NU_PREPARACION;
            documentoReserva.Producto = documentoReservaEntity.CD_PRODUTO;
            documentoReserva.TipoDocumento = documentoReservaEntity.TP_DOCUMENTO;
            documentoReserva.CantidadPreparada = 0;
            documentoReserva.FechaAlta = documentoReservaEntity.DT_ADDROW;
            documentoReserva.FechaModificacion = documentoReservaEntity.DT_UPDROW;
            documentoReserva.NumeroTransaccion = documentoReservaEntity.NU_TRANSACCION;
            documentoReserva.NumeroTransaccionDelete = documentoReservaEntity.NU_TRANSACCION_DELETE;

            if (documentoReservaEntity.T_DOCUMENTO.T_DOCUMENTO_TIPO.TP_OPERACION == TipoDocumentoOperacion.MODIFICACION)
                documentoReserva.DocumentoIngreso = MapToActa(documentoReservaEntity.T_DOCUMENTO, service);
            else
                documentoReserva.DocumentoIngreso = MapToIngreso(documentoReservaEntity.T_DOCUMENTO, service);
        }

        public virtual void SetPropertiesDocumentoAgrupador(IDocumentoAgrupador documentoAgrupador, T_DOCUMENTO_AGRUPADOR agrupadorEntity)
        {
            documentoAgrupador.Cantidad = agrupadorEntity.QT_VOLUMEN;
            documentoAgrupador.Estado = agrupadorEntity.ID_ESTADO;
            documentoAgrupador.Numero = agrupadorEntity.NU_AGRUPADOR;
            documentoAgrupador.NumeroLacre = agrupadorEntity.NU_LACRE;
            documentoAgrupador.Peso = agrupadorEntity.QT_PESO;
            documentoAgrupador.Tipo = new DocumentoAgrupadorTipo()
            {
                Descripcion = agrupadorEntity.T_DOCUMENTO_AGRUPADOR_TIPO.DS_TIPO,
                Habilitado = MapStringToBoolean(agrupadorEntity.T_DOCUMENTO_AGRUPADOR_TIPO.FL_HABILITADO),
                TipoAgrupador = agrupadorEntity.TP_AGRUPADOR,
                TipoOperacion = agrupadorEntity.T_DOCUMENTO_AGRUPADOR_TIPO.TP_OPERACION,
                ManejaPredio = MapStringToBoolean(agrupadorEntity.T_DOCUMENTO_AGRUPADOR_TIPO.FL_MANEJA_PREDIO)
            };
            documentoAgrupador.ValorTotal = agrupadorEntity.VL_TOTAL;
            documentoAgrupador.FechaAlta = agrupadorEntity.DT_ADDROW;
            documentoAgrupador.FechaSalida = agrupadorEntity.DT_SAIDA;
            documentoAgrupador.FechaActualizacion = agrupadorEntity.DT_UPDATEROW;
            documentoAgrupador.Empresa = agrupadorEntity.CD_EMPRESA;
            documentoAgrupador.Motorista = agrupadorEntity.DS_MOTORISTA;
            documentoAgrupador.PesoLiquido = agrupadorEntity.QT_PESO_LIQUIDO;
            documentoAgrupador.Placa = agrupadorEntity.DS_PLACA;

            if (agrupadorEntity.T_TRANSPORTADORA != null)
                documentoAgrupador.Transportadora = new Transportista()
                {
                    Descripcion = agrupadorEntity.T_TRANSPORTADORA.DS_TRANSPORTADORA,
                    Id = agrupadorEntity.T_TRANSPORTADORA.CD_TRANSPORTADORA,
                    NumeroFiscal = agrupadorEntity.T_TRANSPORTADORA.CD_CGC_TRANSP
                };

            if (agrupadorEntity.T_TIPO_VEICULO != null)
                documentoAgrupador.TipoVehiculo = new VehiculoEspecificacion()
                {
                    Id = agrupadorEntity.T_TIPO_VEICULO.CD_TIPO_VEICULO,
                    Tipo = agrupadorEntity.T_TIPO_VEICULO.DS_TIPO_VEICULO
                };

            documentoAgrupador.Anexo1 = agrupadorEntity.ANEXO1;
            documentoAgrupador.Anexo2 = agrupadorEntity.ANEXO2;
            documentoAgrupador.Anexo3 = agrupadorEntity.ANEXO3;
            documentoAgrupador.Anexo4 = agrupadorEntity.ANEXO4;
            documentoAgrupador.FechaLlegada = agrupadorEntity.DT_LLEGADA;
            documentoAgrupador.Predio = agrupadorEntity.NU_PREDIO;
            documentoAgrupador.Motivo = agrupadorEntity.DS_MOTIVO;
            documentoAgrupador.FechaImpreso = agrupadorEntity.DT_IMPRESO;
        }

        public virtual void SetPropertiesDocumentoAgrupadorTipo(DocumentoAgrupadorTipo documentoAgrupadorTipo, T_DOCUMENTO_AGRUPADOR_TIPO agrupadorEntityTipo)
        {
            documentoAgrupadorTipo.Descripcion = agrupadorEntityTipo.DS_TIPO;
            documentoAgrupadorTipo.Habilitado = MapStringToBoolean(agrupadorEntityTipo.FL_HABILITADO);
            documentoAgrupadorTipo.TipoAgrupador = agrupadorEntityTipo.TP_AGRUPADOR;
            documentoAgrupadorTipo.TipoOperacion = agrupadorEntityTipo.TP_OPERACION;
            documentoAgrupadorTipo.CantidadMaximaDocumentos = agrupadorEntityTipo.QT_DOCUMENTO;
            documentoAgrupadorTipo.ManejaPredio = MapStringToBoolean(agrupadorEntityTipo.FL_MANEJA_PREDIO);
            documentoAgrupadorTipo.Secuencia = agrupadorEntityTipo.NM_SECUENCIA;

            documentoAgrupadorTipo.Grupos = new List<DocumentoAgrupadorGrupo>();
        }

        public virtual T_DOCUMENTO_PRODUCCION CreateEntityDocumentoProduccion(DocumentoProduccion documentoProduccion)
        {
            var documentoProduccionEntity = new T_DOCUMENTO_PRODUCCION()
            {
                NU_DOCUMENTO_EGR = documentoProduccion.DocumentoEgreso.Numero,
                TP_DOCUMENTO_EGR = documentoProduccion.DocumentoEgreso.Tipo,
                NU_DOCUMENTO_ING = documentoProduccion.DocumentoIngreso.Numero,
                TP_DOCUMENTO_ING = documentoProduccion.DocumentoIngreso.Tipo,
                NU_PRDC_INGRESO = documentoProduccion.NumeroProduccion,
                DT_ADDROW = documentoProduccion.FechaAlta,
                DT_UPDROW = documentoProduccion.FechaModificacion

            };

            return documentoProduccionEntity;
        }

        public virtual T_DOCUMENTO_TRANSFERENCIA CreateEntityDocumentoTransferencia(DocumentoTransferencia documentoTransferencia)
        {
            var documentoTrasnferenciaEntity = new T_DOCUMENTO_TRANSFERENCIA()
            {
                NU_DOCUMENTO_EGR = documentoTransferencia.DocumentoEgreso.Numero,
                TP_DOCUMENTO_EGR = documentoTransferencia.DocumentoEgreso.Tipo,
                NU_DOCUMENTO_ING = documentoTransferencia.DocumentoIngreso.Numero,
                TP_DOCUMENTO_ING = documentoTransferencia.DocumentoIngreso.Tipo,
                NU_TRANSFERENCIA = documentoTransferencia.NumeroTransferencia,
                DT_ADDROW = documentoTransferencia.FechaAlta,
                DT_UPDROW = documentoTransferencia.FechaModificacion
            };

            return documentoTrasnferenciaEntity;
        }

        public virtual void SetPropertiesDocumentoProduccion(DocumentoProduccion documentoProduccion, T_DOCUMENTO_PRODUCCION documentoProduccionEntity, IFactoryService service)
        {
            documentoProduccion.NumeroProduccion = documentoProduccionEntity.NU_PRDC_INGRESO;
            documentoProduccion.DocumentoIngreso = MapToIngreso(documentoProduccionEntity.T_DOCUMENTO_INGRESO, service);
            documentoProduccion.DocumentoEgreso = MapToEgreso(documentoProduccionEntity.T_DOCUMENTO_EGRESO, service);
            documentoProduccion.FechaAlta = documentoProduccionEntity.DT_ADDROW;
            documentoProduccion.FechaModificacion = documentoProduccionEntity.DT_UPDROW;
        }

        public virtual void SetPropertiesDocumentoTransferencia(DocumentoTransferencia documentoTransferencia, T_DOCUMENTO_TRANSFERENCIA documentoTransferenciaEntity, IFactoryService service)
        {
            documentoTransferencia.NumeroTransferencia = documentoTransferenciaEntity.NU_TRANSFERENCIA;
            documentoTransferencia.DocumentoIngreso = MapToIngreso(documentoTransferenciaEntity.T_DOCUMENTO_INGRESO, service);
            documentoTransferencia.DocumentoEgreso = MapToEgreso(documentoTransferenciaEntity.T_DOCUMENTO_EGRESO, service);
            documentoTransferencia.FechaAlta = documentoTransferenciaEntity.DT_ADDROW;
            documentoTransferencia.FechaModificacion = documentoTransferenciaEntity.DT_UPDROW;
        }

        public virtual DocumentoAccion MapToDocumentoAccion(T_DOCUMENTO_ESTADO_ORDEN entity)
        {
            return new DocumentoAccion()
            {
                Codigo = entity.CD_ACCION,
                TipoDocumento = entity.TP_DOCUMENTO,
                Origen = new DocumentoEstado
                {
                    Id = entity.ID_ESTADO_ORIGEN,
                    Descripcion = entity.T_DOCUMENTO_ESTADO_ORIGEN.DS_ESTADO
                },
                Destino = new DocumentoEstado
                {
                    Id = entity.ID_ESTADO_DESTINO,
                    Descripcion = entity.T_DOCUMENTO_ESTADO_DESTINO.DS_ESTADO
                }
            };
        }

        public virtual DocumentoPreparacion MapToDocumentoPreparacion(T_DOCUMENTO_PREPARACION entity)
        {
            return new DocumentoPreparacion
            {
                NroDocumentoPreparacion = entity.NU_DOCUMENTO_PREPARACION,
                Preparacion = entity.NU_PREPARACION,
                EmpresaIngreso = entity.CD_EMPRESA_INGRESO ?? entity.CD_EMPRESA_EGRESO,
                EmpresaEgreso = entity.CD_EMPRESA_EGRESO,
                TpOperativa = entity.TP_OPERATIVA,
                Activa = MapStringToBoolean(entity.FL_ACTIVE),
                NroDocumentoIngreso = entity.NU_DOCUMENTO_INGRESO,
                TpDocumentoIngreso = entity.TP_DOCUMENTO_INGRESO,
                NroDocumentoEgreso = entity.NU_DOCUMENTO_EGRESO,
                TpDocumentoEgreso = entity.TP_DOCUMENTO_EGRESO,
                FechaAlta = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                Funcionario = entity.CD_FUNCIONARIO
            };
        }

        public virtual T_DOCUMENTO_PREPARACION MapFromDocumentoPreparacion(DocumentoPreparacion doc)
        {
            return new T_DOCUMENTO_PREPARACION
            {
                NU_DOCUMENTO_PREPARACION = doc.NroDocumentoPreparacion,
                NU_PREPARACION = doc.Preparacion,
                CD_EMPRESA_INGRESO = doc.EmpresaIngreso,
                CD_EMPRESA_EGRESO = doc.EmpresaEgreso,
                TP_OPERATIVA = doc.TpOperativa,
                FL_ACTIVE = MapBooleanToString(doc.Activa),
                NU_DOCUMENTO_INGRESO = doc.NroDocumentoIngreso,
                TP_DOCUMENTO_INGRESO = doc.TpDocumentoIngreso,
                NU_DOCUMENTO_EGRESO = doc.NroDocumentoEgreso,
                TP_DOCUMENTO_EGRESO = doc.TpDocumentoEgreso,
                DT_ADDROW = doc.FechaAlta,
                DT_UPDROW = doc.FechaModificacion,
                CD_FUNCIONARIO = doc.Funcionario
            };
        }

        public virtual DocumentoLineaDesafectada MapToDocumentoLineaDesafectada(T_DET_DOCUMENTO entity)
        {
            if (entity == null)
                return null;

            return new DocumentoLineaDesafectada()
            {
                NroDocumento = entity.NU_DOCUMENTO,
                TipoDocumento = entity.TP_DOCUMENTO,
                LineaModificada = MapToDocumentoLinea(entity)
            };
        }
    }
}
