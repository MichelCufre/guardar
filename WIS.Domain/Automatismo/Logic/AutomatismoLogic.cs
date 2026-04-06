using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.Automatismo.Constants;
using WIS.Domain.Automatismo.Enums;
using WIS.Domain.Automatismo.Interfaces;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Recorridos;
using WIS.Domain.Services.Interfaces;
using WIS.GridComponent.Build;
using WIS.GridComponent.Excel;
using WIS.Security;

namespace WIS.Domain.Automatismo.Logic
{
    /// <summary>
    /// Esta clase concentra la lógica relacionada con las reglas del negocio correspondientes a los automatismos
    /// </summary>
    public class AutomatismoLogic
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IAutomatismoFactory _automatismoFactory;

        protected const byte NUMBER_OF_DIGITS_TO_PAD = 2;

        public AutomatismoLogic(IUnitOfWork uow, IAutomatismoFactory automatismoFactory)
        {
            this._uow = uow;
            this._automatismoFactory = automatismoFactory;
        }

        public virtual IAutomatismo GetAutomatismo(string codigoExterno, string descripcion, string tipo, string predio, bool isEnabled)
        {
            var idAutomatismo = _uow.AutomatismoRepository.GetNextNumeroAutomatismo();
            var automatismo = _automatismoFactory.Create(tipo);

            automatismo.Numero = idAutomatismo;
            automatismo.Codigo = string.IsNullOrEmpty(codigoExterno) ? idAutomatismo.ToString() : codigoExterno;
            automatismo.Descripcion = descripcion;
            automatismo.Tipo = tipo;
            automatismo.Predio = predio;
            automatismo.ZonaUbicacion = $"{AutomatismoDb.PREFIJO_AUT}_{idAutomatismo}";
            automatismo.FechaRegistro = DateTime.Now;
            automatismo.IsEnabled = isEnabled;

            return automatismo;
        }

        public virtual void CrearAutomatismoConUbicaciones(IAutomatismo automatismo)
        {
            CrearZonaUbicacion(automatismo);

            _uow.SaveChanges();

            _uow.AutomatismoRepository.Add(automatismo);

            CrearUbicaciones(automatismo);

            _uow.SaveChanges();

            CreateAutomatismoInterfaz(automatismo);
        }

        public virtual bool CreateAutomatismoInterfaz(IAutomatismo automatismo)
        {
            var interfacesAutomatismo = _uow.AutomatismoInterfazRepository.GetCodigoInterfazByTipoAutomatismo(automatismo.Tipo);

            if (interfacesAutomatismo != null && interfacesAutomatismo.Count > 0)
            {
                interfacesAutomatismo.ForEach(interfaz =>
                {
                    _uow.AutomatismoInterfazRepository.Add(new AutomatismoInterfaz
                    {
                        InterfazExterna = interfaz,
                        Interfaz = interfaz,
                        FechaRegistro = DateTime.Now,
                        IdAutomatismo = automatismo.Numero,
                        Id = _uow.AutomatismoInterfazRepository.GetNextNuAutomatismoInterfaz(),
                        Transaccion = _uow.GetTransactionNumber()
                    });
                });

                return true;
            }

            return false;
        }

        public virtual List<ValidationsError> GetJsonResponseFromAutomatismoEjecucion(int numeroAutomatismoEjecucion)
        {
            var ejecucion = _uow.AutomatismoEjecucionRepository.GetAutomatismoEjecucionWithData(numeroAutomatismoEjecucion);

            string jsonResponse = ejecucion.AutomatismoData.FirstOrDefault().ResponseData;

            var result = JsonConvert.DeserializeObject<ValidationsResult>(jsonResponse);

            return result.Errors;
        }

        public virtual void CrearUbicaciones(IAutomatismo automatismo)
        {
            var recorridoPorDefecto = _uow.RecorridoRepository.GetRecorridoPorDefectoParaPredio(automatismo.Predio);

            foreach (var posicion in automatismo.Posiciones)
            {
                CrearUbicacionesAutomatismo(automatismo, posicion, recorridoPorDefecto);
            }
        }

        public virtual void CrearUbicacionesAutomatismo(IAutomatismo automatismo, AutomatismoPosicion posicion, Recorrido recorridoPorDefecto)
        {
            Ubicacion ubicacion = GetUbicacion(automatismo, posicion);

            _uow.UbicacionRepository.AddUbicacion(ubicacion);

            posicion.IdUbicacion = ubicacion.Id;

            _uow.SaveChanges();

            _uow.AutomatismoPosicionRepository.Add(posicion);

            _uow.SaveChanges();

            if (posicion.TipoUbicacion == AutomatismoPosicionesTipoDb.POS_ENTRADA)
                InsertarDetalleRecorrido(recorridoPorDefecto, ubicacion);
        }

