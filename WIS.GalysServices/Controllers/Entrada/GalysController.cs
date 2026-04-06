using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;
using WIS.Automation.Galys;
using WIS.GalysServices.Models;

namespace WIS.GalysServices.Controllers.Entrada
{
    [ApiController]
    [Route("[controller]")]
    public class GalysController : ControllerBase
    {
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     dummy_galys 
        /// </summary>
        /// <remarks>producto</remarks>
        /// <param name="producto"></param>
        /// <returns>producto</returns>
        [HttpPost]
        [Route("Articulo")]
        public GalysResponse Articulo(ProductoRequest producto)
        {
            _logger.Debug($"Producto: {JsonConvert.SerializeObject(producto)}");
            return new GalysResponse() { descError = "", resultado = 0 };
        }

        /// <summary>
        ///     dummy_galys 
        /// </summary>
        /// <remarks>codigoBarras</remarks>
        /// <param name="codigoBarras"></param>
        /// <returns>codigoBarras</returns>
        [HttpPost]
        [Route("Articulo/Cdb")]
        public GalysResponse CodigoBarras(CodigoBarrasRequest codigoBarras)
        {
            _logger.Debug($"Codigo Barras: {JsonConvert.SerializeObject(codigoBarras)}");
            return new GalysResponse() { descError = "", resultado = 0 };
        }

        /// <summary>
        ///     dummy_galys 
        /// </summary>
        /// <remarks>codigoBarras</remarks>
        /// <param name="codigoBarras"></param>
        /// <returns>codigoBarras</returns>
        [HttpDelete]
        [Route("Articulo/Cdb/delete")]
        public GalysResponse CodigoBarrasDelete([FromQuery] string codAlmacen, [FromQuery] string codArticulo, [FromQuery] string codBarras)
        {
            _logger.Debug($"Codigo Barras delete, codAlmacen: {codAlmacen}, codArticulo: {codArticulo}, codBarras: {codBarras}");
            return new GalysResponse() { descError = "", resultado = 0 };
        }


        /// <summary>
        ///     dummy_galys 
        /// </summary>
        /// <remarks>ordenEntrada</remarks>
        /// <param name="ordenEntrada"></param>
        /// <returns>ordenEntrada</returns>
        [HttpPost]
        [Route("OrdenEntrada")]
        public GalysResponse OrdenEntrada(OrdenEntradaRequest ordenEntrada)
        {
            _logger.Debug($"Codigo Barras: {JsonConvert.SerializeObject(ordenEntrada)}");
            return new GalysResponse() { descError = "", resultado = 0 };
        }

        /// <summary>
        ///     dummy_galys 
        /// </summary>
        /// <remarks>OrdenSalida</remarks>
        /// <param name="OrdenSalida"></param>
        /// <returns>OrdenSalida</returns>
        [HttpPost]
        [Route("OrdenSalida")]
        public GalysResponse OrdenSalida(OrdenSalidaRequest ordenSalida)
        {
            _logger.Debug($"Codigo Barras: {JsonConvert.SerializeObject(ordenSalida)}");
            return new GalysResponse() { descError = "", resultado = 0 };
        }
    }
}
