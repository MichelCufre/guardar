namespace WIS.Domain.Validation
{
    public class ValidationMessage
    {
        #region -- General --

        public const string WMSAPI_msg_Error_InterfazExternaInvalida = "La interfaz '{0}' no corresponde a la interfaz externa '{1}'";
        public const string WMSAPI_msg_Error_InterfazSinEstado = "La interfaz '{0}' no tiene estado '{1}'";
        public const string WMSAPI_msg_Error_ErrorInterfaz = "Error Interfaz Nro. '{0}'";
        public const string WMSAPI_msg_Error_DatosRequeridos = "Datos requeridos";

        public const string WMSAPI_msg_Error_ModelStateValidation = "Ocurrieron uno o más errores de validación.";

        public const string WMSAPI_msg_Error_Conflict409 = "Conflicto.";
        public const string WMSAPI_msg_Error_RangeValidation = "El campo '{0}' debe ser un entero válido, comprendido entre '{1}' y '{2}'.";
        public const string WMSAPI_msg_Error_DateTimeIsoValidation = "El campo '{0}' no cumple con el formato esperado. Valor enviado: '{1}' Formato esperado: [yyyy]-[MM]-[dd]T[HH]:[mm]:[ss]";

        public const string WMSAPI_msg_Error_LargoStringValidation = "El campo '{0}' debe ser un string de mínimo '{1}' y máximo '{2}' caracteres.";
        public const string WMSAPI_msg_Error_RequeridoValidation = "El campo '{0}' no puede ser vacío.";
        public const string WMSAPI_msg_Error_DataTypeValidation = "No se pudo convertir de string a '{0}:{1}'. Ruta: '{2}'.";
        public const string WMSAPI_msg_Error_FechaMayorIgualHoyValidation = "La fecha no puede ser menor a la fecha actual. Fecha enviada: '{0}'";
        public const string WMSAPI_msg_Error_RangoValidation = "El campo '{0}' debe ser un número entre '{1}' y '{2}'.";

        public const string WMSAPI_msg_Error_EjecucionNoExiste = "No se encontró la ejecución nro: '{0}' ";

        public const string WMSAPI_msg_Error_Requerido = "'{0}': Valor requerido.";
        public const string WMSAPI_msg_Error_FormatoIncorrecto = "'{0}': Formato incorrecto.";
        public const string WMSAPI_msg_Error_ValorNegativo = "'{0}': El valor no puede ser negativo.";
        public const string WMSAPI_msg_Error_NoPuedeSerCeo = "'{0}': El valor no puede ser igual a 0.";
        public const string WMSAPI_msg_Error_LargoMaximo = "'{0}': El valor no puede superar los '{1}' dígitos.";
        public const string WMSAPI_msg_Error_EmpresaNoExiste = "La empresa {0} no existe o no está asignada al usuario.";
        public const string WMSAPI_msg_Error_ProductoNoExiste = "El producto '{0}' no existe para la empresa '{1}'.";
        public const string WMSAPI_msg_Error_ProductoInactivo = "El producto '{0}' está inactivo para la empresa '{1}'.";
        public const string WMSAPI_msg_Error_SituacionNoValida = "Situación: La situación enviada no es válida.";
        public const string WMSAPI_msg_Error_PredioNoExiste = "El predio '{0}' no existe o no está asignado al usuario.";
        public const string WMSAPI_msg_Error_RutaNoExiste = "Ruta: La ruta enviada no existe.";

        public const string WMSAPI_msg_Error_ValorVacio = "'{0}': Valor vacío.";
        public const string WMSAPI_msg_Error_ErrorConversion = "'{0}': Error en conversión";
        public const string WMSAPI_msg_Error_ValorIngresadoNoExiste = "'{0}': El valor ingresado '{1}' no existe.";
        public const string WMSAPI_msg_Error_SubdivisionPais = "'Subdivisión': El valor ingresado '{0}' no pertenece al pais especificado '{1}'.";
        public const string WMSAPI_msg_Error_SubdivisionNula = "'Municipio': El campo Subdivisión no puede ser nulo si Localidad no lo es.";
        public const string WMSAPI_msg_Error_LocalidadSubdivision = "'Municipio': El valor ingresado '{0}' no existe para la subdivision '{1}'.";
        public const string WMSAPI_msg_Error_ErrorNoControlado = "'{0}': Error no controlado - '{1}'";

        public const string WMSAPI_msg_Error_ValorMayorACero = "El valor ingresado debe ser mayor a 0.";
        public const string WMSAPI_msg_Error_FormatodeFechaIncorrecto = "'{0}': La fecha ingresada no tiene el formato correcto (yyyyMMdd).";
        public const string WMSAPI_msg_Error_FechaNoPuederMenorAX = "'{0}': La fecha ingresada '{1}' no puede ser menor a la fecha '{2}'.";
        public const string WMSAPI_msg_Error_FechaNoPuederMenorAHoy = "'{0}': La fecha ingresada '{1}' no puede ser menor a hoy.";
        public const string WMSAPI_msg_Error_ProductoManejaVencimiento = "El producto '{0}' no es de tipo duradero y la fecha de vencimiento es requerida.";
        public const string WMSAPI_msg_Error_FechaNoPuederMayorAX = "'{0}': La fecha ingresada '{1}' no puede ser mayor a la fecha '{2}'.";
        public const string WMSAPI_msg_Error_FechaNoPuederMayorAHoy = "'{0}': La fecha ingresada '{1}' no puede ser mayor a hoy.";
        public const string WMSAPI_msg_Error_DecimalDebeEstarComprendidoEntre = "'{0}': El valor ingresado debe estar comprendido entre {1} y {2}.";

        public const string WMSAPI_msg_Error_ListaDeObjetosRequerida = "La lista de objetos no puede ser vacía.";
        public const string WMSAPI_msg_Error_DetallesRequeridos = "La lista de detalles no puede ser vacía.";
        public const string WMSAPI_msg_Error_SinEjecucionesPendientes = "No hay ejecuciones pendientes para la empresa '{0}'.";
        public const string WMSAPI_msg_Error_InterfazNoExiste = "La interfaz '{0}' no existe.";
        public const string WMSAPI_msg_Error_InterfazExternaNoCoincide = "El código de interfaz externa '{0}' no corresponde con el de la interfaz de ejecución '{1}'.";
        public const string WMSAPI_msg_Error_InterfazExternaNoExiste = "El código de interfaz externa '{0}' no existe.";
        public const string WMSAPI_msg_Error_InterfazNoEncontrada = "No se encontró la interfaz de salida '{0}' para la empresa '{1}'.";
        public const string WMSAPI_msg_Error_InterfazNoEncontradaEmpresaGrupo = "No se encontró la interfaz de salida '{0}' para la empresa '{1}' perteneciente a unos de los siguientes grupos de consulta: {2}.";
        public const string WMSAPI_msg_Error_InterfazDataNoEncontrada = "No se encontró data de la interfaz '{0}'.";
        public const string WMSAPI_msg_Error_EstadoInterfazIncorrecto = "La interfaz debe tener estado '{0}' para realizar esta operación";
        public const string WMSAPI_msg_Error_APIIncorrecta = "La ejecución enviada no corresponde con una interfaz de '{0}'";
        public const string WMSAPI_msg_Error_InterfazPendientePrevia = "Es necesario confirmar la interfaz '{0}' antes de poder confirmar la interfaz '{1}'";
        public const string WMSAPI_msg_Error_InterfazProcesablePorWebhook = "No es posible procesar la interfaz '{0}' por esta vía. La empresa '{1}' tiene un webhook configurado";
        public const string WMSAPI_msg_Error_EmpresaBloqueada = "El procesamiento de interfaces de salida se encuentra bloqueado para la empresa '{0}'";
        public const string WMSAPI_msg_Error_CaracteresMinuscula = " No puede incluir letras minúsculas.";
        public const string WMSAPI_msg_Error_UbicacionNoExiste = "La ubicación '{0}' no existe.";
        public const string WMSAPI_msg_Error_UbicacionPredioIncorrecto = "La ubicación {0} no pertenece al predio {1}.";
        public const string WMSAPI_msg_Error_MaxItemsSuperado = "La cantidad de items enviados no puede superar el máximo de {0}.";
        public const string WMSAPI_msg_Error_MaxItemsDetallesSuperado = "La cantidad total de detalles enviados no puede superar el máximo de {0}.";
        public const string WMSAPI_msg_Error_ProductoManejaLote = "El producto '{0}' maneja lote.";
        public const string WMSAPI_msg_Error_ProductoManejaLoteNoAUTO = "El producto '{0}' maneja lote pero no es posible enviar lote (AUTO).";
        public const string WMSAPI_msg_Error_IdEjecucionExistente = "El identificador de request '{0}' con la empresa '{1}' ya existe.";

        public const string WMSAPI_msg_Error_NoAdmiteCaracter = "'{0}': No admite el caracter '{1}' ";
        public const string WMSAPI_msg_Error_NoAdmiteLosCaracteres = "'{0}': No admite los siguientes caracteres '{1}' ";

        public const string WMSAPI_msg_Error_ValueSoN = "'{0}': Los valores permitidos son S o N.";
        public const string WMSAPI_msg_Error_ValueCoD = "'{0}': Los valores permitidos son C o D.";

        public const string WMSAPI_msg_Error_UsuarioNoValido = "El usuario '{0}' no es válido.";
        public const string WMSAPI_msg_Error_UsuarioPuestoNoValido = "El usuario '{0}', Puesto '{1}' no son válidos.";
        public const string WMSAPI_msg_Error_OperacionConLpnNoPermitida = "No se puede utilizar un contenedor asociado a un LPN";
        public const string WMSAPI_msg_Error_TipoContenedorNoPermitido = "No se pueden utilizar tipos de contenedores que correspondas a tipos de LPNs";
        public const string WMSAPI_msg_Error_CaracteresNoPermitidos = "El identificador contiene caracteres no permitidos.";

        #endregion



        #region Automatismo

        //Automatismo Validation Service
        public const string WMSAPI_msg_Error_AutomatismoZonaNoExiste = "El automatismo con el codigo de zona {0} no existe.";
        public const string WMSAPI_msg_Error_AutomatismoCodigoNoExiste = "El automatismo con el codigo {0} no existe.";
        public const string WMSAPI_msg_Error_AutomatismoTipoNoDefinido = "El tipo del automatismo {0} no fue definido.";
        public const string WMSAPI_msg_Error_AutomatismoInterfazNoConfigurada = "La interfaz {0} no fue configurada para el automatismo {1}.";
        public const string WMSAPI_msg_Error_AutomatismoInterfazProtocoloNoConfigurado = "La interfaz {0} no fue configurada para el automatismo {1}.";
        public const string WMSAPI_msg_Error_AutomatismoSinServicioIntegracion = "El servicio requiere la definición de un servicio de integración";
        public const string WMSAPI_msg_Error_AutomatismoNoTienePosicion = "El automatismo {0} no tiene configurada ninguna posición";
        public const string WMSAPI_msg_Error_AutomatismoNoTieneUbicacionAjuste = "El automatismo {0} no tiene definida una ubicación de ajustes.";
        public const string WMSAPI_msg_Error_AutomatismoCodigoPuestoNoExiste = "El automatismo con el codigo de puesto {0} no existe.";
        public const string WMSAPI_msg_Error_AutomatismoCampoRequerido = "{0}: es requerido.";
        public const string WMSAPI_msg_Error_AutomatismoLargoMaximo = "{0}: supera los 40 caracteres.";
        public const string WMSAPI_msg_Error_AutomatismoProductoNoExiste = "El producto {0} no existe";
        public const string WMSAPI_msg_Error_AutomatismoCampoNoNumerico = "{0}: no es numerico.";
        public const string WMSAPI_msg_Error_AutomatismoCampoNoNegativo = "{0}: no puede ser negativo.";
        public const string WMSAPI_msg_Error_AutomatismoUsuarioNoExiste = "El usuario {0} no existe";
        public const string WMSAPI_msg_Error_AutomatismoDetalleFaltanteCabezal = "El debe tener al menos un detalle si en el cabezal el tag UltimaOperacion no es true";
        public const string WMSAPI_msg_Error_AutomatismoEntradaInterfazIncorrecta = "La entrada {0} no corresponde a una interfaz de entrada del automatismo";
        public const string WMSAPI_msg_Error_AutomatismoEntradaFinalizada = "La entrada {0} ya fue finalizada";
        public const string WMSAPI_msg_Error_AutomatismoEntradaNoNotificada = "La linea entrada: {0} con el producto: {1} lote: {2} no fue notificado en la orden de entrada original.";
        public const string WMSAPI_msg_Error_AutomatismoEntradaLineaFinalizada = "La linea entrada: {0} del producto {1} lote {2} ya fue finalizado.";
        public const string WMSAPI_msg_Error_AutomatismoEntradaNotificadaCantidad = "La linea entrada: {0} con el producto: {1} lote: {2} no fue notificado en la orden de entrada original.";
        public const string WMSAPI_msg_Error_AutomatismoIncumplimientoFormatoFecha = "{0}: no cumple con formato de fecha.";
        public const string WMSAPI_msg_Error_AutomatismoFechaMenorActual = "{0}: no puede ser menor a hoy.";
        public const string WMSAPI_msg_Error_AutomatismoFechaMenorARango = "{0}: La fecha ingresada {1} no puede ser menor a la fecha {2}.";
        public const string WMSAPI_msg_Error_AutomatismoErrorNoControlado = "{0}: Error no controlado - {1}.";
        public const string WMSAPI_msg_Error_AutomatismoCampoSoN = "{0} solo permite valores S o N";

        //Automatismo Service
        public const string WMSAPI_msg_Error_AutomatismoCantidadItems = "La cantidad de items enviados no puede superar el máximo de {0}.";
        public const string WMSAPI_msg_Error_AutomatismoProductosDuplicados = "Productos duplicados. Empresa: {0} - Producto: {1}.";
        public const string WMSAPI_msg_Error_AutomatismoCodigosBarrasDuplicados = "Codigos de barras duplicados. Empresa: {0} - CodigoBarras: {1}.";
        public const string WMSAPI_msg_Error_AutomatismoCantidadLineas = "La cantidad total de líneas enviadas no puede superar el máximo de {0}.";
        public const string WMSAPI_msg_Error_AutomatismoDetallesDuplicados = "Detalles duplicados. Empresa: {0} - Preparacion: {1} - Pedido: {2} - TipoAgente: {3} - CodigoAgente: {4} - Producto: {5} - Identificador: {6} - Carga: {7} - ComparteContenedor: {8}.";
        public const string WMSAPI_msg_Error_AutomatismEjecucionNoExiste = "La ejecución {0} no existe en el sistema.";
        public const string WMSAPI_msg_Error_AutomatismCodigoInterfazNoCoincide = "El código de interfaz no coincide con una de Entrada.";
        public const string WMSAPI_msg_Error_AutomatismUbicacionesPickingDuplicadas = "Ubicaciones de picking duplicadas. Empresa: {0} - Producto: {1}  - Ubicacion: {2}.";
        public const string WMSAPI_msg_Error_TipoMovimientoNoImplementado = "Tipo de movimiento {0} no implementado";

        #endregion

        #region Agenda
        public const string WMSAPI_msg_Error_AgendaNoEncontrada = "No se encontró la agenda '{0}'.";
        public const string WMSAPI_msg_Error_TipoRecepcionNoExiste = "El tipo de recepción '{0}' no existe o no está asignado a la empresa '{1}'.";
        public const string WMSAPI_msg_Error_TpRecNoHabilitadoEmpresa = "El tipo de recepción '{0}' no está habilitado para la empresa '{1}'.";
        public const string WMSAPI_msg_Error_TpRecSinTpAgente = "El tipo de recepción '{0}' no es válido para el tipo de agente '{1}'.";
        public const string WMSAPI_msg_Error_EsadoReferenciaNoValido = "El estado de la referencia '{0}' no permite esta operación.";
        public const string WMSAPI_msg_Error_ReferenciaSinSaldoDisponible = "La referencia '{0}' no tiene saldo disponible.";
        public const string WMSAPI_msg_Error_PredioReferenciaDistinto = "El predio de la referencia no corresponde con el enviado para la agenda.";
        public const string WMSAPI_msg_Error_OperacionNoDisponibleDocumental = "Operación no disponible para empresas documentales.";
        public const string WMSAPI_msg_Error_TipoRecepcionNoPermitido = "El tipo de recepción no está permitido para el api";

        #endregion

        #region Agente
        public const string WMSAPI_msg_Error_AgenteNoEncontrado = "No se encontró el agente '{0}' para la empresa '{1}' de tipo '{2}'.";
        public const string WMSAPI_msg_Error_ClienteNoExiste = "No existe el cliente '{0}' para la empresa '{1}'.";
        public const string WMSAPI_msg_Error_ExisteAgenteValidation = "El agente con los valores: Código: '{0}', Tipo: '{1}', Código empresa: '{2}' no existe en el sistema.";
        public const string WMSAPI_msg_Error_TpAgenteNoValido = "El tipo de agente '{0}' no es válido.";
        public const string WMSAPI_msg_Error_GLNNoValido = "Numero Localizacion Global: Código no válido.";
        public const string WMSAPI_msg_Error_AgenteCaracteresMinuscula = "Agente: No puede incluir letras minúsculas.";
        public const string WMSAPI_msg_Error_AgentesDuplicados = "Agentes duplicados. Código: '{0}' - Empresa: '{1}' - Tipo: '{2}'.";
        public const string WMSAPI_msg_Error_CodigoAgenteCliLargoMax = "El código de agentes de tipo {2} no puede superar los 10 caracteres. Código: {0} - Empresa: '{1}' - Tipo: '{2}'.";

        #endregion

        #region CrossDocking
        public const string WMSAPI_msg_Error_CrossDockingDuplicadoAgenda = "CrossDocking duplicados. Agenda: {0} - Ubicacion {1}.";
        public const string WMSAPI_msg_Error_CrossDockingDuplicadoContenedor = "CrossDocking duplicados. Id Externo Contenedor: {0} - TipoContenedor: {1} - Preparacion: {2} - Cliente: {3} - TipoAgente: {4} - Código: {5} - Empresa: {6} - Lote: {7} - Agenda: {8}.";

        #endregion

        #region ControlDeCalidad

        public const string WMSAPI_msg_Error_EstadoNoIdentificado = "Estado no identificado.";
        public const string WMSAPI_msg_Error_ProductoLoteNoTieneControlesParaAsignar = "El producto '{0}' lote '{1}' no tiene stock, etiquetas y lpn para asignar.";
        public const string WMSAPI_msg_Error_CriterioPorAprobarNoExistente = "Criterio '{0}' por aprobar no existente.";
        public const string WMSAPI_msg_Error_InstanciaExistenteOperacionAsociar = "La instancia '{0}' ya existe, no se puede asociar nuevamente.";
        public const string WMSAPI_msg_Error_InstanciaNoExiste = "Instancia {0} no existente, no se puede aprobar.";
        public const string WMSAPI_msg_Error_ProductoNoExisteEnUbicacion = "La ubicación {0} no existe o no contiene el producto '{1}' / lote '{2}'.";
        public const string WMSAPI_msg_Error_CodigoControlCalidadNoExiste = "El código de control de calidad '{0}' no existe o no está asignado para el producto.";
        public const string WMSAPI_msg_Error_ProductoLoteNoTieneControlPendiente = "El producto '{0}' lote '{1}' no tiene un control de calidad {2} por aprobar.";

        public const string WMSAPI_msg_Error_EtiquetaAlmacenada = "La etiqueta '{0}' ya fue almacenada.";
        public const string WMSAPI_msg_Error_EtiquetaNoTieneControlPendiente = "La etiqueta '{0}' no tiene un control de calidad {1} por aprobar para el producto '{2}' / lote '{3}'.";
        public const string WMSAPI_msg_Error_EtiquetaSinStock = "La etiqueta '{0}' no tiene stock para el producto '{1}' / lote '{2}'.";
        public const string WMSAPI_msg_Error_EtiquetaCtrlCalidadPendienteYaAsignado = "La etiqueta '{0}', empresa {1}, producto '{2}' y lote '{3}' ya tienen asignado el control de calidad {4}.";
        public const string WMSAPI_msg_Error_EtiquetaNoExiste = "El número de etiqueta externa '{0}' no existe o la misma no contiene al producto '{1}' / lote '{2}'.";
        public const string WMSAPI_msg_Error_EtiquetaNoExisteEnUbicacion = "El número de etiqueta externa '{0}' no existe en la ubicación {1}.";
        public const string WMSAPI_msg_Error_EtiquetaRepetidaParaNumeroExterno = "El número de etiqueta externa '{0}' se encuentra activa para más de un tipo. Especifique el tipo de etiqueta.";
        public const string WMSAPI_msg_Error_EtiquetaNoRequiereUbicacion = "'{0}' es una etiqueta de recepción, no se debe enviar el campo ubicación.";

        public const string WMSAPI_msg_Error_LpnConAreaUbicacionIncorrecta = "El LPN {0} debe estar almacenado en un área de stock general.";
        public const string WMSAPI_msg_Error_LpnConStockReservado = "El LPN '{0}' con el producto '{1}' / lote '{2}' tiene stock reservado.";
        public const string WMSAPI_msg_Error_LpnNoTieneControlCalidadPendiente = "El LPN '{0}' no tiene el control de calidad {1} pendiente para el producto '{2}' / lote '{3}'.";
        public const string WMSAPI_msg_Error_LpnStockAveriado = "El LPN '{0}' con el producto '{1}' / lote '{2}' tiene stock averiado.";
        public const string WMSAPI_msg_Error_LpnStockNoDisponible = "El LPN '{0}' no tiene el total de stock disponible para el producto '{1}' / lote '{2}'.";
        public const string WMSAPI_msg_Error_LpnStockRealizado = "El detalle del LPN '{0}' con el producto '{1}' / lote '{2}' se encuentra marcado con diferencia de inventario.";
        public const string WMSAPI_msg_Error_LpnYaTieneControlPendiente = "El LPN '{0}' ya tiene un control de calidad pendiente para el producto '{1}' / lote '{2}'.";
        public const string WMSAPI_msg_Error_LpnCtrlCalidadPendienteYaAsignado = "El LPN '{0}', empresa {1}, producto '{2}' y lote '{3}' ya tienen asignado el control de calidad {4}.";
        public const string WMSAPI_msg_Error_LpnSinStock = "El LPN '{0}' no tiene stock para el producto '{1}' / lote '{2}'.";

        public const string WMSAPI_msg_Error_UbicacionAreaIncorrecta = "El área de la ubicación '{0}' es inválida.";
        public const string WMSAPI_msg_Error_UbicacionNoContieneProducto = "El producto '{0}' / lote '{1}' no se encuentra en la ubicación '{2}'.";
        public const string WMSAPI_msg_Error_UbicacionProductoStockNoDisponible = "El producto '{0}' / lote '{1}' no tiene todo el stock disponible en la ubicación '{2}'.";
        public const string WMSAPI_msg_Error_UbicacionCtrlCalidadPendienteYaAsignado = "La ubicación '{0}', empresa {1}, producto '{2}' y lote '{3}' ya tienen asignado el control de calidad {4}.";
        public const string WMSAPI_msg_Error_UbicacionSinCtrlCalidadPendiente = "La ubicación '{0}' no tiene un control de calidad {1} asignado para el producto '{2}' / lote '{3}'.";

        #endregion

        #region Barras
        public const string WMSAPI_msg_Error_CodigoDeBarrasNoEncontrado = "No se encontró el código '{0}' para la empresa '{1}'.";
        public const string WMSAPI_msg_Error_tpCodBarrasNoExiste = "El tipo de código de barras '{0}' no existe.";
        public const string WMSAPI_msg_Error_BarrasAsignadoOtroProducto = "El código de barras ya se encuentra asignado al producto '{0}' y la modificación no está habilitada.";
        public const string WMSAPI_msg_Error_TpOperacionASoB = "'TipoOperacion': El tipo de operación debe ser A, S o B.";
        public const string WMSAPI_msg_Error_TpOperacionAoB = "'TipoOperacion': El tipo de operación debe ser A o B.";
        public const string WMSAPI_msg_Error_DeleteCodigoNoExiste = "No se puede realizar esta operacion ya que el código de barras no existe.";
        public const string WMSAPI_msg_Error_CodigosDeBarrasDuplicados = "Códigos de barra duplicados. Código: '{0}' - Empresa: '{1}'.";

        #endregion

        #region Egreso
        public const string WMSAPI_msg_Error_EgresoNoEncontrado = "No se encontró el camión {0}.";
        public const string WMSAPI_msg_Error_EgresoExternoNoEncontrado = "No se encontró camión asociado al id. Externo: {0} y la empresa externa: {1}.";

        public const string WMSAPI_msg_Error_IdExternoExistente = "Ya existe un egreso para el identificador externo {0} y la empresa externa {1}.";
        public const string WMSAPI_msg_Error_RutaNoPerteneceAlPredioEgreso = "La ruta enviada no pertenece al predio del egreso.";
        public const string WMSAPI_msg_Error_VehiculoNoExiste = "El vehículo {0} no existe.";
        public const string WMSAPI_msg_Error_VehiculoNoPerteneceAlPredioEgreso = "El vehiculo enviado no pertenece al predio del egreso.";
        public const string WMSAPI_msg_Error_PuertaNoExiste = "La puerta no existe o no pertenece al predio {0}.";
        public const string WMSAPI_msg_Error_LaPuertaNoPerteneceAlPeedio = "La puerta {0} no pertenece al predio {1}.";
        public const string WMSAPI_msg_Error_EmpresaDistintaAlEgreso = "La empresa del detalle debe coincidir con la del egreso ya que es mono-empresa.";
        public const string WMSAPI_msg_Error_CargaNoExiste = "La carga {0} no existe o no está habilitada para asignarla al egreso.";
        public const string WMSAPI_msg_Error_CargaNoPerteneceRuta = "La carga {0} no pertenece a la ruta del egreso.";
        public const string WMSAPI_msg_Error_PedidoNoPerteneceRuta = "El pedido no pertenece a la ruta del egreso.";
        public const string WMSAPI_msg_Error_PedidoNoHabilitado = "El pedido '{0}' con agente '{1}-{2}' y empresa '{3}' no existe o no está habilitado para asignarlo a un egreso.";
        public const string WMSAPI_msg_Error_ContenedorNoHabilitado = "El contenedor IdExterno: {0} - Tipo: {1} - Empresa: {2} no existe o no está habilitado para asignarlo al egreso.";
        public const string WMSAPI_msg_Error_CargasEnOtroCamion = "La carga {0} cliente {1} empresa {2} está asociada a otro egreso. Para realizar esta acción debe habilitar el uso de carga asignada.";
        public const string WMSAPI_msg_Error_PredioYPredioExterno = "No se puede enviar el Predio y el PredioExterno. Debe enviar uno de los dos.";
        public const string WMSAPI_msg_Error_PredioExternoNoExiste = "El predio externo '{0}' no existe o no está asignado al usuario.";
        public const string WMSAPI_msg_Error_EgresoIdentificadoresDuplicados = "Se enviaron identificadores duplicados. IdExterno: {0}.";
        public const string WMSAPI_msg_Error_EgresoPeiddoDatosDuplicados = "El pedido {0}, agente {1} - {2} y empresa {3} se envió duplicado.";
        public const string WMSAPI_msg_Error_EgresoCargaDuplicados = "La carga {0}, agente {1} - {2} y empresa {3} se envió duplicada.";
        public const string WMSAPI_msg_Error_EgresoContenedorDuplicados = "El contenedor IdExterno: {0} - Tipo: {1} - Empresa: {2} se envió duplicado.";
        #endregion

        #region Empresa
        public const string WMSAPI_msg_Error_EmpresaNoAsignada = "El usuario '{0}' no tiene asignada la empresa '{1}'.";
        public const string WMSAPI_msg_Error_EmpresaInterfazInhabilitada = "La interfaz '{0}' no está habilitada para la empresa '{1}'.";
        public const string WMSAPI_msg_Error_ExisteEmpresaValidation = "La empresa con el código '{0}' no existe en el sistema.";
        public const string WMSAPI_msg_Error_EmpresasDuplicadas = "Empresas duplicadas. Código: '{0}'";
        public const string WMSAPI_msg_Error_UsuarioInvalido = "El usuario '{0}' no es válido.";
        #endregion

        #region Lpn
        public const string WMSAPI_msg_Error_LpnNoEncontrado = "No se encontró el LPN '{0}'.";
        public const string WMSAPI_msg_Error_LpnExternoTipoNoEncontrado = "No se encontró el LPN IdExterno: '{0}' - Tipo: {1}.";
        public const string WMSAPI_msg_Error_LpnExistente = "Ya existe un LPN activo con IdExterno {0} y tipo {1}.";
        public const string WMSAPI_msg_Error_TipoLpnNoExiste = "El tipo de LPN {0} no existe en el sistema.";
        public const string WMSAPI_msg_Error_AtributosFaltantes = "Existen atributos faltantes que son requeridos para el tipo de LPN {0}.";
        public const string WMSAPI_msg_Error_AtributosDetalleFaltantes = "Existen atributos faltantes que son requeridos para los detalles del tipo de LPN {0}.";

        public const string WMSAPI_msg_Error_AtributoNoExisteNoAsociado = "El atributo {0} no existe o no está asociado al tipo de LPN {1}.";
        public const string WMSAPI_msg_Error_NombreAtributoRequerido = "El nombre del atributo es requerido.";
        public const string WMSAPI_msg_Error_TipoAtributoNoHabilitadoParaApi = "No se puede enviar un atributo de tipo 6-Sistema WIS.";
        public const string WMSAPI_msg_Error_ValorDominioIncorrecto = "El valor del dominio no existe o no pertenece al dominio asociado al atributo.";
        public const string WMSAPI_msg_Error_TipoBarraNoExiste = "El tipo de código de barra {0} no existe en el sistema.";
        public const string WMSAPI_msg_Error_LpnBarraExistente = "Ya existe un código de barras {0} activo para la empresa {1}.";
        public const string WMSAPI_msg_Error_LpnBarraDuplicado = "El código de barras {0} se envió duplicado para el LPN.";
        public const string WMSAPI_msg_Error_LpnBarraSinPrefijoWIS = "El código de barras no puede empezar con el prefijo WIS";
        public const string WMSAPI_msg_Error_LpnAtributoDuplicado = "El atributo {0} se envió duplicado para el LPN.";
        public const string WMSAPI_msg_Error_LpnAtributoDetalleDuplicado = "El atributo {0} se envió duplicado para el detalle del LPN.";
        public const string WMSAPI_msg_Error_LpnDetalleProductoLoteDuplicado = "El detalle {0}, producto {1} y Lote {2} se envió duplicado.";
        public const string WMSAPI_msg_Error_LpnProductoLoteDuplicado = "Cuando el tipo de LPN no tiene Atributos definidos para su detalle no es posible enviar más de una linea para el mismo Producto/Lote. Producto: {0} - Lote: {1}";

        public const string WMSAPI_msg_Error_LpnIdentificadoresDuplicados = "Se enviaron identificadores duplicados. IdExterno: {0}.";
        public const string WMSAPI_msg_Error_TipoLpnNOMultiProducto = "El tipo de LPN no es Multi-Producto";
        public const string WMSAPI_msg_Error_TipoLpnNOMultiLote = "El tipo de LPN no es Multi-Lote";
        public const string WMSAPI_msg_Error_LpnNoActivo = "El LPN con el número externo '{0}', no se encuentra Activo.";

        #endregion

        #region Pedidos
        public const string WMSAPI_msg_Error_PedidoNoEncontrado = "No se encontró el pedido '{0}' con el agente '{1}-{2}' para la empresa '{3}'.";
        public const string WMSAPI_msg_Error_PedidoYaExiste = "El pedido '{0}' con agente '{1}-{2}' y empresa '{3}' ya existe.";
        public const string WMSAPI_msg_Error_PedidoNoExiste = "El pedido '{0}' con agente '{1}-{2}' y empresa '{3}' no existe.";
        public const string WMSAPI_msg_Error_PedidoDeProduccion = "El pedido {0} con agente {1}-{2} y empresa {3} es una producción no se puede editar.";
        public const string WMSAPI_msg_Error_DetPedidoNoExiste = "El detalle de pedido no existe. Producto: '{0}' - Idenfiticador: '{1}' - Especifica identificador: '{2}'";
        public const string WMSAPI_msg_Error_CantDuplicadosIncorrecta = "El número de duplicados enviados no coincide. Debe enviar todos los duplicados correspondientes al producto: '{0}' - Idenfiticador: '{1}' - Especifica identificador: '{2}'";
        public const string WMSAPI_msg_Error_TipoExpedicionNoExiste = "El tipo de expedición '{0}' no existe.";
        public const string WMSAPI_msg_Error_TipoPedidoNoExiste = "El tipo de pedido '{0}' no existe.";
        public const string WMSAPI_msg_Error_TpExpIncompatibleTpPedido = "El tipo de expedición '{0}' no es compatible con el tipo de pedido '{1}'.";
        public const string WMSAPI_msg_Error_PedidoCaracteresMinuscula = "NroPedido: No puede incluir letras minúsculas.";
        public const string WMSAPI_msg_Error_CondicionLiberacionNoExiste = "La condición de liberación '{0}' no existe.";
        public const string WMSAPI_msg_Error_TransportadoraNoExiste = "El transportista '{0}' no existe.";
        public const string WMSAPI_msg_Error_PedidosDuplicados = "Pedidos duplicados. Número: {0} - Empresa: {1} - Agente: {2}-{3}.";
        public const string WMSAPI_msg_Error_ZonaNoExiste = "La zona '{0}' no existe.";

        public const string WMSAPI_msg_Error_SinSaldoSuficiente = "No existe saldo suficiente para modificar la cantidad del producto '{0}', identificador '{1}', pedido '{2}', empresa '{3}', cliente '{4}'.";
        public const string WMSAPI_msg_Error_DuplicadoNoExiste = "El duplicado no existe. Producto: '{0}' - Idenfiticador: '{1}' - Especifica identificador: '{2}' - Id Linea Sistema Externo: '{3}'";

        public const string WMSAPI_msg_Error_SinSaldoSuficienteDup = "No existe saldo suficiente para modificar la cantidad del duplicado. Pedido '{0}' - Empresa '{1}' - Cliente '{2}' - Producto: '{3}' - Identificador: '{4}'- Especifica identificador: '{5}' - IdLineaSistemaExterno: '{6}'";
        public const string WMSAPI_msg_Error_PedidoSinSaldo = "No se puede modificar un pedido sin saldo.";
        public const string WMSAPI_msg_Error_NoSePuedeModificarTpPedTpExp = "No se puede modificar el tipo de pedido o el tipo de expedición si el pedido tiene cantidades liberadas.";
        public const string WMSAPI_msg_Error_DuplicadosFaltantes = "No se enviaron todos los duplicados asociados al detalle. Producto: '{0}' - Idenfiticador: '{1}' - Especifica identificador: '{2}'";
        public const string WMSAPI_msg_Error_PreparacionPedido = "No se encontró la preparación '{0}' pedido '{1}' con el agente '{2}-{3}' para la empresa '{4}'.";
        public const string WMSAPI_msg_Error_PreparacionPedidoSinDetalles = "No existe detalles para anular, en la preparación {0} pedido '{1}' con el agente '{2}-{3}' para la empresa '{4}'.";
        public const string WMSAPI_msg_Error_AnulacionPreparacionPendiente = "La preparación {0} está en proceso de anulación";
        public const string WMSAPI_msg_Error_PedidoDetalleDuplicados = "El pedido {0} contiene detalles duplicados. Producto: {1} - Identificador: {2}.";
        public const string WMSAPI_msg_Error_PedidoSinDetalle = "El pedido {0} - Agente {1} - Tipo {2} - Empresa {3} no tiene detalles.";
        public const string WMSAPI_msg_Error_PedidoUsoDuplicados = "El uso de duplicados no está habilitado para la empresa {0}.";
        public const string WMSAPI_msg_Error_PedidoUsoLpn = "El uso de LPN no está habilitado para la empresa {0}.";
        public const string WMSAPI_msg_Error_PedidoUsoAtributos = "El uso de atributos no está habilitado para la empresa {0}.";
        public const string WMSAPI_msg_Error_PedidoLineasDuplicadas = "El pedido {0} contiene líneas duplicadas. Producto: {1} - Identificador: {2} - Id Externo: {3}.";
        public const string WMSAPI_msg_Error_PedidoCantidadNoCoincide = "La cantidad total del pedido {0} no coincide con la suma de las cantidades de sus duplicados. Producto: {1} - Identificador: {2}.";
        public const string WMSAPI_msg_Error_PedidoCantidadMenorALaSumaDeDetalleLpn = "La cantidad total del pedido {0} no puede ser menor a la suma de los detalles de los LPN. Producto: {1} - Identificador: {2}.";
        public const string WMSAPI_msg_Error_PedidoCantidadMenorALaSumaDeDetalleAtributos = "La cantidad total del pedido {0} no puede ser menor a la suma de los detalles de los atributos. Producto: {1} - Identificador: {2}.";
        public const string WMSAPI_msg_Error_PedidoCantidadMenorALaSumaDeDetalleLpnAtributos = "La cantidad total del Producto {1} - Identificador {2} en el pedido {0} no puede ser menor a la suma de los detalles de los atributos del LPN con Id Externo {3} y Tipo {4}.";
        public const string WMSAPI_msg_Error_EstadoPickingValidos = "La situaciones validas son {0} - {1}.";
        public const string WMSAPI_msg_Error_LpnNoEstaRegistrado = "El LPN con id externo {0} de tipo {1} no está registrado en el sistema o no está activo.";
        public const string WMSAPI_msg_Error_TipoLpnNoPermiteCantidadMenorCantidadTotal = "El tipo de LPN {0} no permite extrar cantidad menor a la total. Id Externo: {1} - Producto: {2} Indentificador {3}";
        public const string WMSAPI_msg_Error_TipoLpnNoPermiteCantidadMayorCantidadTotal = "La cantidad no puede ser mayor a la total. Id Externo: {0} - Tipo {1} Producto: {2} Indentificador {3}";
        public const string WMSAPI_msg_Error_TipoLpnNoPermiteExtraerLineas = "El tipo de LPN {0} no permite extrar lineas";
        public const string WMSAPI_msg_Error_PedidoLpnLineasDuplicadas = "El pedido {0} contiene líneas de LPN duplicadas. Producto: {1} - Identificador: {2} - Id Externo: {3} - Tipo {4}.";
        public const string WMSAPI_msg_Error_PedidoConfiguracionesAtributosDuplicadas = "El pedido {0} contiene configuraciones de atributos duplicadas. Producto: {1} - Identificador: {2} - Configuración: {3}.";
        public const string WMSAPI_msg_Error_PedidoConfiguracionesAtributosLpnDuplicadas = "El pedido {0} contiene configuraciones de atributos duplicadas. Producto: {1} - Identificador: {2} - Id LPN Externo: {3} - Tipo LPN: {4} -  Configuración: {5}.";
        public const string WMSAPI_msg_Error_PedidoAtributosDuplicados = "El pedido {0} contiene atributos duplicados. Producto: {1} - Identificador: {2} - Atributo: {3} - Cabezal: {4}.";
        public const string WMSAPI_msg_Error_PedidoAtributosLpnDuplicados = "El pedido {0} contiene atributos duplicados. Producto: {1} - Identificador: {2} - Id LPN Externo: {3} - Tipo LPN: {4} - Atributo: {5} - Cabezal: {6}.";
        public const string WMSAPI_msg_Error_PedidoDetalleLpnTieneDistintoLote = "El detalle del pedido {0} - Producto {1} - Identificador es diferente al LPN con Id Externo {3} y Tipo {4}";
        public const string WMSAPI_msg_Error_PedidoConfiguracionAtributosVacia = "El pedido {0} contiene configuraciones de atributos vacías. Producto: {1} - Identificador: {2}.";
        public const string WMSAPI_msg_Error_PedidoConfiguracionAtributosLpnVacia = "El pedido {0} contiene configuraciones de atributos vacías para el LPN con Id Externo {3} y Tipo {4}.  Producto: {1} - Identificador: {2}.";
        public const string WMSAPI_msg_Error_AtributoNoExiste = "El atributo {0} no está registrado en el sistema.";
        public const string WMSAPI_msg_Error_AtributoLpnNoExiste = "El atributo {0} no está definido para el detalle del LPN con Id Externo {1} - Tipo {2} - Producto {3} - Identificador - {4}.";
        public const string WMSAPI_msg_Error_DetallePedidoLpnTrabajado = "No es posible realizar la operación. Existen detalles de LPNs de pedidos trabajados";
        public const string WMSAPI_msg_Error_CantidadMayorSaldo = "No puede ingresar una cantidad menor al saldo disponible del detalle LPN";
        public const string WMSAPI_msg_Error_LpnYaAsociadoPorCabezal = "El LPN Id Externo {0} y Tipo {1} fue asociado con anterioridad y no es posible mandarlo por el cabezal. Si desea modificarlo deben mandarlo por los detalles.";
        public const string WMSAPI_msg_Error_ProductoSerieNoPermiteDuplicados = "El manejo de identificador del producto es Serie, no se permite el uso de Duplicados.";
        public const string WMSAPI_msg_Error_DetalleNoEspecificoExistente = "No es posible ingresar un detalle con lote especifico cuando existe un detalle con el mismo lote no especifico.";

        #endregion

        #region Picking
        public const string WMSAPI_msg_Error_PreparacionNoExiste = "El preparación {0} no existe.";
        public const string WMSAPI_msg_Error_TipoPreparacionNoHabilitado = "No está habilitado el pickeo para las preparaciones de tipo {0}.";
        public const string WMSAPI_msg_Error_UbicacionAreaAutomatismo = "La ubicación del contenedor debe ser de área {0} utilizada para automatismo";
        public const string WMSAPI_msg_Error_UbicacionTipoAutomatismo = "La ubicación del contenedor debe ser de tipo {0} utilizad para automatismo";
        public const string WMSAPI_msg_Error_TipoContenedorNoExiste = "El tipo de contenedor {0} no existe.";
        public const string WMSAPI_msg_Error_ContenedorCargadoAlCamion = "El contenedor ya está cargado a un camión.";
        public const string WMSAPI_msg_Error_ContenedorYaUsadoPreparacion = "El contenedor ya fue usado para esta preparación y su estado ya no es válido para volver a usarlo.";
        public const string WMSAPI_msg_Error_ContenedorOtroPredio = "El contenedor no pertenece al predio de la preparación.";
        public const string WMSAPI_msg_Error_ContenedorControlado = "El contenedor fue o está siendo controlado.";
        public const string WMSAPI_msg_Error_ContenedorPrecintado = "El contenedor tiene precintos asignados.";
        public const string WMSAPI_msg_Error_ContenedorFacturado = "El contenedor tiene mercaderia facturada.";
        public const string WMSAPI_msg_Error_ContenedorEmpaquetado = "El contenedor ya está empaquetado.";
        public const string WMSAPI_msg_Error_PreparacionDestinoTipoNoHabilitado = "El tipo de la preparación destino no está habilitado.";
        public const string WMSAPI_msg_Error_PreparacionDestinoDistintaEmpresa = "La preparación destino tiene distinta empresa, ingrese otro contenedor.";
        public const string WMSAPI_msg_Error_PreparacionDestinoDistintaAgrupacion = "La preparación destino tiene distinta agrupación, ingrese otro contenedor.";
        public const string WMSAPI_msg_Error_PreparacionDestinoCarga = "La preparación destino tiene diferente camión asignado.";
        public const string WMSAPI_msg_Error_ContProductoSubClaseDistinta = "El producto pertenece a una subclase distinta a la del contenedor.";
        public const string WMSAPI_msg_Error_PreparacionDestinoDistintaOnda = "La preparación destino y origen tienen distitna onda.";
        public const string WMSAPI_msg_Error_ContenedorDistintaRuta = "El contenedor está siendo usado para otra ruta.";
        public const string WMSAPI_msg_Error_ContenedorIncompatibe = "El pedido de origen y los existentes en el contenedor no puede compartir contenedor.";
        public const string WMSAPI_msg_Error_EstadoDetallePickingInvalido = "Estado de detalle de picking inválido.";
        public const string WMSAPI_msg_Error_SinPickingPendiente = "No hay un pickeo pendiente para el producto {0}, identificador {1}, preparación {2}, pedido {3}, empresa {4}, cliente {5}, ubicación {6}";
        public const string WMSAPI_msg_Error_CantPrepararMayorPendiente = "La cantidad a preparar para el producto {0}, identificador {1}, preparación {2}, pedido {3}, empresa {4}, cliente {5}, ubicación {6}, vlComparteContendor {7} no puede ser mayor a lo pendiente {8} .";
        public const string WMSAPI_msg_Error_ReservaInsuficiente = "No existe reserva suficiente para pickear. Ubicación: {0} - Empresa: {1} - Producto: {2} - Identificador: {3} - Faixa: {4}.";
        public const string WMSAPI_msg_Error_SinReservaDocumentalSuficiente = "No existe reserva documental suficiente. Preparación: {0} - Empresa: {1} - Producto: {2} - Identificador {3}";


        public const string WMSAPI_msg_Error_AgrupacionPedidoDatosInnecesarios = "Al pickear agrupado por Pedido no se deben mandar valores en los campos Carga ni ComparteContenedorPicking.";
        public const string WMSAPI_msg_Error_AgrupacionClienteDatosInnecesarios = "Al pickear agrupado por Cliente no se deben mandar valores en los campos Carga ni Pedido.";
        public const string WMSAPI_msg_Error_AgrupacionRutaDatosInnecesarios = "Al pickear agrupado por Ruta no se deben mandar valores en los campos Pedido, CodigoAgente ni TipoAgente.";
        public const string WMSAPI_msg_Error_AgrupacionOndaDatosInnecesarios = "Al pickear agrupado por Onda no se deben mandar valores en los campos Carga, Pedido, CodigoAgente ni TipoAgente.";

        #endregion

        #region Preparacion

        public const string WMSAPI_msg_Error_AnulacionDuplicada = "Anulación duplicados.Pedido: {0} - Preparacion: {1} - Cliente: {2} - TipoAgente: {3}.";
        public const string WMSAPI_msg_Error_LineasDuplicadas = "Lineas duplicadas. Preparación: {0} - Ubicación: {1} - Empresa: {2} - Pedido: {3} - CodigoAgente: {4} - TipoAgente: {5} - Producto: {6} - Identificador: {7} - Faixa: {8} - Contenedor: {9}.";
        public const string WMSAPI_msg_Error_MismoContenedorDistintasPreparaciones = "No es posible enviar el mismo contenedor para distintas preparaciones es una misma petición. Preparacion: {0} - Contenedor: {1}";
        public const string WMSAPI_msg_Error_PreparacionNoTieneAgrupacion = "La preparación {0} no tiene una agrupación definida valida.";
        public const string WMSAPI_msg_Error_PreparacionSinAgrupacion = "La preparación {0} no tiene una agrupación definida valida .";
        public const string WMSAPI_msg_Error_PreparacionAgrupacionDistinta = "Se envio una agrupación distinta a la de la preparación {0} .";
        public const string WMSAPI_msg_Error_CargaPickeoNoExistente = "No existe la carga {0} .";
        public const string WMSAPI_msg_Error_PickeoAgrupacionDesabilitado = "Pickeo por agrupación {0} no habilitado.";

        #endregion

        #region Produccion
        public const string WMSAPI_msg_Error_IdsExternosProduccionDuplicados = "Se enviaron identificadores duplicados. IdProduccionExterno: {0}.";

        public const string WMSAPI_msg_Error_IngresoSinDetalles = "El ingreso {0} no tiene detalles.";
        public const string WMSAPI_msg_Error_IngresoSinInsumos = "El ingreso {0} no tiene insumos.";
        public const string WMSAPI_msg_Error_InsumosDuplicados = "El ingreso {0} contiene insumos duplicados. Producto: {1} - Identificador {2}.";
        public const string WMSAPI_msg_Error_EnvioLoteEspecificoyAutoNoPermitido = "No se permite enviar un lote especifico y un lote (AUTO) para el mismo producto.";
        public const string WMSAPI_msg_Error_ProductosFinalesDuplicados = "El ingreso {0} contiene productos finales duplicados. Producto: {1}.";
        public const string WMSAPI_msg_Error_IdProduccionExternoExiste = "El id externo {0} ya existe para la empresa {1}.";
        public const string WMSAPI_msg_Error_IdModalidadLoteNoExiste = "La modalidad de ingreso lote {0} no existe.";
        public const string WMSAPI_msg_Error_NoPuedeLiberarSiNoGenera = "No se puede liberar si el pedido no es generado. Ingreso: {0}";
        public const string WMSAPI_msg_Error_TipoIngresoNoExiste = "El tipo de igreso a producción {0} no existe.";
        public const string WMSAPI_msg_Error_TipoIngresoNoHabilitadoDocumental = "El tipo de igreso a producción {0} no esta habilitado para empresas documentales.";
        public const string WMSAPI_msg_Error_EspacioProduccionNoExiste = "El espacio de producción {0} no existe.";
        public const string WMSAPI_msg_Error_EspacioDistintoPredioProduccion = "El espacio de producción {0} no pertenece al mismo predio enviado para la produccion.";
        public const string WMSAPI_msg_Error_IngresoConFormulaNoEnviarDetalles = "Al enviar una formula para asociarla al ingreso no puede enviar insumos ni productos de salida.";
        public const string WMSAPI_msg_Error_IngresoConFormulaNoPuedeGenerarPedido = "Al enviar una formula, no se puede generar pedido con los insumos especificados. El pedido debe ser generado a partir de los datos de la formula.";
        public const string WMSAPI_msg_Error_FormulaNoExiste = "La formula {0} no existe";
        public const string WMSAPI_msg_Error_FormulaDistintaEmpresa = "La formula {0} no pertenece a la empresa de la ejecución";
        public const string WMSAPI_msg_Error_FormulaCantidadPasadasMayorCero = "La cantidad de pasadas debe ser mayor a 0";
        public const string WMSAPI_msg_Error_IngresoConTipoColectorNoPuedeNoGenerarPedido = "El ingreso es de tipo colector y el campo GeneraPedido debe estar especificado con true.";

        #endregion

        #region ProducirProduccion

        public const string WMSAPI_msg_Error_IngresoIncorrecto = "El ingreso {0} no existe o no pertenece a la empresa {1}.";
        public const string WMSAPI_msg_Error_TipoIngresoIncorrecto = "El tipo del ingreso {0} no permite producir ni consumir a través de esta operación.";
        public const string WMSAPI_msg_Error_SituacionNoPermiteProducir = "La situación del ingreso {0} no permite producir.";
        public const string WMSAPI_msg_Error_ProduccionSinProductos = "No existen detalles para producir. Ingreso {0}.";
        public const string WMSAPI_msg_Error_UbicacionProduccionNoCoincide = "La ubicación {0} no coincide con la ubicación de producción del espacio ({1} - {2}) asociado al ingreso.";
        public const string WMSAPI_msg_Error_CantidadProducirIncorrecta = "La cantidad a producir del producto {0} debe ser mayor a 0.";
        public const string WMSAPI_msg_Error_MotivoProduccionNoExiste = "El motivo {0} no es válido para esta operación o no existe en el sistema.";
        public const string WMSAPI_msg_Error_ProductosProducirDuplicados = "Existen productos a producir duplicados. Producto {0} - Identificador {1} - Motivo {2}.";
        public const string WMSAPI_msg_Error_ProducirSinProduccionEspacio = "Para producir de una producción hay que especificar el ID del espacio o el ID Externo del ingreso";
        public const string WMSAPI_msg_Error_NoHayIngresoActivoEnEspacio = "No hay un ingreso de producción activo en el espacio.";
        public const string WMSAPI_msg_Error_NoExisteElEspacio = "El espacio ingresado no existe.";

        #endregion

        #region ConsumirProduccion

        public const string WMSAPI_msg_Error_SituacionNoPermiteConsumir = "La situación del ingreso {0} no permite consumir.";
        public const string WMSAPI_msg_Error_ConsumoSinInsumos = "No existen detalles para consumir. Ingreso {0}.";
        public const string WMSAPI_msg_Error_ConsumoSinProduccionEspacio = "Para consumir un ingreso a producción hay que especificar el ID del espacio o el ID Externo del ingreso";
        public const string WMSAPI_msg_Error_CantidadConsumirIncorrecta = "La cantidad a consumir del producto {0} debe ser mayor a 0.";
        public const string WMSAPI_msg_Error_MotivoConsumoNoExiste = "El motivo {0} no es válido para esta operación o existe en el sistema.";
        public const string WMSAPI_msg_Error_InsumosConsumirDuplicados = "Existen insumos a consumir duplicados. Producto {0} - Identificador {1} - Motivo {2}.";
        public const string WMSAPI_msg_Error_IngresoRealNoExiste = "No se encontraron insumos para el Producto {0} - Identificador {1} en el ingreso {2}.";
        public const string WMSAPI_msg_Error_IngresoRealConsumibleNoExiste = "No se encontraron insumos ajustables para el Producto {0} - Identificador {1} en el ingreso {2}.";
        public const string WMSAPI_msg_Error_IngresoRealReferenciaNoExiste = "No se encontraron insumos para el Producto {0} - Identificador {1} - Referencia {2} en el ingreso {3}.";
        public const string WMSAPI_msg_Error_IngresoRealReferenciaConsumibleNoExiste = "No se encontraron insumos ajustables para el Producto {0} - Identificador {1} - Referencia {2} en el ingreso {3}.";
        public const string WMSAPI_msg_Error_CantidadDisponibleInsuficiente = "No existe suficiente cantidad disponible para consumir. Producto {0} - Identificador {1}.";
        public const string WMSAPI_msg_Error_CantidadDisponibleInsuficienteRef = "No existe suficiente cantidad disponible para consumir. Producto {0} - Identificador {1} - Referencia {2}.";
        public const string WMSAPI_msg_Error_AjusteNoPermitidoSobreIngresosReales = "No es posible realizar ajustes de stock sobre ingresos reales.";
        public const string WMSAPI_msg_Error_IngresoYaActivo = "El ingreso a producción {0} ya se encuentra activo.";
        public const string WMSAPI_msg_Error_IngresoSinEspacio = "El ingreso a producción {0} no tiene un espacio de producción asignado.";
        public const string WMSAPI_msg_Error_EspacioOcupadoIngresoActivo = "Ya hay un ingreso a producción activo asociado al espacio.";
        public const string WMSAPI_msg_Error_NoHayOrdenActivaEspacio = "El espacio está asociado a más de una orden de producción compatible con esta operación.";
        public const string WMSAPI_msg_Error_EspacioSinOrdenAsociada = "El espacio ingresado no tiene ninguna orden de producción asociada que permita realizar esta operación.";
        public const string WMSAPI_msg_Error_EspacioIncorrecto = "El espacio asociado al ingreso {0} no coincide con el espacio {1}.";


        #endregion

        #region Producto
        public const string WMSAPI_msg_Error_ProductoNoEncontrado = "No se encontró el producto '{0}' para la empresa '{1}'.";
        public const string WMSAPI_msg_Error_FamiliaNoExiste = "Familia: La familia '{0}' no existe.";
        public const string WMSAPI_msg_Error_RamoNoExiste = "Ramo: El ramo '{0}' no existe.";
        public const string WMSAPI_msg_Error_ClaseNoExiste = "Clase: La clase '{0}' no existe.";
        public const string WMSAPI_msg_Error_ClaseProductoConStock = "Clase: No se permite modificar porque el producto está siendo utilizado.";
        public const string WMSAPI_msg_Error_GrupoConsultaNoExiste = "Grupo consulta: El valor ingresado no existe.";
        public const string WMSAPI_msg_Error_RotatividadNoExiste = "Rotatividad: El valor ingresado no existe.";
        public const string WMSAPI_msg_Error_UnidadMedidaNoExiste = "UnidadMedidia: El valor ingresado no existe.";
        public const string WMSAPI_msg_Error_ProductoEmpresaExistente = "ProductoEmpresa: El valor está asignado a otro producto.";
        public const string WMSAPI_msg_Error_NAMNoExiste = "NAM: El valor ingresado no existe.";
        public const string WMSAPI_msg_Error_AgrupacionNoValida = "Agrupacion: El valor ingresado no puede ser distinto a P, O, R o C";
        public const string WMSAPI_msg_Error_TipoIdentificadorSerieNoAceptaDecimales = "El manejo de identificador Serie no permite aceptar decimales. Producto {0}.";
        public const string WMSAPI_msg_Error_TipoSerieCantidadDistintaAUno = "{0}: El manejo de identificador del producto {1} es Serie, no se permite ingresar una cantidad distinta a 1.";

        public const string WMSAPI_msg_Error_SituacionProductoInvalida = "La situación ingresada '{0}' no es válida. Debe ingresar 15 o 16.";
        public const string WMSAPI_msg_Error_ManejoIdentificadorInvalido = "El manejo de identificador ingresado no es válido.";
        public const string WMSAPI_msg_Error_ManejoIdentificadorProductoStock = "ManejoIdentificador: No se permite modificar porque el producto está siendo utilizado.";
        public const string WMSAPI_msg_Error_TipoManejoFechaInvalido = "El tipo de manejo de fecha ingresado no es válido.";
        public const string WMSAPI_msg_Error_TipoManejoFechaProductoStock = "TipoManejoFecha: No se permite modificar porque el producto está siendo utilizado.";
        public const string WMSAPI_msg_Error_ModLoteIncompatibleConTpIdent = "ModalidadIngresoLote: El tipo de manejo de identificador '{0}' no permite una modalidad distinta a '{1}'.";
        public const string WMSAPI_msg_Error_ModLoteNoValida = "ModalidadIngresoLote: La modalidad ingresada no es válida.";
        public const string WMSAPI_msg_Error_AceptaDecimalesProductoStock = "AceptaDecimales: No se permite modificar porque el producto está siendo utilizado.";
        public const string WMSAPI_msg_Error_ProductoDuplicados = "Productos duplicados. Código: {0} - Empresa: {1}.";

        public const string WMSAPI_msg_Error_PesoBruto = "Peso Bruto; Debe ser 1, 2 o 3.";
        public const string WMSAPI_msg_Error_ManejoDecimalesUnidadBulto = "UnidadBulto: El producto no maneja decimales.";
        public const string WMSAPI_msg_Error_ManejoDecimalesUnidadDistribucion = "UnidadDistribucion: El producto no maneja decimales.";

        #endregion

        #region ProductoProveedor
        public const string WMSAPI_msg_Error_ProductoProveedorNoEncontrado = "No se encontró el producto proveedor '{0}' de la empresa '{1}' y agente '{2}-{3}'.";
        public const string WMSAPI_msg_Error_ProductoProveedorYaExiste = "El producto proveedor '{0}' para la empresa '{1}' y agente '{2}-{3}' ya existe.";
        public const string WMSAPI_msg_Error_ProductoProveedorDuplicado = "Productos duplicados. Código: '{0}' - Empresa: '{1}' - Agente: '{3}'-'{4}'.";
        public const string WMSAPI_msg_Error_ProductoProveedorNoExiste = "El producto proveedor '{0}' para empresa '{1}' y agente '{2}-{3}' no existe.";
        public const string WMSAPI_msg_Error_ExternoOtroProducto = "El código externo está asignado a otro producto.";
        public const string WMSAPI_msg_Error_ProductoProveedorCodigoExternoDuplicado = "Código externo '{0}' duplicado.";

        #endregion

        #region Ptl 

        public const string Ptl_msg_Error_UsuarioNoAutenticado = "Usuario no autenticado";
        //Ptl Service

        public const string WMSAPI_msg_Error_PtlFinalizarOperativa = "Tiene que finalizar operativa en PTL. Hay {0} luces prendidas del producto: {1}.";
        public const string WMSAPI_msg_Error_PtlColorEnUso = "El color no tiene usuario definido. Vuelva a elegir el PTL.";
        public const string WMSAPI_msg_Error_PtlColorOtroUsuario = "El color ya le pertenece a otro usuario. Vuelva a elegir el PTL.";
        public const string WMSAPI_msg_Error_PtlUbicacionCerrandose = "La ubicacion ya se está cerrando.";
        public const string WMSAPI_msg_Error_PtlErrorNoControlado = "Error no controlado:{0} {1}";
        public const string WMSAPI_msg_Error_PtlErrorLuzApagada = "La luz ya fue apagada.";


        //PtlCrossDockingEnUnaFaseService

        public const string WMSAPI_msg_Error_PtlNoSePuedePikear = "No se puede pickear de más.";
        public const string WMSAPI_msg_Error_PtlUbicacionSalidaNoConfigurada = "La ubicación de salida no fue configurada para el PTL notificado";
        public const string WMSAPI_msg_Error_PtlProductoAtendido = "El producto ya está siendo atendido.";
        public const string WMSAPI_msg_Error_PtlOperativaSinFinalizar = "Tiene que finalizar operativa en PTL. Hay {0} luces prendidas del producto: {1}.";
        public const string WMSAPI_msg_Error_PtlNoMercaderiaASeparar = "No mercaderia disponible a separar";
        public const string WMSAPI_msg_Error_PtlErrorNoControladoEx = "Error no controlado:{0}";
        public const string WMSAPI_msg_Error_PtlSinPosicionesDisponibles = "No hay posiciones disponibles para separar";
        public const string WMSAPI_msg_Error_PtlNoSePuedenSepararCantidades = "No se pudieron separar cantidades. Pendiente XD: {0} / Almacenar: {1}";
        public const string WMSAPI_msg_Error_PtlOperativaSinFinalizarPtl = "Tiene que finalizar operativa en PTL. Hay {0}";
        public const string WMSAPI_msg_Error_PtlAgrupacionAtendida = "La agrupación ya está siendo atendida por otro usuario.";
        public const string WMSAPI_msg_Error_PtlYaExisteReferencia = "Existe una referencia para este PTL.";


        //PtlSeparacionEnDosFasesService



        #endregion

        #region Referencias de Recepción

        public const string WMSAPI_msg_Error_ReferenciasDuplicadas = "Referencias duplicadas. Referencia: '{0}' - Empresa: '{1}' - TipoReferencia: '{2}' - Agente: '{3}'-'{4}'.";
        public const string WMSAPI_msg_Error_DetallesReferenciaDuplicados = "La referencia {0} contiene detalles duplicados. IdLineaSistemaExterno: {1} - Producto: {2} - Identificador: {3}.";
        public const string WMSAPI_msg_Error_ReferenciaNoEncontrada = "No se encontró la referencia con el nro '{0}' para la empresa '{1}'.";
        public const string WMSAPI_msg_Error_ReferenciaYaExiste = "La referencia '{0}' con el tipo '{1}' y agente '{2}-{3}' ya existe para la empresa '{4}'.";
        public const string WMSAPI_msg_Error_ReferenciaIDNoEncontrada = "No se encontró la referencia con el id '{0}'.";
        public const string WMSAPI_msg_Error_TipoReferenciaNoExiste = "El tipo de referencia '{0}' no existe.";
        public const string WMSAPI_msg_Error_TpRefSinTpRec = "El tipo de referencia no está asociado a ningún tipo de recepción.";
        public const string WMSAPI_msg_Error_TpRefSinTpAgente = "El tipo de referencia no es válida para el tipo de agente.";
        public const string WMSAPI_msg_Error_MonedaNoExiste = "El código de moneda '{0}' no existe.";
        public const string WMSAPI_msg_Error_ProductoNoManejaDecimales = "El producto '{0}' no maneja decimales.";
        public const string WMSAPI_msg_Error_ProductoDeLpnFueEspecificadoANivelDeCabezal = "El producto '{0}' - IdLineaExterno: '{1}' - Tipo '{2}' ya fue expecificado a nivel de cabezal.";
        public const string WMSAPI_msg_Error_ProductoNoManejaLote = "El producto '{0}' no maneja lote.";

        public const string WMSAPI_msg_Error_ReferenciaNOExiste = "La referencia '{0}' con el tipo '{1}' y agente '{2}-{3}' no existe para la empresa '{4}'.";
        public const string WMSAPI_msg_Error_ReferenciaEstadoIncorrecto = "El estado de la referencia '{0}' no permite realizar esta operación.";
        public const string WMSAPI_msg_Error_LineaDetalleNoExiste = "El detalle de la referencia no existe, si desea agregarlo debe utilizar otro tipo de operación.";
        public const string WMSAPI_msg_Error_LineaDetalleYaExiste = "El detalle de la referencia ya existe, si desea modificarlo debe utilizar otro tipo de operación.";
        public const string WMSAPI_msg_Error_TpOperacionInvalido = "El tipo de operación debe ser A, M, R o N.";
        public const string WMSAPI_msg_Error_TpOperacionACantidadNula = "Para el tipo de operación A la cantidad debe ser 0.";
        public const string WMSAPI_msg_Error_SaldoInsuficiente = "No hay saldo suficiente para realizar esta operación (Ref: '{0}' - IdLineaExterno: '{1}' - Prod: '{2}' - Identificador: '{3}'";

        public const string WMSAPI_msg_Error_ReferenciaYaAnulada = "La referencia '{0}' ya está anulada completamente.";
        public const string WMSAPI_msg_Error_ReferenciaEnUso = "La referencia está en uso y no puede ser modificada.";

        public const string WMSAPI_msg_Error_AgendaNoExiste = "La agenda: {0} empresa {1} no existe.";
        public const string WMSAPI_msg_Error_AgendaSituacionNoPermiteRealizarCrossDocking = "La situación de la agenda {0} no es la correcta para un cross docking en una fase.";
        public const string WMSAPI_msg_Error_AgendaNoTieneCrossDockingActivo = "La agenda {0} no tiene cross docking activo con la preparacion {1}.";
        public const string WMSAPI_msg_Error_LaUbicacionNoEsLaEspecificadaParaLaAgenda = "La ubicación: {0} no es la especificada para la agenda.";
        public const string WMSAPI_msg_Error_LaUbicacionPerteneceAUnaPuerta = "La ubicación: {0} no es una puerta.";
        public const string WMSAPI_msg_Error_AgenteNoExite = "El codigo: {0} tipo {1} empresa: {2} no existe.";
        public const string WMSAPI_msg_Error_ContenedorEstaEnOtraPreparacionActiva = "El IdExternoContenedor {0} - Tipocontenedor: {1} está siendo utilizado por la preparación {2}";
        public const string WMSAPI_msg_Error_CantidadIngrasadaSuperaLaPendiente = "La cantidad de la agenda: {0} producto: {1} identificador: {2} cliente: {3}";
        public const string WMSAPI_msg_Error_DetalleNoTieneCrossDocking = "La agenda: {0} producto: {1} identificador: {2} cliente: {3} no tiene cross docking.";
        public const string WMSAPI_msg_Error_TipoEtiquetaInvalida = "El tipo de etiqueta {0} es invalida";
        public const string WMSAPI_msg_Error_SituacionDestinoNoPermitidaEnInterfaz = "La situación invalida. Solo admite la situacion {0} , {1}.";

        #endregion

        #region Stock
        public const string WMSAPI_msg_Error_StockNoEncontrado = "No se encontró stock para los filtros enviados.";
        public const string WMSAPI_msg_Error_CantidadMovimientoNoPuedeSer0 = "El producto {0} la cantidad no puede ser 0";
        public const string WMSAPI_msg_Error_NoExisteSaldoSuficienteParaEfectuarelAjuste = "No existe saldo suficiente para modificar la cantidad de la ubicación {0}, producto {1}, empresa '{2}', identificador  '{3}', faixa '{4}'.";
        public const string WMSAPI_msg_Error_NoSePuedeAjustarAjuste = "No se puede ajustar el stock ubicación {0}, producto {1}, empresa '{2}', identificador  '{3}', faixa '{4}'.";
        public const string WMSAPI_msg_Error_NoSePuedeAjustarAjusteControlCalidadPendiente = "No se puede ajustar el stock tierne control de calidad pendiente ubicación {0}, producto {1}, empresa '{2}', identificador  '{3}', faixa '{4}'.";
        public const string WMSAPI_msg_Error_AreaNoPermiteAjustar = "El área {0} de la ubicación {1} permite ser ajustada.";
        public const string WMSAPI_msg_Error_UbicacionPickingNoAsignadaProducto = "La ubicación {0} no está asignada al producto {1}";
        public const string WMSAPI_msg_Error_UbicacionMonoLote = "La ubicación {0} es mono-lote";
        public const string WMSAPI_msg_Error_UbicacionMonoProducto = "La ubicación {0} es mono-producto";
        public const string WMSAPI_msg_Error_UbicacionTieneDistintaClaseProducto = "La ubicación {0} tiene clase {1} y es distinta a la del producto {2} de la empresa {3} clase {4}";
        public const string WMSAPI_msg_Error_MotivoAjusteNoExiste = "El codigo de motivo ajuste no existe";
        public const string WMSAPI_msg_Error_TipoDeAjusteInvalido = "Los tipo de ajustes validos son {0} - {1}";

        public const string WMSAPI_msg_Error_PredioUnico = "Las ubicaciones {0} y {1} son de distinto predio.";
        public const string WMSAPI_msg_Error_StockPendienteInventario = "No es posible transferir stock con un inventario activo.";
        public const string WMSAPI_msg_Error_OrigenDistintaAveriaDestino = "El stock de origen tiene distinto Valor de Averia que el stock destino";
        public const string WMSAPI_msg_Error_OrigenDistintoCtrlCalidadDestino = "El stock origen tiene distinto contro lde calidad que el stock destino.";
        public const string WMSAPI_msg_Error_UbicacionEquipoInvalida = "El usuario no tiene una ubicación de equipo asignada para el predio o la establecida en el parametro no es válida.";
        public const string WMSAPI_msg_Error_SinStock = "No existe stock. Ubicación: {0} - Empresa: {1} - Producto: {2} - Identificador: {3},  Faixa {4}";

        #endregion

        #region Tracking
        public const string TRK_msg_Error_ZonaSinRutaAsociada = "No existe ninguna ruta asociada para la zona {0}.";
        public const string TRK_msg_Error_ZonConRutaAsociadaExistente = "Ya existe una ruta asociada para la zona {0}.";
        public const string TRK_msg_Error_AgentesDuplicados = "Agentes duplicados. Codigo: {0} - Tipo: {1} - Empresa: {2}";
        #endregion

        #region Transferencia 
        public const string WMSAPI_msg_Error_TransferenciaDuplicada = "Transferencias duplicadas. Origen: {0} - Destino: {1} - Empresa: {2} - Producto: {3} - Identificador: {4} - Faixa: {5}.";

        #endregion

        #region Factura
        public const string WMSAPI_msg_Error_FacturasDuplicadas = "Facturas duplicadas. Factura: '{0}' - Empresa: '{1}' - Serie: '{2}' - Agente: '{3}'.";
        public const string WMSAPI_msg_Error_FacturaDetallesDuplicadas = "La factura {0} contiene detalles duplicados. Producto: {1} - Identificador: {2}.";
        public const string WMSAPI_msg_Error_FacturaYaExiste = "La factura '{0}' con el nro de serie '{1}' y agente '{2}' ya existe para la empresa '{3}'.";
        public const string WMSAPI_msg_Error_FacturaNoEncontrada = "No se encontró la factura '{0}' para la empresa '{1}'";
        public const string WMSAPI_msg_Error_TioiFacturaNoExiste = "El tipo de factura {0} no existe.";

        #endregion

        #region Ubicaciones Picking

        public const string WMSAPI_msg_Error_UbicacionesPickingDuplicadas = "Ubicaciones de picking duplicadas. Ubicación: '{0}' - Empresa: '{1}' - Producto: '{2}' - Padron: '{3}' - Prioridad: '{4}' .";
        public const string WMSAPI_msg_Error_UbiPickingExistente = "La ubicación '{0}' ya se encuentra asignado al producto '{1}' para la empresa '{2}'.";
        public const string WMSAPI_msg_Error_DeleteUbiPickingNoExiste = "No se encontro una ubicación de picking para la Ubicación: '{0}', Producto: '{1}', Empresa: '{2}'.";
        public const string WMSAPI_msg_Error_StockMaximoMenorMin = "El stock maximo tiene que ser mayor al stock minimo.";
        public const string WMSAPI_msg_Error_UbicacionInvalida = "La ubicación '{0}' no es valida para asignarla como ubicación de picking.";
        public const string WMSAPI_msg_Error_ProdManejaSeriePadron = "El manejo de identificador del producto es Serie, no se permite ingresar un padrón distinto a 1.";
        public const string WMSAPI_msg_Error_UbicacionPickingNoEncontrada = "No se encontró la ubicación de picking '{0}'.";
        public const string General_Sec0_Error_UbicacionPickingPadronExist = "Ya existe una ubicación de picking para el producto: '{0}', Predio '{1}', Padrón: '{2}', Empresa: '{3}', Prioridad: '{4}'";
        public const string General_Sec0_Error_PrioridadMax = "La prioridad no puede ser mayor a 99";
        public const string General_Sec0_Error_CodigoCajaAutomatismoNoExist = "El codigo unidad '{0}' de caja de automatismo no existe";
        public const string General_Sec0_Error_UbicacionMonoProductoAsignada = "La ubicación '{0}' es mono-producto y ya cuenta con otro producto asignado";
        public const string General_Sec0_Error_UbicacionMonoProductoYOtroStock = "La ubicación '{0}' es mono-producto y ya cuenta con stock de otro producto";
        public const string General_Sec0_Error_ProductoDistintaClaseUbicacion = "La ubicacion '{0}' tiene una clase distinta a la del producto.";
        public const string WMSAPI_msg_Error_UbicacionesPickingDuplicadasPk = "Hay lineas duplicadas para el Predio: '{0}' - Empresa: '{1}' - Producto: '{2}' - Padron: '{3}' - Prioridad: '{4}' .";

        #endregion

    }
}
