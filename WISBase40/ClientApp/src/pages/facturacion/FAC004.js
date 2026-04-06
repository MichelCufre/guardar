import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Form } from '../../components/FormComponents/Form';
import { Page } from '../../components/Page';
import { Modal, Row, Col, Container } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';


export default function FAC004(props) {

    const { t } = useTranslation();

    const [infoFacturacion, setInfoFacturacion] = useState({});

    const onAfterInitialize = (context, form, query, nexus) => {
        if (query.parameters.find(x => x.id === "nuEjecucion") != null) {
            setInfoFacturacion({
                NuEjecucion: query.parameters.find(x => x.id === "nuEjecucion").value,
                Nombre: query.parameters.find(x => x.id === "nombre").value,
                FechaDesde: query.parameters.find(x => x.id === "fechaDesde").value,
                FechaHasta: query.parameters.find(x => x.id === "fechaHasta").value,
                FechaProgramacion: query.parameters.find(x => x.id === "fechaProgramacion").value,
                FacturaParcial: query.parameters.find(x => x.id === "facturaParcial").value,
                Situacion: query.parameters.find(x => x.id === "situacion").value,
            });
        }
        nexus.getGrid("FAC004_grid_1").refresh();
    };

    const onAfterButtonAction = (data, nexus) => {
        nexus.getGrid("FAC004_grid_1").refresh();
    }

    const applyParameters = (context, data, nexus) => {
        data.parameters = [
            { id: "situacion", value: infoFacturacion.Situacion },
            { id: "nuEjecucion", value: infoFacturacion.NuEjecucion }
        ];
    };

    return (
        <Page
            title={t("FAC004_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Form
                application="FAC004"
                id="FAC004_form_1"
                application="FAC004"
                onAfterInitialize={onAfterInitialize}
            >
                <Container fluid>
                    <div style={{ marginBottom: '6vh' }}>
                        <Row>
                            <Col sm={2}>
                                <span style={{ fontWeight: "bold" }}>{t("FAC004_frm1_lbl_NroEjecucion")}: </span>
                            </Col>
                            <Col className='p-0'>
                                <span>{`${infoFacturacion.NuEjecucion ? infoFacturacion.NuEjecucion : "-"}`}</span>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={2}>
                                <span style={{ fontWeight: "bold" }}>{t("FAC004_frm1_lbl_Nombre")}: </span>
                            </Col>
                            <Col className='p-0'>
                                <span>{`${infoFacturacion.Nombre ? infoFacturacion.Nombre : "-"}`}</span>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={2}>
                                <span style={{ fontWeight: "bold" }}>{t("FAC004_frm1_lbl_FechaDesde")}: </span>
                            </Col>
                            <Col className='p-0'>
                                <span>{`${infoFacturacion.FechaDesde ? infoFacturacion.FechaDesde : "-"}`}</span>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={2}>
                                <span style={{ fontWeight: "bold" }}>{t("FAC004_frm1_lbl_FechaHasta")}: </span>
                            </Col>
                            <Col className='p-0'>
                                <span>{`${infoFacturacion.FechaHasta ? infoFacturacion.FechaHasta : "-"}`}</span>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={2}>
                                <span style={{ fontWeight: "bold" }}>{t("FAC004_frm1_lbl_FechaProgramacion")}: </span>
                            </Col>
                            <Col className='p-0'>
                                <span>{`${infoFacturacion.FechaProgramacion ? infoFacturacion.FechaProgramacion : "-"}`}</span>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={2}>
                                <span style={{ fontWeight: "bold" }}>{t("FAC004_frm1_lbl_FacturaParcial")}: </span>
                            </Col>
                            <Col className='p-0'>
                                <span>{`${infoFacturacion.FacturaParcial ? infoFacturacion.FacturaParcial : "-"}`}</span>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={2}>
                                <span style={{ fontWeight: "bold" }}>{t("FAC004_frm1_lbl_Situacion")}: </span>
                            </Col>
                            <Col className='p-0'>
                                <span>{`${infoFacturacion.Situacion ? infoFacturacion.Situacion : "-"}`}</span>
                            </Col>
                        </Row>
                    </div>
                </Container>


                <div className="row mb-4">
                    <div className="col-12">
                        <Grid id="FAC004_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport enableExcelImport={false}
                            onAfterButtonAction={onAfterButtonAction}
                            onBeforeFetch={applyParameters}
                        />
                    </div>
                </div>

            </Form>
        </Page>
    );
}