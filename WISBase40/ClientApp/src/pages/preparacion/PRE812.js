import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { PRE812InstanciaModal } from './PRE812InstanciaModal';
import { PRE812ZoominModal } from './PRE812ZoominModal';

export default function PRE812(props) {
    const { t } = useTranslation();

    const [showPopupUpdate, setShowPopupUpdate] = useState(false);
    const [showPopupZoomin, setShowPopupZoomin] = useState(false);
    const [keyInstanciaUpd, setKeyInstancia] = useState(null);

    const openFormUpdateDialog = () => {
        setShowPopupUpdate(true);
    }

    const closeFormUpdateDialog = () => {
        setShowPopupUpdate(false);
    }

    const openFormZoominDialog = () => {
        setShowPopupZoomin(true);
    }

    const closeFormZoominDialog = () => {
        setShowPopupZoomin(false);
    }

    const onAfterInitialize = (context, data, nexus) => {
        /*const redirectWidget = nexus.find(w => w.id === "redirect");
        if (redirectWidget && redirectWidget.value) {            
            window.location = "/";
        }*/
    }

    const GridOnBeforeButtonAction = (context, data, nexus) => {

        if (data.buttonId === "btnEditar") {
            context.abortServerCall = true;

            setKeyInstancia(
                [
                    { id: "cdCliente", value: data.row.cells.find(w => w.column == "CD_CLIENTE").value },
                    { id: "cdEmpresa", value: data.row.cells.find(w => w.column == "CD_EMPRESA").value },
                    { id: "nuPedido", value: data.row.cells.find(w => w.column == "NU_PEDIDO").value },
                    { id: "puntuacion", value: data.row.cells.find(w => w.column == "NU_PUNTUACION").value }
                ]
            );

            openFormUpdateDialog();
        }
        else if (data.buttonId === "btnDetalle") {
            context.abortServerCall = true;

            setKeyInstancia(
                [
                    { id: "cdCliente", value: data.row.cells.find(w => w.column == "CD_CLIENTE").value },
                    { id: "cdEmpresa", value: data.row.cells.find(w => w.column == "CD_EMPRESA").value },
                    { id: "nuPedido", value: data.row.cells.find(w => w.column == "NU_PEDIDO").value }
                ]
            );
            openFormZoominDialog();

        }
    }

    return (
        <Page
            title={t("PRE812_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div className="row mb-4">
                <div className="col">
                    <Grid
                        id="PRE812_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        onAfterInitialize={onAfterInitialize}
                        onBeforeButtonAction={GridOnBeforeButtonAction}
                        enableExcelExport
                    />
                </div>
            </div>
            <PRE812InstanciaModal show={showPopupUpdate} onHide={closeFormUpdateDialog} datosKey={keyInstanciaUpd} />
            <PRE812ZoominModal show={showPopupZoomin} onHide={closeFormZoominDialog} datosKey={keyInstanciaUpd} />
        </Page>
    );
}