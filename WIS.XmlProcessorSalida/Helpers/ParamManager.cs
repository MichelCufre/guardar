using WIS.Persistence.Database;

namespace WIS.XmlProcessorSalida.Helpers
{
    public class ParamManager
    {
        public const string PARAM_USER = Domain.DataModel.Mappers.Constants.ParamManager.PARAM_USER;
        public const string PARAM_CLIENTE = Domain.DataModel.Mappers.Constants.ParamManager.PARAM_CLIENTE;
        public const string GRUPO_CONSULTA = "GRUPO_CONSULTA";
        public const string LIMIT_PACK_WS = "LIMIT_PACK_WS";

        public static Object GetParamValue(WISDB context, string cd_param, List<KeyValuePair<string, string>> entities = null)
        {
            try
            {
                return GetParamValueInternal(context, cd_param, entities);
            }
            catch
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
                throw new Exception($"Parámetro {param_name} no encontrado");
            }

            return dbparams.Last().VL_PARAMETRO;
        }
    }
}
