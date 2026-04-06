import React, { useState, useRef } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldDate, SubmitButton, Button, StatusMessage } from '../../components/FormComponents/Form';
import { Row, Col, FormGroup } from 'react-bootstrap';
import { FormTab, FormTabStep } from '../../components/FormComponents/FormTab';
import * as Yup from 'yup';
import { useTranslation } from 'react-i18next';
import { PRD190CreateLineaModal } from './PRD190CreateLineaModal';
import { PRD190IdentificadoresEgreso } from './PRD190IdentificadoresEgreso';

export default function PRD190(props) {
    const { t } = useTranslation();

    const [showPopup, setShowPopup] = useState(false);
    const [showPopupIdentificadores, setShowPopupIdentificadores] = useState(false);
    const [ubicacionIdentificadores, setUbicacionIdentificadores] = useState(null);

    const openFormDialog = () => {
        setShowPopup(true);
    }

    const closeFormDialog = () => {
        setShowPopup(false);
    }

    const openFormDialogIdentificadores = () => {
        setShowPopupIdentificadores(true);
    }

    const closeFormDialogIdentificadores = () => {
        setShowPopupIdentificadores(false);
    }

    const handleGridOnBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnIdentificadores") {
            context.abortServerCall = true;

            setUbicacionIdentificadores(data.row.cells.find(d => d.column === "CD_ENDERECO_SALIDA").value);

            openFormDialogIdentificadores();
        }
    }

    return (
        <Page
            icon="fas fa-file"
            title={t("PRD190_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button className="btn btn-primary" onClick={openFormDialog}>{t("PRD190_Sec0_btn_AgregarLinea")}</button>
            </div>

            <Grid
                id="PRD190_grid_1"
                rowsToFetch={30}
                rowsToDisplay={15}
                enableExcelExport
                onBeforeButtonAction={handleGridOnBeforeButtonAction}
                enableExcelImport={false}
            />
            <PRD190CreateLineaModal show={showPopup} onHide={closeFormDialog} />
            <PRD190IdentificadoresEgreso show={showPopupIdentificadores} onHide={closeFormDialogIdentificadores} ubicacion={ubicacionIdentificadores} />
        </Page>
    );
}