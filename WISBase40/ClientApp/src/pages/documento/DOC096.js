import React, { useState, useRef } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { Row, Col } from 'react-bootstrap';
import { getDateTimeString } from '../../components/DateTimeUtil';

export default function DOC096(props) {
    const { t } = useTranslation();

    const [documento, setDocumento] = useState({});
    const [isGridIngresoVisible, setGridIngresoVisibility] = useState(false);
    const [isGridEgresoVisible, setGridEgresoVisibility] = useState(false);
    const [isGridLogVisible, setGridLogVisibility] = useState(false);

    const GridOnAfterInitialize = (context, form, parameters) => {
        const mostrarGridEgreso = parameters.find(p => p.id === "mostrarGridEgreso");
        const mostrarGridIngreso = parameters.find(p => p.id === "mostrarGridIngreso");
        const mostrarLogIngreso = parameters.find(p => p.id === "mostrarGridLog");


        if (mostrarGridEgreso && mostrarGridEgreso.value === "true")
            setGridEgresoVisibility(true);

        if (mostrarGridIngreso && mostrarGridIngreso.value === "true")
            setGridIngresoVisibility(true);

        if (mostrarLogIngreso && mostrarLogIngreso.value === "true")
            setGridLogVisibility(true);
    };

    const onAfterPageLoad = (data) => {
        if (data && data.parameters) {

            let documento = data.parameters.reduce((doc, param) => { doc[param.id] = param.value || "-"; return doc; }, {});

            documento.DT_ADDROW = documento.DT_ADDROW && documento.DT_ADDROW !== "-" ? getDateTimeString(new Date(documento.DT_ADDROW)) : documento.DT_ADDROW;
            documento.DT_UPDROW = documento.DT_UPDROW && documento.DT_UPDROW !== "-" ? getDateTimeString(new Date(documento.DT_UPDROW)) : documento.DT_UPDROW;
            documento.DT_PROGRAMADO = documento.DT_PROGRAMADO && documento.DT_PROGRAMADO !== "-" ? getDateTimeString(new Date(documento.DT_PROGRAMADO)) : documento.DT_PROGRAMADO;
            documento.DT_ENVIADO = documento.DT_ENVIADO && documento.DT_ENVIADO !== "-" ? getDateTimeString(new Date(documento.DT_ENVIADO)) : documento.DT_ENVIADO;

            setDocumento(documento);
        }
    };

    const gridIngresoClassName = `row ${isGridIngresoVisible ? "" : "hidden"}`;
    const gridEgresoClassName = `row ${isGridEgresoVisible ? "" : "hidden"}`;
    const gridLogClassName = `row ${isGridLogVisible ? "" : "hidden"}`;

    return (
        <Page
            load
            icon="fas fa-copy"
            title={t("DOC096_Sec0_pageTitle_Titulo")}
            onAfterLoad={onAfterPageLoad}
            {...props}
        >
            <Row>
                <Col>
                    <h1 className="mb-3">{documento.NU_DOCUMENTO}</h1>
                </Col>
            </Row>
            <Row>
                <Col>
                    <Row>
                        <Col lg={6}>
                            <h4 className="form-title">{t("DOC096_frm1_lbl_legend1")}</h4>
                            <Row className="mb-2">
                                <Col>
                                    <span className="text-muted">{t("DOC096_frm1_lbl_CD_EMPRESA")} </span>
                                    <span>{`${documento.CD_EMPRESA} ${documento.NM_EMPRESA === undefined ? '' : '-' + documento.NM_EMPRESA}`}</span>
                                </Col>
                            </Row>
                            <Row className="mb-2">
                                <Col>
                                    <span className="text-muted">{t("DOC096_frm1_lbl_ID_ESTADO")} </span>
                                    <span>{`${documento.ID_ESTADO === undefined ? '-' : documento.ID_ESTADO}`}</span>
                                </Col>
                            </Row>
                            <Row className="mb-2">
                                <Col>
                                    <span className="text-muted">{t("DOC096_frm1_lbl_NU_AGENDA")} </span>
                                    <span>{`${documento.NU_AGENDA === undefined ? '-' : documento.NU_AGENDA}`}</span>
                                </Col>
                            </Row>
                            <Row className="mb-2">
                                <Col>
                                    <span className="text-muted">{t("DOC096_frm1_lbl_NU_FACTURA")} </span>
                                    <span>{`${documento.NU_FACTURA === undefined ? '-' : documento.NU_FACTURA}`}</span>
                                </Col>
                            </Row>
                            <Row className="mb-2">
                                <Col>
                                    <span className="text-muted">{t("DOC096_frm1_lbl_TP_DUA")} </span>
                                    <span>{`${documento.INFO_DUA === undefined ? '-' : documento.INFO_DUA}`}</span>
                                </Col>
                            </Row>
                            <Row className="mb-2">
                                <Col>
                                    <span className="text-muted">{t("DOC096_frm1_lbl_TP_DTI")} </span>
                                    <span>{`${documento.INFO_DTI === undefined ? '-' : documento.INFO_DTI}`}</span>
                                </Col>
                            </Row>
                            <Row className="mb-2">
                                <Col>
                                    <span className="text-muted">{t("DOC096_frm1_lbl_REFERENCIA_EXTERNA")} </span>
                                    <span>{`${documento.INFO_REFERENCIA_EXTERNA === undefined ? '-' : documento.INFO_REFERENCIA_EXTERNA}`}</span>
                                </Col>
                            </Row>
                        </Col>
                        <Col lg={6}>
                            <h4 className="form-title">{t("DOC096_frm1_lbl_legend2")}</h4>
                            <Row className="mb-2">
                                <Col>
                                    <span className="text-muted">{t("DOC096_frm1_lbl_CD_MONEDA")} </span>
                                    <span>{`${documento.CD_MONEDA === undefined ? '-' : documento.CD_MONEDA}`}</span>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <span className="text-muted">{t("DOC096_frm1_lbl_VL_ARBITRAJE")} </span>
                                    <span>{`${documento.VL_ARBITRAJE === undefined ? '-' : documento.VL_ARBITRAJE}`}</span>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <span className="text-muted">{t("DOC096_frm1_lbl_QT_BULTO")} </span>
                                    <span>{`${documento.QT_BULTO === undefined ? '-' : documento.QT_BULTO}`}</span>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <span className="text-muted">{t("DOC096_frm1_lbl_VL_SEGURO")} </span>
                                    <span>{`${documento.VL_SEGURO === undefined ? '-' : documento.VL_SEGURO}`}</span>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <span className="text-muted">{t("DOC096_frm1_lbl_VL_FLETE")} </span>
                                    <span>{`${documento.VL_FLETE === undefined ? '-' : documento.VL_FLETE}`}</span>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <span className="text-muted">{t("DOC096_frm1_lbl_VL_OTROS_GASTOS")} </span>
                                    <span>{`${documento.VL_OTROS_GASTOS === undefined ? '-' : documento.VL_OTROS_GASTOS}`}</span>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <span className="text-muted">{t("DOC096_frm1_lbl_CD_UNIDAD_MEDIDA_BULTO")} </span>
                                    <span>{`${documento.CD_UNIDAD_MEDIDA_BULTO === undefined ? '-' : documento.CD_UNIDAD_MEDIDA_BULTO}`}</span>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <span className="text-muted">{t("DOC096_frm1_lbl_VL_NU_CONOCIMIENTO")} </span>
                                    <span>{`${documento.CD_UNIDAD_MEDIDA_BULTO === undefined ? '-' : documento.CD_UNIDAD_MEDIDA_BULTO}`}</span>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <span className="text-muted">{t("DOC096_frm1_lbl_VL_NU_IMPORT")} </span>
                                    <span>{`${documento.NU_IMPORT === undefined ? '-' : documento.NU_IMPORT}`}</span>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <span className="text-muted">{t("DOC096_frm1_lbl_VL_NU_EXPORT")} </span>
                                    <span>{`${documento.NU_EXPORT === undefined ? '-' : documento.NU_EXPORT}`}</span>
                                </Col>
                            </Row>
                        </Col>
                    </Row>
                </Col>
                <Col>
                    <Row className="mb-3">
                        <Col>
                            <h4 className="form-title">{t("DOC096_frm1_lbl_legend3")}</h4>
                            <Row>
                                <Col>
                                    <span className="text-muted">{t("DOC096_frm1_lbl_DT_PROGRAMADO")} </span>
                                    <span>{`${documento.DT_PROGRAMADO === undefined ? '-' : documento.DT_PROGRAMADO}`}</span>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <span className="text-muted">{t("DOC096_frm1_lbl_DT_ENVIADO")} </span>
                                    <span>{`${documento.DT_ENVIADO === undefined ? '-' : documento.DT_ENVIADO}`}</span>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <span className="text-muted">{t("DOC096_frm1_lbl_DT_ADDROW")} </span>
                                    <span>{`${documento.DT_ADDROW === undefined ? '-' : documento.DT_ADDROW}`}</span>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <span className="text-muted">{t("DOC096_frm1_lbl_DT_UPDROW")} </span>
                                    <span>{`${documento.DT_UPDROW === undefined ? '-' : documento.DT_UPDROW}`}</span>
                                </Col>
                            </Row>
                        </Col>
                    </Row>
                    <Row className="mb-3">
                        <Col>
                            <h4 className="form-title">{t("DOC096_frm1_lbl_legend4")}</h4>
                            <Row>
                                <Col>
                                    <span className="text-muted">{t("DOC096_frm1_lbl_TP_ALMACENAJE")} </span>
                                    <span>{`${documento.TP_ALMACENAJE === undefined ? '-' : documento.TP_ALMACENAJE}`}</span>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <span className="text-muted">{t("DOC096_frm1_lbl_NU_PREDIO")} </span>
                                    <span>{`${documento.NU_PREDIO === undefined ? '-' : documento.NU_PREDIO}`}</span>
                                </Col>
                            </Row>
                        </Col>
                    </Row>
                    <Row className="mb-3">
                        <Col>
                            <h4 className="form-title">{t("DOC096_frm1_lbl_legend5")}</h4>
                            <Row>
                                <Col>
                                    <span className="text-muted">{t("DOC096_frm1_lbl_CD_FUNCIONARIO")} </span>
                                    <span>{`${documento.CD_FUNCIONARIO === undefined ? '-' : documento.CD_FUNCIONARIO}`}</span>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <span className="text-muted">{t("DOC096_frm1_lbl_CD_VIA")} </span>
                                    <span>{`${documento.CD_VIA === undefined ? '-' : documento.CD_VIA}`}</span>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <span className="text-muted">{t("DOC096_frm1_lbl_CD_DESPACHANTE")} </span>
                                    <span>{`${documento.CD_DESPACHANTE === undefined ? '-' : documento.CD_DESPACHANTE}`}</span>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <span className="text-muted">{t("DOC096_frm1_lbl_NU_DOC_TRANSPORTE")} </span>
                                    <span>{`${documento.NU_DOC_TRANSPORTE === undefined ? '-' : documento.NU_DOC_TRANSPORTE}`}</span>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <span className="text-muted">{t("DOC096_frm1_lbl_CD_CAMION")} </span>
                                    <span>{`${documento.CD_CAMION === undefined ? '-' : documento.CD_CAMION}`}</span>
                                </Col>
                            </Row>
                        </Col>
                    </Row>
                </Col>
            </Row>
            <hr />
            <div className={gridIngresoClassName}>
                <div className="col-12">
                    <h2>{t("DOC096_frm1_lbl_legend6")}</h2>
                    <Grid
                        id="DOC096_grid_I"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        onAfterInitialize={GridOnAfterInitialize}
                        enableExcelExport
                    />
                </div>
            </div>
            <div className={gridEgresoClassName}>
                <div className="col-12">
                    <h2>{t("DOC096_frm1_lbl_legend7")}</h2>
                    <Grid
                        id="DOC096_grid_E"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        onAfterInitialize={GridOnAfterInitialize}
                        enableExcelExport
                    />
                </div>
            </div>
            <div className={gridLogClassName}>
                <div className="col-12">
                    <h2>{t("DOC096_frm1_lbl_legend8")}</h2>
                    <Grid
                        id="DOC096_grid_L"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        onAfterInitialize={GridOnAfterInitialize}
                        enableExcelExport
                    />
                </div>
            </div>
        </Page>
    );
}