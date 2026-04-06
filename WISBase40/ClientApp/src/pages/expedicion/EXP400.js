import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import Badge from 'react-bootstrap/Badge';
import Table from 'react-bootstrap/Table';
import { Container, Col, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';

export default function EXP400(props) {
    const { t } = useTranslation();
    const [nuContenedor, setNuContenedor] = useState(null);
    const [nuPreparacion, setNuPreparacion] = useState(null);
    const [cdCamion, setCdCamion] = useState(null);

    const onAfterPageLoad = (data) => {
        setNuContenedor(data.parameters.find(d => d.id === "contenedor").value);
        setNuPreparacion(data.parameters.find(d => d.id === "preparacion").value);
        setCdCamion(data.parameters.find(d => d.id === "camion").value);
    };

    const addParameters = (context, data, nexus) => {
        if ((data.parameters.length > 1) && (data.parameters.find(d => d.id === "contenedor").value == null)) {

            data.parameters = [
                {
                    id: "contenedor",
                    value: nuContenedor || "0"
                },
                {
                    id: "preparacion",
                    value: nuPreparacion || "0"
                },
                {
                    id: "camion",
                    value: cdCamion || "0"
                }
            ];
        }
    };


    return (

        <Page
            title={t("EXP400_Sec0_pageTitle_Titulo")}
            {...props}
            load
            onAfterLoad={onAfterPageLoad}
        >
            <Container fluid>
                <Row>
                    <Col className="d-flex justify-content-center">
                        <h5><span class="badge" style={{ backgroundColor: "#F97568", color: "black" }}>{t("EXP400_frm1_lbl_ContenedorCompartido")}</span></h5>

                    </Col>
                    <Col className="d-flex justify-content-center">
                        <h5><span class="badge" style={{ backgroundColor: "#F4F968", color: "black" }}>{t("EXP400_frm1_lbl_CargaSinAsociar")}</span></h5>
                    </Col>
                </Row>
            </Container>
            <hr></hr>

            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="EXP400_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                        onBeforeInitialize={addParameters}
                        onBeforeFetch={addParameters}
                        onBeforeApplyFilter={addParameters}
                        onBeforeApplySort={addParameters}
                    />
                </div>
            </div>
        </Page>
    );
}