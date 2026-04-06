using WIS.Persistence.Database;
using WIS.XmlData.WInterface.Models;

namespace WIS.XmlData.WInterface.Helpers
{
    public class ParamManager
    {
        public const string INTERFAZ_DS_REFERENCIA_UNICO = "INTERFAZ_DS_REFERENCIA_UNICO";
        public const string PARAM_USER = Domain.DataModel.Mappers.Constants.ParamManager.PARAM_USER;
        public const string PARAM_CLIENTE = Domain.DataModel.Mappers.Constants.ParamManager.PARAM_CLIENTE;
        public const string PARAM_EMPR = Domain.DataModel.Mappers.Constants.ParamManager.PARAM_EMPR;
        public const string GRUPO_CONSULTA = "GRUPO_CONSULTA";
        public const string IE_501_HABILITADA = "IE_501_HABILITADA";
        public const string IE_507_HABILITADA = "IE_507_HABILITADA";
        public const string IE_510_HABILITADA = "IE_510_HABILITADA";
        public const string IE_500_HABILITADA = "IE_500_HABILITADA";
        public const string IE_503_HABILITADA = "IE_503_HABILITADA";
        public const string IE_505_HABILITADA = "IE_505_HABILITADA";
        public const string IE_506_HABILITADA = "IE_506_HABILITADA";
        public const string IE_515_HABILITADA = "IE_515_HABILITADA";
        public const string IS_502_HABILITADA = "IS_502_HABILITADA";
        public const string IS_504_HABILITADA = "IS_504_HABILITADA";
        public const string IS_509_HABILITADA = "IS_509_HABILITADA";
        public const string IS_508_HABILITADA = "IS_508_HABILITADA";
        public const string IS_512_HABILITADA = "IS_512_HABILITADA";
        public const string IS_516_HABILITADA = "IS_516_HABILITADA";
        public const string IE_517_HABILITADA = "IE_517_HABILITADA";
        public const string IE_518_HABILITADA = "IE_518_HABILITADA";
        public const string IE_520_HABILITADA = "IE_520_HABILITADA";
        public const string IS_601_HABILITADA = "IS_601_HABILITADA";
        public const string IS_602_HABILITADA = "IS_602_HABILITADA";

        public static Object GetParamValue(WISDB context, string cd_param, List<KeyValuePair<string, string>> entities = null)
        {
            try
            {
                return GetParamValueInternal(context, cd_param, entities);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static Object GetParamValueInternal(WISDB context, string param_name, List<KeyValuePair<string, string>> entities = null)
        {
            var dbparams1 = (from pc in context.T_LPARAMETRO_CONFIGURACION
                             join n in context.T_LPARAMETRO_NIVEL on new { pc.CD_PARAMETRO, pc.DO_ENTIDAD_PARAMETRIZABLE }
                             equals new { n.CD_PARAMETRO, n.DO_ENTIDAD_PARAMETRIZABLE }
                             where pc.CD_PARAMETRO == param_name
                             select new
                             {
                                 cd_parametro = pc.CD_PARAMETRO,
                                 cd_entidad = pc.DO_ENTIDAD_PARAMETRIZABLE,
                                 do_entidad = pc.ND_ENTIDAD,
                                 pc.VL_PARAMETRO,
                                 nu_nivel = n.NU_NIVEL
                             }).OrderByDescending(o => o.nu_nivel);

            var dbparams = dbparams1.ToList();

            if (entities != null)
            {
                foreach (var p in dbparams)
                {
                    foreach (var e in entities)
                    {
                        if (e.Key == p.cd_entidad
                            && e.Value == p.do_entidad)
                        {
                            return p.VL_PARAMETRO;
                        }
                    }
                }
            }

            if (dbparams == null || dbparams.Count() == 0)
            {
                throw new CMNException("General_Sec0_Error_ParametroNoEncontrado", new string[] { param_name });
            }

            return dbparams.Last().VL_PARAMETRO;
        }
    }
}
