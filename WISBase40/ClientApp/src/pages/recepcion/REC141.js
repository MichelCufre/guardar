import React, { useState, useRef } from 'react';
import { Page } from '../../components/Page';
import { Grid } from '../../components/GridComponents/Grid';
import { useTranslation } from 'react-i18next';
import { Container, Row, Col } from 'react-bootstrap';
import './REC141.css';

export default function REC141(props) {

    const { t } = useTranslation();

    const [infoEtiqueta, setInfoEtiqueta] = useState({
        CD_PRODUCTO: "", DS_PRODUCTO: "", NU_AGENDA: "", NU_IDENTIFICADOR: "", CD_FAIXA: ""
    });

    const [infoAgenda, setInfoAgenda] = useState({
        NU_AGENDA:""
    });

    const [isInfoEtiquetaDisplayed, setIsInfoEtiquetaDisplayed] = useState(false);
    const [isInfoAgendaDisplayed, setIsInfoAgendaDisplayed] = useState(false);

    const onAfterInitialize = (context, grid, parameters, nexus) => {

        if (parameters.find(d => d.id === "REC141_CD_PRODUCTO") != null) {

            setInfoEtiqueta({
                CD_PRODUCTO: parameters.find(d => d.id === "REC141_CD_PRODUCTO").value,
                NU_AGENDA: parameters.find(d => d.id === "REC141_NU_AGENDA").value,
                NU_IDENTIFICADOR: parameters.find(d => d.id === "REC141_NU_IDENTIFICADOR").value,
                CD_FAIXA: parameters.find(d => d.id === "REC141_CD_FAIXA").value,
            });

            setIsInfoEtiquetaDisplayed(true);

            setIsInfoAgendaDisplayed(false);

        } else if (parameters.find(d => d.id === "REC141_NU_AGENDA") != null) {

            setInfoAgenda({
                NU_AGENDA: parameters.find(d => d.id === "REC141_NU_AGENDA").value
            });

            setIsInfoAgendaDisplayed(true);

            setIsInfoEtiquetaDisplayed(false);

        } else {
            setIsInfoAgendaDisplayed(false);

            setIsInfoEtiquetaDisplayed(false);
        }
    }

    const handleAfterMenuItemAction = (context, data, nexus) => {        
        nexus.getGrid("REC141_grid_1").refresh();
    };

    return (
        <Page
            title={t("REC141_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <container style={{ display: isInfoEtiquetaDisplayed ? 'block' : 'none' }} fluid>
                <Row>
                    <Col>
                        <Row>
                            <Col sm={3}>
                                <span style={{ fontWeight: "bold" }}>{t("REC141_Sec0_Info_CodProd")} : </span>
                            </Col>
                            <Col className='p-0'>
                                <span>  {`${infoEtiqueta.CD_PRODUCTO}`} </span>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={3}>
                                <span style={{ fontWeight: "bold" }}>{t("REC141_Sec0_Info_Agenda")} : </span>
                            </Col>
                            <Col className='p-0'>
                                <span>  {`${infoEtiqueta.NU_AGENDA}`}</span>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("REC141_Sec0_Info_Identificador")} : </span>
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
                            <Col sm={3}>
                                <span style={{ fontWeight: "bold" }}>{t("REC141_Sec0_Info_Embalaje")} : </span>
                            </Col>
                            <Col className='p-0'>
                                <span>  {`${infoEtiqueta.CD_FAIXA}`}</span>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={3}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={3}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                    </Col>
                </Row>
                <hr></hr>
            </container>

            <container style={{ display: isInfoAgendaDisplayed ? 'block' : 'none' }} fluid>
                <Row>
                    <Col>
                        <Row>
                            <Col sm={3}>
                                <span style={{ fontWeight: "bold" }}>{t("REC141_Sec0_Info_Agenda")} : </span>
                            </Col>
                            <Col sm={8}>
                                <span>  {`${infoAgenda.NU_AGENDA}`}</span>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={3}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={3}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                    </Col>
                </Row>
                <hr></hr>
            </container>
            <Grid
                id="REC141_grid_1"
                rowsToFetch={30}
                rowsToDisplay={15}
                enableExcelExport
                onAfterInitialize={onAfterInitialize}
                onAfterMenuItemAction={handleAfterMenuItemAction}
                enableSelection
            />
        </Page>
    );
}
