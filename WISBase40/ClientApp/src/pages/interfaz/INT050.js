import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { INT050AdministrarBloqueosModal } from './INT050AdministrarBloqueosModal';
import INT070Modal from './INT070Modal';

export default function INT050(props) {
    const { t } = useTranslation();
    const [showModal, setShowModal] = useState(false);
    const [showModalBloqueos, setShowModalBloqueos] = useState(false);
    const [nuInterfazEjecucion, setNuInterfazEjecucion] = useState('');
    const [showBtnAdministrarBloqueo, setShowBtnAdministrarBloqueo] = useState(false)

    const onAfterButtonAction = (data, nexus) => {
        if (data.buttonId == "btnViewError") {
            setNuInterfazEjecucion(data.parameters.find(e => e.id === "interfaz").value);
            setShowModal(true);
        } else if (data.buttonId == "btnReprocess") {
            nexus.getGrid("INT050_grid_1").refresh();
        }
    }

    const onHide = () => {
        setShowModal(false);
    }

    const openModalBloqueos = () => {
        setShowModalBloqueos(true);
    }

    const closeModalBloqueos = () => {
        setShowModalBloqueos(false);
    }

    const onBeforeInitialize = (context, data, nexus) => {
        if (data.parameters.length > 0) {
            setNuInterfazEjecucion(data.parameters.find(e => e.id === "interfaz").value);
        }
    }

    const handleBeforeImportExcel = (context, data, nexus, api, empresa, referencia) => {
        data.parameters = [
            { id: "api", value: api },
            { id: "empresa", value: empresa },
            { id: "referencia", value: referencia }
        ];
    }

    const onAfterImportExcelSuccess = (context, data, parameters, nexus) => {
        nexus.getGrid("INT050_grid_1").refresh();
    }

    const onAfterPageLoad = (data) => {
        debugger;
        const permiso = data.parameters.find(d => d.id === "PermisoBtnAdministrarBloqueo");
        if (permiso?.value == "true") {
            setShowBtnAdministrarBloqueo(true)
        }
    };

    return (
        <Page
            title={t("INT050_Sec0_pageTitle_Titulo") + " " + nuInterfazEjecucion}
            onAfterLoad={onAfterPageLoad}
            {...props}
        >
            <div style={{ textAlign: "center", display: showBtnAdministrarBloqueo ? "block" : "none" }}>
                <button className="btn btn-primary" onClick={openModalBloqueos}>{t("INT050_Sec0_btn_AdministrarBloqueos")}</button>
            </div>
            <Grid
                application="INT050"
                id="INT050_grid_1"
                rowsToFetch={30}
                rowsToDisplay={15}
                onBeforeInitialize={onBeforeInitialize}
                enableExcelExport
                enableExcelImport
                onAfterButtonAction={onAfterButtonAction}
                onBeforeImportExcel={handleBeforeImportExcel}
                onAfterImportExcelSuccess={onAfterImportExcelSuccess}
                onBeforeGenerateExcelTemplate={handleBeforeImportExcel}

            />
            <INT070Modal show={showModal} onHide={onHide} nuInterfaz={nuInterfazEjecucion} />
            <INT050AdministrarBloqueosModal show={showModalBloqueos} onHide={closeModalBloqueos} />
        </Page>
    )
}