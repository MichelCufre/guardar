using System;

namespace WIS.Domain.Recepcion
{
    public class AgendaEtiqueta
    {
        public string NumeroEtiqueta { get; set; }                  // NU_ETIQUETA
        public int? NumeroAgenda { get; set; }                      // NU_AGENDA
        public string CodigoEndereco { get; set; }                  // CD_ENDERECO
        public string CodigoEnderecoSugerido { get; set; }          // CD_ENDERECO_SUGERIDO
        public int? CodigoSituacao { get; set; }                    // CD_SITUACAO
        public int? CodigoFuncRecepcion { get; set; }               // CD_FUNC_RECEPCION
        public DateTime? FechaRecepcion { get; set; }               // DT_RECEPCION
        public int? CodigoFuncAlmacenamiento { get; set; }          // CD_FUNC_ALMACENAMIENTO
        public DateTime? FechaAlmacenamiento { get; set; }          // DT_ALMACENAMIENTO
        public string CodigoCliente { get; set; }                   // CD_CLIENTE
        public string CodigoGrupo { get; set; }                     // CD_GRUPO
        public int? CodigoPallet { get; set; }                      // CD_PALLET
        public string CodigoBarras { get; set; }                    // CD_BARRAS

    }
}
