import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import * as Yup from 'yup';
import { useTranslation } from 'react-i18next';
import { EVT020CrearContactoModal } from './EVT020CrearContactoModal';
import { EVT020EditarContactoModal } from './EVT020EditarContactoModal';

export default function EVT020(props) {
    const { t } = useTranslation();

    const [showCrearContactoPopup, setShowCrearContactoPopup] = useState(false);
    const [showEditarContactoPopup, setShowEditarContactoPopup] = useState(false);
    const [numeroContacto, setNumeroContacto] = useState();
    const [_nexus, setNexus] = useState(null);

    const openFormDialog = () => {
        setShowCrearContactoPopup(true);
    }

    const closePopup = () => {
        setShowCrearContactoPopup(false);
        setShowEditarContactoPopup(false);
        _nexus.getGrid("EVT020_grid_1").refresh();
    }

    const gridOnAfterButtonAction = (data, nexus) => {
        nexus.getForm("EVT020_form_1").reset(data.parameters);
        setShowForm(true);
    }

    const onAfterInitialize = (context, grid, parameters, nexus) => {
        setNexus(nexus);
    };

    const handleBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnEditar") {
            context.abortServerCall = true;
            setNumeroContacto(data.row.cells.find(d => d.column === "NU_CONTACTO").value);
            setShowEditarContactoPopup(true);
        }
    }

    return (
        <Page
            title={t("EVT020_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button className="btn btn-primary" onClick={openFormDialog}>{t("EVT020_frm1_btn_CrearContacto")}</button>
            </div>

            <div className="row mt-4 mb-4">
                <div className="col-12">
                    <Grid id="EVT020_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        onAfterButtonAction={gridOnAfterButtonAction}
                        onAfterInitialize={onAfterInitialize}
                        onBeforeButtonAction={handleBeforeButtonAction}
                    />
                </div>
            </div>

            <EVT020CrearContactoModal
                show={showCrearContactoPopup}
                onHide={closePopup}
            />
            <EVT020EditarContactoModal
                show={showEditarContactoPopup}
                onHide={closePopup}
                numeroContacto={numeroContacto}
            />
        </Page >
    );
}