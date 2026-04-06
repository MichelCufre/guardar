using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Inventario;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Inventario
{
    public class INV414GridQuery : QueryObject<V_INV412_DET_CONTEO, WISDB>
    {
        protected decimal? _nuInventario;

        public INV414GridQuery(decimal? nuInventario)
        {
            this._nuInventario = nuInventario;
        }

        public override void BuildQuery(WISDB context)
        {
            var estadosAdmitidos = new string[]
            {
                EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_FINALIZADO_REC,
                EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_ACTUALIZADO,
                EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_FINALIZADO_DIF,
                EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_RECONTAR
            };

            this.Query = context.V_INV412_DET_CONTEO.AsNoTracking()
                .Where(i => (_nuInventario.HasValue ? i.NU_INVENTARIO == _nuInventario : true)
                    && estadosAdmitidos.Contains(i.ND_ESTADO_INV_ENDERECO_DET)
                    && i.ND_ESTADO_INVENTARIO != EstadoInventario.Cancelado);
        }

        public virtual List<InventarioUbicacionDetalle> GetSelectedKeys(List<decimal> keysToSelect, bool rechazarConteo)
        {
            var estados = new List<string>()
            {
                EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_FINALIZADO_DIF
            };

            if (rechazarConteo)
                estados.Add(EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_RECONTAR);

            return this.Query
                .Where(d => estados.Contains(d.ND_ESTADO_INV_ENDERECO_DET))
                .OrderBy(s => s.QT_DIFERENCIA)
                .Select(i => i.NU_INVENTARIO_ENDERECO_DET)
                .AsEnumerable()
                .Intersect(keysToSelect)
                .Select(i => new InventarioUbicacionDetalle() { Id = i })
                .ToList();
        }

        public virtual List<InventarioUbicacionDetalle> GetSelectedKeysAndExclude(List<decimal> keysToExclude, bool rechazarConteo)
        {
            var noAdmitidosRechazo = new List<string>()
            {
                EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_FINALIZADO_REC,
                EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_ACTUALIZADO,
            };

            var keyExcluir = this.Query
            .Where(l => (rechazarConteo ? noAdmitidosRechazo.Contains(l.ND_ESTADO_INV_ENDERECO_DET) :
                   (l.ND_ESTADO_INV_ENDERECO_DET != EstadoInventarioUbicacionDetalleDb.ND_ESTADO_ENDERECO_DET_FINALIZADO_DIF)))
            .Select(r => r.NU_INVENTARIO_ENDERECO_DET)
            .ToList();

            keysToExclude.AddRange(keyExcluir);
            keysToExclude = keysToExclude.Distinct().ToList();

            return this.Query
                .OrderBy(s => s.QT_DIFERENCIA)
                .Select(r => r.NU_INVENTARIO_ENDERECO_DET)
                .AsEnumerable()
                .Except(keysToExclude)
                .Select(i => new InventarioUbicacionDetalle() { Id = i })
                .ToList();
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
