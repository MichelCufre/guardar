import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { Row, Col, Container } from 'react-bootstrap';

export default function PRE061(props) {

    const { t } = useTranslation();

    const [infoContenedor, setInfoContenedor] = useState({
        NU_PREPARACION: "", NU_CONTENEDOR: "", UBICACION: "", PUERTA: "", SITUACION: "", SUB_CLASE: "", CD_CAMION: "", ID_EXTERNO_CONTENEDOR: ""
    });

    const [isInfoContenedorDisplayed, setIsInfoContenedorDisplayed] = useState(false);

    const onAfterInitialize = (context, grid, parameters, nexus) => {
        if (parameters.find(d => d.id === "PRE061_PREPARACION") != null) {
            setInfoContenedor({
                NU_PREPARACION: parameters.find(d => d.id === "PRE061_PREPARACION").value,
                NU_CONTENEDOR: parameters.find(d => d.id === "PRE061_CONTENEDOR").value,
                UBICACION: parameters.find(d => d.id === "PRE061_UBICACION").value,
                PUERTA: parameters.find(d => d.id === "PRE061_PUERTA").value,
                SITUACION: parameters.find(d => d.id === "PRE061_SITUACION").value,
                SUB_CLASE: parameters.find(d => d.id === "PRE061_SUB_CLASE").value,
                CD_CAMION: parameters.find(d => d.id === "PRE061_CD_CAMION").value,
                ID_EXTERNO_CONTENEDOR: parameters.find(d => d.id === "PRE061_ID_EXTERNO_CONTENEDOR").value
            });

            setIsInfoContenedorDisplayed(true);

        } else {

            setIsInfoContenedorDisplayed(false);
        }
    }
    return (

        <Page
            title={t("PRE061_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Container fluid style={{ display: isInfoContenedorDisplayed ? 'block' : 'none' }} >
                <Row>
                    <Col>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("PRE061_Sec0_lbl_frm_NU_PREPARACION")} :</span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoContenedor.NU_PREPARACION}`}</span>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("PRE061_Sec0_lbl_frm_NU_CONTENEDOR")} :</span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoContenedor.NU_CONTENEDOR}`} </span>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("PRE061_Sec0_lbl_frm_ID_EXTERNO_CONTENEDOR")} :</span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoContenedor.ID_EXTERNO_CONTENEDOR}`}</span>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("PRE061_Sec0_lbl_frm_CD_UBICACION")} :</span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoContenedor.UBICACION}`}</span>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("PRE061_Sec0_lbl_frm_CD_SITUACION")} :</span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoContenedor.SITUACION}`}</span>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("PRE061_Sec0_lbl_frm_CD_SUB_CLASE")} :</span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoContenedor.SUB_CLASE}`}</span>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={3}>
                                <span style={{ fontWeight: "bold" }}>{t("PRE061_Sec0_lbl_frm_CD_CAMION")} :</span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoContenedor.CD_CAMION}`}</span>
                            </Col>
                        </Row>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("PRE061_Sec0_lbl_frm_CD_PUERTA")} :</span>
                            </Col>
                            <Col className='p-0'>
                                <span> {`${infoContenedor.PUERTA}`}</span>
                            </Col>
                        </Row>
                    </Col>
                </Row>
            </Container>
            <hr style={{ display: isInfoContenedorDisplayed ? 'block' : 'none' }}></hr>

            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="PRE061_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                        onAfterInitialize={onAfterInitialize}
                    />
                </div>
            </div>
        </Page>
    );
}