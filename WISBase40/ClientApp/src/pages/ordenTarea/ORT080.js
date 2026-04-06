import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Modal, Row, Col, Container } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';


export default function ORT080(props) {

    const { t } = useTranslation();

    const [infoOrden, setinfoOrden] = useState({
        numeroOrden: "", descripcionOrden: "", codigoTarea: "", descripcionTarea: "",
        codigoEmpresa: "", descripcionEmpresa: ""
    });

    const [isinfoOrdenDisplayed, setinfoOrdenDisplayed] = useState(false);

    const [modoLectura, setModoLectura] = useState(false);

    const onAfterInitialize = (context, grid, parameters, nexus) => {

        if ((parameters.find(d => d.id === "numeroOrden") != null)) {
            setinfoOrden({
                numeroOrden: parameters.find(d => d.id === "numeroOrden").value,
                descripcionOrden: parameters.find(d => d.id === "descripcionOrden").value,
                codigoTarea: parameters.find(d => d.id === "codigoTarea").value,
                descripcionTarea: parameters.find(d => d.id === "descripcionTarea").value,
                codigoEmpresa: parameters.find(d => d.id === "codigoEmpresa").value,
                descripcionEmpresa: parameters.find(d => d.id === "descripcionEmpresa").value,
                fechaInicioOrden: parameters.find(d => d.id === "fechaInicioOrden").value
            });

            setinfoOrdenDisplayed(true);

        } else {
            setinfoOrdenDisplayed(false);
        }

        if (parameters.find(x => x.id === "ModoLectura") != null) {
            setModoLectura(true);
        }

    };

    const applyParameters = (context, data, nexus) => {
        let parameters =
            [
                { id: "fechaInicioOrden", value: infoOrden.fechaInicioOrden }
            ];

        data.parameters = parameters;
    }


    return (
        <Page
            title={t("ORT080_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Container fluid style={{ display: isinfoOrdenDisplayed ? 'block' : 'none' }} >
                <Row style={{ marginBottom: '2vh' }}>
                    <Col>
                        <Row>
                            <Col>
                                <span style={{ fontWeight: "bold" }}>{t("ORT040_Sec0_Info_Cabezal_Orden")}: </span>
                                <span> {`${infoOrden.numeroOrden ? infoOrden.numeroOrden : "-"}`} </span>
                            </Col>
                            <Col>
                                <span style={{ fontWeight: "bold", }}>{t("ORT060_Sec0_Info_Cabezal_codigoTarea")}: </span>
                                <span> {`${infoOrden.codigoTarea ? infoOrden.codigoTarea : "-"}`}  </span>
                            </Col>
                            <Col>
                                <span style={{ fontWeight: "bold", }}>{t("ORT060_Sec0_Info_Cabezal_codigoEmpresa")}: </span>
                                <span> {`${infoOrden.codigoEmpresa ? infoOrden.codigoEmpresa : "-"}`}  </span>
                            </Col>
                        </Row>
                        <Row>
                            <Col>
                                <span style={{ fontWeight: "bold", }}>{t("ORT040_Sec0_Info_Cabezal_DescripcionOrden")}: </span>
                                <span> {`${infoOrden.descripcionOrden ? infoOrden.descripcionOrden : "-"}`}  </span>
                            </Col>
                            <Col>
                                <span style={{ fontWeight: "bold", }}>{t("ORT060_Sec0_Info_Cabezal_descripcionTarea")}: </span>
                                <span> {`${infoOrden.descripcionTarea ? infoOrden.descripcionTarea : "-"}`}</span>
                            </Col>
                            <Col>
                                <span style={{ fontWeight: "bold", }}>{t("ORT060_Sec0_Info_Cabezal_descripcionEmpresa")}: </span>
                                <span> {`${infoOrden.descripcionEmpresa ? infoOrden.descripcionEmpresa : "-"}`}</span>
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
                    <Grid id="ORT080_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        onAfterInitialize={onAfterInitialize}
                        enableExcelExport={true} 
                        onBeforeValidateRow={applyParameters}
                    />
                </div>
            </div>

        </Page>
    );
}