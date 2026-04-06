using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Reportes.Dtos
{
    public class DtoReporteConfRecepcionDetalle
    {
        public int Id { get; set; }

        public string Producto { get; set; }

        public string Lote { get; set; }

        public decimal Faixa { get; set; }

        public int Empresa { get; set; }

        public short? Situacion { get; set; }

        public decimal? CantidadAgendada { get; set; }

        public decimal? CantidadCrossDocking { get; set; }

        public DateTime? Vencimiento { get; set; }

        public decimal? Precio { get; set; }

        public decimal? CantidadRecibida { get; set; }

        public DateTime? FechaRecepcion { get; set; }

        public int? FuncionarioAceptoRecepcion { get; set; }

        public DateTime? FechaAlta { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public decimal? CantidadAceptada { get; set; }

        public decimal? CantidadOriginal { get; set; }

        public decimal? CantidadFicticia { get; set; }

        public decimal? CIF { get; set; }

        public string ProductoEmpresa { get; set; }

        public string NAM { get; set; }

        public string Mercadologico { get; set; }

        public string SgProducto { get; set; }

        public short? TipoPesoProducto { get; set; }

        public string DescripcionDiferPesoQtDe { get; set; }

        public string DescripcionProducto { get; set; }

        public string UnidadMedida { get; set; }

        public int? Famila { get; set; }

        public short? Rotividad { get; set; }

        public string Clase { get; set; }

        public int? CantidadMinima { get; set; }

        public int? CantidadMaxima { get; set; }

        public decimal? PesoLiquido { get; set; }

        public decimal? PesoBruto { get; set; }

        public decimal? FtConversac { get; set; }

        public decimal? Volumen { get; set; }

        public decimal? PrecioVenta { get; set; }

        public decimal? ValorCostoUltimaEntrada { get; set; }

        public string Origen { get; set; }

        public string DescripcionReducida { get; set; }


        public string Nivel { get; set; }

        public string EnidadEmbalaje { get; set; }

        public short SituacionProducto { get; set; }

        public DateTime? FechaSituacion { get; set; }

        public short? DiasValidos { get; set; }

        public short? DiasDuracion { get; set; }


        public string IdCrossDocking { get; set; }


        public string IdRedondeoValidez { get; set; }

        public string Agrupacion { get; set; }


        public string ManejaIdentificador { get; set; }


        public string Display { get; set; }


        public string Anexo1 { get; set; }


        public string Anexo2 { get; set; }


        public string Anexo3 { get; set; }


        public string Anexo4 { get; set; }

        public decimal? Altura { get; set; }

        public decimal? Largo { get; set; }

        public decimal? Profundidad { get; set; }


        public string ManejoFecha { get; set; }

        public decimal? AvisoAjuste { get; set; }


        public string DsHelpColector { get; set; }

        public short? SubBulto { get; set; }

        public short? Exclusivo { get; set; }

        public decimal UnidadDistribucion { get; set; }

        public short? CantidadDiasValidosLiberacion { get; set; }

        public decimal CantidadBulto { get; set; }


        public string ManejoTomaDato { get; set; }


        public string Anexo5 { get; set; }


        public string GrupoConsulta { get; set; }


        public string DescripcionDisplay { get; set; }

        public decimal? PrecioSegDist { get; set; }

        public decimal? PrecioSegStock { get; set; }

        public decimal? PrecioDistibucion { get; set; }

        public decimal? PrecioEgreso { get; set; }

        public decimal? PrecioIngreso { get; set; }

        public decimal? PrecioStock { get; set; }


        public string UnidadMedodaFac { get; set; }


        public string ProductoUnico { get; set; }

        public short? Ramo { get; set; }


        public string DescripcionFamilia { get; set; }


        public string DescripcionUnidadMedida { get; set; }


        public string DescripcionRotatividad { get; set; }


        public string DescripcionClase { get; set; }
    }
}
