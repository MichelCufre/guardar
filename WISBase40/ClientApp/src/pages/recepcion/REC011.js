import React, { useState } from 'react';
import { Page } from '../../components/Page';
import { Grid } from '../../components/GridComponents/Grid';
import { useTranslation } from 'react-i18next';
import { Row, Col } from 'react-bootstrap';
import { Container } from 'react-bootstrap';

export default function REC011(props) {

    const { t } = useTranslation();

    const [infoDetalle, setInfoDetalle] = useState({
        NU_REFERENCIA: "", DS_ESTADO_REFERENCIA: "", CD_EMPRESA: "", NM_EMPRESA: "", CD_AGENTE: "", DS_TIPO_AGENTE: "", DS_AGENTE: "", DS_TIPO_REFERENCIA: "", TP_REFERENCIA: ""
    });

    const [isInfoDetalleDisplayed, setIsInfoDetalleDisplayed] = useState(false);

    const onAfterInitialize = (context, grid, parameters, nexus) => {

        if (parameters.find(d => d.id === "REC011_NU_REFERENCIA") != null) {

            setInfoDetalle({
                NU_REFERENCIA: parameters.find(d => d.id === "REC011_NU_REFERENCIA").value,
                DS_ESTADO_REFERENCIA: parameters.find(d => d.id === "REC011_DS_ESTADO_REFERENCIA").value,
                DS_TIPO_REFERENCIA: parameters.find(d => d.id === "REC011_DS_TIPO_REFERENCIA").value,
                TP_REFERENCIA: parameters.find(d => d.id === "REC011_TP_REFERENCIA").value,
                CD_AGENTE: parameters.find(d => d.id === "REC011_CD_AGENTE").value,
                DS_AGENTE: parameters.find(d => d.id === "REC011_DS_AGENTE").value,
                DS_TIPO_AGENTE: parameters.find(d => d.id === "REC011_DS_TIPO_AGENTE").value,
                CD_EMPRESA: parameters.find(d => d.id === "REC011_CD_EMPRESA").value,
                NM_EMPRESA: parameters.find(d => d.id === "REC011_NM_EMPRESA").value,
            });

            setIsInfoDetalleDisplayed(true);

        } else {

            setIsInfoDetalleDisplayed(false);
        }
    };

    return (
        <Page
            title={t("REC011_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Container fluid style={{ display: isInfoDetalleDisplayed ? 'block' : 'none' }} >
                <Row>
                    <Col>
                        <Row>
                            <Col sm={3}>
                                <span style={{ fontWeight: "bold" }}>{t("REC011_Sec0_Info_Cabezal_Referencia")} : </span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoDetalle.NU_REFERENCIA}`}</span>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={3} >
                                <span style={{ fontWeight: "bold" }}>{t("REC011_Sec0_Info_Cabezal_Tp_Referencia")} : </span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoDetalle.TP_REFERENCIA}`} - {`${infoDetalle.DS_TIPO_REFERENCIA}`}</span>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={3}>
                                <span style={{ fontWeight: "bold" }}>{t("REC011_Sec0_Info_Cabezal_Estado_Ref")} : </span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoDetalle.DS_ESTADO_REFERENCIA}`}</span>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={3}>
                                <span style={{ fontWeight: "bold" }}>{t("REC011_Sec0_Info_Cabezal_Empresa")} : </span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoDetalle.CD_EMPRESA}`} - {`${infoDetalle.NM_EMPRESA}`}</span>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={3}>
                                <span style={{ fontWeight: "bold" }}>{t("REC011_Sec0_Info_Cabezal_Agente")} : </span>
                            </Col>
                            <Col className='p-0'>
                                <span>{`${infoDetalle.DS_TIPO_AGENTE}`} - {`${infoDetalle.CD_AGENTE}`} - {`${infoDetalle.DS_AGENTE}`}</span>
                            </Col>
                        </Row>
                    </Col>
                </Row>
                <hr></hr>
            </Container>

            <Grid
                id="REC011_grid_1"
                rowsToFetch={30}
                rowsToDisplay={15}
                enableExcelExport
                onAfterInitialize={onAfterInitialize}
            />
        </Page>
    );
}
