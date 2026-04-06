using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.ManejoStock.Constants;

namespace WIS.Domain.ManejoStock.AjusteStockDocumental
{
    public class AjusteStockDocumental
    {
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public virtual void NivelarAjustesPendientes(IUnitOfWork uow, string aplicacion, int user)
        {
            var empresas = uow.AjusteRepository.GetEmpresasConAjustesPendientes();

            NivelacionAjusteDocumental nivelacion = new NivelacionAjusteDocumental();

            foreach (var empresa in empresas)
            {
                var ajustes = uow.AjusteRepository.GetAjustesDocumentosPorEmpresa(empresa);

                //Dividir ajustes positivos y negativos
                var ajustesPositivos = ajustes.Where(a => a.CantidadMovimiento > 0).OrderByDescending(a => a.CantidadMovimiento).ToList();
                var ajustesNegativos = ajustes.Where(a => a.CantidadMovimiento < 0).OrderBy(a => a.CantidadMovimiento).ToList();

                foreach (var ajustePositivo in ajustesPositivos)
                {
                    this.NivelarAjuste(uow, ajustePositivo, ajustesNegativos, nivelacion);
                }
            }

            this.ImpactarNivelacion(uow, nivelacion);
            uow.SaveChanges();
        }

        public virtual void NivelarAjuste(IUnitOfWork uow, DocumentoAjusteStock ajustePositivo, List<DocumentoAjusteStock> ajustesNegativos, NivelacionAjusteDocumental nivelacion)
        {
            var ajustesNivelar = ajustesNegativos
                .Where(a => a.Producto == ajustePositivo.Producto
                    && a.Identificador == ajustePositivo.Identificador
                    && a.CantidadMovimiento < 0)
                .ToList();

            decimal cantidadNivelar = (decimal)ajustePositivo.CantidadMovimiento;

            foreach (var ajusteNegativo in ajustesNivelar)
            {
                if (ajustePositivo.CantidadMovimiento == 0)
                    break;

                if (nivelacion.ajustesEliminar.Any(ae => ae.NumeroAjuste == ajustePositivo.NumeroAjuste))
                    break;

                int numeroOperacion = int.Parse(uow.AjusteRepository.GetNumeroOperacionAjuste());
                decimal margenNivelacion = (decimal)ajustePositivo.CantidadMovimiento + (decimal)ajusteNegativo.CantidadMovimiento;

                if (margenNivelacion == 0) //Los saldos se anulan, se nivelan ambos ajustes en su totalidad
                {
                    DocumentoAjusteStockHistorico historicoPositivo = new DocumentoAjusteStockHistorico(ajustePositivo)
                    {
                        CantidadMovimiento = ajustePositivo.CantidadMovimiento,
                        NumeroOperacion = numeroOperacion,
                        TipoOperacion = TipoOperacion.NIVELACION_AUTOMATICA
                    };

                    DocumentoAjusteStockHistorico historicoNegativo = new DocumentoAjusteStockHistorico(ajusteNegativo)
                    {
                        CantidadMovimiento = ajusteNegativo.CantidadMovimiento,
                        NumeroOperacion = numeroOperacion,
                        TipoOperacion = TipoOperacion.NIVELACION_AUTOMATICA
                    };

                    nivelacion.ajustesHistoricosAgregar.Add(historicoPositivo);
                    nivelacion.ajustesHistoricosAgregar.Add(historicoNegativo);

                    nivelacion.ajustesEliminar.Add(ajusteNegativo);
                    nivelacion.ajustesEliminar.Add(ajustePositivo);

                    if (nivelacion.ajustesModificar.Contains(ajustePositivo))
                    {
                        nivelacion.ajustesModificar.Remove(ajustePositivo);
                    }

                    if (nivelacion.ajustesModificar.Contains(ajusteNegativo))
                    {
                        nivelacion.ajustesModificar.Remove(ajusteNegativo);
                    }
                }
                else if (margenNivelacion > 0) //El saldo negativo no consume todo el ajuste positivo, se consume el ajuste negativo y se modifica el positivo
                {
                    ajustePositivo.CantidadMovimiento = ajustePositivo.CantidadMovimiento + ajusteNegativo.CantidadMovimiento;
                    ajustePositivo.FechaActualizacion = DateTime.Now;

                    DocumentoAjusteStockHistorico historicoPositivo = new DocumentoAjusteStockHistorico(ajustePositivo)
                    {
                        CantidadMovimiento = Math.Abs((decimal)ajusteNegativo.CantidadMovimiento),
                        NumeroOperacion = numeroOperacion,
                        TipoOperacion = TipoOperacion.NIVELACION_AUTOMATICA
                    };

                    DocumentoAjusteStockHistorico historicoNegativo = new DocumentoAjusteStockHistorico(ajusteNegativo)
                    {
                        CantidadMovimiento = ajusteNegativo.CantidadMovimiento,
                        NumeroOperacion = numeroOperacion,
                        TipoOperacion = TipoOperacion.NIVELACION_AUTOMATICA
                    };

                    if (nivelacion.ajustesModificar.Contains(ajustePositivo))
                    {
                        nivelacion.ajustesModificar.Remove(ajustePositivo);
                    }

                    if (nivelacion.ajustesModificar.Contains(ajusteNegativo))
                    {
                        nivelacion.ajustesModificar.Remove(ajusteNegativo);
                    }

                    nivelacion.ajustesModificar.Add(ajustePositivo);
                    nivelacion.ajustesEliminar.Add(ajusteNegativo);

                    nivelacion.ajustesHistoricosAgregar.Add(historicoPositivo);
                    nivelacion.ajustesHistoricosAgregar.Add(historicoNegativo);
                }
                else //El saldo negativo es superior al positivo, se consume el ajuste positivo y se modifica el negativo
                {
                    ajusteNegativo.CantidadMovimiento = ajustePositivo.CantidadMovimiento + ajusteNegativo.CantidadMovimiento;
                    ajusteNegativo.FechaActualizacion = DateTime.Now;

                    DocumentoAjusteStockHistorico historicoPositivo = new DocumentoAjusteStockHistorico(ajustePositivo)
                    {
                        CantidadMovimiento = ajustePositivo.CantidadMovimiento,
                        NumeroOperacion = numeroOperacion,
                        TipoOperacion = TipoOperacion.NIVELACION_AUTOMATICA
                    };

                    DocumentoAjusteStockHistorico historicoNegativo = new DocumentoAjusteStockHistorico(ajusteNegativo)
                    {
                        CantidadMovimiento = (ajustePositivo.CantidadMovimiento * -1),
                        NumeroOperacion = numeroOperacion,
                        TipoOperacion = TipoOperacion.NIVELACION_AUTOMATICA
                    };

                    if (nivelacion.ajustesModificar.Contains(ajustePositivo))
                    {
                        nivelacion.ajustesModificar.Remove(ajustePositivo);
                    }

                    if (nivelacion.ajustesModificar.Contains(ajusteNegativo))
                    {
                        nivelacion.ajustesModificar.Remove(ajusteNegativo);
                    }

                    nivelacion.ajustesModificar.Add(ajusteNegativo);
                    nivelacion.ajustesEliminar.Add(ajustePositivo);

                    nivelacion.ajustesHistoricosAgregar.Add(historicoPositivo);
                    nivelacion.ajustesHistoricosAgregar.Add(historicoNegativo);
                }
            }

            ajustesNegativos.RemoveAll(an => nivelacion.ajustesEliminar.Any(ae => ae.NumeroAjuste == an.NumeroAjuste));
        }

