using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.General.Enums;

namespace WIS.Domain.General
{
    public class Equipo
    {
        public int Id { get; set; }                         //CD_EQUIPO
        public string Descripcion { get; set; }             //DS_EQUIPO
        public string CodigoUbicacion { get; set; }         //CD_ENDERECO
        public int? CodigoFuncionario { get; set; }         //CD_FUNCIONARIO
        public short CodigoHerramienta { get; set; }        //CD_FERRAMENTA
        public string Aplicacion { get; set; }              //CD_APLICACAO
        public DateTime? FechaInsercion { get; set; }       //DT_ADDROW
        public DateTime? FechaModificacion { get; set; }    //DT_UPDROW

        public Ubicacion Ubicacion { get; set; }            //CD_ENDERECO
        public Herramienta Herramienta { get; set; }        //CD_FERRAMENTA

        public string PosicionEquipo { get; set; }
        public string CodigoUbicacionReal { get; set; }
        public string CodigoZona { get; set; }
        public string TipoOperacion { get; set; }
        public string NuComponente { get; set; }

        public virtual Equipo Copiar()
        {
            return new Equipo()
            {
                Id = this.Id,
                Descripcion = this.Descripcion,
                CodigoUbicacion = this.CodigoUbicacion,
                CodigoFuncionario = this.CodigoFuncionario,
                CodigoHerramienta = this.CodigoHerramienta,
                Aplicacion = this.Aplicacion,
                FechaInsercion = this.FechaInsercion,
                FechaModificacion = this.FechaModificacion,

                Ubicacion = this.Ubicacion,
                Herramienta = this.Herramienta,
                PosicionEquipo = this.PosicionEquipo,

                CodigoUbicacionReal = this.CodigoUbicacionReal,
                CodigoZona = this.CodigoZona,
                TipoOperacion = this.TipoOperacion,
            };
        }
    }
}
