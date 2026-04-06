import React, { useState } from 'react';
import { Modal } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { IMP080ImpresionEtiquetasRecModal } from '../impresion/IMP080ImpresionEtiquetasRecModal';
import { REC500CreateFacturaModal } from './REC500CreateFacturaModal';
import { REC500LineasFacturaModal } from './REC500LineasFacturaModal';
import { REC500UpdateFacturaModal } from './REC500UpdateFacturaModal';

export default function REC500(props) {

    const { t } = useTranslation();

    const [showModal, setshowModal] = useState(false);
    const [showPopupAdd, setShowPopupAdd] = useState(false);
    const [showBotonCreate, setShowBotonCreate] = useState(false);
    const [showPopupLineas, setShowPopupLineas] = useState(false);
    const [showPopupUpdate, setShowPopupUpdate] = useState(false);
    const [keyFactura, setkeyFactura] = useState(null);
    const [showModalImpresion, setShowModalImpresion] = useState(false);

    const PageOnAfterLoad = (data) => {
        let deshabilitaCreacion = data.parameters.find(p => p.id === "deshabilitaCreacion");
        if (deshabilitaCreacion) {
            if (deshabilitaCreacion.value == "true") {
                setShowBotonCreate(true);
            }
        }
    }

    const openFormDialog = () => {
        setShowPopupAdd(true);
        setshowModal(true);

        setShowPopupLineas(false);
        setShowPopupUpdate(false);
        setShowModalImpresion(false);
    }

    const openFormLineasDialog = () => {
        setShowPopupLineas(true);
        setshowModal(true);

        setShowPopupAdd(false);
        setShowPopupUpdate(false);
        setShowModalImpresion(false);
    }

    const openFormUpdateDialog = () => {
        setShowPopupUpdate(true);
        setshowModal(true);

        setShowPopupAdd(false);
        setShowPopupLineas(false);
        setShowModalImpresion(false);
    }

    const closeFormDialog = (factura, siguiente) => {
        setShowPopupAdd(false);
        if (siguiente) {
            setkeyFactura(
                [
                    { id: "idFactura", value: factura }
                ]
            );
            if (siguiente === "detalles") {
                openFormLineasDialog();
            }
        } else {
            setshowModal(false);
        }
    }

    const closeFormLineasDialog = () => {
        setShowPopupLineas(false);
        setshowModal(false);
    }

    const closeFormUpdateDialog = (factura, siguiente) => {
        setShowPopupUpdate(false);
        setshowModal(false);

        if (siguiente === "detalles") {
            openFormLineasDialog();
        }

    }

    const openImpresionDialog = () => {
        setShowModalImpresion(true);

        setShowPopupAdd(false);
        setShowPopupLineas(false);
        setShowPopupUpdate(false);
    }

    const closeImpresionDialog = () => {
        setShowModalImpresion(false);
    }

    const GridOnBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnEditar") {
            context.abortServerCall = true;
            setkeyFactura(
                [
                    { id: "idFactura", value: data.row.cells.find(w => w.column == "NU_RECEPCION_FACTURA").value }
                ]
            );
            openFormUpdateDialog();
        }
        else if (data.buttonId === "btnLineas") {
            data.parameters.push({ id: "idFactura", value: data.row.cells.find(w => w.column == "NU_RECEPCION_FACTURA").value });
        }
        else if (data.buttonId === "btnImprimir") {
            context.abortServerCall = true;
            setkeyFactura(
                [
                    { id: "idFactura", value: data.row.cells.find(w => w.column == "NU_RECEPCION_FACTURA").value }
                ]
            );
            openImpresionDialog();
        }

    };

    const GridOnAfterButtonAction = (data, nexus) => {
        if (data.buttonId === "btnLineas") {
            if (!data.parameters.find(p => p.id === "Lockeada")) {
                setkeyFactura(
                    [
                        { id: "idFactura", value: data.row.cells.find(w => w.column == "NU_RECEPCION_FACTURA").value }
                    ]
                );
                openFormLineasDialog();
            }
        }
        if (data.buttonId === "btnCancelarFactura" || data.buttonId === "btnDesvincularDeAgenda") {

            nexus.getGrid("REC500_grid_1").refresh();
        }

    };

    return (
        <Page
            onAfterLoad={PageOnAfterLoad}
            title={t("REC500_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button id="AgregarFactura" className="btn btn-primary" onClick={openFormDialog}>{t("REC500_Sec0_btn_AgregarFactura")}</button>
            </div>
            <Grid
                id="REC500_grid_1"
                application="REC500"
                rowsToFetch={20}
                rowsToDisplay={15}
                enableExcelExport
                onBeforeButtonAction={GridOnBeforeButtonAction}
                onAfterButtonAction={GridOnAfterButtonAction}
            />

            <Modal show={showModal} onHide={closeFormDialog} dialogClassName="modal-90w" backdrop="static" >
                <REC500CreateFacturaModal show={showPopupAdd} onHide={closeFormDialog} />
                <REC500LineasFacturaModal show={showPopupLineas} onHide={closeFormLineasDialog} factura={keyFactura} />
                <REC500UpdateFacturaModal show={showPopupUpdate} onHide={closeFormUpdateDialog} factura={keyFactura} />
            </Modal>
            <IMP080ImpresionEtiquetasRecModal show={showModalImpresion} onHide={closeImpresionDialog} factura={keyFactura} />
        </Page>
    );
}