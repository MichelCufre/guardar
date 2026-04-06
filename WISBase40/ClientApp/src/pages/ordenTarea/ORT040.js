import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Modal, Row, Col, Container } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';


export default function ORT040(props) {

    const { t } = useTranslation();

    const [infoOrden, setinfoOrden] = useState({
        numeroOrden: "", descripcionOrden: "", fechaInicio: "", fechaFin: "",
        UltimaOperacion: ""
    });
    
    const [isinfoOrdenDisplayed, setinfoOrdenDisplayed] = useState(false);

    const [modoLectura, setModoLectura] = useState(false);

    const onAfterInitialize = (context, grid, parameters, nexus) => {


        if ((parameters.find(d => d.id === "numeroOrden") != null)) {

            setinfoOrden({
                numeroOrden: parameters.find(d => d.id === "numeroOrden").value,
                descripcionOrden: parameters.find(d => d.id === "descripcionOrden").value,
                fechaInicio: parameters.find(d => d.id === "fechaInicio").value,
                fechaFin: parameters.find(d => d.id === "fechaFin").value,
                UltimaOperacion: parameters.find(d => d.id === "UltimaOperacion").value,
            });

            setinfoOrdenDisplayed(true);

        }else{
            setinfoOrdenDisplayed(false);
        }

        if (parameters.find(x => x.id === "ModoLectura") != null) {
            setModoLectura(true);
        }

    };

    const applyParameters = (context, data, nexus) => {
        let parameters =
            [
                { id: "fechaInicioOrden", value: infoOrden.fechaInicio }
            ];

        data.parameters = parameters;
    }
    
    return (
        <Page
            title={t("ORT040_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Container fluid style={{ display: isinfoOrdenDisplayed ? 'block' : 'none' }} >
                <Row style={{ marginBottom: '2vh' }}>
                    <Col>
                        <Row>
                            <Col>
                                <span style={{ fontWeight: "bold" }}>{t("ORT040_Sec0_Info_Cabezal_Orden")}: </span>
                                <span>{`${infoOrden.numeroOrden ? infoOrden.numeroOrden : "-"}`}</span>
                            </Col>
                            <Col>
                                <span style={{ fontWeight: "bold", }}>{t("ORT040_Sec0_Info_Cabezal_DescripcionOrden")}: </span>
                                <span> {`${infoOrden.descripcionOrden ? infoOrden.descripcionOrden : "-"}`}  </span>
                            </Col>
                            <Col>
                                <span style={{ fontWeight: "bold", }}>{t("ORT040_Sec0_Info_Cabezal_FechaInicio")}: </span>
                                <span> {`${infoOrden.fechaInicio ? infoOrden.fechaInicio : "-"}`}  </span>
                            </Col>
                        </Row>
                        <Row>
                            <Col>
                                <span style={{ fontWeight: "bold" }}>{t("ORT040_Sec0_Info_Cabezal_FechaFin")}: </span>
                                <span> {`${infoOrden.fechaFin ? infoOrden.fechaFin : "-"}`}</span>
                            </Col>
                            <Col>
                                <span style={{ fontWeight: "bold", }}>{t("ORT040_Sec0_Info_Cabezal_UltimaOperacion")}: </span>
                                <span> {`${infoOrden.UltimaOperacion ? infoOrden.UltimaOperacion : "-"}`}</span>
                            </Col>
                            <Col>
                                <span style={{ fontWeight: "bold", }}>{t("ORT040_Sec0_Info_Cabezal_Tipo")}: </span>
                                <span> {`${infoOrden.tipo ? infoOrden.tipo : "-"}`}</span>
                            </Col>
                        </Row>
                        <br />
                        <Row style={{ marginBottom: '2vh', display: modoLectura ? 'block' : 'none', backgroundColor: 'red' }}>
                            <Col>
                                <span style={{ display: 'flex', justifyContent: 'center', color: 'white', fontWeight: 'bold' }}>{t("FAC010_frm1_msg_ModoLectura")}</span>
                            </Col>
                        </Row>
                    </Col>
                </Row>
            </Container>
            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="ORT040_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        onAfterInitialize={onAfterInitialize}
                        enableExcelExport={true}
                        onBeforeButtonAction={applyParameters}
                        onAfterButtonAction={applyParameters}
                    />
                </div>
            </div>

        </Page>
    );
}