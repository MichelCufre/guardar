using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using NLog.Fluent;
using System.Reflection;
using WIS.XmlData.WInterface.Models;

namespace WIS.XmlData.WInterface.Helpers
{
    public class CSManager
    {
        protected readonly ILogger _logger;
        protected readonly IConfiguration _configuration;

        public CSManager(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public CSTransfer InvokeBackEndCustomMethod(ECSBackEndMethods method, CSTransfer transfer = null)
        {
            string _AssemblyName = "WIS.CS.BackEnd.";
            string _NameSpace = "WIS.CS.BackEnd.";
            string _ClassName = "CustomBackEnd";

            return InvokeCustomMethod(_AssemblyName, _NameSpace, _ClassName, method.ToString(), transfer);
        }

        public CSTransfer InvokeBillingCustomMethod(ECSBilling method, CSTransfer transfer = null)
        {
            string _AssemblyName = "WIS.CS.Billing.";
            string _NameSpace = "WIS.CS.Billing.";
            string _ClassName = "CustomBilling";
            return InvokeCustomMethod(_AssemblyName, _NameSpace, _ClassName, method.ToString(), transfer);
        }

        public CSTransfer InvokeFrontEndCustomMethod(ECSFrontEndMethods method, CSTransfer transfer = null)
        {
            string _AssemblyName = "WIS.CS.FrontEnd.";
            string _NameSpace = "WIS.CS.FrontEnd.";
            string _ClassName = "CustomFrontEnd";
            return InvokeCustomMethod(_AssemblyName, _NameSpace, _ClassName, method.ToString(), transfer);
        }

        public CSTransfer InvokeInterfacesCustomMethod(ECSInterfacesMethods method, CSTransfer transfer = null)
        {
            string _AssemblyName = "WIS.CS.Interfaces.";
            string _NameSpace = "WIS.CS.Interfaces.";
            string _ClassName = "CustomInterfaces";
            return InvokeCustomMethod(_AssemblyName, _NameSpace, _ClassName, method.ToString(), transfer);
        }

        public CSTransfer InvokeReportCustomMethod(ECSReportMethods method, CSTransfer transfer = null)
        {
            string _AssemblyName = "WIS.CS.Report.";
            string _NameSpace = "WIS.CS.Report.";
            string _ClassName = "CustomReport";
            return InvokeCustomMethod(_AssemblyName, _NameSpace, _ClassName, method.ToString(), transfer);
        }

        public CSTransfer InvokeTrackingCustomMethod(ECSTrackingMethods method, CSTransfer transfer = null)
        {
            string _AssemblyName = "WIS.CS.Tracking.";
            string _NameSpace = "WIS.CS.Tracking.";
            string _ClassName = "CustomTracking";
            return InvokeCustomMethod(_AssemblyName, _NameSpace, _ClassName, method.ToString(), transfer);
        }

        public CSTransfer InvokeCanjesCustomMethod(ECSCanjesMethods method, CSTransfer transfer = null)
        {
            string _AssemblyName = "WIS.CS.Backend.";
            string _NameSpace = "WIS.CS.Backend.";
            string _ClassName = "BackendFarmashop";
            return InvokeCustomMethod(_AssemblyName, _NameSpace, _ClassName, method.ToString(), transfer);
        }

        private string GetCliente()
        {
            return _configuration.GetValue<string>("CustomSettings:Client");
        }

        private CSTransfer InvokeCustomMethod(string assemblyName, string nameSpace, string className, string method, CSTransfer transfer = null)
        {
            try
            {
                string cliente = GetCliente();
                string ensamblado = assemblyName + cliente;
                string espacioNombre = nameSpace + cliente;
                string CodeBase = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
                string path = System.IO.Path.GetDirectoryName(CodeBase);

                path = path.Replace("file:\\", "");

                string AbsolutePath = path + @"\" + ensamblado + ".dll";

                if (!string.IsNullOrEmpty(cliente))
                {
                    if (!File.Exists(AbsolutePath))
                    {
                        transfer.status = CSResult.NOT_IMPLEMENTED;
                        _logger.LogDebug("No se econtro dll personalizada del cliente");
                        return transfer;
                    }

                    Assembly a = Assembly.LoadFile(AbsolutePath);
                    Type t = a.GetType(espacioNombre + "." + className);

                    if (t == null)
                    {
                        transfer.status = CSResult.NOT_IMPLEMENTED;
                        _logger.LogDebug("No se econtro nombre de espacio: " + espacioNombre + "." + className);
                        return transfer;
                    }

                    object instance = Activator.CreateInstance(t, null);

                    MethodInfo m = t.GetMethod(method);

                    if (m == null)
                    {
                        transfer.status = CSResult.NOT_IMPLEMENTED;
                        _logger.LogDebug("No se encontro metodo: " + method);
                        return transfer;
                    }

                    transfer = (CSTransfer)m.Invoke(instance, new object[] { transfer /* parameters go here */ });
                }
            }
            catch (Exception ex)
            {
                if (transfer == null)
                    transfer = new CSTransfer();

                transfer.status = CSResult.ERROR;
                transfer.jsonData = null;
                transfer.errorMessage = (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                
                _logger.LogError(transfer.toLog());
            }

            return transfer;
        }
    }
}
