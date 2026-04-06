using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WIS.Common.API.Attributes;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class AgenteResponse
    {
        /// <summary>
        /// CodigoAgente
        /// </summary>
        /// <example>PRO</example>
        [ApiDtoExample("PRO")]
        [StringLength(40)]
        public string CodigoAgente { get; set; }

        [StringLength(3)]
        public string Tipo { get; set; }

        [StringLength(10)]
        public string Categoria { get; set; }

        [StringLength(15)]
        public string CodigoPostal { get; set; }//CD_CEP

        [StringLength(30)]
        public string NumeroFiscal { get; set; } //CD_CGC_CLIENTE

        [StringLength(10)]
        public string CodigoCliente { get; set; }

        [StringLength(10)]
        public string ClienteConsolidado { get; set; }

        public int Empresa { get; set; }

        public int? EmpresaConsolidada { get; set; }

        public long? NumeroLocalizacionGlobal { get; set; } // CD_GLN

        [StringLength(20)]
        public string GrupoConsulta { get; set; }

        [StringLength(20)]
        public string PuntoDeEntrega { get; set; }

        public short Ruta { get; set; }

        public short Situacion { get; set; }

        [StringLength(200)]
        public string Anexo1 { get; set; }

        [StringLength(200)]
        public string Anexo2 { get; set; }

        [StringLength(200)]
        public string Anexo3 { get; set; }

        [StringLength(200)]
        public string Anexo4 { get; set; }

        [StringLength(50)]
        public string Barrio { get; set; }

        [StringLength(100)]
        public string Descripcion { get; set; }

        [StringLength(100)]
        public string Direccion { get; set; }

        public string FechaAlta { get; set; }
        public string FechaModificacion { get; set; }
        public string FechaSituacion { get; set; }
        public string AceptaDevolucion { get; set; }

        [StringLength(1)]
        public string IdClienteFilial { get; set; } //ID_CLIENTE_FILIAL

        public long? IdLocalidad { get; set; }

        [StringLength(20)]
        public string TipoFiscal { get; set; }

        [StringLength(15)]
        public string CaracteristicaTelefonica { get; set; } //NU_DDD

        [StringLength(30)]
        public string TelefonoSecundario { get; set; } //NU_FAX

        [StringLength(30)]
        public string OtroDatoFiscal { get; set; } // NU_INSCRICAO

        public short? OrdenDeCarga { get; set; }

        [StringLength(30)]
        public string TelefonoPrincipal { get; set; }

        [StringLength(1)]
        public string TipoActividad { get; set; }

        public decimal? ValorManejoVidaUtil { get; set; }

        [StringLength(1)]
        public string SincronizacionRealizada { get; set; }

        [StringLength(100)]
        public string Email { get; set; }
    }
}
