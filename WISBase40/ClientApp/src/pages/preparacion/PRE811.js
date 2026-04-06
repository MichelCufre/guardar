import React, { useState, useRef } from 'react';
import { Page } from '../../components/Page';
import { Grid } from '../../components/GridComponents/Grid';
import { useTranslation } from 'react-i18next';
import { PRE811CreatePreferenciaModal } from './PRE811CreatePreferenciaModal';
import { PRE811UpdatePreferenciaModal } from './PRE811UpdatePreferenciaModal';
import { PRE811PreferenciaUsuarioModal } from './PRE811PreferenciaUsuarioModal';
import { PRE811PreferenciaEquipoModal } from './PRE811PreferenciaEquipoModal';
import { PRE811ConfiguracionModal } from './PRE811ConfiguracionModal';

export default function PRE811(props) {

    const { t } = useTranslation();
    const [preferencia, setPreferencia] = useState(null);
    const [_nexus, setNexus] = useState(null);

    const [showPopupAdd, setShowPopupAdd] = useState(false);
    const [showPopupUpdate, setShowPopupUpdate] = useState(false);
    const [showPopupUsuario, setShowPopupUsuario] = useState(false);
    const [showPopupEquipo, setShowPopupEquipo] = useState(false);
    const [showPopupConfiguracion, setShowPopupConfiguracion] = useState(false);

    const openFormDialog = () => {
        setShowPopupAdd(true);

        setShowPopupUpdate(false)
        setShowPopupEquipo(false)
        setShowPopupUsuario(false)
        setShowPopupConfiguracion(false)
    }

    const closeFormDialog = (nuPreferencia) => {
        setShowPopupAdd(false);

        if (nuPreferencia) {

            setPreferencia(nuPreferencia);
            openFormConfiguracionDialog();
        }
    }

    const openFormUpdateDialog = () => {
        setShowPopupUpdate(true);

        setShowPopupAdd(false)
        setShowPopupEquipo(false)
        setShowPopupUsuario(false)
        setShowPopupConfiguracion(false)
    }

    const closeFormUpdateDialog = (nuPreferencia) => {
        setShowPopupUpdate(false);

        if (nuPreferencia) {

            setPreferencia(nuPreferencia);
            openFormConfiguracionDialog();
        }
    }

    const openFormUsuarioDialog = () => {
        setShowPopupUsuario(true);

        setShowPopupAdd(false)
        setShowPopupUpdate(false)
        setShowPopupEquipo(false)
        setShowPopupConfiguracion(false)
    }

    const closeFormUsuarioDialog = () => {
        setShowPopupUsuario(false);
    }

    const openFormEquipoDialog = () => {
        setShowPopupEquipo(true);

        setShowPopupAdd(false)
        setShowPopupUpdate(false)
        setShowPopupUsuario(false)
        setShowPopupConfiguracion(false)
    }

    const closeFormEquipoDialog = () => {
        setShowPopupEquipo(false);
    }

    const openFormConfiguracionDialog = () => {
        setShowPopupConfiguracion(true);

        setShowPopupAdd(false)
        setShowPopupUpdate(false)
        setShowPopupUsuario(false)
        setShowPopupEquipo(false)
    }

    const closeFormConfiguracionDialog = (nuPreferencia) => {
        setShowPopupConfiguracion(false);

        if (nuPreferencia) {

            setPreferencia(nuPreferencia);
            openFormUpdateDialog();
        }

        _nexus.getGrid("PRE811_grid_1").refresh();
    }

    const onBeforeButtonAction = (context, data, nexus) => {

        if (data.buttonId === "btnEditar") {

            context.abortServerCall = true;
            setPreferencia(data.row.cells.find(w => w.column == "NU_PREFERENCIA").value);
            openFormUpdateDialog();
        }
        else if (data.buttonId === "btnAsignarUsuarios") {

            context.abortServerCall = true;
            setPreferencia(data.row.cells.find(w => w.column == "NU_PREFERENCIA").value);
            openFormUsuarioDialog();
        }
        else if (data.buttonId === "btnAsignarEquipos") {

            context.abortServerCall = true;
            setPreferencia(data.row.cells.find(w => w.column == "NU_PREFERENCIA").value);
            openFormEquipoDialog();
        }
        else if (data.buttonId === "btnConfiguracion") {

            context.abortServerCall = true;
            setPreferencia(data.row.cells.find(w => w.column == "NU_PREFERENCIA").value);
            openFormConfiguracionDialog();
        }
    };
    const onBeforeInitialize = (context, data, nexus) => {
        setNexus(nexus);
    };

    return (
        <Page
            title={t("PRE811_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button className="btn btn-success" onClick={openFormDialog}>{t("PRE811_Sec0_btn_CrearPreferencia")}</button>
            </div>

            <Grid
                application="PRE811PanelDePreferencias"
                id="PRE811_grid_1"
                rowsToFetch={30}
                rowsToDisplay={15}
                enableExcelExport
                onBeforeInitialize={onBeforeInitialize}
                onBeforeButtonAction={onBeforeButtonAction}
            />

            <PRE811CreatePreferenciaModal show={showPopupAdd} onHide={closeFormDialog} />
            <PRE811UpdatePreferenciaModal show={showPopupUpdate} onHide={closeFormUpdateDialog} preferencia={preferencia} />
            <PRE811PreferenciaUsuarioModal show={showPopupUsuario} onHide={closeFormUsuarioDialog} preferencia={preferencia} />
            <PRE811PreferenciaEquipoModal show={showPopupEquipo} onHide={closeFormEquipoDialog} preferencia={preferencia} />
            <PRE811ConfiguracionModal show={showPopupConfiguracion} onHide={closeFormConfiguracionDialog} preferencia={preferencia} />
        </Page>
    );
}
