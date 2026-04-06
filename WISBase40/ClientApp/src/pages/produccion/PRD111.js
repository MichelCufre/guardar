
import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { PRD111CrearEspacioModal } from './PRD111CrearEspacioModal';
import { PRD111ModificarEspacioModal } from './PRD111ModificarEspacioModal';
import PRD111ConsumirStock from './PRD111ConsumirStock';
import PRD111ProducirStock from './PRD111ProducirStock';
import { PRD111ExpulsarMercaderiaModal } from './PRD111ExpulsarMercaderiaModal';

export default function PRD111(props) {
    const { t } = useTranslation();

    const [showPopup, setShowPopup] = useState(false);
    const [showPopupModificar, setShowPopupModificar] = useState(false);
    const [showPopupConsumirStock, setShowPopupConsumirStock] = useState(false);
    const [showPopupProducirStock, setShowPopupProducirStock] = useState(false);
    const [showPopupExpulsar, setShowPopupExpulsar] = useState(false);
    const [codigoEspacio, setCodigoEspacio] = useState(null);

    const openFormDialog = () => {
        setShowPopup(true);

        setShowPopupModificar(false);
        setShowPopupConsumirStock(false);
        setShowPopupProducirStock(false);
        setShowPopupExpulsar(false);
    };

    const closeFormDialog = () => {
        setShowPopup(false);
    };

    const openFormDialogModificar = () => {
        setShowPopupModificar(true);

        setShowPopup(false);
        setShowPopupConsumirStock(false);
        setShowPopupProducirStock(false);
        setShowPopupExpulsar(false);
    };

    const closeFormDialogModificar = () => {
        setShowPopupModificar(false);
    };

    const openFormDialogConsumirStock = () => {

        setShowPopupConsumirStock(true);

        setShowPopup(false);
        setShowPopupModificar(false);
        setShowPopupProducirStock(false);
        setShowPopupExpulsar(false);
    };

    const closeFormDialogConsumirStock = () => {
        setShowPopupConsumirStock(false);
    };

    const openFormDialogProducirStock = () => {
        setShowPopupProducirStock(true);

        setShowPopup(false);
        setShowPopupModificar(false);
        setShowPopupConsumirStock(false);
        setShowPopupExpulsar(false);
    };
    const closeFormDialogProducirStock = () => {
        setShowPopupProducirStock(false);
    };

    const openFormDialogExpulsar = () => {
        setShowPopupExpulsar(true);

        setShowPopup(false);
        setShowPopupModificar(false);
        setShowPopupConsumirStock(false);
        setShowPopupProducirStock(false);
    };

    const closeFormDialogExpulsar = () => {
        setShowPopupExpulsar(false);
    };

    const handleGridOnBeforeButtonAction = (context, data, nexus) => {

        context.abortServerCall = true;
        setCodigoEspacio(data.row.cells.find(d => d.column === "CD_PRDC_LINEA").value);

        if (data.buttonId === "btnEditar") {
            openFormDialogModificar();
        }
        else if (data.buttonId === "btnConsumirStock") {
            openFormDialogConsumirStock();
        }
        else if (data.buttonId === "btnProducirStock") {
            openFormDialogProducirStock();
        }
        if (data.buttonId === "btnExpulsarStock") {
            openFormDialogExpulsar();
        }
    }

    return (
        <Page
            icon="fas fa-file"
            application="PRD111"
            title={t("PRD111_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button className="btn btn-primary" onClick={openFormDialog}>{t("PRD111_Sec0_btn_AgregarLinea")}</button>
            </div>

            <Grid
                id="PRD111_grid_1"
                application="PRD111"
                rowsToFetch={30}
                rowsToDisplay={15}
                enableExcelExport
                onBeforeButtonAction={handleGridOnBeforeButtonAction}
            />

            <PRD111CrearEspacioModal show={showPopup} onHide={closeFormDialog} />
            <PRD111ModificarEspacioModal show={showPopupModificar} onHide={closeFormDialogModificar} codigoEspacio={codigoEspacio} />
            <PRD111ConsumirStock show={showPopupConsumirStock} onHide={closeFormDialogConsumirStock} codigoEspacio={codigoEspacio} />
            <PRD111ProducirStock show={showPopupProducirStock} onHide={closeFormDialogProducirStock} codigoEspacio={codigoEspacio} />
            <PRD111ExpulsarMercaderiaModal show={showPopupExpulsar} onHide={closeFormDialogExpulsar} idEspacioProduccion={codigoEspacio}/>
        </Page>
    );
}
