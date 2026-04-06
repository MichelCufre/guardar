import React, { useRef, useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { Container, Row, Col } from 'react-bootstrap';

export default function PRE360(props) {

    const { t } = useTranslation();

    const [infoPickingMal, setInfoPickingMal] = useState({
        EMPRESA: "", UBICACION: "", PRODUCTO: ""
    });

    const [isInfoDisplayed, setIsInfoDisplayed] = useState(false);

    const onAfterInitialize = (context, grid, parameters, nexus) => {

        if (parameters.find(d => d.id === "PRE360_EMPRESA") != null) {

            setInfoPickingMal({
                EMPRESA: parameters.find(d => d.id === "PRE360_EMPRESA").value,
                UBICACION: parameters.find(d => d.id === "PRE360_UBICACION").value,
                PRODUCTO: parameters.find(d => d.id === "PRE360_PRODUCTO").value,
            });

            setIsInfoDisplayed(true);
        }

    };

    return (

        <Page
            title={t("PRE360_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Container fluid style={{ display: isInfoDisplayed ? 'block' : 'none' }}>
                <Row>
                    <Col>
                        <Row>
                            <Col sm={3}>
                                <span style={{ fontWeight: "bold" }}>{t("PRE360_frm1_lbl_UBICACION")}: </span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoPickingMal.UBICACION}`}</span>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={3}>
                                <span style={{ fontWeight: "bold" }}>{t("PRE360_frm1_lbl_PRODUCTO")}: </span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoPickingMal.PRODUCTO}`}</span>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={3}>
                                <span style={{ fontWeight: "bold" }}>{t("PRE360_frm1_lbl_EMPRESA")}: </span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoPickingMal.EMPRESA}`}</span>
                            </Col>
                        </Row>
                    </Col>
                </Row>
            </Container>
            <hr style={{ display: isInfoDisplayed ? 'block' : 'none' }}></hr>

            <div className="col-12">
                <Grid id="PRE360_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                    onAfterInitialize={onAfterInitialize} />
            </div>
        </Page>
    );
}