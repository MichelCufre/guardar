import React, { useState, useRef } from 'react';
import { Page } from '../../components/Page';
import { Grid } from '../../components/GridComponents/Grid';
import { useTranslation } from 'react-i18next';
import { Container, Row, Col } from 'react-bootstrap';

export default function REG603(props) {

    const { t } = useTranslation();

    const [infoCodBarra, setInfoCodBarra] = useState({
        NM_EMPRESA: "", CD_EMPRESA: "", CD_PRODUCTO: "", DS_PRODUCTO: ""
    });

    const [infoEmpresa, setInfoEmpresa] = useState({
        NM_EMPRESA: "", CD_EMPRESA: ""
    });


    const [isInfoCodBarraDisplayed, setInfoCodBarraDisplayed] = useState(false);

    const [isInfoEmpresaOnly, setIsInfoEmpresaOnly] = useState(false);


    const onAfterInitialize = (context, grid, parameters, nexus) => {


        if ((parameters.find(d => d.id === "REG603_CD_EMPRESA") != null) && (parameters.find(d => d.id === "REG603_CD_PRODUCTO") != null)) {

            setInfoCodBarra({
                CD_EMPRESA: parameters.find(d => d.id === "REG603_CD_EMPRESA").value,
                NM_EMPRESA: parameters.find(d => d.id === "REG603_NM_EMPRESA").value,
                CD_PRODUCTO: parameters.find(d => d.id === "REG603_CD_PRODUCTO").value,
                DS_PRODUCTO: parameters.find(d => d.id === "REG603_DS_PRODUCTO").value,
            });

            setInfoCodBarraDisplayed(true);

        } else if (parameters.find(d => d.id === "REG603_CD_EMPRESA") != null) {

            setInfoEmpresa({
                CD_EMPRESA: parameters.find(d => d.id === "REG603_CD_EMPRESA").value,
                NM_EMPRESA: parameters.find(d => d.id === "REG603_NM_EMPRESA").value,
            });

            setIsInfoEmpresaOnly(true);
            setInfoCodBarraDisplayed(false);

        } else {

            setInfoCodBarraDisplayed(false);
            setIsInfoEmpresaOnly(false);
        }
    };

    return (
        <Page
            title={t("REG603_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Container fluid style={{ display: isInfoCodBarraDisplayed || isInfoEmpresaOnly ? 'block' : 'none' }} >
                <Row>
                    <Col>
                        <Row>
                            <Col style={{ display: isInfoCodBarraDisplayed ? 'block' : 'none' }}>
                                <span style={{ fontWeight: "bold" }}>{t("REG603_Sec0_Info_Cabezal_Empresa")} </span>
                                <span> {`${infoCodBarra.CD_EMPRESA}`} - {`${infoCodBarra.NM_EMPRESA}`}</span>
                            </Col>
                            <Col style={{ display: isInfoEmpresaOnly ? 'block' : 'none' }}>
                                <span style={{ fontWeight: "bold", fontSize: '20px' }}>{t("REG603_Sec0_Info_Cabezal_Empresa")} </span>
                                <span style={{ fontSize: '15px' }}> {`${infoEmpresa.CD_EMPRESA}`} - {`${infoEmpresa.NM_EMPRESA}`}</span>
                            </Col>
                            <Col style={{ display: isInfoCodBarraDisplayed ? 'block' : 'none' }}>
                                <span style={{ fontWeight: "bold", }}>{t("REG603_Sec0_Info_Cabezal_Producto")} </span>
                                <span> {`${infoCodBarra.CD_PRODUCTO}`} - {`${infoCodBarra.DS_PRODUCTO}`} </span>
                            </Col>
                            <Col>
                            </Col>
                        </Row>
                    </Col>
                </Row>
            </Container>
            <hr style={{ display: isInfoCodBarraDisplayed || isInfoEmpresaOnly ? 'block' : 'none' }}></hr>

            <Grid
                id="REG603_grid_1"
                rowsToFetch={30}
                rowsToDisplay={15}
                enableExcelExport
                onAfterInitialize={onAfterInitialize}
            />
        </Page >
    );
}