        public virtual Ubicacion GetUbicacion(IAutomatismo automatismo, AutomatismoPosicion posicion)
        {
            var nuAltura = _uow.AutomatismoPosicionRepository.GetNumeroAlturaAutomatismo(automatismo.Numero, posicion.TipoUbicacion);

            var paddedNuAltura = nuAltura.ToString().Length < 9 ? nuAltura.ToString().PadLeft(NUMBER_OF_DIGITS_TO_PAD, '0') : nuAltura.ToString();
            string prefijo = _uow.ParametroRepository.GetParameter(ParamManager.PREFIJO_AUTOMATISMO) ?? AutomatismoDb.PREFIJO_AUT;

            Ubicacion ubicacion = new Ubicacion
            {
                IdUbicacionZona = automatismo.ZonaUbicacion,
                IdProductoRotatividad = 0,
                IdProductoFamilia = 0,
                CodigoClase = ClaseProductoDb.General,
                CodigoSituacion = SituacionDb.Activo,
                IdEmpresa = 1,
                EsUbicacionBaja = true,
                NecesitaReabastecer = true,
                NumeroPredio = automatismo.Predio,
                Bloque = AutomatismoDb.PREFIJO_AUT,
                Altura = nuAltura,
                Columna = automatismo.Numero,
                FechaInsercion = DateTime.Now,
                IdUbicacionTipo = posicion.Ubicacion.IdUbicacionTipo,
                IdUbicacionArea = posicion.Ubicacion.IdUbicacionArea,
                Calle = posicion.Ubicacion.Calle,
                Profundidad = 1
            };


            ubicacion.Id = $"{automatismo.Predio}{prefijo}{automatismo.Numero}-{ubicacion.Calle}{paddedNuAltura}";
            ubicacion.CodigoBarras = $"{BarcodeDb.PREFIX_UBICACION}{ubicacion.Id}";

            return ubicacion;
        }

        public virtual bool InsertarCaracteristicasPorDefecto(IAutomatismo automatismo)
        {
            var caracteristicas = _uow.AutomatismoCaracteristicaRepository.GetCaracteristicasPorDefectoPorTipoAutomatismo(automatismo.Tipo);

            if (caracteristicas == null || caracteristicas.Count == 0) return false;

            foreach (var item in caracteristicas)
            {
                var caract = new AutomatismoCaracteristica
                {
                    Id = _uow.AutomatismoCaracteristicaRepository.GetNextNumeroCaracteristica(),
                    IdAutomatismo = automatismo.Numero,
                    FechaAlta = DateTime.Now,
                    Codigo = item.Codigo,
                    Descripcion = item.Descripcion,
                    CantidadAuxiliar = item.CantidadAuxiliar,
                    FlagAuxiliar = item.FlagAuxiliar,
                    NumeroAuxiliar = item.NumeroAuxiliar,
                    ValorAuxiliar = item.ValorAuxiliar,
                    Valor = item.Valor,
                    Transaccion = _uow.GetTransactionNumber()
                };

                automatismo.Caracteristicas.Add(caract);

                _uow.AutomatismoCaracteristicaRepository.Add(caract);
            }

            return true;
        }

        public virtual void RestablecerCaracteristicasPorDefecto(IAutomatismo automatismo)
        {
            automatismo.Caracteristicas.ToList().ForEach(i => _uow.AutomatismoCaracteristicaRepository.Remove(i));

            _uow.SaveChanges();

            InsertarCaracteristicasPorDefecto(automatismo);

            _uow.SaveChanges();
        }

        public virtual void CrearZonaUbicacion(IAutomatismo automatismo)
        {
            var zona = new ZonaUbicacion
            {
                Id = automatismo.ZonaUbicacion,
                Descripcion = automatismo.Descripcion,
                TipoZonaUbicacion = TipoUbicacionZonaDb.Automatismo,
                Estacion = automatismo.Numero.ToString(),
                EstacionAlmacenado = automatismo.Numero.ToString(),
                Habilitada = true,
                Alta = DateTime.Now,
            };

            _uow.ZonaUbicacionRepository.AddZona(zona);
            _uow.DominioRepository.AddDetalleDominioZona(zona.IdInterno, zona.Id);
        }

