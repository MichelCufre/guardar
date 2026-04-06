import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { STO820AsignarPreparacionModal } from './STO820AsignarPreparacion';
import { STO820CreateTraspasoModal } from './STO820CreateTraspaso';
import { STO820UpdateTraspasoModal } from './STO820UpdateTraspaso';

export default function STO820(props) {

    const { t } = useTranslation();

    const [showPopup, setShowPopup] = useState(false);
    const [showUpdatePopup, setShowUpdatePopup] = useState(false);
    const [showAsignarPreparacionPopup, setShowAsignarPreparacionPopup] = useState(false);
    const [traspasoEditar, setTraspasoEditar] = useState(null);

    const openFormDialog = () => {
        setShowPopup(true);
    }

    const closeFormDialog = () => {
        setShowPopup(false);
    }

    const closeUpdateFormDialog = () => {
        setShowUpdatePopup(false);
    }

    const closeAsignarPreparacionDialog = () => {
        setShowAsignarPreparacionPopup(false);
    }

    const handleBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnEditar" || data.buttonId === "btnDetalles") {
            context.abortServerCall = true;

            setTraspasoEditar(
                {
                    Id: data.row.cells.find(d => d.column === "NU_TRASPASO").value,
                    Estado: data.row.cells.find(d => d.column === "ID_ESTADO").value,
                    TipoTraspaso: data.row.cells.find(d => d.column === "TP_TRASPASO").value,
                }
            );

            setShowUpdatePopup(true);
        }
        else if (data.buttonId === "btnAsignarPreparacion" || data.buttonId === "btnDetPreparacion") {

            context.abortServerCall = true;

            setTraspasoEditar(
                {
                    Id: data.row.cells.find(d => d.column === "NU_TRASPASO").value,
                    Estado: data.row.cells.find(d => d.column === "ID_ESTADO").value,
                    TipoTraspaso: data.row.cells.find(d => d.column === "TP_TRASPASO").value,
                }
            );

            setShowAsignarPreparacionPopup(true);
        }
        else if (data.buttonId === "btnCancelarTraspaso") {
            data.parameters =
            [
                { id: "idTraspaso", value: data.row.cells.find(d => d.column == "NU_TRASPASO").value },
            ];
        }
    }

    const handleAfterButtonAction = (data, nexus) => {
        if (data.buttonId === "btnCancelarTraspaso") 
            nexus.getGrid("STO820_grid_1").refresh();
    }

    return (
        <Page
            icon="fas fa-cubes"
            title={t("STO820_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button id="btnCrear" className="btn btn-primary" onClick={openFormDialog}>{t("STO820_Sec0_btn_CrearTraspaso")}</button>
            </div>
            <div className="row mb-4">
                <div className="col">
                    <Grid
                        id="STO820_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        onBeforeButtonAction={handleBeforeButtonAction}
                        onAfterButtonAction={handleAfterButtonAction}
                    />
                </div>
            </div>

            <STO820CreateTraspasoModal
                show={showPopup}
                onHide={closeFormDialog} />
            <STO820UpdateTraspasoModal
                show={showUpdatePopup}
                onHide={closeUpdateFormDialog}
                traspaso={traspasoEditar} />
            <STO820AsignarPreparacionModal
                show={showAsignarPreparacionPopup}
                onHide={closeAsignarPreparacionDialog}
                traspaso={traspasoEditar}/>
        </Page>
    );
}