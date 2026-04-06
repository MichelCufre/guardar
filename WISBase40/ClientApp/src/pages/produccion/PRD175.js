import React, { useState, useRef } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldDate, SubmitButton, Button, StatusMessage } from '../../components/FormComponents/Form';
import { Page } from '../../components/Page';
import { Row, Col } from 'react-bootstrap';
import * as Yup from 'yup';
import { useTranslation } from 'react-i18next';
import { getDateTimeString } from '../../components/DateTimeUtil';

export default function PRD175(props) {
    const { t } = useTranslation();
    const InfoIngresoRef = useRef({});

    const [isLoaded, setLoaded] = useState(false);

    const onAfterPageLoad = (data) => {
        if (data && data.parameters) {

            let ingreso = data.parameters.reduce((ing, param) => { ing[param.id] = param.value || "-"; return ing; }, {});

            ingreso.DT_ADDROW = ingreso.DT_ADDROW && ingreso.DT_ADDROW !== "-" ? getDateTimeString(new Date(ingreso.DT_ADDROW)) : ingreso.DT_ADDROW;

            InfoIngresoRef.current = ingreso;

            console.log(ingreso.current);

            setLoaded(true);
        }
    };

    return (

        <Page
            icon="fas fa-file"
            title={t("PRD175_Sec0_pageTitle_Titulo")}
            onAfterLoad={onAfterPageLoad}
            load
            {...props}
        >

            <Row>
                <Col>
                    <h1 className="mb-3">{InfoIngresoRef.current.NU_PRDC_INGRESO}</h1>
                </Col>
            </Row>

            <Row>
                <Col>
                    <h4 className="form-title">{t("PRD175_frm1_lbl_legend1")}</h4>
                    <Row>
                        <Col lg={6}>
                            <Row className="mb-2">
                                <Col>
                                    <span className="text-muted">{t("PRD175_frm1_lbl_CD_LINEA")} </span>
                                    <span>{`${InfoIngresoRef.current.CD_LINEA === undefined ? '-' : InfoIngresoRef.current.CD_LINEA}`}</span>
                                </Col>
                            </Row>
                            <Row className="mb-2">
                                <Col>
                                    <span className="text-muted">{t("PRD175_frm1_lbl_CD_ENDERECO_ENTRADA")} </span>
                                    <span>{`${InfoIngresoRef.current.CD_ENDERECO_ENTRADA === undefined ? '-' : InfoIngresoRef.current.CD_ENDERECO_ENTRADA}`}</span>
                                </Col>
                            </Row>
                            <Row className="mb-2">
                                <Col>
                                    <span className="text-muted">{t("PRD175_frm1_lbl_TP_LINEA")} </span>
                                    <span>{`${InfoIngresoRef.current.TP_LINEA === undefined ? '-' : InfoIngresoRef.current.TP_LINEA}`}</span>
                                </Col>
                            </Row>
                        </Col>
                        <Col lg={6}>
                            <Row className="mb-2">
                                <Col>
                                    <span className="text-muted">{t("PRD175_frm1_lbl_DS_LINEA")} </span>
                                    <span>{`${InfoIngresoRef.current.DS_LINEA === undefined ? '-' : InfoIngresoRef.current.DS_LINEA}`}</span>
                                </Col>
                            </Row>
                            <Row className="mb-2">
                                <Col>
                                    <span className="text-muted">{t("PRD175_frm1_lbl_CD_ENDERECO_SALIDA")} </span>
                                    <span>{`${InfoIngresoRef.current.CD_ENDERECO_SALIDA === undefined ? '-' : InfoIngresoRef.current.CD_ENDERECO_SALIDA}`}</span>
                                </Col>
                            </Row>
                        </Col>
                    </Row>
                </Col>
            </Row>

            <br />

            <Row>
                <Col>
                    <h4 className="form-title">{t("PRD175_frm1_lbl_legend2")}</h4>
                    <Row>
                        <Col lg={6}>
                            <Row className="mb-2">
                                <Col>
                                    <span className="text-muted">{t("PRD175_frm1_lbl_CD_SITUACION")} </span>
                                    <span>{`${InfoIngresoRef.current.CD_SITUACION === undefined ? '-' : InfoIngresoRef.current.CD_SITUACION}`}</span>
                                </Col>
                            </Row>
                            <Row className="mb-2">
                                <Col>
                                    <span className="text-muted">{t("PRD175_frm1_lbl_NU_PRDC_DEFNICION")} </span>
                                    <span>{`${InfoIngresoRef.current.NU_PRDC_DEFNICION === undefined ? '-' : InfoIngresoRef.current.NU_PRDC_DEFNICION}`}</span>
                                </Col>
                            </Row>
                            <Row className="mb-2">
                                <Col>
                                    <span className="text-muted">{t("PRD175_frm1_lbl_NU_PREDIO")} </span>
                                    <span>{`${InfoIngresoRef.current.NU_PREDIO === undefined ? '-' : InfoIngresoRef.current.NU_PREDIO}`}</span>
                                </Col>
                            </Row>
                            <Row className="mb-2">
                                <Col>
                                    <span className="text-muted">{t("PRD175_frm1_lbl_NU_DOCUMENTO_INGRESO")} </span>
                                    <span>{`${InfoIngresoRef.current.NU_DOCUMENTO_INGRESO === undefined ? '-' : InfoIngresoRef.current.NU_DOCUMENTO_INGRESO}`}</span>
                                </Col>
                            </Row>
                        </Col>
                        <Col lg={6}>
                            <Row className="mb-2">
                                <Col>
                                    <span className="text-muted">{t("PRD175_frm1_lbl_DT_ADDROW")} </span>
                                    <span>{`${InfoIngresoRef.current.DT_ADDROW === undefined ? '-' : InfoIngresoRef.current.DT_ADDROW}`}</span>
                                </Col>
                            </Row>
                            <Row className="mb-2">
                                <Col>
                                    <span className="text-muted">{t("PRD175_frm1_lbl_QT_FORMULA")} </span>
                                    <span>{`${InfoIngresoRef.current.QT_FORMULA === undefined ? '-' : InfoIngresoRef.current.QT_FORMULA}`}</span>
                                </Col>
                            </Row>
                            <Row className="mb-2">
                                <Col>
                                    <span className="text-muted">{t("PRD175_frm1_lbl_CD_FUNCIONARIO")} </span>
                                    <span>{`${InfoIngresoRef.current.CD_FUNCIONARIO === undefined ? '-' : InfoIngresoRef.current.CD_FUNCIONARIO}`}</span>
                                </Col>
                            </Row>
                            <Row className="mb-2">
                                <Col>
                                    <span className="text-muted">{t("PRD175_frm1_lbl_NU_DOCUMENTO_EGRESO")} </span>
                                    <span>{`${InfoIngresoRef.current.NU_DOCUMENTO_EGRESO === undefined ? '-' : InfoIngresoRef.current.NU_DOCUMENTO_EGRESO}`}</span>
                                </Col>
                            </Row>
                        </Col>
                    </Row>
                </Col>
            </Row>

            <hr />

            <Form id="PRD175_form_1">
                <Row>
                    <Col>
                        <SubmitButton id="btnCerrarProduccionWhiteBox" value={t("PRD175_Sec0_btn_CerrarProduccion")} className="btn" />
                    </Col>
                    <Col>
                        <SubmitButton id="btnConfirmarProduccionWhiteBox" value={t("PRD175_Sec0_btn_ConfirmarProduccion")} className="btn" />
                    </Col>
                    <Col>
                        <SubmitButton id="btnDesreservarProduccionWhiteBox" value={t("PRD175_Sec0_btn_DesreservarProduccion")} className="btn" />
                    </Col>
                    <Col>
                        <SubmitButton id="btnDescartarPasadasProduccionWhiteBox" value={t("PRD175_Sec0_btn_DescartarPasadaProduccion")} className="btn" />
                    </Col>
                </Row>
            </Form>

        </Page>
    );
}