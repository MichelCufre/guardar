import React, { useState, useRef } from 'react';
import { Page } from '../../components/Page';
import { Grid } from '../../components/GridComponents/Grid';
import { Form, Field, FieldDate, FieldSelect, FieldSelectAsync, FieldTextArea, FormButton, SubmitButton, StatusMessage } from '../../components/FormComponents/Form';
import { Modal, Row, Col, FormGroup, Button, Tab, Tabs } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { REG220CreateAgenteModal } from './REG220CreateAgenteModal';
import { REG220UpdateAgenteModal } from './REG220UpdateAgenteModal';
import { REG220RutasAgenteModal } from './REG220RutasAgenteModal';

export default function REG220(props) {

    const { t } = useTranslation();

    const [showPopupAdd, setShowPopupAdd] = useState(false);
    const [showPopupUpdate, setShowPopupUpdate] = useState(false);
    const [keyAgente, setkeyAgente] = useState(null);
    const [showPopupRutas, setShowPopupRutas] = useState(false);

    const openFormDialog = () => {
        setShowPopupAdd(true);
    }

    const closeFormDialog = () => {
        setShowPopupAdd(false);
    }

    const openFormUpdateDialog = () => {
        setShowPopupUpdate(true);
    }

    const closeFormUpdateDialog = () => {
        setShowPopupUpdate(false);
    }

    const openRutaDialog = () => {
        setShowPopupRutas(true);
    }
    const closeRutaDialog = () => {
        setShowPopupRutas(false);
    }

    const GridOnBeforeButtonAction = (context, data, nexus) => {

        if (data.buttonId === "btnEditar") {

            context.abortServerCall = true;

            setkeyAgente(
                [
                    { id: "CodigoInterno", value: data.row.cells.find(w => w.column == "CD_CLIENTE").value },
                    { id: "IdEmpresa", value: data.row.cells.find(w => w.column == "CD_EMPRESA").value }
                ]
            );

            openFormUpdateDialog();
        }

        if (data.buttonId === "btnRutas") {

            context.abortServerCall = true;

            setkeyAgente(
                [
                    { id: "CodigoInterno", value: data.row.cells.find(w => w.column == "CD_CLIENTE").value },
                    { id: "IdEmpresa", value: data.row.cells.find(w => w.column == "CD_EMPRESA").value }
                ]
            );

            openRutaDialog();
        }

    };

    return (

        <Page
            title={t("REG220_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button className="btn btn-primary" onClick={openFormDialog}>{t("REG220_Sec0_btn_AgregarLinea")}</button>
            </div>

            <Grid
                id="REG220_grid_1"
                rowsToFetch={30}
                rowsToDisplay={15}
                enableExcelExport
                onBeforeButtonAction={GridOnBeforeButtonAction}
            />
            <REG220CreateAgenteModal show={showPopupAdd} onHide={closeFormDialog} />
            <REG220UpdateAgenteModal show={showPopupUpdate} onHide={closeFormUpdateDialog} agente={keyAgente} />
            <REG220RutasAgenteModal show={showPopupRutas} onHide={closeRutaDialog} agente={keyAgente} />

        </Page>
    );
}
