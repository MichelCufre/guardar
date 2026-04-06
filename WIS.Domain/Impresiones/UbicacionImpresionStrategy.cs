using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.Configuracion;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Impresiones
{
    public class UbicacionImpresionStrategy : IImpresionDetalleBuildingStrategy
    {
        protected readonly string _separator = " ";
        protected readonly IEstiloTemplate _estilo;
        protected readonly List<Ubicacion> _ubicaciones;
        protected readonly IPrintingService _printingService;
        protected readonly UbicacionConfiguracion _configuracion;

        public UbicacionImpresionStrategy(IEstiloTemplate estilo,
            IPrintingService printingService,
            UbicacionConfiguracion configuracion,
            List<Ubicacion> ubicaciones)
        {
            this._estilo = estilo;
            this._ubicaciones = ubicaciones;
            this._printingService = printingService;
            this._configuracion = configuracion;
        }

        public virtual List<DetalleImpresion> Generar(Impresora impresora)
        {
            TemplateImpresion template = this._estilo.GetTemplate(impresora);

            var detalles = new List<DetalleImpresion>();

            foreach (var ubicacion in this._ubicaciones.OrderBy(s => s.Id))
            {
                Dictionary<string, string> claves = this.GetDiccionarioInformacion(ubicacion, template.EstiloEtiqueta);

                detalles.Add(new DetalleImpresion
                {
                    Contenido = template.Parse(claves),
                    Estado = _printingService.GetEstadoInicial(),
                    FechaProcesado = DateTime.Now,
                });
            }

            return detalles;
        }

        public virtual Dictionary<string, string> GetDiccionarioInformacion(Ubicacion ubicacion, string estiloEtiqueta)
        {
            var claves = new Dictionary<string, string>()
            {
                {"T_ENDERECO_ESTOQUE.CD_ENDERECO", ubicacion.Id},
                {"T_ENDERECO_ESTOQUE.CD_EMPRESA",ubicacion.IdEmpresa.ToString()},
                {"T_ENDERECO_ESTOQUE.CD_TIPO_ENDERECO", ubicacion.IdUbicacionTipo.ToString()},
                {"T_ENDERECO_ESTOQUE.CD_ROTATIVIDADE", ubicacion.IdProductoRotatividad.ToString()},
                {"T_ENDERECO_ESTOQUE.CD_FAMILIA_PRINCIPAL", ubicacion.IdProductoFamilia.ToString()},
                {"T_ENDERECO_ESTOQUE.CD_CLASSE", ubicacion.CodigoClase},
                {"T_ENDERECO_ESTOQUE.CD_SITUACAO", ubicacion.CodigoSituacion.ToString()},
                {"T_ENDERECO_ESTOQUE.ID_ENDERECO_BAIXO", ubicacion.EsUbicacionBaja ? "S" : "N"},
                {"T_ENDERECO_ESTOQUE.ID_NECESSIDADE_RESUPRIR", ubicacion.NecesitaReabastecer ? "S" : "N"},
                {"T_ENDERECO_ESTOQUE.DT_UPDROW", ubicacion.FechaModificacion.ToString()},
                {"T_ENDERECO_ESTOQUE.DT_ADDROW", ubicacion.FechaInsercion.ToString()},
                {"T_ENDERECO_ESTOQUE.CD_CONTROL", ubicacion.CodigoControl},
                {"T_ENDERECO_ESTOQUE.CD_AREA_ARMAZ", ubicacion.IdUbicacionArea.ToString()},
                {"T_ENDERECO_ESTOQUE.NU_COMPONENTE", ubicacion.FacturacionComponente},
                {"T_ENDERECO_ESTOQUE.ID_BLOQUE", ubicacion.Bloque},
                {"T_ENDERECO_ESTOQUE.ID_CALLE", ubicacion.Calle},
                {"T_ENDERECO_ESTOQUE.NU_ALTURA", ubicacion.Altura.ToString()},
                {"T_ENDERECO_ESTOQUE.NU_COLUMNA", ubicacion.Columna.ToString()},
                {"T_ENDERECO_ESTOQUE.NU_PREDIO", ubicacion.NumeroPredio},
                {"T_ENDERECO_ESTOQUE.NU_PROFUNDIDAD", ubicacion.Profundidad.ToString()},
                {"T_ENDERECO_ESTOQUE.CD_BARRAS", ubicacion.CodigoBarras},

                {"T_CLASSE.DS_CLASSE", ubicacion.Clase?.Descripcion},
                {"T_TIPO_ENDERECO.DS_TIPO_ENDERECO", ubicacion.UbicacionTipo?.Descripcion},
                {"T_FAMILIA_PRODUTO.T_DS_FAMILIA_PRODUTO", ubicacion.ProductoFamilia?.Descripcion},
                {"T_EMPRESA.NM_EMPRESA", ubicacion.Empresa?.Nombre},
                {"T_ROTATIVIDADE.DS_ROTATIVIDADE", ubicacion.ProductoRotatividad?.Descripcion},
            };

            var ubicacionFormateada = string.Empty;

            try
            {
                //Repsetar los espacios establecidos para cada estilo al armar la ubicación formateada 

                switch (estiloEtiqueta)
                {
                    case EstiloEtiquetaUbicacion.Altura:

                        ubicacionFormateada = $"{ubicacion.NumeroPredio}{_separator}{GetBloque(ubicacion)}{_separator}{GetCalle(ubicacion)}{_separator}{GetColumna(ubicacion)}{_separator}XX";

                        var barras = $"WISSD{ubicacion.NumeroPredio}{GetBloque(ubicacion)}{GetCalle(ubicacion)}{GetColumna(ubicacion)}00";
                        claves.Add("T_ENDERECO_ESTOQUE.CD_BARRAS_ENDERECO", barras);
                        break;

                    case EstiloEtiquetaUbicacion.Puerta:

                        var aux = $"{ubicacion.NumeroPredio}{ubicacion.Bloque}";
                        ubicacionFormateada = $"{ubicacion.NumeroPredio}{_separator}{GetBloque(ubicacion)}{_separator}{ubicacion.Id.Replace(aux, "")}";
                        break;

                    case EstiloEtiquetaUbicacion.Externa:
                        ubicacionFormateada = ubicacion.Id;
                        break;

                    default:

                        if (!string.IsNullOrEmpty(ubicacion.Bloque) && !string.IsNullOrEmpty(ubicacion.Calle) && ubicacion.Columna != null && ubicacion.Altura != null && !ubicacion.Id.Contains("-"))
                        {
                            ubicacionFormateada = $"{ubicacion.NumeroPredio}{_separator}{GetBloque(ubicacion)}{_separator}{GetCalle(ubicacion)}{_separator}{GetColumna(ubicacion)}{_separator}{GetAltura(ubicacion)}";
                        }
                        else
                            ubicacionFormateada = ubicacion.Id;

                        break;
                }
            }
            catch (Exception ex)
            {
                ubicacionFormateada = ubicacion.Id;
            }

            claves.Add("WIS.CD_ENDERECO_FORMATEADO", ubicacionFormateada);

            return claves;
        }

        public virtual string GetBloque(Ubicacion ubicacion)
        {
            if (_configuracion.BloqueNumerico)
                return ubicacion.Bloque?.PadLeft(_configuracion.BloqueLargo, '0');
            else
                return ubicacion.Bloque;
        }

        public virtual string GetCalle(Ubicacion ubicacion)
        {
            if (_configuracion.CalleNumerico)
                return ubicacion.Calle?.PadLeft(_configuracion.CalleLargo, '0');
            else
                return ubicacion.Calle;
        }

        public virtual string GetColumna(Ubicacion ubicacion)
        {
            return ubicacion.Columna?.ToString()?.PadLeft(_configuracion.ColumnaLargo, '0');
        }

        public virtual string GetAltura(Ubicacion ubicacion)
        {
            return ubicacion.Altura?.ToString()?.PadLeft(_configuracion.AlturaLargo, '0');
        }
    }
}
