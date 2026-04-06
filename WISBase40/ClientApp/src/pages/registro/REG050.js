import React, { useState, useRef } from 'react';
import { Page } from '../../components/Page';
import { Grid } from '../../components/GridComponents/Grid';
import { useTranslation } from 'react-i18next';
import { Container, Row, Col } from 'react-bootstrap';
import { useToaster } from '../../components/ToasterHook';
import { withPageContext } from '../../components/WithPageContext';

export default function REG050(props) {

    const { t } = useTranslation();
    const toaster = useToaster();

    const [infoPickProd, setInfoPickProd] = useState({
        NM_EMPRESA: "", CD_EMPRESA: "", CD_PRODUCTO: "", DS_PRODUCTO: ""
    });


    const [isInfoPickProdDisplayed, setInfoPickProdDisplayed] = useState(false);

    const onAfterInitialize = (context, grid, parameters, nexus) => {


        if ((parameters.find(d => d.id === "REG050_CD_EMPRESA") != null) && (parameters.find(d => d.id === "REG050_CD_PRODUCTO") != null)) {

            setInfoPickProd({
                CD_EMPRESA: parameters.find(d => d.id === "REG050_CD_EMPRESA").value,
                NM_EMPRESA: parameters.find(d => d.id === "REG050_NM_EMPRESA").value,
                CD_PRODUCTO: parameters.find(d => d.id === "REG050_CD_PRODUCTO").value,
                DS_PRODUCTO: parameters.find(d => d.id === "REG050_DS_PRODUCTO").value,
            });

            setInfoPickProdDisplayed(true);

        } else {

            setInfoPickProdDisplayed(false);
        }
    };

    return (
        <Page
            title={t("REG050_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Container fluid style={{ display: isInfoPickProdDisplayed ? 'block' : 'none' }} >
                <Row>
                    <Col>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("REG050_Sec0_Info_Cabezal_Empresa")} : </span>
                                <span> {`${infoPickProd.CD_EMPRESA}`} - {`${infoPickProd.NM_EMPRESA}`}</span>
                            </Col>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold", }}>{t("REG050_Sec0_Info_Cabezal_Producto")} : </span>
                                <span> {`${infoPickProd.CD_PRODUCTO}`} - {`${infoPickProd.DS_PRODUCTO}`} </span>
                            </Col>
                            <Col>
                            </Col>
                        </Row>
                    </Col>
                </Row>
            </Container>
            <hr style={{ display: isInfoPickProdDisplayed ? 'block' : 'none' }}></hr>

            <Grid
                id="REG050_grid_1"
                rowsToFetch={30}
                rowsToDisplay={15}
                enableExcelExport
                onAfterInitialize={onAfterInitialize}
            />
        </Page >
    );
}