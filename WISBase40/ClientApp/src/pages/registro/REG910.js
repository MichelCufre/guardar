import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Modal, Row, Col, Container } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { REG910DetallesModal } from './REG910DetallesModal';

export default function REG910(props) {

const [codigoDominio, setCodigoDominio] = useState(null);
const [dominioInterno, setDominioInterno] = useState(null);
const [showPopupDetalleDominio, setShowPopupDetalleDominio] = useState(false);
const [showModal, setShowModal] = useState(false);

const openFormDetalleDominio = () => {
    setShowPopupDetalleDominio(true);
    setShowModal(true);
}

const closeFormDialog = (nexus) => {
    setShowPopupDetalleDominio(false);
    setShowModal(false);
};

    const GridOnBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnDetalles") {
            context.abortServerCall = true;
            setCodigoDominio(data.row.cells.find(w => w.column == "CD_DOMINIO").value);
            setDominioInterno(data.row.cells.find(w => w.column == "FL_INTERNO_WIS").value);
            openFormDetalleDominio();
        }
    };

    const showFormDetalleDominio = () => {
        return (<REG910DetallesModal show={showPopupDetalleDominio} onHide={closeFormDialog} codigoDominio={codigoDominio} dominioInterno={dominioInterno}/>);
    }

    const { t } = useTranslation();

    return (
        <Page
            title={t("WREG910_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="REG910_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport={true}
                        onBeforeButtonAction={GridOnBeforeButtonAction}
                    />
                </div>
            </div>

            <Modal show={showModal} onHide={closeFormDialog} dialogClassName="modal-90w" backdrop="static" >
                {showPopupDetalleDominio ? showFormDetalleDominio() : null}
            </Modal>
        </Page>
    );
}