        public virtual void ImpactarNivelacion(IUnitOfWork uow, NivelacionAjusteDocumental nivelacion)
        {
            var nuTransaccion = uow.GetTransactionNumber();

            foreach (var agregar in nivelacion.ajustesAgregar)
            {
                uow.AjusteRepository.AddAjusteStockDocumental(agregar, nuTransaccion);
            }

            foreach (var modificacion in nivelacion.ajustesModificar)
            {
                uow.AjusteRepository.UpdateAjusteStockDocumental(modificacion, nuTransaccion);
            }

            foreach (var eliminacion in nivelacion.ajustesEliminar)
            {
                uow.AjusteRepository.RemoveAjusteStockDocumental(eliminacion, nuTransaccion);
            }

            foreach (var historico in nivelacion.ajustesHistoricosAgregar)
            {
                uow.AjusteRepository.AddAjusteStockDocumentalHistorico(historico, nuTransaccion);
            }

            foreach (var historico in nivelacion.ajustesHistoricosEliminar)
            {
                uow.AjusteRepository.EliminarAjusteStockDocumentalHistorico(historico, nuTransaccion);
            }
        }

        public virtual void ImportarAjustesDeStock(IUnitOfWork uow, int usuario, string aplicacion, int cantidadAjustes)
        {
            try
            {
                var mapperDocumento = new AjusteMapper();
                var ajustes = uow.AjusteRepository.GetAjustesStockDocumentalPendientes(cantidadAjustes);

                if (ajustes != null && ajustes.Count > 0)
                {
                    uow.CreateTransactionNumber("AjusteStockDocumentalService");
                    var nuTransaccion = uow.GetTransactionNumber();

                    foreach (var ajuste in ajustes)
                    {
                        ajuste.TpDocumento = "P";
                        ajuste.NuTransaccion = nuTransaccion;

                        uow.AjusteRepository.UpdateAjusteStock(ajuste);
                        uow.AjusteRepository.AddAjusteStockDocumental(mapperDocumento.MapFromAjusteToAjusteDocumental(ajuste), nuTransaccion);
                    }

                    uow.SaveChanges();
                }

                NivelarAjustesPendientes(uow, aplicacion, usuario);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                throw ex;
            }
        }

    }
}
