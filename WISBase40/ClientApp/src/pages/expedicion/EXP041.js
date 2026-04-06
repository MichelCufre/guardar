import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Row, Col, FormGroup } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { ro } from 'date-fns/esm/locale';

export default function EXP041(props) {

    const { t } = useTranslation();

    const [infoCamion, setInfoCamion] = useState({
        CD_CAMION: "", DS_CAMION:""
    });
    const [isShowCamion, setIsShowCamion] = useState(false);

    const [infoEmpPedProd, setInfoEmpPedProd] = useState({
        CD_EMPRESA: "", NM_EMPRESA: "", CD_PRODUTO: "", DS_PRODUTO: "", NU_PEDIDO: ""
    });

    const [isShowTripleInfo, setIsShowTripleInfo] = useState(false);

    const [cdCamion, setCdCamion] = useState({});

    const onAfterInitialize = (context, grid, parameters, nexus) => {

        if (parameters.find(x => x.id === "EXP041_CD_CAMION") != null) {

            setInfoCamion({
                CD_CAMION: parameters.find(d => d.id === "EXP041_CD_CAMION").value,
                DS_CAMION: parameters.find(d => d.id === "EXP041_DS_CAMION").value,
            });

            setIsShowCamion(true);

        } else if (parameters.find(x => x.id === "EXP041_CD_EMPRESA") != null) {

            setInfoEmpPedProd({
                CD_EMPRESA: parameters.find(d => d.id === "EXP041_CD_EMPRESA").value,
                NM_EMPRESA: parameters.find(d => d.id === "EXP041_NM_EMPRESA").value,
                CD_PRODUTO: parameters.find(d => d.id === "EXP041_CD_PRODUTO").value,
                DS_PRODUTO: parameters.find(d => d.id === "EXP041_DS_PRODUTO").value,
                NU_PEDIDO: parameters.find(d => d.id === "EXP041_NU_PEDIDO").value

            });

            setIsShowTripleInfo(true);
        }
    };

    return (
        <Page
            title={t("EXP041_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <container style={{ display: isShowTripleInfo ? 'block' : 'none' }} fluid>
                <Row>
                    <Col>
                        <Row>
                            <Col sm={2}>
                                <span style={{ fontWeight: "bold" }}>{t("EXP041_Sec0_Info_Cabezal_Pedido")} </span>
                            </Col>
                            <Col>
                                <span> {`${infoEmpPedProd.NU_PEDIDO}`}</span>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={2}>
                                <span style={{ fontWeight: "bold" }}>{t("EXP041_Sec0_Info_Cabezal_Empresa")} </span>
                            </Col>
                            <Col>
                                <span> {`${infoEmpPedProd.CD_EMPRESA}`} - {`${infoEmpPedProd.NM_EMPRESA}`}</span>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={2}>
                                <span style={{ fontWeight: "bold" }}>{t("EXP041_Sec0_Info_Cabezal_Producto")} </span>
                            </Col>
                            <Col>
                                <span> {`${infoEmpPedProd.CD_PRODUTO}`} - {`${infoEmpPedProd.DS_PRODUTO}`}</span>
                            </Col>
                        </Row>
                    </Col>
                </Row>
                <hr></hr>
            </container>
            <container style={{ display: isShowCamion ? 'block' : 'none' }} fluid>
                <Row>
                    <Col>
                        <Row>
                            <Col sm={2}>
                                <span style={{ fontWeight: "bold" }}>{t("EXP041_Sec0_Info_Cabezal_Camion")} </span>
                            </Col>
                            <Col>
                                <span> {`${infoCamion.CD_CAMION}`} - {`${infoCamion.DS_CAMION}`} </span>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={2} >
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
                    <Grid id="EXP041_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                        onAfterInitialize={onAfterInitialize}
                    />
                </div>
            </div>
        </Page>
    );
}