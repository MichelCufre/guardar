import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { useTranslation } from 'react-i18next';
import { UpdateEjecucionAutomatismoModal } from './UpdateEjecucionAutomatismoModal';
import { VerErroresAutomatismoModal } from './VerErroresAutomatismoModal';

export function EjecucionesAutomatismoGrid(props) {

    const { t } = useTranslation();

    const [showEditarEjecucionesModal, setShowEditarEjecucionesModal] = useState(false);

    const [showVerErroresModal, setShowVerErroresModal] = useState(false);

    const [automatismoEjecucion, setAutomatismoEjecucion] = useState(null);

    //--------------GRID--------------

    const onBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnEditar") {

            context.abortServerCall = true;

            setAutomatismoEjecucion(data.row.cells.find(w => w.column == "NU_AUTOMATISMO_EJECUCION").value);

            openEditarEjecucionesModal();
        }
        else if (data.buttonId === "btnVerErrores") {
            context.abortServerCall = true;

            setAutomatismoEjecucion(data.row.cells.find(w => w.column == "NU_AUTOMATISMO_EJECUCION").value);

            openVerErroresModal();
        }
    }
    const onAfterButtonAction = (data, nexus) => {
        nexus.getGrid(props.id).refresh();
    }

    //----------------------------------------


    //--------------UPDATE POPUP--------------

    const openEditarEjecucionesModal = () => {
        setShowEditarEjecucionesModal(true);
    }

    const closeEditarEjecucionModal = () => {
        setShowEditarEjecucionesModal(false);
    }

    //----------------------------------------

    //--------------VER ERRORES POPUP--------------

    const openVerErroresModal = () => {
        setShowVerErroresModal(true);
    }

    const closeVerErroresModal = () => {
        setShowVerErroresModal(false);
    }

    //----------------------------------------

    return (
        <div>
            <Grid
                application="AUT100Ejecuciones"
                id={props.id}
                onBeforeInitialize={props.onBeforeInitialize}
                onBeforeFetch={props.onBeforeFetch}
                onBeforeFetchStats={props.onBeforeFetchStats}
                onBeforeExportExcel={props.onBeforeExportExcel}
                onBeforeApplyFilter={props.onBeforeApplyFilter}
                onBeforeButtonAction={onBeforeButtonAction}
                onAfterButtonAction={onAfterButtonAction}
                onBeforeApplySort={props.onBeforeApplySort}
                rowsToFetch={30}
                rowsToDisplay={15}
                enableExcelExport
            />

            <UpdateEjecucionAutomatismoModal show={showEditarEjecucionesModal} onHide={closeEditarEjecucionModal} automatismoEjecucion={automatismoEjecucion} />

            <VerErroresAutomatismoModal show={showVerErroresModal} onHide={closeVerErroresModal} automatismoEjecucion={automatismoEjecucion} />
        </div>
    );
}
