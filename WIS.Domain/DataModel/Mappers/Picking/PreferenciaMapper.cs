using WIS.Domain.Picking;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers.Picking
{
    public class PreferenciaMapper : Mapper
    {
        public virtual Preferencia MapToObject(T_PREFERENCIA p)
        {
            return p == null ? null : new Preferencia
            {
                NU_PREFERENCIA = p.NU_PREFERENCIA,
                DS_PREFERENCIA = p.DS_PREFERENCIA,
                NU_PREDIO = p.NU_PREDIO,
                ID_BLOQUE_MIN = p.ID_BLOQUE_MIN,
                ID_BLOQUE_MAX = p.ID_BLOQUE_MAX,
                ID_CALLE_MIN = p.ID_CALLE_MIN,
                ID_CALLE_MAX = p.ID_CALLE_MAX,
                NU_COLUMNA_MIN = p.NU_COLUMNA_MIN,
                NU_COLUMNA_MAX = p.NU_COLUMNA_MAX,
                NU_ALTURA_MIN = p.NU_ALTURA_MIN,
                NU_ALTURA_MAX = p.NU_ALTURA_MAX,
                PS_BRUTO_MAXIMO = p.PS_BRUTO_MAXIMO,
                VL_CUBAGEM_MAXIMO = p.VL_CUBAGEM_MAXIMO,
                QT_CLIENTES = p.QT_CLIENTES,
                QT_PEDIDOS = p.QT_PEDIDOS,
                QT_MAXIMO_PICKEOS = p.QT_MAXIMO_PICKEOS,
                QT_MAXIMO_UNIDADES = p.QT_MAXIMO_UNIDADES,
                FL_HABILITADO_EMPRESA = p.FL_HABILITADO_EMPRESA,
                FL_HABILITADO_CLIENTE = p.FL_HABILITADO_CLIENTE,
                FL_HABILITADO_RUTA = p.FL_HABILITADO_RUTA,
                FL_HABILITADO_ZONA = p.FL_HABILITADO_ZONA,
                FL_HABILITADO_COND_LIBERACION = p.FL_HABILITADO_COND_LIBERACION,
                FL_HABILITADO_TP_PEDIDO = p.FL_HABILITADO_TP_PEDIDO,
                FL_HABILITADO_TP_EXPEDICION = p.FL_HABILITADO_TP_EXPEDICION,
                FL_HABILITADO_CLASE = p.FL_HABILITADO_CLASE,
                FL_HABILITADO_FAMILIA = p.FL_HABILITADO_FAMILIA,
                FL_HABILITADO_CONT_ACCESO = p.FL_HABILITADO_CONT_ACCESO,
                FL_HABILITADO_RANGO_UBIC = p.FL_HABILITADO_RANGO_UBIC,
                FL_HABILITADO_PEDIDO_COMPLETO = p.FL_HABILITADO_PEDIDO_COMPLETO,
                FL_HABILITADO_LIB_COMPLETO = p.FL_HABILITADO_LIB_COMPLETO,
                DT_ADDROW = p.DT_ADDROW,
                DT_UPDROW = p.DT_UPDROW,
            };
        }

        public virtual T_PREFERENCIA MapToEntity(Preferencia p)
        {
            return p == null ? null : new T_PREFERENCIA
            {
                NU_PREFERENCIA = p.NU_PREFERENCIA,
                DS_PREFERENCIA = p.DS_PREFERENCIA,
                NU_PREDIO = p.NU_PREDIO,
                ID_BLOQUE_MIN = p.ID_BLOQUE_MIN,
                ID_BLOQUE_MAX = p.ID_BLOQUE_MAX,
                ID_CALLE_MIN = p.ID_CALLE_MIN,
                ID_CALLE_MAX = p.ID_CALLE_MAX,
                NU_COLUMNA_MIN = p.NU_COLUMNA_MIN,
                NU_COLUMNA_MAX = p.NU_COLUMNA_MAX,
                NU_ALTURA_MIN = p.NU_ALTURA_MIN,
                NU_ALTURA_MAX = p.NU_ALTURA_MAX,
                PS_BRUTO_MAXIMO = p.PS_BRUTO_MAXIMO,
                VL_CUBAGEM_MAXIMO = p.VL_CUBAGEM_MAXIMO,
                QT_CLIENTES = p.QT_CLIENTES,
                QT_PEDIDOS = p.QT_PEDIDOS,
                QT_MAXIMO_PICKEOS = p.QT_MAXIMO_PICKEOS,
                QT_MAXIMO_UNIDADES = p.QT_MAXIMO_UNIDADES,
                FL_HABILITADO_EMPRESA = p.FL_HABILITADO_EMPRESA,
                FL_HABILITADO_CLIENTE = p.FL_HABILITADO_CLIENTE,
                FL_HABILITADO_RUTA = p.FL_HABILITADO_RUTA,
                FL_HABILITADO_ZONA = p.FL_HABILITADO_ZONA,
                FL_HABILITADO_COND_LIBERACION = p.FL_HABILITADO_COND_LIBERACION,
                FL_HABILITADO_TP_PEDIDO = p.FL_HABILITADO_TP_PEDIDO,
                FL_HABILITADO_TP_EXPEDICION = p.FL_HABILITADO_TP_EXPEDICION,
                FL_HABILITADO_CLASE = p.FL_HABILITADO_CLASE,
                FL_HABILITADO_FAMILIA = p.FL_HABILITADO_FAMILIA,
                FL_HABILITADO_CONT_ACCESO = p.FL_HABILITADO_CONT_ACCESO,
                FL_HABILITADO_RANGO_UBIC = p.FL_HABILITADO_RANGO_UBIC,
                FL_HABILITADO_PEDIDO_COMPLETO = p.FL_HABILITADO_PEDIDO_COMPLETO,
                FL_HABILITADO_LIB_COMPLETO = p.FL_HABILITADO_LIB_COMPLETO,
                DT_ADDROW = p.DT_ADDROW,
                DT_UPDROW = p.DT_UPDROW,
            };
        }

    }
}

