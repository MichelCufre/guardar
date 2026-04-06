import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Modal, Row, Col, Container } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { COF030Modal } from './COF030Modal';


export default function COF030(props) {

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
            nexus.getGrid("COF030_grid_1").refresh();

        setinfoTemplate(
            null
        );
    }

    const GridOnBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnEditar") {

            context.abortServerCall = true;

            setinfoTemplate(
                [
                    { id: "idTemplate", value: data.row.cells.find(w => w.column == "CD_LABEL_ESTILO").value },
                    { id: "lenguaje", value: data.row.cells.find(w => w.column == "CD_LENGUAJE_IMPRESION").value },
                ]
            );

            openFormDialog();
        };
    }

    const gridOnAfterButtonAction = (data, nexus) => {
        if (data.buttonId === "btnEliminarTemplate") {
            nexus.getGrid("COF030_grid_1").refresh();
        };
    }

    const showFormCreate = () => { return (<COF030Modal show={showFormCreate} onHide={closeFormDialog} template={infoTemplate} />); }

    return (

        <Page
            title={t("COF030_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button className="btn btn-primary" onClick={openFormDialog}>{t("COF030_Sec0_btn_NuevoTemplate")}</button>
            </div>
            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="COF030_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport
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