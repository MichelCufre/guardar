import React, { useState, useRef } from 'react';
import { Page } from '../../components/Page';
import { Grid } from '../../components/GridComponents/Grid';
import { Form, Field, FieldDate, FieldSelect, FieldSelectAsync, FieldTextArea, FormButton, SubmitButton, StatusMessage } from '../../components/FormComponents/Form';
import { Modal, Row, Col, FormGroup, Button, Tab, Tabs } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { REG221CreateModal } from './REG221CreateModal';
import { REG221UpdateModal } from './REG221UpdateModal';


export default function REG221(props) {

    const { t } = useTranslation();

    const [showPopupAdd, setShowPopupAdd] = useState(false);
    const [showPopupUpdate, setShowPopupUpdate] = useState(false);
    const [keyAgente, setkeyAgente] = useState(null);

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


    const GridOnBeforeButtonAction = (context, data, nexus) => {

        if (data.buttonId === "btnEditar") {

            context.abortServerCall = true;

            setkeyAgente(
                [
                    { id: "cliente", value: data.row.cells.find(w => w.column == "CD_CLIENTE").value },
                    { id: "empresa", value: data.row.cells.find(w => w.column == "CD_EMPRESA").value },
                    { id: "ventanaLiberacion", value: data.row.cells.find(w => w.column == "CD_VENTANA_LIBERACION").value }
                ]
            );

            openFormUpdateDialog();
        }
    };

    return (

        <Page
            title={t("REG221_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button className="btn btn-primary" onClick={openFormDialog}>{t("REG221_Sec0_btn_AgregarLinea")}</button>
            </div>

            <Grid
                application="REG221"
                id="REG221_grid_1"
                rowsToFetch={30}
                rowsToDisplay={15}
                enableExcelExport
                onBeforeButtonAction={GridOnBeforeButtonAction}
            />
            <REG221CreateModal show={showPopupAdd} onHide={closeFormDialog} />
            <REG221UpdateModal show={showPopupUpdate} onHide={closeFormUpdateDialog} agente={keyAgente} />
        </Page>
    );
}
