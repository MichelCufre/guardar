using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Common.API.Attributes;
using WIS.Domain.Validation;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class ProductosRequest : IApiEntradaRequest
    {
        /// <summary>
        /// Código de empresa de la ejecución
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RequiredValidation]
        [ExisteEmpresaValidation()]
        public int Empresa { get; set; }

        /// <summary>
        /// Sirve para generar una referencia o un campo de búsqueda en el panel de ejecuciones de interfaces de WIS. Mediante este campo es posible identificar o buscar la traza de la ejecución de la interfaz. Ante un incidente o un procesamiento no esperado se puede reportar el problema haciendo referencia al valor de este campo.
        /// </summary>
        /// <example>Creación de productos</example>
        [ApiDtoExample("Creación de productos")]
        [RequiredValidation]
        [StringLengthValidation(200, MinimumLength = 1)]
        public string DsReferencia { get; set; }

        /// <summary>
        /// Generalmente se utiliza en sistemas externos que generan archivos y un middleware que oficia de intermediario entre el archivo y el API. 
        /// En este campo se puede guardar el nombre del archivo original en el cual vinieron los datos. En caso de que la implantación no utilice archivos se puede utilizar con otros fines.
        /// </summary>
        /// <example>Archivo</example>
        [ApiDtoExample("Archivo")]
        [StringLengthValidation(100)]
        public string Archivo { get; set; }

		/// <summary>
		/// Sirve para controlar la unicidad de las ejecuciones
		/// </summary>
		/// <example>123</example>
		[ApiDtoExample("123")]
		[StringLengthValidation(50, MinimumLength = 0)]
		public string IdRequest { get; set; }

		/// <summary>
		/// Lista de productos
		/// </summary>
		[RequiredListValidation]
        public List<ProductoRequest> Productos { get; set; }

        public ProductosRequest()
        {
            Productos = new List<ProductoRequest>();
        }

        public void Add(IApiEntradaItemRequest item)
        {
            Productos.Add(item as ProductoRequest);
        }
    }

    public class ProductoRequest : IApiEntradaItemRequest
    {
        /// <summary>
        /// Código de producto.
        /// Solo puede estar compuesto por los siguientes caracteres: 01234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ-_/.* 
        /// </summary>
        /// <example>PR1</example>
        [ApiDtoExample("PR1")]
        [RequiredValidation]
        [StringLengthValidation(40, MinimumLength = 1)]
        public string Codigo { get; set; }

        /// <summary>
        /// Descripción del producto
        /// </summary>
        /// <example>Producto de ejemplo</example>
        [ApiDtoExample("Producto de ejemplo")]
        [RequiredValidation]
        [StringLengthValidation(65, MinimumLength = 1)]
        public string Descripcion { get; set; }

        /// <summary>
        /// Código de clasificación adicional. 
        /// No es un código único es un código de clasificación. Puede ser compartido por varios productos.
        /// </summary>
        /// <example>PR1</example>
        [ApiDtoExample("PR1")]
        [StringLengthValidation(40, MinimumLength = 0)]
        public string CodigoMercadologico { get; set; }

        /// <summary>
        /// Tipo de manejo de fecha para los productos. 
        /// Este campo define si un producto es duradero, FIFO o FEFO.
        /// Duradero: no necesita llevar fechas en el sistema.
        /// FEFO: Lo que se solicita al ingreso es la fecha de vencimiento de un artículo o partida.
        /// FIFO: El sistema le asigna al producto una fecha de entrada y va sacando el más viejo (no existe un vencimiento en este caso).
        /// F (FIFO) - D (Duradero) - E (Expirable)
        /// </summary>
        /// <example>D</example>
        [ApiDtoExample("D")]
        [StringLengthValidation(1, MinimumLength = 0)]
        public string TipoManejoFecha { get; set; }

        /// <summary>
        /// Código de producto único adicional.
        /// Si no se usa se le asigna el mismo código de artículo.
        /// </summary>
        /// <example>PR1</example>
        [ApiDtoExample("PR1")]
        [StringLengthValidation(40, MinimumLength = 0)]
        public string CodigoProductoEmpresa { get; set; }


        /// <summary>
        /// Situación del producto. 
        /// 15 (Activo) - 16 (Inactivo)
        /// </summary>
        /// <example>15</example>
        [ApiDtoExample("15")]
        [RangeValidation(3)]
        public short? Situacion { get; set; }

        /// <summary>
        /// Especifica como se maneja el producto.
        /// En el caso de manejar PRODUCTO no requiere un dato adicional, es la modalidad de operación más simple.
        /// En el caso de manejar LOTE o SERIE , con solo identificar el producto no es suficiente, es necesario especificar o indicar el LOTE o SERIE.
        /// P (Producto) - L (Lote) - S (Serie)
        /// </summary>
        /// <example>P</example>
        [ApiDtoExample("P")]
        [StringLengthValidation(1, MinimumLength = 0)]
        public string ManejoIdentificador { get; set; }

        /// <summary>
        /// Acepta decimales
        /// S (Sí) - N (No)
        /// </summary>
        /// <example>S</example> 
        [ApiDtoExample("S")]
        [StringLengthValidation(1, MinimumLength = 0)]
        public string AceptaDecimales { get; set; }

        /// <summary>
        /// Descripción reducida del producto
        /// </summary>
        /// <example>Producto de ejemplo</example>
        [ApiDtoExample("Producto de ejemplo")]
        [StringLengthValidation(20, MinimumLength = 0)]
        public string DescripcionReducida { get; set; }

        /// <summary>
        /// Código NAM (Nomenclatura Aduanera del Mercosur)
        /// </summary>
        /// <example>NE</example>
        [ApiDtoExample("NE")]
        [StringLengthValidation(20, MinimumLength = 0)]
        public string NAM { get; set; }

        /// <summary>
        /// Unidad de medida
        /// </summary>
        /// <example>UND</example>
        [ApiDtoExample("UND")]
        [StringLengthValidation(10, MinimumLength = 0)]
        public string UnidadMedida { get; set; }

        /// <summary>
        /// Días o ventana para liberar un producto antes de su vencimiento
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(4, 0, false)]
        public short? DiasLiberacion { get; set; }

        /// <summary>
        /// Días para ventana de Recepción.
        /// Solo aplica a productos FEFO.
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(4, 0, false)]
        public short? DiasDuracion { get; set; }

        /// <summary>
        /// Días para ventana de Vencimiento.
        /// Solo aplica a productos FEFO
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(4, 0, false)]
        public short? DiasValidez { get; set; }

        /// <summary>
        /// Modalidad de ingreso de lote
        /// PMIDNOR (Normal) - PMIDVEN (Vencimiento) - PMIDAGE (Agenda) - PMIDDOC (Documento) - PMIDIDVE (Vencimiento)
        /// </summary>
        /// <example>PMIDNOR</example>
        [ApiDtoExample("PMIDNOR")]
        [StringLengthValidation(10, MinimumLength = 0)]
        public string ModalidadIngresoLote { get; set; }

        /// <summary>
        /// Cantidad de stock mínimo a tener para el artículo
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(9, 0, false)]
        public int? StockMinimo { get; set; }

        /// <summary>
        /// Cantidad de stock máximo a tener para el artículo
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(9, 0, false)]
        public int? StockMaximo { get; set; }

        /// <summary>
        /// Peso bruto del artículo
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(12, 3, false)]
        public decimal? PesoBruto { get; set; }

        /// <summary>
        /// Peso líquido del producto
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(12, 3, false)]
        public decimal? PesoNeto { get; set; }

        /// <summary>
        /// Altura
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(10, 4, false)]
        public decimal? Altura { get; set; }

        /// <summary>
        /// Ancho
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(10, 4, false)]
        public decimal? Ancho { get; set; }

        /// <summary>
        /// Profundidad
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(10, 4, false)]
        public decimal? Profundidad { get; set; }

        /// <summary>
        /// Volumen del producto expresado en CM3
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(14, 4, false)]
        public decimal? VolumenCC { get; set; }

        /// <summary>
        /// Cantidad de unidades por embalaje. Cantidad de unidades por caja.
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(9, 3)]
        public decimal? UnidadBulto { get; set; }

        /// <summary>
        /// Unidad de distribución
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(9, 3)]
        public decimal? UnidadDistribucion { get; set; }

        /// <summary>
        /// Cantidad minima para distribuir
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(14, 4, false)]
        public decimal? AvisoAjusteInventario { get; set; }

        /// <summary>
        /// Tipo de display
        /// </summary>
        /// <example>D</example>
        [ApiDtoExample("D")]
        [StringLengthValidation(1, MinimumLength = 0)]
        public string TipoDisplay { get; set; }

        /// <summary>
        /// Descripción de display
        /// </summary>
        /// <example>Descripción de ejemplo</example>
        [ApiDtoExample("Descripción de ejemplo")]
        [StringLengthValidation(100, MinimumLength = 0)]
        public string DescripcionDisplay { get; set; }

        /// <summary>
        /// Sub Bulto
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(3)]
        public short? SubBulto { get; set; }

        /// <summary>
        /// Último costo
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(16, 2, false)]
        public decimal? UltimoCosto { get; set; }

        /// <summary>
        /// Precio venta
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(16, 2, false)]
        public decimal? PrecioVenta { get; set; }

        /// <summary>
        /// Ayuda colector
        /// </summary>
        /// <example>Ayuda de ejemplo</example>
        [ApiDtoExample("Ayuda de ejemplo")]
        [StringLengthValidation(200, MinimumLength = 0)]
        public string AyudaColector { get; set; }

        /// <summary>
        /// Componente 1
        /// </summary>
        /// <example>Componente 1</example>
        [ApiDtoExample("Componente 1")]
        [StringLengthValidation(20, MinimumLength = 0)]
        public string Componente1 { get; set; }

        /// <summary>
        /// Componente 2
        /// </summary>
        /// <example>Componente 2</example>
        [ApiDtoExample("Componente 2")]
        [StringLengthValidation(20, MinimumLength = 0)]
        public string Componente2 { get; set; }

        /// <summary>
        /// Información anexa
        /// </summary>
        /// <example></example>
        [StringLengthValidation(200, MinimumLength = 0)]
        public string Anexo1 { get; set; }

        /// <summary>
        /// Información anexa
        /// </summary>
        /// <example></example>
        [StringLengthValidation(200, MinimumLength = 0)]
        public string Anexo2 { get; set; }

        /// <summary>
        /// Información anexa
        /// </summary>
        /// <example></example>
        [StringLengthValidation(200, MinimumLength = 0)]
        public string Anexo3 { get; set; }

        /// <summary>
        /// Información anexa
        /// </summary>
        /// <example></example>
        [StringLengthValidation(200, MinimumLength = 0)]
        public string Anexo4 { get; set; }

        /// <summary>
        /// Información anexa
        /// </summary>
        /// <example></example>
        [StringLengthValidation(18, MinimumLength = 0)]
        public string Anexo5 { get; set; }

        /// <summary>
        /// Código del sector o clase. Es un dato de configuración en WIS.
        /// Se usa para sectorizar mercadería (Congelados, Tóxicos, Explosivos, Alimentos, etc.)
        /// </summary>
        /// <example>GR</example>
        [ApiDtoExample("GR")]
        [StringLengthValidation(2, MinimumLength = 0)]
        public string CodigoClase { get; set; }

        /// <summary>
        /// Código de la familia del producto. 
        /// La familia es una clasificación que engloba varios productos.
        /// Puede ser creada automáticamente con la interfaz de producto.
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(10, 0, false)]
        public int? CodigoFamilia { get; set; }

        /// <summary>
        /// Código o valor de rotatividad del producto.
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(2, 0, false)]
        public short? CodigoRotatividad { get; set; }

        /// <summary>
        /// Código de agrupador de producto a los efectos de filtrar accesos externos o asignar permisos a determinado grupo de productos.
        /// </summary>
        /// <example>S/N</example>
        [ApiDtoExample("S/N")]
        [StringLengthValidation(20, MinimumLength = 0)]
        public string GrupoConsulta { get; set; }

        /// <summary>
        /// Exclusivo
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(4)]
        public short? Exclusivo { get; set; }

        /// <summary>
        /// Tipo de peso
        ///  1 - 2 - 3
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(1)]
        public short? TipoPeso { get; set; }

        /// <summary>
        /// Nivel
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [StringLengthValidation(11, MinimumLength = 0)]
        public string Nivel { get; set; }

        /// <summary>
        /// Código de origen
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [StringLengthValidation(1, MinimumLength = 0)]
        public string CodigoOrigen { get; set; }

        /// <summary>
        /// Producto único
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [StringLengthValidation(2, MinimumLength = 0)]
        public string ProductoUnico { get; set; }

        /// <summary>
        /// Código del ramo del producto. 
        /// El ramo es una clasificación que engloba varios productos.
        /// Puede ser creado automáticamente con la interfaz de producto.
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(2, 0, false)]
        public short? Ramo { get; set; }

        /// <summary>
        /// Unidad medida facturación.
        /// </summary>
        /// <example>UND</example>
        [ApiDtoExample("UND")]
        [StringLengthValidation(10, MinimumLength = 0)]
        public string UndMedidaFact { get; set; }

        /// <summary>
        /// Unidad embalaje.
        /// </summary>
        /// <example>UND</example>
        [ApiDtoExample("UND")]
        [StringLengthValidation(10, MinimumLength = 0)]
        public string UndEmb { get; set; }

        /// <summary>
        /// Descripción diferencia peso
        /// </summary>
        /// <example>X</example>
        [ApiDtoExample("X")]
        [StringLengthValidation(4, MinimumLength = 0)]
        public string DescDifPeso { get; set; }

        /// <summary>
        /// Conversión
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(12, 3)]
        public decimal? Conversion { get; set; }

        /// <summary>
        /// Valor por defecto para la agrupación de las líneas de pedidos en los cuales se solicita este producto.
        /// P (Pedido) - O (Onda) - R (Ruta) - C (Cliente)
        /// </summary>
        /// <example>P</example>
        [ApiDtoExample("P")]
        [StringLengthValidation(1, MinimumLength = 0)]
        public string Agrupacion { get; set; }

        /// <summary>
        /// Cantidad genérica
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(9, 3)]
        public decimal? CantidadGenerica { get; set; }

        /// <summary>
        /// Cantidad padrón de stock
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(9)]
        public int? CantidadPadronStock { get; set; }

        /// <summary>
        /// Sg. Producto
        /// </summary>
        /// <example>X</example>
        [ApiDtoExample("X")]
        [StringLengthValidation(13, MinimumLength = 0)]
        public string SgProducto { get; set; }

        /// <summary>
        /// Precio distribución
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(10, 4)]
        public decimal? PrecioDistribucion { get; set; }

        /// <summary>
        /// Precio egreso
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(10, 4)]
        public decimal? PrecioEgreso { get; set; }

        /// <summary>
        /// Precio ingreso
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(10, 4)]
        public decimal? PrecioIngreso { get; set; }

        /// <summary>
        /// Precio seg. Distribución
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(10, 4)]
        public decimal? PrecioSegDistribucion { get; set; }

        /// <summary>
        /// Precio seg. Stock
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(10, 4)]
        public decimal? PrecioSegStock { get; set; }

        /// <summary>
        /// Precio stock
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [RangeValidation(10, 4, false)]
        public decimal? PrecioStock { get; set; }

        /// <summary>
        /// Código base del producto
        /// </summary>
        /// <example>PR1</example>
        [ApiDtoExample("PR1")]
        [StringLengthValidation(40, MinimumLength = 0)]
        public string CodigoBase { get; set; }

        /// <summary>
        /// Talle del producto
        /// </summary>
        /// <example>1</example>
        [ApiDtoExample("1")]
        [StringLengthValidation(40, MinimumLength = 0)]
        public string Talle { get; set; }

        /// <summary>
        /// Color del producto
        /// </summary>
        /// <example>Rojo</example>
        [StringLengthValidation(40, MinimumLength = 0)]
        public string Color { get; set; }

        /// <summary>
        /// Temporada
        /// </summary>
        /// <example>Verano</example>
        [ApiDtoExample("Verano")]
        [StringLengthValidation(40, MinimumLength = 0)]
        public string Temporada { get; set; }

        /// <summary>
        /// Categoría del Producto
        /// </summary>
        /// <example></example>
        [ApiDtoExample("1")]
        [StringLengthValidation(40, MinimumLength = 0)]
        public string Categoria1 { get; set; }

        /// <summary>
        /// Categoría del Producto
        /// </summary>
        /// <example></example>
        [ApiDtoExample("2")]
        [StringLengthValidation(40, MinimumLength = 0)]
        public string Categoria2 { get; set; }

        /// <summary>
        /// Categoría del Producto
        /// </summary>
        /// <example></example>
        [ApiDtoExample("POR_DEFECTO")]
        [StringLengthValidation(20, MinimumLength = 0)]
        public string VentanaLiberacion { get; set; }

    }
}
