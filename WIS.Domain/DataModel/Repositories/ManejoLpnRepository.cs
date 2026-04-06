using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WIS.Domain.CodigoMultidato.Constants;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Extensions;
using WIS.Domain.General;
using WIS.Domain.General.API.Bulks;
using WIS.Domain.Parametrizacion;
using WIS.Domain.Picking;
using WIS.Domain.Picking.Dtos;
using WIS.Domain.Recepcion;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
using WIS.Persistence.Database;
using WIS.Persistence.General;
using WIS.Security;

namespace WIS.Domain.DataModel.Repositories
{
    public class ManejoLpnRepository
    {
        protected readonly WISDB _context;
        protected readonly int _userId;
        protected readonly string _application;
        protected readonly LpnMapper _lpnMapper;
        protected readonly IDapper _dapper;
        protected readonly AtributoMapper _atributoMapper;

        public ManejoLpnRepository(WISDB context, string application, int userid, IDapper dapper)
        {
            this._dapper = dapper;
            this._userId = userid;
            this._context = context;
            this._application = application;
            this._lpnMapper = new LpnMapper();
            this._atributoMapper = new AtributoMapper();
        }

        #region Any

        public virtual bool AnyTipoLpnEnUso(string value)
        {
            return _context.T_LPN
                .AsNoTracking()
                .Any(x => x.TP_LPN_TIPO == value);
        }

        public virtual bool AnyTipoLpn(string tpLpn)
        {
            return _context.T_LPN_TIPO.AsNoTracking().Any(x => x.TP_LPN_TIPO == tpLpn);
        }

        public virtual bool AnyTipoDeEtiquetaDeRecepcion(string tipoEtiqueta)
        {
            return _context.T_ETIQUETA_LOTE
                .AsNoTracking()
                .Any(x => x.TP_ETIQUETA == tipoEtiqueta);
        }

        public virtual bool IsEtiquetaLpnRecepcion(string codigoBarras, string tipoEtiqueta)
        {
            var isEtiquetaLpnRecepcion = false;
            var barralpn = _context.T_LPN_BARRAS
                .AsNoTracking()
                .Where(x => x.CD_BARRAS == codigoBarras)
                .OrderByDescending(x => x.NU_LPN)
                .FirstOrDefault();

            if (barralpn != null)
            {
                if (_context.T_LPN
                    .Include("T_LPN_TIPO")
                    .AsNoTracking()
                    .Any(x => x.NU_LPN == barralpn.NU_LPN
                        && x.T_LPN_TIPO.TP_ETIQUETA_RECEPCION == tipoEtiqueta))
                {
                    isEtiquetaLpnRecepcion = true;
                }
            }

            return isEtiquetaLpnRecepcion;
        }

        public virtual bool AnyLpnAsociadoAgenda(int nuAgenda)
        {
            return _context.T_LPN.AsNoTracking().Any(l => l.NU_AGENDA == nuAgenda);
        }

        public virtual bool ExisteReservaAtributo(string tpLpn, string ubicacion, int empresa, string producto, string identificador, decimal faixa, string atributos)
        {
            return _context.T_DET_PICKING_LPN
                .AsNoTracking()
                .Any(a => a.TP_LPN_TIPO == tpLpn
                    && a.CD_ENDERECO == ubicacion
                    && a.CD_EMPRESA == empresa
                    && a.CD_PRODUTO == producto
                    && a.NU_IDENTIFICADOR == identificador
                    && a.CD_FAIXA == faixa
                    && a.VL_ATRIBUTOS.Contains(atributos)
                    && a.QT_RESERVA > 0);

        }

        public virtual bool ExisteReservaAtributo(string tpLpn, string ubicacion, string atributos, bool detalle, int empresa, string producto = "", string identificador = "", decimal faixa = 1)
        {
            if (detalle)
            {
                return _context.T_DET_PICKING_LPN
                    .AsNoTracking()
                    .Any(a => a.TP_LPN_TIPO == tpLpn
                    && a.CD_ENDERECO == ubicacion
                    && a.CD_EMPRESA == empresa
                    && a.CD_PRODUTO == producto
                    && (a.NU_IDENTIFICADOR == identificador || a.NU_IDENTIFICADOR == ManejoIdentificadorDb.IdentificadorAuto)
                    && a.CD_FAIXA == faixa
                    && a.VL_ATRIBUTOS.Contains(atributos)
                    && a.QT_RESERVA > 0);
            }
            else
            {
                return _context.T_DET_PICKING_LPN
                    .AsNoTracking()
                    .Any(a => a.TP_LPN_TIPO == tpLpn
                    && a.CD_ENDERECO == ubicacion
                    && a.CD_EMPRESA == empresa
                    && a.VL_ATRIBUTOS.StartsWith(atributos)
                    && a.QT_RESERVA > 0);
            }
        }

        public virtual bool ExisteTipoLpnExterno(string numeroSecuencia, string tipoLpn)
        {
            return this._context.T_LPN
                .AsNoTracking()
                .Any(x => x.TP_LPN_TIPO == tipoLpn
                    && x.ID_LPN_EXTERNO == numeroSecuencia);
        }

        public virtual bool AnyProductoDetPedidoLpnAtributo(string pedido, string cliente, int empresa, string producto, decimal faixa, string identificador, string idEspecificaIdentificador, string tipoLpn, string idExterno)
        {
            return this._context.T_DET_PEDIDO_SAIDA_LPN_ATRIB
                .AsNoTracking()
                .Any(d => d.NU_PEDIDO == pedido
                    && d.CD_CLIENTE == cliente
                    && d.CD_EMPRESA == empresa
                    && d.CD_PRODUTO == producto
                    && d.CD_FAIXA == faixa
                    && d.NU_IDENTIFICADOR == identificador
                    && d.ID_ESPECIFICA_IDENTIFICADOR == idEspecificaIdentificador
                    && d.TP_LPN_TIPO == tipoLpn
                    && d.ID_LPN_EXTERNO == idExterno);
        }

        public virtual bool AnyDetallePedidoLpnAtributoTrabajado(DetallePedidoLpnEspecifico datos)
        {
            return this._context.T_DET_PEDIDO_SAIDA_LPN_ATRIB
                .AsNoTracking()
                .Any(d => d.NU_PEDIDO == datos.Pedido
                    && d.CD_CLIENTE == datos.Cliente
                    && d.CD_EMPRESA == datos.Empresa
                    && d.CD_PRODUTO == datos.Producto
                    && d.CD_FAIXA == datos.Faixa
                    && d.NU_IDENTIFICADOR == datos.Identificador
                    && d.ID_ESPECIFICA_IDENTIFICADOR == datos.IdEspecificaIdentificador
                    && d.TP_LPN_TIPO == datos.TipoLpn
                    && d.ID_LPN_EXTERNO == datos.IdExternoLpn
                    && d.NU_DET_PED_SAI_ATRIB == datos.IdConfiguracion
                    && ((d.QT_LIBERADO ?? 0) > 0 || (d.QT_ANULADO ?? 0) > 0));
        }

        public virtual bool AnyDetallePedidoAtributoTrabajado(DetallePedidoLpnEspecifico datos)
        {
            return this._context.T_DET_PEDIDO_SAIDA_ATRIB
                .AsNoTracking()
                .Any(d => d.NU_PEDIDO == datos.Pedido
                    && d.CD_CLIENTE == datos.Cliente
                    && d.CD_EMPRESA == datos.Empresa
                    && d.CD_PRODUTO == datos.Producto
                    && d.CD_FAIXA == datos.Faixa
                    && d.NU_IDENTIFICADOR == datos.Identificador
                    && d.ID_ESPECIFICA_IDENTIFICADOR == datos.IdEspecificaIdentificador
                    && d.NU_DET_PED_SAI_ATRIB == datos.IdConfiguracion
                    && ((d.QT_LIBERADO ?? 0) > 0 || (d.QT_ANULADO ?? 0) > 0));
        }

        public virtual bool AnyDetallePedidoAtributoDefinicion(long idConfiguracion)
        {
            return this._context.T_DET_PEDIDO_SAIDA_ATRIB_DET
                .AsNoTracking()
                .Any(d => d.NU_DET_PED_SAI_ATRIB == idConfiguracion);
        }

        public virtual bool AnyAtributoLpnAsociado(DetallePedidoLpnEspecifico datos)
        {
            return _context.V_PRE100_ATRIBUTOS_LPN_DEFINIDOS
                .AsNoTracking()
                .Any(a => a.NU_PEDIDO == datos.Pedido
                    && a.CD_CLIENTE == datos.Cliente
                    && a.CD_EMPRESA == datos.Empresa
                    && a.CD_PRODUTO == datos.Producto
                    && a.CD_FAIXA == datos.Faixa
                    && a.NU_IDENTIFICADOR == datos.Identificador
                    && a.ID_ESPECIFICA_IDENTIFICADOR == datos.IdEspecificaIdentificador
                    && a.TP_LPN_TIPO == datos.TipoLpn
                    && a.ID_LPN_EXTERNO == datos.IdExternoLpn
                    && ((datos.IdConfiguracion.HasValue) ? (a.NU_DET_PED_SAI_ATRIB == datos.IdConfiguracion.Value || a.NU_DET_PED_SAI_ATRIB == -1) : a.NU_DET_PED_SAI_ATRIB == -1)
                    && (a.USERID == datos.UserId.Value || a.USERID == null));
        }

        public virtual bool AnyAtributoAsociadoDetPed(DetallePedidoLpnEspecifico datos)
        {
            return _context.V_PRE100_ATRIBUTOS_DEFINIDOS
                .AsNoTracking()
                .Any(a => a.NU_PEDIDO == datos.Pedido
                    && a.CD_CLIENTE == datos.Cliente
                    && a.CD_EMPRESA == datos.Empresa
                    && a.CD_PRODUTO == datos.Producto
                    && a.CD_FAIXA == datos.Faixa
                    && a.NU_IDENTIFICADOR == datos.Identificador
                    && a.ID_ESPECIFICA_IDENTIFICADOR == datos.IdEspecificaIdentificador
                    && ((datos.IdConfiguracion.HasValue) ? (a.NU_DET_PED_SAI_ATRIB == datos.IdConfiguracion.Value || a.NU_DET_PED_SAI_ATRIB == -1) : a.NU_DET_PED_SAI_ATRIB == -1)
                    && (a.USERID == datos.UserId.Value || a.USERID == null));
        }

        public virtual bool AnyProductoDetPedidoAtributo(string pedido, string cliente, int empresa, string producto, decimal faixa, string identificador, string idEspecificaIdentificador)
        {
            return this._context.T_DET_PEDIDO_SAIDA_ATRIB
                .AsNoTracking()
                .Any(d => d.NU_PEDIDO == pedido
                    && d.CD_CLIENTE == cliente
                    && d.CD_EMPRESA == empresa
                    && d.CD_PRODUTO == producto
                    && d.CD_FAIXA == faixa
                    && d.NU_IDENTIFICADOR == identificador
                    && d.ID_ESPECIFICA_IDENTIFICADOR == idEspecificaIdentificador);
        }

        public virtual bool ExistePrefijoEtiqueta(string prefijo)
        {
            return _context.T_LPN_TIPO.Any(x => x.VL_PREFIJO == prefijo);
        }

        public virtual bool AnyStockUbicacionLpn(string ubicacion, int empresa, string producto, string identificador, decimal faixa)
        {
            return _context.T_LPN_DET
                .Include("T_LPN")
                .AsNoTracking()
                .Any(x => x.T_LPN.CD_ENDERECO == ubicacion
                    && x.CD_EMPRESA == empresa
                    && x.NU_IDENTIFICADOR == identificador
                    && x.CD_PRODUTO == producto
                    && x.CD_FAIXA == faixa && x.QT_ESTOQUE > 0);
        }

        public virtual bool AnyDetalleAtributo(int idDetLpn)
        {
            return _context.T_LPN_DET_ATRIBUTO
                .AsNoTracking()
                .Any(a => a.ID_LPN_DET == idDetLpn);
        }

        public virtual bool AnyDetalleLpnConStock(long nuLpn)
        {
            return _context.T_LPN_DET.Any(d => d.NU_LPN == nuLpn && d.QT_ESTOQUE > 0);
        }

        public virtual bool AnyAuditoriaEnCurso(long nuLpn)
        {
            var estadosPermitidos = new List<string>() { EstadoAuditoriaLpn.Aprobada, EstadoAuditoriaLpn.Cancelada };

            return _context.T_LPN_AUDITORIA
                .AsNoTracking()
                .Any(a => a.NU_LPN == nuLpn
                    && !estadosPermitidos.Contains(a.ID_ESTADO));
        }

        public virtual bool AnyLpnActivo()
        {
            return this._context.T_LPN.Any(x => x.ID_ESTADO == EstadosLPN.Activo);
        }

        public virtual bool AnyLpnGeneradoConUbicacion()
        {
            return this._context.T_LPN.Any(x => x.ID_ESTADO == EstadosLPN.Generado && x.CD_ENDERECO != null);
        }

        #endregion

        #region Get

        public virtual long GetNextNuLpn()
        {
            return this._context.GetNextSequenceValueLong(_dapper, Secuencias.S_LPN);
        }

        public virtual int GetNextLpnBarras()
        {
            return this._context.GetNextSequenceValueInt(_dapper, Secuencias.S_LPN_BARRAS);
        }

        public virtual int GetNextIdDetalleLpn()
        {
            return this._context.GetNextSequenceValueInt(_dapper, Secuencias.S_LPN_DET);
        }

        public virtual List<LpnTipo> GetTiposLPN()
        {
            return this._context.T_LPN_TIPO
                .AsNoTracking()
                .Select(t => _lpnMapper.MapToObject(t))
                .ToList();
        }

        public virtual List<LpnTipo> GetAllTipoLPNByDescriptionOrCodePartial(string value)
        {
            return this._context.T_LPN_TIPO
                .AsNoTracking()
                .Where(d => d.DS_LPN_TIPO.ToLower().Contains(value.ToLower())
                    || d.TP_LPN_TIPO.ToLower().Contains(value.ToLower()))
                .Select(d => this._lpnMapper.MapToObject(d))
                .ToList();
        }

        public virtual string GetEstadoAuditoria(long numeroAuditoriaAgrupado)
        {
            return _context.V_STO730_AUDITORIA_LPN.FirstOrDefault(x => x.NU_AUDITORIA_AGRUPADOR == numeroAuditoriaAgrupado).ID_ESTADO;
        }

        public virtual string GetCodigoEstiloEtiquetaLPN(string tipoLpn)
        {
            var numeroTemplate = this._context.T_LPN_TIPO
                .AsNoTracking()
                .FirstOrDefault(x => x.TP_LPN_TIPO == tipoLpn).NU_TEMPLATE_ETIQUETA;

            return this._context.T_LABEL_ESTILO
                .AsNoTracking()
                .FirstOrDefault(x => x.CD_LABEL_ESTILO == numeroTemplate).CD_LABEL_ESTILO;
        }

        public virtual string GetEstiloEtiquetaLPN(string tipoLpn)
        {
            var numeroTemplate = this._context.T_LPN_TIPO
                .AsNoTracking()
                .FirstOrDefault(x => x.TP_LPN_TIPO == tipoLpn).NU_TEMPLATE_ETIQUETA;

            return this._context.T_LABEL_ESTILO
                .AsNoTracking()
                .FirstOrDefault(x => x.CD_LABEL_ESTILO == numeroTemplate).DS_LABEL_ESTILO;
        }

        public virtual AuditoriaLpn GetAuditoria(long nuAuditoria)
        {
            return _lpnMapper.MapToObject(this._context.T_LPN_AUDITORIA.AsNoTracking().FirstOrDefault(x => x.NU_AUDITORIA == nuAuditoria));

        }

        public virtual List<AuditoriaLpn> GetDetallesAuditadosAgrupados(long nuAuditoriaAgrupado)
        {
            return _lpnMapper.MapToObject(this._context.T_LPN_AUDITORIA.AsNoTracking().Where(x => x.NU_AUDITORIA_AGRUPADOR == nuAuditoriaAgrupado).OrderByDescending(x => x.ID_LPN_DET).ToList());

        }

        public virtual long? GetNumeroSequenciaTipoLpn(string tipo)
        {
            return this._context.T_LPN_TIPO
                .AsNoTracking()
                .FirstOrDefault(x => x.TP_LPN_TIPO == tipo).NU_SEQ_LPN;
        }

        public virtual List<LpnConsolidadorTipo> GetListaLpnConsolidadorTipo()
        {
            return this._context.T_LPN_CONSOLIDACION_TIPO
                .Where(x => x.ID_CONSOLIDACION_TIPO != ConsolidadorTipoDb.USAR_EL_ÚLTIMO_VALOR_CARGADO
                    && x.ID_CONSOLIDACION_TIPO != ConsolidadorTipoDb.SOLICITAR_AL_USUARIO)
                .AsNoTracking()
                .Select(t => _lpnMapper.MapToObject(t))
                .ToList();
        }

        public virtual Lpn GetLpn(long numeroLpn)
        {
            var entity = _context.T_LPN
                .Include("T_LPN_DET")
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_LPN == numeroLpn);

            return this._lpnMapper.MapToObject(entity);
        }

        public virtual LpnTipo GetTipoLpn(string value)
        {
            T_LPN_TIPO attachedEntity = _context.T_LPN_TIPO
                .AsNoTracking()
                .FirstOrDefault(x => x.TP_LPN_TIPO == value);

            return this._lpnMapper.MapToObject(attachedEntity);

        }

        public virtual List<LpnBarras> GetCodigoDeBarras(long numeroLPN)
        {
            return this._context.T_LPN_BARRAS
                .AsNoTracking()
                .Where(x => x.NU_LPN == numeroLPN)
                .OrderBy(x => x.NU_ORDEN)
                .Select(t => _lpnMapper.MapToObject(t))
                .ToList();
        }

        public virtual List<LpnDetalle> GetDetallesLpn(long numeroLPN)
        {
            return this._context.T_LPN_DET
                .AsNoTracking()
                .Where(x => x.NU_LPN == numeroLPN)
                .Select(t => _lpnMapper.MapToObject(t))
                .ToList();
        }

        public virtual LpnDetalle GetDetalleLpnByIdDetalle(long nuLpn, int idDetalle)
        {
            var entity = this._context.T_LPN_DET
                .FirstOrDefault(x =>
                x.NU_LPN == nuLpn &&
                x.ID_LPN_DET == idDetalle);

            return this._lpnMapper.MapToObject(entity);
        }

        public virtual List<LpnAtributo> GetAllLpnAtributo(long numeroLPN)
        {
            return this._context.T_LPN_ATRIBUTO
                .Include("T_ATRIBUTO")
                .AsNoTracking()
                .Where(x => x.NU_LPN == numeroLPN)
                .Select(t => _lpnMapper.MapToObject(t))
                .ToList();
        }

        public virtual LpnAtributo GetLpnAtributo(long nuLpn, int idAtributo, string lpnTipo)
        {
            var entity = this._context.T_LPN_ATRIBUTO
                .Include("T_ATRIBUTO")
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_LPN == nuLpn
                    && x.ID_ATRIBUTO == idAtributo
                    && x.TP_LPN_TIPO == lpnTipo);

