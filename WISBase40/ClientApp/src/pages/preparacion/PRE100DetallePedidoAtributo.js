import React, { useState } from 'react';
import { Modal, Col, Row, Button, Container } from 'react-bootstrap';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import PRE100AsociarAtributoDetalle from './PRE100AsociarAtributoDetalle';

export default function PRE100DetallePedidoAtributo(props) {
    const { t } = useTranslation();
    const [editable, setEditable] = useState(false);
    const [showAtributosPopup, setShowAtributosPopup] = useState(false);
    const [datos, setDatos] = useState("");
    const [nexus, setNexus] = useState("");

    const [empresaNombre, setEmpresaNombre] = useState("");
    const [agenteDescripcion, setAgenteDescripcion] = useState("");
    const [agenteCodigo, setAgenteCodigo] = useState("");
    const [agenteTipo, setAgenteTipo] = useState("");

    const handleClose = () => {
        props.onHide();
    };

    const applyParameters = (context, data, nexus) => {
        data.parameters = [
            { id: "pedido", value: props.pedido },
            { id: "cliente", value: props.cliente },
            { id: "empresa", value: props.empresa }
        ];

        setNexus(nexus);
    };

    const onBeforePageLoad = (data) => {
        var datos = [
            { id: "pedido", value: props.pedido },
            { id: "cliente", value: props.cliente },
            { id: "empresa", value: props.empresa }
        ]

        setDatos(datos);
        data.parameters = datos;
    }

    const onAfterPageLoad = (data) => {
        if (data.parameters.length > 0) {
            setEditable(data.parameters.find(x => x.id === "editable").value)

            if (data.parameters.find(s => s.id === "empresaNombre")) {
                setEmpresaNombre(data.parameters.find(d => d.id === "empresaNombre").value);
                setAgenteDescripcion(data.parameters.find(d => d.id === "agenteDescripcion").value);
                setAgenteCodigo(data.parameters.find(d => d.id === "agenteCodigo").value);
                setAgenteTipo(data.parameters.find(d => d.id === "agenteTipo").value);
            }
        }
    }

    const openAtributosFormDialog = () => {
        setShowAtributosPopup(true)
    }

    const closeAtributosFormDialog = () => {
        setShowAtributosPopup(false)
        nexus.getGrid("PRE100DetallePedidoAtributo_grid_1").refresh();

        var datos = [
            { id: "pedido", value: props.pedido },
            { id: "cliente", value: props.cliente },
            { id: "empresa", value: props.empresa }
        ]

        setDatos(datos);
    }

    const onBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnEditar") {
            context.abortServerCall = true;

            setDatos(
                [
                    { id: "pedido", value: props.pedido },
                    { id: "cliente", value: props.cliente },
                    { id: "empresa", value: props.empresa },
                    { id: "producto", value: data.row.cells.find(d => d.column === "CD_PRODUTO").value },
                    { id: "faixa", value: data.row.cells.find(d => d.column === "CD_FAIXA").value },
                    { id: "identificador", value: data.row.cells.find(d => d.column === "NU_IDENTIFICADOR").value },
                    { id: "idEspecificaIdentificador", value: data.row.cells.find(d => d.column === "ID_ESPECIFICA_IDENTIFICADOR").value },
                    { id: "idConfiguracion", value: data.row.cells.find(d => d.column === "NU_DET_PED_SAI_ATRIB").value },
                    { id: "cantidad", value: data.row.cells.find(d => d.column === "QT_PEDIDO").value },
                    { id: "update", value: "S" },
                ]
            );
            setShowAtributosPopup(true);
        }

        setNexus(nexus);
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-70w" backdrop="static">
            <Page
                application="PRE100DetallePedidoAtributo"
                onBeforeLoad={onBeforePageLoad}
                onAfterLoad={onAfterPageLoad}

                {...props}
            >
                <Modal.Header closeButton>
                    <Modal.Title>{t("PRE100DetallePedidoAtributo_Sec0_modalTitle_Titulo")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>

                    <Row>
                        <Col lg={5}>
                            <Row >
                                <Col lg={2}>
                                    <span style={{ fontWeight: "bold" }}> {t("PRE100_frm1_lbl_pedido")}: </span>
                                </Col>
                                <Col lg={10}>
                                    <span >{`${props.pedido}`}</span>
                                </Col>
                            </Row>
                        </Col>
                        <Col lg={3}>
                            <Row>
                                <Col lg={4}>
                                    <span style={{ fontWeight: "bold" }}>{t("PRE100_frm1_lbl_empresa")}: </span>
                                </Col>
                                <Col lg={6}>
                                    <span> {`${props.empresa}`} - {`${empresaNombre}`}</span>
                                </Col>
                            </Row>
                        </Col>
                        <Col lg={4}>
                            <Row>
                                <Col lg={4}>
                                    <span style={{ fontWeight: "bold" }}>{t("PRE100_frm1_lbl_cliente")}: </span>
                                </Col>
                                <Col lg={8}>
                                    <span> {`${agenteTipo}`}-{`${agenteCodigo}`}-{`${agenteDescripcion}`}  </span>
                                </Col>
                            </Row>
                        </Col>
                    </Row>

                    <hr />

                    <div style={{ textAlign: "center" }}>
                        <button id="btnAgregarAtributos" className="btn btn-primary" disabled={editable !== "S"} onClick={openAtributosFormDialog}>{t("PRE100DetallePedidoAtributo_Sec0_btn_AgregarAtributos")}</button>
                    </div>

                    <hr />

                    <div className="row mb-4">
                        <div className="col-12">
                            <Grid
                                id="PRE100DetallePedidoAtributo_grid_1"
                                application="PRE100DetallePedidoAtributo"
                                onBeforeInitialize={applyParameters}
                                onBeforeFetch={applyParameters}
                                onBeforeExportExcel={applyParameters}
                                onBeforeFetchStats={applyParameters}
                                onBeforeApplySort={applyParameters}
                                onBeforeApplyFilter={applyParameters}
                                onBeforeButtonAction={onBeforeButtonAction}
                                rowsToFetch={30}
                                rowsToDisplay={15}
                                enableExcelExport
                            />
                        </div>
                    </div>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="outline-secondary" onClick={handleClose}>
                        {t("General_Sec0_btn_Cerrar")}
                    </Button>
                </Modal.Footer>
                <PRE100AsociarAtributoDetalle show={showAtributosPopup} onHide={closeAtributosFormDialog} datos={datos} />
            </Page>
        </Modal>
    );
}