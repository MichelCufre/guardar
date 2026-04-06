import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Modal, Row, Col, Container } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { SEG020CreateModal } from './SEG020CreateModal';
import { SEG020AsignaModal } from './SEG020AsignaModal';


export default function SEG020(props) {

    const { t } = useTranslation();

    const [showModal, setShowModal] = useState(false);

    const [showPopupCreate, setShowPopupCreate] = useState(false);

    const [showPopupAsignacion, setShowPopupAsignacion] = useState(false);

    const [infoUsuario, setInfoUsuario] = useState(null);

    const [sizeModal, setSizeModal] = useState({
        SIZE: ""
    })

    const resizeModal = (tam) => {

        if (tam == "create") {
            setSizeModal({
                SIZE: "modal-50w"
            });

        } else if (tam == "asignacion") {

            setSizeModal({
                SIZE: "modal-90w"
            });
        }
    }

    const openFormDialog = () => {

        setShowPopupCreate(true);
        setShowModal(true);

        resizeModal("create");
    }

    const openModalAsignacion = () => {

        setShowPopupAsignacion(true);
        setShowModal(true);

        resizeModal("asignacion");
    }

    const closeModalAsignacion = (nexus) => {
        setShowPopupAsignacion(false);
        setShowModal(false);

    }

    const closeFormDialog = (userId, atras, nexus) => {

        setShowPopupCreate(false);

        if (atras) {

            setShowPopupAsignacion(false);

            openFormDialog();

        } else if (userId) {

            setInfoUsuario(userId)
            openModalAsignacion();

        } else {
            setInfoUsuario(null);
            setShowPopupCreate(false);
            closeModalAsignacion(nexus);
        }

        if(nexus)
            nexus.getGrid("SEG020_grid_1").refresh();
    }

    const GridOnBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnEditar") {

            context.abortServerCall = true;

            setInfoUsuario(
                [
                    { id: "idPerfil", value: data.row.cells.find(w => w.column == "PROFILEID").value }
                ]
            );

            openFormDialog();

        } else if (data.buttonId === "btnAsignarPerfiles") {
            context.abortServerCall = true;

            setInfoUsuario(
                [
                    { id: "idPerfil", value: data.row.cells.find(w => w.column == "PROFILEID").value }
                ]
            );

            openModalAsignacion();
        }

        

    };

    const showFormCreate = () => { return (<SEG020CreateModal show={showFormCreate} onHide={closeFormDialog} usuario={infoUsuario} />); }
    const showFormAsignacion = () => { return (<SEG020AsignaModal show={showFormAsignacion} onHide={closeFormDialog} usuario={infoUsuario} />); }

    return (

        <Page
            title={t("SEG020_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button className="btn btn-primary" onClick={openFormDialog}>{t("SEG020_Sec0_btn_NuevoPerfil")}</button>
            </div>
            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="SEG020_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                        onBeforeButtonAction={GridOnBeforeButtonAction}
                    />
                </div>
            </div>

            <Modal id="modal" show={showModal} onHide={closeFormDialog} dialogClassName={`${sizeModal.SIZE}`} backdrop="static" >
                {showPopupCreate ? showFormCreate() : null}
                {showPopupAsignacion ? showFormAsignacion() : null}
            </Modal>
        </Page>
    );
}