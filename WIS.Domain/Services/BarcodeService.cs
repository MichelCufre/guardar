using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.Auxiliares;
using WIS.Domain.Recepcion;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;

namespace WIS.Domain.Services
{
    public class BarcodeService : IBarcodeService
    {
        protected readonly IUnitOfWorkFactory _uowFactory;

        public BarcodeService(IUnitOfWorkFactory uowFactory)
        {
            _uowFactory = uowFactory;
        }

        public virtual EtiquetaLote GetEtiquetaLote(string cdBarras)
        {
            var prefix = BarcodeDb.PREFIX_RECEPCION;

            cdBarras = cdBarras?.ToUpper();

            if (!(cdBarras?.StartsWith(prefix) ?? false))
                return null;

            return new EtiquetaLote
            {
                NumeroExterno = cdBarras.Substring(prefix.Length),
                TipoEtiqueta = BarcodeDb.TP_ETIQUETA_REC,
                CodigoBarras = cdBarras,
            };
        }

        public virtual EtiquetaPosicionEquipo GetEtiquetaPosicionEquipo(string cdBarras)
        {
            var prefix = BarcodeDb.PREFIX_EQUIPO;

            cdBarras = cdBarras?.ToUpper();

            if (!(cdBarras?.StartsWith(prefix) ?? false))
                return null;

            var indexSeparador = cdBarras.LastIndexOf(GeneralDb.SeparadorEquipoPosicion);

            if (indexSeparador < 0 || indexSeparador - prefix.Length < 2)
                return null;

            var strEquipo = cdBarras.Substring(prefix.Length, indexSeparador - prefix.Length - 1);
            var strVerificador = cdBarras.Substring(indexSeparador - 1, 1);
            var strPosicion = cdBarras.Substring(indexSeparador + 1);

            if (!short.TryParse(strEquipo, out short equipo)
                || !int.TryParse(strVerificador, out int verificador)
                || !short.TryParse(strPosicion, out short posicion))
                return null;

            var modulo = BarcodeDb.NUM_MODULO_PALLET;

            if (GetDefaultVerifier(strEquipo, modulo) != verificador)
                return null;

            return new EtiquetaPosicionEquipo
            {
                Equipo = equipo,
                Posicion = posicion,
                NumeroExterno = cdBarras.Substring(prefix.Length),
                Tipo = TipoEtiquetaTransferencia.Equipo,
            };
        }

        public virtual string GenerateBarcode(string nroEtiqueta, string tipoEtiqueta, string predio = null, string anexoEtiqueta = null, string prefijo = "")
        {
            tipoEtiqueta = GetPrefijoTipo(tipoEtiqueta);

            var barrCode = string.Empty;
            if (nroEtiqueta != null)
            {
                barrCode = PrefixFromCode(tipoEtiqueta, predio, prefijo) + nroEtiqueta.PadLeft(LenghtFromCode(tipoEtiqueta), BarcodeDb.PADDING_CHAR);

                if (tipoEtiqueta != BarcodeDb.TIPO_ET_EQUIPO_MANUAL)
                    barrCode += GenerateVerifier(nroEtiqueta, tipoEtiqueta);
            }

            return string.Format("{0}{1}", barrCode, anexoEtiqueta);
        }

        public virtual bool ValidarEtiquetaContenedor(string codigoBarras, int userId, out AuxContenedor datosContenedor, out int cantidadEmpresa, string cdCliente = null, int? cdEmpresa = null)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var formatoValido = false;
            var preValidaCliente = false;
            codigoBarras = codigoBarras.ToUpper();

            datosContenedor = new AuxContenedor(codigoBarras);

            if (string.IsNullOrEmpty(codigoBarras))
                throw new ValidationFailedException("General_Sec0_Error_COL80");

            var contenedor = uow.ContenedorRepository.GetContenedorByCodigoBarrasLpn(codigoBarras, userId, cdEmpresa, out cantidadEmpresa, out bool isBarraLpn);

            if (!isBarraLpn && contenedor == null)
                contenedor = uow.ContenedorRepository.GetContenedorByCodigoBarras(codigoBarras);

