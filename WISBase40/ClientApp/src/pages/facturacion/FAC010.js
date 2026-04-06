import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Form } from '../../components/FormComponents/Form';
import { Page } from '../../components/Page';
import { Modal, Row, Col, Container } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { AddRemovePanel } from '../../components/AddRemovePanel';
import './FAC010.css';

export default function FAC010(props) {

    const { t } = useTranslation();

    const [infoFacturacion, setInfoFacturacion] = useState({});

    const [modoLectura, setModoLectura] = useState(false);

    const FormOnAfterInitialize = (context, form, query, nexus) => {
        if (query.parameters.find(x => x.id === "nuEjecucion") != null) {
            setInfoFacturacion({
                NuEjecucion: query.parameters.find(x => x.id === "nuEjecucion").value,
                Nombre: query.parameters.find(x => x.id === "nombre").value,
                FechaDesde: query.parameters.find(x => x.id === "fechaDesde").value,
                FechaHasta: query.parameters.find(x => x.id === "fechaHasta").value
            });
        }
    };

    const GridOnAfterInitialize = (context, grid, parameters, nexus) => {
        if (parameters.find(x => x.id === "ModoLectura") != null) {
            setModoLectura(true);
        }
    };

    const GridOnAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("FAC010_grid_1").refresh();
        nexus.getGrid("FAC010_grid_2").refresh();
    }

    const GridOnBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnAsignacionSolapada") {
            context.abortServerCall = true;
        }
    }

    const handleAdd = (evt, nexus) => {
        nexus.getGrid("FAC010_grid_1").triggerMenuAction("btnAsociar", false, evt.ctrlKey);
    };

    const handleRemove = (evt, nexus) => {
        nexus.getGrid("FAC010_grid_2").triggerMenuAction("btnDesasociar", false, evt.ctrlKey);
    };

    return (
        <Page
            title={t("FAC010_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Form
                application="FAC010"
                id="FAC010_form_1"
                application="FAC010"
                onAfterInitialize={FormOnAfterInitialize}
            >
                <Container fluid>
                    <Row className="d-flex justify-content-center">
                        <Col className="d-flex justify-content-center label-warning">
                            <div className="btn solapadoCabezal  btn-sm">{t("FAC010_frm1_lbl_AsignacionSolapada")}</div>
                        </Col>
                    </Row>
                </Container>
                <hr></hr>
                <Container fluid>
                    <Row>
                        <Col sm={1}>
                            <span style={{ fontWeight: "bold" }}>{t("FAC010_frm1_lbl_NroEjecucion")}: </span>
                        </Col>
                        <Col className='p-0'>
                            <span>{`${infoFacturacion.NuEjecucion ? infoFacturacion.NuEjecucion : "-"} | ${infoFacturacion.Nombre ? infoFacturacion.Nombre : "-"}`}</span>
                        </Col>
                    </Row>
                    <Row>
                        <Col sm={1}>
                            <span style={{ fontWeight: "bold" }}>{t("FAC010_frm1_lbl_Desde")}: </span>
                        </Col>
                        <Col className='p-0'>
                            <span>{`${infoFacturacion.FechaDesde ? infoFacturacion.FechaDesde : "-"}`}</span>
                        </Col>
                    </Row>
                    <Row style={{ marginBottom: '6vh' }}>
                        <Col sm={1}>
                            <span style={{ fontWeight: "bold" }}>{t("FAC010_frm1_lbl_FechaHasta")}: </span>
                        </Col>
                        <Col className='p-0'>
                            <span>{`${infoFacturacion.FechaHasta ? infoFacturacion.FechaHasta : "-"}`}</span>
                        </Col>
                    </Row>
                    <Row style={{ marginBottom: '2vh', display: modoLectura ? 'block' : 'none', backgroundColor: 'red' }}>
                        <Col>
                            <span style={{ display: 'flex', justifyContent: 'center', color: 'white', fontWeight: 'bold' }}>{t("FAC010_frm1_msg_ModoLectura")}</span>
                        </Col>
                    </Row>
                </Container>
            </Form>

            <div className="row mb-4">
                <div className="col-12">
                    <AddRemovePanel
                        onAdd={handleAdd}
                        onRemove={handleRemove}
                        from={(
                            <Grid
                                application="FAC010"
                                id="FAC010_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                                enableSelection={!modoLectura}
                                enableExcelImport={false}
                                onAfterMenuItemAction={GridOnAfterMenuItemAction}
                                onAfterInitialize={GridOnAfterInitialize}
                                onBeforeButtonAction={GridOnBeforeButtonAction}
                            />
                        )}
                        to={(
                            <Grid
                                application="FAC010"
                                id="FAC010_grid_2" rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                                enableSelection={!modoLectura}
                                enableExcelImport={false}
                                onAfterMenuItemAction={GridOnAfterMenuItemAction}
                                onAfterInitialize={GridOnAfterInitialize}
                                onBeforeButtonAction={GridOnBeforeButtonAction}
                            />
                        )}
                    />
                </div>
            </div>

        </Page>
    );
}