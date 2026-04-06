import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Modal, Row, Col, Container } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';

export default function PRE220(props) {
    const { t } = useTranslation();

    const [infoDemora, setInfoDemora] = useState({
        fechaInicio: "", fechaFin: "", demora: ""
    });

    const onAfterInitialize = (context, form, query, nexus) => {
        actualizarDemora(query);
    };

    const onAfterFetch = (context, newRows, parameters, nexus) => {
        actualizarDemora(parameters);
    };

    const onAfterApplyFilter = (context, form, query, nexus) => {
        actualizarDemora(query.parameters);
    };

    const actualizarDemora = (parameters) => {
        if (parameters && parameters.find(f => f.id === "fechaInicio") !== undefined) {
            var diferenciaDias = parameters.find(d => d.id === "diferenciaDias").value;
            var diferenciaHoras = parameters.find(d => d.id === "diferenciaHoras").value;
            var diferenciaMinutos = parameters.find(d => d.id === "diferenciaMinutos").value;

            var fechaInicio = parameters.find(d => d.id === "fechaInicio").value;
            var fechaFin = parameters.find(d => d.id === "fechaFin").value;

            setInfoDemora({
                fechaInicio: fechaInicio ? fechaInicio : " ",
                fechaFin: fechaFin ? fechaFin : " ",
                demora: diferenciaDias + " " + t("PRE220_frm1_lbl_Dias") + "." + diferenciaHoras + " " + t("PRE220_frm1_lbl_Horas") + "." + diferenciaMinutos + " " + t("PRE220_frm1_lbl_Minutos"),
            });
        }
    };

    return (

        <Page
            title={t("PRE220_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Container fluid>
                <Row>
                    <Col>
                        <Row>
                            <Col>
                                <span style={{ fontWeight: "bold" }}>{t("PRE220_Sec0_Info_Cabezal_Inicio")}: </span>
                                <span> {`${infoDemora.fechaInicio}`} </span>
                            </Col>
                            <Col>
                                <span style={{ fontWeight: "bold", }}>{t("PRE220_Sec0_Info_Cabezal_Fin")}: </span>
                                <span> {`${infoDemora.fechaFin}`} </span>
                            </Col>
                            <Col>
                                <span style={{ fontWeight: "bold", }}>{t("PRE220_Sec0_Info_Cabezal_Demora")}: </span>
                                <span> {`${infoDemora.demora}`} </span>
                            </Col>
                            <Col>
                            </Col>
                        </Row>
                    </Col>
                </Row>
            </Container>
            <hr />
            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="PRE220_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        onAfterInitialize={onAfterInitialize}
                        onAfterFetch={onAfterFetch}
                        onAfterApplyFilter={onAfterApplyFilter}
                    />
                </div>
            </div>
        </Page>
    );
}