using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Liberacion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Registro
{
    public class RutaOndaQuery : QueryObject<V_REG220_RUTA_ONDA, WISDB>
    {
        protected readonly int _idUsuario;

        public RutaOndaQuery(int idUsuario)
        {
            this._idUsuario = idUsuario;
        }

        public override void BuildQuery(WISDB context)
        {

            var predios = context.V_PREDIO_USUARIO.Where(s => s.USERID == _idUsuario).Select(s => s.NU_PREDIO).ToList();

            this.Query = context.V_REG220_RUTA_ONDA.Where(d => (d.NU_PREDIO == null || predios.Contains(d.NU_PREDIO))).Select(d => d);

        }

        /// <summary>
        /// Toma en cuenta los predios asignados al usuario para filtrar las ondas/rutas
        /// </summary>
        /// <returns></returns>
        public virtual List<Ruta> GetRutasDisponibles()
        {

            var entities = this.Query.ToList();

            List<Ruta> rutasDisponibles = new List<Ruta>();

            foreach (var entity in entities)
            {
                var ruta = new Ruta()
                {
                    Id = entity.CD_ROTA,
                    Descripcion = entity.DS_ROTA,
                    Zona = entity.CD_ZONA
                };
                if (entity.CD_ONDA != null)
                {
                    var onda = new Onda()
                    {
                        Id = (short)entity.CD_ONDA,
                        Descripcion = entity.DS_ONDA,
                        Predio = entity.NU_PREDIO
                    };
                    ruta.Onda = onda;
                }

                rutasDisponibles.Add(ruta);
            }

            return rutasDisponibles;
        }

        public virtual List<Ruta> GetRutasDisponibles(string predio)
        {
            List<Ruta> rutasDisponibles = this.GetRutasDisponibles();

            return rutasDisponibles.Where(s => s.Onda==null || s.Onda.Predio == predio || s.Onda.Predio == null || s.Onda.Predio == GeneralDb.PredioSinDefinir).ToList();
        }

    }
}
