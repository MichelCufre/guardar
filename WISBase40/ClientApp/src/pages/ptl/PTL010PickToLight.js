import React, { useState } from 'react';
import { Button } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { PTL010ColoresActivosModal } from './PTL010ColoresActivosModal';
import { PTL010NotificarPTLModal } from './PTL010NotificarPTLModal';


export default function PTL010PickToLight(props) {

    const { t } = useTranslation();
    const [showModalColoresActivos, setShowModalColoresActivos] = useState(false);
    const [showNotificarPTLPopup, setShowNotificarPTLPopup] = useState(false);
    const [preparacionEditar, setPreparacionEditar] = useState(null);
    const [clienteEditar, setClienteEditar] = useState(null);
    const [empresaEditar, setEmpresaEditar] = useState(null);
    const [numeroAutomatismoEditar, setNuAutomatismoEditar] = useState(null);
    const [nexus, setNexus] = useState(null);

    const openModalColoresActivos = () => {
        setShowModalColoresActivos(true);
    };

    const closeModalColoresActivos = () => {
        setShowModalColoresActivos(false);
    }
    //*********************************************************************************************
    //                                         GRID EVENTS
    //*********************************************************************************************
    const onAfterInitialize = (context, grid, parameters, nexus) => {
        setNexus(nexus);
    };

    const closeNotificarPTLPopup = (datos) => {
        debugger;
        nexus.getGrid("PTL010_grid_1").refresh();
        setShowNotificarPTLPopup(false);
    }

    const addParameters = (context, data, nexus) => {
    };

    const handleAfterButtonAction = (data, nexus) => {

        if (data.buttonId === "btnNotificarPTL") {

            setPreparacionEditar(data.row.cells.find(d => d.column === "NU_PREPARACION").value);
            setClienteEditar(data.row.cells.find(d => d.column === "CD_CLIENTE").value);
            setEmpresaEditar(data.row.cells.find(d => d.column === "CD_EMPRESA").value);
            setNuAutomatismoEditar(data.row.cells.find(d => d.column === "NU_AUTOMATISMO").value);

            setShowNotificarPTLPopup(true);
        }
        else {
            nexus.getGrid("PTL010_grid_1").refresh();
        }
    }

    //*********************************************************************************************

    return (
        <Page
            application="PTL010"
            title={t("PTL010_Sec0_pageTitle_Titulo")}
            {...props}
        >

            <div style={{ textAlign: "center" }}>
                <Button className="btn btn-primary" onClick={openModalColoresActivos}>{t("PTL010_Sec0_btn_VerColoresActivos")}</Button>
            </div>

            <Grid
                application="PTL010"
                id="PTL010_grid_1"
                rowsToFetch={20}
                rowsToDisplay={15}
                enableExcelExport
                onBeforeButtonAction={addParameters}
                onAfterButtonAction={handleAfterButtonAction}
                onAfterInitialize={onAfterInitialize}

            />

            <PTL010NotificarPTLModal
                show={showNotificarPTLPopup}
                onHide={closeNotificarPTLPopup}
                numeroAutomatismo={numeroAutomatismoEditar}
                cliente={clienteEditar}
                empresa={empresaEditar}
                preparacion={preparacionEditar}
                nexus={nexus}
            />

            <PTL010ColoresActivosModal show={showModalColoresActivos} onHide={closeModalColoresActivos} nexus={nexus} />

        </Page>
    );
}