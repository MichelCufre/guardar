import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { Row, Col } from 'react-bootstrap';
import { Container } from 'react-bootstrap';

export default function PRE150(props) {
    const { t } = useTranslation();

    const [infoDisplay, setInfoDisplay] = useState({
        CD_AGENTE: "", DS_AGENTE: "", DS_TIPO_AGENTE: "", CD_EMPRESA: "", NM_EMPRESA: "", NU_PEDIDO: "",
    });

    const [infoPedido, setInfoPedido] = useState(null);

    const [isInfoDisplayed, setIsInfoDisplayed] = useState(false);

    const onAfterInitialize = (context, grid, parameters, nexus) => {

        if (parameters.find(s => s.id === "PRE150_CD_AGENTE") != null) {

            setInfoDisplay({

                CD_AGENTE: parameters.find(s => s.id === "PRE150_CD_AGENTE").value,
                DS_AGENTE: parameters.find(s => s.id === "PRE150_DS_AGENTE").value,
                DS_TIPO_AGENTE: parameters.find(s => s.id === "PRE150_DS_TIPO_AGENTE").value,
                NM_EMPRESA: parameters.find(s => s.id === "PRE150_NM_EMPRESA").value,
                CD_EMPRESA: parameters.find(s => s.id === "PRE150_CD_EMPRESA").value,
                NU_PEDIDO: parameters.find(s => s.id === "PRE150_NU_PEDIDO").value

            });

            setIsInfoDisplayed(true);
        } else if (parameters.find(s => s.id === "PRE150_NU_PEDIDO") != null) {
            setInfoPedido(parameters.find(s => s.id === "PRE150_NU_PEDIDO").value);
        }


    };

    return (

        <Page
            title={t("PRE150_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Container fluid style={{ display: isInfoDisplayed ? 'block' : 'none' }} >
                <Row>
                    <Col lg={5}>
                        <Row>
                            <Col lg={2}>
                                <span style={{ fontWeight: "bold" }}>{t("PRE150_Sec0_Info_Cabezal_Pedido")} </span>
                            </Col>
                            <Col lg={10}>
                                <span>{`${infoDisplay.NU_PEDIDO}`}</span>
                            </Col>
                        </Row>
                    </Col>
                    <Col lg={3}>
                        <Row>
                            <Col lg={4}>
                                <span style={{ fontWeight: "bold" }}>{t("PRE150_Sec0_Info_Cabezal_Empresa")} </span>
                            </Col>
                            <Col lg={6}>
                                <span> {`${infoDisplay.CD_EMPRESA}`} - {`${infoDisplay.NM_EMPRESA}`}</span>
                            </Col>
                        </Row>
                    </Col>
                    <Col lg={4}>
                        <Row>
                            <Col lg={4}>
                                <span style={{ fontWeight: "bold" }}>{t("PRE150_Sec0_Info_Cabezal_Agente")} </span>
                            </Col>
                            <Col lg={8}>
                                <span> {`${infoDisplay.DS_TIPO_AGENTE}`} - {`${infoDisplay.CD_AGENTE}`} - {`${infoDisplay.DS_AGENTE}`}  </span>
                            </Col>
                        </Row>
                    </Col>

                </Row>
            </Container>

            <hr style={{ display: isInfoDisplayed ? 'block' : 'none' }}></hr>

            <Grid
                id="PRE150_grid_1"
                rowsToFetch={30}
                rowsToDisplay={15}
                enableExcelExport
                onAfterInitialize={onAfterInitialize}
            />

        </Page>
    );
}