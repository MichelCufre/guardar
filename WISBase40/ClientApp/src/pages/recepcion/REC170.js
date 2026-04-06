import React, { useState } from 'react';
import { Modal } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { IMP080ImpresionEtiquetasRecModal } from '../impresion/IMP080ImpresionEtiquetasRecModal';
import { REC170AsociarAgendaFacturaModal } from './REC170AsociarAgendaFacturaModal';
import { REC170CreateAgendaModal } from './REC170CreateAgendaModal';
import { REC170LineasAgendaModal } from './REC170LineasAgendaModal';
import { REC170PlanificacionLpnModal } from './REC170PlanificacionLpnModal';
import { REC170RecepcionLpnModal } from './REC170RecepcionLpnModal';
import { REC170SeleccionLpnModal } from './REC170SeleccionLpnModal';
import { REC170SincronizarTrackingModal } from './REC170SincronizarTrackingModal';
import { REC170UpdateAgendaModal } from './REC170UpdateAgendaModal';
import { REC170ValidarFacturaModal } from './REC170ValidarFacturaModal';

export default function REC170(props) {

    const { t } = useTranslation();

    const [keyAgenda, setkeyAgenda] = useState(null);

    const [showModal, setShowModal] = useState(false);
    const [showPopupAdd, setShowPopupAdd] = useState(false);
    const [showBotonCreate, setShowBotonCreate] = useState(false);
    const [showPopupUpdate, setShowPopupUpdate] = useState(false);
    const [showPopupLineas, setShowPopupLineas] = useState(false);
    const [showPopupLpn, setShowPopupLpn] = useState(false);
    const [showAsociarAgendaFacturaPopup, setshowAsociarAgendaFacturaPopup] = useState(false);
    const [showValidarFacturaPopup, setshowValidarFacturaPopup] = useState(false);
    const [showModalImpresion, setShowModalImpresion] = useState(false);

    const [showTrackingModal, setShowTrackingModal] = useState(false);
    const [showPopUpTracking, setShowPopUpTracking] = useState(false);
    const [showPlanificacionLpn, setShowPlanificacionLpn] = useState(false);
    const [showModalRecepcionLpn, setShowModalRecepcionLpn] = useState(false);
    const [_nexus, setNexus] = useState(null);

    const PageOnAfterLoad = (data) => {

        let deshabilitaCreacion = data.parameters.find(p => p.id === "deshabilitaCreacion");

        if (deshabilitaCreacion) {

            if (deshabilitaCreacion.value == "true") {
                setShowBotonCreate(true);
            }
        }
    }

    const openFormDialog = () => {
        setShowModal(true);

        setShowPopupAdd(true);
        setShowPopupLpn(false);
        setShowPopupUpdate(false);
        setShowPopupLineas(false);
        setShowModalImpresion(false);
        setShowPlanificacionLpn(false);
        setShowModalRecepcionLpn(false);
        setshowValidarFacturaPopup(false);
        setshowAsociarAgendaFacturaPopup(false);
    }

    const openFormUpdateDialog = () => {
        setShowModal(true);

        setShowPopupAdd(false);
        setShowPopupLpn(false);
        setShowPopupUpdate(true);
        setShowPopupLineas(false);
        setShowModalImpresion(false);
        setShowPlanificacionLpn(false);
        setShowModalRecepcionLpn(false);
        setshowValidarFacturaPopup(false);
        setshowAsociarAgendaFacturaPopup(false);
    }

    const openFormLineasDialog = () => {
        setShowModal(true);

        setShowPopupAdd(false);
        setShowPopupLpn(false);
        setShowPopupUpdate(false);
        setShowPopupLineas(true);
        setShowModalImpresion(false);
        setShowPlanificacionLpn(false);
        setShowModalRecepcionLpn(false);
        setshowValidarFacturaPopup(false);
        setshowAsociarAgendaFacturaPopup(false);
    }

    const openFormValidarFacturaDialog = () => {
        setShowModal(true);

        setShowPopupAdd(false);
        setShowPopupLpn(false);
        setShowPopupUpdate(false);
        setShowPopupLineas(false);
        setShowModalImpresion(false);
        setShowPlanificacionLpn(false);
        setShowModalRecepcionLpn(false);
        setshowValidarFacturaPopup(true);
        setshowAsociarAgendaFacturaPopup(false);
    }

    const closeValidarFacturaDialog = () => {
        _nexus.getGrid("REC170_grid_1").refresh();
        setShowModal(false);
        setshowValidarFacturaPopup(false);
    }

    const openFormAsociarAgendaFacturaDialog = () => {
        setShowModal(true);

        setShowPopupAdd(false);
        setShowPopupLpn(false);
        setShowPopupUpdate(false);
        setShowPopupLineas(false);
        setShowModalImpresion(false);
        setShowPlanificacionLpn(false);
        setShowModalRecepcionLpn(false);
        setshowValidarFacturaPopup(false);
        setshowAsociarAgendaFacturaPopup(true);
    }

    const closeAsociarAgendaFacturaDialog = () => {
        _nexus.getGrid("REC170_grid_1").refresh();
        setShowModal(false);
        setshowAsociarAgendaFacturaPopup(false);
    }

    const openFormLpnDialog = () => {
        setShowModal(true);

        setShowPopupAdd(false);
        setShowPopupLpn(true);
        setShowPopupUpdate(false);
        setShowPopupLineas(false);
        setShowModalImpresion(false);
        setShowPlanificacionLpn(false);
        setShowModalRecepcionLpn(false);
        setshowValidarFacturaPopup(false);
        setshowAsociarAgendaFacturaPopup(false);
    }

    const closeFormDialog = (agenda, siguiente) => {

        setShowPopupAdd(false);

        if (siguiente) {

            setkeyAgenda(
                [
                    { id: "idAgenda", value: agenda }
                ]
            );

            if (siguiente === "detalles") {
                openFormLineasDialog();
            }
            else if (siguiente === "seleccionLpn") {
                openFormLpnDialog();
            }

        } else {

            setShowModal(false);
        }
    }

    const closeFormLineasDialog = () => {
        _nexus.getGrid("REC170_grid_1").refresh();
        setShowModal(false);
        setShowPopupLineas(false);
    }
    const closeFormUpdateDialog = () => {

        setShowModal(false);
        setShowPopupUpdate(false);
    }

    const closeFormLpnDialog = () => {
        _nexus.getGrid("REC170_grid_1").refresh();
        setShowModal(false);
        setShowPopupLpn(false);
    }

    const openImpresionDialog = () => {
        setShowModalImpresion(true);
    }
    const closeImpresionDialog = () => {
        setShowModalImpresion(false);
    }

    const openPlanificacionLpnDialog = () => {
        setShowPlanificacionLpn(true);
    }
    const closePlanificacionLpnDialog = () => {
        setShowPlanificacionLpn(false);
    }

    const openRecepcionLpnDialog = () => {
        setShowModalRecepcionLpn(true);
    }
    const closeRecepcionLpnDialog = () => {
        setShowModalRecepcionLpn(false);
    }

    const GridOnBeforeButtonAction = (context, data, nexus) => {

        if (data.buttonId === "btnEditar") {

            context.abortServerCall = true;

            setkeyAgenda(
                [
                    { id: "idAgenda", value: data.row.cells.find(w => w.column == "NU_AGENDA").value }
                ]
            );

            openFormUpdateDialog();

        }
        else if (data.buttonId === "btnLineas") {

            data.parameters.push({ id: "idAgenda", value: data.row.cells.find(w => w.column == "NU_AGENDA").value });

        }
        else if (data.buttonId === "btnAsociarFacturaAgenda") {

            data.parameters.push({ id: "idAgenda", value: data.row.cells.find(w => w.column == "NU_AGENDA").value });
            setkeyAgenda(
                [
                    { id: "idAgenda", value: data.row.cells.find(w => w.column == "NU_AGENDA").value }
                ]);
        }
        else if (data.buttonId === "btnImprimir") {

            context.abortServerCall = true;

            setkeyAgenda(
                [
                    { id: "idAgenda", value: data.row.cells.find(w => w.column == "NU_AGENDA").value }
                ]
            );

            openImpresionDialog();
        }
        else if (data.buttonId === "btnPlanificacionLpns") {

            context.abortServerCall = true;

            setkeyAgenda(
                [
                    { id: "idAgenda", value: data.row.cells.find(w => w.column == "NU_AGENDA").value }
                ]
            );

            openPlanificacionLpnDialog();
        }
        else if (data.buttonId === "btnRecepcionLpns") {

            context.abortServerCall = true;

            setkeyAgenda(
                [
                    { id: "idAgenda", value: data.row.cells.find(w => w.column == "NU_AGENDA").value }
                ]
            );

            openRecepcionLpnDialog();
        }
    };

    const GridOnAfterButtonAction = (data, nexus) => {

        if (data.buttonId === "btnLineas") {

            if (data.parameters.find(p => p.id === "lpn")) {
                setkeyAgenda(
                    [
                        { id: "idAgenda", value: data.row.cells.find(w => w.column == "NU_AGENDA").value }
                    ]
                );

                openFormLpnDialog();
            }
            else if (!data.parameters.find(p => p.id === "Lockeada")) {

                setkeyAgenda(
                    [
                        { id: "idAgenda", value: data.row.cells.find(w => w.column == "NU_AGENDA").value }
                    ]
                );

                openFormLineasDialog();
            }

        }
        else if (data.buttonId === "btnAsociarFacturaAgenda") {
            if (!data.parameters.find(p => p.id === "Lockeada")) {
                setkeyAgenda(
                    [
                        { id: "idAgenda", value: data.row.cells.find(w => w.column == "NU_AGENDA").value }
                    ]
                );

                openFormAsociarAgendaFacturaDialog();
            }
        }
        else
            if (data.buttonId === "btnValidarFacturas") {
                if (!data.parameters.find(p => p.id === "Lockeada")) {
                    setkeyAgenda(
                        [
                            { id: "idAgenda", value: data.row.cells.find(w => w.column == "NU_AGENDA").value }
                        ]
                    );
                    openFormValidarFacturaDialog();
                }
            }
            else if (data.buttonId === "btnLiberarRecepcion"
                || data.buttonId === "btnCerrarAgenda"
                || data.buttonId === "btnCancelarAgenda"
                || data.buttonId === "btnDesacerEmbarque"
                || data.buttonId === "btnFinalizarCrossDocking") {

                nexus.getGrid("REC170_grid_1").refresh();

            } else if (data.buttonId === "btnEnviarTracking") {

                if (data.parameters !== undefined && data.parameters.find(f => f.id === "openTrackingDialog") !== undefined) {
                    var open = data.parameters.find(d => d.id === "openTrackingDialog").value;
                    if (open === "S") {

                        setkeyAgenda(
                            [
                                { id: "idAgenda", value: data.row.cells.find(w => w.column == "NU_AGENDA").value }
                            ]
                        );
                        openFormTrackingDialog();
                    } else {
                        nexus.getGrid("REC170_grid_1").refresh();

                    }
                }
            }
    };


    const openFormTrackingDialog = () => {
        setShowTrackingModal(true);
        setShowPopUpTracking(true);
        setShowModal(false);
    }

    const closeFormDialogTracking = () => {
        setShowTrackingModal(false);
        setShowPopUpTracking(false);
    }

    const onAfterInitialize = (context, grid, parameters, nexus) => {
        setNexus(nexus);
    };

    return (
        <Page
            onAfterLoad={PageOnAfterLoad}
            title={t("REC170_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button id="AgregarReferencia" className="btn btn-primary" onClick={openFormDialog}>{t("REC170_Sec0_btn_AgregarLinea")}</button>
            </div>

            <Grid
                id="REC170_grid_1"
                rowsToFetch={20}
                rowsToDisplay={15}
                enableExcelExport
                onAfterInitialize={onAfterInitialize}
                onBeforeButtonAction={GridOnBeforeButtonAction}
                onAfterButtonAction={GridOnAfterButtonAction}
            />

            <Modal show={showModal} onHide={closeFormDialog} dialogClassName="modal-90w" backdrop="static" >

                <REC170CreateAgendaModal show={showPopupAdd} onHide={closeFormDialog} />
                <REC170UpdateAgendaModal show={showPopupUpdate} onHide={closeFormUpdateDialog} agenda={keyAgenda} />
                <REC170LineasAgendaModal show={showPopupLineas} onHide={closeFormLineasDialog} agenda={keyAgenda} />
                <REC170AsociarAgendaFacturaModal show={showAsociarAgendaFacturaPopup} onHide={closeAsociarAgendaFacturaDialog} agenda={keyAgenda} />
                <REC170ValidarFacturaModal show={showValidarFacturaPopup} onHide={closeValidarFacturaDialog} agenda={keyAgenda} />
                <REC170SeleccionLpnModal show={showPopupLpn} onHide={closeFormLpnDialog} agenda={keyAgenda} />
            </Modal>

            <Modal show={showTrackingModal} onHide={closeFormDialogTracking} backdrop="static" >
                <REC170SincronizarTrackingModal show={showPopUpTracking} onHide={closeFormDialogTracking} agenda={keyAgenda} />
            </Modal>

            <IMP080ImpresionEtiquetasRecModal show={showModalImpresion} onHide={closeImpresionDialog} agenda={keyAgenda} />
            <REC170PlanificacionLpnModal show={showPlanificacionLpn} onHide={closePlanificacionLpnDialog} agenda={keyAgenda} />
            <REC170RecepcionLpnModal show={showModalRecepcionLpn} onHide={closeRecepcionLpnDialog} agenda={keyAgenda} />
        </Page>
    );
}
