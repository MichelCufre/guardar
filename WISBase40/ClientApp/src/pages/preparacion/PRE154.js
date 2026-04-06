import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { Row, Col } from 'react-bootstrap';
import { Container } from 'react-bootstrap';

export default function PRE154(props) {
    const { t } = useTranslation();

    const [infoDisplay, setInfoDisplay] = useState({
        NU_DET_PED_SAI_ATRIB: "", 
        ID_LPN_EXTERNO: "", 
        TP_LPN_TIPO: "", 
    });

    const [isInfoDisplayed, setIsInfoDisplayed] = useState(false);

    const onAfterInitialize = (context, grid, parameters, nexus) => {

        if (parameters.find(s => s.id === "PRE154_NU_DET_PED_SAI_ATRIB") != null) {

            setInfoDisplay({
                NU_DET_PED_SAI_ATRIB: parameters.find(s => s.id === "PRE154_NU_DET_PED_SAI_ATRIB").value,
                ID_LPN_EXTERNO: parameters.find(s => s.id === "PRE154_ID_LPN_EXTERNO").value,
                TP_LPN_TIPO: parameters.find(s => s.id === "PRE154_TP_LPN_TIPO").value,
            });

            setIsInfoDisplayed(true);

        } else {
            setIsInfoDisplayed(false);
        }
    };

    return (

        <Page
            title={t("PRE154_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Container fluid style={{ display: isInfoDisplayed ? 'block' : 'none' }} >
                <Row>
                    <Col lg={4}>
                        <Row>
                            <Col lg={6}>
                                <span style={{ fontWeight: "bold" }}>{t("PRE154_Sec0_Info_Cabezal_NuDetalle")} </span>
                            </Col>
                            <Col lg={6}>
                                <span>{`${infoDisplay.NU_DET_PED_SAI_ATRIB}`}</span>
                            </Col>
                        </Row>

                    </Col>
                    <Col lg={4}>
                        <Row>
                            <Col lg={6}>
                                <span style={{ fontWeight: "bold" }}>{t("PRE154_Sec0_Info_Cabezal_IdExterno")} </span>
                            </Col>
                            <Col lg={6}>
                                <span> {`${infoDisplay.ID_LPN_EXTERNO}`}</span>
                            </Col>
                        </Row>
                    </Col>
                    <Col lg={4}>
                        <Row>
                            <Col lg={6}>
                                <span style={{ fontWeight: "bold" }}>{t("PRE154_Sec0_Info_Cabezal_Tipo")} </span>
                            </Col>
                            <Col lg={6}>
                                <span> {`${infoDisplay.TP_LPN_TIPO}`}</span>
                            </Col>
                        </Row>
                    </Col>
                </Row>
              
            </Container>
            
            <hr style={{ display: isInfoDisplayed ? 'block' : 'none' }}></hr>
            
            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="PRE154_grid_1" 
                    rowsToFetch={30} 
                    rowsToDisplay={15} 
                    enableExcelExport 
                    onAfterInitialize={onAfterInitialize} />
                </div>
            </div>
        </Page>
    );
}