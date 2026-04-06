import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Row, Col } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { Container } from 'react-bootstrap';

export default function REC210(props) {
    const { t } = useTranslation();
    const [agenda, setAgenda] = useState(null);

    const [infoDetalle, setInfoDetalle] = useState({
        NU_AGENDA: "", CD_EMPRESA: "", NM_EMPRESA: "", NU_PREPARACION: ""
    });

    const [isInfoDisplayed, setIsInfoDisplayed] = useState(false);

    const onAfterInitialize = (context, grid, parameters, nexus) => {

        if (parameters.find(d => d.id === "REC210_NU_AGENDA") != null) {

            setInfoDetalle({
                NU_AGENDA: parameters.find(d => d.id === "REC210_NU_AGENDA").value,
                CD_EMPRESA: parameters.find(d => d.id === "REC210_CD_EMPRESA").value,
                NM_EMPRESA: parameters.find(d => d.id === "REC210_NM_EMPRESA").value,
                NU_PREPARACION: parameters.find(d => d.id === "REC210_NU_PREPARACION").value,
            });

            setIsInfoDisplayed(true);
        } else {

            setIsInfoDisplayed(false);
        }
    };

    return (
        <Page
            load
            title={t("REC210_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Container fluid style={{ display: isInfoDisplayed ? 'block' : 'none' }} >
                <Row>
                    <Col>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("REC210_Sec0_Info_Cabezal_Agenda")}: </span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoDetalle.NU_AGENDA}`}</span>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("REC210_Sec0_Info_Cabezal_Empresa")}: </span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoDetalle.CD_EMPRESA}`} - {`${infoDetalle.NM_EMPRESA}`}</span>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("REC210_Sec0_Info_Cabezal_Preparacion")}: </span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoDetalle.NU_PREPARACION}`}</span>
                            </Col>
                        </Row>
                    </Col>
                </Row>
            </Container>


            <hr style={{ display: isInfoDisplayed ? 'block' : 'none' }}></hr>

            <Grid
                id="REC210_grid_1"
                rowsToFetch={30}
                rowsToDisplay={15}
                enableExcelExport
                onAfterInitialize={onAfterInitialize}
            />
        </Page>
    );
}