            if (contenedor != null)
            {
                datosContenedor.NuContenedor = contenedor.Numero;
                datosContenedor.TipoContenedor = contenedor.TipoContenedor;
                datosContenedor.NuPreparacion = contenedor.NumeroPreparacion;
                datosContenedor.Estado = contenedor.Estado;
                datosContenedor.Ubicacion = contenedor.Ubicacion;
                datosContenedor.IdExternoContenedor = contenedor.IdExterno;
                datosContenedor.NroLpn = contenedor.NroLpn;
                datosContenedor.ExisteContenedorActivo = true;
            }
            else
            {
                if (codigoBarras.Length < BarcodeDb.LENGHT_MIN_CB_CONT)
                    throw new ValidationFailedException("General_Sec0_Error_BarrasLargoMinimo", new string[] { BarcodeDb.LENGHT_MIN_CB_CONT.ToString() });

                switch (codigoBarras.Substring(0, 5))
                {
                    case BarcodeDb.PREFIX_CONT_TIPO_W:
                        datosContenedor.TipoContenedor = BarcodeDb.TIPO_CONTENEDOR_W;
                        datosContenedor.IdExternoContenedor = GetNumberWithoutVerifier(codigoBarras.Substring(5), GetPrefijoTipo(datosContenedor.TipoContenedor))?.ToString();

                        if (string.IsNullOrEmpty(datosContenedor.IdExternoContenedor))
                            throw new ValidationFailedException("General_Sec0_Error_COL81");

                        break;
                    case BarcodeDb.PREFIX_CONT_TIPO_C:
                        preValidaCliente = true;
                        datosContenedor.TipoContenedor = BarcodeDb.TIPO_CONTENEDOR_C;
                        datosContenedor.IdExternoContenedor = GetNumberWithoutVerifier(codigoBarras.Substring(5), GetPrefijoTipo(datosContenedor.TipoContenedor))?.ToString();

                        if (string.IsNullOrEmpty(datosContenedor.IdExternoContenedor))
                            throw new ValidationFailedException("General_Sec0_Error_COL81");

                        break;
                    default:

                        var lpn = uow.ManejoLpnRepository.GetLpnByCodigoBarras(codigoBarras, cdEmpresa);
                        if (lpn != null)
                        {
                            datosContenedor.TipoContenedor = lpn.Tipo;
                            datosContenedor.IdExternoContenedor = lpn.IdExterno;
                            datosContenedor.NroLpn = lpn.NumeroLPN;
                            formatoValido = true;
                        }
                        else
                        {
                            datosContenedor.TipoContenedor = codigoBarras.Substring(0, 1);
                            var idExterno = GetNumberWithoutVerifier(codigoBarras, GetPrefijoTipo(datosContenedor.TipoContenedor))?.ToString();

                            if (!string.IsNullOrEmpty(idExterno) && datosContenedor.TipoContenedor == BarcodeDb.TIPO_CONTENEDOR_GRANDE)
                            {
                                datosContenedor.TipoContenedor = codigoBarras.Substring(0, 1);
                                datosContenedor.IdExternoContenedor = idExterno;
                                formatoValido = true;
                            }
                            else if (!string.IsNullOrEmpty(idExterno) && ((int.Parse(codigoBarras.Substring(1, codigoBarras.Length - 1)) % 7) == int.Parse(codigoBarras.Substring(codigoBarras.Length - 1))))
                            {
                                //Contenedor DHL
                                preValidaCliente = true;
                                datosContenedor.TipoContenedor = BarcodeDb.TIPO_CONTENEDOR_D;
                                datosContenedor.IdExternoContenedor = codigoBarras;
                                formatoValido = true;
                            }
                            else if (!string.IsNullOrEmpty(idExterno))
                            {
                                datosContenedor.TipoContenedor = codigoBarras.Substring(0, 1);
                                datosContenedor.IdExternoContenedor = codigoBarras.Substring(0, codigoBarras.Length - 1);
                                formatoValido = true;
                            }
                        }

                        if (!formatoValido)
                            throw new ValidationFailedException("General_Sec0_Error_COL82");

                        break;
                }

                contenedor = uow.ContenedorRepository.GetContenedorByIdExternoTipo(datosContenedor.IdExternoContenedor, datosContenedor.TipoContenedor);

                if (contenedor != null)
                {
                    datosContenedor.NuContenedor = contenedor.Numero;
                    datosContenedor.NuPreparacion = contenedor.NumeroPreparacion;
                    datosContenedor.Estado = contenedor.Estado;
                    datosContenedor.Ubicacion = contenedor.Ubicacion;
                    datosContenedor.NroLpn = contenedor.NroLpn;
                    datosContenedor.ExisteContenedorActivo = true;
                }

                if (preValidaCliente && !string.IsNullOrEmpty(cdCliente))
                {
                    var contPredefinido = uow.ContenedorRepository.GetContenedorPredefinidoByIdExternoTipo(datosContenedor.IdExternoContenedor, datosContenedor.TipoContenedor);
                    if (contPredefinido == null)
                        throw new ValidationFailedException("General_Sec0_Error_COL83");
                    else if (contPredefinido.CodigoCliente != cdCliente || contPredefinido.CodigoEmpresa != cdEmpresa)
                        throw new ValidationFailedException("General_Sec0_Error_COL84", new string[] { contPredefinido.CodigoCliente });
                }
            }

