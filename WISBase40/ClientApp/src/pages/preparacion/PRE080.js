import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Modal, Row, Col, Container } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';

export default function PRE080(props) {

    const { t } = useTranslation();
    const [isInfoDisplayed, setIsInfoDisplayed] = useState(false);

    const [infoDisplayed, setInfoDisplayed] = useState({
        NU_PREPARACION: ""
    });

    const onAfterInitialize = (context, grid, parameters, nexus) => {

        if (parameters.find(d => d.id === "PRE080_NU_PREPARACION") != null) {

            setInfoDisplayed({
                NU_PREPARACION: parameters.find(d => d.id === "PRE080_NU_PREPARACION").value,
            });

            setIsInfoDisplayed(true);

        } else {

            setIsInfoDisplayed(false);
        }
    };

    return (
        <Page
            title={t("PRE080_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Container fluid style={{ display: isInfoDisplayed ? 'block' : 'none' }}>
                <Row>
                    <Col >
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("PRE170_frm1_lbl_NU_PREPARACION")} :</span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoDisplayed.NU_PREPARACION}`} </span>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                    </Col>
                    <Col>
                    </Col>
                </Row>
            </Container>

            <hr style={{ display: isInfoDisplayed ? 'block' : 'none' }}></hr>

            <div className="row mb-4">
                <div className="col-12">
                    <Grid
                        id="PRE080_grid_1"
                        application='PRE080'
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        enableExcelImport={false}
                        onAfterInitialize={onAfterInitialize}
                    />
                </div>
            </div>
        </Page>
    );
}
