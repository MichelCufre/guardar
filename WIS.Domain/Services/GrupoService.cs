using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;
using WIS.Security;

namespace WIS.Domain.Services
{
    public class GrupoService : IGrupoService
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IParameterService _parameterService;
        protected readonly ProductoMapper _productoMapper;

        public GrupoService(IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IParameterService parameterService)
        {
            this._uowFactory = uowFactory;
            this._identity = identity;
            this._parameterService = parameterService;
            this._productoMapper = new ProductoMapper();
        }

        public virtual Grupo GetGrupo(Producto producto)
        {
            Grupo grupo = null;

            //Funcionalidad estandar por defecto habilitada
            var manejoDeGrupos = (_parameterService.GetValue("MANEJO_GRUPOS_HABILITADO") ?? "S") == "S";

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                if (manejoDeGrupos)
                {
                    var reglas = uow.GrupoRepository.GetReglasAgrupacion();

                    foreach (var regla in reglas)
                    {
                        if (grupo != null)
                            break;

                        //Por cada regla obtengo lo parametros y se verifican los datos
                        //En cuanto un parametro ya no coincida se continua con la siguiente regla
                        //Al final si la regla cumple con los datos del producto se carga el grupo para terminar el foreach

                        var parametros = uow.GrupoRepository.GetParametrosRegla(regla.Id);
                        var parametroIds = new Dictionary<string, int>();

                        foreach (var p in uow.GrupoRepository.GetParametros())
                        {
                            parametroIds[p.Nombre] = p.Id;
                        }

                        var grupoAux = uow.GrupoRepository.GetGrupo(regla.CodigoGrupo);

                        if (!ValidarRegla(grupoAux, parametros, parametroIds, producto))
                            continue;

                        grupo = grupoAux;
                    }
                }

                //Si no se logró obtener un grupo a partir de los datos del producto y los valores definidos para los parametros de las reglas
                //se obtiene el grupo por defecto en base a la clase

                if (grupo == null)
                    grupo = uow.GrupoRepository.GetDefaultGrupo(producto.CodigoClase);
            }

