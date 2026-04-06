using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class EmpresaResponse
    {
        public int Codigo { get; set; } //CD_EMPRESA

        [StringLength(55)]
        public string Nombre { get; set; } //NM_EMPRESA
        public short Estado { get; set; } //CD_SITUACAO
        public string FechaInsercion { get; set; } //DT_ADDROW
        public string FechaModificacion { get; set; } //DT_UPDROW

        [StringLength(100)]
        public string Direccion { get; set; } //DS_ENDERECO

        [StringLength(30)]
        public string Telefono { get; set; } //NU_TELEFONE

        [StringLength(30)]
        public string NumeroFiscal { get; set; } //CD_CGC_EMPRESA

        [StringLength(20)]
        public string TipoFiscal { get; set; } //ND_TIPO_FISCAL

        [StringLength(15)]
        public string CodigoPostal { get; set; } //DS_CP_POSTAL

        [StringLength(10)]
        public string ClienteArmadoKit { get; set; } //CD_CLIENTE_ARMADO_KIT

        [StringLength(200)]
        public string Anexo1 { get; set; } //DS_ANEXO1

        [StringLength(200)]
        public string Anexo2 { get; set; } //DS_ANEXO2

        [StringLength(200)]
        public string Anexo3 { get; set; } //DS_ANEXO3

        [StringLength(200)]
        public string Anexo4 { get; set; } //DS_ANEXO4

        public long? IdLocalidad { get; set; } //ID_LOCALIDAD
        public short? TipoDeAlmacenajeYSeguro { get; set; } //TP_ALMACENAJE_Y_SEGURO
        public decimal? ValorMinimoStock { get; set; } //IM_MINIMO_STOCK

        public int? EmpresaConsolidado { get; set; } //CD_EMPRESA_DE_CONSOLIDADO
        public int? ProveedorDevolucion { get; set; } //CD_FORN_DEVOLUCAO
        public int? ListaPrecio { get; set; } //CD_LISTA_PRECIO

        [StringLength(1)]
        public string IdDAP { get; set; } //ID_DAP

        [StringLength(1)]
        public string IdOperativo { get; set; } //ID_OPERATIVO

        [StringLength(1)]
        public string IdUnidadFactura { get; set; } //ID_UND_FACT_EMPRESA
        public short? CantidadDiasPeriodo { get; set; } //QT_DIAS_POR_PERIODO
        public decimal? ValorPallet { get; set; } //VL_POS_PALETE
        public decimal? ValorPalletDia { get; set; } //VL_POS_PALETE_DIA
    }
}
