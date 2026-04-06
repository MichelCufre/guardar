using System;
using System.Collections.Generic;
using System.Linq;

namespace WIS.Domain.Documento.TiposDocumento
{
    public class DocumentoActa : Documento, IDocumentoActa
    {
        public IDocumento InReference { get; set; }
        public List<DocumentoLineaEgreso> OutDetail { get; set; }
        public List<DocumentoActaDetalle> ActaDetail { get; set; }

        public DocumentoActa()
        {
            this.OutDetail = new List<DocumentoLineaEgreso>();
            this.ActaDetail = new List<DocumentoActaDetalle>();
        }

        public virtual void CalcularValoresCifFob()
        {
            var qtTotal = this.Lineas.Sum(l => l.CantidadIngresada);
            var vlTotal = this.Lineas.Sum(l => l.ValorMercaderia);

            decimal? raz = (vlTotal + (this.InReference.ValorSeguro ?? 0) + (this.InReference.ValorFlete ?? 0) + (this.InReference.ValorOtrosGastos ?? 0)) / vlTotal;

            foreach (var linea in this.Lineas)
            {
                decimal? cif = (linea.ValorMercaderia * raz * this.ValorArbitraje);
                linea.CIF = Decimal.Round((decimal)cif, 3);
            }

            this.Validado = true;
            this.FechaModificacion = DateTime.Now;
        }

        public virtual void ValidarActa()
        {
            if (this.Lineas.Count > 0)
            {
                // Acta positiva
                // Controla que tenga valor positivo cargado en las lineas en los campos "VL_TRIBUTO", "VL_MERCADERIA", "VL_CIF_INGRESO"
                if (this.Lineas.Any(s => (s.ValorTributo <= 0 || s.ValorTributo == null) || (s.ValorMercaderia <= 0 || s.ValorMercaderia == null) || (s.CIF <= 0 || s.CIF == null)))
                {
                    this.Validado = false;
                }
                else
                {
                    this.Validado = true;
                }
            }
            else
            {
                // Acta negativa
                //"VL_TRIBUTO", "VL_FOB_INGRESO", "VL_CIF_INGRESO"
                if (this.OutDetail.Any(s => (s.Tributo <= 0 || s.Tributo == null) || (s.FOB <= 0 || s.FOB == null) || (s.CIF <= 0 || s.CIF == null)))
                {
                    this.Validado = false;
                }
                else
                {
                    this.Validado = true;
                }
            }
        }
    }
}