            return grupo;
        }

        public virtual bool ValidarRegla(Grupo grupo, IEnumerable<GrupoReglaParametro> parametros, Dictionary<string, int> parametroIds, Producto producto)
        {
            var empresa = (int?)GetParametro(parametros, parametroIds, "CD_EMPRESA", typeof(int));
            if (empresa.HasValue)
            {
                if (producto.CodigoEmpresa != (int)empresa)
                    return false;
            }

            var unidadMedida = GetParametro(parametros, parametroIds, "CD_UNIDADE_MEDIDA")?.ToString();
            if (!string.IsNullOrEmpty(unidadMedida))
            {
                if (producto.UnidadMedida != unidadMedida)
                    return false;
            }

            //var clase = GetParametro(parametros, parametroIds, "CD_CLASSE")?.ToString();
            if (!string.IsNullOrEmpty(grupo.CodigoClase))
            {
                if (producto.CodigoClase != grupo.CodigoClase)
                    return false;
            }

            var familia = (int?)GetParametro(parametros, parametroIds, "CD_FAMILIA_PRODUTO", typeof(int));
            if (familia.HasValue)
            {
                if (producto.CodigoFamilia != familia)
                    return false;
            }

            var ramo = (short?)GetParametro(parametros, parametroIds, "CD_RAMO_PRODUTO", typeof(short));
            if (ramo.HasValue)
            {
                if (producto.Ramo != ramo)
                    return false;
            }

            var rotatividad = (short?)GetParametro(parametros, parametroIds, "CD_ROTATIVIDADE", typeof(short));
            if (rotatividad.HasValue)
            {
                if (producto.CodigoRotatividad != rotatividad)
                    return false;
            }

            var situacion = (short?)GetParametro(parametros, parametroIds, "CD_SITUACAO", typeof(short));
            if (situacion.HasValue)
            {
                if (producto.Situacion != (short)situacion)
                    return false;
            }

            var idManejoIdentificador = GetParametro(parametros, parametroIds, "ID_MANEJO_IDENTIFICADOR")?.ToString();
            if (!string.IsNullOrEmpty(idManejoIdentificador))
            {
                if (producto.ManejoIdentificador != _productoMapper.MapManejoIdentificador(idManejoIdentificador))
                    return false;
            }

            var modalidadIngresoLote = GetParametro(parametros, parametroIds, "ND_MODALIDAD_INGRESO_LOTE")?.ToString();
            if (!string.IsNullOrEmpty(modalidadIngresoLote))
            {
                if (producto.ModalidadIngresoLote != modalidadIngresoLote)
                    return false;
            }

            var manejoFecha = GetParametro(parametros, parametroIds, "TP_MANEJO_FECHA")?.ToString();
            if (!string.IsNullOrEmpty(manejoFecha))
            {
                if (producto.TipoManejoFecha != manejoFecha)
                    return false;
            }

            var aceptaDecimales = GetParametro(parametros, parametroIds, "FL_ACEPTA_DECIMALES")?.ToString();
            if (!string.IsNullOrEmpty(aceptaDecimales))
            {
                if (producto.AceptaDecimales != ((aceptaDecimales).ToUpper() == "S"))
                    return false;
            }

            var grupoConsulta = GetParametro(parametros, parametroIds, "CD_GRUPO_CONSULTA")?.ToString();
            if (!string.IsNullOrEmpty(grupoConsulta))
            {
                if (producto.GrupoConsulta != grupoConsulta)
                    return false;
            }

            var mercadoLogico = GetParametro(parametros, parametroIds, "CD_MERCADOLOGICO")?.ToString();
            if (!string.IsNullOrEmpty(mercadoLogico))
            {
                if (producto.CodigoMercadologico != mercadoLogico)
                    return false;
            }

            var codigoProdEmpresa = GetParametro(parametros, parametroIds, "CD_PRODUTO_EMPRESA")?.ToString();
            if (!string.IsNullOrEmpty(codigoProdEmpresa))
            {
                if (producto.CodigoProductoEmpresa != codigoProdEmpresa)
                    return false;
            }

            var nam = GetParametro(parametros, parametroIds, "CD_NAM")?.ToString();
            if (!string.IsNullOrEmpty(nam))
            {
                if (producto.NAM != nam)
                    return false;
            }

            var codigoBase = GetParametro(parametros, parametroIds, "CODIGO_BASE")?.ToString();
            if (!string.IsNullOrEmpty(codigoBase))
            {
                if (producto.CodigoBase != codigoBase)
                    return false;
            }

            var talle = GetParametro(parametros, parametroIds, "TALLE")?.ToString();
            if (!string.IsNullOrEmpty(talle))
            {
                if (producto.Talle != talle)
                    return false;
            }

            var color = GetParametro(parametros, parametroIds, "COLOR")?.ToString();
            if (!string.IsNullOrEmpty(color))
            {
                if (producto.Color != color)
                    return false;
            }

            var temporada = GetParametro(parametros, parametroIds, "TEMPORADA")?.ToString();
            if (!string.IsNullOrEmpty(temporada))
            {
                if (producto.Temporada != temporada)
                    return false;
            }

            var categoria = GetParametro(parametros, parametroIds, "VL_CATEGORIA_01")?.ToString();
            if (!string.IsNullOrEmpty(categoria))
            {
                if (producto.Categoria1 != categoria)
                    return false;
            }

            var categoria2 = GetParametro(parametros, parametroIds, "VL_CATEGORIA_02")?.ToString();
            if (!string.IsNullOrEmpty(categoria2))
            {
                if (producto.Categoria2 != categoria2)
                    return false;
            }

            var componente = GetParametro(parametros, parametroIds, "ND_FACTURACION_COMP1")?.ToString();
            if (!string.IsNullOrEmpty(componente))
            {
                if (producto.Componente1 != componente)
                    return false;
            }

            var componente2 = GetParametro(parametros, parametroIds, "ND_FACTURACION_COMP2")?.ToString();
            if (!string.IsNullOrEmpty(componente2))
            {
                if (producto.Componente2 != componente2)
                    return false;
            }

            var codigoExclusividad = (short?)GetParametro(parametros, parametroIds, "CD_EXCLUSIVO", typeof(short));
            if (codigoExclusividad.HasValue)
            {
                if (producto.Exclusivo != codigoExclusividad)
                    return false;
            }

            var diasValidarDesde = (short?)GetParametro(parametros, parametroIds, "QT_DIAS_VALIDADE_DESDE", typeof(short));
            if (diasValidarDesde.HasValue)
            {
                var diasValidarHasta = (short?)GetParametro(parametros, parametroIds, "QT_DIAS_VALIDADE_HASTA", typeof(short));
                if (producto.DiasValidez < diasValidarDesde || producto.DiasValidez > diasValidarHasta)
                    return false;
            }

            var diasDuracionDesde = (short?)GetParametro(parametros, parametroIds, "QT_DIAS_DURACAO_DESDE", typeof(short));
            if (diasDuracionDesde.HasValue)
            {
                var diasDuracionHasta = (short?)GetParametro(parametros, parametroIds, "QT_DIAS_DURACAO_HASTA", typeof(short));
                if (producto.DiasDuracion < diasDuracionDesde || producto.DiasDuracion > diasDuracionHasta)
                    return false;
            }

            var diasLiberacionDesde = (short?)GetParametro(parametros, parametroIds, "QT_DIAS_VALIDADE_LIBERACION_DESDE", typeof(short));
            if (diasLiberacionDesde.HasValue)
            {
                var diasLiberacionHasta = (short?)GetParametro(parametros, parametroIds, "QT_DIAS_VALIDADE_LIBERACION_HASTA", typeof(short));
                if (producto.DiasLiberacion < diasLiberacionDesde || producto.DiasLiberacion > diasLiberacionHasta)
                    return false;
            }

            var stockMinimoDesde = (int?)GetParametro(parametros, parametroIds, "QT_ESTOQUE_MINIMO_DESDE", typeof(int));
            if (stockMinimoDesde.HasValue)
            {
                var stockMinimoHasta = (int?)GetParametro(parametros, parametroIds, "QT_ESTOQUE_MINIMO_HASTA", typeof(int));
                if (producto.StockMinimo < stockMinimoDesde || producto.StockMinimo > stockMinimoHasta)
                    return false;
            }

            var stockMaximoDesde = (int?)GetParametro(parametros, parametroIds, "QT_ESTOQUE_MAXIMO_DESDE", typeof(int));
            if (stockMaximoDesde.HasValue)
            {
                var stockMaximoHasta = (int?)GetParametro(parametros, parametroIds, "QT_ESTOQUE_MAXIMO_HASTA", typeof(int));
                if (producto.StockMaximo < stockMaximoDesde || producto.StockMaximo > stockMaximoHasta)
                    return false;
            }

            var pesoLiquidoDesde = (decimal?)GetParametro(parametros, parametroIds, "PS_LIQUIDO_DESDE", typeof(decimal));
            if (pesoLiquidoDesde.HasValue)
            {
                var pesoLiquidoHasta = (decimal?)GetParametro(parametros, parametroIds, "PS_LIQUIDO_HASTA", typeof(decimal));
                if (producto.PesoNeto < pesoLiquidoDesde || producto.PesoNeto > pesoLiquidoHasta)
                    return false;
            }

            var pesoBrutoDesde = (decimal?)GetParametro(parametros, parametroIds, "PS_BRUTO_DESDE", typeof(decimal));
            if (pesoBrutoDesde.HasValue)
            {
                var pesoBrutoHasta = (decimal?)GetParametro(parametros, parametroIds, "PS_BRUTO_HASTA", typeof(decimal));
                if (producto.PesoBruto < pesoBrutoDesde || producto.PesoBruto > pesoBrutoHasta)
                    return false;
            }

            var alturaDesde = (decimal?)GetParametro(parametros, parametroIds, "VL_ALTURA_DESDE", typeof(decimal));
            if (alturaDesde.HasValue)
            {
                var alturaHasta = (decimal?)GetParametro(parametros, parametroIds, "VL_ALTURA_HASTA", typeof(decimal));
                if (producto.Altura < alturaDesde || producto.Altura > alturaHasta)
                    return false;
            }

            var largoDesde = (decimal?)GetParametro(parametros, parametroIds, "VL_LARGURA_DESDE", typeof(decimal));
            if (largoDesde.HasValue)
            {
                var largoHasta = (decimal?)GetParametro(parametros, parametroIds, "VL_LARGURA_HASTA", typeof(decimal));
                if (producto.Ancho < largoDesde || producto.Ancho > largoHasta)
                    return false;
            }

            var profundidadDesde = (decimal?)GetParametro(parametros, parametroIds, "VL_PROFUNDIDADE_DESDE", typeof(decimal));
            if (profundidadDesde.HasValue)
            {
                var profundidadHasta = (decimal?)GetParametro(parametros, parametroIds, "VL_PROFUNDIDADE_HASTA", typeof(decimal));
                if (producto.Profundidad < profundidadDesde || producto.Profundidad > profundidadHasta)
                    return false;
            }

            var volumenDesde = (decimal?)GetParametro(parametros, parametroIds, "VL_CUBAGEM_DESDE", typeof(decimal));
            if (volumenDesde.HasValue)
            {
                var volumenHasta = (decimal?)GetParametro(parametros, parametroIds, "VL_CUBAGEM_HASTA", typeof(decimal));
                if (producto.VolumenCC < volumenDesde || producto.VolumenCC > volumenHasta)
                    return false;
            }

            var undBultoDesde = (decimal?)GetParametro(parametros, parametroIds, "QT_UND_BULTO_DESDE", typeof(decimal));
            if (undBultoDesde.HasValue)
            {
                var undBultoHasta = (decimal?)GetParametro(parametros, parametroIds, "QT_UND_BULTO_HASTA", typeof(decimal));
                if (producto.UnidadBulto < undBultoDesde || producto.UnidadBulto > undBultoHasta)
                    return false;
            }

            var undDistribucionDesde = (decimal?)GetParametro(parametros, parametroIds, "QT_UND_DISTRIBUCION_DESDE", typeof(decimal));
            if (undDistribucionDesde.HasValue)
            {
                var undDistribucionHasta = (decimal?)GetParametro(parametros, parametroIds, "QT_UND_DISTRIBUCION_HASTA", typeof(decimal));
                if (producto.UnidadDistribucion < undDistribucionDesde || producto.UnidadDistribucion > undDistribucionHasta)
                    return false;
            }

            var subBultoDesde = (short?)GetParametro(parametros, parametroIds, "QT_SUBBULTO_DESDE", typeof(short));
            if (subBultoDesde.HasValue)
            {
                var subBultoHasta = (short?)GetParametro(parametros, parametroIds, "QT_SUBBULTO_HASTA", typeof(short));
                if (producto.SubBulto < subBultoDesde || producto.SubBulto > subBultoHasta)
                    return false;
            }

            var ultimoCostoDesde = (decimal?)GetParametro(parametros, parametroIds, "VL_CUSTO_ULT_ENT_DESDE", typeof(decimal));
            if (ultimoCostoDesde.HasValue)
            {
                var ultimoCostoHasta = (decimal?)GetParametro(parametros, parametroIds, "VL_CUSTO_ULT_ENT_HASTA", typeof(decimal));
                if (producto.UltimoCosto < ultimoCostoDesde || producto.UltimoCosto > ultimoCostoHasta)
                    return false;
            }

            var precioVentaDesde = (decimal?)GetParametro(parametros, parametroIds, "VL_PRECO_VENDA_DESDE", typeof(decimal));
            if (precioVentaDesde.HasValue)
            {
                var precioVentaHasta = (decimal?)GetParametro(parametros, parametroIds, "VL_PRECO_VENDA_HASTA", typeof(decimal));
                if (producto.PrecioVenta < precioVentaDesde || producto.PrecioVenta > precioVentaHasta)
                    return false;
            }

            var cantGenericaDesde = (decimal?)GetParametro(parametros, parametroIds, "QT_GENERICO_DESDE", typeof(decimal));
            if (cantGenericaDesde.HasValue)
            {
                var cantGenericaHasta = (decimal?)GetParametro(parametros, parametroIds, "QT_GENERICO_HASTA", typeof(decimal));
                if (producto.CantidadGenerica < cantGenericaDesde || producto.CantidadGenerica > cantGenericaHasta)
                    return false;
            }

            var precioStockDesde = (decimal?)GetParametro(parametros, parametroIds, "VL_PRECIO_STOCK_DESDE", typeof(decimal));
            if (precioStockDesde.HasValue)
            {
                var precioStockHasta = (decimal?)GetParametro(parametros, parametroIds, "VL_PRECIO_STOCK_HASTA", typeof(decimal));
                if (producto.PrecioStock < precioStockDesde || producto.PrecioStock > precioStockHasta)
                    return false;
            }
            return true;
        }

        public virtual object GetParametro(IEnumerable<GrupoReglaParametro> parametros, Dictionary<string, int> parametroIds, string nmParametro, Type type = null)
        {
            var param = parametros.FirstOrDefault(p => p.NroParametro == parametroIds[nmParametro])?.Valor;

            if (!string.IsNullOrEmpty(param))
            {
                if (type == null)
                    return param;
                else if (type == typeof(int))
                    return int.Parse(param);
                else if (type == typeof(short))
                    return short.Parse(param);
                else if (type == typeof(long))
                    return long.Parse(param);
                else if (type == typeof(double))
                    return double.Parse(param);
                else if (type == typeof(decimal))
                {
                    if (decimal.TryParse(param, NumberStyles.Number, _identity.GetFormatProvider(), out decimal parsedValue))
                        return parsedValue;
                    else
                        return null;
                }
            }
            return null;
        }

        public virtual List<SelectOption> GetOptionsByParam(IUnitOfWork uow, string codigoParametro, string tipoParametro, string searchValue, int userId)
        {
            var opciones = new List<SelectOption>();

            if ((tipoParametro == "SEARCH" && string.IsNullOrEmpty(searchValue)) || (string.IsNullOrEmpty(codigoParametro) && string.IsNullOrEmpty(tipoParametro)) )
                return opciones;

            switch (codigoParametro)
            {
                case "CD_EMPRESA":

                    var empresasUsuario = uow.EmpresaRepository.GetByNombreOrCodePartialForUsuario(searchValue, userId);
                    foreach (var emp in empresasUsuario)
                    {
                        opciones.Add(new SelectOption(emp.Id.ToString(), $"{emp.Id} - {emp.Nombre}"));
                    }

                    break;
                case "CD_UNIDADE_MEDIDA":

                    var unidades = uow.UnidadMedidaRepository.GetByNombreOrCodePartial(searchValue);
                    foreach (var unidad in unidades)
                    {
                        opciones.Add(new SelectOption(unidad.Id.ToString(), $"{unidad.Id} - {unidad.Descripcion}"));
                    }

                    break;
                case "CD_CLASSE":

                    var clases = uow.ClaseRepository.GetClases();
                    foreach (var clase in clases)
                    {
                        opciones.Add(new SelectOption(clase.Id.ToString(), $"{clase.Id} - {clase.Descripcion}")); ;
                    }

                    break;
                case "CD_FAMILIA_PRODUTO":

                    var familias = uow.ProductoFamiliaRepository.GetByNombreOrCodePartial(searchValue);
                    foreach (var familia in familias)
                    {
                        opciones.Add(new SelectOption(familia.Id.ToString(), $"{familia.Id} - {familia.Descripcion}"));
                    }

                    break;
                case "CD_RAMO_PRODUTO":

                    var ramos = uow.ProductoRamoRepository.GetProductoRamos();
                    foreach (var ramo in ramos)
                    {
                        opciones.Add(new SelectOption(ramo.Id.ToString(), $"{ramo.Id} - {ramo.Descripcion}")); ;
                    }

                    break;
                case "CD_ROTATIVIDADE":

                    var rotatividades = uow.ProductoRotatividadRepository.GetProductoRotatividades();
                    foreach (var rotatividad in rotatividades)
                    {
                        opciones.Add(new SelectOption(rotatividad.Id.ToString(), $"{rotatividad.Id} - {rotatividad.Descripcion}")); ;
                    }

                    break;
                case "CD_SITUACAO":

                    opciones.Add(new SelectOption(SituacionDb.Activo.ToString(), "REG009_frm_opt_Situacion_Activo"));
                    opciones.Add(new SelectOption(SituacionDb.Inactivo.ToString(), "REG009_frm_opt_Situacion_Inactivo"));

                    break;
                case "ID_MANEJO_IDENTIFICADOR":

                    opciones.Add(new SelectOption(ManejoIdentificadorDb.Lote, "REG009_frm_opt_ManejoIdentificador_Lote"));
                    opciones.Add(new SelectOption(ManejoIdentificadorDb.Serie, "REG009_frm_opt_ManejoIdentificador_Serie"));
                    opciones.Add(new SelectOption(ManejoIdentificadorDb.Producto, "REG009_frm_opt_ManejoIdentificador_Producto"));

                    break;
                case "ND_MODALIDAD_INGRESO_LOTE":

                    opciones.Add(new SelectOption(ModalidadIngresoLoteDb.Normal, "REG009_frm_opt_ModalidadLote_Normal"));
                    opciones.Add(new SelectOption(ModalidadIngresoLoteDb.Vencimiento, "REG009_frm_opt_ModalidadLote_Vencimiento"));
                    opciones.Add(new SelectOption(ModalidadIngresoLoteDb.Agenda, "REG009_frm_opt_ModalidadLote_Agenda"));
                    opciones.Add(new SelectOption(ModalidadIngresoLoteDb.Documento, "REG009_frm_opt_ModalidadLote_Documento"));
                    opciones.Add(new SelectOption(ModalidadIngresoLoteDb.VencimientoYYYYMM, "REG009_frm_opt_ModalidadLote_VencimientoYYYYMM"));

                    break;
                case "TP_MANEJO_FECHA":

                    opciones.Add(new SelectOption(ManejoFechaProductoDb.Duradero, "REG009_frm_opt_ManejoFecha_Duradero"));
                    opciones.Add(new SelectOption(ManejoFechaProductoDb.Fifo, "REG009_frm_opt_ManejoFecha_Fifo"));
                    opciones.Add(new SelectOption(ManejoFechaProductoDb.Expirable, "REG009_frm_opt_ManejoFecha_Fefo"));

                    break;
                case "CD_GRUPO_CONSULTA":

                    var gruposAsignados = uow.GrupoConsultaRepository.GetGrupoConsultaAsignados(userId);
                    foreach (var grupo in gruposAsignados)
                    {
                        opciones.Add(new SelectOption(grupo.Id, $"{grupo.Id} - {grupo.Descripcion}"));
                    }

                    break;
                case "CD_NAM":
                    var ncms = uow.NcmRepository.GetByNombreOrCodePartial(searchValue);

                    foreach (var ncm in ncms)
                    {
                        opciones.Add(new SelectOption(ncm.Id.ToString(), $"{ncm.Id} - {ncm.Descripcion}"));
                    }
                    break;
                default:
                    //FL_ACEPTA_DECIMALES
                    opciones = new List<SelectOption>
                    {
                        new SelectOption("S", "General_form_select_SI"),
                        new SelectOption("N", "General_form_select_NO"),
                        new SelectOption(" ", "General_form_select_Todos"),
                    };
                    break;
            }
            return opciones;
        }
    }
}
