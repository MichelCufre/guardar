using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.Automatismo.Constants;
using WIS.Domain.Automatismo.Interfaces;
using WIS.Domain.DataModel.Mappers.Automatismo;
using WIS.Domain.DataModel.Mappers.Constants;

namespace WIS.Domain.Automatismo
{
    public class AutomatismoPtl : Automatismo, IPtl
    {
        public AutomatismoPtl()
        {
            Posiciones = new List<AutomatismoPosicion>();
            Puestos = new List<AutomatismoPuesto>();
            Caracteristicas = new List<AutomatismoCaracteristica>();
            Interfaces = new List<AutomatismoInterfaz>();
            Ejecuciones = new List<AutomatismoEjecucion>();
        }

        private AutomatismoCaracteristicaMapper _mapper = new AutomatismoCaracteristicaMapper();

        public virtual List<PtlColor> GetColoresActivos()
        {
            return this.Caracteristicas.Where(w => w.Codigo == AutomatismoDb.CARACTERISTICA_COLOR && w.FlagAuxiliar)?.Select(s => _mapper.Map(s))?.ToList() ?? new List<PtlColor>();
        }

        public virtual List<PtlColor> GetColores()
        {
            return this.Caracteristicas.Where(w => w.Codigo == AutomatismoDb.CARACTERISTICA_COLOR && w.FlagAuxiliar == false)?.Select(s => _mapper.Map(s))?.ToList() ?? new List<PtlColor>();
        }

        public virtual string GetColorCerrado()
        {
            string codigoColorCerrado = this.GetCaracteristicaByCodigo(AutomatismoDb.CARACTERISTICA_COLOR_CERRADO).Codigo;
            List<PtlColor> coloresIternos = this.GetColores();
            return coloresIternos.FirstOrDefault(w => w.Code == codigoColorCerrado).Code;
        }

        public virtual string GetColorError()
        {
            return this.GetColores().FirstOrDefault(w => w.Code == this.GetCaracteristicaByCodigo(AutomatismoDb.CARACTERISTICA_COLOR_ERROR).Codigo).Code;
        }

        public virtual string GetTipo()
        {
            return this.GetCaracteristicaByCodigo(AutomatismoDb.CARACTERISTICA_TIPO_PTL)?.Codigo;
        }

        public virtual int GetTipoAgrupacion()
        {
            return int.Parse(this.GetCaracteristicaByCodigo(AutomatismoDb.CARACTERISTICA_TIPO_AGRUPACION_POSICION)?.Valor);
        }

        public override void GetConfiguracionUbicacionDefault(string tipoPosicion, out short area, out short tipo, out int? tipoAgrupacion)
        {
            switch (tipoPosicion)
            {
                case AutomatismoDb.TP_UBIC_PICKING:
                    area = AreaUbicacionDb.Picking;
                    tipo = TipoUbicacionDb.Monoproducto;
                    tipoAgrupacion = AutomatismoDb.TIPO_AGRUPACION_PICKING_MONO_PROD;
                    break;
                case AutomatismoDb.TP_UBIC_ENTRADA:
                    area = AreaUbicacionDb.Transferencia;
                    tipo = TipoUbicacionDb.Monoproducto;
                    tipoAgrupacion = AutomatismoDb.TIPO_AGRUPACION_GENERAL;
                    break;
                case AutomatismoDb.TP_UBIC_TRANSITO:
                    area = AreaUbicacionDb.AutomatismoTransferencia;
                    tipo = TipoUbicacionDb.Monoproducto;
                    tipoAgrupacion = null;
                    break;
                case AutomatismoDb.TP_UBIC_SALIDA:
                    area = AreaUbicacionDb.Transferencia;
                    tipo = TipoUbicacionDb.Monoproducto;
                    tipoAgrupacion = AutomatismoDb.TIPO_AGRUPACION_GENERAL;
                    break;
                default: throw new Exception("AUT100_Sec0_Err_TipoNoValidoParaElTipoDeAut");

            }
        }

        public override void GenerarUbicaciones(AutomatismoCantidades cantidades)
        {
            GenerarUbicacionesByTipoUbicacion(cantidades.UbicacionesPicking, AutomatismoDb.TP_UBIC_PICKING);
            GenerarUbicacionesByTipoUbicacion(cantidades.UbicacionesTransito, AutomatismoDb.TP_UBIC_TRANSITO);
            GenerarUbicacionesByTipoUbicacion(1, AutomatismoDb.TP_UBIC_ENTRADA);
            GenerarUbicacionesByTipoUbicacion(1, AutomatismoDb.TP_UBIC_SALIDA);
        }

        public override void ActualizarValoresSegunCaracteristica(AutomatismoCaracteristica caracteristica, List<AutomatismoCaracteristicaConfiguracion> configDefault)
        {
            base.ActualizarValoresSegunCaracteristica(caracteristica, configDefault);

            if (caracteristica.Codigo == AutomatismoDb.CARACTERISTICA_PROVEEDOR_PTL)
            {
                var configs = configDefault.Where(w => w.TipoAutomatismo == $"PTL_{caracteristica.Codigo}");

                foreach (var caract in this.Caracteristicas.Where(w => configs.Any(a => a.Codigo == w.Codigo)))
                {
                    caract.Transaccion = -1;
                }

                foreach (var item in configs)
                {
                    this.Caracteristicas.Add(new AutomatismoCaracteristica
                    {
                        IdAutomatismo = this.Numero,
                        FechaAlta = DateTime.Now,
                        Codigo = item.Codigo,
                        Descripcion = item.Descripcion,
                        CantidadAuxiliar = item.CantidadAuxiliar,
                        FlagAuxiliar = item.FlagAuxiliar,
                        NumeroAuxiliar = item.NumeroAuxiliar,
                        ValorAuxiliar = item.ValorAuxiliar,
                        Valor = item.Valor,
                        Transaccion = 0
                    });
                }
            }
        }

        public override bool AllowEditionEjecucion()
        {
            return false;
        }

        public virtual bool ManejaApagadoLuz()
        {
            return this.GetCaracteristicaByCodigo(AutomatismoDb.CARACTERISTICA_PTL_MAN_OFF)?.Codigo == "S";
        }

        public virtual string GetCodigoError()
        {
            return this.GetCaracteristicaByCodigo(AutomatismoDb.CARACTERISTICA_PTL_COD_ERR)?.Codigo ?? "";
        }

        public virtual bool RequiereConfirmacionCierre()
        {
            return this.GetCaracteristicaByCodigo(AutomatismoDb.CARACTERISTICA_PTL_CONF_CIERRE)?.Codigo == "S";

        }

        public virtual string GetCodigoCancelacion()
        {
            return this.GetCaracteristicaByCodigo(AutomatismoDb.CARACTERISTICA_PTL_COD_CAN)?.Codigo ?? "";
        }

        public virtual string GetCodigoLleno()
        {
            return this.GetCaracteristicaByCodigo(AutomatismoDb.CARACTERISTICA_PTL_COD_LLENO)?.Codigo ?? "";
        }

        public virtual string GetCodigoCierre()
        {
            return this.GetCaracteristicaByCodigo(AutomatismoDb.CARACTERISTICA_PTL_COD_CIERRE)?.Codigo ?? "";
        }
    }
}
