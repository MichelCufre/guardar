import React, { useState,useRef } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldDate, SubmitButton, Button, StatusMessage } from '../../components/FormComponents/Form';
import { Page } from '../../components/Page';
import { Row, Col } from 'react-bootstrap';
import * as Yup from 'yup';
import { useTranslation } from 'react-i18next';
import { getDateTimeString } from '../../components/DateTimeUtil';

export default function PRD260(props) {
    const { t } = useTranslation();
    const ejecucionRef = useRef({});
    const [isLoaded, setLoaded] = useState(false);

    const onAfterPageLoad = (data) => {
        if (data && data.parameters) {

            let ejecucion = data.parameters.reduce((ejec, param) => { ejec[param.id] = param.value || "-"; return ejec; }, {});

            ejecucion.DT_COMIENZO = ejecucion.DT_COMIENZO && ejecucion.DT_COMIENZO !== "-" ? getDateTimeString(new Date(ejecucion.DT_COMIENZO)) : ejecucion.DT_COMIENZO;

            ejecucionRef.current = ejecucion;

            console.log(ejecucionRef.current);

            setLoaded(true);
        }
    };

    return (

        <Page
            icon="fas fa-file"
            title={t("PRD260_Sec0_pageTitle_Titulo")}
            onAfterLoad={onAfterPageLoad}
            load
            {...props}
        >

            <Row>
                <Col>
                    <h1 className="mb-3">{ejecucionRef.current.NU_EJECUCION}</h1>
                </Col>
            </Row>
            <Row>
                <Col>
                    <Row>
                        <Col lg={6}>
                            <Row className="mb-2">
                                <Col>
                                    <span className="text-muted">{t("PRD260_frm1_lbl_CD_SITUACION")} </span>
                                    <span>{`${ejecucionRef.current.CD_SITUACION === undefined ? '-' : ejecucionRef.current.CD_SITUACION}`}</span>
                                </Col>
                            </Row>
                            <Row className="mb-2">
                                <Col>
                                    <span className="text-muted">{t("PRD260_frm1_lbl_DS_REFERENCIA")} </span>
                                    <span>{`${ejecucionRef.current.DS_REFERENCIA === undefined ? '-' : ejecucionRef.current.DS_REFERENCIA}`}</span>
                                </Col>
                            </Row>
                        </Col>
                        <Col lg={6}>
                            <Row className="mb-2">
                                <Col>
                                    <span className="text-muted">{t("PRD260_frm1_lbl_NM_EMPRESA")} </span>
                                    <span>{`${ejecucionRef.current.NM_EMPRESA === undefined ? '-' : ejecucionRef.current.NM_EMPRESA}`}</span>
                                </Col>
                            </Row>
                            <Row className="mb-2">
                                <Col>
                                    <span className="text-muted">{t("PRD260_frm1_lbl_DT_COMIENZO")} </span>
                                    <span>{`${ejecucionRef.current.DT_COMIENZO === undefined ? '-' : ejecucionRef.current.DT_COMIENZO}`}</span>
                                </Col>
                            </Row>
                        </Col>
                    </Row>
                </Col>
            </Row>

            <hr/>

            <div className="row mb-4">
                <div className="col-12">
                    <Grid
                        id="PRD260_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                    />
                </div>
            </div>

            <Form id="PRD260_form_1">

                <div className="row">
                    <div className="col-6">
                        <div className="form-group">
                            <label htmlFor="dsMotivo">{t("PRD260_frm1_lbl_DS_MOTIVO")}</label>
                            <Field name="dsMotivo" />
                            <StatusMessage for="dsMotivo" />
                        </div>
                    </div>

                    <div className="col-6">
                        <div style={{ marginTop: '5%' }}>
                            <SubmitButton id="btnAprobar" value={t("PRD260_Sec0_btn_Aprobar")} className="btn btn-success" />
                            &nbsp;
                            <SubmitButton id="btnRechazar" value={t("PRD260_Sec0_btn_Rechazar")} className="btn btn-danger" />
                        </div>
                    </div>
                </div>

            </Form>

        </Page>
    );
}