            return true;
        }

        #region Auxs

        public virtual string GetPrefijoTipo(string tipoEtiqueta)
        {
            switch (tipoEtiqueta)
            {
                case BarcodeDb.TIPO_CONTENEDOR_W:
                    return BarcodeDb.TIPO_ET_CON_W;
                case BarcodeDb.TIPO_CONTENEDOR_C:
                    return BarcodeDb.TIPO_ET_CON_C;
                case BarcodeDb.TIPO_CONTENEDOR_T:
                    return BarcodeDb.TIPO_ET_CON_T;
                case BarcodeDb.TIPO_CONTENEDOR_D:
                    return BarcodeDb.TIPO_ET_CON_D;
                case BarcodeDb.TIPO_CONTENEDOR_GRANDE:
                    return BarcodeDb.TIPO_ET_CON_GRA;
                case BarcodeDb.TIPO_CONTENEDOR_CHICO:
                    return BarcodeDb.TIPO_ET_CON_CHI;
                case BarcodeDb.TIPO_CONTENEDOR_BULTO:
                    return BarcodeDb.TIPO_ET_CON_BUL;
                case BarcodeDb.TIPO_CONTENEDOR_MEDIANO:
                    return BarcodeDb.TIPO_ET_CON_MED;
                case BarcodeDb.TIPO_CONTENEDOR_VIRTUAL:
                    return BarcodeDb.TIPO_ET_CON_VIR;
                case BarcodeDb.TIPO_CONTENEDOR_V:
                    return BarcodeDb.TIPO_ET_CON_V;
                default:
                    return tipoEtiqueta;
            }
        }

        public virtual string PrefixFromCode(string prefixCode, string predio, string prefijo = "")
        {

            switch (prefixCode)
            {
                case BarcodeDb.TIPO_ET_RECEPCION:
                    prefijo = BarcodeDb.PREFIX_RECEPCION;
                    break;
                case BarcodeDb.TIPO_ET_TRANFERENCIA:
                    prefijo = BarcodeDb.PREFIX_TRANSFERENCIA;
                    break;
                case BarcodeDb.TIPO_ET_PALLET:
                    prefijo = BarcodeDb.PREFIX_PALLET;
                    break;
                case BarcodeDb.TIPO_ET_CON_W:
                    prefijo = BarcodeDb.PREFIX_CONT_TIPO_W;
                    break;
                case BarcodeDb.TIPO_ET_CON_T:
                    prefijo = BarcodeDb.PREFIX_CONT_TIPO_T;
                    break;
                case BarcodeDb.TIPO_ET_CON_D:
                    prefijo = BarcodeDb.PREFIX_CONT_TIPO_D;
                    break;
                case BarcodeDb.TIPO_ET_CON_C:
                    prefijo = BarcodeDb.PREFIX_CONT_TIPO_C;
                    break;
                case BarcodeDb.TIPO_ET_CON_V:
                    prefijo = BarcodeDb.PREFIX_CONT_TIPO_V;
                    break;
                case BarcodeDb.TIPO_ET_UBICEQUIPO_FUN:
                    prefijo = string.Format("{0}{1}", predio, BarcodeDb.PREFIX_UBICEQUIPO_TIPO_FUNC);
                    break;
                case BarcodeDb.TIPO_ET_EQUIPO_MANUAL:
                    prefijo = string.Format("{0}{1}", predio, prefijo = string.IsNullOrEmpty(prefijo) ? BarcodeDb.TIPO_ET_EQUIPO_MANUAL : prefijo);
                    break;
                case BarcodeDb.TIPO_ET_UBICEQUIPO_EQI:
                    prefijo = string.Format("{0}{1}", predio, BarcodeDb.PREFIX_UBICEQUIPO_TIPO_EQI);
                    break;
                case BarcodeDb.TIPO_ET_EQUIPO:
                    prefijo = BarcodeDb.PREFIX_EQUIPO;
                    break;
                case BarcodeDb.TIPO_ET_CON_GRA:
                    prefijo = BarcodeDb.PREFIX_CONT_TIPO_GRA;
                    break;
                case BarcodeDb.TIPO_ET_CON_CHI:
                    prefijo = BarcodeDb.PREFIX_CONT_TIPO_CHI;
                    break;
                case BarcodeDb.TIPO_ET_CON_BUL:
                    prefijo = BarcodeDb.PREFIX_CONT_TIPO_BUL;
                    break;
                case BarcodeDb.TIPO_ET_CON_VIR:
                    prefijo = BarcodeDb.PREFIX_CONT_TIPO_VIR;
                    break;
                case BarcodeDb.TIPO_ET_CON_MED:
                    prefijo = BarcodeDb.PREFIX_CONT_TIPO_MED;
                    break;
                case BarcodeDb.TIPO_ET_UT:
                    prefijo = BarcodeDb.PREFIX_UT;
                    break;
            }
            return prefijo;
        }

