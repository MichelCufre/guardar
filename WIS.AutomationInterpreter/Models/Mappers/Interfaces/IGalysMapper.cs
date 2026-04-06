using System.Collections.Generic;
using WIS.Automation.Galys;
using WIS.Domain.Automatismo.Dtos;

namespace WIS.AutomationInterpreter.Models.Mappers.Interfaces
{
	public interface IGalysMapper
	{
		ProductoGalysRequest Map(ProductoAutomatismoRequest request);
		CodigoBarraGalysRequest Map(CodigoBarraAutomatismoRequest request);
		EntradaStockGalysRequest Map(EntradaStockAutomatismoRequest cabezal, List<EntradaStockLineaAutomatismoRequest> detalles);
        SalidaStockGalysRequest Map(SalidaStockAutomatismoRequest cabezal);

        NotificacionAjustesStockRequest Map(NotificacionAjusteStockGalysRequest request);
		ConfirmacionEntradaStockRequest Map(ConfirmacionEntradaStockGalysRequest request);
		ConfirmacionSalidaStockRequest Map(ConfirmacionSalidaStockGalysRequest request);

        ConfirmacionMovimientoStockRequest Map(ConfirmacionMovimientoStockGalysRequest request);
    }
}
