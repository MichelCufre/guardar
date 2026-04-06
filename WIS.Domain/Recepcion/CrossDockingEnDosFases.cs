using WIS.Domain.DataModel.Mappers.Constants;

namespace WIS.Domain.Recepcion
{
    public class CrossDockingEnDosFases : CrossDocking
    {
        public CrossDockingEnDosFases(): base()
        {
            this.Tipo = TipoCrossDockingDb.SegundaFase;
        }
    }
}
