import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import REG104Create from './REG104Create';
import REG104Update from './REG104Update';

export default function REG104(props) {

    const { t } = useTranslation();

    const [zonaSeleccionada, setZonaSeleccionada] = useState(null);
    const [showPopupCreate, setshowPopupCreate] = useState(false);
    const [showPopupUpdate, setshowPopupUpdate] = useState(false);
    const [nexxus, setNexxus] = useState(null);

    const openModalCreate = () => {

        setshowPopupCreate(true);
    }

    const closeModalCreate = (nexus) => {
        setshowPopupCreate(false);
        if (nexus)
            nexus.getGrid("REG104_grid_1").refresh()
    }

    const openModalUpdate = () => {

        setshowPopupUpdate(true);
    }

    const closeModalUpdate = (nexus) => {
        setshowPopupUpdate(false);
        if (nexus)
            nexus.getGrid("REG104_grid_1").refresh()
    }

    const onAfterInitialize = (context, grid, parameters, nexus) => {
        setNexxus(nexus);
    };

    const handleBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnEditar") {
            context.abortServerCall = true;

            setZonaSeleccionada(data.row.cells.find(d => d.column === "CD_ZONA").value);

            openModalUpdate();
        }
    }

    return (
        <Page
            title={t("REG104_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button id="btnNuevaZona" className="btn btn-primary" onClick={openModalCreate}>{t("REG104_Sec0_btn_NuevaZona")}</button>
            </div>
            <Grid
                id="REG104_grid_1"
                rowsToFetch={30}
                rowsToDisplay={15}
                enableExcelExport
                onAfterInitialize={onAfterInitialize}
                onBeforeButtonAction={handleBeforeButtonAction}
            />

            <REG104Create show={showPopupCreate} onHide={closeModalCreate} nexxus={nexxus} hideLoadError={true} opener={"btnNuevaZona"} />
            <REG104Update show={showPopupUpdate} onHide={closeModalUpdate} zona={zonaSeleccionada} nexxus={nexxus} hideLoadError={true} />

        </Page>
    );
}