import React, { useState, useEffect } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { Container, Row, Col } from 'react-bootstrap';

export default function PRE351(props) {
    const { t } = useTranslation();
    const [cdEmpresa, setCdEmpresa] = useState(null);
    const [cdProducto, setCdProducto] = useState(null);
    const [cdEnderecoPicking, setCdEnderecoPicking] = useState(null);
    const [cdFaixa, setCdFaixa] = useState(null);
    const [isInfoDisplayed, setInfoDisplayed] = useState(null);

    useEffect(() => {
        setInfoDisplayed(true);
    }, []);

    const onAfterInitialize = (context, grid, parameters, nexus) => {
        parameters.length > 0 ? setInfoDisplayed(true) : null;
        parameters.find(s => s.id === "empresa") ? setCdEmpresa(parameters.find(s => s.id === "empresa").value) : null;
        parameters.find(s => s.id === "producto") ? setCdProducto(parameters.find(s => s.id === "producto").value) : null;
        parameters.find(s => s.id === "ubicacionPicking") ? setCdEnderecoPicking(parameters.find(s => s.id === "ubicacionPicking").value) : null;
        parameters.find(s => s.id === "faixa") ? setCdFaixa(parameters.find(s => s.id === "faixa").value) : null; 
    }
    return (

        <Page
            title={t("PRE351_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Container fluid style={{ display: isInfoDisplayed ? 'block' : 'none' }}>
                <Row>
                    <Col>
                        <Row>
                            <Col sm={3}>
                                <span style={{ fontWeight: "bold" }}>{t("PRE350_frm1_lbl_CD_EMPRESA")}:</span>
                            </Col>
                            <Col className='p-0'>
                                <span>{cdEmpresa}</span>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={3}>
                                <span style={{ fontWeight: "bold" }}>{t("PRE350_frm1_lbl_CD_PRODUCTO")}:</span>
                            </Col>
                            <Col className='p-0'>
                                <span>{cdProducto}</span>
                            </Col>
                        </Row>
                    </Col>
                </Row>
                <Row>
                    <Col>
                        <Row>
                            <Col sm={3}>
                                <span style={{ fontWeight: "bold" }}>{t("PRE350_frm1_lbl_CD_ENDERECO_PICKING")}:</span>
                            </Col>
                            <Col className='p-0'>
                                <span>{cdEnderecoPicking}</span>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={3}>
                                <span style={{ fontWeight: "bold" }}>{t("PRE350_frm1_lbl_CD_FAIXA")}:</span>
                            </Col>
                            <Col className='p-0'>
                                <span>{cdFaixa}</span>
                            </Col>
                        </Row>
                    </Col>
                </Row>
            </Container>
            <hr></hr>
            <div className="row mb-4 mt-2">
                <div className="col-12">
                    <Grid
                        id="PRE351_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        onAfterInitialize={onAfterInitialize}
                        enableExcelExport
                    />
                </div>
            </div>
        </Page>
    );
}