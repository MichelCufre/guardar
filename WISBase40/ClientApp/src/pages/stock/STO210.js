import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { Page } from '../../components/Page';
import { Form, Field, FieldSelect, FieldSelectAsync, FormButton, SubmitButton, StatusMessage } from '../../components/FormComponents/Form';
import { Row, Col, FormGroup, Button } from 'react-bootstrap';
import { Grid } from '../../components/GridComponents/Grid';

export default function STO210(props) {
    const { t } = useTranslation();

    const [isShowForm, setIsShowForm] = useState(false);
    const FormShowClassName = isShowForm ? "" : "hidden";
    const BtnShowClassName = !isShowForm ? "" : "hidden";

    const HideForm = (evt) => {
        setIsShowForm(false);
        evt.preventDefault();
    }

    const validationSchema = {

        ID_ENVASE: Yup.string().required().max(20),
        CD_AGENTE: Yup.string().required().max(40),
        ND_TP_ENVASE: Yup.string().required().max(10),
        CD_BARRAS: Yup.string().nullable().max(50),
        ND_ESTADO_ENVASE: Yup.string().required().max(10),
        DS_OBSERVACIONES: Yup.string().nullable().max(200),
        CD_EMPRESA: Yup.string().required(),
        TP_AGENTE: Yup.string().required().max(3),
    };

    const onAfterSubmit = (context, form, query, nexus) => {

        if (context.status === "ERROR") return;

        nexus.getGrid("STO210_grid_1").refresh();
        nexus.getForm("STO210_form_1").reset();
        setIsShowForm(false);

    };

    const FormOnBeforeButtonAction = (context, form, query, nexus) => {

        if (query.buttonId === "btnOpenForm") {
            context.abortServerCall = true;
            nexus.getForm("STO210_form_1").reset();
            setIsShowForm(true);
        }

    };

    const GridOnBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnEditar") {
            context.abortServerCall = true;
            nexus.getForm("STO210_form_1").reset([
                {
                    id: "ROW_KEY", value: data.row.id
                }
            ]);
            setIsShowForm(true);
        }
    };

    return (

        <Page
            title={t("STO210_Sec0_pageTitle_Titulo")}
            {...props}
        >

            <Form
                id="STO210_form_1"
                validationSchema={validationSchema}
                onAfterSubmit={onAfterSubmit}
                onBeforeButtonAction={FormOnBeforeButtonAction}
            >

                <div style={{ textAlign: "center" }} className={BtnShowClassName}>
                    <FormButton id="btnOpenForm" label="General_Sec0_btn_OpenForm" />
                </div>

                <div className={FormShowClassName}>

                    <Row>
                        <Col>
                            <FormGroup>
                                <label htmlFor="ND_TP_ENVASE">{t("STO210_frm1_lbl_ND_TP_ENVASE")}</label>
                                <FieldSelect name="ND_TP_ENVASE" />
                                <StatusMessage for="ND_TP_ENVASE" />
                            </FormGroup>
                        </Col>
                        <Col>
                            <FormGroup>
                                <label htmlFor="ID_ENVASE">{t("STO210_frm1_lbl_ID_ENVASE")}</label>
                                <Field name="ID_ENVASE" />
                                <StatusMessage for="ID_ENVASE" />
                            </FormGroup>
                        </Col>
                    </Row>
                    <Row>
                        <Col lg="4">
                            <FormGroup>
                                <label htmlFor="CD_EMPRESA">{t("STO210_frm1_lbl_CD_EMPRESA")}</label>
                                <FieldSelectAsync name="CD_EMPRESA" />
                                <StatusMessage for="CD_EMPRESA" />
                            </FormGroup>
                        </Col>
                        <Col lg="2">
                            <FormGroup>
                                <label htmlFor="TP_AGENTE">{t("STO210_frm1_lbl_TP_AGENTE")}</label>
                                <FieldSelect name="TP_AGENTE" />
                                <StatusMessage for="TP_AGENTE" />
                            </FormGroup>
                        </Col>
                        <Col lg="6">
                            <FormGroup>
                                <label htmlFor="CD_AGENTE">{t("STO210_frm1_lbl_CD_AGENTE")}</label>
                                <FieldSelectAsync name="CD_AGENTE" />
                                <StatusMessage for="CD_AGENTE" />
                            </FormGroup>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <FormGroup>
                                <label htmlFor="ND_ESTADO_ENVASE">{t("STO210_frm1_lbl_ND_ESTADO_ENVASE")}</label>
                                <FieldSelect name="ND_ESTADO_ENVASE" />
                                <StatusMessage for="ND_ESTADO_ENVASE" />
                            </FormGroup>
                        </Col>
                        <Col>
                            <FormGroup>
                                <label htmlFor="CD_BARRAS">{t("STO210_frm1_lbl_CD_BARRAS")}</label>
                                <Field name="CD_BARRAS" readOnly/>
                                <StatusMessage for="CD_BARRAS" />
                            </FormGroup>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <FormGroup>
                                <label htmlFor="DS_OBSERVACIONES">{t("STO210_frm1_lbl_DS_OBSERVACIONES")}</label>
                                <Field name="DS_OBSERVACIONES" />
                                <StatusMessage for="DS_OBSERVACIONES" />
                            </FormGroup>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <SubmitButton id="btnConfirmar" variant="warning" label="General_Sec0_btn_Confirmar" />
                            &nbsp;
                            < Button color="primary" onClick={HideForm}>{t("General_Sec0_btn_Cancelar")}</ Button >
                        </Col>
                    </Row>

                </div>

            </Form>

            <hr />

            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="STO210_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                        onBeforeButtonAction={GridOnBeforeButtonAction}
                    />
                </div>
            </div>

        </Page>
    );
}
