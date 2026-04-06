using System.Collections.Generic;
using System.Linq;
using WIS.Domain.Reportes;
using WIS.Domain.Reportes.Dtos;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class ReporteMapper : Mapper
    {
        public ReporteMapper()
        {
        }

        public virtual Reporte MapToObject(T_REPORTE entity)
        {
            if (entity == null)
                return null;

            var reporte = new Reporte
            {
                Tipo = entity.CD_REPORTE,
                FechaAlta = entity.DT_ADDROW,
                Usuario = entity.CD_USUARIO,
                FechaModificacion = entity.DT_UPDROW,
                Estado = entity.ND_SITUACION,
                NombreArchivo = entity.NM_ARCHIVO,
                Predio = entity.NU_PREDIO,
                Id = entity.NU_REPORTE,
                Contenido = entity.VL_DATA,
                Zona = entity.CD_ZONA,
                RelacionEntidad = new List<ReporteRelacion>()
            };

            if (entity.T_REPORTE_RELACION != null)
            {
                foreach (var relacion in entity.T_REPORTE_RELACION)
                {
                    reporte.RelacionEntidad.Add(this.MapToObject(relacion));
                }
            }

            return reporte;
        }

        public virtual T_REPORTE MapToEntity(Reporte reporte)
        {
            if (reporte == null)
                return null;

            var entity = new T_REPORTE
            {
                CD_REPORTE = reporte.Tipo,
                CD_USUARIO = reporte.Usuario,
                VL_DATA = reporte.Contenido,
                NU_REPORTE = reporte.Id,
                DT_ADDROW = reporte.FechaAlta,
                DT_UPDROW = reporte.FechaModificacion,
                ND_SITUACION = reporte.Estado,
                NM_ARCHIVO = reporte.NombreArchivo,
                NU_PREDIO = reporte.Predio,
                CD_ZONA = reporte.Zona,
                T_REPORTE_RELACION = new List<T_REPORTE_RELACION>()
            };

            if (reporte.RelacionEntidad != null)
            {
                foreach (var relacion in reporte.RelacionEntidad)
                {
                    entity.T_REPORTE_RELACION.Add(this.MapToEntity(reporte, relacion));
                }
            }

            return entity;
        }

        public virtual ReporteRelacion MapToObject(T_REPORTE_RELACION entity)
        {
            if (entity == null)
                return null;

            var tipo = new ReporteRelacion
            {
                Id = entity.NU_REPORTE_RELACION,
                Clave = entity.CD_CLAVE,
                Tabla = entity.NM_TABLA
            };

            return tipo;
        }

        public virtual T_REPORTE_RELACION MapToEntity(Reporte reporte, ReporteRelacion relacion)
        {
            if (relacion == null)
                return null;

            return new T_REPORTE_RELACION
            {
                CD_CLAVE = relacion.Clave,
                NM_TABLA = relacion.Tabla,
                NU_REPORTE = reporte.Id,
            };
        }

        public virtual DtoReporteConfRecepcion MapAgendaToEntity(V_REPORTE_CONF_RECEPCION agenda)
        {
            if (agenda == null)
                return null;
            DtoReporteConfRecepcion dtoReporteInfoAgenda = new DtoReporteConfRecepcion();
            dtoReporteInfoAgenda.FAX = agenda.NU_FAX;
            dtoReporteInfoAgenda.CGCCiente = agenda.CD_CGC_CLIENTE;
            dtoReporteInfoAgenda.AnexoCliente1 = agenda.DS_ANEXO_CLIENTE1;
            dtoReporteInfoAgenda.AnexoCliente2 = agenda.DS_ANEXO_CLIENTE2;
            dtoReporteInfoAgenda.AnexoCliente3 = agenda.DS_ANEXO_CLIENTE3;
            dtoReporteInfoAgenda.AnexoCliente4 = agenda.DS_ANEXO_CLIENTE4;
            dtoReporteInfoAgenda.DescripcionSituacion = agenda.DS_SITUACAO;
            dtoReporteInfoAgenda.DescripcionRuta = agenda.DS_ROTA;
            dtoReporteInfoAgenda.Localidad = agenda.ID_LOCALIDAD;
            dtoReporteInfoAgenda.Cuidad = agenda.CD_CIUDAD;
            dtoReporteInfoAgenda.DescripcionLugar = agenda.DS_LUGAR;
            dtoReporteInfoAgenda.CD_SUBDIV = agenda.CD_SUBDIV;
            dtoReporteInfoAgenda.NM_SUBDIVISION = agenda.NM_SUBDIVISION;
            dtoReporteInfoAgenda.Pais = agenda.CD_PAIS;
            dtoReporteInfoAgenda.DescripcionPais = agenda.DS_PAIS;
            dtoReporteInfoAgenda.Lugar = agenda.CD_LUGAR;
            dtoReporteInfoAgenda.NombreEmpresa = agenda.NM_EMPRESA;
            dtoReporteInfoAgenda.DescripcionRecepcionExterno = agenda.DS_RECEPCION_EXTERNO;
            dtoReporteInfoAgenda.Id = agenda.NU_AGENDA;
            dtoReporteInfoAgenda.Empresa = agenda.CD_EMPRESA;
            dtoReporteInfoAgenda.TipoDocumento = agenda.CD_TIPO_DOCUMENTO;
            dtoReporteInfoAgenda.Documento = agenda.NU_DOCUMENTO;
            dtoReporteInfoAgenda.Situacion = agenda.CD_SITUACAO;
            dtoReporteInfoAgenda.Operacion = agenda.CD_OPERACAO;
            dtoReporteInfoAgenda.FechaAlta = agenda.DT_ADDROW;
            dtoReporteInfoAgenda.FechaModificacion = agenda.DT_UPDROW;
            dtoReporteInfoAgenda.Puerta = agenda.CD_PORTA;
            dtoReporteInfoAgenda.FechaInicio = agenda.DT_INICIO;
            dtoReporteInfoAgenda.FechaFin = agenda.DT_FIN;
            dtoReporteInfoAgenda.Placa = agenda.DS_PLACA;
            dtoReporteInfoAgenda.DUA = agenda.NU_DUA;
            dtoReporteInfoAgenda.Anexo1 = agenda.DS_ANEXO1;
            dtoReporteInfoAgenda.Anexo2 = agenda.DS_ANEXO2;
            dtoReporteInfoAgenda.Anexo3 = agenda.DS_ANEXO3;
            dtoReporteInfoAgenda.Anexo4 = agenda.DS_ANEXO4;
            dtoReporteInfoAgenda.IdEnvioDocumento = agenda.ID_ENVIO_DOCUMENTACION;
            dtoReporteInfoAgenda.FunEnvioDocumento = agenda.CD_FUNC_ENVIO_DOCU;
            dtoReporteInfoAgenda.Averia = agenda.ID_AVERIA;
            dtoReporteInfoAgenda.IdFechaVencimiento = agenda.ID_FECHA_VENCIMIENTO;
            dtoReporteInfoAgenda.IdPeso = agenda.ID_PESO;
            dtoReporteInfoAgenda.IdVolumen = agenda.ID_VOLUMEN;
            dtoReporteInfoAgenda.Cliente = agenda.CD_CLIENTE;
            dtoReporteInfoAgenda.FechaFacturacion = agenda.DT_FACTURACION;
            dtoReporteInfoAgenda.FechaCierre = agenda.DT_CIERRE;
            dtoReporteInfoAgenda.TipoRecepcion = agenda.TP_RECEPCION;
            dtoReporteInfoAgenda.FuncionarioAsignado = agenda.CD_FUNCIONARIO_ASIGNADO;
            dtoReporteInfoAgenda.TipoRecepcionExterno = agenda.TP_RECEPCION_EXTERNO;
            dtoReporteInfoAgenda.Agente = agenda.CD_AGENTE;
            dtoReporteInfoAgenda.TipoAgente = agenda.TP_AGENTE;
            dtoReporteInfoAgenda.DescripcionCliente = agenda.DS_CLIENTE;
            dtoReporteInfoAgenda.Ruta = agenda.CD_ROTA;
            dtoReporteInfoAgenda.DS_ENDERECO = agenda.DS_ENDERECO;
            dtoReporteInfoAgenda.Barrio = agenda.DS_BAIRRO;
            dtoReporteInfoAgenda.GLN = agenda.CD_GLN;
            dtoReporteInfoAgenda.Incripcion = agenda.NU_INSCRICAO;
            dtoReporteInfoAgenda.Telefono = agenda.NU_TELEFONE;
            dtoReporteInfoAgenda.CEP = agenda.CD_CEP;
            return dtoReporteInfoAgenda;
        }

        public virtual DtoReporteConfRecepcionDetalle MapAgendaDetalleToObject(V_REPORTE_CONF_RECEPCION_DET det)
        {
            DtoReporteConfRecepcionDetalle detalle = new DtoReporteConfRecepcionDetalle();
            detalle.Anexo1 = det.DS_ANEXO1;
            detalle.Anexo2 = det.DS_ANEXO2;
            detalle.Anexo3 = det.DS_ANEXO3;
            detalle.Anexo4 = det.DS_ANEXO4;
            detalle.Altura = det.VL_ALTURA;
            detalle.Largo = det.VL_LARGURA;
            detalle.Profundidad = det.VL_PROFUNDIDADE;
            detalle.ManejoFecha = det.TP_MANEJO_FECHA;
            detalle.AvisoAjuste = det.VL_AVISO_AJUSTE;
            detalle.DsHelpColector = det.DS_HELP_COLECTOR;
            detalle.SubBulto = det.QT_SUBBULTO;
            detalle.Exclusivo = det.CD_EXCLUSIVO;
            detalle.UnidadDistribucion = det.QT_UND_DISTRIBUCION;
            detalle.CantidadDiasValidosLiberacion = det.QT_DIAS_VALIDADE_LIBERACION;
            detalle.CantidadBulto = det.QT_UND_BULTO;
            detalle.ManejoTomaDato = det.ID_MANEJA_TOMA_DATO;
            detalle.Anexo5 = det.DS_ANEXO5;
            detalle.GrupoConsulta = det.CD_GRUPO_CONSULTA;
            detalle.DescripcionDisplay = det.DS_DISPLAY;
            detalle.PrecioSegDist = det.VL_PRECIO_SEG_DISTR;
            detalle.PrecioSegStock = det.VL_PRECIO_SEG_STOCK;
            detalle.PrecioDistibucion = det.VL_PRECIO_DISTRIB;
            detalle.PrecioEgreso = det.VL_PRECIO_EGRESO;
            detalle.PrecioIngreso = det.VL_PRECIO_INGRESO;
            detalle.PrecioStock = det.VL_PRECIO_STOCK;
            detalle.UnidadMedodaFac = det.CD_UND_MEDIDA_FACT;
            detalle.ProductoUnico = det.CD_PRODUTO_UNICO;
            detalle.Ramo = det.CD_RAMO_PRODUTO;
            detalle.DescripcionFamilia = det.DS_FAMILIA_PRODUTO;
            detalle.DescripcionUnidadMedida = det.DS_UNIDADE_MEDIDA;
            detalle.DescripcionRotatividad = det.DS_ROTATIVIDADE;
            detalle.DescripcionClase = det.DS_CLASSE;
            detalle.Id = det.NU_AGENDA;
            detalle.Producto = det.CD_PRODUTO;
            detalle.Lote = det.NU_IDENTIFICADOR?.Trim()?.ToUpper();
            detalle.Faixa = det.CD_FAIXA;
            detalle.Empresa = det.CD_EMPRESA;
            detalle.Situacion = det.CD_SITUACAO;
            detalle.CantidadAgendada = det.QT_AGENDADO;
            detalle.CantidadCrossDocking = det.QT_CROSS_DOCKING;
            detalle.Vencimiento = det.DT_FABRICACAO;
            detalle.Precio = det.VL_PRECIO;
            detalle.CantidadRecibida = det.QT_RECIBIDA;
            detalle.FechaRecepcion = det.DT_ACEPTADA_RECEPCION;
            detalle.FuncionarioAceptoRecepcion = det.CD_FUNC_ACEPTO_RECEPCION;
            detalle.FechaAlta = det.DT_ADDROW;
            detalle.FechaModificacion = det.DT_UPDROW;
            detalle.CantidadAceptada = det.QT_ACEPTADA;
            detalle.CantidadOriginal = det.QT_AGENDADO_ORIGINAL;
            detalle.CantidadFicticia = det.QT_RECIBIDA_FICTICIA;
            detalle.CIF = det.VL_CIF;
            detalle.ProductoEmpresa = det.CD_PRODUTO_EMPRESA;
            detalle.NAM = det.CD_NAM;
            detalle.Mercadologico = det.CD_MERCADOLOGICO;
            detalle.SgProducto = det.SG_PRODUTO;
            detalle.TipoPesoProducto = det.TP_PESO_PRODUTO;
            detalle.DescripcionDiferPesoQtDe = det.DS_DIFER_PESO_QTDE;
            detalle.DescripcionProducto = det.DS_PRODUTO;
            detalle.UnidadMedida = det.CD_UNIDADE_MEDIDA;
            detalle.Famila = det.CD_FAMILIA_PRODUTO;
            detalle.Rotividad = det.CD_ROTATIVIDADE;
            detalle.Clase = det.CD_CLASSE;
            detalle.CantidadMinima = det.QT_ESTOQUE_MINIMO;
            detalle.CantidadMaxima = det.QT_ESTOQUE_MAXIMO;
            detalle.PesoLiquido = det.PS_LIQUIDO;
            detalle.PesoBruto = det.PS_BRUTO;
            detalle.FtConversac = det.FT_CONVERSAO;
            detalle.Volumen = det.VL_CUBAGEM;
            detalle.PrecioVenta = det.VL_PRECO_VENDA;
            detalle.ValorCostoUltimaEntrada = det.VL_CUSTO_ULT_ENT;
            detalle.Origen = det.CD_ORIGEM;
            detalle.DescripcionReducida = det.DS_REDUZIDA;
            detalle.Nivel = det.CD_NIVEL;
            detalle.EnidadEmbalaje = det.CD_UNID_EMB;
            detalle.SituacionProducto = det.CD_SITUACAO_PRODUTO;
            detalle.FechaSituacion = det.DT_SITUACAO;
            detalle.DiasValidos = det.QT_DIAS_VALIDADE;
            detalle.DiasDuracion = det.QT_DIAS_DURACAO;
            detalle.IdCrossDocking = det.ID_CROSS_DOCKING;
            detalle.IdRedondeoValidez = det.ID_REDONDEO_VALIDEZ;
            detalle.Agrupacion = det.ID_AGRUPACION;
            detalle.ManejaIdentificador = det.ID_MANEJO_IDENTIFICADOR;
            detalle.Display = det.TP_DISPLAY;
            return detalle;
        }

        public virtual List<DtoDetallesPackingListSinLpn> MapToObjectListaEmpaque(List<V_REPORTE_PACKING_LIST_SIN_LPN> entities, T_CLIENTE agente)
        {
            if (entities == null || entities.Count == 0)
                return new List<DtoDetallesPackingListSinLpn>();

            var listaEmpaqueData = new List<DtoDetallesPackingListSinLpn>();

            foreach (var entity in entities)
            {
                string direccion = string.IsNullOrEmpty(entity.DS_ENDERECO) ? entity.CLI_DS_ENDERECO : entity.DS_ENDERECO;

                var contenedor = new DtoListaAgenteContenedorPackingListSinLpn
                {
                    TipoContenedor = entity.DS_TIPO_CONTENEDOR,
                    Contenedor = entity.NU_CONTENEDOR,
                    IdExternoContenedor = entity.ID_EXTERNO_CONTENEDOR,
                    Bulto = entity.QT_BULTO_CONTENEDOR ?? 1,
                    Detalles = new List<DtoDetalleContenedorPackingListSinLpn>()
                };

                var detalle = new DtoDetalleContenedorPackingListSinLpn
                {
                    Pedido = entity.NU_PEDIDO,
                    Producto = entity.CD_PRODUTO,
                    Descripcion = entity.DS_PRODUTO,
                    Cantidad = entity.QT_PRODUTO ?? 0,
                    Identificador = entity.NU_IDENTIFICADOR?.Trim()?.ToUpper(),
                    Vencimiento = entity.VENCIMIENTO
                };

                var existeDetalle = listaEmpaqueData
                    .FirstOrDefault(x => x.CodigoCamion == entity.CD_CAMION
                        && x.Empresa == entity.CD_EMPRESA
                        && x.CodigoCliente == entity.CD_CLIENTE
                        && x.CodigoAgente == entity.CD_AGENTE
                        && x.DireccionEntrega == direccion);

                if (existeDetalle == null)
                {
                    var newData = new DtoDetallesPackingListSinLpn
                    {
                        DireccionEntrega = direccion,
                        DireccionEmpresaBase = entity.DS_ENDERECO,
                        CodigoCamion = entity.CD_CAMION,
                        DescripcionCamion = entity.DS_CAMION,
                        CodigoAgente = entity.CD_AGENTE,
                        DescripcionAgente = entity.DS_CLIENTE,
                        TipoAgente = entity.TP_AGENTE,
                        CodigoTransportista = entity.CD_TRANSPORTADORA,
                        DescripcionTransportista = entity.DS_TRANSPORTADORA,
                        Empresa = entity.CD_EMPRESA,
                        DescripcionEmpresa = entity.NM_EMPRESA,
                        Fecha = entity.FECHA_EGRESO,
                        CodigoCliente = entity.CD_CLIENTE,
                        Predio = entity.NU_PREDIO,
                        Matricula = entity.CD_PLACA_CARRO,
                        Ruta = entity.CD_ROTA,
                        DescripcionRuta = entity.DS_ROTA,
                        Contenedores = new List<DtoListaAgenteContenedorPackingListSinLpn> { contenedor }
                    };

                    if (agente != null)
                    {
                        newData.RazonSocial = agente.CD_CGC_CLIENTE;
                        newData.Telefono = agente.NU_TELEFONE;
                        newData.Rut = agente.NU_INSCRICAO;
                        newData.DireccionDeposito = agente.DS_ENDERECO;
                    }

                    contenedor.Detalles.Add(detalle);
                    listaEmpaqueData.Add(newData);
                }
                else
                {
                    var contenedorExistente = existeDetalle.Contenedores
                        .FirstOrDefault(x => x.Contenedor == contenedor.Contenedor
                            && x.TipoContenedor == contenedor.TipoContenedor);

                    if (contenedorExistente == null)
                    {
                        contenedor.Detalles.Add(detalle);
                        existeDetalle.Contenedores.Add(contenedor);
                    }
                    else
                    {
                        contenedorExistente.Detalles.Add(detalle);
                    }
                }
            }

            return listaEmpaqueData;
        }
    }
}
