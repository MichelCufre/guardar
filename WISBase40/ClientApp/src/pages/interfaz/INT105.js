import React, { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { Page } from '../../components/Page';
import { Grid } from '../../components/GridComponents/Grid';
import { Container, Row, Col } from 'react-bootstrap';

export default function INT105(props) {
    const { t } = useTranslation();
    const [nuInterfaz, setNuInterfazEjecucion] = useState(null);
    const [referencia, setReferencia] = useState('');

    const onBeforeInitialize = (context, data, nexus) => {
        if (data.parameters.length > 0) {
            setNuInterfazEjecucion(data.parameters.find(e => e.id === "interfaz").value);
            // setReferencia(data.parameters.find(e => e.id === "referencia").value);
        }
    }

    return (
        <Page
            title={t("INT105_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Container fluid>
                <Row>
                    <Col>
                        <Row>
                            <Col sm={1}>
                                <span style={{ fontWeight: "bold" }}>{t("INT_lbl_NU_INTERFAZ_EJECUCION")}:</span>
                            </Col>
                            <Col className='p-0'>
                                <span>{nuInterfaz}</span>
                            </Col>
                        </Row>
                    </Col>
                    {/* <Col>
                        <Row>
                            <Col sm={3}>
                                <span style={{ fontWeight: "bold" }}>{t("INT100_frm1_lbl_DS_REFERENCIA")}:</span>
                            </Col>
                            <Col className='p-0'>
                                <span>{referencia}</span>
                            </Col>
                        </Row>
                    </Col> */}
                </Row>
            </Container>
            <hr></hr>

            <Grid
                id="INT105_grid_1"
                rowsToFetch={30}
                rowsToDisplay={15}
                enableExcelExport
                onBeforeInitialize={onBeforeInitialize}
            />
        </Page>
    )
}