        public virtual int LenghtFromCode(string tipoEtiqueta)
        {
            int lenght = 6;
            switch (tipoEtiqueta)
            {
                case BarcodeDb.TIPO_ET_RECEPCION:
                    lenght = BarcodeDb.LENGHT_RECEPCION;
                    break;
                case BarcodeDb.TIPO_ET_TRANFERENCIA:
                    lenght = BarcodeDb.LENGHT_TRANFERENCIA;
                    break;
                case BarcodeDb.TIPO_ET_PALLET:
                    lenght = BarcodeDb.LENGHT_PALLET;
                    break;
                case BarcodeDb.TIPO_ET_CON_W:
                    lenght = BarcodeDb.LENGHT_CON_W;
                    break;
                case BarcodeDb.TIPO_ET_CON_T:
                    lenght = BarcodeDb.LENGHT_CON_T;
                    break;
                case BarcodeDb.TIPO_ET_CON_D:
                    lenght = BarcodeDb.LENGHT_CON_D;
                    break;
                case BarcodeDb.TIPO_ET_CON_C:
                    lenght = BarcodeDb.LENGHT_CON_C;
                    break;
                case BarcodeDb.TIPO_ET_CON_V:
                    lenght = BarcodeDb.LENGHT_CON_V;
                    break;
                case BarcodeDb.TIPO_ET_UBICEQUIPO_FUN:
                    lenght = BarcodeDb.LENGHT_UBICEQUIPO_FUN;
                    break;
                case BarcodeDb.TIPO_ET_UBICEQUIPO_EQI:
                    lenght = BarcodeDb.LENGHT_UBICEQUIPO_QUI;
                    break;
                case BarcodeDb.TIPO_ET_EQUIPO:
                    lenght = BarcodeDb.LENGHT_EQUIPO;
                    break;
                case BarcodeDb.TIPO_ET_CON_GRA:
                    lenght = BarcodeDb.LENGHT_ET_GRA;
                    break;
                case BarcodeDb.TIPO_ET_CON_CHI:
                    lenght = BarcodeDb.LENGHT_ET_CHI;
                    break;
                case BarcodeDb.TIPO_ET_CON_MED:
                    lenght = BarcodeDb.LENGHT_ET_MDA;
                    break;
                case BarcodeDb.TIPO_ET_CON_BUL:
                    lenght = BarcodeDb.LENGHT_ET_BUL;
                    break;
                case BarcodeDb.TIPO_ET_CON_VIR:
                    lenght = BarcodeDb.LENGHT_ET_VIR;
                    break;
            }
            return lenght;
        }

