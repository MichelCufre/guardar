using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class TransaccionMapper :Mapper
    {
        public TransaccionMapper()
        {

        }
        public virtual Transaccion MapToObject(T_TRANSACCION transaccionEntity)
        {
            return new Transaccion
            {
                NumeroTransaccion = transaccionEntity.NU_TRANSACCION,
                DescripcionTransaccion = transaccionEntity.DS_TRANSACCION,
                CodigoAplicacion = transaccionEntity.CD_APLICACION,
                CodigoFuncionario = transaccionEntity.CD_FUNCIONARIO,
                Data = transaccionEntity.VL_DATA,
                FechaAlta = transaccionEntity.DT_ADDROW,
                FechaFinTransaccion = transaccionEntity.DT_ADDROW_FIN_TRAN,


            };
        }

        public virtual T_TRANSACCION MapToEntity(Transaccion transaccion)
        {
            return new T_TRANSACCION
            {
                NU_TRANSACCION = transaccion.NumeroTransaccion,
                DS_TRANSACCION = transaccion.DescripcionTransaccion,
                CD_APLICACION = transaccion.CodigoAplicacion,
                CD_FUNCIONARIO = transaccion.CodigoFuncionario,
                VL_DATA = transaccion.Data,
                DT_ADDROW = transaccion.FechaAlta,
                DT_ADDROW_FIN_TRAN = transaccion.FechaFinTransaccion,

            };
        }
    }
}
