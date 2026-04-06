import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Form, Field, FieldCheckbox, FieldSelect, FieldSelectAsync, FieldDate, SubmitButton, FormButton, StatusMessage, FieldDateTime } from '../../components/FormComponents/Form';
import { Row, Col, FormGroup } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { REG080CreatePuerta } from './REG080CreatePuertaModal';
import { REG080UpdatePuerta } from './REG080EditPuertaModal';

export default function REG080(props) {
    const { t } = useTranslation();

    const [isShowForm, setIsShowForm] = useState(false);
    const [showPopup, setShowPopup] = useState(false);
    const [showPopupUpdate, setShowPopupUpdate] = useState(false);
    const [codigoPuerta, setCodigoPuerta] = useState(null);

    const BtnShowClassName = !isShowForm ? "" : "hidden";

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

            setCodigoPuerta(data.row.cells.find(w => w.column == "CD_PORTA").value);

            openFormUpdateDialog();
        }

    };

    return (
        <Page
            title={t("REG080_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div style={{ textAlign: "center" }} className={BtnShowClassName}>
                <button id="btnShowForm" onClick={openFormDialog} className="btn btn-primary">{t("REG080_frm1_btn_ShowForm")}</button>
            </div>

            <Grid
                id="REG080_grid_1"
                rowsToFetch={30}
                rowsToDisplay={15}
                enableExcelExport
                onBeforeButtonAction={GridOnBeforeButtonAction}
            />
            <REG080CreatePuerta show={showPopup} onHide={closeFormDialog} />
            <REG080UpdatePuerta show={showPopupUpdate} onHide={closeFormUpdateDialog} puerta={codigoPuerta} />
        </Page>
    )
}