            return this._lpnMapper.MapToObject(entity);
        }

        public virtual List<LpnDetalleAtributo> GetAllLpnDetalleAtributo(long numeroLPN)
        {
            return this._context.T_LPN_DET_ATRIBUTO
                .Include("T_ATRIBUTO")
                .AsNoTracking()
                .Where(x => x.NU_LPN == numeroLPN)
                .Select(t => _lpnMapper.MapToObject(t))
                .ToList();
        }

        public virtual List<LpnDetalleAtributo> GetAllLpnDetalleAtributo(long numeroLPN, int idDetLpn, int empresa, string producto, string identificador, decimal faixa)
        {
            return this._context.T_LPN_DET_ATRIBUTO
                .Include("T_ATRIBUTO")
                .AsNoTracking()
                .Where(x => x.NU_LPN == numeroLPN
                    && x.ID_LPN_DET == idDetLpn
                    && x.CD_EMPRESA == empresa
                    && x.CD_PRODUTO == producto
                    && x.CD_FAIXA == faixa
                    && x.NU_IDENTIFICADOR == identificador)
                .Select(t => _lpnMapper.MapToObject(t))
                .ToList();
        }

        public virtual List<LpnDetalleAtributo> GetAllLpnDetalleAtributo(long numeroLPN, int empresa, string producto, string identificador, decimal faixa)
        {
            return this._context.T_LPN_DET_ATRIBUTO
                .Include("T_ATRIBUTO")
                .AsNoTracking()
                .Where(x => x.NU_LPN == numeroLPN
                    && x.CD_EMPRESA == empresa
                    && x.CD_PRODUTO == producto
                    && x.CD_FAIXA == faixa
                    && x.NU_IDENTIFICADOR == identificador)
                .Select(t => _lpnMapper.MapToObject(t))
                .ToList();
        }

        public virtual List<LpnAuditoriaAtributo> GetAtributosAuditados(long auditoria)
        {
            return this._context.T_LPN_AUDITORIA_ATRIBUTO
               .AsNoTracking()
               .Where(x => x.NU_AUDITORIA == auditoria)
               .Select(t => _lpnMapper.MapToObject(t))
               .ToList();
        }

        public virtual List<LpnTipoAtributo> GetTipoAsociadoAtributo(string tipo)
        {
            return this._context.T_LPN_TIPO_ATRIBUTO
                .AsNoTracking()
                .Where(x => x.TP_LPN_TIPO == tipo)
                .Select(t => _lpnMapper.MapToObject(t))
                .ToList();
        }

        public virtual LpnTipoAtributo GetLpnTipoAtributo(int idAtributo, string tpLpn)
        {
            T_LPN_TIPO_ATRIBUTO entities = this._context.T_LPN_TIPO_ATRIBUTO
                .AsNoTracking()
                .FirstOrDefault(x => x.TP_LPN_TIPO == tpLpn && x.ID_ATRIBUTO == idAtributo);

            if (entities == null)
                return null;

            return _lpnMapper.MapToObject(entities);

        }

        public virtual LpnDetalle GetDetalleEtiquetaLpn(long nuLpn, int idLpnDet, string cdProduto, int cdEmpresa, string identificador, decimal cdFaixa)
        {
            return this._lpnMapper.MapToObject(_context.T_LPN_DET
               .AsNoTracking()
               .FirstOrDefault(x => x.NU_LPN == nuLpn
                   && x.ID_LPN_DET == idLpnDet
                   && x.CD_PRODUTO == cdProduto
                   && x.CD_EMPRESA == cdEmpresa
                   && x.NU_IDENTIFICADOR == identificador
                   && x.CD_FAIXA == cdFaixa));
        }

        public virtual LpnDetalleAtributo GetLpnDetalleAtributo(int idLpnDet, long numeroLPN, int IdAtributo, string lpnTipo, string prodructo, int empresa, string identificador)
        {
            var entity = this._context.T_LPN_DET_ATRIBUTO
                .Include("T_ATRIBUTO")
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_LPN == numeroLPN
                    && x.ID_LPN_DET == idLpnDet
                    && x.ID_ATRIBUTO == IdAtributo
                    && x.TP_LPN_TIPO == lpnTipo
                    && x.CD_PRODUTO == prodructo
                    && x.CD_EMPRESA == empresa
                    && x.NU_IDENTIFICADOR == identificador);

            return this._lpnMapper.MapToObject(entity);
        }

        public virtual LpnDetalle AgregarDetalleLpnRecepcion(Agenda agenda, long nuLpn, string tpLpnTipo, string cdProduto, int cdEmpresa, string nuIdentificador, decimal cdFaixa, decimal cantProduto, DateTime? dtFabricacao, IIdentityService identity, long nuTransaccionDB)
        {
            LpnDetalle new_detalle = new LpnDetalle();

            new_detalle.Id = _context.GetNextSequenceValueInt(_dapper, Secuencias.S_LPN_DET);
            new_detalle.NumeroLPN = nuLpn;
            new_detalle.CodigoProducto = cdProduto;
            new_detalle.Faixa = cdFaixa;
            new_detalle.Empresa = cdEmpresa;
            new_detalle.Lote = nuIdentificador;
            new_detalle.Cantidad = cantProduto;
            new_detalle.NumeroTransaccion = nuTransaccionDB;
            new_detalle.Vencimiento = dtFabricacao;
            new_detalle.CantidadRecibida = cantProduto;
            new_detalle.CantidadDeclarada = 0;
            new_detalle.CantidadReserva = 0;
            new_detalle.IdLineaSistemaExterno = "1";
            new_detalle.CantidadExpedida = 0;
            new_detalle.IdAveria = "N";
            new_detalle.IdCtrlCalidad = EstadoControlCalidad.Controlado;
            new_detalle.IdInventario = "R";

            _context.T_LPN_DET.Add(_lpnMapper.MapToEntity(new_detalle));

            var listaAtributosDefault = GetAllLpnTipoAtributoDet(tpLpnTipo);

            foreach (var lpnDetalleAtributo in listaAtributosDefault)
            {
                T_LPN_DET_ATRIBUTO lpnDetAtributo = new T_LPN_DET_ATRIBUTO();

                lpnDetAtributo.ID_LPN_DET = new_detalle.Id;
                lpnDetAtributo.NU_LPN = nuLpn;
                lpnDetAtributo.CD_PRODUTO = cdProduto;
                lpnDetAtributo.TP_LPN_TIPO = tpLpnTipo;
                lpnDetAtributo.CD_FAIXA = cdFaixa;
                lpnDetAtributo.CD_EMPRESA = cdEmpresa;
                lpnDetAtributo.NU_IDENTIFICADOR = nuIdentificador;
                lpnDetAtributo.ID_ATRIBUTO = lpnDetalleAtributo.IdAtributo;
                lpnDetAtributo.NU_TRANSACCION = nuTransaccionDB;

                Atributo atributo = GetAtributo(lpnDetalleAtributo.IdAtributo);

                if (atributo.IdTipo != TipoAtributoDb.SISTEMA)
                {
                    lpnDetAtributo.VL_LPN_DET_ATRIBUTO = lpnDetalleAtributo.ValorInicial;
                    lpnDetAtributo.ID_ESTADO = lpnDetalleAtributo.EstadoInicial;
                }
                else
                {
                    if (agenda != null)
                    {
                        string[] NM_CAMPO = atributo.Campo.Split('.');

                        string sql = @"SELECT " + NM_CAMPO[1] + @" VALOR FROM " + NM_CAMPO[0] + @" WHERE 
                                            NU_AGENDA = :NU_AGENDA";

                        lpnDetAtributo.VL_LPN_DET_ATRIBUTO = _dapper.Query<string>(_context.Database.GetDbConnection(), sql, new Dictionary<string, object>
                        {
                            { ":NU_AGENDA", agenda.Id  }
                        }, transaction: _context.Database.CurrentTransaction?.GetDbTransaction()).FirstOrDefault();

                        lpnDetAtributo.ID_ESTADO = EstadoLpnAtributo.Ingresado;
                    }
                }

                _context.T_LPN_DET_ATRIBUTO.Add(lpnDetAtributo);
            }

            return new_detalle;
        }

        public virtual int GetCantidadAtributoTipoDet(string tplpn)
        {
            return (this._context.T_LPN_TIPO_ATRIBUTO_DET.AsNoTracking().Where(x => x.TP_LPN_TIPO == tplpn).Count() + 1);
        }

        public virtual Atributo GetAtributo(int idAtributo)
        {
            var atributo = this._context.T_ATRIBUTO
                .AsNoTracking()
                .FirstOrDefault(x => x.ID_ATRIBUTO == idAtributo);

            return this._atributoMapper.MapToObject(atributo);
        }

        public virtual int GetCantidadDeTipoLpnPrefijoExistente(string letraPrefrijo)
        {
            return _context.T_LPN_TIPO
                .Where(x => x.TP_ETIQUETA_RECEPCION.StartsWith(letraPrefrijo))
                .Count();
        }

        public virtual LpnTipoAtributoDet GetLpnAtributoTipoDet(int idAtributo, string tpLpn)
        {
            T_LPN_TIPO_ATRIBUTO_DET entities = this._context.T_LPN_TIPO_ATRIBUTO_DET
                .AsNoTracking()
                .FirstOrDefault(x => x.TP_LPN_TIPO == tpLpn && x.ID_ATRIBUTO == idAtributo);

            if (entities == null)
                return null;

            return _lpnMapper.MapToObject(entities);
        }

        public virtual List<LpnTipoAtributoDet> GetAllLpnTipoAtributoDet(string lpnTipo)
        {
            List<LpnTipoAtributoDet> listAtributos = new List<LpnTipoAtributoDet>();

            var entities = this._context.T_LPN_TIPO_ATRIBUTO_DET
                .AsNoTracking()
                .Where(x => x.TP_LPN_TIPO == lpnTipo).ToList();

            foreach (var entiti in entities)
            {
                listAtributos.Add(_lpnMapper.MapToObject(entiti));
            }

            return listAtributos;
        }

        public virtual IEnumerable<long> GetNumeroLpnByAgenda(int nuAgenda)
        {
            return _context.T_LPN
                .Where(l => l.NU_AGENDA == nuAgenda)
                .Select(l => l.NU_LPN);
        }

        public virtual Lpn GetLpnByCodigoBarras(string codigoBarras, int? empresa)
        {
            var entity = _context.T_LPN_BARRAS
                .Join(_context.T_LPN.Where(l => empresa.HasValue ? l.CD_EMPRESA == empresa.Value : true),
                    cb => cb.NU_LPN,
                    l => l.NU_LPN,
                    (cb, l) => new { Barras = cb, Lpn = l })
                .Where(x => x.Barras.CD_BARRAS == codigoBarras)
                .OrderByDescending(x => x.Lpn.NU_LPN)
                .AsNoTracking()
                .Select(x => x.Lpn)
                .FirstOrDefault();

            return _lpnMapper.MapToObject(entity);
        }

        public virtual int? GetFirstEmpresaWithCodigoBarrasMultidato(string codigoBarras, int userId, int? empresa, out int cantidad)
        {
            var empresas = _context.T_LPN_BARRAS
                .Join(_context.T_LPN,
                    cb => new { cb.NU_LPN },
                    l => new { l.NU_LPN },
                    (cb, l) => new { Barras = cb, Lpn = l })
                .Join(_context.T_CODIGO_MULTIDATO_EMPRESA,
                    cbl => new { cbl.Lpn.CD_EMPRESA },
                    cm => new { cm.CD_EMPRESA },
                    (cbl, cm) => new { cbl.Barras, cbl.Lpn, Multidato = cm })
                .Join(_context.T_EMPRESA_FUNCIONARIO,
                    c => new { c.Lpn.CD_EMPRESA },
                    ef => new { ef.CD_EMPRESA },
                    (c, ef) => new { Codigo = c, EmpresaFuncionario = ef })
                .Where(x => x.Codigo.Barras.CD_BARRAS == codigoBarras
                    && x.Codigo.Multidato.CD_CODIGO_MULTIDATO == TipoCodigoMultidato.EAN128
                    && x.Codigo.Multidato.FL_HABILITADO == "S"
                    && x.EmpresaFuncionario.USERID == userId
                    && (empresa == null || x.EmpresaFuncionario.CD_EMPRESA == empresa))
                .GroupBy(x => new { x.Codigo.Lpn.CD_EMPRESA })
                .OrderBy(x => x.Key.CD_EMPRESA)
                .AsNoTracking()
                .Select(x => x.Key.CD_EMPRESA);

            cantidad = empresas.Count();

            return empresas.FirstOrDefault();
        }

        public virtual List<Lpn> GetLpnByIdExternoPartial(string tipoLpn, int empresa, string producto, string identificador, string value)
        {
            var estadosHabilitados = new List<string>() { EstadosLPN.Importado, EstadosLPN.Activo };

            return _context.T_LPN
                .Include("T_LPN_DET")
                .AsNoTracking()
                .Where(l => l.CD_EMPRESA == empresa
                    && l.TP_LPN_TIPO == tipoLpn
                    && estadosHabilitados.Contains(l.ID_ESTADO)
                    && l.T_LPN_DET.Any(d => d.CD_EMPRESA == empresa
                        && d.CD_PRODUTO == producto
                        && ((!string.IsNullOrEmpty(identificador) && identificador != ManejoIdentificadorDb.IdentificadorAuto) ? d.NU_IDENTIFICADOR == identificador : true)
                        && (d.QT_ESTOQUE - (d.QT_RESERVA_SAIDA ?? 0)) > 0)
                    && (l.ID_LPN_EXTERNO.ToLower().Contains(value.ToLower())))
                .Select(l => this._lpnMapper.MapToObject(l))
                .ToList();
        }

        public virtual Lpn GetLpnByIdExternoTipo(string tipoLpn, string idExterno)
        {
            var entity = _context.T_LPN
                .Include("T_LPN_DET")
                .AsNoTracking()
                .FirstOrDefault(x => x.TP_LPN_TIPO == tipoLpn
                    && x.ID_LPN_EXTERNO == idExterno
                    && x.ID_ESTADO != EstadosLPN.Finalizado);

            return this._lpnMapper.MapToObject(entity);
        }

        public virtual DetallePedidoLpn GetDetallePedidoLpn(string pedido, string cliente, int empresa, string producto, decimal faixa, string identificador, string idEspecificaIdentificador, string tipoLpn, string idExterno)
        {
            var entity = _context.T_DET_PEDIDO_SAIDA_LPN
                .AsNoTracking()
                .FirstOrDefault(d => d.NU_PEDIDO == pedido
                    && d.CD_CLIENTE == cliente
                    && d.CD_EMPRESA == empresa
                    && d.CD_PRODUTO == producto
                    && d.CD_FAIXA == faixa
                    && d.NU_IDENTIFICADOR == identificador
                    && d.ID_ESPECIFICA_IDENTIFICADOR == idEspecificaIdentificador
                    && d.TP_LPN_TIPO == tipoLpn
                    && d.ID_LPN_EXTERNO == idExterno);

            return this._lpnMapper.MapToObject(entity);
        }

        public virtual List<DetallePedidoLpnAtributo> GetDetallesPedidoLpnAtributo(string pedido, string cliente, int empresa, string producto, decimal faixa, string identificador, string idEspecificaIdentificador, string tipoLpn, string idExterno)
        {
            return _context.T_DET_PEDIDO_SAIDA_LPN_ATRIB
                .AsNoTracking()
                .Where(d => d.NU_PEDIDO == pedido
                    && d.CD_CLIENTE == cliente
                    && d.CD_EMPRESA == empresa
                    && d.CD_PRODUTO == producto
                    && d.CD_FAIXA == faixa
                    && d.NU_IDENTIFICADOR == identificador
                    && d.ID_ESPECIFICA_IDENTIFICADOR == idEspecificaIdentificador
                    && d.TP_LPN_TIPO == tipoLpn
                    && d.ID_LPN_EXTERNO == idExterno)
                .Select(d => _lpnMapper.MapToObject(d))
                .ToList();
        }

        public virtual DetallePedidoLpnAtributo GetDetallePedidoLpnAtributo(string pedido, string cliente, int empresa, string producto, decimal faixa, string identificador, string idEspecificaIdentificador, string tipoLpn, string idExterno, long idConfiguracion)
        {
            return _lpnMapper.MapToObject(_context.T_DET_PEDIDO_SAIDA_LPN_ATRIB
                .AsNoTracking()
                .FirstOrDefault(d => d.NU_PEDIDO == pedido
                    && d.CD_CLIENTE == cliente
                    && d.CD_EMPRESA == empresa
                    && d.CD_PRODUTO == producto
                    && d.CD_FAIXA == faixa
                    && d.NU_IDENTIFICADOR == identificador
                    && d.ID_ESPECIFICA_IDENTIFICADOR == idEspecificaIdentificador
                    && d.TP_LPN_TIPO == tipoLpn
                    && d.ID_LPN_EXTERNO == idExterno
                    && d.NU_DET_PED_SAI_ATRIB == idConfiguracion));
        }

        public virtual DetallePedidoAtributoDefinicion GetDetallePedidoAtributoDefinicion(long idConfiguracion, int idAtributo, string idCabezal)
        {
            return _lpnMapper.MapToObject(
                _context.T_DET_PEDIDO_SAIDA_ATRIB_DET
                .FirstOrDefault(a => a.NU_DET_PED_SAI_ATRIB == idConfiguracion
                    && a.ID_ATRIBUTO == idAtributo
                    && a.FL_CABEZAL == idCabezal));
        }

        public virtual DetallePedidoAtributoLpnTemporal GetDetallePedidoAtributoLpnTemporal(string pedido, string cliente, int empresa, string producto, decimal faixa, string identificador, string idEspecificaIdentificador, string tipoLpn, string idExterno, int idAtributo, int userId, string idCabezal)
        {
            return _lpnMapper.MapToObject(
                _context.T_TEMP_DET_PEDIDO_SAIDA_LPN_ATRIB
                .FirstOrDefault(a => a.NU_PEDIDO == pedido
                    && a.CD_CLIENTE == cliente
                    && a.CD_EMPRESA == empresa
                    && a.CD_PRODUTO == producto
                    && a.CD_FAIXA == faixa
                    && a.NU_IDENTIFICADOR == identificador
                    && a.ID_ESPECIFICA_IDENTIFICADOR == idEspecificaIdentificador
                    && a.TP_LPN_TIPO == tipoLpn
                    && a.ID_LPN_EXTERNO == idExterno
                    && a.ID_ATRIBUTO == idAtributo
                    && a.USERID == userId
                    && a.FL_CABEZAL == idCabezal));
        }

        public virtual long GetNextIdConfiguracion()
        {
            return this._context.GetNextSequenceValueLong(_dapper, Secuencias.S_DET_PEDIDO_SAIDA_ATRIB);
        }

        public virtual List<DetallePedidoAtributoLpnTemporal> GetDetallesPedidoAtributoLpnTemporal(string pedido, string cliente, int empresa, string producto, decimal faixa, string identificador, string idEspecificaIdentificador, string tipoLpn, string idExterno, int userId)
        {
            return _context.T_TEMP_DET_PEDIDO_SAIDA_LPN_ATRIB
                .AsNoTracking()
                .Where(d => d.NU_PEDIDO == pedido
                    && d.CD_CLIENTE == cliente
                    && d.CD_EMPRESA == empresa
                    && d.CD_PRODUTO == producto
                    && d.CD_FAIXA == faixa
                    && d.NU_IDENTIFICADOR == identificador
                    && d.ID_ESPECIFICA_IDENTIFICADOR == idEspecificaIdentificador
                    && d.TP_LPN_TIPO == tipoLpn
                    && d.ID_LPN_EXTERNO == idExterno
                    && d.USERID == userId)
                .Select(d => _lpnMapper.MapToObject(d))
                .ToList();
        }

        public virtual List<DetallePedidoAtributoTemporal> GetDetallesPedidoAtributoTemporal(string pedido, string cliente, int empresa, string producto, decimal faixa, string identificador, string idEspecificaIdentificador, int userId)
        {
            return _context.T_TEMP_DET_PEDIDO_SAIDA_ATRIB
                .AsNoTracking()
                .Where(d => d.NU_PEDIDO == pedido
                    && d.CD_CLIENTE == cliente
                    && d.CD_EMPRESA == empresa
                    && (!string.IsNullOrEmpty(producto) ? (d.CD_PRODUTO == producto
                        && d.CD_FAIXA == faixa
                        && d.NU_IDENTIFICADOR == identificador
                        && d.ID_ESPECIFICA_IDENTIFICADOR == idEspecificaIdentificador) : true)
                    && d.USERID == userId)
                .Select(d => _lpnMapper.MapToObject(d))
                .ToList();
        }

        public virtual DetallePedidoAtributo GetDetallePedidoAtributo(string pedido, string cliente, int empresa, string producto, decimal faixa, string identificador, string idEspecificaIdentificador, long idConfiguracion)
        {
            return _lpnMapper.MapToObject(_context.T_DET_PEDIDO_SAIDA_ATRIB
                .AsNoTracking()
                .FirstOrDefault(d => d.NU_PEDIDO == pedido
                    && d.CD_CLIENTE == cliente
                    && d.CD_EMPRESA == empresa
                    && d.CD_PRODUTO == producto
                    && d.CD_FAIXA == faixa
                    && d.NU_IDENTIFICADOR == identificador
                    && d.ID_ESPECIFICA_IDENTIFICADOR == idEspecificaIdentificador
                    && d.NU_DET_PED_SAI_ATRIB == idConfiguracion));
        }

        public virtual DetallePedidoAtributoTemporal GetDetallePedidoAtributoTemporal(string pedido, string cliente, int empresa, string producto, decimal faixa, string identificador, string idEspecificaIdentificador, int idAtributo, int userId, string idCabezal)
        {
            return _lpnMapper.MapToObject(
                _context.T_TEMP_DET_PEDIDO_SAIDA_ATRIB
                .FirstOrDefault(a => a.NU_PEDIDO == pedido
                    && a.CD_CLIENTE == cliente
                    && a.CD_EMPRESA == empresa
                    && a.CD_PRODUTO == producto
                    && a.CD_FAIXA == faixa
                    && a.NU_IDENTIFICADOR == identificador
                    && a.ID_ESPECIFICA_IDENTIFICADOR == idEspecificaIdentificador
                    && a.ID_ATRIBUTO == idAtributo
                    && a.USERID == userId
                    && a.FL_CABEZAL == idCabezal));
        }

        public virtual LpnDetalle GetDetalleLpnByAtributos(long nuLpn, string cdProduto, int cdEmpresa, string nuIdentificador, decimal cdFaixa, List<LpnAuditoriaAtributo> atributosDetalleLpn)
        {
            var detalles = _context.T_LPN_DET
                .Join(_context.T_LPN_DET_ATRIBUTO,
                    ld => new { ld.ID_LPN_DET, ld.NU_LPN, ld.CD_PRODUTO, ld.CD_FAIXA, ld.CD_EMPRESA, ld.NU_IDENTIFICADOR },
                    lda => new { lda.ID_LPN_DET, lda.NU_LPN, lda.CD_PRODUTO, lda.CD_FAIXA, lda.CD_EMPRESA, lda.NU_IDENTIFICADOR },
                    (lda, ld) => new { Detalle = lda, AtributoDetalleLpn = ld })
                .Where(x => x.Detalle.NU_LPN == nuLpn
                    && x.Detalle.CD_PRODUTO == cdProduto
                    && x.Detalle.CD_FAIXA == cdFaixa
                    && x.Detalle.CD_EMPRESA == cdEmpresa
                    && x.Detalle.NU_IDENTIFICADOR == nuIdentificador)
                .Select(x => new { x.Detalle, x.AtributoDetalleLpn });

            foreach (var atributoDetalleLpn in atributosDetalleLpn)
            {
                Atributo atributo = GetAtributo(atributoDetalleLpn.IdAtributo);
                detalles = detalles
                    .Where(x => x.AtributoDetalleLpn.ID_ATRIBUTO == atributoDetalleLpn.IdAtributo
                        && x.AtributoDetalleLpn.VL_LPN_DET_ATRIBUTO == atributoDetalleLpn.ValorAtributo);
            }

            return _lpnMapper.MapToObject(detalles.FirstOrDefault()?.Detalle);
        }

        public virtual List<DetallePedidoLpn> GetDetallesPedidoLpn(string id, string cliente, int empresa, string producto, string identificador, decimal faixa)
        {
            return _context.T_DET_PEDIDO_SAIDA_LPN
                .AsNoTracking()
                .Where(x => x.NU_PEDIDO == id
                    && x.CD_CLIENTE == cliente
                    && x.CD_EMPRESA == empresa
                    && x.CD_PRODUTO == producto
                    && x.NU_IDENTIFICADOR == identificador
                    && x.CD_FAIXA == faixa)
                .Select(d => this._lpnMapper.MapToObject(d))
                .ToList();
        }

        public virtual DetallePedidoLpn GetDetallePedidoLpn(string id, string idExterno, string tipoLpn, string cliente, int empresa, string producto, string identificador, decimal faixa)
        {
            return this._lpnMapper.MapToObject(_context.T_DET_PEDIDO_SAIDA_LPN
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_PEDIDO == id
                    && x.CD_CLIENTE == cliente
                    && x.CD_EMPRESA == empresa
                    && x.CD_PRODUTO == producto
                    && x.NU_IDENTIFICADOR == identificador
                    && x.CD_FAIXA == faixa));
        }

        public virtual List<DetallePedidoLpnAtributo> GetDetallesPedidoAtributoLpn(string id, string cliente, int empresa, string producto, string identificador, decimal faixa)
        {
            return _context.T_DET_PEDIDO_SAIDA_LPN_ATRIB
                .AsNoTracking()
                .Where(x => x.NU_PEDIDO == id
                    && x.CD_CLIENTE == cliente
                    && x.CD_EMPRESA == empresa
                    && x.CD_PRODUTO == producto
                    && x.NU_IDENTIFICADOR == identificador
                    && x.CD_FAIXA == faixa
                    && (x.QT_PEDIDO - (x.QT_LIBERADO ?? 0) - (x.QT_ANULADO ?? 0)) > 0)
                .Select(d => this._lpnMapper.MapToObject(d))
                .ToList();
        }

        public virtual List<DetallePedidoAtributo> GetDetallesPedidoAtributo(string id, string cliente, int empresa, string producto, string identificador, decimal faixa)
        {
            return _context.T_DET_PEDIDO_SAIDA_ATRIB
                .AsNoTracking()
                .Where(x => x.NU_PEDIDO == id
                    && x.CD_CLIENTE == cliente
                    && x.CD_EMPRESA == empresa
                    && x.CD_PRODUTO == producto
                    && x.NU_IDENTIFICADOR == identificador
                    && x.CD_FAIXA == faixa)
                .Select(d => this._lpnMapper.MapToObject(d))
                .ToList();
        }

        public virtual List<LpnDetalle> GetDetallesLpn(long nuLpn, int? idLpnDet, string cdProduto, int cdEmpresa, string identificador, decimal cdFaixa)
        {
            if (idLpnDet != null)
            {
                return this._context.T_LPN_DET
                    .AsNoTracking()
                    .Where(x => x.NU_LPN == nuLpn
                        && x.CD_PRODUTO == cdProduto
                        && x.CD_EMPRESA == cdEmpresa
                        && x.NU_IDENTIFICADOR == identificador
                        && x.CD_FAIXA == cdFaixa
                        && x.ID_LPN_DET == idLpnDet
                        && x.QT_ESTOQUE > 0)
                    .Select(t => _lpnMapper.MapToObject(t))
                    .ToList();
            }
            else
            {
                return this._context.T_LPN_DET
                      .AsNoTracking()
                      .Where(x => x.NU_LPN == nuLpn
                          && x.CD_PRODUTO == cdProduto
                          && x.CD_EMPRESA == cdEmpresa
                          && x.NU_IDENTIFICADOR == identificador
                          && x.CD_FAIXA == cdFaixa
                          && x.QT_ESTOQUE > 0)
                       .Select(t => _lpnMapper.MapToObject(t))
                      .ToList();
            }
        }

        public virtual void ValidarCantidadesAuditadasContraReservas(Lpn cabezalLpn, long? agrupador, int empresa, string producto, string identificador, decimal faixa, string nivel)
        {
            if (_context.T_LPN_AUDITORIA
                .Join(_context.T_LPN_DET,
                    la => new { la.NU_LPN, la.ID_LPN_DET, la.CD_EMPRESA, la.CD_PRODUTO, la.CD_FAIXA, la.NU_IDENTIFICADOR },
                    ld => new { ld.NU_LPN, ID_LPN_DET = (int?)ld.ID_LPN_DET, ld.CD_EMPRESA, ld.CD_PRODUTO, ld.CD_FAIXA, ld.NU_IDENTIFICADOR },
                    (la, ld) => new { DetalleAuditoria = la, DetalleLpn = ld })
                .AsNoTracking()
                .Any(x => x.DetalleLpn.NU_LPN == cabezalLpn.NumeroLPN
                    && x.DetalleAuditoria.CD_EMPRESA == empresa
                    && x.DetalleAuditoria.NU_IDENTIFICADOR == identificador
                    && x.DetalleAuditoria.CD_PRODUTO == producto
                    && x.DetalleAuditoria.CD_FAIXA == faixa
                    && (x.DetalleAuditoria.ID_ESTADO == EstadoAuditoriaLpn.Pendiente || x.DetalleAuditoria.ID_ESTADO == EstadoAuditoriaLpn.EnProgreso)
                    && x.DetalleAuditoria.ID_NIVEL == nivel
                    && x.DetalleAuditoria.NU_AUDITORIA_AGRUPADOR == agrupador
                    && x.DetalleAuditoria.QT_AUDITADA < x.DetalleLpn.QT_RESERVA_SAIDA))
            {
                throw new Exception("STO700_Sec0_Error_LaReservaDelLpnCambio");
            }
        }

        public virtual decimal GetCantidadStockDisponibleDetalleLpn(long nuLpn, string ubicacion, int cdEmpresa, string producto, string identificador, decimal faixa, int? idLpnDet = null)
        {
            var disponibles = GetCantidadStockDetallesLpnDisponible(nuLpn, ubicacion, cdEmpresa, producto, identificador, faixa);

            var disponible = disponibles
                .Where(x => idLpnDet == null || x.DetalleLpn == idLpnDet)
                .Sum(x => x.Disponible) ?? 0;

            return disponible;
        }

        public virtual List<LpnDetalleDisponible> GetCantidadStockDetallesLpnDisponible(long nuLpn, string ubicacion, int cdEmpresa, string producto, string identificador, decimal faixa)
        {

            var disponibleAtributos = _context.V_STOCK_LPN
              .AsNoTracking()
              .Where(ld => ld.CD_ENDERECO == ubicacion
                  && ld.CD_PRODUTO == producto
                  && ld.CD_EMPRESA == cdEmpresa
                  && ld.NU_IDENTIFICADOR == identificador
                  && ld.CD_FAIXA == faixa
                  && ld.QT_ESTOQUE > 0
                  && ld.ID_AVERIA == "N"
                  && ld.ID_CTRL_CALIDAD == "C"
                  && ld.ID_INVENTARIO == "R")
              .Join(_context.T_PRODUTO,
                  ld => new { ld.CD_EMPRESA, ld.CD_PRODUTO },
                  p => new { p.CD_EMPRESA, p.CD_PRODUTO },
                  (ld, p) => new { DetalleLpn = ld, Producto = p })
              .Where(ldp => ldp.DetalleLpn.DT_FABRICACAO == null
                 || (ldp.Producto.TP_MANEJO_FECHA == ManejoFechaProductoDb.Fifo)
                 || (ldp.Producto.TP_MANEJO_FECHA == ManejoFechaProductoDb.Expirable && (DateTime.Today.AddDays((double)(ldp.Producto.QT_DIAS_VALIDADE == null ? 0 : ldp.Producto.QT_DIAS_VALIDADE)) <= ldp.DetalleLpn.DT_FABRICACAO)))
              .Select(ldp => ldp.DetalleLpn)
              .GroupBy(dlrlp => new
              {
                  dlrlp.VL_ATRIBUTOS
              })
              .Select(g => new
              {
                  g.Key.VL_ATRIBUTOS,
                  QT_DISPONIBLE = g.Sum(x => x.QT_ESTOQUE - (x.QT_RESERVA_SAIDA == null ? 0 : x.QT_RESERVA_SAIDA))
              });

            var reservaAtributos = _context.T_DET_PICKING_LPN
                .Where(dpl => dpl.CD_ENDERECO == ubicacion
                    && dpl.CD_EMPRESA == cdEmpresa
                    && dpl.CD_PRODUTO == producto
                    && dpl.CD_FAIXA == faixa
                    && (dpl.NU_IDENTIFICADOR == identificador || dpl.NU_IDENTIFICADOR == ManejoIdentificadorDb.IdentificadorAuto)
                    && dpl.QT_RESERVA > 0
                    && dpl.VL_ATRIBUTOS != null)
                .GroupBy(dpl => new
                {
                    dpl.VL_ATRIBUTOS
                })
                .Select(g => new
                {
                    g.Key.VL_ATRIBUTOS,
                    QT_RESERVA = (decimal?)g.Sum(dpl => dpl.QT_RESERVA)
                });

            var stockLpn = _context.V_STOCK_LPN
                .AsNoTracking()
                .Where(ld => ld.NU_LPN == nuLpn
                   && ld.CD_ENDERECO == ubicacion
                   && ld.CD_PRODUTO == producto
                   && ld.CD_EMPRESA == cdEmpresa
                   && ld.NU_IDENTIFICADOR == identificador
                   && ld.CD_FAIXA == faixa
                   && ld.QT_ESTOQUE > 0
                   && ld.ID_AVERIA == "N"
                   && ld.ID_CTRL_CALIDAD == "C"
                   && ld.ID_INVENTARIO == "R")
                .Join(_context.T_PRODUTO,
                   ld => new { ld.CD_EMPRESA, ld.CD_PRODUTO },
                   p => new { p.CD_EMPRESA, p.CD_PRODUTO },
                   (ld, p) => new { DetalleLpn = ld, Producto = p })
                .Join(_context.T_ENDERECO_ESTOQUE,
                   ld => ld.DetalleLpn.CD_ENDERECO,
                   ee => ee.CD_ENDERECO,
                   (ld, ee) => new { Ubicacion = ee, DetalleLpn = ld.DetalleLpn, Producto = ld.Producto, ee })
                .Join(_context.T_TIPO_AREA,
                   ee => ee.Ubicacion.CD_AREA_ARMAZ,
                   tp => tp.CD_AREA_ARMAZ,
                   (ld, ee) => new { DetalleLpn = ld.DetalleLpn, Producto = ld.Producto, ee, TipoArea = ee })
                .Where(ldp => (ldp.TipoArea.ID_AREA_AVARIA == "S"
                   || ldp.TipoArea.ID_ESTOQUE_GERAL == "S"
                   || ldp.TipoArea.ID_AREA_PICKING == "S")
                   && (ldp.DetalleLpn.DT_FABRICACAO == null
                   || (ldp.Producto.TP_MANEJO_FECHA == ManejoFechaProductoDb.Fifo)
                   || (ldp.Producto.TP_MANEJO_FECHA == ManejoFechaProductoDb.Expirable && (DateTime.Today.AddDays((double)(ldp.Producto.QT_DIAS_VALIDADE == null ? 0 : ldp.Producto.QT_DIAS_VALIDADE)) <= ldp.DetalleLpn.DT_FABRICACAO))))
                .Select(ldp => ldp.DetalleLpn)
                .GroupJoin(disponibleAtributos,
                   ld => new { ld.VL_ATRIBUTOS },
                   da => new { da.VL_ATRIBUTOS },
                   (ld, da) => new { DetalleLpn = ld, DisponibleAtributos = da })
                .SelectMany(
                   ldda => ldda.DisponibleAtributos.DefaultIfEmpty(),
                   (lddas, da) => new { lddas.DetalleLpn, DisponibleAtributos = da })
                .GroupJoin(reservaAtributos,
                   ldda => new { ldda.DetalleLpn.VL_ATRIBUTOS },
                   ra => new { ra.VL_ATRIBUTOS },
                   (ldda, ra) => new { ldda.DetalleLpn, ldda.DisponibleAtributos, ReservaAtributos = ra })
                .SelectMany(
                   lddara => lddara.ReservaAtributos.DefaultIfEmpty(),
                   (lddaras, ra) => new { lddaras.DetalleLpn, lddaras.DisponibleAtributos, ReservaAtributos = ra })
                .Select(lddararlprap => new
                {
                    Lpn = lddararlprap.DetalleLpn.NU_LPN,
                    DetalleLpn = lddararlprap.DetalleLpn.ID_LPN_DET,
                    StockLpn = lddararlprap.DetalleLpn.QT_ESTOQUE,
                    ReservaLpn = (lddararlprap.DetalleLpn.QT_RESERVA_SAIDA == null ? 0 : lddararlprap.DetalleLpn.QT_RESERVA_SAIDA),
                    StockAtributos = lddararlprap.DisponibleAtributos.QT_DISPONIBLE == null ? 0 : lddararlprap.DisponibleAtributos.QT_DISPONIBLE,
                    ReservaAtributos = lddararlprap.ReservaAtributos.QT_RESERVA == null ? 0 : lddararlprap.ReservaAtributos.QT_RESERVA,
                    JsonAtributos = lddararlprap.DetalleLpn.VL_ATRIBUTOS,
                });

            var disponibles = stockLpn
                .Select(x => new
                {
                    x.Lpn,
                    x.DetalleLpn,
                    x.JsonAtributos,
                    DisponibleLpn = x.StockLpn - x.ReservaLpn,
                    DisponibleAtributos = x.StockAtributos - x.ReservaAtributos
                })
                .Select(x => new LpnDetalleDisponible
                {
                    Lpn = x.Lpn,
                    DetalleLpn = x.DetalleLpn,
                    JsonAtributos = x.JsonAtributos,
                    Disponible = (x.DisponibleLpn < x.DisponibleAtributos) ? x.DisponibleLpn : x.DisponibleAtributos,
                });

            return disponibles.ToList();
        }

        public virtual decimal GetCantidNoDisponibleEnLpn(long nuLpn, int cdEmpresa, string producto, string identificador, decimal faixa, int idLpnDet)
        {
            decimal disponible = 0;
            var detallesVencidosoAveriados = _context.T_LPN_DET
               .AsNoTracking()
               .Where(ld => ld.NU_LPN == nuLpn
                   && ld.ID_LPN_DET == idLpnDet
                   && ld.CD_PRODUTO == producto
                   && ld.CD_EMPRESA == cdEmpresa
                   && ld.NU_IDENTIFICADOR == identificador
                   && ld.CD_FAIXA == faixa
                   && ld.QT_ESTOQUE > 0
                   )
               .Join(_context.T_LPN,
               dlpn => dlpn.NU_LPN,
               lpn => lpn.NU_LPN,
               (dlpn, lpn) => new { DetalleLpn = dlpn, Lpn = lpn })
               .Join(_context.T_PRODUTO,
                   ld => new { ld.DetalleLpn.CD_EMPRESA, ld.DetalleLpn.CD_PRODUTO },
                   p => new { p.CD_EMPRESA, p.CD_PRODUTO },
                   (ld, p) => new { DetalleLpn = ld.DetalleLpn, Lpn = ld.Lpn, Producto = p })
               .Where(ldp => ldp.Lpn.FL_DISPONIBLE_LIBERACION == "N" || ldp.DetalleLpn.ID_AVERIA == "S" || ldp.DetalleLpn.ID_CTRL_CALIDAD == EstadoControlCalidad.Pendiente || ldp.DetalleLpn.ID_INVENTARIO == "D" ||
                (ldp.Producto.TP_MANEJO_FECHA == ManejoFechaProductoDb.Expirable
                 && (DateTime.Today.AddDays((double)(ldp.Producto.QT_DIAS_VALIDADE == null ? 0 : ldp.Producto.QT_DIAS_VALIDADE)) > ldp.DetalleLpn.DT_FABRICACAO)))
               .Select(ldp => ldp.DetalleLpn).ToList();
            if (detallesVencidosoAveriados != null && detallesVencidosoAveriados.Count > 0)
                disponible = detallesVencidosoAveriados.Sum(c => c.QT_ESTOQUE);
            return disponible;
        }

        public virtual decimal GetStockLpnUbicacion(string ubicacion, int empresa, string producto, string identificador, decimal faixa, out decimal cantidadReservaLpn, out decimal cantidadReservaAtributo)
        {
            decimal cantidadLpn = 0;
            cantidadReservaLpn = 0;
            cantidadReservaAtributo = 0;
            List<T_LPN_DET> listLpn = new List<T_LPN_DET>();

            listLpn = _context.T_LPN_DET
                .Include("T_LPN")
                .AsNoTracking()
                .Where(x => x.T_LPN.CD_ENDERECO == ubicacion
                    && x.CD_EMPRESA == empresa
                    && x.NU_IDENTIFICADOR == identificador
                    && x.CD_PRODUTO == producto
                    && x.CD_FAIXA == faixa
                    && x.QT_ESTOQUE > 0)
                .ToList();

            if (listLpn.Count > 0)
            {
                var reservasLpnAtributo = _context.T_DET_PICKING_LPN.Where(x => x.CD_ENDERECO == ubicacion
                                                && x.CD_EMPRESA == empresa
                                                && (x.NU_IDENTIFICADOR == identificador || x.NU_IDENTIFICADOR == ManejoIdentificadorDb.IdentificadorAuto)
                                                && x.CD_PRODUTO == producto
                                                && x.CD_FAIXA == faixa
                                                && x.QT_RESERVA > 0
                                                && x.VL_ATRIBUTOS != null).ToList();
                if (reservasLpnAtributo.Count > 0)
                    cantidadReservaAtributo = reservasLpnAtributo.Sum(x => x.QT_RESERVA);
                cantidadReservaLpn = listLpn.Sum(x => x.QT_RESERVA_SAIDA ?? 0);
                cantidadLpn = listLpn.Sum(x => x.QT_ESTOQUE);
            }

            return cantidadLpn;
        }

        public virtual decimal GetReservaDetalleLpn(LpnDetalle detalleLpn)
        {
            var disponibleAtributos = _context.V_STOCK_LPN
              .AsNoTracking()
              .Where(ld => ld.CD_ENDERECO == detalleLpn.Ubicacion
                  && ld.CD_PRODUTO == detalleLpn.CodigoProducto
                  && ld.CD_EMPRESA == detalleLpn.Empresa
                  && ld.NU_IDENTIFICADOR == detalleLpn.Lote
                  && ld.CD_FAIXA == detalleLpn.Faixa
                  && ld.QT_ESTOQUE > 0
                  && ld.ID_AVERIA == "N"
                  && ld.ID_CTRL_CALIDAD == "C"
                  && ld.ID_INVENTARIO == "R")
              .Join(_context.T_PRODUTO,
                  ld => new { ld.CD_EMPRESA, ld.CD_PRODUTO },
                  p => new { p.CD_EMPRESA, p.CD_PRODUTO },
                  (ld, p) => new { DetalleLpn = ld, Producto = p })
              .Where(ldp => ldp.DetalleLpn.DT_FABRICACAO == null
                 || (ldp.Producto.TP_MANEJO_FECHA == ManejoFechaProductoDb.Fifo)
                 || (ldp.Producto.TP_MANEJO_FECHA == ManejoFechaProductoDb.Expirable && (DateTime.Today.AddDays((double)(ldp.Producto.QT_DIAS_VALIDADE == null ? 0 : ldp.Producto.QT_DIAS_VALIDADE)) <= ldp.DetalleLpn.DT_FABRICACAO)))
              .Select(ldp => ldp.DetalleLpn)
              .GroupBy(dlrlp => new
              {
                  dlrlp.VL_ATRIBUTOS
              })
              .Select(g => new
              {
                  g.Key.VL_ATRIBUTOS,
                  QT_DISPONIBLE = g.Sum(x => x.QT_ESTOQUE - (x.QT_RESERVA_SAIDA == null ? 0 : x.QT_RESERVA_SAIDA))
              });

            var reservaAtributos = _context.T_DET_PICKING_LPN
                .Where(dpl => dpl.CD_ENDERECO == detalleLpn.Ubicacion
                    && dpl.CD_EMPRESA == detalleLpn.Empresa
                    && dpl.CD_PRODUTO == detalleLpn.CodigoProducto
                    && dpl.CD_FAIXA == detalleLpn.Faixa
                    && (dpl.NU_IDENTIFICADOR == detalleLpn.Lote || dpl.NU_IDENTIFICADOR == ManejoIdentificadorDb.IdentificadorAuto)
                    && dpl.QT_RESERVA > 0
                    && dpl.VL_ATRIBUTOS != null)
                .GroupBy(dpl => new
                {
                    dpl.VL_ATRIBUTOS
                })
                .Select(g => new
                {
                    g.Key.VL_ATRIBUTOS,
                    QT_RESERVA = (decimal?)g.Sum(dpl => dpl.QT_RESERVA)
                });

            var stockLpn = _context.V_STOCK_LPN
                .AsNoTracking()
                .Where(ld => ld.NU_LPN == detalleLpn.NumeroLPN
                   && ld.CD_ENDERECO == detalleLpn.Ubicacion
                   && ld.CD_PRODUTO == detalleLpn.CodigoProducto
                   && ld.CD_EMPRESA == detalleLpn.Empresa
                   && ld.NU_IDENTIFICADOR == detalleLpn.Lote
                   && ld.CD_FAIXA == detalleLpn.Faixa
                   && ld.ID_LPN_DET == detalleLpn.Id)
                .GroupJoin(disponibleAtributos,
                   ld => new { ld.VL_ATRIBUTOS },
                   da => new { da.VL_ATRIBUTOS },
                   (ld, da) => new { DetalleLpn = ld, DisponibleAtributos = da })
                .SelectMany(
                   ldda => ldda.DisponibleAtributos.DefaultIfEmpty(),
                   (lddas, da) => new { lddas.DetalleLpn, DisponibleAtributos = da })
                .GroupJoin(reservaAtributos,
                   ldda => new { ldda.DetalleLpn.VL_ATRIBUTOS },
                   ra => new { ra.VL_ATRIBUTOS },
                   (ldda, ra) => new { ldda.DetalleLpn, ldda.DisponibleAtributos, ReservaAtributos = ra })
                .SelectMany(
                   lddara => lddara.ReservaAtributos.DefaultIfEmpty(),
                   (lddaras, ra) => new { lddaras.DetalleLpn, lddaras.DisponibleAtributos, ReservaAtributos = ra })
                .Select(lddararlprap => new
                {
                    Lpn = lddararlprap.DetalleLpn.NU_LPN,
                    DetalleLpn = lddararlprap.DetalleLpn.ID_LPN_DET,
                    StockLpn = lddararlprap.DetalleLpn.QT_ESTOQUE,
                    ReservaLpn = (lddararlprap.DetalleLpn.QT_RESERVA_SAIDA == null ? 0 : lddararlprap.DetalleLpn.QT_RESERVA_SAIDA),
                    StockAtributos = lddararlprap.DisponibleAtributos.QT_DISPONIBLE == null ? 0 : lddararlprap.DisponibleAtributos.QT_DISPONIBLE,
                    ReservaAtributos = lddararlprap.ReservaAtributos.QT_RESERVA == null ? 0 : lddararlprap.ReservaAtributos.QT_RESERVA,
                    JsonAtributos = lddararlprap.DetalleLpn.VL_ATRIBUTOS,
                })
                .FirstOrDefault();

            return (stockLpn?.ReservaLpn ?? 0) + (stockLpn?.ReservaAtributos ?? 0);
        }

        public virtual Lpn GetLpn(string idExterno, string tipo, int empresa)
        {
            var entity = _context.T_LPN.Where(l => l.ID_LPN_EXTERNO == idExterno && l.TP_LPN_TIPO == tipo && l.CD_EMPRESA == empresa)
                .OrderByDescending(l => l.NU_LPN).
                FirstOrDefault();

            return _lpnMapper.MapToObject(entity);
        }

        #endregion

        #region Add

        public virtual void AddTipoLpn(LpnTipo tipoLpn)
        {
            T_LPN_TIPO entity = this._lpnMapper.MapToEntity(tipoLpn);

            this._context.T_LPN_TIPO.Add(entity);
        }

        public virtual void AddLPNBarras(LpnBarras lpnBarra)
        {
            T_LPN_BARRAS entity = this._lpnMapper.MapToEntity(lpnBarra);

            this._context.T_LPN_BARRAS.Add(entity);
        }

        public virtual void AddLPN(Lpn lpn)
        {
            if (lpn.NumeroLPN == 0)
                lpn.NumeroLPN = this._context.GetNextSequenceValueLong(_dapper, Secuencias.S_LPN);

            T_LPN entity = this._lpnMapper.MapToEntity(lpn);

            this._context.T_LPN.Add(entity);
        }

        public virtual void AddDetalleLpn(LpnDetalle detalle)
        {
            T_LPN_DET entity = this._lpnMapper.MapToEntity(detalle);

            this._context.T_LPN_DET.Add(entity);
        }

        public virtual void AddLpnTipoAtributo(LpnTipoAtributo atributoLpn)
        {
            T_LPN_TIPO_ATRIBUTO entity = this._lpnMapper.MapToEntity(atributoLpn);

            this._context.T_LPN_TIPO_ATRIBUTO.Add(entity);
        }

        public virtual void AddAtributoAsociado(LpnAtributo atributoAsociado)
        {
            T_LPN_ATRIBUTO entity = this._lpnMapper.MapToEntity(atributoAsociado);

            this._context.T_LPN_ATRIBUTO.Add(entity);
        }

        public virtual void AddAtributoDetalle(LpnDetalleAtributo atributo)
        {
            T_LPN_DET_ATRIBUTO entity = _lpnMapper.MapToEntity(atributo);

            this._context.T_LPN_DET_ATRIBUTO.Add(entity);
        }

        public virtual void AddLpnTipoAtributoDet(LpnTipoAtributoDet atributoLpn)
        {
            T_LPN_TIPO_ATRIBUTO_DET entity = this._lpnMapper.MapToEntity(atributoLpn);

            this._context.T_LPN_TIPO_ATRIBUTO_DET.Add(entity);
        }

        public virtual void AddAtributoTemporal(DetallePedidoAtributoTemporal atributo)
        {
            T_TEMP_DET_PEDIDO_SAIDA_ATRIB entity = this._lpnMapper.MapToEntity(atributo);
            this._context.T_TEMP_DET_PEDIDO_SAIDA_ATRIB.Add(entity);
        }

        public virtual void AddAtributoLpnTemporal(DetallePedidoAtributoLpnTemporal atributo)
        {
            T_TEMP_DET_PEDIDO_SAIDA_LPN_ATRIB entity = this._lpnMapper.MapToEntity(atributo);
            this._context.T_TEMP_DET_PEDIDO_SAIDA_LPN_ATRIB.Add(entity);
        }

        public virtual void AddDetallePedidoLpnAtributo(DetallePedidoLpnAtributo detallePedidoLpnAtributo)
        {
            T_DET_PEDIDO_SAIDA_LPN_ATRIB entity = this._lpnMapper.MapToEntity(detallePedidoLpnAtributo);
            this._context.T_DET_PEDIDO_SAIDA_LPN_ATRIB.Add(entity);
        }

        public virtual void AddDetallePedidoAtributo(DetallePedidoAtributo detallePedidoAtributo)
        {
            T_DET_PEDIDO_SAIDA_ATRIB entity = this._lpnMapper.MapToEntity(detallePedidoAtributo);
            this._context.T_DET_PEDIDO_SAIDA_ATRIB.Add(entity);
        }

        public virtual void AddDetallePedidoAtributoDefinicion(DetallePedidoAtributoDefinicion atributo)
        {
            T_DET_PEDIDO_SAIDA_ATRIB_DET entity = this._lpnMapper.MapToEntity(atributo);
            this._context.T_DET_PEDIDO_SAIDA_ATRIB_DET.Add(entity);
        }

        public virtual void AddDetallePreparacionLpn(DetallePreparacionLpn detPickingLpn)
        {
            var entity = this._lpnMapper.MapToEntity(detPickingLpn);
            this._context.T_DET_PICKING_LPN.Add(entity);
        }

        #endregion

        #region Update

        public virtual void UpdateLpn(Lpn lpn)
        {
            T_LPN entity = this._lpnMapper.MapToEntity(lpn);
            T_LPN attachedEntity = _context.T_LPN.Local
                .FirstOrDefault(x => x.NU_LPN == entity.NU_LPN);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.State = EntityState.Modified;
                attachedEntry.CurrentValues.SetValues(entity);
            }
            else
            {
                _context.T_LPN.Attach(entity);
                _context.Entry<T_LPN>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateTipoLpn(LpnTipo tipoLpn)
        {
            T_LPN_TIPO entity = this._lpnMapper.MapToEntity(tipoLpn);
            T_LPN_TIPO attachedEntity = _context.T_LPN_TIPO.Local
                .FirstOrDefault(x => x.TP_LPN_TIPO == entity.TP_LPN_TIPO);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.State = EntityState.Modified;
                attachedEntry.CurrentValues.SetValues(entity);
            }
            else
            {
                _context.T_LPN_TIPO.Attach(entity);
                _context.Entry<T_LPN_TIPO>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateLpnAtributo(LpnAtributo tipoLpn)
        {
            T_LPN_ATRIBUTO entity = this._lpnMapper.MapToEntity(tipoLpn);
            T_LPN_ATRIBUTO attachedEntity = _context.T_LPN_ATRIBUTO.Local
                .FirstOrDefault(x => x.NU_LPN == tipoLpn.NumeroLpn
                    && x.ID_ATRIBUTO == tipoLpn.Id
                    && x.TP_LPN_TIPO == tipoLpn.Tipo); ;

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.State = EntityState.Modified;
                attachedEntry.CurrentValues.SetValues(entity);
            }
            else
            {
                _context.T_LPN_ATRIBUTO.Attach(entity);
                _context.Entry<T_LPN_ATRIBUTO>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateLpnTipoAtributo(LpnTipoAtributo tarea)
        {
            T_LPN_TIPO_ATRIBUTO entity = this._lpnMapper.MapToEntity(tarea);
            T_LPN_TIPO_ATRIBUTO attachedEntity = _context.T_LPN_TIPO_ATRIBUTO.Local
                .FirstOrDefault(w => w.ID_ATRIBUTO == tarea.IdAtributo && w.TP_LPN_TIPO == tarea.TipoLpn);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_LPN_TIPO_ATRIBUTO.Attach(entity);
                _context.Entry<T_LPN_TIPO_ATRIBUTO>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateLpnDetalleAtributo(LpnDetalleAtributo tipoLpn)
        {
            T_LPN_DET_ATRIBUTO entity = this._lpnMapper.MapToEntity(tipoLpn);
            T_LPN_DET_ATRIBUTO attachedEntity = _context.T_LPN_DET_ATRIBUTO.Local
                .FirstOrDefault(x => x.NU_LPN == tipoLpn.NumeroLpn
                    && x.ID_LPN_DET == tipoLpn.IdLpnDetalle
                    && x.ID_ATRIBUTO == tipoLpn.IdAtributo
                    && x.TP_LPN_TIPO == tipoLpn.Tipo
                    && x.CD_PRODUTO == tipoLpn.Producto
                    && x.CD_EMPRESA == tipoLpn.Empresa
                    && x.NU_IDENTIFICADOR == tipoLpn.Lote);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.State = EntityState.Modified;
                attachedEntry.CurrentValues.SetValues(entity);
            }
            else
            {
                _context.T_LPN_DET_ATRIBUTO.Attach(entity);
                _context.Entry<T_LPN_DET_ATRIBUTO>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateLpnTipoAtributoDetalle(LpnTipoAtributoDet tarea)
        {
            T_LPN_TIPO_ATRIBUTO_DET entity = this._lpnMapper.MapToEntity(tarea);
            T_LPN_TIPO_ATRIBUTO_DET attachedEntity = _context.T_LPN_TIPO_ATRIBUTO_DET.Local
                .FirstOrDefault(w => w.ID_ATRIBUTO == tarea.IdAtributo && w.TP_LPN_TIPO == tarea.TipoLpn);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_LPN_TIPO_ATRIBUTO_DET.Attach(entity);
                _context.Entry<T_LPN_TIPO_ATRIBUTO_DET>(entity).State = EntityState.Modified;
            }
        }

        public virtual void ReordenarAtributosLpn(string tpLpn)
        {
            short orden = 1;
            List<LpnTipoAtributo> listAtributoTipo = new List<LpnTipoAtributo>();
            List<T_LPN_TIPO_ATRIBUTO> entities = this._context.T_LPN_TIPO_ATRIBUTO.AsNoTracking().Where(x => x.TP_LPN_TIPO == tpLpn).OrderBy(x => x.NU_ORDEN).ToList();
            foreach (var entity in entities)
            {
                listAtributoTipo.Add(this._lpnMapper.MapToObject(entity));
            }
            foreach (var entity in listAtributoTipo)
            {
                entity.Orden = orden;
                UpdateLpnTipoAtributo(entity);
                orden += 1;
            }
        }

        public virtual void CambiarOrdenLineaAtributoLpn(LpnTipoAtributo lpnTipoAtributo, bool ascender = true)
        {
            List<LpnTipoAtributo> listAtributoTipo = new List<LpnTipoAtributo>();
            List<T_LPN_TIPO_ATRIBUTO> entities = this._context.T_LPN_TIPO_ATRIBUTO.AsNoTracking().Where(x => x.TP_LPN_TIPO == lpnTipoAtributo.TipoLpn).OrderByDescending(x => x.NU_ORDEN).ToList();

            foreach (var entity in entities)
            {
                listAtributoTipo.Add(this._lpnMapper.MapToObject(entity));
            }

            if (lpnTipoAtributo != null)
            {
                if (listAtributoTipo.Count == 2)
                {
                    if (ascender) //esta en la pos 2
                    {
                        lpnTipoAtributo.Orden = 1;
                        this.UpdateLpnTipoAtributo(lpnTipoAtributo);
                        LpnTipoAtributo lpnTipoAtributoAuxiliar = listAtributoTipo.LastOrDefault();
                        lpnTipoAtributoAuxiliar.Orden = 2;
                        this.UpdateLpnTipoAtributo(lpnTipoAtributoAuxiliar);
                    }
                    else
                    {
                        lpnTipoAtributo.Orden = 2;
                        this.UpdateLpnTipoAtributo(lpnTipoAtributo);
                        LpnTipoAtributo lpnTipoAtributoAuxiliar = listAtributoTipo.FirstOrDefault();
                        lpnTipoAtributoAuxiliar.Orden = 1;
                        this.UpdateLpnTipoAtributo(lpnTipoAtributoAuxiliar);
                    }
                }
                else if (listAtributoTipo.Count > 2)
                {
                    if (listAtributoTipo.Count == lpnTipoAtributo.Orden) //ultimo elemento en la lista
                    {
                        LpnTipoAtributo lpnTipoAtributoAuxiliar = listAtributoTipo.Skip(1).ToList().FirstOrDefault();
                        lpnTipoAtributoAuxiliar.Orden++;
                        this.UpdateLpnTipoAtributo(lpnTipoAtributoAuxiliar);
                        lpnTipoAtributo.Orden--;
                        this.UpdateLpnTipoAtributo(lpnTipoAtributo);
                    }
                    else if (lpnTipoAtributo.Orden == 1)
                    {
                        LpnTipoAtributo lpnTipoAtributoAuxiliar = listAtributoTipo.OrderBy(x => x.Orden).Skip(1).ToList().FirstOrDefault();
                        lpnTipoAtributoAuxiliar.Orden--;
                        this.UpdateLpnTipoAtributo(lpnTipoAtributoAuxiliar);
                        lpnTipoAtributo.Orden++;
                        this.UpdateLpnTipoAtributo(lpnTipoAtributo);
                    }
                    else //es un elemento intermedio
                    {
                        if (ascender)
                        {
                            LpnTipoAtributo lpnTipoAtributoAuxiliar = listAtributoTipo.FirstOrDefault(x => x.Orden == lpnTipoAtributo.Orden - 1);
                            lpnTipoAtributoAuxiliar.Orden++;
                            this.UpdateLpnTipoAtributo(lpnTipoAtributoAuxiliar);
                            lpnTipoAtributo.Orden--;
                            this.UpdateLpnTipoAtributo(lpnTipoAtributo);

                        }
                        else
                        {
                            LpnTipoAtributo lpnTipoAtributoAuxiliar = listAtributoTipo.FirstOrDefault(x => x.Orden == lpnTipoAtributo.Orden + 1);
                            lpnTipoAtributoAuxiliar.Orden--;
                            this.UpdateLpnTipoAtributo(lpnTipoAtributoAuxiliar);
                            lpnTipoAtributo.Orden++;
                            this.UpdateLpnTipoAtributo(lpnTipoAtributo);
                        }
                    }
                }
            }
        }

        public virtual void ReordenarAtributosLpnDetalle(string TpLpnTipo)
        {
            short orden = 1;
            List<LpnTipoAtributoDet> listAtributoTipo = new List<LpnTipoAtributoDet>();
            List<T_LPN_TIPO_ATRIBUTO_DET> entities = this._context.T_LPN_TIPO_ATRIBUTO_DET.AsNoTracking().Where(x => x.TP_LPN_TIPO == TpLpnTipo).OrderBy(x => x.NU_ORDEN).ToList();
            foreach (var entity in entities)
            {
                listAtributoTipo.Add(this._lpnMapper.MapToObject(entity));
            }
            foreach (var entity in listAtributoTipo)
            {
                entity.Orden = orden;
                UpdateLpnTipoAtributoDetalle(entity);
                orden += 1;
            }
        }

        public virtual void CambiarOrdenLineaLpnAtributoDetalle(LpnTipoAtributoDet tipoAtributoDet, bool ascender = true)
        {
            List<LpnTipoAtributoDet> listAtributoTipo = new List<LpnTipoAtributoDet>();
            List<T_LPN_TIPO_ATRIBUTO_DET> entities = this._context.T_LPN_TIPO_ATRIBUTO_DET.AsNoTracking().Where(x => x.TP_LPN_TIPO == tipoAtributoDet.TipoLpn).OrderByDescending(x => x.NU_ORDEN).ToList();

            foreach (var entity in entities)
            {
                listAtributoTipo.Add(this._lpnMapper.MapToObject(entity));
            }

            if (tipoAtributoDet != null)
            {
                if (listAtributoTipo.Count == 2)
                {
                    if (ascender) //esta en la pos 2
                    {
                        tipoAtributoDet.Orden = 1;
                        this.UpdateLpnTipoAtributoDetalle(tipoAtributoDet);
                        LpnTipoAtributoDet TipoAtributoDetAuxiliar = listAtributoTipo.LastOrDefault();
                        TipoAtributoDetAuxiliar.Orden = 2;
                        this.UpdateLpnTipoAtributoDetalle(TipoAtributoDetAuxiliar);
                    }
                    else
                    {
                        tipoAtributoDet.Orden = 2;
                        this.UpdateLpnTipoAtributoDetalle(tipoAtributoDet);
                        LpnTipoAtributoDet TipoAtributoDetAuxiliar = listAtributoTipo.FirstOrDefault();
                        TipoAtributoDetAuxiliar.Orden = 1;
                        this.UpdateLpnTipoAtributoDetalle(TipoAtributoDetAuxiliar);
                    }
                }
                else if (listAtributoTipo.Count > 2)
                {
                    if (listAtributoTipo.Count == tipoAtributoDet.Orden) //ultimo elemento en la lista
                    {
                        LpnTipoAtributoDet TareaAuxiliar = listAtributoTipo.Skip(1).ToList().FirstOrDefault();
                        TareaAuxiliar.Orden++;
                        this.UpdateLpnTipoAtributoDetalle(TareaAuxiliar);
                        tipoAtributoDet.Orden--;
                        this.UpdateLpnTipoAtributoDetalle(tipoAtributoDet);
                    }
                    else if (tipoAtributoDet.Orden == 1)
                    {
                        LpnTipoAtributoDet TipoAtributoDetAuxiliar = listAtributoTipo.OrderBy(x => x.Orden).Skip(1).ToList().FirstOrDefault();
                        TipoAtributoDetAuxiliar.Orden--;
                        this.UpdateLpnTipoAtributoDetalle(TipoAtributoDetAuxiliar);
                        tipoAtributoDet.Orden++;
                        this.UpdateLpnTipoAtributoDetalle(tipoAtributoDet);
                    }
                    else //es un elemento intermedio
                    {
                        if (ascender)
                        {
                            LpnTipoAtributoDet TipoAtributoDetAuxiliar = listAtributoTipo.FirstOrDefault(x => x.Orden == tipoAtributoDet.Orden - 1);
                            TipoAtributoDetAuxiliar.Orden++;
                            this.UpdateLpnTipoAtributoDetalle(TipoAtributoDetAuxiliar);
                            tipoAtributoDet.Orden--;
                            this.UpdateLpnTipoAtributoDetalle(tipoAtributoDet);
                        }
                        else
                        {
                            LpnTipoAtributoDet TipoAtributoDetAuxiliar = listAtributoTipo.FirstOrDefault(x => x.Orden == tipoAtributoDet.Orden + 1);
                            TipoAtributoDetAuxiliar.Orden--;
                            this.UpdateLpnTipoAtributoDetalle(TipoAtributoDetAuxiliar);
                            tipoAtributoDet.Orden++;
                            this.UpdateLpnTipoAtributoDetalle(tipoAtributoDet);

                        }
                    }
                }
            }
        }

        public virtual void UpdateAuditoriaLpn(AuditoriaLpn auditoria)
        {
            T_LPN_AUDITORIA entity = this._lpnMapper.MapToEntity(auditoria);
            T_LPN_AUDITORIA attachedEntity = _context.T_LPN_AUDITORIA.Local
                .FirstOrDefault(w => w.NU_AUDITORIA == auditoria.Auditoria);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_LPN_AUDITORIA.Attach(entity);
                _context.Entry<T_LPN_AUDITORIA>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateDetalleLpn(LpnDetalle detalleLpn)
        {
            var entity = _lpnMapper.MapToEntity(detalleLpn);
            T_LPN_DET attachedEntity = _context.T_LPN_DET.Local
                .FirstOrDefault(x => x.ID_LPN_DET == detalleLpn.Id
                    && x.NU_LPN == detalleLpn.NumeroLPN
                    && x.CD_PRODUTO == detalleLpn.CodigoProducto
                    && x.CD_FAIXA == detalleLpn.Faixa
                    && x.CD_EMPRESA == detalleLpn.Empresa
                    && x.NU_IDENTIFICADOR == detalleLpn.Lote);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_LPN_DET.Attach(entity);
                _context.Entry<T_LPN_DET>(entity).State = EntityState.Modified;
            }
        }

        public virtual void ExpedirLpn(long nuLpn, string ubicacionDestino, long nuTransaccion)
        {
            _context.T_LPN
                .Where(d => d.NU_LPN == nuLpn)
                .ExecuteUpdate(setters => setters
                    .SetProperty(d => d.CD_ENDERECO, ubicacionDestino)
                    .SetProperty(d => d.NU_TRANSACCION, nuTransaccion)
                    .SetProperty(d => d.ID_ESTADO, EstadosLPN.Finalizado)
                    .SetProperty(d => d.DT_FIN, DateTime.Now)
                    .SetProperty(d => d.DT_UPDROW, DateTime.Now));

            _context.T_LPN_DET
                .Where(d => d.NU_LPN == nuLpn)
                .ExecuteUpdate(setters => setters
                    .SetProperty(d => d.QT_EXPEDIDA, d => d.QT_ESTOQUE)
                    .SetProperty(d => d.QT_ESTOQUE, 0)
                    .SetProperty(d => d.NU_TRANSACCION, nuTransaccion));
        }

        public virtual void UpdateDetallePedidoAtributoDefinicion(DetallePedidoAtributoDefinicion atributo)
        {
            var entity = this._lpnMapper.MapToEntity(atributo);

            var attachedEntity = _context.T_DET_PEDIDO_SAIDA_ATRIB_DET.Local
                .FirstOrDefault(w => w.NU_DET_PED_SAI_ATRIB == entity.NU_DET_PED_SAI_ATRIB
                    && w.ID_ATRIBUTO == entity.ID_ATRIBUTO
                    && w.FL_CABEZAL == entity.FL_CABEZAL);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_DET_PEDIDO_SAIDA_ATRIB_DET.Attach(entity);
                _context.Entry<T_DET_PEDIDO_SAIDA_ATRIB_DET>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateDetallePedidoAtributoLpnTemporal(DetallePedidoAtributoLpnTemporal temp)
        {
            var entity = this._lpnMapper.MapToEntity(temp);

            var attachedEntity = _context.T_TEMP_DET_PEDIDO_SAIDA_LPN_ATRIB.Local
                .FirstOrDefault(a => a.NU_PEDIDO == entity.NU_PEDIDO
                    && a.CD_CLIENTE == entity.CD_CLIENTE
                    && a.CD_EMPRESA == entity.CD_EMPRESA
                    && a.CD_PRODUTO == entity.CD_PRODUTO
                    && a.CD_FAIXA == entity.CD_FAIXA
                    && a.NU_IDENTIFICADOR == entity.NU_IDENTIFICADOR
                    && a.ID_ESPECIFICA_IDENTIFICADOR == entity.ID_ESPECIFICA_IDENTIFICADOR
                    && a.TP_LPN_TIPO == entity.TP_LPN_TIPO
                    && a.ID_LPN_EXTERNO == entity.ID_LPN_EXTERNO
                    && a.ID_ATRIBUTO == entity.ID_ATRIBUTO
                    && a.USERID == entity.USERID
                    && a.FL_CABEZAL == entity.FL_CABEZAL);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_TEMP_DET_PEDIDO_SAIDA_LPN_ATRIB.Attach(entity);
                _context.Entry<T_TEMP_DET_PEDIDO_SAIDA_LPN_ATRIB>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateDetallePedidoLpnAtributo(DetallePedidoLpnAtributo detLpnAtributo)
        {
            var entity = this._lpnMapper.MapToEntity(detLpnAtributo);

            var attachedEntity = _context.T_DET_PEDIDO_SAIDA_LPN_ATRIB.Local
                .FirstOrDefault(d =>
                       d.NU_PEDIDO == entity.NU_PEDIDO
                    && d.CD_CLIENTE == entity.CD_CLIENTE
                    && d.CD_EMPRESA == entity.CD_EMPRESA
                    && d.CD_PRODUTO == entity.CD_PRODUTO
                    && d.CD_FAIXA == entity.CD_FAIXA
                    && d.NU_IDENTIFICADOR == entity.NU_IDENTIFICADOR
                    && d.ID_ESPECIFICA_IDENTIFICADOR == entity.ID_ESPECIFICA_IDENTIFICADOR
                    && d.TP_LPN_TIPO == entity.TP_LPN_TIPO
                    && d.ID_LPN_EXTERNO == entity.ID_LPN_EXTERNO
                    && d.NU_DET_PED_SAI_ATRIB == entity.NU_DET_PED_SAI_ATRIB);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_DET_PEDIDO_SAIDA_LPN_ATRIB.Attach(entity);
                _context.Entry<T_DET_PEDIDO_SAIDA_LPN_ATRIB>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateDetallePedidoLpn(DetallePedidoLpn detLpnAtributo)
        {
            var entity = this._lpnMapper.MapToEntity(detLpnAtributo);

            var attachedEntity = _context.T_DET_PEDIDO_SAIDA_LPN.Local
                .FirstOrDefault(d =>
                       d.NU_PEDIDO == entity.NU_PEDIDO
                    && d.CD_CLIENTE == entity.CD_CLIENTE
                    && d.CD_EMPRESA == entity.CD_EMPRESA
                    && d.CD_PRODUTO == entity.CD_PRODUTO
                    && d.CD_FAIXA == entity.CD_FAIXA
                    && d.NU_IDENTIFICADOR == entity.NU_IDENTIFICADOR
                    && d.ID_ESPECIFICA_IDENTIFICADOR == entity.ID_ESPECIFICA_IDENTIFICADOR
                    && d.TP_LPN_TIPO == entity.TP_LPN_TIPO
                    && d.ID_LPN_EXTERNO == entity.ID_LPN_EXTERNO);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_DET_PEDIDO_SAIDA_LPN.Attach(entity);
                _context.Entry<T_DET_PEDIDO_SAIDA_LPN>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateDetallePedidoAtributoTemporal(DetallePedidoAtributoTemporal temp)
        {
            var entity = this._lpnMapper.MapToEntity(temp);

            var attachedEntity = _context.T_TEMP_DET_PEDIDO_SAIDA_ATRIB.Local
                .FirstOrDefault(a => a.NU_PEDIDO == entity.NU_PEDIDO
                    && a.CD_CLIENTE == entity.CD_CLIENTE
                    && a.CD_EMPRESA == entity.CD_EMPRESA
                    && a.CD_PRODUTO == entity.CD_PRODUTO
                    && a.CD_FAIXA == entity.CD_FAIXA
                    && a.NU_IDENTIFICADOR == entity.NU_IDENTIFICADOR
                    && a.ID_ESPECIFICA_IDENTIFICADOR == entity.ID_ESPECIFICA_IDENTIFICADOR
                    && a.ID_ATRIBUTO == entity.ID_ATRIBUTO
                    && a.USERID == entity.USERID
                    && a.FL_CABEZAL == entity.FL_CABEZAL);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_TEMP_DET_PEDIDO_SAIDA_ATRIB.Attach(entity);
                _context.Entry<T_TEMP_DET_PEDIDO_SAIDA_ATRIB>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateDetallePedidoAtributo(DetallePedidoAtributo detAtributo)
        {
            var entity = this._lpnMapper.MapToEntity(detAtributo);

            var attachedEntity = _context.T_DET_PEDIDO_SAIDA_ATRIB.Local
                .FirstOrDefault(d =>
                       d.NU_PEDIDO == entity.NU_PEDIDO
                    && d.CD_CLIENTE == entity.CD_CLIENTE
                    && d.CD_EMPRESA == entity.CD_EMPRESA
                    && d.CD_PRODUTO == entity.CD_PRODUTO
                    && d.CD_FAIXA == entity.CD_FAIXA
                    && d.NU_IDENTIFICADOR == entity.NU_IDENTIFICADOR
                    && d.ID_ESPECIFICA_IDENTIFICADOR == entity.ID_ESPECIFICA_IDENTIFICADOR
                    && d.NU_DET_PED_SAI_ATRIB == entity.NU_DET_PED_SAI_ATRIB);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_DET_PEDIDO_SAIDA_ATRIB.Attach(entity);
                _context.Entry<T_DET_PEDIDO_SAIDA_ATRIB>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateDetallePreparacionLpn(DetallePreparacionLpn detPickingLpn)
        {
            var entity = this._lpnMapper.MapToEntity(detPickingLpn);

            var attachedEntity = _context.T_DET_PICKING_LPN.Local
                .FirstOrDefault(d =>
                    d.NU_PREPARACION == entity.NU_PREPARACION &&
                    d.ID_DET_PICKING_LPN == entity.ID_DET_PICKING_LPN);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_DET_PICKING_LPN.Attach(entity);
                _context.Entry<T_DET_PICKING_LPN>(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove

        public virtual void DeleteTipoLpn(LpnTipo value)
        {
            var entity = this._lpnMapper.MapToEntity(value);
            var attachedEntity = this._context.T_LPN_TIPO.Local
                .FirstOrDefault(d => d.TP_LPN_TIPO == value.Tipo);

            if (attachedEntity != null)
            {
                this._context.T_LPN_TIPO.Remove(attachedEntity);
            }
            else
            {
                this._context.T_LPN_TIPO.Attach(entity);
                this._context.T_LPN_TIPO.Remove(entity);
            }
        }

        public virtual void DeleteLpnTipoAtributo(LpnTipoAtributo lpnAtributoTipo)
        {
            var entity = this._lpnMapper.MapToEntity(lpnAtributoTipo);
            var attachedEntity = _context.T_LPN_TIPO_ATRIBUTO.Local
                .FirstOrDefault(w => w.ID_ATRIBUTO == lpnAtributoTipo.IdAtributo
                    && w.TP_LPN_TIPO == lpnAtributoTipo.TipoLpn);

            if (attachedEntity != null)
            {
                this._context.T_LPN_TIPO_ATRIBUTO.Remove(attachedEntity);
            }
            else
            {
                this._context.T_LPN_TIPO_ATRIBUTO.Attach(entity);
                this._context.T_LPN_TIPO_ATRIBUTO.Remove(entity);
            }
        }

        public virtual void DeleteLpnTipoAtributoDet(LpnTipoAtributoDet lpnAtributoTipoDet)
        {
            var entity = this._lpnMapper.MapToEntity(lpnAtributoTipoDet);
            var attachedEntity = _context.T_LPN_TIPO_ATRIBUTO_DET.Local
                .FirstOrDefault(w => w.ID_ATRIBUTO == lpnAtributoTipoDet.IdAtributo
                    && w.TP_LPN_TIPO == lpnAtributoTipoDet.TipoLpn);

            if (attachedEntity != null)
            {
                this._context.T_LPN_TIPO_ATRIBUTO_DET.RemoveRange(attachedEntity);
            }
            else
            {
                this._context.T_LPN_TIPO_ATRIBUTO_DET.Attach(entity);
                this._context.T_LPN_TIPO_ATRIBUTO_DET.Remove(entity);
            }
        }

        public virtual void RemoveAtributoDetalle(LpnDetalleAtributo lpnDetAtributo)
        {
            var entity = this._lpnMapper.MapToEntity(lpnDetAtributo);
            var attachedEntity = _context.T_LPN_DET_ATRIBUTO.Local
              .FirstOrDefault(x => x.NU_LPN == lpnDetAtributo.NumeroLpn
                     && x.ID_ATRIBUTO == lpnDetAtributo.IdAtributo
                     && x.ID_LPN_DET == lpnDetAtributo.IdLpnDetalle
                     && x.CD_EMPRESA == lpnDetAtributo.Empresa
                     && x.CD_PRODUTO == lpnDetAtributo.Producto
                     && x.NU_IDENTIFICADOR == lpnDetAtributo.Lote
                     && x.CD_FAIXA == lpnDetAtributo.Faixa);

            if (attachedEntity != null)
            {
                _context.T_LPN_DET_ATRIBUTO.Remove(attachedEntity);
            }
            else
            {
                _context.T_LPN_DET_ATRIBUTO.Attach(entity);
                _context.T_LPN_DET_ATRIBUTO.Remove(entity);
            }
        }

        public virtual void RemoveDetalleLpn(LpnDetalle detalleLpn)
        {
            var entity = this._lpnMapper.MapToEntity(detalleLpn);

            var attachedEntity = _context.T_LPN_DET.Local
                .FirstOrDefault(x => x.NU_LPN == detalleLpn.NumeroLPN
                       && x.ID_LPN_DET == detalleLpn.Id
                       && x.CD_EMPRESA == detalleLpn.Empresa
                       && x.CD_PRODUTO == detalleLpn.CodigoProducto
                       && x.NU_IDENTIFICADOR == detalleLpn.Lote
                       && x.CD_FAIXA == detalleLpn.Faixa);

            if (attachedEntity != null)
            {
                _context.T_LPN_DET.Remove(attachedEntity);
            }
            else
            {
                _context.T_LPN_DET.Attach(entity);
                _context.T_LPN_DET.Remove(entity);
            }
        }

        public virtual void DeleteDetallePedidoAtributoTemporal(DetallePedidoAtributoTemporal atributo)
        {
            var entity = this._lpnMapper.MapToEntity(atributo);

            var attachedEntity = _context.T_TEMP_DET_PEDIDO_SAIDA_ATRIB.Local
                .FirstOrDefault(w => w.NU_PEDIDO == entity.NU_PEDIDO
                    && w.CD_CLIENTE == entity.CD_CLIENTE
                    && w.CD_EMPRESA == entity.CD_EMPRESA
                    && w.CD_PRODUTO == entity.CD_PRODUTO
                    && w.CD_FAIXA == entity.CD_FAIXA
                    && w.NU_IDENTIFICADOR == entity.NU_IDENTIFICADOR
                    && w.ID_ESPECIFICA_IDENTIFICADOR == entity.ID_ESPECIFICA_IDENTIFICADOR
                    && w.ID_ATRIBUTO == entity.ID_ATRIBUTO
                    && w.USERID == entity.USERID
                    && w.FL_CABEZAL == entity.FL_CABEZAL);

            if (attachedEntity != null)
            {
                this._context.T_TEMP_DET_PEDIDO_SAIDA_ATRIB.Remove(attachedEntity);
            }
            else
            {
                this._context.T_TEMP_DET_PEDIDO_SAIDA_ATRIB.Attach(entity);
                this._context.T_TEMP_DET_PEDIDO_SAIDA_ATRIB.Remove(entity);
            }
        }

        public virtual void DeleteDetallePedidoAtributoLpnTemporal(DetallePedidoAtributoLpnTemporal temp)
        {
            var entity = this._lpnMapper.MapToEntity(temp);

            var attachedEntity = _context.T_TEMP_DET_PEDIDO_SAIDA_LPN_ATRIB.Local
                .FirstOrDefault(a => a.NU_PEDIDO == entity.NU_PEDIDO
                    && a.CD_CLIENTE == entity.CD_CLIENTE
                    && a.CD_EMPRESA == entity.CD_EMPRESA
                    && a.CD_PRODUTO == entity.CD_PRODUTO
                    && a.CD_FAIXA == entity.CD_FAIXA
                    && a.NU_IDENTIFICADOR == entity.NU_IDENTIFICADOR
                    && a.ID_ESPECIFICA_IDENTIFICADOR == entity.ID_ESPECIFICA_IDENTIFICADOR
                    && a.TP_LPN_TIPO == entity.TP_LPN_TIPO
                    && a.ID_LPN_EXTERNO == entity.ID_LPN_EXTERNO
                    && a.ID_ATRIBUTO == entity.ID_ATRIBUTO
                    && a.USERID == entity.USERID
                    && a.FL_CABEZAL == entity.FL_CABEZAL);

            if (attachedEntity != null)
            {
                this._context.T_TEMP_DET_PEDIDO_SAIDA_LPN_ATRIB.Remove(attachedEntity);
            }
            else
            {
                this._context.T_TEMP_DET_PEDIDO_SAIDA_LPN_ATRIB.Attach(entity);
                this._context.T_TEMP_DET_PEDIDO_SAIDA_LPN_ATRIB.Remove(entity);
            }
        }

        public virtual void DeleteDetallePedidoAtributoDefinicion(DetallePedidoAtributoDefinicion atributo)
        {
            var entity = this._lpnMapper.MapToEntity(atributo);

            var attachedEntity = _context.T_DET_PEDIDO_SAIDA_ATRIB_DET.Local
                .FirstOrDefault(w => w.NU_DET_PED_SAI_ATRIB == entity.NU_DET_PED_SAI_ATRIB
                    && w.ID_ATRIBUTO == entity.ID_ATRIBUTO
                    && w.FL_CABEZAL == entity.FL_CABEZAL);

            if (attachedEntity != null)
            {
                this._context.T_DET_PEDIDO_SAIDA_ATRIB_DET.Remove(attachedEntity);
            }
            else
            {
                this._context.T_DET_PEDIDO_SAIDA_ATRIB_DET.Attach(entity);
                this._context.T_DET_PEDIDO_SAIDA_ATRIB_DET.Remove(entity);
            }
        }

        public virtual void DeleteDetallePedidoLpnAtributo(DetallePedidoLpnAtributo detLpnAtributo)
        {
            var entity = this._lpnMapper.MapToEntity(detLpnAtributo);

            var attachedEntity = _context.T_DET_PEDIDO_SAIDA_LPN_ATRIB.Local
                .FirstOrDefault(d =>
                       d.NU_PEDIDO == entity.NU_PEDIDO
                    && d.CD_CLIENTE == entity.CD_CLIENTE
                    && d.CD_EMPRESA == entity.CD_EMPRESA
                    && d.CD_PRODUTO == entity.CD_PRODUTO
                    && d.CD_FAIXA == entity.CD_FAIXA
                    && d.NU_IDENTIFICADOR == entity.NU_IDENTIFICADOR
                    && d.ID_ESPECIFICA_IDENTIFICADOR == entity.ID_ESPECIFICA_IDENTIFICADOR
                    && d.TP_LPN_TIPO == entity.TP_LPN_TIPO
                    && d.ID_LPN_EXTERNO == entity.ID_LPN_EXTERNO
                    && d.NU_DET_PED_SAI_ATRIB == entity.NU_DET_PED_SAI_ATRIB);

            if (attachedEntity != null)
            {
                this._context.T_DET_PEDIDO_SAIDA_LPN_ATRIB.Remove(attachedEntity);
            }
            else
            {
                this._context.T_DET_PEDIDO_SAIDA_LPN_ATRIB.Attach(entity);
                this._context.T_DET_PEDIDO_SAIDA_LPN_ATRIB.Remove(entity);
            }
        }

        public virtual void DeleteDetallePedidoAtributo(DetallePedidoAtributo detAtributo)
        {
            var entity = this._lpnMapper.MapToEntity(detAtributo);

            var attachedEntity = _context.T_DET_PEDIDO_SAIDA_ATRIB.Local
                .FirstOrDefault(d =>
                       d.NU_PEDIDO == entity.NU_PEDIDO
                    && d.CD_CLIENTE == entity.CD_CLIENTE
                    && d.CD_EMPRESA == entity.CD_EMPRESA
                    && d.CD_PRODUTO == entity.CD_PRODUTO
                    && d.CD_FAIXA == entity.CD_FAIXA
                    && d.NU_IDENTIFICADOR == entity.NU_IDENTIFICADOR
                    && d.ID_ESPECIFICA_IDENTIFICADOR == entity.ID_ESPECIFICA_IDENTIFICADOR
                    && d.NU_DET_PED_SAI_ATRIB == entity.NU_DET_PED_SAI_ATRIB);

            if (attachedEntity != null)
            {
                this._context.T_DET_PEDIDO_SAIDA_ATRIB.Remove(attachedEntity);
            }
            else
            {
                this._context.T_DET_PEDIDO_SAIDA_ATRIB.Attach(entity);
                this._context.T_DET_PEDIDO_SAIDA_ATRIB.Remove(entity);
            }
        }

        #endregion

        #region Dapper

        #region Get

        public virtual async Task<Lpn> GetLpnOrNull(long nuLpn, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                var model = GetLpn(new Lpn
                {
                    NumeroLPN = nuLpn
                }, connection);

                Fill(connection, model);

                return model;
            }
        }

        public virtual List<Lpn> GetLpnsActivos(IEnumerable<Lpn> lpns)
        {
            List<Lpn> resultado = new List<Lpn>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_LPN_TEMP (ID_LPN_EXTERNO, TP_LPN_TIPO, CD_EMPRESA) VALUES (:IdExterno, :Tipo, :Empresa)";
                    _dapper.Execute(connection, sql, lpns, transaction: tran);

                    sql = GetSqlSelectLpn() +
                        @" INNER JOIN T_LPN_TEMP T ON l.ID_LPN_EXTERNO = T.ID_LPN_EXTERNO 
                            AND l.TP_LPN_TIPO = T.TP_LPN_TIPO
                            AND l.CD_EMPRESA = T.CD_EMPRESA
                        WHERE l.ID_ESTADO = 'LPNACT' ";

                    resultado = _dapper.Query<Lpn>(connection, sql, transaction: tran).ToList();

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual List<LpnDetalle> GetDetallesLpnActivo(IEnumerable<LpnDetalle> productos)
        {
            List<LpnDetalle> resultado = new List<LpnDetalle>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_LPN_TEMP (ID_LPN_EXTERNO, TP_LPN_TIPO, CD_EMPRESA) VALUES (:IdExterno, :Tipo,:Empresa)";
                    _dapper.Execute(connection, sql, productos, transaction: tran);

                    sql = @"SELECT 
                                P.ID_LPN_DET as Id,
                                P.NU_LPN as NumeroLPN,
                                P.CD_PRODUTO as CodigoProducto,
                                P.CD_FAIXA as Faixa,
                                P.CD_EMPRESA as Empresa,
                                P.NU_IDENTIFICADOR as Lote,
                                P.NU_TRANSACCION as NumeroTransaccion,
                                P.DT_FABRICACAO as Vencimiento,
                                P.QT_ESTOQUE as Cantidad,
                                P.QT_DECLARADA as CantidadDeclarada,
                                P.QT_RECIBIDA as CantidadRecibida,
                                P.ID_LINEA_SISTEMA_EXTERNO as IdLineaSistemaExterno,
                                P.QT_RESERVA_SAIDA as CantidadReserva,
                                P.NU_TRANSACCION_DELETE as NumeroTransaccionDelete,
                                P.QT_EXPEDIDA as CantidadExpedida,
                                P.ID_AVERIA as IdAveria,
                                P.ID_INVENTARIO as IdInventario,
                                P.ID_CTRL_CALIDAD as IdCtrlCalidad 
                            FROM (
                                    SELECT
                                        T.TP_LPN_TIPO,
                                        T.ID_LPN_EXTERNO,
                                        P.ID_LPN_DET,
                                        P.NU_LPN,
                                        P.CD_PRODUTO,
                                        P.CD_FAIXA,
                                        P.CD_EMPRESA,
                                        P.NU_IDENTIFICADOR,
                                        P.NU_TRANSACCION,
                                        P.DT_FABRICACAO,
                                        P.QT_ESTOQUE,
                                        P.QT_DECLARADA,
                                        P.QT_RECIBIDA,
                                        P.ID_LINEA_SISTEMA_EXTERNO,
                                        P.QT_RESERVA_SAIDA,
                                        P.NU_TRANSACCION_DELETE,
                                        P.QT_EXPEDIDA,
                                        P.ID_AVERIA,
                                        P.ID_INVENTARIO,
                                        P.ID_CTRL_CALIDAD 
                                    FROM T_LPN T 
                                    INNER JOIN T_LPN_DET P ON P.NU_LPN = T.NU_LPN
                                    WHERE T.ID_ESTADO = 'LPNACT'
                            ) P
                        INNER JOIN T_LPN_TEMP T ON P.ID_LPN_EXTERNO = T.ID_LPN_EXTERNO 
                            AND P.TP_LPN_TIPO = T.TP_LPN_TIPO
                            AND  P.CD_EMPRESA = T.CD_EMPRESA";

                    resultado = _dapper.Query<LpnDetalle>(connection, sql, transaction: tran).ToList();

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual List<LpnDetalle> GetDetallesLpn(IEnumerable<Lpn> Lpn)
        {
            List<LpnDetalle> resultado = new List<LpnDetalle>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_LPN_TEMP (ID_LPN_EXTERNO, TP_LPN_TIPO, CD_EMPRESA) VALUES (:IdExterno, :Tipo, :Empresa)";
                    _dapper.Execute(connection, sql, Lpn, transaction: tran);

                    sql = @"SELECT 
                                P.ID_LPN_DET as Id,
                                P.NU_LPN as NumeroLPN,
                                P.CD_PRODUTO as CodigoProducto,
                                P.CD_FAIXA as Faixa,
                                P.CD_EMPRESA as Empresa,
                                P.NU_IDENTIFICADOR as Lote,
                                P.NU_TRANSACCION as NumeroTransaccion,
                                P.DT_FABRICACAO as Vencimiento,
                                P.QT_ESTOQUE as Cantidad,
                                P.QT_DECLARADA as CantidadDeclarada,
                                P.QT_RECIBIDA as CantidadRecibida,
                                P.ID_LINEA_SISTEMA_EXTERNO as IdLineaSistemaExterno,
                                P.QT_RESERVA_SAIDA as CantidadReserva,
                                P.NU_TRANSACCION_DELETE as NumeroTransaccionDelete,
                                P.QT_EXPEDIDA as CantidadExpedida,
                                P.ID_AVERIA as IdAveria,
                                P.ID_INVENTARIO as IdInventario,
                                P.ID_CTRL_CALIDAD as IdCtrlCalidad 
                            FROM (
                                    SELECT
                                        T.TP_LPN_TIPO,
                                        T.ID_LPN_EXTERNO,
                                        P.ID_LPN_DET,
                                        P.NU_LPN,
                                        P.CD_PRODUTO,
                                        P.CD_FAIXA,
                                        P.CD_EMPRESA,
                                        P.NU_IDENTIFICADOR,
                                        P.NU_TRANSACCION,
                                        P.DT_FABRICACAO,
                                        P.QT_ESTOQUE,
                                        P.QT_DECLARADA,
                                        P.QT_RECIBIDA,
                                        P.ID_LINEA_SISTEMA_EXTERNO,
                                        P.QT_RESERVA_SAIDA,
                                        P.NU_TRANSACCION_DELETE,
                                        P.QT_EXPEDIDA,
                                        P.ID_AVERIA,
                                        P.ID_INVENTARIO,
                                        P.ID_CTRL_CALIDAD 
                                    FROM T_LPN T 
                                    INNER JOIN T_LPN_DET P ON P.NU_LPN = T.NU_LPN
                                    WHERE T.ID_ESTADO = 'LPNACT'
                            ) P
                        INNER JOIN T_LPN_TEMP T ON P.ID_LPN_EXTERNO = T.ID_LPN_EXTERNO 
                            AND P.TP_LPN_TIPO = T.TP_LPN_TIPO
                            AND  P.CD_EMPRESA = T.CD_EMPRESA";

                    resultado = _dapper.Query<LpnDetalle>(connection, sql, transaction: tran).ToList();

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual List<LpnDetalle> GetDetallesLpnConStock(IEnumerable<Lpn> Lpn)
        {
            List<LpnDetalle> resultado = new List<LpnDetalle>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_LPN_TEMP (ID_LPN_EXTERNO, TP_LPN_TIPO, CD_EMPRESA) VALUES (:IdExterno, :Tipo, :Empresa)";
                    _dapper.Execute(connection, sql, Lpn, transaction: tran);

                    sql = @"SELECT 
                                P.ID_LPN_DET as Id,
                                P.NU_LPN as NumeroLPN,
                                P.CD_PRODUTO as CodigoProducto,
                                P.CD_FAIXA as Faixa,
                                P.CD_EMPRESA as Empresa,
                                P.NU_IDENTIFICADOR as Lote,
                                P.NU_TRANSACCION as NumeroTransaccion,
                                P.DT_FABRICACAO as Vencimiento,
                                P.QT_ESTOQUE as Cantidad,
                                P.QT_DECLARADA as CantidadDeclarada,
                                P.QT_RECIBIDA as CantidadRecibida,
                                P.ID_LINEA_SISTEMA_EXTERNO as IdLineaSistemaExterno,
                                P.QT_RESERVA_SAIDA as CantidadReserva,
                                P.NU_TRANSACCION_DELETE as NumeroTransaccionDelete,
                                P.QT_EXPEDIDA as CantidadExpedida,
                                P.ID_AVERIA as IdAveria,
                                P.ID_INVENTARIO as IdInventario,
                                P.ID_CTRL_CALIDAD as IdCtrlCalidad 
                            FROM (
                                    SELECT
                                        T.TP_LPN_TIPO,
                                        T.ID_LPN_EXTERNO,
                                        P.ID_LPN_DET,
                                        P.NU_LPN,
                                        P.CD_PRODUTO,
                                        P.CD_FAIXA,
                                        P.CD_EMPRESA,
                                        P.NU_IDENTIFICADOR,
                                        P.NU_TRANSACCION,
                                        P.DT_FABRICACAO,
                                        P.QT_ESTOQUE,
                                        P.QT_DECLARADA,
                                        P.QT_RECIBIDA,
                                        P.ID_LINEA_SISTEMA_EXTERNO,
                                        P.QT_RESERVA_SAIDA,
                                        P.NU_TRANSACCION_DELETE,
                                        P.QT_EXPEDIDA,
                                        P.ID_AVERIA,
                                        P.ID_INVENTARIO,
                                        P.ID_CTRL_CALIDAD 
                                    FROM T_LPN T 
                                    INNER JOIN T_LPN_DET P ON P.NU_LPN = T.NU_LPN
                                    WHERE P.QT_ESTOQUE > 0
                            ) P
                        INNER JOIN T_LPN_TEMP T ON P.ID_LPN_EXTERNO = T.ID_LPN_EXTERNO 
                            AND P.TP_LPN_TIPO = T.TP_LPN_TIPO
                            AND  P.CD_EMPRESA = T.CD_EMPRESA";

                    resultado = _dapper.Query<LpnDetalle>(connection, sql, transaction: tran).ToList();

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual async Task<Lpn> GetLpnOrNull(string idExterno, string tpLpn, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                var model = GetLpn(new Lpn
                {
                    IdExterno = idExterno,
                    Tipo = tpLpn
                }, connection);

                Fill(connection, model);

                return model;
            }
        }

        public virtual Lpn GetLpn(Lpn model, DbConnection connection)
        {
            string sql = GetSqlSelectLpn();

            if (!string.IsNullOrEmpty(model.IdExterno) && !string.IsNullOrEmpty(model.Tipo))
                sql += " WHERE l.ID_LPN_EXTERNO = :idExterno AND  l.TP_LPN_TIPO= :tpLpn ORDER BY NU_LPN DESC ";
            else
                sql += " WHERE l.NU_LPN = :nuLpn ";

            return _dapper.Query<Lpn>(connection, sql, param: new
            {
                nuLpn = model.NumeroLPN,
                idExterno = model.IdExterno,
                tpLpn = model.Tipo
            }, commandType: CommandType.Text).FirstOrDefault();
        }

        public virtual void Fill(DbConnection connection, Lpn model)
        {
            if (model != null)
            {
                model.Detalles = GetDetallesLpn(connection, new LpnDetalle()
                {
                    NumeroLPN = model.NumeroLPN
                });
            }
        }

        public virtual List<LpnDetalle> GetDetallesLpn(DbConnection connection, LpnDetalle model)
        {
            string sql = @"SELECT 
                            ld.ID_LPN_DET as Id,
                            ld.NU_LPN as NumeroLPN,
                            ld.CD_PRODUTO as CodigoProducto,
                            ld.CD_FAIXA as Faixa,
                            ld.CD_EMPRESA as Empresa,
                            ld.NU_IDENTIFICADOR as Lote,
                            ld.NU_TRANSACCION as NumeroTransaccion,
                            ld.NU_TRANSACCION_DELETE as NumeroTransaccionDelete,
                            ld.DT_FABRICACAO as Vencimiento,
                            ld.QT_ESTOQUE as Cantidad,
                            ld.QT_DECLARADA as CantidadDeclarada,
                            ld.QT_RECIBIDA as CantidadRecibida,
                            ld.ID_LINEA_SISTEMA_EXTERNO as IdLineaSistemaExterno,
                            ld.QT_RESERVA_SAIDA as CantidadReserva,
                            ld.QT_EXPEDIDA as CantidadExpedida,
                            ld.ID_AVERIA as IdAveria,
                            ld.ID_INVENTARIO as IdInventario,
                            ld.ID_CTRL_CALIDAD as IdCtrlCalidad,
                            ld.CD_MOTIVO_AVERIA as MotivoAveria,
                            dd.DS_DOMINIO_VALOR as DescripcionMotivoAveria
                        FROM T_LPN_DET ld
                        LEFT JOIN T_DET_DOMINIO dd ON dd.CD_DOMINIO = 'MOTAVE' AND ld.CD_MOTIVO_AVERIA = dd.NU_DOMINIO
                        WHERE ld.NU_LPN = :nuLpn";

            return _dapper.Query<LpnDetalle>(connection, sql, param: new { nuLpn = model.NumeroLPN }, commandType: CommandType.Text).ToList();
        }

        public virtual IEnumerable<Lpn> GetLpns(IEnumerable<Lpn> lpns)
        {
            IEnumerable<Lpn> resultado = new List<Lpn>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_LPN_TEMP (ID_LPN_EXTERNO, TP_LPN_TIPO) VALUES (:IdExterno, :Tipo)";
                    _dapper.Execute(connection, sql, lpns, transaction: tran);

                    sql = GetSqlSelectLpn() +
                        @" INNER JOIN T_LPN_TEMP T ON l.ID_LPN_EXTERNO = T.ID_LPN_EXTERNO 
                            AND l.TP_LPN_TIPO = T.TP_LPN_TIPO ";

                    resultado = _dapper.Query<Lpn>(connection, sql, transaction: tran).ToList();

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public static string GetSqlSelectLpn()
        {
            return @"SELECT 
                        l.NU_LPN as NumeroLPN,
                        l.ID_LPN_EXTERNO as IdExterno,
                        l.TP_LPN_TIPO as Tipo,
                        l.ID_ESTADO as Estado,
                        l.CD_ENDERECO as Ubicacion,
                        l.DT_ADDROW as FechaAdicion,
                        l.DT_ACTIVACION as FechaActivacion,
                        l.DT_FIN as FechaFin,
                        l.NU_TRANSACCION as NumeroTransaccion,
                        l.NU_TRANSACCION_DELETE as NumeroTransaccionDelete,
                        l.CD_EMPRESA as Empresa,
                        l.ID_PACKING as IdPacking,
                        l.DT_UPDROW as FechaModificacion,
                        l.NU_AGENDA as NroAgenda,
                        l.FL_DISPONIBLE_LIBERACION as DisponibleLiberacion
                    FROM T_LPN l";
        }

        public virtual IEnumerable<LpnTipo> GetTiposLpn(IEnumerable<LpnTipo> lpns)
        {
            IEnumerable<LpnTipo> resultado = new List<LpnTipo>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_LPN_TIPO_TEMP (TP_LPN_TIPO) VALUES (:Tipo)";
                    _dapper.Execute(connection, sql, lpns, transaction: tran);

                    sql = GetSqlSelectTipoLpn() +
                        @" INNER JOIN T_LPN_TIPO_TEMP T ON lt.TP_LPN_TIPO = T.TP_LPN_TIPO ";

                    resultado = _dapper.Query<LpnTipo>(connection, sql, transaction: tran).ToList();

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public static string GetSqlSelectTipoLpn()
        {
            return @"SELECT 
	                    lt.TP_LPN_TIPO as Tipo,
	                    lt.NM_LPN_TIPO as Nombre,
	                    lt.DS_LPN_TIPO as Descripcion,
	                    lt.FL_PERMITE_CONSOLIDAR as PermiteConsolidar,
	                    lt.FL_PERMITE_EXTRAER_LINEAS as PermiteExtraerLineas,
	                    lt.FL_PERMITE_AGREGAR_LINEAS as PermiteAgregarLineas,
	                    lt.FL_CREAR_SOLO_AL_INGRESO as CrearSoloAlIngreso,
	                    lt.FL_MULTIPRODUCTO as MultiProducto,
	                    lt.FL_MULTI_LOTE as MultiLote,
	                    lt.FL_PERMITE_ANIDACION as PermiteAnidacion,
	                    lt.NU_TEMPLATE_ETIQUETA as NumeroTemplate,
	                    lt.NU_COMPONENTE as NumeroComponente,
	                    lt.FL_CONTENEDOR_LPN as ContenedorLPN,
	                    lt.NU_SEQ_LPN as NumeroSecuencia,
	                    lt.FL_PERMITE_GENERAR as PermiteGenerar,
	                    lt.FL_INGRESO_RECEPCION_ATRIBUTO as IngresoRecepcionAtributo,
	                    lt.FL_INGRESO_PICKING_ATRIBUTO as IngresoPickingAtributo,
	                    lt.VL_PREFIJO as Prefijo,
	                    lt.TP_ETIQUETA_RECEPCION as EtiquetaRecepcion,
	                    lt.FL_PERMITE_DESTRUIR_ALM as PermiteDestruirAlmacenaje
                    FROM T_LPN_TIPO lt ";
        }

        public virtual IEnumerable<LpnTipoAtributo> GetTipoLpnAtributos(IEnumerable<LpnTipo> lpns)
        {
            IEnumerable<LpnTipoAtributo> resultado = new List<LpnTipoAtributo>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_LPN_TIPO_ATRIBUTO_TEMP (TP_LPN_TIPO) VALUES (:Tipo)";
                    _dapper.Execute(connection, sql, lpns, transaction: tran);

                    sql = GetSqlSelectLpnTipoAtributo() +
                        @" INNER JOIN T_LPN_TIPO_ATRIBUTO_TEMP T ON ta.TP_LPN_TIPO = T.TP_LPN_TIPO ";

                    resultado = _dapper.Query<LpnTipoAtributo>(connection, sql, transaction: tran).ToList();

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public static string GetSqlSelectLpnTipoAtributo()
        {
            return @"SELECT 
	                    ta.TP_LPN_TIPO as TipoLpn,
	                    ta.ID_ATRIBUTO as IdAtributo,
	                    ta.VL_INICIAL as ValorInicial,
	                    ta.FL_REQUERIDO as Requerido,
	                    ta.VL_VALIDO_INTERFAZ as ValidoInterfaz,
	                    ta.NU_ORDEN as Orden,
	                    ta.ID_CONSOLIDACION_TIPO as IdConsolidador,
                        a.NM_ATRIBUTO as NombreAtributo,
                        ta.ID_ESTADO_INICIAL as EstadoInicial
                    FROM T_LPN_TIPO_ATRIBUTO ta
                    INNER JOIN T_ATRIBUTO a ON a.ID_ATRIBUTO = ta.ID_ATRIBUTO ";
        }

        public virtual IEnumerable<LpnTipoAtributoDet> GetTipoLpnAtributosDetalle(IEnumerable<LpnTipo> lpns)
        {
            IEnumerable<LpnTipoAtributoDet> resultado = new List<LpnTipoAtributoDet>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_LPN_TIPO_ATRIBUTO_TEMP (TP_LPN_TIPO) VALUES (:Tipo)";
                    _dapper.Execute(connection, sql, lpns, transaction: tran);

                    sql = GetSqlSelectLpnTipoAtributoDet() +
                        @" INNER JOIN T_LPN_TIPO_ATRIBUTO_TEMP T ON tad.TP_LPN_TIPO = T.TP_LPN_TIPO ";

                    resultado = _dapper.Query<LpnTipoAtributoDet>(connection, sql, transaction: tran).ToList();

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public static string GetSqlSelectLpnTipoAtributoDet()
        {
            return @"SELECT 
	                    tad.TP_LPN_TIPO as TipoLpn,
	                    tad.ID_ATRIBUTO as IdAtributo,
	                    tad.VL_INICIAL as ValorInicial,
	                    tad.FL_REQUERIDO as Requerido,
	                    tad.VL_VALIDO_INTERFAZ as ValidoInterfaz,
	                    tad.NU_ORDEN as Orden,
                        a.NM_ATRIBUTO as NombreAtributo ,
                        tad.ID_ESTADO_INICIAL as EstadoInicial
                    FROM T_LPN_TIPO_ATRIBUTO_DET tad
                    INNER JOIN T_ATRIBUTO a ON a.ID_ATRIBUTO = tad.ID_ATRIBUTO ";
        }

        public virtual IEnumerable<LpnBarras> GetLpnBarrasEmpresa(int empresa, IEnumerable<LpnBarras> lpnBarras)
        {
            IEnumerable<LpnBarras> resultado = new List<LpnBarras>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_LPN_BARRAS_TEMP (CD_BARRAS) VALUES (:CodigoBarras)";
                    _dapper.Execute(connection, sql, lpnBarras, transaction: tran);

                    sql = $@"SELECT 
                                lb.ID_LPN_BARRAS as IdLpnBarras,
                                lb.NU_LPN as NumeroLpn,
                                lb.CD_BARRAS as CodigoBarras,
                                lb.NU_ORDEN as Orden,
                                lb.TP_BARRAS as Tipo,
                                l.ID_ESTADO as EstadoLpn
                            FROM T_LPN_BARRAS lb
                            INNER JOIN T_LPN_BARRAS_TEMP T ON lb.CD_BARRAS = T.CD_BARRAS 
                            INNER JOIN T_LPN l ON lb.NU_LPN = l.NU_LPN 
                            WHERE l.CD_EMPRESA = {empresa} ";

                    resultado = _dapper.Query<LpnBarras>(connection, sql, transaction: tran).ToList();

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual IEnumerable<string> GetLpnTipoBarras()
        {
            IEnumerable<string> resultado = new HashSet<string>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();
                string sql = GetSqlSelectTipoBarra();
                resultado = _dapper.Query<string>(connection, sql);
            }

            return resultado;
        }

        public static string GetSqlSelectTipoBarra()
        {
            return @"SELECT tp.TP_BARRAS FROM T_LPN_TIPO_BARRAS tp ";
        }

        public virtual IEnumerable<Stock> GetStocksLpn(IEnumerable<Stock> stocks)
        {
            IEnumerable<Stock> resultado = new List<Stock>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_STOCK_TEMP (CD_ENDERECO, CD_EMPRESA, CD_PRODUTO, NU_IDENTIFICADOR, CD_FAIXA) 
                                   VALUES (:Ubicacion, :Empresa, :Producto, :Identificador, :Faixa)";
                    _dapper.Execute(connection, sql, stocks, transaction: tran);

                    sql = @"SELECT
                        spl.CD_ENDERECO as Ubicacion,
                        spl.CD_EMPRESA as Empresa,
                        spl.CD_PRODUTO as Producto,
                        spl.CD_FAIXA as Faixa,
                        spl.NU_IDENTIFICADOR as Identificador,
                        spl.QT_ESTOQUE_LPN as CantidadLpn,
                        spl.QT_RESERVA_LPN as CantidadReservaLpn,
                        spl.QT_DISPONIBLE_LPN as CantidadDisponibleLpn
                    FROM V_STOCK_PRODUCTO_LPN spl 
                    INNER JOIN T_STOCK_TEMP T ON spl.CD_EMPRESA = T.CD_EMPRESA
                        AND spl.CD_ENDERECO = T.CD_ENDERECO                            
                        AND spl.CD_FAIXA = T.CD_FAIXA
                        AND spl.CD_PRODUTO = T.CD_PRODUTO
                        AND spl.NU_IDENTIFICADOR = T.NU_IDENTIFICADOR";

                    resultado = _dapper.Query<Stock>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual IEnumerable<LpnDetalleAtributo> GetAtributosDetallesLpn(IEnumerable<LpnDetalleAtributo> detalles)
        {
            var resultado = new List<LpnDetalleAtributo>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    // Se omite el lote dado que se recalcula al validar el pedido, considerando si el producto lo maneja
                    string sql = @"INSERT INTO T_LPN_DET_ATRIBUTO_TEMP (ID_LPN_EXTERNO, TP_LPN_TIPO, CD_EMPRESA, CD_PRODUTO, CD_FAIXA, NM_ATRIBUTO) VALUES (:IdLpnExterno, :Tipo, :Empresa, :Producto, :Faixa, :NombreAtributo)";
                    _dapper.Execute(connection, sql, detalles, transaction: tran);

                    sql = @"
                        SELECT 
                            L.ID_LPN_EXTERNO as IdLpnExterno,
                            L.TP_LPN_TIPO as Tipo,
                            L.CD_EMPRESA as Empresa,
                            LDA.CD_PRODUTO as Producto,
                            LDA.CD_FAIXA as Faixa,
                            LDA.NU_IDENTIFICADOR as Lote,
                            A.NM_ATRIBUTO as NombreAtributo, 
                            A.ID_ATRIBUTO as IdAtributo
                        FROM T_LPN_DET_ATRIBUTO LDA 
                        INNER JOIN T_LPN L ON L.NU_LPN = LDA.NU_LPN
                        INNER JOIN T_ATRIBUTO A ON A.ID_ATRIBUTO = LDA.ID_ATRIBUTO
                        INNER JOIN T_LPN_DET_ATRIBUTO_TEMP T ON T.ID_LPN_EXTERNO = L.ID_LPN_EXTERNO
                            AND T.TP_LPN_TIPO = L.TP_LPN_TIPO
                            AND T.CD_EMPRESA = L.CD_EMPRESA
                            AND T.CD_PRODUTO = LDA.CD_PRODUTO
                            AND T.CD_FAIXA = LDA.CD_FAIXA
                            AND T.NM_ATRIBUTO = A.NM_ATRIBUTO
                        GROUP BY 
                            L.ID_LPN_EXTERNO,
                            L.TP_LPN_TIPO,
                            L.CD_EMPRESA,
                            LDA.CD_PRODUTO,
                            LDA.CD_FAIXA,
                            LDA.NU_IDENTIFICADOR,
                            A.NM_ATRIBUTO, 
                            A.ID_ATRIBUTO ";

                    resultado = _dapper.Query<LpnDetalleAtributo>(connection, sql, transaction: tran).ToList();

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual IEnumerable<LpnAtributo> GetLpnAtributosCabezal(IEnumerable<Lpn> lpns)
        {
            IEnumerable<LpnAtributo> resultado = new List<LpnAtributo>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_LPN_TEMP (NU_LPN) VALUES (:NumeroLPN)";
                    _dapper.Execute(connection, sql, lpns, transaction: tran);

                    sql = @"SELECT 
                                la.NU_LPN as NumeroLpn,
                                la.TP_LPN_TIPO as Tipo,
                                la.ID_ATRIBUTO as Id,
                                la.VL_LPN_ATRIBUTO as Valor,
                                la.ID_ESTADO as Estado,
                                la.NU_TRANSACCION_DELETE as NumeroTransaccion,
                                la.NU_TRANSACCION as NumeroTransaccionDelete,
                                AT.NM_ATRIBUTO as Nombre
                            FROM T_LPN_ATRIBUTO la 
                            INNER JOIN T_LPN_TEMP T ON la.NU_LPN = T.NU_LPN 
                            INNER JOIN T_ATRIBUTO AT ON la.ID_ATRIBUTO = AT.ID_ATRIBUTO 
                            WHERE AT.ID_ATRIBUTO_TIPO != :tipoAtributo ";

                    resultado = _dapper.Query<LpnAtributo>(connection, sql, param: new { tipoAtributo = TipoAtributoDb.SISTEMA }, transaction: tran).ToList();

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual IEnumerable<LpnDetalle> GetDetallesLpnUbicacion(IEnumerable<LpnDetalle> keys)
        {
            var resultado = new List<LpnDetalle>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    var sql = @"INSERT INTO T_LPN_TEMP (NU_LPN, ID_LPN_DET) VALUES (:NumeroLPN, :Id)";
                    _dapper.Execute(connection, sql, keys, transaction: tran);

                    sql = @"SELECT 
                                LD.ID_LPN_DET as Id,
                                LD.NU_LPN as NumeroLPN,
                                LD.CD_PRODUTO as CodigoProducto,
                                LD.CD_FAIXA as Faixa,
                                LD.CD_EMPRESA as Empresa,
                                LD.NU_IDENTIFICADOR as Lote,
                                LD.NU_TRANSACCION as NumeroTransaccion,
                                LD.NU_TRANSACCION_DELETE as NumeroTransaccionDelete,
                                LD.DT_FABRICACAO as Vencimiento,
                                LD.QT_ESTOQUE as Cantidad,
                                LD.QT_DECLARADA as CantidadDeclarada,
                                LD.QT_RECIBIDA as CantidadRecibida,
                                LD.ID_LINEA_SISTEMA_EXTERNO as IdLineaSistemaExterno,
                                LD.QT_RESERVA_SAIDA as CantidadReserva,
                                LD.QT_EXPEDIDA as CantidadExpedida,
                                LD.ID_AVERIA as IdAveria,
                                LD.ID_INVENTARIO as IdInventario,
                                LD.ID_CTRL_CALIDAD as IdCtrlCalidad,
                                EE.CD_ENDERECO as Ubicacion,
                                EE.NU_PREDIO as Predio                        
                            FROM T_LPN_DET LD
                            INNER JOIN T_LPN_TEMP T ON LD.ID_LPN_DET = T.ID_LPN_DET AND LD.NU_LPN = T.NU_LPN
                            INNER JOIN T_LPN L ON LD.NU_LPN = L.NU_LPN
                            INNER JOIN T_ENDERECO_ESTOQUE EE ON L.CD_ENDERECO = EE.CD_ENDERECO ";

                    resultado = _dapper.Query<LpnDetalle>(connection, sql, transaction: tran).ToList();

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual IEnumerable<LpnDetalle> GetDetallesLpn(IEnumerable<LpnDetalle> registros)
        {
            string sql = @" INSERT INTO T_LPN_TEMP (NU_LPN, ID_LPN_DET) 
                            VALUES (:NumeroLPN, :Id)";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, registros, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            sql = @"SELECT 
                        LD.ID_LPN_DET as Id,
                        LD.NU_LPN as NumeroLPN,
                        LD.CD_PRODUTO as CodigoProducto,
                        LD.CD_FAIXA as Faixa,
                        LD.CD_EMPRESA as Empresa,
                        LD.NU_IDENTIFICADOR as Lote,
                        LD.NU_TRANSACCION as NumeroTransaccion,
                        LD.NU_TRANSACCION_DELETE as NumeroTransaccionDelete,
                        LD.DT_FABRICACAO as Vencimiento,
                        LD.QT_ESTOQUE as Cantidad,
                        LD.QT_DECLARADA as CantidadDeclarada,
                        LD.QT_RECIBIDA as CantidadRecibida,
                        LD.ID_LINEA_SISTEMA_EXTERNO as IdLineaSistemaExterno,
                        LD.QT_RESERVA_SAIDA as CantidadReserva,
                        LD.QT_EXPEDIDA as CantidadExpedida,
                        LD.ID_AVERIA as IdAveria,
                        LD.ID_INVENTARIO as IdInventario,
                        LD.ID_CTRL_CALIDAD as IdCtrlCalidad,
                        LD.CD_MOTIVO_AVERIA as MotivoAveria
                    FROM T_LPN_DET LD
                    INNER JOIN T_LPN_TEMP T ON LD.NU_LPN = T.NU_LPN
                        AND LD.ID_LPN_DET = T.ID_LPN_DET ";

            var detalles = _dapper.Query<LpnDetalle>(_context.Database.GetDbConnection(), sql, transaction: _context.Database.CurrentTransaction?.GetDbTransaction()).ToList();

            sql = @"DELETE T_LPN_TEMP WHERE NU_LPN = :NumeroLPN AND ID_LPN_DET = :Id";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, registros, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            return detalles;
        }

        public virtual IEnumerable<Lpn> GetLpnsByIds(IEnumerable<Lpn> ids)
        {
            string sql = @" INSERT INTO T_LPN_TEMP (NU_LPN) 
                            VALUES (:NumeroLPN)";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, ids, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            sql = GetSqlSelectLpn() +
                  @" INNER JOIN T_LPN_TEMP T ON l.NU_LPN = T.NU_LPN ";

            var detalles = _dapper.Query<Lpn>(_context.Database.GetDbConnection(), sql, transaction: _context.Database.CurrentTransaction?.GetDbTransaction()).ToList();

            sql = @"DELETE T_LPN_TEMP WHERE NU_LPN = :NumeroLPN ";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, ids, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            return detalles;
        }

        public virtual IEnumerable<Lpn> GetLpnCriterios(IEnumerable<CriterioControlCalidadAPI> criteriosEtiqueta)
        {
            IEnumerable<Lpn> resultado = new List<Lpn>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    _dapper.BulkInsert(connection, tran, criteriosEtiqueta, "T_STOCK_PREDIO_TEMP", new Dictionary<string, Func<CriterioControlCalidadAPI, ColumnInfo>>
                    {
                        { "NU_PREDIO", x => new ColumnInfo(x.Predio)},
                        { "CD_PRODUTO", x => new ColumnInfo(x.Producto)},
                        { "CD_EMPRESA", x => new ColumnInfo(x.Empresa)},
                        { "NU_IDENTIFICADOR", x => new ColumnInfo(x.Lote)},
                        { "CD_FAIXA", x => new ColumnInfo(x.Faixa)},
                    });

                    string sql = @"SELECT 
                                    DISTINCT
                                    l.NU_LPN as NumeroLPN,
                                    l.ID_LPN_EXTERNO as IdExterno,
                                    l.TP_LPN_TIPO as Tipo,
                                    l.ID_ESTADO as Estado,
                                    l.CD_ENDERECO as Ubicacion,
                                    l.DT_ADDROW as FechaAdicion,
                                    l.DT_ACTIVACION as FechaActivacion,
                                    l.DT_FIN as FechaFin,
                                    l.NU_TRANSACCION as NumeroTransaccion,
                                    l.NU_TRANSACCION_DELETE as NumeroTransaccionDelete,
                                    l.CD_EMPRESA as Empresa,
                                    l.ID_PACKING as IdPacking,
                                    l.DT_UPDROW as FechaModificacion,
                                    l.NU_AGENDA as NroAgenda,
                                    l.FL_DISPONIBLE_LIBERACION as DisponibleLiberacion,
                                    EE.NU_PREDIO as Predio
                                FROM T_LPN l
                               INNER JOIN T_LPN_DET DET ON l.NU_LPN = DET.NU_LPN
                               INNER JOIN T_ENDERECO_ESTOQUE EE ON l.CD_ENDERECO = EE.CD_ENDERECO
                               INNER JOIN T_STOCK_PREDIO_TEMP TEMP ON TEMP.NU_PREDIO = EE.NU_PREDIO AND TEMP.CD_FAIXA = DET.CD_FAIXA AND 
                                TEMP.CD_PRODUTO = DET.CD_PRODUTO AND TEMP.CD_EMPRESA = DET.CD_EMPRESA AND TEMP.NU_IDENTIFICADOR = DET.NU_IDENTIFICADOR
                               WHERE DET.QT_ESTOQUE > 0";

                    resultado = _dapper.Query<Lpn>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        #endregion

        #region Entrada

        public virtual async Task AddLpns(List<Lpn> lpns, ILpnServiceContext context, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                BulkSecuenciaTipoLpn(context, connection);
                var bulkContext = GetBulkOperationContext(lpns, context, connection);

                using (var tran = connection.BeginTransaction())
                {
                    await BulkInsertLpn(connection, tran, bulkContext.NewLpns);
                    await BulkInsertLpnBarras(connection, tran, bulkContext.NewLpnBarras.Values);
                    await BulkInsertLpnDetalles(connection, tran, bulkContext.NewLpnDetalles);
                    await BulkInsertLpnAtributos(connection, tran, bulkContext.NewLpnAtributos);
                    await BulkInsertLpnAtributosDetalles(connection, tran, bulkContext.NewLpnDetalleAtributos);

                    tran.Commit();
                }
            }
        }

        public virtual void BulkSecuenciaTipoLpn(ILpnServiceContext context, DbConnection connection)
        {
            string sql = GetSqlSelectTipoLpn();
            var tipos = new List<LpnTipo>();
            var tiposDb = _dapper.Query<LpnTipo>(connection, sql, transaction: _context.Database.CurrentTransaction?.GetDbTransaction()).ToList();

            foreach (var tipo in context._lpns.GroupBy(l => l.Tipo).Select(x => x.Key))
            {
                var tipoLpn = tiposDb.FirstOrDefault(x => x.Tipo == tipo);

                int cantidadLpnTipo = context._lpns.Where(x => x.Tipo == tipo).Count();
                var ti = new LpnTipo()
                {
                    Tipo = tipoLpn.Tipo,
                    NumeroSecuencia = (tipoLpn.NumeroSecuencia ?? 0) + cantidadLpnTipo,
                    Prefijo = tipoLpn.Prefijo
                };
                tipos.Add(ti);
            }
            sql = @" UPDATE T_LPN_TIPO 
                            SET NU_SEQ_LPN = :NumeroSecuencia
                            WHERE TP_LPN_TIPO = :Tipo ";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, tipos, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
            context.TiposLpn = tiposDb.ToDictionary(tp => tp.Tipo, tp => tp);
        }

        public virtual LpnBulkOperationContext GetBulkOperationContext(List<Lpn> lpns, ILpnServiceContext context, DbConnection connection)
        {
            var bulkContext = new LpnBulkOperationContext();
            var lpnIds = GetNewLpnIds(lpns.Count, connection);
            var lpnDetalleIds = GetNewLpnDetalleIds(lpns, connection);

            for (int i = 0; i < lpns.Count; i++)
            {
                var lpn = Map(lpns[i], lpnIds[i], context);
                bulkContext.NewLpns.Add(GetLpnEntity(lpn));

                BulkBarras(lpn, context, bulkContext);
                BulkAtributos(lpn, context, bulkContext);
                BulkLpnDetalles(lpn, context, bulkContext, lpnDetalleIds);
            }

            return bulkContext;
        }

        public virtual Lpn Map(Lpn request, long nuLpn, ILpnServiceContext context)
        {
            Lpn lpn = new Lpn
            {
                NumeroLPN = request.NumeroLPN = nuLpn,
                IdExterno = request.IdExterno,
                Empresa = request.Empresa,
                Tipo = request.Tipo,
                Estado = request.Estado,
                IdPacking = request.IdPacking,
                FechaAdicion = DateTime.Now,
                NumeroTransaccion = request.NumeroTransaccion,
                NumeroTransaccionDelete = request.NumeroTransaccionDelete,
                Detalles = request.Detalles,
                AtributosSinDefinir = request.AtributosSinDefinir,
                BarrasSinDefinir = request.BarrasSinDefinir,
            };

            return lpn;
        }

        public virtual void BulkBarras(Lpn lpn, ILpnServiceContext context, LpnBulkOperationContext bulkContext)
        {
            short nuOrden = 0;
            if (lpn.BarrasSinDefinir != null && lpn.BarrasSinDefinir.Count > 0)
            {
                foreach (var b in lpn.BarrasSinDefinir)
                {
                    var newBarras = new LpnBarras()
                    {
                        NumeroLpn = lpn.NumeroLPN,
                        CodigoBarras = b.CodigoBarras,
                        Orden = nuOrden,
                        Tipo = b.Tipo,
                    };

                    if (!bulkContext.NewLpnBarras.ContainsKey(newBarras.CodigoBarras))
                        bulkContext.NewLpnBarras[newBarras.CodigoBarras] = GetLpnBarrasEntity(newBarras);

                    nuOrden++;
                }
            }
            GenearCodigoDefault(lpn, context, bulkContext, nuOrden);

            var generaCodBarrasIdExterno = context.GetParametro(ParamManager.GENERAR_CB_ID_EXTERNO_LPN)?.ToUpper() == "S";
            if (generaCodBarrasIdExterno)
            {
                nuOrden++;
                GenerarCodigoIdExterno(lpn, bulkContext, nuOrden);
            }
        }

        public virtual void BulkAtributos(Lpn lpn, ILpnServiceContext context, LpnBulkOperationContext bulkContext)
        {
            var atributosTipoLpn = context.TipoLpnAtributosAsociados.GetValueOrDefault(lpn.Tipo, null);

            if (atributosTipoLpn != null && atributosTipoLpn.Count > 0)
            {
                foreach (var at in atributosTipoLpn)
                {
                    var valorAtributo = at.ValorInicial;
                    var estado = at.EstadoInicial;

                    var atributoEnviado = lpn.AtributosSinDefinir.FirstOrDefault(a => a.Nombre == at.NombreAtributo);
                    if (atributoEnviado != null && !string.IsNullOrEmpty(atributoEnviado.Valor))
                    {
                        valorAtributo = atributoEnviado.Valor;
                        estado = EstadoLpnAtributo.Ingresado;
                    }

                    var atributoLpn = new LpnAtributo()
                    {
                        NumeroLpn = lpn.NumeroLPN,
                        Tipo = lpn.Tipo,
                        Id = at.IdAtributo,
                        Valor = valorAtributo,
                        NumeroTransaccion = lpn.NumeroTransaccion,
                        Estado = estado,
                    };

                    bulkContext.NewLpnAtributos.Add(GetLpnAtributoEntity(atributoLpn));
                }
            }
        }

        public virtual void BulkLpnDetalles(Lpn lpn, ILpnServiceContext context, LpnBulkOperationContext bulkContext, List<int> detallesIds)
        {
            if (lpn.Detalles != null && lpn.Detalles.Count > 0)
            {
                foreach (var d in lpn.Detalles)
                {
                    var lpnDetalle = new LpnDetalle()
                    {
                        Id = detallesIds.FirstOrDefault(),
                        NumeroLPN = lpn.NumeroLPN,
                        NumeroTransaccion = lpn.NumeroTransaccion,

                        IdLineaSistemaExterno = d.IdLineaSistemaExterno,
                        CodigoProducto = d.CodigoProducto,
                        Empresa = d.Empresa,
                        Faixa = d.Faixa,
                        Lote = d.Lote,
                        Cantidad = d.Cantidad,
                        Vencimiento = d.Vencimiento,
                        CantidadRecibida = d.CantidadRecibida,
                        CantidadDeclarada = d.CantidadDeclarada,
                        CantidadReserva = d.CantidadReserva,
                        CantidadExpedida = d.CantidadExpedida,
                        AtributosSinDefinir = d.AtributosSinDefinir,
                        NumeroTransaccionDelete = d.NumeroTransaccionDelete,
                        IdAveria = d.IdAveria,
                        IdCtrlCalidad = d.IdCtrlCalidad,
                        IdInventario = d.IdInventario,
                    };

                    detallesIds.Remove(lpnDetalle.Id);

                    bulkContext.NewLpnDetalles.Add(GetLpnDetalleEntity(lpnDetalle));
                    BulkAtributosDetalle(lpn, lpnDetalle, context, bulkContext);
                }
            }
        }

        public virtual void BulkAtributosDetalle(Lpn lpn, LpnDetalle lpnDet, ILpnServiceContext context, LpnBulkOperationContext bulkContext)
        {
            var atributosTipoLpnDetalle = context.TipoLpnAtributosDetalleAsociados.GetValueOrDefault(lpn.Tipo, null);

            if (atributosTipoLpnDetalle != null && atributosTipoLpnDetalle.Count > 0)
            {
                foreach (var atd in atributosTipoLpnDetalle)
                {
                    var valorAtributo = atd.ValorInicial;
                    var estado = atd.EstadoInicial;

                    var atributoEnviado = lpnDet.AtributosSinDefinir.FirstOrDefault(a => a.Nombre == atd.NombreAtributo);
                    if (atributoEnviado != null && !string.IsNullOrEmpty(atributoEnviado.Valor))
                    {
                        valorAtributo = atributoEnviado.Valor;
                        estado = EstadoLpnAtributo.Ingresado;
                    }

                    var atributoLpnDetalle = new LpnDetalleAtributo()
                    {
                        IdLpnDetalle = lpnDet.Id,
                        NumeroLpn = lpnDet.NumeroLPN,
                        Tipo = lpn.Tipo,
                        IdAtributo = atd.IdAtributo,
                        ValorAtributo = valorAtributo,
                        Producto = lpnDet.CodigoProducto,
                        Faixa = lpnDet.Faixa,
                        Empresa = lpnDet.Empresa,
                        Lote = lpnDet.Lote,
                        NumeroTransaccion = lpn.NumeroTransaccion,
                        Estado = estado,
                    };

                    bulkContext.NewLpnDetalleAtributos.Add(GetLpnAtributoDetalleEntity(atributoLpnDetalle));
                }
            }
        }

        public virtual async Task BulkInsertLpn(DbConnection connection, DbTransaction tran, List<object> lpns)
        {
            string sql =
            @"INSERT INTO T_LPN 
                (NU_LPN, 
                ID_LPN_EXTERNO, 
                TP_LPN_TIPO, 
                CD_EMPRESA, 
                ID_ESTADO,
                ID_PACKING,
                DT_ADDROW,
                NU_TRANSACCION,
                NU_TRANSACCION_DELETE) 
            VALUES (
                :NumeroLPN,
                :IdExterno,
                :Tipo,
                :Empresa,
                :Estado,
                :IdPacking,
                :FechaAdicion,
                :NumeroTransaccion,
                :NumeroTransaccionDelete)";

            await _dapper.ExecuteAsync(connection, sql, lpns, transaction: tran);
        }

        public virtual async Task BulkInsertLpnDetalles(DbConnection connection, DbTransaction tran, List<object> detalles)
        {
            string sql =
            @"INSERT INTO T_LPN_DET 
                (ID_LPN_DET,
                NU_LPN,
                CD_PRODUTO,
                CD_FAIXA,
                CD_EMPRESA,
                NU_IDENTIFICADOR,
                NU_TRANSACCION,
                QT_ESTOQUE,
                DT_FABRICACAO,
                QT_DECLARADA,
                QT_RECIBIDA,
                ID_LINEA_SISTEMA_EXTERNO,
                NU_TRANSACCION_DELETE,
                QT_RESERVA_SAIDA,
                QT_EXPEDIDA,
                ID_AVERIA,
                ID_CTRL_CALIDAD,
                ID_INVENTARIO) 
            VALUES (
                :Id,
                :NumeroLPN,
                :CodigoProducto,
                :Faixa,
                :Empresa,
                :Lote,
                :NumeroTransaccion,
                :Cantidad,
                :Vencimiento,
                :CantidadDeclarada,
                :CantidadRecibida,
                :IdLineaSistemaExterno,
                :NumeroTransaccionDelete,
                :CantidadReserva,
                :CantidadExpedida,
                :IdAveria,
                :IdCtrlCalidad,
                :IdInventario)";

            await _dapper.ExecuteAsync(connection, sql, detalles, transaction: tran);
        }

        public virtual async Task BulkInsertLpnAtributos(DbConnection connection, DbTransaction tran, List<object> atributos)
        {
            string sql =
            @"INSERT INTO T_LPN_ATRIBUTO 
                (NU_LPN,
                TP_LPN_TIPO,
                ID_ATRIBUTO,
                VL_LPN_ATRIBUTO,
                NU_TRANSACCION,
                ID_ESTADO) 
            VALUES (
                :NumeroLpn,
                :Tipo,
                :IdAtributo,
                :ValorAtributo,
                :NumeroTransaccion,
                :Estado)";

            await _dapper.ExecuteAsync(connection, sql, atributos, transaction: tran);
        }

        public virtual async Task BulkInsertLpnBarras(DbConnection connection, DbTransaction tran, IEnumerable<object> barras)
        {
            string sql =
            @"INSERT INTO T_LPN_BARRAS 
                (NU_LPN, 
                CD_BARRAS,
                NU_ORDEN,
                TP_BARRAS) 
            VALUES (
                :NumeroLpn,
                :CodigoBarras,
                :Orden,
                :Tipo)";

            await _dapper.ExecuteAsync(connection, sql, barras, transaction: tran);
        }

        public virtual async Task BulkInsertLpnAtributosDetalles(DbConnection connection, DbTransaction tran, List<object> atributos)
        {
            string sql =
            @"INSERT INTO T_LPN_DET_ATRIBUTO 
                (ID_LPN_DET,
                NU_LPN,
                TP_LPN_TIPO,
                ID_ATRIBUTO,
                VL_LPN_DET_ATRIBUTO,
                CD_PRODUTO,
                CD_FAIXA,
                CD_EMPRESA,
                NU_IDENTIFICADOR,
                ID_ESTADO,
                NU_TRANSACCION) 
            VALUES (
                :IdLpnDetalle,
                :NumeroLpn,
                :Tipo,
                :IdAtributo,
                :ValorDetalleAtributo,
                :Producto,
                :Faixa,
                :Empresa,
                :Lote,
                :Estado,
                :NumeroTransaccion)";

            await _dapper.ExecuteAsync(connection, sql, atributos, transaction: tran);
        }

        public virtual object GetLpnEntity(Lpn lpn)
        {
            return new
            {
                NumeroLPN = lpn.NumeroLPN,
                IdExterno = lpn.IdExterno,
                Empresa = lpn.Empresa,
                Tipo = lpn.Tipo,
                Estado = lpn.Estado,
                IdPacking = lpn.IdPacking,
                FechaAdicion = lpn.FechaAdicion,
                NumeroTransaccion = lpn.NumeroTransaccion,
                NumeroTransaccionDelete = lpn.NumeroTransaccionDelete,
            };
        }

        public virtual object GetLpnBarrasEntity(LpnBarras lpnBarras)
        {
            return new
            {
                NumeroLpn = lpnBarras.NumeroLpn,
                CodigoBarras = lpnBarras.CodigoBarras,
                Orden = lpnBarras.Orden,
                Tipo = lpnBarras.Tipo,
            };
        }

        public virtual object GetLpnDetalleEntity(LpnDetalle lpnDetalle)
        {
            return new
            {
                Id = lpnDetalle.Id,
                NumeroLPN = lpnDetalle.NumeroLPN,
                NumeroTransaccion = lpnDetalle.NumeroTransaccion,
                IdLineaSistemaExterno = lpnDetalle.IdLineaSistemaExterno,
                CodigoProducto = lpnDetalle.CodigoProducto,
                Empresa = lpnDetalle.Empresa,
                Faixa = lpnDetalle.Faixa,
                Lote = lpnDetalle.Lote,
                Cantidad = lpnDetalle.Cantidad,
                Vencimiento = lpnDetalle.Vencimiento,
                CantidadRecibida = lpnDetalle.CantidadRecibida,
                CantidadDeclarada = lpnDetalle.CantidadDeclarada,
                CantidadReserva = lpnDetalle.CantidadReserva,
                CantidadExpedida = lpnDetalle.CantidadExpedida,
                NumeroTransaccionDelete = lpnDetalle.NumeroTransaccionDelete,
                IdAveria = lpnDetalle.IdAveria,
                IdCtrlCalidad = lpnDetalle.IdCtrlCalidad,
                IdInventario = lpnDetalle.IdInventario,
            };
        }

        public virtual object GetLpnAtributoEntity(LpnAtributo at)
        {
            return new
            {
                NumeroLpn = at.NumeroLpn,
                Tipo = at.Tipo,
                IdAtributo = at.Id,
                ValorAtributo = at.Valor,
                NumeroTransaccion = at.NumeroTransaccion,
                NumeroTransaccionDelete = at.NumeroTransaccionDelete,
                Estado = at.Estado,
            };
        }

        public virtual object GetLpnAtributoDetalleEntity(LpnDetalleAtributo atd)
        {
            return new
            {
                IdLpnDetalle = atd.IdLpnDetalle,
                NumeroLpn = atd.NumeroLpn,
                Tipo = atd.Tipo,
                IdAtributo = atd.IdAtributo,
                ValorDetalleAtributo = atd.ValorAtributo,
                Producto = atd.Producto,
                Faixa = atd.Faixa,
                Empresa = atd.Empresa,
                Lote = atd.Lote,
                NumeroTransaccion = atd.NumeroTransaccion,
                NumeroTransaccionDelete = atd.NumeroTransaccionDelete,
                Estado = atd.Estado,
            };
        }

        public virtual List<long> GetNewLpnIds(int count, DbConnection connection)
        {
            return _dapper.GetNextSequenceValues<long>(connection, Secuencias.S_LPN, count).ToList();
        }

        public virtual List<int> GetNewLpnDetalleIds(List<Lpn> lpns, DbConnection connection)
        {
            int count = 0;
            foreach (var l in lpns)
            {
                if (l.Detalles != null && l.Detalles.Count > 0)
                    count += l.Detalles.Count;
            }

            return _dapper.GetNextSequenceValues<int>(connection, Secuencias.S_LPN_DET, count).ToList();
        }

        public virtual void GenearCodigoDefault(Lpn lpn, ILpnServiceContext context, LpnBulkOperationContext bulkContext, short nuOrden)
        {
            int sumaCaracteres = 0;
            var tipoLpn = context.GetTipoLpn(lpn.Tipo);
            context.TiposLpn.Remove(lpn.Tipo);
            tipoLpn.NumeroSecuencia = tipoLpn.NumeroSecuencia + 1;
            context.TiposLpn.Add(lpn.Tipo, tipoLpn);
            var barra = tipoLpn.NumeroSecuencia.ToString().PadLeft(15, '0');

            foreach (var digito in barra)
            {
                sumaCaracteres += int.Parse(digito.ToString());
            }

            var mod = sumaCaracteres % DefaultDb.ModuloDigitoVerificacion;

            var codigoBarras = tipoLpn.Prefijo + barra + mod.ToString();

            var barras = new LpnBarras()
            {
                NumeroLpn = lpn.NumeroLPN,
                Tipo = BarcodeDb.TIPO_LPN_CB,
                Orden = nuOrden,
                CodigoBarras = codigoBarras
            };

            if (!bulkContext.NewLpnBarras.ContainsKey(barras.CodigoBarras))
                bulkContext.NewLpnBarras[barras.CodigoBarras] = GetLpnBarrasEntity(barras);
        }

        public virtual void GenerarCodigoIdExterno(Lpn lpn, LpnBulkOperationContext bulkContext, short nuOrden)
        {
            var newBarras = new LpnBarras()
            {
                NumeroLpn = lpn.NumeroLPN,
                CodigoBarras = lpn.IdExterno,
                Orden = nuOrden,
                Tipo = BarcodeDb.TIPO_LPN_CB
            };

            if (!bulkContext.NewLpnBarras.ContainsKey(newBarras.CodigoBarras))
                bulkContext.NewLpnBarras[newBarras.CodigoBarras] = GetLpnBarrasEntity(newBarras);
        }

        #endregion

        public virtual void ExpedirLpns(IEnumerable<long> nuLpns, string ubicacionPuerta, long nuTransaccion)
        {
            var lpns = nuLpns.Select(l => new Lpn()
            {
                NumeroLPN = l,
                Ubicacion = ubicacionPuerta,
                NumeroTransaccion = nuTransaccion,
                Estado = EstadosLPN.Finalizado,
                FechaModificacion = DateTime.Now,
                FechaFin = DateTime.Now
            });

            var detallesLpns = nuLpns.Select(l => new LpnDetalle()
            {
                NumeroLPN = l,
                NumeroTransaccion = nuTransaccion
            });

            string sql = @" UPDATE T_LPN 
                            SET CD_ENDERECO = :Ubicacion,
                                ID_ESTADO = :Estado,
                                DT_FIN = :FechaFin,
                                DT_UPDROW = :FechaModificacion,
                                NU_TRANSACCION = :NumeroTransaccion
                            WHERE NU_LPN = :NumeroLPN ";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, lpns, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            sql = @"UPDATE T_LPN_DET 
                    SET QT_EXPEDIDA = QT_ESTOQUE,
                        QT_ESTOQUE = 0,
                        NU_TRANSACCION = :NumeroTransaccion
                    WHERE NU_LPN = :NumeroLPN ";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, detallesLpns, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void AnulacionPedidoLpn(PedidoAnuladoLpnBulkOperationContext context)
        {
            string sql = @" UPDATE T_DET_PEDIDO_SAIDA_LPN 
                            SET QT_ANULADO = :CantidadAnulada,
                                DT_UPDROW = :FechaModificacion,
                                NU_TRANSACCION = :Transaccion
                            WHERE NU_PEDIDO = :Pedido 
                            AND CD_CLIENTE = :Cliente 
                            AND CD_EMPRESA = :Empresa
                            AND CD_PRODUTO = :Producto 
                            AND CD_FAIXA = :Faixa 
                            AND NU_IDENTIFICADOR =:Identificador 
                            AND ID_ESPECIFICA_IDENTIFICADOR = :IdEspecificaIdentificador
                            AND ID_LPN_EXTERNO = :IdLpnExterno
                            AND TP_LPN_TIPO = :Tipo";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, context.UpdateDetallePedidoLpn, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());


            sql = @" UPDATE T_DET_PEDIDO_SAIDA_LPN_ATRIB 
                            SET QT_ANULADO = :CantidadAnulada,
                                DT_UPDROW = :FechaModificacion,
                                NU_TRANSACCION = :Transaccion
                            WHERE NU_PEDIDO = :Pedido
                            AND CD_CLIENTE = :Cliente
                            AND CD_EMPRESA = :Empresa
                            AND CD_PRODUTO = :Producto
                            AND CD_FAIXA = :Faixa
                            AND NU_IDENTIFICADOR = :Identificador
                            AND ID_ESPECIFICA_IDENTIFICADOR = :IdEspecificaIdentificador
                            AND ID_LPN_EXTERNO = :IdLpnExterno
                            AND TP_LPN_TIPO = :Tipo
                            AND NU_DET_PED_SAI_ATRIB = :IdConfiguracion ";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, context.UpdateDetallePedidoLpnAtributo, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());


            sql = @" UPDATE T_DET_PEDIDO_SAIDA_ATRIB 
                            SET QT_ANULADO = :CantidadAnulada,
                                DT_UPDROW = :FechaModificacion,
                                NU_TRANSACCION = :Transaccion
                            WHERE NU_PEDIDO = :Pedido
                            AND CD_CLIENTE = :Cliente
                            AND CD_EMPRESA = :Empresa
                            AND CD_PRODUTO = :Producto
                            AND CD_FAIXA = :Faixa
                            AND NU_IDENTIFICADOR = :Identificador
                            AND ID_ESPECIFICA_IDENTIFICADOR = :IdEspecificaIdentificador
                            AND NU_DET_PED_SAI_ATRIB = :IdConfiguracion ";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, context.UpdateDetallePedidoAtributo, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());


            sql = @"INSERT INTO T_LOG_PEDIDO_ANULADO_LPN 
                    (NU_LOG_PEDIDO_ANULADO_LPN, 
                     NU_LOG_PEDIDO_ANULADO, 
                     TP_OPERACION,
                     ID_LPN_EXTERNO,
                     TP_LPN_TIPO,
                     NU_DET_PED_SAI_ATRIB,
                     QT_ANULADO,
                     DT_ADDROW) 
                    VALUES 
                    (:Id,
                     :IdLogPedidoAnulado,
                     :TipoOperacion,
                     :IdExternoLpn,
                     :TipoLpn,
                     :IdConfiguracion,
                     :CantidadAnulada,
                     :FechaInsercion)";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, context.NewLogPedidoAnuladoLpn, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void EliminarAtributoAsociados(string tipoLpn)
        {
            string sql = @" DELETE FROM T_LPN_TIPO_ATRIBUTO_DET WHERE TP_LPN_TIPO = :tipoLpn";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, param: new { tipoLpn = tipoLpn }, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            sql = @" DELETE FROM T_LPN_TIPO_ATRIBUTO WHERE TP_LPN_TIPO = :tipoLpn";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, param: new { tipoLpn = tipoLpn }, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual List<long> GetNewNroLpn(int count)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();
            return _dapper.GetNextSequenceValues<long>(connection, Secuencias.S_LPN, count, tran).ToList();
        }

        public virtual List<int> GetNewIdsDetalleLpn(int count)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();
            return _dapper.GetNextSequenceValues<int>(connection, Secuencias.S_LPN_DET, count, tran).ToList();
        }

        #endregion
    }
}
