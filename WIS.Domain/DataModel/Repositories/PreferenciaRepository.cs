using Microsoft.EntityFrameworkCore;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers.Picking;
using WIS.Domain.Extensions;
using WIS.Domain.General;
using WIS.Domain.Picking;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class PreferenciaRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly PreferenciaMapper _mapper;
        protected readonly IDapper _dapper;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();

        public PreferenciaRepository(WISDB context, string cdAplicacion, int userId, IDapper dapper)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new PreferenciaMapper();
            this._dapper = dapper;
        }

        public virtual Preferencia GetPreferencia(int nuPreferencia)
        {
            T_PREFERENCIA preferencia = this._context.T_PREFERENCIA.AsNoTracking().FirstOrDefault(f => f.NU_PREFERENCIA == nuPreferencia);

            return this._mapper.MapToObject(preferencia);
        }

        public virtual void AddPreferencia(Preferencia preferencia)
        {
            T_PREFERENCIA entity = this._mapper.MapToEntity(preferencia);

            entity.DT_ADDROW = DateTime.Now;

            this._context.T_PREFERENCIA.Add(entity);
        }

        public virtual void UpdatePreferencia(Preferencia preferencia)
        {
            T_PREFERENCIA entity = this._mapper.MapToEntity(preferencia);

            entity.DT_UPDROW = DateTime.Now;

            this._context.T_PREFERENCIA.Attach(entity);

            this._context.Entry(entity).State = EntityState.Modified;
        }

        public virtual int GetNextNuPreferencia()
        {
            //TODO revisar secuencia
            return this._context.GetNextSequenceValueInt(_dapper, "S_PREFERENCIA");
        }

        public virtual int GetNextNuPreferenciaRel()
        {
            //TODO revisar secuencia
            return this._context.GetNextSequenceValueInt(_dapper, "S_PREFERENCIA_REL");
        }

        #region Preferencia-Control-Acceso

        public virtual List<ControlAcceso> GetAllControlAcceso()
        {
            var entities = this._context.T_CONTROL_ACCESO.AsNoTracking()
               .ToList().OrderBy(x => x.CD_CONTROL_ACCESO);

            var controlesAcceso = new List<ControlAcceso>();
            var mapperControlAcceso = new ControlAccesoMapper();

            foreach (var entity in entities)
            {
                controlesAcceso.Add(mapperControlAcceso.MapToObject(entity));
            }

            return controlesAcceso;
        }

        public virtual bool AnyControlAcceso(string cdControlAcceso)
        {
            return this._context.T_CONTROL_ACCESO.Any(d => d.CD_CONTROL_ACCESO == cdControlAcceso);
        }

        public virtual List<PreferenciaAsociarContolAcceso> GetAllControlAccesoPreferencia(int nuPreferencia)
        {
            return this._context.T_PREFERENCIA_CONTROL_ACCESO.Where(x => x.NU_PREFERENCIA == nuPreferencia).Select(w => new PreferenciaAsociarContolAcceso { cdControlAcceso = w.CD_CONTROL_ACCESO, nuPreferencia = w.NU_PREFERENCIA }).ToList();
        }

        public virtual void AddControlAccesoPreferencia(List<PreferenciaAsociarContolAcceso> controles)
        {
            foreach (var control in controles)
            {
                T_PREFERENCIA_CONTROL_ACCESO entity = new T_PREFERENCIA_CONTROL_ACCESO
                {
                    NU_PREFERENCIA = control.nuPreferencia,
                    CD_CONTROL_ACCESO = control.cdControlAcceso,
                    DT_ADDROW = DateTime.Now,
                };

                this._context.T_PREFERENCIA_CONTROL_ACCESO.Add(entity);
            }
        }

        public virtual void RemoveControlAccesoPreferencia(List<PreferenciaAsociarContolAcceso> controles)
        {
            foreach (var control in controles)
            {
                var linea = this._context.T_PREFERENCIA_CONTROL_ACCESO.FirstOrDefault(f => f.NU_PREFERENCIA == control.nuPreferencia && f.CD_CONTROL_ACCESO == control.cdControlAcceso);

                this._context.T_PREFERENCIA_CONTROL_ACCESO.Remove(linea);
            }
        }
        #endregion

        #region Preferencia-Empresa

        public virtual List<PreferenciaAsociarEmpresa> GetAllEmpresaPreferencia(int nuPreferencia)
        {
            return this._context.T_PREFERENCIA_EMPRESA.Where(x => x.NU_PREFERENCIA == nuPreferencia).Select(w => new PreferenciaAsociarEmpresa { cdEmpresa = w.CD_EMPRESA, nuPreferencia = w.NU_PREFERENCIA }).ToList();
        }

        public virtual void AddEmpresasPreferencia(List<PreferenciaAsociarEmpresa> empresas)
        {
            foreach (var empresa in empresas)
            {
                T_PREFERENCIA_EMPRESA entity = new T_PREFERENCIA_EMPRESA
                {
                    NU_PREFERENCIA = empresa.nuPreferencia,
                    CD_EMPRESA = empresa.cdEmpresa,
                    DT_ADDROW = DateTime.Now,
                };

                this._context.T_PREFERENCIA_EMPRESA.Add(entity);
            }
        }

        public virtual void RemoveEmpresasPreferencia(List<PreferenciaAsociarEmpresa> empresas)
        {
            foreach (var empresa in empresas)
            {
                var linea = this._context.T_PREFERENCIA_EMPRESA.FirstOrDefault(f => f.NU_PREFERENCIA == empresa.nuPreferencia && f.CD_EMPRESA == empresa.cdEmpresa);

                this._context.T_PREFERENCIA_EMPRESA.Remove(linea);
            }
        }
        #endregion

        #region Preferencia-Cliente

        public virtual List<PreferenciaAsociarCliente> GetAllClientePreferencia(int nuPreferencia)
        {
            return this._context.T_PREFERENCIA_CLIENTE.Where(x => x.NU_PREFERENCIA == nuPreferencia).Select(w => new PreferenciaAsociarCliente { cdCliente = w.CD_CLIENTE, nuPreferencia = w.NU_PREFERENCIA }).ToList();
        }

        public virtual void AddClientesPreferencia(List<PreferenciaAsociarCliente> clientes)
        {
            foreach (var cliente in clientes)
            {
                T_PREFERENCIA_CLIENTE entity = new T_PREFERENCIA_CLIENTE
                {
                    NU_PREFERENCIA = cliente.nuPreferencia,
                    CD_CLIENTE = cliente.cdCliente,
                    CD_EMPRESA = cliente.cdEmpresa,
                    DT_ADDROW = DateTime.Now,
                };

                this._context.T_PREFERENCIA_CLIENTE.Add(entity);
            }
        }

        public virtual void RemoveClientesPreferencia(List<PreferenciaAsociarCliente> clientes)
        {
            foreach (var cliente in clientes)
            {
                var linea = this._context.T_PREFERENCIA_CLIENTE.FirstOrDefault(f => f.NU_PREFERENCIA == cliente.nuPreferencia && f.CD_CLIENTE == cliente.cdCliente && f.CD_EMPRESA == cliente.cdEmpresa);

                this._context.T_PREFERENCIA_CLIENTE.Remove(linea);
            }
        }
        #endregion

        #region Preferencia-Ruta

        public virtual List<PreferenciaAsociarRuta> GetAllRutaPreferencia(int nuPreferencia)
        {
            return this._context.T_PREFERENCIA_ROTA.Where(x => x.NU_PREFERENCIA == nuPreferencia).Select(w => new PreferenciaAsociarRuta { codRuta = w.CD_ROTA, nuPreferencia = w.NU_PREFERENCIA }).ToList();
        }

        public virtual void AddRutasPreferencia(List<PreferenciaAsociarRuta> rutas)
        {
            foreach (var ruta in rutas)
            {
                T_PREFERENCIA_ROTA entity = new T_PREFERENCIA_ROTA
                {
                    NU_PREFERENCIA = ruta.nuPreferencia,
                    CD_ROTA = ruta.codRuta,
                    DT_ADDROW = DateTime.Now,
                };

                this._context.T_PREFERENCIA_ROTA.Add(entity);
            }
        }

        public virtual void RemoveRutasPreferencia(List<PreferenciaAsociarRuta> rutas)
        {
            foreach (var ruta in rutas)
            {
                var linea = this._context.T_PREFERENCIA_ROTA.FirstOrDefault(f => f.NU_PREFERENCIA == ruta.nuPreferencia && f.CD_ROTA == ruta.codRuta);

                this._context.T_PREFERENCIA_ROTA.Remove(linea);
            }
        }
        #endregion

        #region Preferencia-Zona

        public virtual List<PreferenciaAsociarZona> GetAllZonaPreferencia(int nuPreferencia)
        {
            return this._context.T_PREFERENCIA_ZONA.Where(x => x.NU_PREFERENCIA == nuPreferencia).Select(w => new PreferenciaAsociarZona { codZona = w.CD_ZONA, nuPreferencia = w.NU_PREFERENCIA }).ToList();
        }

        public virtual void AddZonasPreferencia(List<PreferenciaAsociarZona> zonas)
        {
            foreach (var zona in zonas)
            {
                T_PREFERENCIA_ZONA entity = new T_PREFERENCIA_ZONA
                {
                    NU_PREFERENCIA = zona.nuPreferencia,
                    CD_ZONA = zona.codZona,
                    DT_ADDROW = DateTime.Now,
                };

                this._context.T_PREFERENCIA_ZONA.Add(entity);
            }
        }

        public virtual void RemoveZonasPreferencia(List<PreferenciaAsociarZona> zonas)
        {
            foreach (var zona in zonas)
            {
                var linea = this._context.T_PREFERENCIA_ZONA.FirstOrDefault(f => f.NU_PREFERENCIA == zona.nuPreferencia && f.CD_ZONA == zona.codZona);

                this._context.T_PREFERENCIA_ZONA.Remove(linea);
            }
        }

        #endregion

        #region Preferencia-Cond.Liberacion

        public virtual List<PreferenciaAsociarCondLib> GetAllCondLibPreferencia(int nuPreferencia)
        {
            return this._context.T_PREFERENCIA_COND_LIBERACION.Where(x => x.NU_PREFERENCIA == nuPreferencia).Select(w => new PreferenciaAsociarCondLib { codCondicionLiberacion = w.CD_CONDICION_LIBERACION, nuPreferencia = w.NU_PREFERENCIA }).ToList();
        }

        public virtual void AddCondLibPreferencia(List<PreferenciaAsociarCondLib> condiciones)
        {
            foreach (var condicion in condiciones)
            {
                T_PREFERENCIA_COND_LIBERACION entity = new T_PREFERENCIA_COND_LIBERACION
                {
                    NU_PREFERENCIA = condicion.nuPreferencia,
                    CD_CONDICION_LIBERACION = condicion.codCondicionLiberacion,
                    DT_ADDROW = DateTime.Now,
                };

                this._context.T_PREFERENCIA_COND_LIBERACION.Add(entity);
            }
        }

        public virtual void RemoveCondLibPreferencia(List<PreferenciaAsociarCondLib> condiciones)
        {
            foreach (var condicion in condiciones)
            {
                var linea = this._context.T_PREFERENCIA_COND_LIBERACION.FirstOrDefault(f => f.NU_PREFERENCIA == condicion.nuPreferencia && f.CD_CONDICION_LIBERACION == condicion.codCondicionLiberacion);

                this._context.T_PREFERENCIA_COND_LIBERACION.Remove(linea);
            }
        }

        #endregion

        #region Preferencia-Tipo-Pedido

        public virtual List<PreferenciaAsociarTpPedido> GetAllTpPedidoPreferencia(int nuPreferencia)
        {
            return this._context.T_PREFERENCIA_TIPO_PEDIDO.Where(x => x.NU_PREFERENCIA == nuPreferencia).Select(w => new PreferenciaAsociarTpPedido { tpPedido = w.TP_PEDIDO, nuPreferencia = w.NU_PREFERENCIA }).ToList();
        }

        public virtual void AddTpPedidoPreferencia(List<PreferenciaAsociarTpPedido> tipos)
        {
            foreach (var tipo in tipos)
            {
                T_PREFERENCIA_TIPO_PEDIDO entity = new T_PREFERENCIA_TIPO_PEDIDO
                {
                    NU_PREFERENCIA = tipo.nuPreferencia,
                    TP_PEDIDO = tipo.tpPedido,
                    DT_ADDROW = DateTime.Now,
                };

                this._context.T_PREFERENCIA_TIPO_PEDIDO.Add(entity);
            }
        }

        public virtual void RemoveTpPedidoPreferencia(List<PreferenciaAsociarTpPedido> tipos)
        {
            foreach (var tipo in tipos)
            {
                var linea = this._context.T_PREFERENCIA_TIPO_PEDIDO.FirstOrDefault(f => f.NU_PREFERENCIA == tipo.nuPreferencia && f.TP_PEDIDO == tipo.tpPedido);

                this._context.T_PREFERENCIA_TIPO_PEDIDO.Remove(linea);
            }
        }

        #endregion

        #region Preferencia-Tipo-Expedicion

        public virtual List<PreferenciaAsociarTpExpedicion> GetAllTpExpedicionPreferencia(int nuPreferencia)
        {
            return this._context.T_PREFERENCIA_TIPO_EXPEDICION.Where(x => x.NU_PREFERENCIA == nuPreferencia).Select(w => new PreferenciaAsociarTpExpedicion { tpExpedicion = w.TP_EXPEDICION, nuPreferencia = w.NU_PREFERENCIA }).ToList();
        }

        public virtual void AddTpExpedicionPreferencia(List<PreferenciaAsociarTpExpedicion> tipos)
        {
            foreach (var tipo in tipos)
            {
                T_PREFERENCIA_TIPO_EXPEDICION entity = new T_PREFERENCIA_TIPO_EXPEDICION
                {
                    NU_PREFERENCIA = tipo.nuPreferencia,
                    TP_EXPEDICION = tipo.tpExpedicion,
                    DT_ADDROW = DateTime.Now,
                };

                this._context.T_PREFERENCIA_TIPO_EXPEDICION.Add(entity);
            }
        }

        public virtual void RemoveTpExpedicionPreferencia(List<PreferenciaAsociarTpExpedicion> tipos)
        {
            foreach (var tipo in tipos)
            {
                var linea = this._context.T_PREFERENCIA_TIPO_EXPEDICION.FirstOrDefault(f => f.NU_PREFERENCIA == tipo.nuPreferencia && f.TP_EXPEDICION == tipo.tpExpedicion);

                this._context.T_PREFERENCIA_TIPO_EXPEDICION.Remove(linea);
            }
        }

        #endregion

        #region Preferencia-Classe

        public virtual List<PreferenciaAsociarClase> GetAllClasePreferencia(int nuPreferencia)
        {
            return this._context.T_PREFERENCIA_CLASSE.Where(x => x.NU_PREFERENCIA == nuPreferencia).Select(w => new PreferenciaAsociarClase { codClasse = w.CD_CLASSE, nuPreferencia = w.NU_PREFERENCIA }).ToList();
        }

        public virtual void AddClassePreferencia(List<PreferenciaAsociarClase> clases)
        {
            foreach (var clase in clases)
            {
                T_PREFERENCIA_CLASSE entity = new T_PREFERENCIA_CLASSE
                {
                    NU_PREFERENCIA = clase.nuPreferencia,
                    CD_CLASSE = clase.codClasse,
                    DT_ADDROW = DateTime.Now,
                };

                this._context.T_PREFERENCIA_CLASSE.Add(entity);
            }
        }

        public virtual void RemoveClassePreferencia(List<PreferenciaAsociarClase> clases)
        {
            foreach (var clase in clases)
            {
                var linea = this._context.T_PREFERENCIA_CLASSE.FirstOrDefault(f => f.NU_PREFERENCIA == clase.nuPreferencia && f.CD_CLASSE == clase.codClasse);

                this._context.T_PREFERENCIA_CLASSE.Remove(linea);
            }
        }

        #endregion

        #region Preferencia-Familia

        public virtual List<PreferenciaAsociarFamilia> GetAllFamiliaPreferencia(int nuPreferencia)
        {
            return this._context.T_PREFERENCIA_FAMILIA_PRODUTO.Where(x => x.NU_PREFERENCIA == nuPreferencia).Select(w => new PreferenciaAsociarFamilia { codFamilia = w.CD_FAMILIA_PRODUTO, nuPreferencia = w.NU_PREFERENCIA }).ToList();
        }

        public virtual void AddFamiliasPreferencia(List<PreferenciaAsociarFamilia> familias)
        {
            foreach (var familia in familias)
            {
                T_PREFERENCIA_FAMILIA_PRODUTO entity = new T_PREFERENCIA_FAMILIA_PRODUTO
                {
                    NU_PREFERENCIA = familia.nuPreferencia,
                    CD_FAMILIA_PRODUTO = familia.codFamilia,
                    DT_ADDROW = DateTime.Now,
                };

                this._context.T_PREFERENCIA_FAMILIA_PRODUTO.Add(entity);
            }
        }

        public virtual void RemoveFamiliasPreferencia(List<PreferenciaAsociarFamilia> familias)
        {
            foreach (var familia in familias)
            {
                var linea = this._context.T_PREFERENCIA_FAMILIA_PRODUTO.FirstOrDefault(f => f.NU_PREFERENCIA == familia.nuPreferencia && f.CD_FAMILIA_PRODUTO == familia.codFamilia);

                this._context.T_PREFERENCIA_FAMILIA_PRODUTO.Remove(linea);
            }
        }

        #endregion

        #region Preferencia-Usuario
        public virtual void AddUsuariosPreferencia(List<PreferenciaAsociarUsuario> usuarios)
        {
            foreach (var usuario in usuarios)
            {
                T_PREFERENCIA_REL entity = new T_PREFERENCIA_REL
                {
                    NU_PREFERENCIA_REL = GetNextNuPreferenciaRel(),
                    NU_PREFERENCIA = usuario.nuPreferencia,
                    USERID = usuario.userId,
                    DT_ADDROW = DateTime.Now,
                };

                this._context.T_PREFERENCIA_REL.Add(entity);
            }
        }

        public virtual void RemoveUsuariosPreferencia(List<PreferenciaAsociarUsuario> usuarios)
        {
            foreach (var usuario in usuarios)
            {
                var linea = this._context.T_PREFERENCIA_REL.FirstOrDefault(f => f.NU_PREFERENCIA == usuario.nuPreferencia && f.USERID == usuario.userId);

                this._context.T_PREFERENCIA_REL.Remove(linea);
            }
        }

        #endregion

        #region Preferencia-Equipo
        public virtual void AddEquiposPreferencia(List<PreferenciaAsociarEquipo> equipos)
        {
            foreach (var equipo in equipos)
            {
                T_PREFERENCIA_REL entity = new T_PREFERENCIA_REL
                {
                    NU_PREFERENCIA_REL = GetNextNuPreferenciaRel(),
                    NU_PREFERENCIA = equipo.nuPreferencia,
                    CD_EQUIPO = equipo.codEquipo,
                    DT_ADDROW = DateTime.Now,
                };

                this._context.T_PREFERENCIA_REL.Add(entity);
            }
        }

        public virtual void RemoveEquiposPreferencia(List<PreferenciaAsociarEquipo> equipos)
        {
            foreach (var equipo in equipos)
            {
                var linea = this._context.T_PREFERENCIA_REL.FirstOrDefault(f => f.NU_PREFERENCIA == equipo.nuPreferencia && f.CD_EQUIPO == equipo.codEquipo);

                this._context.T_PREFERENCIA_REL.Remove(linea);
            }
        }

        #endregion
    }
}
