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
    public class GetReferenciasDisponiblesQuery : QueryObject<V_REC170_REFERENCIAS_DISPONIB, WISDB>
    {

        protected readonly int _idEmpresa;
        protected readonly string _tipoReferencia;
        protected readonly string _codigoInternoCliente;
        protected readonly string _predio;
        protected readonly ReferenciaRecepcionMapper _mapper;
        protected readonly RecepcionTipoMapper _recepcionTipoMapper;

        public GetReferenciasDisponiblesQuery(int idEmpresa, string tipoReferencia, string codigoInternoCliente, string predio)
        {
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


        }

        public virtual List<ReferenciaRecepcion> GetByMemoOrNumeroPartial(string searchValue)
        {
            var referencias = new List<ReferenciaRecepcion>();

            var entries = this.Query
                .Where(d => d.DS_MEMO.ToLower().Contains(searchValue.ToLower()) || d.NU_REFERENCIA.ToLower().Contains(searchValue.ToLower()))
                .ToList();

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

        public virtual bool AnyReferenciaDisponible(int idReferencia)
        {
            return this.Query.Any(d => d.NU_RECEPCION_REFERENCIA == idReferencia);
        }

        

    }
}
