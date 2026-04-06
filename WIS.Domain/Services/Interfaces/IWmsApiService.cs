using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Services.Interfaces
{
    public interface IWmsApiService
    {
        bool IsEnabled();
        string CallService(string method, string content);
    }
}
