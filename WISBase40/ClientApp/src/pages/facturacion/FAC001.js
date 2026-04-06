import React, { useState } from 'react';
import { Modal } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { FAC001Modal } from './FAC001Modal';

export default function FAC001(props) {

    const { t } = useTranslation();

    const [showModal, setShowModal] = useState(false);

    const [showPopupCreate, setShowPopupCreate] = useState(false);

    const [infoEjecucion, setInfoEjecucion] = useState(null);

    const openFormDialog = () => {
        setShowPopupCreate(true);
        setShowModal(true);
    }

    const closeFormDialog = (nexus) => {

        setShowPopupCreate(false);
        setShowModal(false);

        setInfoEjecucion(
            null
        );

        if (nexus)
            nexus.getGrid("FAC001_grid_1").refresh();
    }

    const GridOnBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnEditar") {

            context.abortServerCall = true;

            setInfoEjecucion(
                [
                    { id: "nuEjecucion", value: data.row.cells.find(w => w.column == "NU_EJECUCION").value },
                ]
            );

            openFormDialog();
        };
    }

    const gridOnAfterButtonAction = (data, nexus) => {
        if (data.buttonId === "btnAnularEjecucion"
            || data.buttonId === "btnHabilitarEjecucion"
            || data.buttonId === "btnHabilitarFacturacion") {
            nexus.getGrid("FAC001_grid_1").refresh();
        };
    }

    const showFormCreate = () => { return (<FAC001Modal show={showFormCreate} onHide={closeFormDialog} ejecucion={infoEjecucion} />); }

    return (
        <Page
            title={t("FAC001_Sec0_pageTitle_Titulo")}
            {...props}
        >

            <div style={{ textAlign: "center" }}>
                <button className="btn btn-primary" onClick={openFormDialog}>{t("FAC001_Sec0_btn_NuevaEjecucion")}</button>
            </div>

            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="FAC001_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport enableExcelImport={false}
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