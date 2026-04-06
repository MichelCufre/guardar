import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { STO810CreateMapeoModal } from './STO810CreateMapeo';
import { STO810UpdateMapeoModal } from './STO810UpdateMapeo';

export default function STO810(props) {

    const { t } = useTranslation();

    const [showPopup, setShowPopup] = useState(false);
    const [showUpdatePopup, setShowUpdatePopup] = useState(false);
    const [mapeoEditar, setMapeoEditar] = useState(null);

    const openFormDialog = () => {
        setShowPopup(true);
    }

    const closeFormDialog = () => {
        setShowPopup(false);
    }

    const closeUpdateFormDialog = () => {
        setShowUpdatePopup(false);
    }


    const handleBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnEditar") {
            context.abortServerCall = true;

            setMapeoEditar(
                {
                    cdEmpresaOrigen: data.row.cells.find(d => d.column === "CD_EMPRESA").value,
                    cdEmpresaDestino: data.row.cells.find(d => d.column === "CD_EMPRESA_DESTINO").value,
                    cdProdutoOrigen: data.row.cells.find(d => d.column === "CD_PRODUTO").value,
                }
            );

            setShowUpdatePopup(true);
        }
    }

    return (
        <Page
            icon="fas fa-cubes"
            title={t("STO810_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button id="btnCrear" className="btn btn-primary" onClick={openFormDialog}>{t("STO810_Sec0_btn_CrearMapeo")}</button>
            </div>
            <div className="row mb-4">
                <div className="col">
                    <Grid
                        id="STO810_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        onBeforeButtonAction={handleBeforeButtonAction}
                    />
                </div>
            </div>

            <STO810CreateMapeoModal show={showPopup} onHide={closeFormDialog} />
            <STO810UpdateMapeoModal show={showUpdatePopup} onHide={closeUpdateFormDialog} mapeo={mapeoEditar} />
        </Page>
    );
}