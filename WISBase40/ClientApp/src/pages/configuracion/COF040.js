import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Modal, Row, Col, Container } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { COF040Modal } from './COF040Modal';


export default function COF040(props) {

    const { t } = useTranslation();

    const [showModal, setShowModal] = useState(false);

    const [showPopupCreate, setShowPopupCreate] = useState(false);

    const [infoImpresora, setInfoImpresora] = useState(null);


    const openFormDialog = () => {
        setShowPopupCreate(true);
        setShowModal(true);
    }

    const closeFormDialog = (nexus) => {

        setShowPopupCreate(false);
        setShowModal(false);

        if (nexus)
            nexus.getGrid("COF040_grid_1").refresh();

        setInfoImpresora(
            null
        );
    }

    const GridOnBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnEditar") {

            context.abortServerCall = true;

            setInfoImpresora(
                [
                    { id: "idImpresora", value: data.row.cells.find(w => w.column == "CD_IMPRESORA").value },
                    { id: "predio", value: data.row.cells.find(w => w.column == "NU_PREDIO").value }
                ]
            );

            openFormDialog();
        };
    }

    const showFormCreate = () => { return (<COF040Modal show={showFormCreate} onHide={closeFormDialog} impresora={infoImpresora} />); }

    return (

        <Page
            title={t("COF040_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button className="btn btn-primary" onClick={openFormDialog}>{t("COF040_Sec0_btn_NuevaImpresora")}</button>
            </div>
            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="COF040_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport
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