        public virtual void UpdateZonaUbicacion(IAutomatismo automatismo)
        {
            var zona = _uow.ZonaUbicacionRepository.GetZona(automatismo.ZonaUbicacion);

            zona.Descripcion = automatismo.Descripcion;
            zona.Modificacion = DateTime.Now;

            _uow.ZonaUbicacionRepository.UpdateZona(zona);
        }

        /// <summary>
        /// Factory Method para obtener las estrategias de comportamiento de cada request de ejecución de automatismo.
        /// </summary>
        public virtual IAutomatismoRequestStategy CreateAutomatismoRequestStategy(AutomatismoEjecucion ejecucion, IGridService gridService, IIdentityService identity, IGridExcelService excelService, IUnitOfWork uow)
        {
            switch (ejecucion.InterfazExterna)
            {
                case CodigoInterfazAutomatismoDb.CD_INTERFAZ_PRODUCTOS:
                    return new AutomatismoProductoRequestStrategy(gridService, identity, excelService, uow, ejecucion);

                case CodigoInterfazAutomatismoDb.CD_INTERFAZ_CODIGO_BARRAS:
                    return new AutomatismoCodigoBarrasRequestStrategy(gridService, identity, excelService, uow, ejecucion);

                case CodigoInterfazAutomatismoDb.CD_INTERFAZ_ENTRADAS:
                    return new AutomatismoEntradaRequestStrategy(gridService, identity, excelService, uow, ejecucion);

                case CodigoInterfazAutomatismoDb.CD_INTERFAZ_SALIDA:
                    return new AutomatismoSalidaRequestStrategy(gridService, identity, excelService, uow, ejecucion);

                case CodigoInterfazAutomatismoDb.CD_INTERFAZ_CONF_ENTRADAS:
                    return new AutomatismoConfirmacionEntradaRequestStrategy(gridService, identity, excelService, uow, ejecucion);

                case CodigoInterfazAutomatismoDb.CD_INTERFAZ_CONF_SALIDAS:
                    return new AutomatismoConfirmacionSalidaRequestStrategy(gridService, identity, excelService, uow, ejecucion);

                case CodigoInterfazAutomatismoDb.CD_INTERFAZ_NOTIF_AJUSTES:
                    return new AutomatismoNotificacionAjusteStockRequestStrategy(gridService, identity, excelService, uow, ejecucion);

                case CodigoInterfazAutomatismoDb.CD_INTERFAZ_UBICACIONES_PICKING:
                    return new AutomatismoUbicacionPickingRequestStrategy(gridService, identity, excelService, uow, ejecucion);

                default: throw new NotImplementedException("No implementa RequestStrategy");

            }
        }

        /// <summary>
        /// Método encargado de reprocesar la ejecución de automatismo
        /// </summary>
        public virtual void Reprocesar(int ejecucion, IAutomatismoAutoStoreClientService _automatismoAutoStoreClientService)
        {

            if (_automatismoAutoStoreClientService.IsEnabled())
            {
                var ejec = _uow.AutomatismoEjecucionRepository.GetAutomatismoEjecucionWithData(ejecucion);

                if (ejec.Estado == EstadoEjecucion.PROCESADO_OK || ejec.Estado == EstadoEjecucion.ESTPROCREP) throw new Exception("AUT101Update_Sec0_Error_EjecucionYaProcesada");

                var data = ejec.AutomatismoData.OrderByDescending(w => w.Id).FirstOrDefault();

                ejec.Estado = EstadoEjecucion.ESTPROCREP;

                _uow.AutomatismoEjecucionRepository.Update(ejec);

                _automatismoAutoStoreClientService.SendReprocesar(ejec.InterfazExterna, data.RequestData);

                _uow.SaveChanges();
            }

        }

        public virtual void InsertarDetalleRecorrido(Recorrido recorridoPorDefecto, Ubicacion ubicacion)
        {
            var detalleRecorrido = new DetalleRecorrido
            {
                IdRecorrido = recorridoPorDefecto.Id,
                Ubicacion = ubicacion.Id,
                ValorOrden = ubicacion.Id,
                NumeroOrden = -1,
                Transaccion = _uow.GetTransactionNumber()
            };

            _uow.RecorridoRepository.AddDetalleRecorrido(detalleRecorrido);

            _uow.SaveChanges();
        }
    }
}