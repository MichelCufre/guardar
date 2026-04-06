import React, { useState, useRef } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldDateTime, SubmitButton, FormButton as Button, StatusMessage } from '../../components/FormComponents/Form';
import { useTranslation } from 'react-i18next';
import { Row, Col } from 'react-bootstrap';
import { getDateTimeString } from '../../components/DateTimeUtil';
import * as Yup from 'yup';

export default function DOC350(props) {
    const { t } = useTranslation();
    const [agrupadorRef, setAgrupadorRef] = useState("");
    const [fechaImpresoRef, setfechaImpresoRef] = useState("-");
    const nuAgrupador = useRef("");
    const tpAgrupador = useRef("");

    const initialValues = {
        fechLlegada: ""
    }

    const validationSchema =
    {
        fechLlegada: Yup.string().nullable()
    }
    const secondarySubmitStyle = { width: "300px !important" };
    const confirmButtonClassName = "btn btn-warning";

    const onAfterPageLoad = (data) => {
        if (data && data.parameters) {

            let agrupador = data.parameters.reduce((doc, param) => { doc[param.id] = param.value || "-"; return doc; }, {});

            setAgrupadorRef(agrupador);
            nuAgrupador.current = agrupador.NU_AGRUPADOR;
            tpAgrupador.current = agrupador.TP
        }
    };

    const setFechaImpreso = (context, form, query, nexus) => {
        const fechaImpreso = query.parameters.find(p => p.id === "fechaImpreso");

        if (fechaImpreso) {
            setfechaImpresoRef(fechaImpreso.value);
        }
    };
    const getPDF = (context, form, query, nexus) => {

        //const parameters = [
        //    {
        //        id: "NU_AGRUPADOR",
        //        value: nuAgrupador.current
        //    },
        //    {
        //        id: "TP_AGRUPADOR",
        //        value: tpAgrupador.current
        //    }
        //];

        //nexus.generateReport("DOC350_Report", parameters);
    };
    const onAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {
            nexus.getForm("DOC350_form_1").reset();
        }
    };

    return (
        <Page
            load
            icon="fas fa-copy"
            title={t("DOC350_Sec0_pageTitle_Titulo")}
            onAfterLoad={onAfterPageLoad}
            {...props}
        >
            <Row>
                <Col>
                    <h1 className="mb-3">{agrupadorRef.NU_AGRUPADOR}</h1>
                </Col>
            </Row>
            <Row>
                <Col lg={6}>
                    <Row>
                        <Col>
                            <h4 className="form-title">{t("DOC350_frm1_lbl_legend1")}</h4>
                            <Row className="mb-2">
                                <Col>
                                    <span className="text-muted">{t("DOC350_frm1_lbl_NM_EMPRESA")} </span>
                                    <span>{`${agrupadorRef.NM_EMPRESA === undefined ? '-' : agrupadorRef.NM_EMPRESA}`}</span>
                                </Col>
                            </Row>
                            <Row className="mb-2">
                                <Col>
                                    <span className="text-muted">{t("DOC350_frm1_lbl_CNPJ_EMPRESA")} </span>
                                    <span>{`${agrupadorRef.CNPJ_EMPRESA === undefined ? '-' : agrupadorRef.CNPJ_EMPRESA}`}</span>
                                </Col>
                            </Row>
                            <Row className="mb-2">
                                <Col>
                                    <span className="text-muted">{t("DOC350_frm1_lbl_DS_ENDERECO_EMPRESA")} </span>
                                    <span>{`${agrupadorRef.DS_ENDERECO_EMPRESA === undefined ? '-' : agrupadorRef.DS_ENDERECO_EMPRESA}`}</span>
                                </Col>
                            </Row>
                        </Col>
                    </Row>
                </Col>
                <Col lg={6}>
                    <Row>
                        <Col>
                            <h4 className="form-title">{t("DOC350_frm1_lbl_legend5")}</h4>
                            <Row>
                                <Col>
                                    <span className="text-muted">{t("DOC350_frm1_lbl_DT_ADDROW")} </span>
                                    <span>{`${agrupadorRef.DT_ADDROW === undefined ? '-' : agrupadorRef.DT_ADDROW}`}</span>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <span className="text-muted">{t("DOC350_frm1_lbl_DT_SAIDA")} </span>
                                    <span>{`${agrupadorRef.DT_SAIDA === undefined ? '-' : agrupadorRef.DT_SAIDA}`}</span>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <span className="text-muted">{t("DOC350_frm1_lbl_DT_LLEGADA")} </span>
                                    <span>{`${agrupadorRef.DT_LLEGADA === undefined ? '-' : agrupadorRef.DT_LLEGADA}`}</span>
                                </Col>
                            </Row>
                        </Col>
                    </Row>
                </Col>
            </Row>
            <Row >
                <div className="col-6 offset-6">
                    <h4 className="form-title">{t("DOC350_frm1_lbl_legend7")}</h4>
                    <Row>
                        <Col>
                            <span className="text-muted">{t("DOC350_frm1_lbl_DS_TRANSPORTADORA")} </span>
                            <span>{`${agrupadorRef.DS_TRANSPORTADORA === undefined ? '-' : agrupadorRef.DS_TRANSPORTADORA}`}</span>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <span className="text-muted">{t("DOC350_frm1_lbl_CD_CGC_TRANSP")} </span>
                            <span>{`${agrupadorRef.CD_CGC_TRANSP === undefined ? '-' : agrupadorRef.CD_CGC_TRANSP}`}</span>
                        </Col>
                    </Row>
                </div>
            </Row>
            <Row>
                <Col lg={6}>
                    <h4 className="form-title">{t("DOC350_frm1_lbl_legend4")}</h4>
                    <Row className="mb-2">
                        <Col>
                            <span className="text-muted">{t("DOC350_frm1_lbl_TP_AGRUPADOR")} </span>
                            <span>{`${agrupadorRef.TP_AGRUPADOR === undefined ? '-' : agrupadorRef.TP_AGRUPADOR}`}</span>
                        </Col>
                    </Row>
                    <Row className="mb-2">
                        <Col>
                            <span className="text-muted">{t("DOC350_frm1_lbl_ID_ESTADO")} </span>
                            <span>{`${agrupadorRef.ID_ESTADO === undefined ? '-' : agrupadorRef.ID_ESTADO}`}</span>
                        </Col>
                    </Row>
                </Col>
                <Col lg={3}>
                    <h4 className="form-title">{t("DOC350_frm1_lbl_legend8")}</h4>
                    <Row>
                        <Col>
                            <span className="text-muted">{t("DOC350_frm1_lbl_DS_PLACA")} </span>
                            <span>{`${agrupadorRef.DS_PLACA === undefined ? '-' : agrupadorRef.DS_PLACA}`}</span>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <span className="text-muted">{t("DOC350_frm1_lbl_TP_VEHICULO")} </span>
                            <span>{`${agrupadorRef.TP_VEHICULO === undefined ? '-' : agrupadorRef.TP_VEHICULO}`}</span>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <span className="text-muted">{t("DOC350_frm1_lbl_NU_LACRE")} </span>
                            <span>{`${agrupadorRef.NU_LACRE === undefined ? '-' : agrupadorRef.NU_LACRE}`}</span>
                        </Col>
                    </Row>
                </Col>
                <Col lg={3}>
                    <h4 className="form-title">{t("DOC350_frm1_lbl_legend9")}</h4>
                    <Row>
                        <Col>
                            <span className="text-muted">{t("DOC350_frm1_lbl_DS_MOTORISTA")} </span>
                            <span>{`${agrupadorRef.DS_MOTORISTA === undefined ? '-' : agrupadorRef.DS_MOTORISTA}`}</span>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <span className="text-muted">{t("DOC350_frm1_lbl_CPF")} </span>
                            <span>{`${agrupadorRef.CPF === undefined ? '-' : agrupadorRef.CPF}`}</span>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <span className="text-muted">{t("DOC350_frm1_lbl_CNH")} </span>
                            <span>{`${agrupadorRef.CNH === undefined ? '-' : agrupadorRef.CNH}`}</span>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <span className="text-muted">{t("DOC350_frm1_lbl_ANEXO1")} </span>
                            <span>{`${agrupadorRef.ANEXO === undefined ? '-' : agrupadorRef.ANEXO}`}</span>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <span className="text-muted">{t("DOC350_frm1_lbl_ANEXO2")} </span>
                            <span>{`${agrupadorRef.ANEXO2 === undefined ? '-' : agrupadorRef.ANEXO2}`}</span>
                        </Col>
                    </Row>
                </Col>
            </Row>
            <Row style={{ display: agrupadorRef.ID_ESTADO === 'CAN' ? 'block' : 'none' }}>
                <Col lg={6}>
                    <h4 className="form-title">{t("DOC350_frm1_lbl_legend11")}</h4>
                    <Row>
                        <Col>
                            <span>{`${agrupadorRef.DS_MOTIVO === undefined ? '-' : agrupadorRef.DS_MOTIVO}`}</span>
                        </Col>
                    </Row>

                </Col>
            </Row>
            <hr />

            <Form id="DOC350_form_1"
                onAfterInitialize={setFechaImpreso}
                onBeforeButtonAction={getPDF}
                onAfterButtonAction={setFechaImpreso}
                validationSchema={validationSchema}
                onAfterSubmit={onAfterSubmit}
                initialValues={initialValues}
            >
                <div className="row" >

                    <div className="col-2">
                        <div className="form-group">
                            <label htmlFor="fechLlegada">{t("DOC350_frm1_lbl_DT_LLEGADA_INP")}</label>
                            <FieldDateTime name="fechLlegada" id="fechLlegada" />
                            <StatusMessage for="fechLlegada" />
                        </div>
                    </div>

                    <div className="col-2">
                        <div style={{ marginTop: "10%" }}>
                            <SubmitButton id="btnSubmit" value={t("DOC350_Sec0_btn_CONFIRMAR")} />
                            {/*&nbsp;*/}
                            {/*<Button id="BtnGetPdf" value={t("DOC350_Sec0_btn_GET_PDF")} className={confirmButtonClassName} style={secondarySubmitStyle} />*/}
                        </div>
                    </div>

                    {/*<div className="col-2">*/}
                    {/*    <div style={{ marginTop: "10%" }}>*/}
                    {/*        <span className="text-muted">{t("DOC350_frm1_lbl_DT_IMPRESO")} </span>*/}
                    {/*        <span>{`${fechaImpresoRef === undefined ? '-' : fechaImpresoRef}`}</span>*/}
                    {/*    </div>*/}
                    {/*</div>*/}

                </div>
            </Form>

            <div className="col-12">
                <h2>{t("DOC350_frm1_lbl_legend10")}</h2>
                <Grid
                    id="DOC350_grid_1"
                    rowsToFetch={30}
                    rowsToDisplay={15}
                    enableExcelExport
                />
            </div>

            <div className="col-3 offset-9" style={{ paddingRight: '2%' }}>

                <Row>
                    <Col>
                        <span className="text-muted">{t("DOC350_frm1_lbl_QTD_DOCUMENTOS")} </span>
                    </Col>
                    <div textAlign='right' >
                        <span >{`${agrupadorRef.QTD_DOCUMENTOS === undefined ? '-' : agrupadorRef.QTD_DOCUMENTOS}`}</span>
                    </div>
                </Row>
                <Row>
                    <Col>
                        <span className="text-muted">{t("DOC350_frm1_lbl_VL_TOTAL_TRANSPORTADO")} </span>
                    </Col>
                    <div textAlign='right' >
                        <span>{`${agrupadorRef.VL_TOTAL_TRANSPORTADO === undefined ? '-' : agrupadorRef.VL_TOTAL_TRANSPORTADO}`}</span>
                    </div>
                </Row>
                <Row>
                    <Col>
                        <span className="text-muted">{t("DOC350_frm1_lbl_VL_TOTAL_TRANSPORTADO_CIF")} </span>
                    </Col>
                    <div textAlign='right' >
                        <span>{`${agrupadorRef.VL_TOTAL_TRANSPORTADO_CIF === undefined ? '-' : agrupadorRef.VL_TOTAL_TRANSPORTADO_CIF}`}</span>
                    </div>
                </Row>
            </div>
        </Page>
    );
}