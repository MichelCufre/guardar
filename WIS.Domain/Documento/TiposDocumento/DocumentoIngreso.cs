using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Documento.Integracion.Recepcion;

namespace WIS.Domain.Documento.TiposDocumento
{
    public class DocumentoIngreso : Documento, IDocumentoIngreso
    {
        public bool Balanceado { get; set; }

        public virtual bool IsHabilitadoParaBalanceo(IUnitOfWork uow)
        {
            var documentosBalanceo = uow.DocumentoTipoRepository.GetDocumentosHabilitadosParaBalanceo();
            return documentosBalanceo.ContainsKey(this.Tipo) && documentosBalanceo[this.Tipo].Contains(this.Estado);
        }

        public virtual DocumentoLineaBalanceo BalancearLote(List<InformacionBalanceo> balanceos)
        {
            var result = new DocumentoLineaBalanceo();
            var totalesIngresados = new Dictionary<string, decimal>();
            var totalesRecibidos = new Dictionary<string, decimal>();
            var cifIngresado = new Dictionary<string, decimal>();
            var fobIngresado = new Dictionary<string, decimal>();
            var tributoIngresado = new Dictionary<string, decimal>();
            var lotesRecibidos = new Dictionary<string, List<InformacionBalanceo>>();
            var lineasOriginales = new Dictionary<string, HashSet<string>>();

            foreach (var linea in this.Lineas)
            {
                if (!totalesIngresados.ContainsKey(linea.Producto))
                {
                    totalesIngresados[linea.Producto] = 0;
                    cifIngresado[linea.Producto] = 0;
                    fobIngresado[linea.Producto] = 0;
                    tributoIngresado[linea.Producto] = 0;
                }

                totalesIngresados[linea.Producto] += (linea.CantidadIngresada ?? 0);
                cifIngresado[linea.Producto] += (linea.CIF ?? 0);
                fobIngresado[linea.Producto] += (linea.ValorMercaderia ?? 0);
                tributoIngresado[linea.Producto] += (linea.ValorTributo ?? 0);

                if (!lineasOriginales.ContainsKey(linea.Producto))
                    lineasOriginales[linea.Producto] = new HashSet<string>();

                if (!lineasOriginales[linea.Producto].Contains(linea.Identificador))
                    lineasOriginales[linea.Producto].Add(linea.Identificador);
            }

            foreach (var balanceo in balanceos)
            {
                if (!totalesRecibidos.ContainsKey(balanceo.Producto))
                    totalesRecibidos[balanceo.Producto] = 0;

                totalesRecibidos[balanceo.Producto] += (balanceo.CantidadIngresada ?? 0);

                if (!lotesRecibidos.ContainsKey(balanceo.Producto))
                    lotesRecibidos[balanceo.Producto] = new List<InformacionBalanceo>();

                lotesRecibidos[balanceo.Producto].Add(balanceo);
            }

            foreach (var producto in totalesIngresados.Keys)
            {
                var totalIngresado = totalesIngresados[producto];

                cifIngresado[producto] /= (totalIngresado == 0 ? 1 : totalIngresado);
                fobIngresado[producto] /= (totalIngresado == 0 ? 1 : totalIngresado);
                tributoIngresado[producto] /= (totalIngresado == 0 ? 1 : totalIngresado);

                if (totalesRecibidos.ContainsKey(producto))
                {
                    var totalRecibido = totalesRecibidos[producto];
                    var lotes = lotesRecibidos[producto];

                    if (totalIngresado > totalRecibido)
                    {
                        var lote = lotes.First(l => !l.RecepcionCompleta);
                        var faltante = totalIngresado - totalRecibido;
                        lote.CantidadIngresada = (lote.CantidadIngresada ?? 0) + faltante;
                    }
                    else
                    {
                        var sobrante = totalRecibido - totalIngresado;                       
                        for (int i = lotes.Count - 1; i >= 0 && sobrante > 0; i--) // Recorre en orden inverso para poder borrar del final
                        {
                            var lote = lotes[i];

                            if (!lote.RecepcionCompleta) // Balancea solo los lotes desbalanceados
                            {
                                if ((lote.CantidadIngresada ?? 0) > sobrante)
                                {
                                    lote.CantidadIngresada = (lote.CantidadIngresada ?? 0) - sobrante;
                                    sobrante = 0;
                                }
                                else
                                {
                                    sobrante -= (lote.CantidadIngresada ?? 0);
                                    lotes.RemoveAt(i);
                                }
                            }
                        }
                    }
                }
            }

            foreach (var linea in this.Lineas)
            {
                if (lotesRecibidos.ContainsKey(linea.Producto))
                {
                    var lote = lotesRecibidos[linea.Producto].FirstOrDefault(l => l.NumeroIdentificador == linea.Identificador);

                    if (lote == null)
                    {
                        result.LineasEliminadas.Add(linea);
                    }
                    else
                    {
                        var cif = (lote.CantidadIngresada ?? 0) * cifIngresado[linea.Producto];
                        var fob = (lote.CantidadIngresada ?? 0) * fobIngresado[linea.Producto];
                        var tributo = (lote.CantidadIngresada ?? 0) * tributoIngresado[linea.Producto];

                        if ((lote.CantidadIngresada ?? 0) != (linea.CantidadIngresada ?? 0)
                            || cif != (linea.CIF ?? 0)
                            || fob != (linea.ValorMercaderia ?? 0)
                            || tributo != (linea.ValorTributo ?? 0))
                        {
                            linea.CantidadIngresada = (lote.CantidadIngresada ?? 0);
                            linea.CIF = linea.CantidadIngresada * cifIngresado[linea.Producto];
                            linea.ValorMercaderia = linea.CantidadIngresada * fobIngresado[linea.Producto];
                            linea.ValorTributo = linea.CantidadIngresada * tributoIngresado[linea.Producto];
                            result.LineasModificadas.Add(linea);
                        }
                    }
                }
            }

            foreach (var producto in lineasOriginales.Keys)
            {
                if (lotesRecibidos.ContainsKey(producto))
                {
                    foreach (var lote in lotesRecibidos[producto])
                    {
                        if (!lineasOriginales[producto].Contains(lote.NumeroIdentificador))
                        {
                            result.LineasAgregadas.Add(new DocumentoLinea()
                            {
                                Empresa = this.Empresa.Value,
                                Producto = producto,
                                Faixa = 1,
                                Identificador = lote.NumeroIdentificador,
                                CantidadIngresada = (lote.CantidadIngresada ?? 0),
                                CIF = (lote.CantidadIngresada ?? 0) * cifIngresado[producto],
                                ValorMercaderia = (lote.CantidadIngresada ?? 0) * fobIngresado[producto],
                                ValorTributo = (lote.CantidadIngresada ?? 0) * tributoIngresado[producto],
                                FechaAlta = DateTime.Now,
                                Situacion = SituacionDb.Activo,
                                IdentificadorIngreso = lote.NumeroIdentificador
                            });
                        }
                    }
                }
            }

            var lineasSinIngreso = this.Lineas.Where(l => (l.CantidadIngresada ?? 0) == 0);

            result.LineasModificadas.RemoveAll(l => lineasSinIngreso.Contains(l));
            result.LineasEliminadas.AddRange(lineasSinIngreso);

            this.Lineas.RemoveAll(l => result.LineasEliminadas.Contains(l));
            this.Lineas.AddRange(result.LineasAgregadas);

            return result;
        }

        public virtual void CalcularValoresCifFob()
        {
            var qtTotal = this.Lineas.Sum(l => l.CantidadIngresada);
            var vlTotal = this.Lineas.Sum(l => l.ValorMercaderia);

            decimal? raz = (vlTotal + (this.ValorSeguro ?? 0) + (this.ValorFlete ?? 0) + (this.ValorOtrosGastos ?? 0)) / vlTotal;

            foreach (var linea in this.Lineas)
            {
                decimal? cif = (linea.ValorMercaderia * raz * this.ValorArbitraje);
                linea.CIF = Decimal.Round((decimal)cif, 3);
            }

            this.Validado = true;
            this.FechaModificacion = DateTime.Now;
        }

        public virtual void ConfirmarBalanceo()
        {
            this.Balanceado = true;
            this.FechaModificacion = DateTime.Now;
        }
    }
}
