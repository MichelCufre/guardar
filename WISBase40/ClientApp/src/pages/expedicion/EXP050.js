import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Row, Col, FormGroup, Container } from 'react-bootstrap';
import { Form, Field, FieldDateTime, FormButton, FieldCheckbox } from '../../components/FormComponents/Form';
import { useTranslation } from 'react-i18next';

export default function EXP050(props) {
    const { t } = useTranslation();

    const [cdCamion, setCdCamion] = useState(null);

    const [isInfoDisplayed, setIsInfoDisplayed] = useState(false);

    const [infoCabezalCamion, setInfoCabezalCamion] = useState({
        CAMION: "", EMPRESA: "", PEDIDO: "", SITUACION: "", CLIENTE: "", RESPETAORDEN: "", MATRICULA: "", FECHAINGRESO: "", PUERTA: ""
    });

    const onAfterInitialize = (context, grid, parameters, nexus) => {
        if (parameters.find(d => d.id === "EXP050_CD_CAMION") != null) {
            setInfoCabezalCamion({
                CAMION: parameters.find(s => s.id === "EXP050_CD_CAMION").value,
                EMPRESA: parameters.find(s => s.id === "EXP050_EMPRESA").value,
                MATRICULA: parameters.find(s => s.id === "EXP050_PLACA").value,
                FECHAINGRESO: parameters.find(s => s.id === "EXP050_DT_INGRESO").value,
                PUERTA: parameters.find(s => s.id === "EXP050_PUERTA").value,
                RUTA: parameters.find(s => s.id === "EXP050_RUTA").value,
                SITUACION: parameters.find(s => s.id === "EXP050_SITUACION").value,
                RESPETAORDEN: parameters.find(s => s.id === "EXP050_RESPETA_ORDEN").value == "S" ? "EXP050_frm1_lbl_Si" : "EXP050_frm1_lbl_No",
                CAMION: parameters.find(s => s.id === "EXP050_CD_CAMION").value,
            });
            setIsInfoDisplayed(true);
        }
    };

    const onAfterButtonAction = (context, form, query, nexus) => {
        window.location = query.redirect;
    }

    const gridOnBeforeButtonAction = (context, data, nexus) => {

        data.parameters = [
            { id: "idRespetaOrdenCarga", value: data.parameters.find(s => s.id === "EXP050_RESPETA_ORDEN").value }
        ];
    }

    const onBeforeFetch = (context, data, nexus) => {
        data.parameters = [
            { id: "camion", value: cdCamion },
        ];
    }

    const onBeforeExportExcel = (context, data, nexus) => {
        data.parameters = [
            { id: "camion", value: cdCamion },
        ];
    }

    const formOnAfterInitialize = (context, form, query, nexus) => {
        setCdCamion(query.parameters.find(w => w.id === "camion").value);
        nexus.getGrid("EXP050_grid_1").refresh();
    }

    const onBeforeMenuItemAction = (context, data, nexus) => {
        data.parameters = [
            { id: "camion", value: cdCamion },
        ];
    }

    const onAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("EXP050_grid_1").refresh();
    }

    return (

        <Page
            title={t("EXP050_Sec0_pageTitle_Titulo")}
            {...props}
        >

            <Container fluid style={{ display: isInfoDisplayed ? 'block' : 'none' }}>
                <Row>
                    <Col>
                        <Row>
                            <Col sm={3}>
                                <span style={{ fontWeight: "bold" }}>{t("EXP050_frm1_lbl_CD_CAMION")}:</span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoCabezalCamion.CAMION}`}</span>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={3}>
                                <span style={{ fontWeight: "bold" }}>{t("EXP050_frm1_lbl_CD_PUERTA")}:</span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoCabezalCamion.PUERTA}`}</span>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={3}>
                                <span style={{ fontWeight: "bold" }}>{t("EXP050_frm1_lbl_DT_INGRESO")}:</span>
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
                                <span style={{ fontWeight: "bold" }}>{t("EXP050_frm1_lbl_CD_PLACA")}:</span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoCabezalCamion.MATRICULA}`}</span>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={3}>
                                <span style={{ fontWeight: "bold" }}>{t("EXP050_frm1_lbl_CD_RUTA")}:</span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoCabezalCamion.RUTA}`}</span>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={3}>
                                <span style={{ fontWeight: "bold" }}>{t("EXP050_frm1_lbl_CD_SITUACAO")}:</span>
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
                                <span style={{ fontWeight: "bold" }}>{t("EXP050_frm1_lbl_CD_EMPRESA")}:</span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoCabezalCamion.EMPRESA}`}</span>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={3}>
                                <span style={{ fontWeight: "bold" }}>{t("EXP050_frm1_lbl_ID_RESPETA_ORD_CARGA")}:</span>
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

            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="EXP050_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport enableSelection
                        onAfterMenuItemAction={onAfterMenuItemAction}
                        onAfterInitialize={onAfterInitialize}
                    />
                </div>
            </div>

        </Page>
    );
}