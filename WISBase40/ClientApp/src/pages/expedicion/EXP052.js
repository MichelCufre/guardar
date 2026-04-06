import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Row, Col, Container } from 'react-bootstrap';
import { Form, Field, FieldSelect, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { useTranslation } from 'react-i18next';
import InputMask from 'react-input-mask';
import { Page } from '../../components/Page';
import * as Yup from 'yup';

export default function (props) {

    const { t } = useTranslation("translation", { useSuspense: false });

    const [isInfoDisplayed, setIsInfoDisplayed] = useState(false);
    const [isAutoGenerado, setAutoGenerado] = useState(true);
    const [formatoNumDoc, setFormatoNumDoc] = useState("");
    const [state, setState] = useState('');
    const [infoCabezalCamion, setInfoCabezalCamion] = useState({
        CAMION: "", EMPRESA: "", PEDIDO: "", SITUACION: "", CLIENTE: "", RESPETAORDEN: "", MATRICULA: "", FECHAINGRESO: "", PUERTA: ""
    });

    const onChange = (event) => {
        setState(event.target.value);
    }

    const onBeforeSubmit = (context, form, query, nexus) => {
        if (formatoNumDoc) {
            var param = {
                id: "nroDocNoGenerado",
                value: state
            }
            query.parameters.push(param);
        }
    };

    const onAfterInitialize = (context, form, query, nexus) => {
        if (query.parameters.find(d => d.id === "EXP052_CD_CAMION") != null) {
            setInfoCabezalCamion({
                CAMION: query.parameters.find(s => s.id === "EXP052_CD_CAMION").value,
                EMPRESA: query.parameters.find(s => s.id === "EXP052_EMPRESA").value,
                MATRICULA: query.parameters.find(s => s.id === "EXP052_PLACA").value,
                FECHAINGRESO: query.parameters.find(s => s.id === "EXP052_DT_INGRESO").value,
                PUERTA: query.parameters.find(s => s.id === "EXP052_PUERTA").value,
                RUTA: query.parameters.find(s => s.id === "EXP052_RUTA").value,
                SITUACION: query.parameters.find(s => s.id === "EXP052_SITUACION").value,
                RESPETAORDEN: query.parameters.find(s => s.id === "EXP052_RESPETA_ORDEN").value == "S" ? "EXP052_frm1_lbl_Si" : "EXP052_frm1_lbl_No",
                CAMION: query.parameters.find(s => s.id === "EXP052_CD_CAMION").value,
            });
            setIsInfoDisplayed(true);
        }
    };

    const initialValues = {
        tpEgreso: "",
        nroDoc: "",
        nroDocNoGenerado: "",
    };

    const validationSchema = {
        tpEgreso: Yup.string().required(),
        nroDocNoGenerado: Yup.string().nullable(),
    };

    const handleClose = () => {
        props.onHide();
    }

    const onAfterValidateField = (context, form, query, nexus) => {

        if (query.fieldId === "tpEgreso") {
            const isAutoGeneradoParam = query.parameters.find(p => p.id === "isAutoGenerado");

            if (isAutoGeneradoParam && isAutoGeneradoParam.value === "true") {
                setAutoGenerado(true);
            }
            else if (isAutoGeneradoParam && isAutoGeneradoParam.value === "false") {
                setAutoGenerado(false);
            }

            const _formatoNumDoc = query.parameters.find(p => p.id === "formatoNumDoc");

            if (_formatoNumDoc && _formatoNumDoc.value) {
                setFormatoNumDoc(_formatoNumDoc.value);
            }

            setState("");
        }

        query.fieldId == ""
    };

    return (
        <Page
            {...props}
            title={t("EXP052_Sec0_pageTitle_Titulo")}
        >
            <Form
                id="EXP052_form_1"
                application="EXP052"
                initialValues={initialValues}
                validationSchema={validationSchema}
                onBeforeSubmit={onBeforeSubmit}
                onAfterInitialize={onAfterInitialize}
                onAfterValidateField={onAfterValidateField}
            >
                <Container fluid style={{ display: isInfoDisplayed ? 'block' : 'none' }}>
                    <Row>
                        <Col>
                            <Row>
                                <Col sm={3}>
                                    <span style={{ fontWeight: "bold" }}>{t("EXP052_frm1_lbl_CD_CAMION")}:</span>
                                </Col>
                                <Col className='p-0'>
                                    <span> {`${infoCabezalCamion.CAMION}`}</span>
                                </Col>
                            </Row>
                        </Col>
                        <Col>
                            <Row>
                                <Col sm={3}>
                                    <span style={{ fontWeight: "bold" }}>{t("EXP052_frm1_lbl_CD_PUERTA")}:</span>
                                </Col>
                                <Col className='p-0'>
                                    <span> {`${infoCabezalCamion.PUERTA}`}</span>
                                </Col>
                            </Row>
                        </Col>
                        <Col>
                            <Row>
                                <Col sm={3}>
                                    <span style={{ fontWeight: "bold" }}>{t("EXP052_frm1_lbl_DT_INGRESO")}:</span>
                                </Col>
                                <Col className='p-0'>
                                    <span> {`${infoCabezalCamion.FECHAINGRESO}`}</span>
                                </Col>
                            </Row>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <Row>
                                <Col sm={3}>
                                    <span style={{ fontWeight: "bold" }}>{t("EXP052_frm1_lbl_CD_PLACA")}:</span>
                                </Col>
                                <Col className='p-0'>
                                    <span> {`${infoCabezalCamion.MATRICULA}`}</span>
                                </Col>
                            </Row>
                        </Col>
                        <Col>
                            <Row>
                                <Col sm={3}>
                                    <span style={{ fontWeight: "bold" }}>{t("EXP052_frm1_lbl_CD_RUTA")}:</span>
                                </Col>
                                <Col className='p-0'>
                                    <span> {`${infoCabezalCamion.RUTA}`}</span>
                                </Col>
                            </Row>
                        </Col>
                        <Col>
                            <Row>
                                <Col sm={3}>
                                    <span style={{ fontWeight: "bold" }}>{t("EXP052_frm1_lbl_CD_SITUACAO")}:</span>
                                </Col>
                                <Col className='p-0'>
                                    <span> {`${infoCabezalCamion.SITUACION}`}</span>
                                </Col>
                            </Row>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <Row>
                                <Col sm={3}>
                                    <span style={{ fontWeight: "bold" }}>{t("EXP052_frm1_lbl_CD_EMPRESA")}:</span>
                                </Col>
                                <Col className='p-0'>
                                    <span> {`${infoCabezalCamion.EMPRESA}`}</span>
                                </Col>
                            </Row>
                        </Col>
                        <Col>
                            <Row>
                                <Col sm={3}>
                                    <span style={{ fontWeight: "bold" }}>{t("EXP052_frm1_lbl_ID_RESPETA_ORD_CARGA")}:</span>
                                </Col>
                                <Col className='p-0'>
                                    <span> {t(`${infoCabezalCamion.RESPETAORDEN}`)}</span>
                                </Col>
                            </Row>
                        </Col>
                        <Col>
                            <Row>
                                <Col className='p-0'>
                                    <span></span>
                                </Col>
                            </Row>
                        </Col>
                    </Row>
                </Container>

                <hr style={{ display: isInfoDisplayed ? 'block' : 'none' }}></hr>

                <Container fluid>

                    <Row className="mb-2">
                        <Col>
                            <div className="form-group" >
                                <label htmlFor="tpEgreso">{t("EXP052_frm1_lbl_TP_DOCUMENTO")}</label>
                                <FieldSelect name="tpEgreso" />
                                <StatusMessage for="tpEgreso" />
                            </div>
                        </Col>
                    </Row>
                    <Row className="mb-2">
                        <Col >
                            <div className="form-group" style={{ display: isAutoGenerado ? 'block' : 'none' }}>
                                <label htmlFor="nroDoc">{t("EXP052_frm1_lbl_NU_DOCUMENTO")}</label>
                                <Field name="nroDoc" readOnly />
                                <StatusMessage for="nroDoc" />
                            </div>

                            <div style={{ display: isAutoGenerado ? 'none' : 'block' }}>

                                {formatoNumDoc ? (
                                    <div className="form-group">
                                        <label htmlFor="nroDocNoGenerado">{t("EXP052_frm1_lbl_NU_DOCUMENTO")}</label>
                                        <InputMask readonly className="undefined form-control" mask={formatoNumDoc} maskChar={null} value={state} onChange={onChange} />
                                        <Field hidden className="hidden" name="nroDocNoGenerado" />
                                        <StatusMessage for="nroDocNoGenerado" />
                                    </div>
                                )
                                    : (
                                        <div className="form-group">
                                            <label htmlFor="nroDocNoGenerado">{t("EXP052_frm1_lbl_NU_DOCUMENTO")}</label>
                                            <Field name="nroDocNoGenerado" />
                                            <StatusMessage for="nroDocNoGenerado" />
                                        </div>
                                    )
                                }
                            </div>
                        </Col>
                    </Row>
                </Container>
                <div style={{ textAlign: "center" }}>
                    <SubmitButton id="btnGenerar" variant="primary" label="EXP052_frm1_btn_generar" />
                </div>
            </Form>
        </Page>
    );
}