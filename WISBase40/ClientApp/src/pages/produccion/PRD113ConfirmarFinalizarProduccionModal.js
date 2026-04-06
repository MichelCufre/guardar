import React, { useState } from 'react';
import { Modal, Button, Row, Col, Tab, Tabs } from 'react-bootstrap';
import { useCustomTranslation } from '../../components/TranslationHook';
import { Grid } from '../../components/GridComponents/Grid';
export function PRD113ConfirmarFinalizarProduccionModal(props) {
    const { t } = useCustomTranslation("translation", { useSuspense: false });

    const handleClose = () => {
        props.onHide();
    }

    const confirmarFinalizacion = () => {
        props.confirmarFinalizar();
    }

    const addParams = (context, data, nexus) => {
        data.parameters = [
            { id: "idIngresoProduccion", value: props.idIngresoProduccion },
            { id: "ubicacionProduccion", value: props.ubicacionProduccion }
        ];
    }

    const defaultTab = props.diferenciaProducido ? 'diferenciaProducido' : (props.diferenciaConsumo ? 'diferenciaConsumo' : (props.remanenteProduccion ? 'remanenteProduccion' : 'remanenteInsumos'));

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w" backdrop="static">
            <Modal.Header closeButton>
                <Modal.Title>{t("PRD113_lbl_Title_ConfirmarFinalizarModal")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <p>{t("PRD113_grid1_Msg_ConfirmarFin")}</p>
                <ul>
                    {(props.diferenciaProducido) && (
                        <li>{t("PRD113_grid1_Msg_ConfirmarFinProduccionDif")}</li>
                    )}
                    {(props.diferenciaConsumo) && (
                        <li>{t("PRD113_grid1_Msg_ConfirmarFinProduccionDifCon")}</li>
                    )}
                    {props.remanenteProduccion  && (
                        <li>{t("PRD113_grid1_Msg_ConfirmarFinProStockProducidoEspacio")}</li>
                    )}
                    {props.remanenteInsumos && (
                        <li>{t("PRD113_grid1_Msg_ConfirmarFinProStockInsumosEspacio")}</li>
                    )}
                </ul>
                <Tabs defaultActiveKey={defaultTab} transition={false} id="noanim-tab-example">

                    {props.diferenciaProducido && (
                        <Tab eventKey="diferenciaProducido" title={t("PRD113_tab_title_DiferenciaProducido")} >
                            <br />
                            <Grid
                                application="PRD113DifProducido"
                                id="PRD113DifProducido_grid_1"
                                rowsToFetch={30}
                                rowsToDisplay={5}
                                enableExcelExport
                                onBeforeFetch={addParams}
                                onBeforeApplyFilter={addParams}
                                onBeforeApplySort={addParams}
                                onBeforeExportExcel={addParams}
                                onBeforeInitialize={addParams}
                            />
                        </Tab>)}
                    {props.diferenciaConsumo  && (
                        <Tab eventKey="diferenciaConsumo" title={t("PRD113_tab_title_DiferenciaConsumo")}>
                            <br />
                            <Grid
                                application="PRD113DifConsumo"
                                id="PRD113DifConsumo_grid_1"
                                rowsToFetch={30}
                                rowsToDisplay={5}
                                enableExcelExport
                                onBeforeFetch={addParams}
                                onBeforeApplyFilter={addParams}
                                onBeforeApplySort={addParams}
                                onBeforeExportExcel={addParams}
                                onBeforeInitialize={addParams}
                            />
                        </Tab>)}
                    {props.remanenteProduccion && (
                        <Tab eventKey="remanenteProduccion" title={t("PRD113_tab_title_RemanenteProduccion")}>
                            <br />
                            <Grid
                                application="PRD113RemanenteProd"
                                id="PRD113RemanenteProd_grid_1"
                                rowsToFetch={30}
                                rowsToDisplay={5}
                                enableExcelExport
                                onBeforeFetch={addParams}
                                onBeforeApplyFilter={addParams}
                                onBeforeApplySort={addParams}
                                onBeforeExportExcel={addParams}
                                onBeforeInitialize={addParams}
                            />
                        </Tab>)}
                    {props.remanenteInsumos  && (
                        <Tab eventKey="remanenteInsumos" title={t("PRD113_tab_title_RemanenteInsumos")}>
                            <br />
                            <Grid
                                application="PRD113RemanenteIns"
                                id="PRD113RemanenteIns_grid_1"
                                rowsToFetch={30}
                                rowsToDisplay={5}
                                enableExcelExport
                                onBeforeFetch={addParams}
                                onBeforeApplyFilter={addParams}
                                onBeforeApplySort={addParams}
                                onBeforeExportExcel={addParams}
                                onBeforeInitialize={addParams}
                            />
                        </Tab>)}
                </Tabs>

            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}>
                    {t("General_Sec0_btn_Cerrar")}
                </Button>
                <Button variant="btn btn-outline-primary" onClick={confirmarFinalizacion}>
                    {t("General_Sec0_btn_Confirmar")}
                </Button>
            </Modal.Footer>
        </Modal>
    );
}