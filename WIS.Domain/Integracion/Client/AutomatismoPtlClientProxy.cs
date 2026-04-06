using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Integracion.Client
{
    public class AutomatismoPtlClientProxy
    {
        private readonly IAutomatismoPtlClientService _automatismoPtlClientService;

        public AutomatismoPtlClientProxy(IAutomatismoPtlClientService automatismoPtlClientService)
        {
            this._automatismoPtlClientService = automatismoPtlClientService;
        }

        public List<PtlColorActivo> GetColoresActivosByPtl(string ptl, IUnitOfWork uow)
        {
            if (string.IsNullOrEmpty(ptl)) return new List<PtlColorActivo>();

            (ValidationsResult validationsResult, List<PtlCommandConfirmRequest> response) = _automatismoPtlClientService.GetColoresActivosRecords(ptl);

            if (validationsResult != null && validationsResult.Errors.Count > 0)
                throw new Exception(string.Join('.', validationsResult.GetErrors()));

            var coloresActivos = LoadColoresActivosDataInMemory(uow, response);

            (ValidationsResult validationsResult2, List<PtlColorResponse> coloresReservadosPtl) = _automatismoPtlClientService.GetColoresActivosByPtl(ptl);

            var coloresActivosParaFinalizar = new List<PtlColorActivo>();
            var idPtl = uow.AutomatismoRepository.GetAutomatismoByZona(ptl);
            foreach (var item in coloresReservadosPtl)
            {
                coloresActivosParaFinalizar.Add(new PtlColorActivo
                {
                    Color = item.Code,
                    UserId = item.UserId.Value,
                    Usuario = uow.FuncionarioRepository.GetFuncionario(item.UserId.Value).Nombre,
                    IdPtl = idPtl.Numero.ToString()
                });
            }
            var joinColoresActivos = (from colReservado in coloresActivosParaFinalizar
                                      join colActivo in coloresActivos
                                     on new { colReservado.UserId, colReservado.Color } equals new { colActivo.UserId, colActivo.Color }
                                     into coloresActivosJoin
                                      from pco in coloresActivosJoin.DefaultIfEmpty()
                                      select new PtlColorActivo
                                      {
                                          UserId = pco != null ? pco.UserId : colReservado.UserId,
                                          Cliente = pco != null ? pco.Cliente : colReservado.Cliente,
                                          Usuario = pco != null ? pco.Usuario : colReservado.Usuario,
                                          Color = pco != null ? pco.Color : colReservado.Color,
                                          Contenedor = pco != null ? pco.Contenedor : colReservado.Contenedor,
                                          IdPtl = pco != null ? pco.IdPtl : colReservado.IdPtl,
                                          Preparacion = pco != null ? pco.Preparacion : colReservado.Preparacion,
                                          SubClase = pco != null ? pco.SubClase : colReservado.SubClase,
                                          ComparteContenedorPicking = pco != null ? pco.ComparteContenedorPicking : colReservado.ComparteContenedorPicking
                                      }).ToList();

            return joinColoresActivos;
        }

        private List<PtlColorActivo> LoadColoresActivosDataInMemory(IUnitOfWork uow, List<PtlCommandConfirmRequest> coloresActivos)
        {
            List<PtlColorActivo> coloresActivosByPtl = new List<PtlColorActivo>();

            coloresActivos = coloresActivos.GroupBy(i => new
            {
                Id = i.Id,
                Color = i.Color,
                Detail = i.Detail,
                UserId = i.UserId
            }).Select(s => new PtlCommandConfirmRequest
            {
                Id = s.Key.Id,
                Color = s.Key.Color,
                Detail = s.Key.Detail,
                UserId = s.Key.UserId
            }).ToList();

            foreach (var item in coloresActivos)
            {
                var detail = JsonConvert.DeserializeObject<PtlDetailPicking>(item.Detail);

                coloresActivosByPtl.Add(new PtlColorActivo
                {
                    Color = item.Color,
                    Contenedor = detail.Contenedor.ToString(),
                    Preparacion = detail.Preparacion,
                    UserId = item.UserId,
                    Usuario = uow.FuncionarioRepository.GetFuncionario(item.UserId).Nombre,
                    Cliente = detail.Cliente,
                    SubClase = detail.SubClase,
                    ComparteContenedorPicking = detail.ComparteContenedorPicking,
                    IdPtl = item.Id
                });
            }

            return coloresActivosByPtl;
        }

        public void UpdateLuzByPtlColor(PtlColorActivo colorActivo)
        {
            (ValidationsResult validationsResult, bool response) = _automatismoPtlClientService.UpdateLuzByPtlColor(new PtlColorActivoRequest
            {
                Cliente = colorActivo.Cliente,
                Color = colorActivo.Color,
                ComparteContenedorPicking = colorActivo.ComparteContenedorPicking,
                Contenedor = colorActivo.Contenedor,
                IdPtl = colorActivo.IdPtl,
                Preparacion = colorActivo.Preparacion,
                SubClase = colorActivo.SubClase,
                UserId = colorActivo.UserId,
                Usuario = colorActivo.Usuario,
            });

            if (validationsResult != null && validationsResult.Errors.Count > 0)
                throw new Exception(string.Join('.', validationsResult.GetErrors()));
        }
    }
}
