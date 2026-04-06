import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { Row, Col, Container } from 'react-bootstrap';
import { IMP110ImpresionGralContenedoresModal } from '../impresion/IMP110ImpresionGralContenedoresModal';
import { IMP110ImpresionYGenerarContenedoresModal } from '../impresion/IMP110ImpresionYGenerarContenedoresModal';
import { IMP200ImpresionUTModal } from '../impresion/IMP200ImpresionUTModal'

export default function PRE060(props) {

    const { t } = useTranslation();

    const [showPopupImprimir, setShowPopupImprimir] = useState(false);
    const [showPopupReimprimir, setShowPopupReimprimir] = useState(false);
    const [showPopupImprimirUT, setShowPopupImprimirUT] = useState(false);
    const [keyContenedor, setKeyContenedor] = useState(null);

    const openImprimirDialog = () => {
        setShowPopupImprimir(true);
        setShowPopupReimprimir(false);
        setShowPopupImprimirUT(false);
    }

    const closeImprimirDialog = () => {
        setShowPopupImprimir(false);
        setShowPopupReimprimir(false);
        setShowPopupImprimirUT(false);
    }

    const openReimprimirModal = () => {
        setShowPopupImprimir(false);
        setShowPopupReimprimir(true);
        setShowPopupImprimirUT(false);
    }

    const closeReimprimirModal = () => {
        setShowPopupImprimir(false);
        setShowPopupReimprimir(false);
        setShowPopupImprimirUT(false);
    }

    const openImprimirDialogUT = () => {
        setShowPopupImprimir(false);
        setShowPopupReimprimir(false);
        setShowPopupImprimirUT(true);
    }

    const closeImprimirDialogUT = () => {
        setShowPopupImprimir(false);
        setShowPopupReimprimir(false);
        setShowPopupImprimirUT(false);
    }

    const onBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnImprimir") {
            context.abortServerCall = true;

            setKeyContenedor([
                { id: "contenedor", value: data.row.cells.find(d => d.column === "NU_CONTENEDOR").value },
                { id: "preparacion", value: data.row.cells.find(d => d.column === "NU_PREPARACION").value }
            ]);

            openImprimirDialog();
        }
    }

    return (

        <Page
            title={t("PRE060_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button className="btn btn-primary" onClick={openReimprimirModal}>{t("PRE060_Sec0_btn_GenerarEtiquetas")}</button>
                &nbsp;
                <button className="btn btn-primary" onClick={openImprimirDialogUT}>{t("PRE060_Sec0_btn_GenerarEtiquetaUT")}</button>
            </div>
            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="PRE060_grid_1" rowsToFetch={30} rowsToDisplay={15}
                        enableExcelExport
                        onBeforeButtonAction={onBeforeButtonAction}
                    />
                </div>
            </div>

            <IMP110ImpresionGralContenedoresModal show={showPopupImprimir} onHide={closeImprimirDialog} contenedor={keyContenedor} />
            <IMP110ImpresionYGenerarContenedoresModal show={showPopupReimprimir} onHide={closeReimprimirModal} />
            <IMP200ImpresionUTModal show={showPopupImprimirUT} onHide={closeImprimirDialogUT} selectedKeys={null} reimprimir={false} />

        </Page>
    );
}