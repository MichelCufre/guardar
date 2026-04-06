import React, { useRef, useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { Container, Row, Col } from 'react-bootstrap';

export default function PRE160(props) {

    const { t } = useTranslation();

    const [infoPreparacion, setInfoPreparacion] = useState({
        CD_EMPRESA: "", NM_EMPRESA: "", NU_PREPARACION: "", DS_PREPARACION: ""
    });

    const [isInfoPreparacionDisplayed, setIsInfoPreparacion] = useState(false);

    const onAfterInitialize = (context, grid, parameters, nexus) => {

        if (parameters.find(d => d.id === "PRE162_CD_EMPRESA") != null) {

            setInfoPreparacion({
                CD_EMPRESA: parameters.find(d => d.id === "PRE162_CD_EMPRESA").value,
                NM_EMPRESA: parameters.find(d => d.id === "PRE162_NM_EMPRESA").value,
                CD_PREPARACION: parameters.find(d => d.id === "PRE162_NU_PREPARACION").value,
                DS_PREPARACION: parameters.find(d => d.id === "PRE162_DS_PREPARACION").value
            });

            setIsInfoPreparacion(true);

        } else {

            setIsInfoPreparacion(false);
        }
    };

    return (

        <Page
            title={isInfoPreparacionDisplayed ? t("PRE162_Sec0_pageTitle_Titulo") : t("PRE162_Sec0_pageTitle_SinParametros")}
            {...props}

        >
            <Container fluid style={{ display: isInfoPreparacionDisplayed ? 'block' : 'none' }}>
                <h5 className='form-title'>{t("PRE162_Sec0_subTitle_Titulo")}</h5>
                <Row>
                    <Col >
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("PRE162_frm1_lbl_CD_EMPRESA")}</span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoPreparacion.CD_EMPRESA}`} - {`${infoPreparacion.NM_EMPRESA}`}</span>
                            </Col>
                        </Row>

                    </Col>
                    <Col>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("PRE162_frm1_lbl_NU_PREPARACION")}</span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoPreparacion.CD_PREPARACION}`} - {`${infoPreparacion.DS_PREPARACION}`}</span>
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
                    </Col>
                </Row>
                <br></br>
            </Container>
            <div className="col-12">
                <Grid id="PRE162_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport onAfterInitialize={onAfterInitialize}
                />
            </div>
        </Page>
    );
}