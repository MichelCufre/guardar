import React, { useState, useRef } from 'react';
import { Page } from '../../components/Page';
import { Grid } from '../../components/GridComponents/Grid';
import { useTranslation } from 'react-i18next';
import { IMP050ImpresionesHerramientaModal } from '../impresion/IMP050ImpresionHerramientasModal';
import { REG010CreateEquipoModal } from './REG010CreateEquipoModal';
import { IMP050ImpresionHerramientaPosicionModal } from '../impresion/IMP050ImpresionHerramientaPosicionModal';
export default function REG010(props) {

    const { t } = useTranslation();

    const [rowSeleccionadasImprimir, setRowSeleccionadasImprimir] = useState(null);
    const [showModalAdd, setShowModalAdd] = useState(false);
    const [showPopupImprimir, setShowPopupImprimir] = useState(false);

    const [showPopupImprimirPosicion, setShowPopupImprimirPosicion] = useState(false);
    const [equipo, setEquipo] = useState(null);
    const gridOnAfterMenuItemAction = (context, data, nexus) => {

        let jsonAdded = data.parameters.find(w => w.id === "ListaFilasSeleccionadas").value;

        setRowSeleccionadasImprimir(jsonAdded);

        openImprimirDialog();
    };

    const openImprimirDialog = () => {
        setShowPopupImprimir(true);
        setShowModalAdd(false);
        setShowPopupImprimirPosicion(false);
    }

    const closeImprimirDialog = () => {
        setShowPopupImprimir(false);
    }
    const openFormDialog = () => {
        setShowModalAdd(true);
        setShowPopupImprimir(false);
        setShowPopupImprimirPosicion(false);
    }

    const closeFormDialog = () => {
        setShowModalAdd(false);
    }

    const onAfterCommit = (context, rows, parameters, nexus) => {
        nexus.getGrid("REG010_grid_1").refresh();
    }
    const closeImprimirPosicionDialog = () => {
        setShowPopupImprimirPosicion(false);
    }
    const GridOnBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnImpPosicion") {
            context.abortServerCall = true;
            setEquipo(
                [
                    { id: "equipo", value: data.row.cells.find(w => w.column == "CD_EQUIPO").value }
                ]
            );

            openFormDialogImpresionPosicion();
        }
    };
    const openFormDialogImpresionPosicion = () => {
        setShowModalAdd(false);
        setShowPopupImprimir(false);
        setShowPopupImprimirPosicion(true);
    }

    return (
        <Page
            title={t("REG010_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button className="btn btn-primary" onClick={openFormDialog}>{t("REG010_Sec0_modalTitle_TituloCrear")}</button>
            </div>

            <Grid
                id="REG010_grid_1"
                rowsToFetch={30}
                rowsToDisplay={15}
                enableExcelExport
                enableSelection
                onAfterMenuItemAction={gridOnAfterMenuItemAction}
                onAfterCommit={onAfterCommit}
                onBeforeButtonAction={GridOnBeforeButtonAction}
            />

            <REG010CreateEquipoModal show={showModalAdd} onHide={closeFormDialog} />
            <IMP050ImpresionesHerramientaModal show={showPopupImprimir} onHide={closeImprimirDialog} rowSeleccionadas={rowSeleccionadasImprimir} />
            <IMP050ImpresionHerramientaPosicionModal show={showPopupImprimirPosicion} onHide={closeImprimirPosicionDialog} equipo={equipo} />
        </Page>
    );
}