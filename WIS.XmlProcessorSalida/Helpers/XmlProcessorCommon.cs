using Microsoft.EntityFrameworkCore;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.API.Dtos.Salida;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.XmlProcessorSalida.Helpers
{
    public class XmlProcessorCommon
    {
        public static int MaxSizePerPackage = 1048576;

        public virtual bool CrearEjecucionSalida(WISDB context, IDapper dapper, int userId, int empresa, EjecucionPendienteResponse salida, string xml)
        {
            var idRequest = $"WISIS{salida.NumeroInterfazEjecucion}";
            var existeInterfazJson = context.T_INTERFAZ_EJECUCION
                .AsNoTracking()
                .Any(ie => ie.CD_EMPRESA == empresa
                    && ie.ID_REQUEST == idRequest);

            if (!existeInterfazJson)
            {
                var nuevaInterfaz = new T_INTERFAZ_EJECUCION();
                var nu_interfaz_ejecucion = BLSecuencia.GetNextValSecIntfc(context, dapper);
                var parametro = GetParamUsuario(context, userId, "");

                nuevaInterfaz.NU_INTERFAZ_EJECUCION = nu_interfaz_ejecucion;
                nuevaInterfaz.CD_INTERFAZ_EXTERNA = salida.CodigoInterfazExterna;
                nuevaInterfaz.DS_REFERENCIA = $"Ejecución Interna: {salida.NumeroInterfazEjecucion}";
                nuevaInterfaz.FL_ERROR_CARGA = "N";
                nuevaInterfaz.FL_ERROR_PROCEDIMIENTO = "N";
                nuevaInterfaz.DT_COMIENZO = DateTime.Now;
                nuevaInterfaz.DT_SITUACAO = DateTime.Now;
                nuevaInterfaz.CD_SITUACAO = SituacionDb.ProcesandoInterfaz;
                nuevaInterfaz.CD_EMPRESA = empresa;
                nuevaInterfaz.USERID = userId;
                nuevaInterfaz.CD_GRUPO_CONSULTA = parametro;
                nuevaInterfaz.ID_REQUEST = idRequest;

                var tHelper = new TransactionHelper();

                using (var tran = tHelper.BeginTransaction(context))
                {
                    context.T_INTERFAZ_EJECUCION.Add(nuevaInterfaz);
                    context.SaveChanges();

                    CargarDatosEjecucion(context, nuevaInterfaz, xml);

                    context.SaveChanges();
                    tran.Commit();
                }

                return true;
            }

            return false;
        }

        protected virtual string GetParamUsuario(WISDB context, int userId, string cliente)
        {
            List<KeyValuePair<string, string>> colParams = new List<KeyValuePair<string, string>>();
            string codigoFuncionario = ParamManager.PARAM_USER + "_" + userId;

            cliente = ParamManager.PARAM_CLIENTE + "_" + userId;

            colParams.Add(new KeyValuePair<string, string>(ParamManager.PARAM_USER, codigoFuncionario));
            colParams.Add(new KeyValuePair<string, string>(ParamManager.PARAM_CLIENTE, cliente));

            return (string)ParamManager.GetParamValue(context, ParamManager.GRUPO_CONSULTA, colParams);
        }

        protected virtual void CargarDatosEjecucion(WISDB context, T_INTERFAZ_EJECUCION ejecucion, string xml)
        {
            var dataChunks = SplitPackage(xml, XmlProcessorCommon.MaxSizePerPackage);
            var fullData = ByteString.GetBytes(xml);

            context.T_INTERFAZ_EJECUCION_DATA.Add(new T_INTERFAZ_EJECUCION_DATA()
            {
                NU_INTERFAZ_EJECUCION = ejecucion.NU_INTERFAZ_EJECUCION,
                DT_ADDROW = DateTime.Now,
                DATA = fullData
            });

            context.T_INTERFAZ_EJECUCION_DATEXT.Add(new T_INTERFAZ_EJECUCION_DATEXT()
            {
                NU_INTERFAZ_EJECUCION = ejecucion.NU_INTERFAZ_EJECUCION,
                CD_EMPRESA = ejecucion.CD_EMPRESA ?? -1,
                NU_TOTAL_PAQUETES = (short)dataChunks.Count,
                FINALIZA_EJECUCION = "N"
            });

            short pkgNum = 1;
            foreach (var chunk in dataChunks)
            {
                context.T_INTERFAZ_EJECUCION_DATEXTDET.Add(new T_INTERFAZ_EJECUCION_DATEXTDET()
                {
                    NU_INTERFAZ_EJECUCION = ejecucion.NU_INTERFAZ_EJECUCION,
                    NU_PAQUETE = pkgNum,
                    DATA = chunk,
                    DT_ADDROW = DateTime.Now
                });

                pkgNum++;
            }
        }

        protected virtual List<byte[]> SplitPackage(string xml, int maxPackageSize)
        {
            string encode64 = Encrypter.EncodeBase64(xml);

            byte[] bytes = ByteString.GetBytes(encode64);

            List<byte[]> list = new List<byte[]>();

            if (bytes.Length > maxPackageSize)
            {
                list = ByteString.SplitByteArray(encode64, maxPackageSize);
            }
            else
            {
                list.Add(bytes);
            }

            return list;
        }
    }
}
