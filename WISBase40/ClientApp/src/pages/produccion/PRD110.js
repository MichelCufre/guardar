import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { PRD110CreateIngresoModal } from './PRD110CreateIngresoModal';
import { Modal } from 'react-bootstrap';
import { PRD110DetallesTeoricos } from './PRD110DetallesTeoricos';
import { PRD110DetallesProducccion } from './PRD110DetallesProducccion';
import { PRD110AsociarEspacioProduccionModal } from './PRD110AsociarEspacioProduccionModal';

export default function PRD110(props) {
    const { t } = useTranslation();

    const [showPopup, setShowPopup] = useState(false);
    const [_nexus, setNexus] = useState(false);

    const [isAsociarEpsacioModalOpen, setIsAsociarEpsacioModalOpen] = useState(false);
    const [isAsociarControlesModalOpen, setIsAsociarControlesModalOpen] = useState(false);
    const [isDetallesProduccionOpen, setIsDetallesProduccionOpen] = useState(false);

    const [ingreso, setIngreso] = useState();
    const [empresa, setEmpresa] = useState();

    const openFormDialog = () => {
        setShowPopup(true);
    }

    const closeFormDialog = () => {
        _nexus.getGrid("PRD110_grid_1").refresh();
        setShowPopup(false);
    }

    const closeFormAsociarEspacio = () => {
        _nexus.getGrid("PRD110_grid_1").refresh();
        setIsAsociarEpsacioModalOpen(false);
    }

    const onBeforeInitialize = (context, data, nexus) => {
        setNexus(nexus);
    }

    const handleBeforeButtonAction = (context, data, nexus) => {
        setIngreso(data.row.cells.find(d => d.column === "NU_PRDC_INGRESO").value);

        if (data.buttonId === "btnAsociarEspacio") {
            context.abortServerCall = true;
            setIsAsociarEpsacioModalOpen(true);
        }
        else if (data.buttonId === "btnDetalles") {
            context.abortServerCall = true;
            setEmpresa(data.row.cells.find(d => d.column === "CD_EMPRESA").value);
            setIsAsociarControlesModalOpen(true);
        }
        else if (data.buttonId === "btnDetallesProducidos") {
            context.abortServerCall = true;
            setEmpresa(data.row.cells.find(d => d.column === "CD_EMPRESA").value);

            setIsDetallesProduccionOpen(true);
        }
        else if (data.buttonId === "btnPlanificacionInsumos") {
            context.abortServerCall = false;
        }
        else if (data.buttonId === "btnInstrucciones") {
            context.abortServerCall = true;
            setIsInstruccionesModalOpen(true);
        }
        else if (data.buttonId === "btnInstrucciones") {
            context.abortServerCall = true;
            setIngresoEditar(data.row.cells.find(d => d.column === "NU_PRDC_INGRESO").value);
            setIsInstruccionesModalOpen(true);
        }
        else if (data.buttonId === "btnProduccir") {
            context.abortServerCall = false;
        }
    }

    const handleAfterButtonAction = (context, nexus) => {
        if (context.buttonId === "btnIniciarProduccion") {
            nexus.getGrid("PRD110_grid_1").refresh();
        }
    }

    return (

        <Page
            icon="fas fa-file"
            title={t("PRD110_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button className="btn btn-primary" onClick={openFormDialog}>{t("PRD110_Sec0_btn_AbrirModal")}</button>
            </div>

            <Grid id="PRD110_grid_1"
                onBeforeInitialize={onBeforeInitialize}
                rowsToFetch={30}
                rowsToDisplay={15}
                enableExcelExport
                enableExcelImport={false}
                onBeforeButtonAction={handleBeforeButtonAction}
                onAfterButtonAction={handleAfterButtonAction}
            />
            <PRD110AsociarEspacioProduccionModal show={isAsociarEpsacioModalOpen} onHide={closeFormAsociarEspacio} ingresoEditar={ingreso} />
            <PRD110CreateIngresoModal show={showPopup} onHide={closeFormDialog} />
            <PRD110DetallesTeoricos show={isAsociarControlesModalOpen} onHide={() => setIsAsociarControlesModalOpen(false)} ingresoEditar={ingreso} empresa={empresa} />
            <PRD110DetallesProducccion show={isDetallesProduccionOpen} onHide={() => setIsDetallesProduccionOpen(false)} ingresoProduccion={ingreso} empresa={empresa} />

        </Page>
    );
}