using System.Collections.Generic;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.General.Enums;
using WIS.Extension;
using WIS.WMS_API.Models.Mappers.Interfaces;

namespace WIS.WMS_API.Models.Mappers
{
    public class EmpresaMapper : Mapper, IEmpresaMapper
    {
        public EmpresaMapper()
        {
        }

        public virtual List<Empresa> Map(EmpresasRequest request)
        {
            List<Empresa> models = new List<Empresa>();

            foreach (var e in request.Empresas)
            {
                short estado = (short)(e.Estado ?? -1);

                Empresa model = new Empresa(estado, e.TipoFiscal, e.Pais, e.Subdivision, e.Localidad);
                model.Anexo1 = e.Anexo1;
                model.Anexo2 = e.Anexo2;
                model.Anexo3 = e.Anexo3;
                model.Anexo4 = e.Anexo4;
                model.CantidadDiasPeriodo = e.CantidadDiasPeriodo;
                model.CdClienteArmadoKit = e.ClienteArmadoKit;
                model.cdTipoDeAlmacenajeYSeguro = e.TipoDeAlmacenajeYSeguro;
                model.CodigoPostal = e.CodigoPostal;
                model.Direccion = e.Direccion;
                model.EmpresaConsolidado = e.EmpresaConsolidado;
                model.Id = e.Codigo;
                model.IdDAP = e.IdDAP;
                model.IdOperativo = e.IdOperativo;
                model.IdUnidadFactura = e.IdUnidadFactura;
                model.ListaPrecio = e.ListaPrecio;
                model.Nombre = e.Nombre;
                model.NumeroFiscal = e.NumeroFiscal;
                model.ProveedorDevolucion = e.ProveedorDevolucion;
                model.Telefono = e.Telefono;
                model.ValorMinimoStock = e.ValorMinimoStock;
                model.ValorPallet = e.ValorPallet;
                model.ValorPalletDia = e.ValorPalletDia;
                models.Add(model);
            }
            return models;
        }

        public virtual EmpresaResponse MapToResponse(Empresa empresa)
        {
            return new EmpresaResponse()
            {
                Anexo1 = empresa.Anexo1,
                Anexo2 = empresa.Anexo2,
                Anexo3 = empresa.Anexo3,
                Anexo4 = empresa.Anexo4,
                CantidadDiasPeriodo = empresa.CantidadDiasPeriodo,
                ClienteArmadoKit = empresa.CdClienteArmadoKit,
                CodigoPostal = empresa.CodigoPostal,
                Codigo = empresa.Id,
                Direccion = empresa.Direccion,
                EmpresaConsolidado = empresa.EmpresaConsolidado,
                Estado = this.MapEstado(empresa.Estado) ?? -1,
                FechaInsercion = empresa.FechaInsercion.ToString(CDateFormats.DATE_ONLY),
                FechaModificacion = empresa.FechaModificacion.ToString(CDateFormats.DATE_ONLY),
                IdDAP = empresa.IdDAP,
                IdLocalidad = empresa.IdLocalidad,
                IdOperativo = empresa.IdOperativo,
                IdUnidadFactura = empresa.IdUnidadFactura,
                ListaPrecio = empresa.ListaPrecio,
                Nombre = empresa.Nombre,
                NumeroFiscal = empresa.NumeroFiscal,
                ProveedorDevolucion = empresa.ProveedorDevolucion,
                Telefono = empresa.Telefono,
                TipoDeAlmacenajeYSeguro = empresa.cdTipoDeAlmacenajeYSeguro,
                TipoFiscal = empresa.TipoFiscal?.Id,
                ValorMinimoStock = empresa.ValorMinimoStock,
                ValorPallet = empresa.ValorPallet,
                ValorPalletDia = empresa.ValorPalletDia
            };
        }

        public virtual short? MapEstado(EstadoEmpresa estado)
        {
            switch (estado)
            {
                case EstadoEmpresa.Activo: return SituacionDb.Activo;
                case EstadoEmpresa.Inactivo: return SituacionDb.Inactivo;
            }
            return null;
        }
    }
}
