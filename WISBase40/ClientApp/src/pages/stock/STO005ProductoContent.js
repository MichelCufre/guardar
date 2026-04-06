import React from 'react';
import { Row, Col } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';

export function STO005ProductoContent(props) {
    const { t } = useTranslation();

    if (!props.stockGeneral.CantidadFisica)
        return null;

    return (
        <React.Fragment>
            <Row className="mb-4">
                <Col>
                    <Row>
                        <Col>
                            <span className="text-muted">{t("STO005_frm1_lbl_Situacion")}</span>
                        </Col>
                    </Row>
                    <Row>
                        <Col>{props.stockGeneral.Situacion}</Col>
                    </Row>
                </Col>
            </Row>
            <Row className="mb-4">
                <Col>
                    <Row>
                        <Col>
                            <span className="text-muted">{t("STO005_frm1_lbl_CodigoMercadologico")}</span>
                        </Col>
                    </Row>
                    <Row>
                        <Col>{props.stockGeneral.CodigoMercadologico}</Col>
                    </Row>
                </Col>
                <Col>
                    <Row>
                        <Col>
                            <span className="text-muted">{t("STO005_frm1_lbl_CodigoProdEmpresa")}</span>
                        </Col>
                    </Row>
                    <Row>
                        <Col>{props.stockGeneral.CodigoProductoEmpresa}</Col>
                    </Row>
                </Col>
                <Col>
                    <Row>
                        <Col>
                            <span className="text-muted">{t("STO005_frm1_lbl_DescripcionReducida")}</span>
                        </Col>
                    </Row>
                    <Row>
                        <Col>{props.stockGeneral.DescripcionReducida}</Col>
                    </Row>
                </Col>
            </Row>
            <Row className="mb-4">
                <Col>
                    <Row>
                        <Col>
                            <span className="text-muted">{t("STO005_frm1_lbl_Clase")}</span>
                        </Col>
                    </Row>
                    <Row>
                        <Col>{props.stockGeneral.Clase.Id} - {props.stockGeneral.Clase.Descripcion}</Col>
                    </Row>
                </Col>
                <Col>
                    <Row>
                        <Col>
                            <span className="text-muted">{t("STO005_frm1_lbl_Familia")}</span>
                        </Col>
                    </Row>
                    <Row>
                        <Col>{props.stockGeneral.Familia.Id} - {props.stockGeneral.Familia.Descripcion}</Col>
                    </Row>
                </Col>
                <Col>
                    <Row>
                        <Col>
                            <span className="text-muted">{t("STO005_frm1_lbl_Rotatividad")}</span>
                        </Col>
                    </Row>
                    <Row>
                        <Col>{props.stockGeneral.Rotatividad.Id} - {props.stockGeneral.Rotatividad.Descripcion}</Col>
                    </Row>
                </Col>
            </Row>
            <Row className="mb-4">
                <Col>
                    <Row>
                        <Col>
                            <span className="text-muted">{t("STO005_frm1_lbl_TransitoEntrada")}</span>
                        </Col>
                    </Row>
                    <Row>
                        <Col>{props.stockGeneral.CantidadEntrada}</Col>
                    </Row>
                </Col>
                <Col>
                    <Row>
                        <Col>
                            <span className="text-muted">{t("STO005_frm1_lbl_StockFisico")}</span>
                        </Col>
                    </Row>
                    <Row>
                        <Col>{props.stockGeneral.CantidadFisica}</Col>
                    </Row>
                </Col>
                <Col>
                    <Row>
                        <Col>
                            <span className="text-muted">{t("STO005_frm1_lbl_Reserva")}</span>
                        </Col>
                    </Row>
                    <Row>
                        <Col>{props.stockGeneral.CantidadReserva}</Col>
                    </Row>
                </Col>
            </Row>
            <Row className="mb-4">
                <Col>
                    <Row>
                        <Col>
                            <span className="text-muted">{t("STO005_frm1_lbl_Disponible")}</span>
                        </Col>
                    </Row>
                    <Row>
                        <Col>{props.stockGeneral.CantidadDisponible}</Col>
                    </Row>
                </Col>
                <Col>
                    <Row>
                        <Col>
                            <span className="text-muted">{t("STO005_frm1_lbl_NoDisponible")}</span>
                        </Col>
                    </Row>
                    <Row>
                        <Col>{props.stockGeneral.CantidadNoDisponible}</Col>
                    </Row>
                </Col>
                <Col>
                    <Row>
                        <Col>
                            <span className="text-muted">{t("STO005_frm1_lbl_DisponibleDocumento")}</span>
                        </Col>
                    </Row>
                    <Row>
                        <Col>{props.stockGeneral.CantidadDisponibleDocumental}</Col>
                    </Row>
                </Col>
            </Row>
            <hr />
        </React.Fragment>
    );
}