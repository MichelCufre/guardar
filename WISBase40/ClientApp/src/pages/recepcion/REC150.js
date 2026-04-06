import React, { useState, useRef } from 'react';
import { Page } from '../../components/Page';
import { Grid } from '../../components/GridComponents/Grid';
import { useTranslation } from 'react-i18next';
import { Modal, Container, Row, Col } from 'react-bootstrap';
import { IMP080ConsulEtiquetasModal } from '../impresion/IMP080ConsulEtiquetasModal';
import { IMP200ImpresionUTModal } from '../impresion/IMP200ImpresionUTModal'

export default function REC150(props) {

    const { t } = useTranslation();

    const [infoEtiqueta, setInfoEtiqueta] = useState({
        CD_PRODUCTO: "", DS_PRODUCTO: "", NU_AGENDA: "", NU_IDENTIFICADOR: "", CD_FAIXA: ""
    });

    const [infoAgenda, setInfoAgenda] = useState({
        NU_AGENDA: ""
    });

    const [isInfoEtiquetaDisplayed, setIsInfoEtiquetaDisplayed] = useState(false);

    const [isInfoAgenda, setIsInfoAgenda] = useState(false);

    const [showPopupImprimir, setShowPopupImprimir] = useState(false);
    const [showPopupImprimirUT, setShowPopupImprimirUT] = useState(false);

    const onAfterInitialize = (context, grid, parameters, nexus) => {

        if (parameters.find(d => d.id === "REC150_CD_PRODUCTO") != null) {
            setInfoEtiqueta({
                CD_PRODUCTO: parameters.find(d => d.id === "REC150_CD_PRODUCTO").value,
                NU_AGENDA: parameters.find(d => d.id === "REC150_NU_AGENDA").value,
                NU_IDENTIFICADOR: parameters.find(d => d.id === "REC150_NU_IDENTIFICADOR").value,
                CD_FAIXA: parameters.find(d => d.id === "REC150_CD_FAIXA").value,
            });
            setIsInfoEtiquetaDisplayed(true);
        }
        else if (parameters.find(d => d.id === "REC150_NU_AGENDA") != null) {
            setInfoAgenda({
                NU_AGENDA: parameters.find(d => d.id === "REC150_NU_AGENDA").value,
            });
            setIsInfoAgenda(true);
        }
    }

    const openImprimirDialog = () => {
        setShowPopupImprimir(true);
        setShowPopupImprimirUT(false);
    }

    const closeImprimirDialog = () => {
        setShowPopupImprimir(false);
        setShowPopupImprimirUT(false);

    }

    const openImprimirDialogUT = () => {
        setShowPopupImprimir(false);
        setShowPopupImprimirUT(true);
    }

    const closeImprimirDialogUT = () => {
        setShowPopupImprimir(false);
        setShowPopupImprimirUT(false);
    }

    return (
        <Page
            title={t("REC150_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <container style={{ display: isInfoEtiquetaDisplayed ? 'block' : 'none' }} fluid>
                <Row>
                    <Col>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("REC150_Sec0_Info_CodProd")} </span>
                            </Col>
                            <Col className='p-0'>
                                <span>  {`${infoEtiqueta.CD_PRODUCTO}`} </span>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("REC150_Sec0_Info_Agenda")} </span>
                            </Col>
                            <Col className='p-0'>
                                <span>  {`${infoEtiqueta.NU_AGENDA}`}</span>
                            </Col>
                        </Row>
                    </Col>
                    <Col>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("REC150_Sec0_Info_Identificador")} </span>
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
                                <span style={{ fontWeight: "bold" }}>{t("REC150_Sec0_Info_Embalaje")} </span>
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
                    <Col>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                    </Col>
                </Row>
            </container>
            <hr style={{ display: isInfoEtiquetaDisplayed ? 'block' : 'none' }}></hr>

            <container style={{ display: isInfoAgenda ? 'block' : 'none' }} fluid>
                <Row>
                    <Col>
                        <Row>
                            <Col sm={4}>
                                <span style={{ fontWeight: "bold" }}>{t("REC150_Sec0_Info_Agenda")} </span>
                            </Col>
                            <Col className='p-0'>
                                <span>  {`${infoAgenda.NU_AGENDA}`} </span>
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
                    <Col>
                        <Row>
                            <Col sm={4}>
                            </Col>
                            <Col className='p-0'>
                            </Col>
                        </Row>
                    </Col>
                </Row>
            </container>

            <div style={{ textAlign: "center" }}>
                <Row>
                    <Col>
                        <button className="btn btn-primary" onClick={openImprimirDialog}>{t("REC150_Sec0_btn_GenerarEtiqueta")}</button>
                        &nbsp;
                        <button className="btn btn-primary" onClick={openImprimirDialogUT}>{t("REC150_Sec0_btn_GenerarEtiquetaUT")}</button>
                    </Col>
                </Row>
            </div>
            <Grid
                id="REC150_grid_1"
                rowsToFetch={30}
                rowsToDisplay={15}
                enableExcelExport
                onAfterInitialize={onAfterInitialize}
            />

            <IMP080ConsulEtiquetasModal show={showPopupImprimir} onHide={closeImprimirDialog} />
            <IMP200ImpresionUTModal show={showPopupImprimirUT} onHide={closeImprimirDialogUT} selectedKeys={null} reimprimir={false} />
        </Page >
    );
}
