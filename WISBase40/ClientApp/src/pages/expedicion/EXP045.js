import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Modal, Button, Row, Col, Tab, Tabs, Container } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { ro } from 'date-fns/esm/locale';
import './EXP045.css';

export default function EXP045(props) {

    const { t } = useTranslation();

    const [cdCamion, setCdCamion] = useState(null);

    const [isInfoDisplayed, setIsInfoDisplayed] = useState(false);

    const [infoCabezalCamion, setInfoCabezalCamion] = useState({
        CAMION: "", EMPRESA: "", PEDIDO: "", SITUACION: "", CLIENTE: "", RESPETAORDEN: "", MATRICULA: "", FECHAINGRESO: "", PUERTA: ""
    });

    const onAfterInitialize = (context, grid, parameters, nexus) => {
        if (parameters.find(d => d.id === "EXP045_CD_CAMION") != null) {
            setInfoCabezalCamion({
                CAMION: parameters.find(s => s.id === "EXP045_CD_CAMION").value,
                EMPRESA: parameters.find(s => s.id === "EXP045_EMPRESA").value,
                MATRICULA: parameters.find(s => s.id === "EXP045_PLACA").value,
                FECHAINGRESO: parameters.find(s => s.id === "EXP045_DT_INGRESO").value,
                PUERTA: parameters.find(s => s.id === "EXP045_PUERTA").value,
                RUTA: parameters.find(s => s.id === "EXP045_RUTA").value,
                SITUACION: parameters.find(s => s.id === "EXP045_SITUACION").value,
                RESPETAORDEN: parameters.find(s => s.id === "EXP045_RESPETA_ORDEN").value == "S" ? "EXP045_frm1_lbl_Si" : "EXP045_frm1_lbl_No",
                CAMION: parameters.find(s => s.id === "EXP045_CD_CAMION").value,
            });
            setIsInfoDisplayed(true);

            setKeyTab("embarcados");
        }
    };

    const [keyTab, setKeyTab] = useState(null);

    const onAfterButtonAction = (context, form, query, nexus) => {
        window.location = query.redirect;
    }

    const gridOnBeforeButtonAction = (context, data, nexus) => {

        data.parameters = [
            { id: "idRespetaOrdenCarga", value: data.parameters.find(s => s.id === "EXP045_RESPETA_ORDEN").value }
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
        nexus.getGrid("EXP045_grid_1").refresh();
    }
    const onBeforeMenuItemAction = (context, data, nexus) => {
        data.parameters = [
            { id: "camion", value: cdCamion },
        ];
    }
    const onAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("EXP045_grid_1").refresh();
    }

    return (
        <Page
            title={t("EXP045_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Container fluid style={{ display: isInfoDisplayed ? 'block' : 'none' }}>
                <Row>
                    <Col>
                        <Row>
                            <Col sm={3}>
                                <span style={{ fontWeight: "bold" }}>{t("EXP045_frm1_lbl_CD_CAMION")}:</span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoCabezalCamion.CAMION}`}</span>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={3}>
                                <span style={{ fontWeight: "bold" }}>{t("EXP045_frm1_lbl_CD_PUERTA")}:</span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoCabezalCamion.PUERTA}`}</span>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={3}>
                                <span style={{ fontWeight: "bold" }}>{t("EXP045_frm1_lbl_DT_INGRESO")}:</span>
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
                                <span style={{ fontWeight: "bold" }}>{t("EXP045_frm1_lbl_CD_PLACA")}:</span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoCabezalCamion.MATRICULA}`}</span>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={3}>
                                <span style={{ fontWeight: "bold" }}>{t("EXP045_frm1_lbl_CD_RUTA")}:</span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoCabezalCamion.RUTA}`}</span>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={3}>
                                <span style={{ fontWeight: "bold" }}>{t("EXP045_frm1_lbl_CD_SITUACAO")}:</span>
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
                                <span style={{ fontWeight: "bold" }}>{t("EXP045_frm1_lbl_CD_EMPRESA")}:</span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoCabezalCamion.EMPRESA}`}</span>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={3}>
                                <span style={{ fontWeight: "bold" }}>{t("EXP045_frm1_lbl_ID_RESPETA_ORD_CARGA")}:</span>
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

            <Tabs defaultActiveKey="embarcados" transition={false} id="noanim-tab-example"
                activeKey={keyTab}
                onSelect={(k) => setKeyTab(k)}
            >
                <Tab eventKey="embarcados" title={t("EXP045_Nav1_lbl_tab_1")}>

                    <Row className='mt-3'>
                        <Col>
                            <Grid id="EXP045_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                                onAfterInitialize={onAfterInitialize}
                            />
                        </Col>
                    </Row>
                </Tab>
                <Tab eventKey="sinEmbarcar" title={t("EXP045_Nav1_lbl_tab_2")}>

                    <Row className='mt-3'>
                        <Col>
                            <Grid id="EXP045_grid_2" rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                                onAfterInitialize={onAfterInitialize}
                            />
                        </Col>
                    </Row>
                </Tab>
                <Tab eventKey="sinPreparacion" title={t("EXP045_Nav1_lbl_tab_3")}>

                    <Row className='mt-3'>
                        <Col>
                            <Grid id="EXP045_grid_3" rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                                onAfterInitialize={onAfterInitialize}
                            />
                        </Col>
                    </Row>
                </Tab>
                <Tab eventKey="prodDetalleCamion" title={t("EXP045_Nav1_lbl_tab_4")}>

                    <Row className='mt-3'>
                        <Col>
                            <Grid id="EXP045_grid_4" rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                                onAfterInitialize={onAfterInitialize}
                            />
                        </Col>
                    </Row>
                </Tab>
            </Tabs>
        </Page>
    );
}