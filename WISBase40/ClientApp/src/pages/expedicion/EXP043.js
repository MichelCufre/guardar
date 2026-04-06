import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Row, Col, FormGroup } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';

export default function EXP043(props) {

    const { t } = useTranslation();

    const [infoCamion, setInfoCamion] = useState({
        CD_CAMION: "", DS_CAMION: ""
    });
    const [isShowCamion, setIsShowCamion] = useState(false);

    const onAfterInitialize = (context, grid, parameters, nexus) => {

        if (parameters.find(x => x.id === "EXP043_CD_CAMION") != null) {

            setInfoCamion({
                CD_CAMION: parameters.find(d => d.id === "EXP043_CD_CAMION").value,
                DS_CAMION: parameters.find(d => d.id === "EXP043_DS_CAMION").value,
            });

            setIsShowCamion(true);

        }
    };

    return (
        <Page
            title={t("EXP043_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <container style={{ display: isShowCamion ? 'block' : 'none' }} fluid>
                <Row>
                    <Col>
                        <Row>
                            <Col sm={2}>
                                <span style={{ fontWeight: "bold" }}>{t("EXP043_Sec0_Info_Cabezal_Camion")} </span>
                            </Col>
                            <Col>
                                <span> {`${infoCamion.CD_CAMION}`} - {`${infoCamion.DS_CAMION}`} </span>
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
                <hr></hr>
            </container>

            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="EXP043_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                        onAfterInitialize={onAfterInitialize}
                    />
                </div>
            </div>
        </Page>
    );
}