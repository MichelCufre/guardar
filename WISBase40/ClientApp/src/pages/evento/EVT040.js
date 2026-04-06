import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { EVT040CrearInstanciaModal } from './EVT040CrearInstanciaModal';
import { EVT040ModificarInstanciaModal } from './EVT040ModificarInstanciaModal';
import { EVT040DestinatariosInstanciaModal } from './EVT040DestinatariosInstanciaModal';

export default function EVT040(props) {
    const { t } = useTranslation();

    const [showModal, setShowModal] = useState(false);
    const [showModifyModal, setShowModifyModal] = useState(false);
    const [showModalDesinatarios, setShowModalDesinatarios] = useState(false);

    const [nexus, setNexus] = useState(null);
    const [instancia, setInstancia] = useState(null);

    const initialValues = {

    };

    const validationSchema = {
    };

    const openFormDialog = () => {
        setShowModal(true);
    }

    const openFormDestinatarios = () => {
        setShowModalDesinatarios(true);
    }

 
    const closeFormDialogEditar = (datos) => {
        setShowModifyModal(false);
        setInstancia(null);

        nexus.getGrid("EVT040_grid_Instancias").refresh();
    }

    const closeFormDialogCrear = (instancia, irDestinatarios) => {
        setShowModal(false);
        setInstancia(null);

        nexus.getGrid("EVT040_grid_Instancias").refresh();

        if (irDestinatarios) {
            setInstancia(instancia);
            openFormDestinatarios();
        }
    }

    const closeFormDialogDestinatarios = (datos) => {
        setShowModalDesinatarios(false);
        setInstancia(null);

        nexus.getGrid("EVT040_grid_Instancias").refresh();
    }

    const onAfterInitialize = (context, form, parameters, nexus) => {
        setNexus(nexus);
    }

    const onBeforeButtonAction = (context, form, query, nexus) => {
        context.abortServerCall = true;

        setInstancia(form.row.cells.find(d => d.column === "NU_EVENTO_INSTANCIA").value);

        if (form.buttonId === "btnEditar") {
            setShowModifyModal(true);
        } else if (form.buttonId === "btnDestinatarios") {
            setShowModalDesinatarios(true);
        }
    };

    return (
        <Page
            title={t("EVT040_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button id="btnCrearInstancia" className="btn btn-primary" onClick={openFormDialog}>{t("EVT040_Sec0_btn_CrearInstancia")}</button>
            </div>
            <div className="row mt-5 mb-4">
                <div className="col-12">
                    <Grid id="EVT040_grid_Instancias"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        onAfterInitialize={onAfterInitialize }
                        onBeforeButtonAction={onBeforeButtonAction}
                    />
                </div>
            </div>
            <EVT040CrearInstanciaModal show={showModal} onHide={closeFormDialogCrear} />
            <EVT040ModificarInstanciaModal show={showModifyModal} instancia={instancia} onHide={closeFormDialogEditar} />
            <EVT040DestinatariosInstanciaModal show={showModalDesinatarios} instancia={instancia} onHide={closeFormDialogDestinatarios} />
        </Page >
    );
}
