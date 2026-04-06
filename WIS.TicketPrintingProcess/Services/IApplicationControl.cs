using System.Threading.Tasks;

namespace WIS.TicketPrintingProcess.Services
{
    public interface IApplicationControl
    {
        Task StartAsync();
    }
}