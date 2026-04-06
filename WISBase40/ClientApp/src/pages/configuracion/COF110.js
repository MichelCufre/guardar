import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { COF110CreateOrUpdate } from './COF110CreateOrUpdate';

export default function COF110(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    const [showPopup, setShowPopup] = useState(false);
    const [isUpdate, setIsUpdate] = useState(false);
    const [nroIntegracion, setNroIntegracion] = useState(null);

    const openFormDialog = () => {
        setShowPopup(true);
    }

    const closeFormDialog = (nexus) => {
        setShowPopup(false);
        setIsUpdate(false);
    }

    const handleBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnEditar") {
            context.abortServerCall = true;
            setNroIntegracion(data.row.cells.find(d => d.column === "NU_INTEGRACION").value);
            setIsUpdate(true);
            openFormDialog(true);
        }
    }

    return (
        <Page
            title={t("COF110_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button className="btn btn-primary" onClick={openFormDialog}>{t("COF110_Sec0_btn_CrearServicioIntegracion")}</button>
            </div>
            <div className="row mb-4">
                <div className="col-12">
                    <Grid
                        id="COF110_grid_1"
                        application="COF110"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        onBeforeButtonAction={handleBeforeButtonAction}
                        enableExcelExport
                    />
                </div>
            </div>

            <COF110CreateOrUpdate show={showPopup} onHide={closeFormDialog} isUpdate={isUpdate} nroIntegracion={nroIntegracion} />
        </Page>
    );
}