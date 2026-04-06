import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Modal, Row, Col, Container } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { COF060Modal } from './COF060Modal';


export default function COF060(props) {

    const { t } = useTranslation();

    const [showModal, setShowModal] = useState(false);

    const [showPopupCreate, setShowPopupCreate] = useState(false);

    const [infoServidor, setInfoServidor] = useState(null);


    const openFormDialog = () => {
        setShowPopupCreate(true);
        setShowModal(true);
    }

    const closeFormDialog = (nexus) => {

        setShowPopupCreate(false);
        setShowModal(false);

        if (nexus)
            nexus.getGrid("COF060_grid_1").refresh();

        setInfoServidor(
            null
        );
    }

    const GridOnBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnEditar") {

            context.abortServerCall = true;

            setInfoServidor(
                [
                    { id: "codigoServidor", value: data.row.cells.find(w => w.column == "CD_SERVIDOR").value },
                ]
            );

            openFormDialog();
        };
    }

    const gridOnAfterButtonAction = (data, nexus) => {
        if (data.buttonId === "btnEliminarServidor") {
            nexus.getGrid("COF060_grid_1").refresh();
        };
    }

    const showFormCreate = () => { return (<COF060Modal show={showFormCreate} onHide={closeFormDialog} servidor={infoServidor} />); }

    return (

        <Page
            title={t("COF060_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button className="btn btn-primary" onClick={openFormDialog}>{t("COF060_Sec0_btn_NuevoServidor")}</button>
            </div>
            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="COF060_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                        onBeforeButtonAction={GridOnBeforeButtonAction}
                        onAfterButtonAction={gridOnAfterButtonAction}
                    />
                </div>
            </div>

            <Modal id="modal" show={showModal} onHide={closeFormDialog} dialogClassName={"modal-50w"} backdrop="static" >
                {showPopupCreate ? showFormCreate() : null}
            </Modal>
        </Page>
    );
}