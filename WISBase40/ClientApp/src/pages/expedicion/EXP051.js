import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Row, Col, FormGroup, Container } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { ro } from 'date-fns/esm/locale';
import './EXP051.css';

export default function EXP051(props) {

    const { t } = useTranslation();

    const [infoCamion, setInfoCamion] = useState({
        CAMION: "", PEDIDO: ""
    });

    const [isShowCamion, setIsShowCamion] = useState(false);

    const onAfterInitialize = (context, grid, parameters, nexus) => {

        if (parameters.find(x => x.id === "EXP051_CD_CAMION") != null) {

            setInfoCamion({
                CAMION: parameters.find(d => d.id === "EXP051_CD_CAMION").value,
                PEDIDO: parameters.find(d => d.id === "EXP051_NU_PEDIDO").value,
            });

            setIsShowCamion(true);

        } else {
            setIsShowCamion(false);
        }
    };

    return (
        <Page
            title={t("EXP051_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Container fluid>
                <Row className="d-flex justify-content-center">
                    <Col className="d-flex justify-content-center label-warning">
                        <div className="btn btn-danger btn-sm">{t("EXP051_grid_lbl_pendLiberar")}</div>
                    </Col>
                    <Col className="d-flex justify-content-center">
                        <div className="btn btn-danger btn-sm">{t("EXP051_grid_lbl_pendAsignar")}</div>
                    </Col>
                </Row>
            </Container>
            <hr></hr>
            <container style={{ display: isShowCamion ? 'block' : 'none' }} fluid>
                <Row>
                    <Col>
                        <Row>
                            <Col sm={2}>
                                <span style={{ fontWeight: "bold" }}>{t("EXP051_Sec0_Info_Cabezal_Pedido")} </span>
                            </Col>
                            <Col className="p-0">
                                <span> {`${infoCamion.PEDIDO}`}</span>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={2}>
                                <span style={{ fontWeight: "bold" }}>{t("EXP051_Sec0_Info_Cabezal_Camion")} </span>
                            </Col>
                            <Col className="p-0">
                                <span> {`${infoCamion.CAMION}`}</span>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={2}>
                                <span></span>
                            </Col>
                            <Col>
                                <span></span>
                            </Col>
                        </Row>
                    </Col>
                </Row>
            </container>
            <hr style={{ display: isShowCamion ? 'block' : 'none' }} ></hr>

            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="EXP051_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                        onAfterInitialize={onAfterInitialize}
                    />
                </div>
            </div>
        </Page>
    );
}