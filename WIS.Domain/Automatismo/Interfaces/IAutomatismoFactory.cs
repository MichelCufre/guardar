namespace WIS.Domain.Automatismo.Interfaces
{
    public interface IAutomatismoFactory
    {
        IAutomatismo Create(string tipo);
    }
}
