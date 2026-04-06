using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Extensions;
using WIS.Domain.General;
using WIS.Domain.Porteria;
using WIS.Domain.Recepcion;
using WIS.Domain.Services.Interfaces;
using WIS.Extension;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class PorteriaRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly PorteriaMapper _mapper;
        protected readonly DominioMapper _mapperDominio;
        protected readonly AgenteMapper _mapperAgente;
        protected readonly IDapper _dapper;

        public PorteriaRepository(WISDB context, string cdAplicacion, int userId, IDapper dapper)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapperDominio = new DominioMapper();
            this._mapperAgente = new AgenteMapper();
            this._mapper = new PorteriaMapper(this._mapperAgente);
            this._dapper = dapper;
        }

        public virtual void AddRegistroVehiculo(PorteriaRegistroVehiculo obj)
        {
            obj.NU_PORTERIA_VEHICULO = _context.GetNextSequenceValueInt(_dapper, "S_NU_PORTERIA_VEHICULO");
            obj.DT_ADDROW = DateTime.Now;
            T_PORTERIA_REGISTRO_VEHICULO entity = this._mapper.Map(obj);
            this._context.T_PORTERIA_REGISTRO_VEHICULO.Add(entity);
        }
        public virtual void AddRegistroPersona(PorteriaRegistroPersona obj)
        {
            obj.NU_PORTERIA_REGISTRO_PERSONA = _context.GetNextSequenceValueInt(_dapper, "S_NU_PORTERIA_REGISTRO_PERSONA");
            obj.DT_ADDROW = DateTime.Now;
            T_PORTERIA_REGISTRO_PERSONA entity = this._mapper.Map(obj);
            this._context.T_PORTERIA_REGISTRO_PERSONA.Add(entity);
        }
        public virtual void AddPersona(PorteriaPersona obj)
        {
            obj.NU_POTERIA_PERSONA = _context.GetNextSequenceValueInt(_dapper, "S_NU_POTERIA_PERSONA");
            obj.DT_ADDROW = DateTime.Now;
            T_PORTERIA_PERSONA entity = this._mapper.Map(obj);
            this._context.T_PORTERIA_PERSONA.Add(entity);
        }
        public virtual void AddPorteriaVehiculoAgenda(PorteriaVehiculoAgenda obj)
        {
            obj.NU_PORTERIA_VEHICULO_AGENDA = _context.GetNextSequenceValueInt(_dapper, "S_NU_PORTERIA_VEHICULO_AGENDA");

            T_PORTERIA_VEHICULO_AGENDA entity = _mapper.Map(obj);

            _context.T_PORTERIA_VEHICULO_AGENDA.Add(entity);

        }
        public virtual void AddPorteriaVehiculoCamion(PorteriaVehiculoCamion obj)
        {
            obj.NU_PORTERIA_VEHICULO_CAMION = _context.GetNextSequenceValueInt(_dapper, "S_NU_PORTERIA_VEHICULO_CAMION");

            T_PORTERIA_VEHICULO_CAMION entity = _mapper.Map(obj);

            _context.T_PORTERIA_VEHICULO_CAMION.Add(entity);

        }
        public virtual void AddVehiculoObjeto(PorteriaVehiculoObjeto obj)
        {
            obj.NU_PORTERIA_VEHICULO_OBJETO = _context.GetNextSequenceValueInt(_dapper, "S_NU_PORTERIA_VEHICULO_OBJETO");

            T_PORTERIA_VEHICULO_OBJETO entity = _mapper.Map(obj);

            _context.T_PORTERIA_VEHICULO_OBJETO.Add(entity);

        }

        public virtual void RemovePorteriaVehiculoAgenda(int nuVehiculo, int nuAgenda)
        {
            T_PORTERIA_VEHICULO_AGENDA entity = this._context.T_PORTERIA_VEHICULO_AGENDA
                .FirstOrDefault(d => d.NU_PORTERIA_VEHICULO == nuVehiculo && d.NU_AGENDA == nuAgenda);
            T_PORTERIA_VEHICULO_AGENDA attachedEntity = _context.T_PORTERIA_VEHICULO_AGENDA.Local
                .FirstOrDefault(d => d.NU_PORTERIA_VEHICULO == nuVehiculo && d.NU_AGENDA == nuAgenda);

            if (attachedEntity != null)
            {
                this._context.T_PORTERIA_VEHICULO_AGENDA.Remove(attachedEntity);
            }
            else if (entity != null)
            {
                this._context.T_PORTERIA_VEHICULO_AGENDA.Remove(entity);
            }
        }

        public virtual void RemovePorteriaVehiculoCamion(int nuVehiculo, int camion)
        {
            T_PORTERIA_VEHICULO_CAMION entity = this._context.T_PORTERIA_VEHICULO_CAMION
                .FirstOrDefault(d => d.NU_PORTERIA_VEHICULO == nuVehiculo
                    && d.CD_CAMION == camion);
            T_PORTERIA_VEHICULO_CAMION attachedEntity = _context.T_PORTERIA_VEHICULO_CAMION.Local
                .FirstOrDefault(d => d.NU_PORTERIA_VEHICULO == nuVehiculo
                    && d.CD_CAMION == camion);

            if (attachedEntity != null)
            {
                this._context.T_PORTERIA_VEHICULO_CAMION.Remove(attachedEntity);
            }
            else if (entity != null)
            {
                this._context.T_PORTERIA_VEHICULO_CAMION.Remove(entity);
            }
        }

        public virtual void UpdateRegistroVehiculo(PorteriaRegistroVehiculo obj)
        {
            obj.DT_UPDROW = DateTime.Now;

            T_PORTERIA_REGISTRO_VEHICULO entity = this._mapper.Map(obj);
            T_PORTERIA_REGISTRO_VEHICULO attachedEntity = _context.T_PORTERIA_REGISTRO_VEHICULO.Local
                .FirstOrDefault(x => x.NU_PORTERIA_VEHICULO == entity.NU_PORTERIA_VEHICULO);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_PORTERIA_REGISTRO_VEHICULO.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateRegistroPersona(PorteriaRegistroPersona obj)
        {
            obj.DT_UPDROW = DateTime.Now;

            T_PORTERIA_REGISTRO_PERSONA entity = this._mapper.Map(obj);
            T_PORTERIA_REGISTRO_PERSONA attachedEntity = _context.T_PORTERIA_REGISTRO_PERSONA.Local
                .FirstOrDefault(x => x.NU_PORTERIA_REGISTRO_PERSONA == entity.NU_PORTERIA_REGISTRO_PERSONA);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_PORTERIA_REGISTRO_PERSONA.Attach(entity);
                _context.Entry<T_PORTERIA_REGISTRO_PERSONA>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateVehiculoObjeto(PorteriaVehiculoObjeto obj)
        {
            T_PORTERIA_VEHICULO_OBJETO entity = this._mapper.Map(obj);
            T_PORTERIA_VEHICULO_OBJETO attachedEntity = _context.T_PORTERIA_VEHICULO_OBJETO.Local
                .FirstOrDefault(x => x.NU_PORTERIA_VEHICULO_OBJETO == entity.NU_PORTERIA_VEHICULO_OBJETO);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_PORTERIA_VEHICULO_OBJETO.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual bool AnyPorteriaPersona(string nuDocumento)
        {
            return this._context.T_PORTERIA_PERSONA
                .Any(w => w.NU_DOCUMENTO == nuDocumento && w.ND_TP_DOCUMENTO == "CDI"
            );
        }
        public virtual bool PersonaTieneRegistroSalidaSinMarcar(int? NU_POTERIA_PERSONA)
        {
            return _context.T_PORTERIA_REGISTRO_PERSONA.Any(x => x.NU_POTERIA_PERSONA == NU_POTERIA_PERSONA
            && x.DT_PERSONA_SALIDA == null
            && x.DT_PERSONA_ENTRADA != null);
        }
        public virtual bool AnyVehiculoObjeto(string codigo, string tipo, int nuVehiculo)
        {
            return _context.T_PORTERIA_VEHICULO_OBJETO.Any(w => w.CD_OBJETO == codigo && w.TP_OBJETO == tipo && w.NU_PORTERIA_VEHICULO == nuVehiculo);
        }
        public virtual bool AnyVehiculoAgenda(int nuVehiculo, int nuAgenda)
        {
            return _context.T_PORTERIA_VEHICULO_AGENDA.Any(w => w.NU_PORTERIA_VEHICULO == nuVehiculo && w.NU_AGENDA == nuAgenda);
        }


        public virtual PorteriaRegistroVehiculo GetRegistroVehiculoByNum(int numVehiculo)
        {
            T_PORTERIA_REGISTRO_VEHICULO entity = this._context.T_PORTERIA_REGISTRO_VEHICULO
                .AsNoTracking()
                .FirstOrDefault(f => f.NU_PORTERIA_VEHICULO == numVehiculo);

            if (entity == null) return null;

            PorteriaRegistroVehiculo obj = _mapper.Map(entity);

            return obj;
        }

        public virtual List<PorteriaRegistroVehiculo> GetRegistroVehiculoByKeysPartial(string searchValue)
        {
            return this._context.T_PORTERIA_REGISTRO_VEHICULO
                .AsNoTracking()
                .Where(w => w.DT_PORTERIA_ENTRADA != null
                    && (w.VL_MATRICULA_1 != null && w.VL_MATRICULA_1.ToLower().Contains(searchValue.ToLower()))
                    || w.NU_PORTERIA_VEHICULO.ToString().Contains(searchValue.ToLower()))
                .ToList()
                .Select(w => this._mapper.Map(w))
                .ToList();
        }

        public virtual PorteriaRegistroVehiculo GetRegistroVehiculoByMatricula(string matricula1)
        {

            T_PORTERIA_REGISTRO_VEHICULO entity = this._context.T_PORTERIA_REGISTRO_VEHICULO
                .AsNoTracking()
                .OrderByDescending(od => od.DT_ADDROW)
                .FirstOrDefault(w => w.VL_MATRICULA_1 == matricula1
                    && w.DT_PORTERIA_SALIDA == null
                    && w.DT_PORTERIA_ENTRADA != null);

            if (entity == null) return null;

            List<T_PORTERIA_REGISTRO_PERSONA> colPersonas = _context.T_PORTERIA_REGISTRO_PERSONA.Where(w => w.NU_PORTERIA_VEHICULO_ENTRADA == entity.NU_PORTERIA_VEHICULO).ToList();

            PorteriaRegistroVehiculo obj = _mapper.Map(entity, colPersonas);

            return obj;
        }

        public virtual PorteriaRegistroVehiculo GetPreRegistroVehiculoByMatricula(string matricula1)
        {
            T_PORTERIA_REGISTRO_VEHICULO entity = this._context.T_PORTERIA_REGISTRO_VEHICULO.AsNoTracking()
                   .OrderByDescending(od => od.DT_ADDROW)
                   .FirstOrDefault(w =>
                       w.VL_MATRICULA_1 == matricula1
                       && w.DT_PORTERIA_SALIDA == null
                       && w.DT_PORTERIA_ENTRADA == null
                  );


            if (entity == null) return null;

            List<T_PORTERIA_REGISTRO_PERSONA> colPersonas = _context.T_PORTERIA_REGISTRO_PERSONA.Where(w => w.NU_PORTERIA_VEHICULO_ENTRADA == entity.NU_PORTERIA_VEHICULO).ToList();

            PorteriaRegistroVehiculo obj = _mapper.Map(entity, colPersonas);

            return obj;
        }
        public virtual PorteriaRegistroPersona GetPreRegistroPersona(int nuPersona, int nuVehiculo)
        {
            return _mapper.Map(
                this._context.T_PORTERIA_REGISTRO_PERSONA.AsNoTracking()
                .Where(w => w.NU_POTERIA_PERSONA == nuPersona
                 && w.DT_PERSONA_ENTRADA == null && w.DT_PERSONA_SALIDA == null
                 && w.NU_PORTERIA_VEHICULO_ENTRADA == nuVehiculo
                ).FirstOrDefault()
                );
        }
        public virtual PorteriaRegistroVehiculo GetRegistroVehiculoByDocumento(string documento)
        {

            V_PORTERIA_PERSONA entityPersona = this._context.V_PORTERIA_PERSONA.AsNoTracking()
                   .OrderByDescending(od => od.DT_ADDROW)
                   .FirstOrDefault(w =>
                       w.NU_DOCUMENTO == documento
                  );


            if (entityPersona == null) return null;

            T_PORTERIA_REGISTRO_VEHICULO entity = this._context.T_PORTERIA_REGISTRO_VEHICULO.AsNoTracking()
                 .OrderByDescending(od => od.DT_ADDROW)
                 .FirstOrDefault(w =>
                     w.NU_PORTERIA_VEHICULO == entityPersona.NU_PORTERIA_VEHICULO_ENTRADA && w.DT_PORTERIA_SALIDA == null
                     && w.DT_PORTERIA_ENTRADA != null
                );

            if (entity == null) return null;

            List<T_PORTERIA_REGISTRO_PERSONA> colPersonas = _context.T_PORTERIA_REGISTRO_PERSONA.Where(w => w.NU_PORTERIA_VEHICULO_ENTRADA == entity.NU_PORTERIA_VEHICULO).ToList();

            PorteriaRegistroVehiculo obj = _mapper.Map(entity, colPersonas);

            return obj;
        }
        public virtual PorteriaRegistroPersona getRegistroPersonaByNum(int numRegistroPersona)
        {
            T_PORTERIA_REGISTRO_PERSONA curr = _context.T_PORTERIA_REGISTRO_PERSONA.AsNoTracking().FirstOrDefault(f => f.NU_PORTERIA_REGISTRO_PERSONA == numRegistroPersona);
            return this._mapper.Map(curr);
        }
        public virtual PorteriaPersona GetPorteriaPersona(string nuDocumento)
        {
            return _mapper.Map(this._context.T_PORTERIA_PERSONA.FirstOrDefault(w =>
                      w.NU_DOCUMENTO == nuDocumento && w.ND_TP_DOCUMENTO == "PERTPDOCCDI"
           ));
        }
        public virtual PorteriaPersona GetPorteriaPersona(int nuPorteriaPersona)
        {
            return _mapper.Map(this._context.T_PORTERIA_PERSONA.AsNoTracking().FirstOrDefault(w =>
                      w.NU_POTERIA_PERSONA == nuPorteriaPersona
           ));
        }
        public virtual List<PorteriaPersona> GetPersonaByKeysPartial(string searchValue)
        {
            return this._context.T_PORTERIA_PERSONA.AsNoTracking()
                   .Where(w =>
                         (w.NU_DOCUMENTO != null && w.NU_DOCUMENTO.ToLower().Contains(searchValue.ToLower()) && w.ND_TP_DOCUMENTO == "PERTPDOCCDI")
                        || (w.AP_PERSONA != null && w.AP_PERSONA.ToLower().Contains(searchValue.ToLower()))
                        || (w.NM_PERSONA != null && w.NM_PERSONA.ToLower().Contains(searchValue.ToLower()))
                    ).ToList()
                   .Select(w => this._mapper.Map(w)).ToList();
        }
        public virtual List<PorteriaPersona> GetPersonaParaEntradaByKeysPartial(string searchValue)
        {
            return this._context.T_PORTERIA_PERSONA.AsNoTracking()
          .Where(w =>
                ((w.NU_DOCUMENTO != null && w.NU_DOCUMENTO.ToLower().Contains(searchValue.ToLower()) && w.ND_TP_DOCUMENTO == "PERTPDOCCDI")
               || (w.AP_PERSONA != null && w.AP_PERSONA.ToLower().Contains(searchValue.ToLower()))
               || (w.NM_PERSONA != null && w.NM_PERSONA.ToLower().Contains(searchValue.ToLower())))
               && !this._context.T_PORTERIA_REGISTRO_PERSONA.Any(a => a.NU_POTERIA_PERSONA == w.NU_POTERIA_PERSONA
                && a.DT_PERSONA_ENTRADA != null && a.DT_PERSONA_SALIDA == null
               )
           ).ToList()
          .Select(w => this._mapper.Map(w)).ToList();
        }
        public virtual List<Container> GetContainerParaPreRegistroByKeysPartial(string searchValue)
        {
            return this._context.V_PORTERIA_CONT_PRE_REG.AsNoTracking()
           .Where(w =>
                (w.NU_CONTAINER != null && w.NU_CONTAINER.ToLower().Contains(searchValue.ToLower()))
            )
           .ToList()
           .Select(w => new Container
           {
               Id = w.NU_SEQ_CONTAINER,
               Numero = w.NU_CONTAINER,
           })
           .ToList();

        }
        public virtual List<PorteriaPersona> GetPersonaParaSalidaByKeysPartial(string searchValue)
        {
            return this._context.V_PORTERIA_PERSONA.AsNoTracking()
                   .Where(w =>
                        ((w.NU_DOCUMENTO != null && w.NU_DOCUMENTO.ToLower().Contains(searchValue.ToLower()) && w.ND_TP_DOCUMENTO == "PERTPDOCCDI")
                        || (w.AP_PERSONA != null && w.AP_PERSONA.ToLower().Contains(searchValue.ToLower()))
                        || (w.NM_PERSONA != null && w.NM_PERSONA.ToLower().Contains(searchValue.ToLower()))
                        )
                        && w.DT_PERSONA_SALIDA == null
                        && w.DT_PERSONA_ENTRADA != null
                    ).ToList()
                   .Select(w => new PorteriaPersona
                   {
                       NU_POTERIA_PERSONA = w.NU_POTERIA_PERSONA ?? -2,
                       NM_PERSONA = w.NM_PERSONA,
                       AP_PERSONA = w.AP_PERSONA,

                   }).ToList();
        }
        public virtual PorteriaPersona GetPorteriaPersonaByDocumento(string nuDocumento)
        {
            return _mapper.Map(this._context.T_PORTERIA_PERSONA.AsNoTracking().FirstOrDefault(w =>
                      w.NU_DOCUMENTO == nuDocumento && w.ND_TP_DOCUMENTO == "PERTPDOCCDI"
           ));
        }
        public virtual List<short> GetContainersDeVehiculo(int numPorVehiculo)
        {
            return this._context.T_PORTERIA_VEHICULO_OBJETO
               .Where(w => w.NU_PORTERIA_VEHICULO == numPorVehiculo && w.TP_OBJETO == "CON")
               .ToList()
               .Select(w => w.CD_OBJETO.ToNumber<short>())
               .ToList();
            ;
        }


        public virtual List<DominioDetalle> GetTipoPorterias()
        {
            return this._context.T_DET_DOMINIO.AsNoTracking().ToList()
                   .Where(w => w.CD_DOMINIO == "PORTPREG").ToList()
                   .Select(w => this._mapperDominio.MapToObject(w)).ToList();

        }
        public virtual List<DominioDetalle> GetMotivos()
        {
            return this._context.T_DET_DOMINIO.AsNoTracking()
                   .Where(w => w.CD_DOMINIO == "PORREGMOT").ToList()
                   .Select(w => this._mapperDominio.MapToObject(w)).ToList();

        }
        public virtual List<Agente> GetAgenteByKeysPartialAndEmpresa(string valueSearch, int cdEmpresa)
        {
            return this._context.T_CLIENTE.AsNoTracking()
                   .Where(w =>
                        w.CD_AGENTE.ToLower().Contains(valueSearch.ToLower())
                        && w.TP_AGENTE == "PRO"
                        && w.CD_EMPRESA == cdEmpresa
                    ).ToList()
                   .Select(w => this._mapperAgente.MapToObject(w)).ToList();

        }
        public virtual List<DominioDetalle> GetTipoTransportes()
        {
            return this._context.T_DET_DOMINIO.AsNoTracking().ToList()
                   .Where(w => w.CD_DOMINIO == "PORREGTRA").ToList()
                   .Select(w => this._mapperDominio.MapToObject(w)).ToList();

        }
        public virtual List<PorteriaSector> GetSectoresByPredio(string predio)
        {
            return this._context.T_PORTERIA_SECTOR.AsNoTracking().ToList()
                   .Where(w => (w.NU_PREDIO == predio || string.IsNullOrEmpty(predio))
                   ).ToList()
                   .Select(w => this._mapper.Map(w)).ToList();

        }
        public virtual PorteriaSector GetSector(string predio, short? puerta)
        {
            return this._mapper.Map(

                this._context.T_PORTERIA_SECTOR.AsNoTracking()
                .FirstOrDefault(w => w.CD_PORTA == puerta && w.NU_PREDIO == predio)

                );
        }
        public virtual PorteriaSector GetSector(string codigo)
        {
            return this._mapper.Map(

                 this._context.T_PORTERIA_SECTOR.AsNoTracking()
                 .FirstOrDefault(w => w.CD_SECTOR == codigo)

                 );
        }


        public virtual void UpdateContainer(Container obj)
        {
            obj.FechaModificacion = DateTime.Now;

            T_CONTAINER entity = this._mapper.Map(obj);
            T_CONTAINER attachedEntity = _context.T_CONTAINER.Local
                .FirstOrDefault(c => c.NU_CONTAINER == entity.NU_CONTAINER
                    && c.NU_SEQ_CONTAINER == entity.NU_SEQ_CONTAINER);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_CONTAINER.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual List<Container> GetContainersAgenda(int nuAgenda)
        {
            return _context.T_RECEPC_AGENDA_CONTAINER_REL
                .Include("T_CONTAINER")
                .AsNoTracking()
                .Where(w => w.NU_AGENDA == nuAgenda)
                .ToList()
                .Select(w => this._mapper.Map(w.T_CONTAINER)).ToList();
        }

        public virtual Container GetContainer(short nuSeqConteiner)
        {
            return this._mapper.Map(this._context.T_CONTAINER
                .AsNoTracking()
                .FirstOrDefault(w => w.NU_SEQ_CONTAINER == nuSeqConteiner));
        }
    }
}