        public virtual int ModFromCode(string tipoEtiqueta)
        {
            int modulo = 10;
            switch (tipoEtiqueta)
            {
                case BarcodeDb.TIPO_ET_RECEPCION:
                    modulo = BarcodeDb.NUM_MODULO_RECEPCION;
                    break;
                case BarcodeDb.TIPO_ET_TRANFERENCIA:
                    modulo = BarcodeDb.NUM_MODULO_TRANFERENCIA;
                    break;
                case BarcodeDb.TIPO_ET_PALLET:
                    modulo = BarcodeDb.NUM_MODULO_PALLET;
                    break;
                case BarcodeDb.TIPO_ET_CON_W:
                    modulo = BarcodeDb.NUM_MODULO_CON_W;
                    break;
                case BarcodeDb.TIPO_ET_CON_T:
                    modulo = BarcodeDb.NUM_MODULO_CON_T;
                    break;
                case BarcodeDb.TIPO_ET_CON_D:
                    modulo = BarcodeDb.NUM_MODULO_CON_D;
                    break;
                case BarcodeDb.TIPO_ET_CON_C:
                    modulo = BarcodeDb.NUM_MODULO_CON_C;
                    break;
                case BarcodeDb.TIPO_ET_EQUIPO:
                    modulo = BarcodeDb.NUM_MODULO_EQUIPO;
                    break;
                case BarcodeDb.TIPO_ET_CON_GRA:
                    modulo = BarcodeDb.NUM_MODULO_ET_GRA;
                    break;
                case BarcodeDb.TIPO_ET_CON_CHI:
                    modulo = BarcodeDb.NUM_MODULO_ET_CHI;
                    break;
                case BarcodeDb.TIPO_ET_CON_MED:
                    modulo = BarcodeDb.NUM_MODULO_ET_MDA;
                    break;
                case BarcodeDb.TIPO_ET_CON_BUL:
                    modulo = BarcodeDb.NUM_MODULO_ET_BUL;
                    break;
            }
            return modulo;
        }

        public virtual int GetDefaultVerifier(string numero, int modulo)
        {
            int suma = 0;

            numero.ToCharArray().ToList().ForEach(s => { suma += int.Parse(s.ToString()); });

            return suma % modulo;
        }

        public virtual int? GenerateVerifier(string nro, string tipoEtiqueta)
        {
            var modulo = ModFromCode(tipoEtiqueta);

            int? aux = null;
            if (int.TryParse(nro, out int number))
            {
                int suma = 0;
                if (tipoEtiqueta == BarcodeDb.TIPO_ET_CON_T)
                {
                    suma = int.Parse(nro.Substring(1, 1)) * 8 +
                    int.Parse(nro.Substring(1, 2)) * 6 +
                    int.Parse(nro.Substring(1, 3)) * 4 +
                    int.Parse(nro.Substring(1, 4)) * 2 +
                    int.Parse(nro.Substring(1, 5)) * 3 +
                    int.Parse(nro.Substring(1, 6)) * 5 +
                    int.Parse(nro.Substring(1, 7)) * 9 +
                    int.Parse(nro.Substring(1, 8)) * 7;

                    aux = (suma % modulo);

                    if (aux == 0)
                        aux = 5;
                    if (aux == 1)
                        aux = 0;

                    aux = 11 - aux;
                }
                else if (tipoEtiqueta == BarcodeDb.TIPO_ET_PALLET)
                {
                    nro.ToCharArray().ToList().ForEach(s => { suma += int.Parse(s.ToString()); });
                    aux = (suma % modulo);
                }
                else if (tipoEtiqueta == BarcodeDb.TIPO_ET_CON_W || tipoEtiqueta == BarcodeDb.TIPO_ET_CON_V || tipoEtiqueta == BarcodeDb.TIPO_ET_CON_C ||
                         tipoEtiqueta == BarcodeDb.TIPO_ET_CON_GRA || tipoEtiqueta == BarcodeDb.TIPO_ET_CON_CHI || tipoEtiqueta == BarcodeDb.TIPO_ET_CON_BUL ||
                         tipoEtiqueta == BarcodeDb.TIPO_ET_CON_VIR || tipoEtiqueta == BarcodeDb.TIPO_ET_CON_MED)
                {
                    nro.ToCharArray().ToList().ForEach(s => { suma += int.Parse(s.ToString()); });
                    aux = (suma % modulo);
                }
                else if (tipoEtiqueta != "TRA" && tipoEtiqueta != "REC")
                {
                    nro.ToCharArray().ToList().ForEach(s => { suma += int.Parse(s.ToString()); });
                    aux = (suma % modulo);
                }
            }

            return aux;
        }

        public virtual int? GetNumberWithoutVerifier(string strCodificado, string tipoEtiqueta)
        {
            // Obtengo el digito verificador del numero
            // Y Por otro lado agarro el numero real y recalculo el digito verificado
            // si coinciden devuelve el numero sin el digito de Verificación si no -1

            var strNoCodificado = strCodificado.Substring(0, strCodificado.Length - 1);
            var verificadorEnviado = strCodificado.Substring(strCodificado.Length - 1);
            var verificadorCalculado = GenerateVerifier(strNoCodificado, tipoEtiqueta);
            if (int.TryParse(verificadorEnviado, out int enviado) &&
                verificadorCalculado.HasValue &&
                verificadorCalculado.Value == enviado)
                return int.Parse(strNoCodificado);
            else
                return null;
        }

        #endregion
    }
}
