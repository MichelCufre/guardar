import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Modal } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { REC275CrearEstrategiaModal } from './REC275CrearEstrategiaModal';
import { REC275AsociarEstrategiaModal } from './REC275AsociarEstrategiaModal';
import { REC275ModificarEstrategiaModal } from './REC275ModificarEstrategiaModal';

export default function REC275(props) {
    const { t } = useTranslation();

    const [showModal, setShowModal] = useState(false);

    const [showPopupCrearEstrategia, setShowPopupCrearEstrategia] = useState(false);

    const [showPopupAsociarEstrategia, setShowPopupAsociarEstrategia] = useState(false);

    const [showPopupModificarEstrategia, setShowPopupModificarEstrategia] = useState(false);

    const [codigoEstrategia, setCodigoEstrategia] = useState("");

    const openFormDialog = () => {
        setShowPopupCrearEstrategia(true);
        setShowModal(true);
    }

    const openFormEditarEstrategiaDialog = () => {
        setShowPopupModificarEstrategia(true);
        setShowModal(true);
    }

    const openFormAsociarEstrategia = () => {
        setShowPopupAsociarEstrategia(true);
        setShowModal(true);
    }

    const closeFormDialog = (numeroEstrategia, nexus) => {

        setShowPopupCrearEstrategia(false);
        setShowPopupModificarEstrategia(false);
        setShowPopupAsociarEstrategia(false);

        if (numeroEstrategia) {
            setCodigoEstrategia(numeroEstrategia);
            openFormEditarEstrategiaDialog();
        } else {
            setShowModal(false);
            setCodigoEstrategia(null);

            if (nexus) {
                nexus.getGrid("REC275_grid_1").refresh();
            }
        }
    }

    const GridOnBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnEditar") {
            context.abortServerCall = true;
            setCodigoEstrategia(data.row.cells.find(w => w.column == "NU_ALM_ESTRATEGIA").value);
            openFormEditarEstrategiaDialog();
        } else if (data.buttonId === "btnAsociar") {
            context.abortServerCall = true;
            setCodigoEstrategia(data.row.cells.find(w => w.column == "NU_ALM_ESTRATEGIA").value);
            openFormAsociarEstrategia();
        }
    }

    const GridOnBeforeInitialize = (context, data, nexus) => {
        if (data.buttonId === "btnEditar") {
            context.abortServerCall = true;
            setCodigoEstrategia(data.row.cells.find(w => w.column == "NU_ALM_ESTRATEGIA").value);
            openFormDialog();
        } 
    }

    const showFormCrearEstrategia = () => { return (<REC275CrearEstrategiaModal show={showPopupCrearEstrategia} onHide={closeFormDialog} codigoEstrategia={codigoEstrategia} />); }
    const showFormAsociarEstrategiaModal = () => { return (<REC275AsociarEstrategiaModal show={showPopupAsociarEstrategia} onHide={closeFormDialog} codigoEstrategia={codigoEstrategia}  />); }
    const showFormModificarEstrategiaModal = () => { return (<REC275ModificarEstrategiaModal show={showPopupModificarEstrategia} onHide={closeFormDialog} codigoEstrategia={codigoEstrategia} />); }

    return (
        <Page
            title={t("REC275_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button className="btn btn-primary" onClick={openFormDialog}>{t("REC275_Sec0_btn_CrearNuevaEstrategia")}</button>
            </div>

            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="REC275_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport={true}
                        onBeforeButtonAction={GridOnBeforeButtonAction}
                        onBeforeInitialize={GridOnBeforeInitialize}
                    />
                </div>
            </div>

            <Modal show={showModal} onHide={closeFormDialog} dialogClassName="modal-90w" backdrop="static" >
                {showPopupCrearEstrategia ? showFormCrearEstrategia() : null}
                {showPopupModificarEstrategia ? showFormModificarEstrategiaModal() : null}
                {showPopupAsociarEstrategia ? showFormAsociarEstrategiaModal() : null}
            </Modal>
        </Page>
    );
}