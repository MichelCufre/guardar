import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Modal, Row, Col, Container } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { SEG030CreateModal } from './SEG030CreateModal';
import { SEG030RecursosModal } from './SEG030RecursosModal';
import { SEG030AsignarInfoModal } from './SEG030AsignarInfoModal';
import { IMP050ImpresionesUsuarioModal } from '../impresion/IMP050ImpresionUsuariosModal';


export default function SEG030(props) {

    const { t } = useTranslation();

    const [showModal, setShowModal] = useState(false);

    const [showPopupCreate, setShowPopupCreate] = useState(false);

    const [showPopupAsignacion, setShowPopupAsignacion] = useState(false);

    const [showPopupAsignacionInfo, setShowPopupAsignacionInfo] = useState(false);

    const [showPopupImprimir, setShowPopupImprimir] = useState(false);

    const [infoUsuario, setInfoUsuario] = useState(null);

    const [actionEditar, setActionEditar] = useState(null);

    const [botonInfo, setBotonInfo] = useState(null);

    const [sizeModal, setSizeModal] = useState("modal-50w");

    const [rowSeleccionadasImprimir, setRowSeleccionadasImprimir] = useState(null);

    const [permiteCrear, setPermiteCrear] = useState(false);

    const openFormDialog = () => {
        setSizeModal("modal-50w");
        setShowPopupCreate(true);
        setShowModal(true);
    }

    const openModalAsignacion = () => {
        setSizeModal("modal-90w");
        setShowPopupAsignacion(true);
        setShowModal(true);

    }

    const closeModalAsignacion = (nexus) => {
        setShowPopupAsignacion(false);
        setShowModal(false);
    }

    const openModalAsignacionInfo = () => {
        setSizeModal("modal-90w");
        setShowPopupAsignacionInfo(true);
        setShowModal(true);
    }

    const closeModalAsignacionInfo = (nexus) => {

        setShowPopupAsignacionInfo(false);
        setShowModal(false);
    }

    const openImprimirDialog = () => {
        setShowPopupImprimir(true);
    }

    const closeImprimirDialog = () => {
        setShowPopupImprimir(false);
    }


    const closeFormDialog = (userId, asignacion, nexus) => {

        setShowPopupCreate(false);

        if (userId && !asignacion) {

            setInfoUsuario(userId)
            openModalAsignacion();

        } else if (userId && asignacion) {

            setInfoUsuario(userId);

            setBotonInfo([
                { id: "btnConfiPredios", vale: "true" }
            ]);

            openModalAsignacionInfo();

        } else {
            setInfoUsuario(null);
            setShowPopupCreate(false);
            closeModalAsignacion(nexus);
            closeModalAsignacionInfo(nexus);
        }

        if (nexus)
            nexus.getGrid("SEG030_grid_1").refresh();
    }

    const GridOnBeforeButtonAction = (context, data, nexus) => {
        setInfoUsuario(
            [{ id: "idUsuario", value: data.row.cells.find(w => w.column == "USERID").value }]
        );

        context.abortServerCall = true;

        if (data.buttonId === "btnEditar") {

            setActionEditar([
                { id: "btnEditar", vale: "true" }
            ]);

            openFormDialog();

        } else if (data.buttonId === "btnAsignarPerfiles") {

            openModalAsignacion();

        } else if (data.buttonId === "btnConfiEmpre") {

            setBotonInfo([
                { id: "btnConfiEmpre", vale: "true" }
            ]);

            openModalAsignacionInfo();
        } else if (data.buttonId === "btnConfiPredios") {
            setBotonInfo([
                { id: "btnConfiPredios", vale: "true" }
            ]);
            openModalAsignacionInfo();
        }
    };

    const handleAfterButtonAction = (context, data, nexus) => {

        if (nexus)
            nexus.getGrid("SEG030_grid_1").refresh();

        data.getGrid("SEG030_grid_1").refresh();

    };

    const GridOnAfterMenuItemAction = (context, data, nexus) => {

        let jsonAdded = data.parameters.find(w => w.id === "ListaFilasSeleccionadas").value;

        setRowSeleccionadasImprimir(jsonAdded);

        openImprimirDialog();
    }

    const showFormCreate = () => { return (<SEG030CreateModal show={showFormCreate} onHide={closeFormDialog} usuario={infoUsuario} accionEditar={actionEditar} />); }
    const showFormAsignacion = () => { return (<SEG030RecursosModal show={showFormAsignacion} onHide={closeFormDialog} usuario={infoUsuario} />); }
    const showPageAsignacionInfo = () => { return (<SEG030AsignarInfoModal show={showFormAsignacion} onHide={closeFormDialog} usuario={infoUsuario} boton={botonInfo} />); }

    return (

        <Page
            title={t("SEG030_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="SEG030_grid_1" rowsToFetch={30} rowsToDisplay={15}
                        enableExcelExport
                        enableSelection
                        onBeforeButtonAction={GridOnBeforeButtonAction}
                        onAfterButtonAction={handleAfterButtonAction}
                        onAfterMenuItemAction={GridOnAfterMenuItemAction}
                    />
                </div>
            </div>

            <Modal id="modal" show={showModal} onHide={closeFormDialog} dialogClassName={`${sizeModal}`} backdrop="static" >
                {showPopupCreate ? showFormCreate() : null}
                {showPopupAsignacion ? showFormAsignacion() : null}
                {showPopupAsignacionInfo ? showPageAsignacionInfo() : null}
            </Modal>

            <IMP050ImpresionesUsuarioModal show={showPopupImprimir} onHide={closeImprimirDialog} rowSeleccionadas={rowSeleccionadasImprimir} />
        </Page>
    );
}