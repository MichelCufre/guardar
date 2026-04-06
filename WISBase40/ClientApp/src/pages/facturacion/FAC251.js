import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Modal, Row, Col, Container } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { FAC251Modal } from './FAC251Modal';

export default function FAC251(props) {

    const { t } = useTranslation();

    const [showModal, setShowModal] = useState(false);

    const [showPopupCreate, setShowPopupCreate] = useState(false);

    const [infoTemplate, setinfoTemplate] = useState(null);

    const openFormDialog = () => {
        setShowPopupCreate(true);
        setShowModal(true);
    }

    const closeFormDialog = (nexus) => {

        setShowPopupCreate(false);
        setShowModal(false);

        if (nexus)
            nexus.getGrid("FAC251_grid_1").refresh();

        setinfoTemplate(
            null
        );
    }

    const GridOnBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnEditar") {

            context.abortServerCall = true;

            setinfoTemplate(
                [
                    { id: "codigoProceso", value: data.row.cells.find(w => w.column == "CD_PROCESO").value },
                ]
            );

            openFormDialog();
        };
    }

    const showFormCreate = () => { return (<FAC251Modal show={showFormCreate} onHide={closeFormDialog} template={infoTemplate} />); }

    return (
        <Page
            title={t("FAC251_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button className="btn btn-primary" onClick={openFormDialog}>{t("FAC251_Sec0_btn_NuevoProcesoFacturacion")}</button>
            </div>

            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="FAC251_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport={true} 
                        onBeforeButtonAction={GridOnBeforeButtonAction}
                    />
                </div>
            </div>

            <Modal id="modal" show={showModal} onHide={closeFormDialog} dialogClassName={"modal-50w"} backdrop="static" >
                {showPopupCreate ? showFormCreate() : null}
            </Modal>
        </Page>
    );
}