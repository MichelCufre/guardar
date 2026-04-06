import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { Row, Col, Container } from 'react-bootstrap';

export default function PRE170(props) {

    const { t } = useTranslation();


    const [infoDisplayed, setInfoDisplayed] = useState({
        NU_PREPARACION: ""
    });

    const [isInfoDisplayed, setIsInfoDisplayed] = useState(false);

    const onAfterInitialize = (context, grid, parameters, nexus) => {

        if (parameters.find(d => d.id === "PRE170_NU_PREPARACION") != null) {

            setInfoDisplayed({
                NU_PREPARACION: parameters.find(d => d.id === "PRE170_NU_PREPARACION").value,
            });

            setIsInfoDisplayed(true);

        } else {

            setIsInfoDisplayed(false);
        }
    };

    return (

        <Page
            title={t("PRE170_Sec0_pageTitle_Titulo")}
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
                    <h4 className='form-title'>{t("PRE170_Sec0_logs_GrillaMercaderia")}</h4>
                        <Grid
                            id="PRE170_grid_1"
                            rowsToFetch={30}
                            rowsToDisplay={15}
                            enableExcelExport
                            onAfterInitialize={onAfterInitialize}
                        />
                </div>
            </div>

            <div className="row mb-4">
                <div className="col-12">
                    <h4 className='form-title'>{t("PRE170_Sec0_logs_GrillaLpn")}</h4>
                    <Grid
                        id="PRE170_grid_2"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        onAfterInitialize={onAfterInitialize}
                    />
                </div>
            </div>
        </Page>
    );
}