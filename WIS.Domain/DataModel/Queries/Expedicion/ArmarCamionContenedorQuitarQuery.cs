using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Expedicion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Expedicion
{
    public class ArmarCamionContenedorQuitarQuery : QueryObject<V_EXP011_CONTENEDOR_CAMION, WISDB>
    {
        protected readonly int _camion;
        protected readonly int? _empresa;
        protected readonly short? _ruta;

        public ArmarCamionContenedorQuitarQuery(int camion, int? empresa = null, short? ruta = null)
        {
            this._camion = camion;
            this._empresa = empresa;
            this._ruta = ruta;
        }

        public override void BuildQuery(WISDB context)
        {
            var situacionesCamion = new List<short> 
            {
                SituacionDb.CamionAguardandoCarga,
                SituacionDb.CamionCargando
            };

            this.Query = context.V_EXP011_CONTENEDOR_CAMION.Where(d => d.CD_CAMION == this._camion && situacionesCamion.Contains(d.CD_SITUACAO_CAMION ?? -1) && d.ID_CARGAR == "S");

            if (this._empresa != null)
                this.Query = this.Query.Where(d => d.CD_EMPRESA == this._empresa);

            if (this._ruta != null)
                this.Query = this.Query.Where(d => d.CD_ROTA_CARGA == this._ruta);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<ContenedorAsociarUnidad> GetContenedoresUnidad()
        {
            return this.Query.Select(d => new ContenedorAsociarUnidad
            {
                Carga = d.NU_CARGA,
                Cliente = d.CD_CLIENTE,
                Contenedor = d.NU_CONTENEDOR,
                Empresa = d.CD_EMPRESA,
                GrupoExpedicion = d.CD_GRUPO_EXPEDICION,
                Preparacion = d.NU_PREPARACION
            }).ToList();
        }
    }
}
