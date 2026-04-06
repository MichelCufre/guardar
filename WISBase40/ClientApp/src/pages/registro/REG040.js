import React, { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { IMP050ImpresionesUbicacionModal } from '../impresion/IMP050ImpresionUbicacionesModal';
import { REG040AtributosMasivosModal } from './REG040AtributosMasivosModal';
import { REG040CreateUbicacionModal } from './REG040CreateUbicacionModal';
import { REG040PrediosModal } from './REG040PrediosModal';
import { REG040ImportExcel } from './REG040ImportExcel';


export default function REG040(props) {

    const { t } = useTranslation();

    const [showPopupAdd, setShowPopupAdd] = useState(false);
    const [showPopupPredios, setShowPopupPredios] = useState(false);
    const [showPopupAtributos, setShowPopupAtributos] = useState(false);
    const [showPopupImprimir, setShowPopupImprimir] = useState(false);
    const [rowSeleccionadasImprimir, setRowSeleccionadasImprimir] = useState("[]");
    const [importarHabilitado, setImportarHabilitado] = useState(false);

    useEffect(() => {
        if (rowSeleccionadasImprimir !== "[]") {
            openImprimirDialog();
        }
    }, [rowSeleccionadasImprimir]);


    const openAddDialog = () => {
        setShowPopupAdd(true);
    }

    const closeAddDialog = () => {
        setShowPopupAdd(false);
    }

    const openPrediosDialog = () => {
        setShowPopupPredios(true);
    }

    const closePrediosDialog = () => {
        setShowPopupPredios(false);
    }

    const openImprimirDialog = () => {
        setShowPopupImprimir(true);
    }

    const closeImprimirDialog = () => {
        setRowSeleccionadasImprimir("[]");
        setShowPopupImprimir(false);
    }

    const openAtributosDialog = () => {
        setShowPopupAtributos(true);
    }

    const closeAtributosDialog = () => {
        setShowPopupAtributos(false);
    }

    const onAfterInitialize = (context, grid, parameters, nexus) => {
        const showDetailsParam = parameters.find(d => d.id === "REG040_IMPORT_HABILITADO");

        if (showDetailsParam != null && showDetailsParam.value == "S") {
            setImportarHabilitado(true);
        }
    };

    const GridOnBeforeButtonAction = (context, data, nexus) => {
        if (data.parameters.find(x => x.id === "ubicacion") != null)
            data.parameters = [{ id: "ubicacion", value: data.parameters.find(x => x.id === "ubicacion").value }];
    };

    const GridOnAfterMenuItemAction = (context, data, nexus) => {

        let jsonAdded = data.parameters.find(w => w.id === "ListaFilasSeleccionadas").value;

        setRowSeleccionadasImprimir(jsonAdded);

    }

    const handleBeforeImportExcel = (context, data, nexus) => {
        data.parameters.push({ "id": "importExcel", "value": "true" });
    }

    return (

        <Page
            title={t("REG040_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div style={{
                textAlign: "center",
            }}>
                <button className="btn btn-info" onClick={openPrediosDialog}>{t("REG040_Sec0_btn_Predios")} </button>
                &nbsp;
                <button className="btn btn-primary" onClick={openAddDialog}>{t("REG040_Sec0_btn_Crear")}</button>
                &nbsp;
                <button className="btn btn-primary" onClick={openAtributosDialog}>{t("REG040_Sec0_btn_AtributosMasivos")}</button>
            </div>

            <Grid
                id="REG040_grid_1"
                rowsToFetch={30}
                rowsToDisplay={15}
                enableExcelExport
                enableSelection
                enableExcelImport={importarHabilitado}
                onAfterInitialize={onAfterInitialize}
                onBeforeButtonAction={GridOnBeforeButtonAction}
                onBeforeMenuItemAction={GridOnBeforeButtonAction}
                onAfterMenuItemAction={GridOnAfterMenuItemAction}
                onBeforeImportExcel={handleBeforeImportExcel}
                onBeforeGenerateExcelTemplate={handleBeforeImportExcel}
                importExcelCustom={(gridEventHandlersInternal, showModal) => {
                    return <REG040ImportExcel eventHandlers={gridEventHandlersInternal} showModal={showModal} />
                }}

            />

            <REG040CreateUbicacionModal show={showPopupAdd} onHide={closeAddDialog} />
            <REG040PrediosModal show={showPopupPredios} onHide={closePrediosDialog} />
            <REG040AtributosMasivosModal show={showPopupAtributos} onHide={closeAtributosDialog} />
            <IMP050ImpresionesUbicacionModal show={showPopupImprimir} onHide={closeImprimirDialog} rowSeleccionadas={rowSeleccionadasImprimir} />

        </Page>
    );
}
