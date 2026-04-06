using System;
using System.Collections.Generic;
using WIS.Domain.Automatismo.Constants;
using WIS.Domain.DataModel.Mappers.Constants;

namespace WIS.Domain.Automatismo
{
    public class AutomatismoAutoStore : Automatismo
    {
        public AutomatismoAutoStore()
        {
            Posiciones = new List<AutomatismoPosicion>();
            Puestos = new List<AutomatismoPuesto>();
            Caracteristicas = new List<AutomatismoCaracteristica>();
            Interfaces = new List<AutomatismoInterfaz>();
            Ejecuciones = new List<AutomatismoEjecucion>();
        }

        public override void GenerarUbicaciones(AutomatismoCantidades cantidades)
        {
            GenerarUbicacionesByTipoUbicacion(cantidades.UbicacionesPicking, AutomatismoDb.TP_UBIC_PICKING);
            GenerarUbicacionesByTipoUbicacion(cantidades.UbicacionesEntrada, AutomatismoDb.TP_UBIC_ENTRADA);
            GenerarUbicacionesByTipoUbicacion(cantidades.UbicacionesAjuste, AutomatismoDb.TP_UBIC_AJUSTES);
            GenerarUbicacionesByTipoUbicacion(cantidades.UbicacionesRechazo, AutomatismoDb.TP_UBIC_RECHAZOS);
            GenerarUbicacionesByTipoUbicacion(cantidades.UbicacionesTransito, AutomatismoDb.TP_UBIC_TRANSITO);
            GenerarUbicacionesByTipoUbicacion(cantidades.UbicacionesSalida, AutomatismoDb.TP_UBIC_SALIDA);
        }

        public override void GetConfiguracionUbicacionDefault(string tipoPosicion, out short area, out short tipo, out int? tipoAgrupacion)
        {
            tipoAgrupacion = null;

            switch (tipoPosicion)
            {
                case AutomatismoDb.TP_UBIC_PICKING:
                    area = AreaUbicacionDb.AutomatismoPicking;
                    tipo = TipoUbicacionDb.AutomatismoMultiproducto;
                    break;

                case AutomatismoDb.TP_UBIC_ENTRADA:
                    area = AreaUbicacionDb.AutomatismoEntrada;
                    tipo = TipoUbicacionDb.AutomatismoMultiproducto;
                    break;
                case AutomatismoDb.TP_UBIC_TRANSITO:
                    area = AreaUbicacionDb.AutomatismoTransferencia;
                    tipo = TipoUbicacionDb.AutomatismoMultiproducto;
                    break;
                case AutomatismoDb.TP_UBIC_SALIDA:
                    area = AreaUbicacionDb.AutomatismoSalida;
                    tipo = TipoUbicacionDb.AutomatismoMultiproducto;
                    break;
                case AutomatismoDb.TP_UBIC_AJUSTES:
                    area = AreaUbicacionDb.AutomatismoAjuste;
                    tipo = TipoUbicacionDb.AutomatismoMultiproducto;
                    break;
                case AutomatismoDb.TP_UBIC_RECHAZOS:
                    area = AreaUbicacionDb.AutomatismoRechazo;
                    tipo = TipoUbicacionDb.AutomatismoMultiproducto;
                    break;
                default: throw new Exception("AUT100_Sec0_Err_TipoNoValidoParaElTipoDeAut");

            }

        }
    }
}
