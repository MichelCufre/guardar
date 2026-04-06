import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Modal } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { REC400CrearEstacion } from './REC400CrearEstacion';

export default function REC400(props) {

    const { t } = useTranslation();

    const [showModal, setShowModal] = useState(false);

    const [showPopupCreate, setShowPopupCreate] = useState(false);

    const [infoTemplate, setInfoTemplate] = useState(null);

    const openFormDialog = () => {
        setShowPopupCreate(true);
        setShowModal(true);
    }

    const closeFormDialog = (nexus) => {

        setShowPopupCreate(false);
        setShowModal(false);

        if (nexus)
            nexus.getGrid("REC400_grid_1").refresh();

        setInfoTemplate(
            null
        );
    }

    const showFormCreate = () => { return (<REC400CrearEstacion show={showFormCreate} onHide={closeFormDialog} template={infoTemplate} />); }

    return (
        <Page
            title={t("REC400_Sec0_pageTitle_Titulo")}
            application="REC400"
            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button className="btn btn-primary" onClick={openFormDialog}>{t("REC400_Sec0_btn_NuevaEstacion")}</button>
            </div>

            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="REC400_grid_1"
                        application="REC400"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport={true}
                    />
                </div>
            </div>

            <Modal id="modal" show={showModal} onHide={closeFormDialog} dialogClassName={"modal-50w"} backdrop="static" >
                {showPopupCreate ? showFormCreate() : null}
            </Modal>
        </Page>
    );
}