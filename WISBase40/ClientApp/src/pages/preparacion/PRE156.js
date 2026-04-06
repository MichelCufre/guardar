import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { Row, Col } from 'react-bootstrap';
import { Container } from 'react-bootstrap';

export default function PRE156(props) {
    const { t } = useTranslation();

    const [infoDisplay, setInfoDisplay] = useState({
        NU_DET_PED_SAI_ATRIB: "",
        CD_EMPRESA: "",
        NM_EMPRESA: "",
        NU_PEDIDO: "",
        CD_PRODUTO: "",
        DS_PRODUTO: "",
    });

    const [isInfoDisplayed, setIsInfoDisplayed] = useState(false);

    const onAfterInitialize = (context, grid, parameters, nexus) => {

        if (parameters.find(s => s.id === "PRE156_NU_DET_PED_SAI_ATRIB") != null) {

            setInfoDisplay({
                NU_DET_PED_SAI_ATRIB: parameters.find(s => s.id === "PRE156_NU_DET_PED_SAI_ATRIB").value,
                CD_EMPRESA: parameters.find(s => s.id === "PRE156_CD_EMPRESA").value,
                NM_EMPRESA: parameters.find(s => s.id === "PRE156_NM_EMPRESA").value,
                NU_PEDIDO: parameters.find(s => s.id === "PRE156_NU_PEDIDO").value,
                CD_PRODUTO: parameters.find(s => s.id === "PRE156_CD_PRODUTO").value,
                DS_PRODUTO: parameters.find(s => s.id === "PRE156_DS_PRODUTO").value,

            });

            setIsInfoDisplayed(true);

        } else {
            setIsInfoDisplayed(false);
        }
    };

    return (

        <Page
            title={t("PRE156_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Container fluid style={{ display: isInfoDisplayed ? 'block' : 'none' }} >
                <Row>
                    <Col lg={4}>
                        <Row>
                            <Col lg={6}>
                                <span style={{ fontWeight: "bold" }}>{t("PRE156_Sec0_Info_Cabezal_Pedido")} </span>
                            </Col>
                            <Col lg={6}>
                                <span>{`${infoDisplay.NU_PEDIDO}`}</span>
                            </Col>
                        </Row>

                    </Col>
                    <Col lg={4}>
                        <Row>
                            <Col lg={6}>
                                <span style={{ fontWeight: "bold" }}>{t("PRE156_Sec0_Info_Cabezal_Empresa")} </span>
                            </Col>
                            <Col lg={6}>
                                <span> {`${infoDisplay.CD_EMPRESA}`} - {`${infoDisplay.NM_EMPRESA}`}</span>
                            </Col>
                        </Row>
                    </Col>
                    <Col lg={4}>
                        <Row>
                            <Col lg={6}>
                                <span style={{ fontWeight: "bold" }}>{t("PRE156_Sec0_Info_Cabezal_Producto")} </span>
                            </Col>
                            <Col lg={6}>
                                <span> {`${infoDisplay.CD_PRODUTO}`} - {`${infoDisplay.DS_PRODUTO}`} </span>
                            </Col>
                        </Row>
                    </Col>
                </Row>
                <Row>
                    <Col lg={4}>
                        <Row>
                            <Col lg={6}>
                                <span style={{ fontWeight: "bold" }}>{t("PRE156_Sec0_Info_Cabezal_Id")} </span>
                            </Col>
                            <Col lg={6}>
                                <span>{`${infoDisplay.NU_DET_PED_SAI_ATRIB}`}</span>
                            </Col>
                        </Row>
                    </Col>
                </Row>
            </Container>

            <hr style={{ display: isInfoDisplayed ? 'block' : 'none' }}></hr>

            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="PRE156_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        onAfterInitialize={onAfterInitialize} />
                </div>
            </div>
        </Page>
    );
}