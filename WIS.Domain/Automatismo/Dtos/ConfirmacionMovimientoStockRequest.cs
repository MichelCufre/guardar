using System;
using System.Collections.Generic;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Validation;

namespace WIS.Domain.Automatismo.Dtos
{
    public class ConfirmacionMovimientoStockRequest : IApiAutomatismoRequest
    {
        /// <summary>
        /// Código de empresa de la ejecución
        /// </summary>
        /// <example>1</example>
        [ExisteEmpresaValidation()]
        public int Empresa { get; set; }

        /// <summary>
        /// Sirve para generar una referencia o un campo de búsqueda en el panel de ejecuciones de interfaces de WIS. Mediante este campo es posible identificar o buscar la traza de la ejecución de la interfaz. Ante un incidente o un procesamiento no esperado se puede reportar el problema haciendo referencia al valor de este campo.
        /// </summary>
        /// <example>Confirmación de entrada de stock</example>
        [RequiredValidation]
        [StringLengthValidation(200, MinimumLength = 1)]
        public string DsReferencia { get; set; }

        /// <summary>
        /// Generalmente se utiliza en sistemas externos que generan archivos y un middleware que oficia de intermediario entre el archivo y el API. 
        /// En este campo se puede guardar el nombre del archivo original en el cual vinieron los datos. En caso de que la implantación no utilice archivos se puede utilizar con otros fines.
        /// </summary>
        /// <example>Archivo</example>
        [StringLengthValidation(100)]
        public string Archivo { get; set; }

        public string Puesto { get; set; }

        public string Automatismo { get; set; }

        public string IdEntrada { get; set; }

        public UsuarioRequest Usuario { get; set; }

        public int TipoMovimiento { get; set; }

        public int EstadoEntrada { get; set; }

        /// <summary>
        /// Lista de detalles
        /// </summary>
        public List<ConfirmacionMovimientoStockLineaRequest> Detalles { get; set; }

        public ConfirmacionMovimientoStockRequest()
        {
            Detalles = new List<ConfirmacionMovimientoStockLineaRequest>();
        }

        public void Add(IApiAutomatismoItemRequest item)
        {
            Detalles.Add(item as ConfirmacionMovimientoStockLineaRequest);
        }

        public class ConfirmacionMovimientoStockLineaRequest : IApiAutomatismoItemRequest
        {
            public int IdLinea { get; set; }

            public string IdPeticion { get; set; }

            public string Producto { get; set; }

            public decimal Cantidad { get; set; }

            public string Identificador { get; set; }

            public DateTime? FechaVencimiento { get; set; }

            public string CodigoCausa { get; set; }
        }
    }
}
