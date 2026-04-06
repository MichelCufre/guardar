using System;

namespace WIS.Domain.StockEntities
{
    public class MapeoProducto
    {
        public int EmpresaOrigen { get; set; } // CD_EMPRESA

        public string ProductoOrigen { get; set; } // CD_PRODUTO

        public decimal FaixaOrigen { get; set; } // CD_FAIXA

        public int EmpresaDestino { get; set; } // CD_EMPRESA_DESTINO

        public string ProductoDestino { get; set; } // CD_PRODUTO_DESTINO

        public decimal FaixaDestino { get; set; } // CD_FAIXA_DESTINO

        public decimal CantidadOrigen { get; set; } // QT_ORIGEN

        public decimal CantidadDestino { get; set; } // QT_DESTINO

        public DateTime? FechaAlta { get; set; } // DT_ADDROW

        public DateTime? FechaModificacion { get; set; } // DT_UPDROW
    }
}
