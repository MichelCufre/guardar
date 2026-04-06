namespace WIS.Domain.Reportes
{
    public interface IReportKeyService
    {
        string ResolveKey(params string[] keys);
    }
}
