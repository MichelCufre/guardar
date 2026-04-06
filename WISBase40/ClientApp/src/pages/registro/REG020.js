import React, { useState, useRef } from 'react';
import { Page } from '../../components/Page';
import { Grid } from '../../components/GridComponents/Grid';
import { Form, Field, FieldDate, FieldSelect, FieldSelectAsync, FieldTextArea, FormButton, SubmitButton, StatusMessage } from '../../components/FormComponents/Form';
import { Modal, Row, Col, FormGroup, Button, Tab, Tabs } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { REG020CreateFamiliaModal } from './REG020CreateFamiliaModal';
import { REG020UpdateFamiliaModal } from './REG020UpdateFamiliaModal';

export default function REG020(props) {

    const { t } = useTranslation();

    const [showPopup, setShowPopup] = useState(false);
    const [showPopupUpdate, setShowPopupUpdate] = useState(false);
    const [codigoFamilia, setcodigoFamilia] = useState(null);

    const openFormDialog = () => {
        setShowPopup(true);
    }

    const closeFormDialog = () => {
        setShowPopup(false);
    }

    const openFormUpdateDialog = () => {
        setShowPopupUpdate(true);
    }

    const closeFormUpdateDialog = () => {
        setShowPopupUpdate(false);
    }

    const GridOnBeforeButtonAction = (context, data, nexus) => {

        if (data.buttonId === "btnEditar") {

            context.abortServerCall = true;

            setcodigoFamilia(data.row.cells.find(w => w.column == "CD_FAMILIA_PRODUTO").value);

            openFormUpdateDialog();
        }

    };

    return (

        <Page
            title={t("REG020_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button className="btn btn-primary" onClick={openFormDialog}>{t("REG020_Sec0_btn_AgregarLinea")}</button>
            </div>

            <Grid
                id="REG020_grid_1"
                rowsToFetch={30}
                rowsToDisplay={15}
                enableExcelExport
                onBeforeButtonAction={GridOnBeforeButtonAction}
            />
            <REG020CreateFamiliaModal show={showPopup} onHide={closeFormDialog} />
            <REG020UpdateFamiliaModal show={showPopupUpdate} onHide={closeFormUpdateDialog} familia={codigoFamilia} />
        </Page>
    );
}
