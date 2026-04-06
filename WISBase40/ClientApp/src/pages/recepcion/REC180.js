import React, { useState, useRef } from 'react';
import { Page } from '../../components/Page';
import { Grid } from '../../components/GridComponents/Grid';
import { useTranslation } from 'react-i18next';
import { Container, Col, Row } from 'react-bootstrap';

export default function REC180(props) {

    const { t } = useTranslation();
    const [infoEtiqueta, setInfoEtiqueta] = useState({
        CD_PRODUCTO: "", NU_ETIQUETA_LOTE: "", NU_IDENTIFICADOR: "", CD_FAIXA: "", NU_EXTERNO_ETIQUETA:""
    });

    const [isInfoEtiquetaDisplayed, setIsInfoEtiquetaDisplayed] = useState(false);

    const onAfterInitialize = (context, grid, parameters, nexus) => {

        if (parameters.find(d => d.id === "REC180_CD_PRODUCTO") != null) {
            setInfoEtiqueta({
                CD_PRODUCTO: parameters.find(d => d.id === "REC180_CD_PRODUCTO").value,
                NU_ETIQUETA_LOTE: parameters.find(d => d.id === "REC180_NU_ETIQUETA_LOTE").value,
                NU_IDENTIFICADOR: parameters.find(d => d.id === "REC180_NU_IDENTIFICADOR").value,
                CD_FAIXA: parameters.find(d => d.id === "REC180_CD_FAIXA").value,
                NU_EXTERNO_ETIQUETA: parameters.find(d => d.id === "REC180_NU_EXTERNO_ETIQUETA").value,
            });
            setIsInfoEtiquetaDisplayed(true);

        } else {
            setIsInfoEtiquetaDisplayed(false);
        }
    }

    return (
        <Page
            title={t("REC180_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <container style={{ display: isInfoEtiquetaDisplayed ? 'block' : 'none' }} fluid>
                <Row>
                    <Col>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("REC180_Sec0_Info_Etiqueta_lote")} </span>
                            </Col>
                            <Col className='p-0'>
                                <span>  {`${infoEtiqueta.NU_ETIQUETA_LOTE}`}</span>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("REC180_Sec0_Info_CodProd")} </span>
                            </Col>
                            <Col className='p-0'>
                                <span>  {`${infoEtiqueta.CD_PRODUCTO}`} </span>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("REC180_Sec0_Info_Identificador")} </span>
                            </Col>
                            <Col className='p-0'>
                                <span>  {`${infoEtiqueta.NU_IDENTIFICADOR}`}</span>
                            </Col>
                        </Row>
                    </Col>
                </Row>
                <Row>
                    <Col>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("REC180_Sec0_Info_NumeroExterno")} </span>
                            </Col>
                            <Col className='p-0'>
                                <span>  {`${infoEtiqueta.NU_EXTERNO_ETIQUETA}`}</span>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("REC180_Sec0_Info_Embalaje")} </span>
                            </Col>
                            <Col className='p-0'>
                                <span>  {`${infoEtiqueta.CD_FAIXA}`}</span>
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
                <hr></hr>
            </container>
            <Grid
                id="REC180_grid_1"
                rowsToFetch={30}
                rowsToDisplay={15}
                enableExcelExport
                onAfterInitialize={onAfterInitialize}
            />
        </Page>
    );
}
