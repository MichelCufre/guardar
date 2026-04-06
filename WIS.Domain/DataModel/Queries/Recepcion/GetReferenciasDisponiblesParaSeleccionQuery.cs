using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Recepcion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
    public class GetReferenciasDisponiblesParaSeleccionQuery : QueryObject<V_REC170_REFERENCIAS_DISPONIB, WISDB>
    {

        protected readonly int? _idAgenda;
        protected readonly int _idEmpresa;
        protected readonly string _tipoReferencia;
        protected readonly string _codigoInternoCliente;
        protected readonly string _predio;
        protected readonly ReferenciaRecepcionMapper _mapper;
        protected readonly RecepcionTipoMapper _recepcionTipoMapper;

        public GetReferenciasDisponiblesParaSeleccionQuery(int idEmpresa, string tipoReferencia, string codigoInternoCliente, string predio, int? idAgenda)
        {

            this._idAgenda = idAgenda;
            this._idEmpresa = idEmpresa;
            this._tipoReferencia = tipoReferencia;
            this._codigoInternoCliente = codigoInternoCliente;
            this._predio = predio;
            this._mapper = new ReferenciaRecepcionMapper();
            this._recepcionTipoMapper = new RecepcionTipoMapper();
        }

        public override void BuildQuery(WISDB context)
        {
            DateTime fecha = DateTime.Now.Date;

            this.Query = context.V_REC170_REFERENCIAS_DISPONIB.AsNoTracking()
                .Where(s => s.CD_EMPRESA == _idEmpresa
                          && s.TP_REFERENCIA == _tipoReferencia
                          && s.ND_ESTADO_REFERENCIA == EstadoReferenciaRecepcionDb.Abierta
                          && (_codigoInternoCliente == null || s.CD_CLIENTE == _codigoInternoCliente)
                          && (s.DT_VENCIMIENTO_ORDEN >= fecha || s.DT_VENCIMIENTO_ORDEN == null));

            if (!string.IsNullOrEmpty(_predio) && (!_predio.Equals(GeneralDb.PredioSinDefinir) || !_predio.Equals(GeneralDb.PredioSinPredio)))
            {
                this.Query = this.Query.Where(s => (s.NU_PREDIO == _predio || s.NU_PREDIO == null));
            }

            if (_idAgenda != null)
            {
                var referenciasAsociadas = context.T_RECEPC_AGENDA_REFERENCIA_REL.Where(s => s.NU_AGENDA == _idAgenda).Select(s => s.NU_RECEPCION_REFERENCIA).ToList();

                this.Query = this.Query.Where(s => !referenciasAsociadas.Contains(s.NU_RECEPCION_REFERENCIA));
            }

        }

        public virtual List<ReferenciaRecepcion> GetReferenciasDisponibles()
        {
            var referencias = new List<ReferenciaRecepcion>();

            var entries = this.Query.ToList();

            foreach (var entry in entries)
            {
                referencias.Add(new ReferenciaRecepcion()
                {
                    Id = entry.NU_RECEPCION_REFERENCIA,
                    Numero = entry.NU_REFERENCIA,
                    Memo = entry.DS_MEMO
                });
            }

            return referencias;

        }

    }
}
