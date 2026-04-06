import React, { useState, useRef } from 'react';
import { Page } from '../../components/Page';
import { Grid } from '../../components/GridComponents/Grid';
import { useTranslation } from 'react-i18next';
import { Container, Row, Col } from 'react-bootstrap';


export default function REG602(props) {

    const { t } = useTranslation();

    const [infoControl, setInfoControl] = useState({
        NM_EMPRESA: "", CD_EMPRESA: "", CD_PRODUCTO: "", DS_PRODUCTO: ""
    });

    const [infoControlDisplayed, setInfoControlDisplayed] = useState(false);

    const onAfterInitialize = (context, grid, parameters, nexus) => {


        if ((parameters.find(d => d.id === "REG602_CD_EMPRESA") != null) && (parameters.find(d => d.id === "REG602_CD_PRODUCTO") != null)) {

            setInfoControl({
                CD_EMPRESA: parameters.find(d => d.id === "REG602_CD_EMPRESA").value,
                NM_EMPRESA: parameters.find(d => d.id === "REG602_NM_EMPRESA").value,
                CD_PRODUCTO: parameters.find(d => d.id === "REG602_CD_PRODUCTO").value,
                DS_PRODUCTO: parameters.find(d => d.id === "REG602_DS_PRODUCTO").value,
            });

            setInfoControlDisplayed(true);

        } else {

            setInfoControlDisplayed(false);
        }
    };
    const handleBeforeImportExcel = (context, data, nexus) => {

        data.parameters = [
            { id: "importExcel", value: "true" }
        ];

    }

    return (
        <Page
            title={t("REG602_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Container style={{ display: infoControlDisplayed ? 'block' : 'none' }} fluid>
                <Row>
                    <Col>
                        <Row>
                            <Col sm={2}>
                                <span style={{ fontWeight: "bold" }}>{t("REG603_Sec0_Info_Cabezal_Empresa")}: </span>
                            </Col>
                            <Col sm={10}>
                                <span> {`${infoControl.CD_EMPRESA}`} - {`${infoControl.NM_EMPRESA}`}</span>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={2}>
                                <span style={{ fontWeight: "bold" }}>{t("REG603_Sec0_Info_Cabezal_Producto")}: </span>
                            </Col>
                            <Col sm={10}>
                                <span> {`${infoControl.CD_PRODUCTO}`} - {`${infoControl.DS_PRODUCTO}`} </span>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={2}>
                            </Col>
                            <Col sm={10}>
                            </Col>
                        </Row>
                    </Col>
                </Row>
                <hr></hr>
            </Container>
            <Grid
                id="REG602_grid_1"
                rowsToFetch={30}
                rowsToDisplay={15}
                enableExcelExport
                onAfterInitialize={onAfterInitialize}
                onBeforeImportExcel={handleBeforeImportExcel}
                onBeforeGenerateExcelTemplate={handleBeforeImportExcel}
            />
        </Page>
    );
}