import React, { useRef, useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { Container, Row, Col } from 'react-bootstrap';

export default function PRE161(props) {

    const { t } = useTranslation();

    const [infoPreparacion, setInfoPreparacion] = useState({
        CD_EMPRESA: "", NM_EMPRESA: ""
    });

    const [isInfoPreparacionDisplayed, setIsInfoPreparacion] = useState(false);

    const onAfterInitialize = (context, grid, parameters, nexus) => {

        if (parameters.find(d => d.id === "PRE161_CD_EMPRESA") != null) {

            setInfoPreparacion({
                CD_EMPRESA: parameters.find(d => d.id === "PRE161_CD_EMPRESA").value,
                NM_EMPRESA: parameters.find(d => d.id === "PRE161_NM_EMPRESA").value,
            });

            setIsInfoPreparacion(true);

        } else {

            setIsInfoPreparacion(false);
        }

    };

    return (

        <Page
            title={isInfoPreparacionDisplayed ? t("PRE161_Sec0_pageTitle_Titulo") : t("PRE161_Sec0_subTitle_SinEmpresa")}
            {...props}
        >
            <Container fluid style={{ display: isInfoPreparacionDisplayed ? 'block' : 'none' }}>

                <h5 className='form-title'>{t("PRE161_Sec0_subTitle_Titulo")}</h5>
                <Row>
                    <Col>
                        <Row>
                            <Col sm={3}>
                                <span style={{ fontWeight: "bold" }}>{t("PRE161_frm1_lbl_CD_EMPRESA")}</span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoPreparacion.CD_EMPRESA}`} - {`${infoPreparacion.NM_EMPRESA}`}</span>
                            </Col>

                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col>
                                <span></span>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col>
                                <span></span>
                            </Col>
                        </Row>
                    </Col>
                </Row>
            </Container>
            <hr style={{ display: isInfoPreparacionDisplayed ? 'block' : 'none' }}></hr>
            <div className="col-12">
                <Grid id="PRE161_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                    onAfterInitialize={onAfterInitialize} />
            </div>
        </Page>
    );
}