import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { EXP040CreateEgresoModal } from './EXP040CreateEgresoModal';
import { EXP040UpdateEgresoModal } from './EXP040UpdateEgresoModal';
import { EXP040ArmarEgresoContenedorModal } from './EXP040ArmarEgresoContenedorModal';
import { EXP040ArmarEgresoCargaModal } from './EXP040ArmarEgresoCarga';
import { EXP040ArmarEgresoPedidoModal } from './EXP040ArmarEgresoPedido';
import { EXP040ErrorModal } from './EXP040ErrorModal';
import { EXP040CreatePlanificacionModal } from './EXP040CreatePlanificacionModal';
import { EXP040UpdatePlanificacionModal } from './EXP040UpdatePlanificacionModal';

export default function EXP040(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    const PermissionsCheck = useState({ Egreso: 'EXP040CreateEgreso_Page_Access_Allow', Planificacion: 'EXP040CreatePlanificacion_Page_Access_Allow' });

    const [showPopup, setShowPopup] = useState(false);
    const [showUpdatePopup, setShowUpdatePopup] = useState(false);
    const [showArmarEgresoContenedorPopup, setShowArmarEgresoContenedorPopup] = useState(false);
    const [showArmarEgresoCargaPopup, setShowArmarEgresoCargaPopup] = useState(false);
    const [showArmarEgresoPedidoPopup, setShowArmarEgresoPedidoPopup] = useState(false);
    const [camionEditar, setCamionEditar] = useState(null);
    const [emp, setEmpresa] = useState(null);
    const [cdRuta, setRuta] = useState(null);
    const [nexus, setNexus] = useState(null);

    const [showErrorPopup, setShowErrorPopup] = useState(false);
    const [erroresValidacion, setErroresValidacion] = useState(null);

    const [disabledEgreso, setDisabledEgreso] = useState(false);
    const [disabledPlanificacion, setDisabledPlanificacion] = useState(false);

    const [showPopupPlanificacion, setShowPopupPlanificacion] = useState(false);
    const [showUpdatePlanificacion, setShowUpdatePlanificacion] = useState(false);

    const openFormDialog = () => {
        setShowPopup(true);
    }
    const closeFormDialog = () => {
        setShowPopup(false);
    }
    const closeUpdateFormDialog = () => {
        setShowUpdatePopup(false);
    }

    const openFormPlanificacion = () => {
        setShowPopupPlanificacion(true);
    }
    const closeFormPlanificacion = () => {
        setShowPopupPlanificacion(false);
    }
    const closeUpdatePlanificacion = () => {
        setShowUpdatePlanificacion(false);
    }

    const closeArmarEgresoContenedorDialog = () => {
        setShowArmarEgresoContenedorPopup(false);
        nexus.getGrid("EXP040_grid_1").refresh();
    }

    const closeArmarEgresoCargaDialog = () => {
        setShowArmarEgresoCargaPopup(false);
        nexus.getGrid("EXP040_grid_1").refresh();
    }

    const closeArmarEgresoPedidoDialog = () => {
        setShowArmarEgresoPedidoPopup(false);
        nexus.getGrid("EXP040_grid_1").refresh();
    }

    const closeErrorDialog = () => {
        setShowErrorPopup(false);
    }

    const handleBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnEditar") {
            context.abortServerCall = true;

            setCamionEditar(data.row.cells.find(d => d.column === "CD_CAMION").value);
            const tpArmado = data.row.cells.find(d => d.column === "TP_ARMADO_EGRESO").value;

            if (tpArmado === "P")
                setShowUpdatePlanificacion(true);
            else
                setShowUpdatePopup(true);
        }
        else {
            const camion = data.row.cells.find(d => d.column === "CD_CAMION").value;
            data.parameters = [{ id: "camion", value: camion }];
        }
    }

    const handleAfterButtonAction = (data, nexus) => {
        if (data.buttonId === "btnArmarCamionCont") {
            setCamionEditar(data.row.cells.find(d => d.column === "CD_CAMION").value);
            setEmpresa(data.row.cells.find(d => d.column === "CD_EMPRESA").value);
            setRuta(data.row.cells.find(d => d.column === "CD_ROTA").value);

            setShowArmarEgresoContenedorPopup(true);
        }
        else if (data.buttonId === "btnArmarCamion") {
            setCamionEditar(data.row.cells.find(d => d.column === "CD_CAMION").value);
            setEmpresa(data.row.cells.find(d => d.column === "CD_EMPRESA").value);
            setRuta(data.row.cells.find(d => d.column === "CD_ROTA").value);

            setShowArmarEgresoCargaPopup(true);
        }
        else if (data.buttonId === "btnArmarCamionPedido") {
            setCamionEditar(data.row.cells.find(d => d.column === "CD_CAMION").value);
            setEmpresa(data.row.cells.find(d => d.column === "CD_EMPRESA").value);
            setRuta(data.row.cells.find(d => d.column === "CD_ROTA").value);

            setShowArmarEgresoPedidoPopup(true);
        }
        else if (data.buttonId === "btnFacturarCamion") {
            const errores = data.parameters.find(d => d.id === "resultadoValidacion");

            if (errores) {
                setErroresValidacion(JSON.parse(errores.value));

                setShowErrorPopup(true);

                return;
            }

            nexus.getGrid("EXP040_grid_1").refresh();
        }
        else if (data.buttonId === "btnCerrarCamion" || data.buttonId === "btnSincronizarTracking" ||
                data.buttonId === "btnReSincronizarTracking" || data.buttonId === "btnCancelarCamion") {
            const errores = data.parameters.find(d => d.id === "resultadoValidacion");
            if (errores) {
                setErroresValidacion(JSON.parse(errores.value));
                setShowErrorPopup(true);
                return;
            }
            nexus.getGrid("EXP040_grid_1").refresh();
        }
    }

    const onAfterPageLoad = (data) => {
        if (data.parameters.length > 0) {
            setDisabledEgreso(data.parameters.find(x => x.id === PermissionsCheck[0].Egreso).value)
            setDisabledPlanificacion(data.parameters.find(x => x.id === PermissionsCheck[0].Planificacion).value)
        }
    }

    const onAfterInitialize = (context, grid, params, nexus) => {
        setNexus(nexus)
    }

    return (
        <Page
            title={t("EXP040_Sec0_pageTitle_Titulo")}
            onAfterLoad={onAfterPageLoad}
            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button id="CrearEgreso" className="btn btn-primary" disabled={disabledEgreso !== "True"} onClick={openFormDialog}>{t("EXP040_Sec0_btn_CrearEgreso")}</button>
                &nbsp;
                <button id="CrearPlanificacion" className="btn btn-primary" disabled={disabledPlanificacion !== "True"} onClick={openFormPlanificacion}>{t("EXP040_Sec0_btn_CrearPlanificacion")}</button>

            </div>
            <div className="row mb-4">
                <div className="col-12">
                    <Grid
                        id="EXP040_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        onBeforeButtonAction={handleBeforeButtonAction}
                        onAfterButtonAction={handleAfterButtonAction}
                        onAfterInitialize={onAfterInitialize}
                        enableExcelExport
                    />
                </div>
            </div>
            <EXP040CreateEgresoModal show={showPopup} onHide={closeFormDialog} />
            <EXP040CreatePlanificacionModal show={showPopupPlanificacion} onHide={closeFormPlanificacion} />
            <EXP040UpdateEgresoModal show={showUpdatePopup} onHide={closeUpdateFormDialog} camion={camionEditar} />
            <EXP040UpdatePlanificacionModal show={showUpdatePlanificacion} onHide={closeUpdatePlanificacion} camion={camionEditar} />
            <EXP040ArmarEgresoContenedorModal show={showArmarEgresoContenedorPopup} onHide={closeArmarEgresoContenedorDialog} camion={camionEditar} empresa={emp} ruta={cdRuta} />
            <EXP040ArmarEgresoCargaModal show={showArmarEgresoCargaPopup} onHide={closeArmarEgresoCargaDialog} camion={camionEditar} empresa={emp} ruta={cdRuta} />
            <EXP040ArmarEgresoPedidoModal show={showArmarEgresoPedidoPopup} onHide={closeArmarEgresoPedidoDialog} camion={camionEditar} empresa={emp} ruta={cdRuta} {...props} />
            <EXP040ErrorModal show={showErrorPopup} onHide={closeErrorDialog} errores={erroresValidacion} />
        </Page>
    );
}