import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { STO800AgregarEmpresasModal } from './STO800AgregarEmpresasDestino';
import { STO800AgregarTiposTraspasoModal } from './STO800AgregarTiposTraspaso';
import { STO800CreateConfiguracionModal } from './STO800CreateConfiguracion';
import { STO800UpdateConfiguracionModal } from './STO800UpdateConfiguracion';

export default function STO800(props) {

    const { t } = useTranslation();

    const [showPopup, setShowPopup] = useState(false);
    const [showUpdatePopup, setShowUpdatePopup] = useState(false);
    const [showAsignarEmpresasPopup, setShowAsignarEmpresasPopup] = useState(false);
    const [showAsignarTiposTraspasoPopup, setShowAsignarTiposTraspasoPopup] = useState(false);
    const [configEditar, setConfigEditar] = useState(null);

    const openFormDialog = () => {
        setShowPopup(true);
    }

    const closeFormDialog = () => {
        setShowPopup(false);
    }

    const closeUpdateFormDialog = () => {
        setShowUpdatePopup(false);
    }

    const closeAsignarEmpresasDialog = () => {
        setShowAsignarEmpresasPopup(false);
    }

    const closeAsignarTiposTraspasoDialog = () => {
        setShowAsignarTiposTraspasoPopup(false);
    }

    const handleBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnEditar") {
            context.abortServerCall = true;

            setConfigEditar(data.row.cells.find(d => d.column === "NU_TRASPASO_CONFIGURACION").value);

            setShowUpdatePopup(true);
        }
        else if (data.buttonId === "btnAgregarEmpresasDestino") {

            context.abortServerCall = true;

            setConfigEditar(data.row.cells.find(d => d.column == "NU_TRASPASO_CONFIGURACION").value);

            setShowAsignarEmpresasPopup(true);
        }
        else if (data.buttonId === "btnAgregarTiposTraspaso") {

            context.abortServerCall = true;

            setConfigEditar(data.row.cells.find(d => d.column == "NU_TRASPASO_CONFIGURACION").value);

            setShowAsignarTiposTraspasoPopup(true);
        }
    }

    return (
        <Page
            icon="fas fa-cubes"
            title={t("STO800_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button id="btnCrear" className="btn btn-primary" onClick={openFormDialog}>{t("STO800_Sec0_btn_CrearConfig")}</button>
            </div>
            <div className="row mb-4">
                <div className="col">
                    <Grid
                        id="STO800_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        onBeforeButtonAction={handleBeforeButtonAction}
                    />
                </div>
            </div>

            <STO800CreateConfiguracionModal show={showPopup} onHide={closeFormDialog} />
            <STO800UpdateConfiguracionModal show={showUpdatePopup} onHide={closeUpdateFormDialog} config={configEditar} />
            <STO800AgregarEmpresasModal show={showAsignarEmpresasPopup} onHide={closeAsignarEmpresasDialog} config={configEditar} />
            <STO800AgregarTiposTraspasoModal show={showAsignarTiposTraspasoPopup} onHide={closeAsignarTiposTraspasoDialog} config={configEditar} />
        </Page>
    );
}