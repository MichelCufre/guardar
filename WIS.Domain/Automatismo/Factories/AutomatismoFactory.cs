using WIS.Domain.Automatismo.Constants;
using WIS.Domain.Automatismo.Interfaces;

namespace WIS.Domain.Automatismo.Factories
{
    public class AutomatismoFactory : IAutomatismoFactory
    {
        public virtual IAutomatismo Create(string tipo)
        {
            switch (tipo)
            {
                case AutomatismoTipo.AutoStore:
                    return new AutomatismoAutoStore();

                case AutomatismoTipo.Vertical:
                    return new AutomatismoVertical();

                case AutomatismoTipo.Ptl:
                    return new AutomatismoPtl();
            }

            return null;
        }
    }
}
