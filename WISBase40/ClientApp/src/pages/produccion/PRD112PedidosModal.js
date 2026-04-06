import React, { useState } from 'react';
import { Modal, Button, Row, Col } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../../components/WithPageContext';
import { Grid } from '../../components/GridComponents/Grid';

function InternalPRD112PedidosModal(props) {
    const { t } = useTranslation();

    const [selectedDetalle, setSelectedDetalle] = useState();

    const [isPedidosGridVisible, setIsPedidosGridVisible] = useState(true);
    const [isDetallesModalVisible, setIsDetallesModalVisible] = useState(false);

    const handleClose = () => {
        handleCancel();
        props.onHide();
    };

    const handleCancel = () => {
        setIsPedidosGridVisible(true);
        setIsDetallesModalVisible(false);
    };

    const handleGridPedidosBeforeInitialize = (context, data, nexus) => {
        data.parameters = [
            { id: "idIngreso", value: props.ingresoEditar },
        ];
    }

    const handleGridPedidosBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnDetalles") {
            context.abortServerCall = true;
            setSelectedDetalle({
                nuPedido: data.row.cells.find(d => d.column === "NU_PEDIDO").value,
                cdCliente: data.row.cells.find(d => d.column === "CD_CLIENTE").value,
                cdEmpresa: data.row.cells.find(d => d.column === "CD_EMPRESA").value,
            });
            setIsPedidosGridVisible(false);
            setIsDetallesModalVisible(true);
        }
    }

    const handleGridDetallesBeforeInitialize = (context, data, nexus) => {
        data.parameters = [
            { id: "nuPedido", value: selectedDetalle.nuPedido },
            { id: "cdCliente", value: selectedDetalle.cdCliente },
            { id: "cdEmpresa", value: selectedDetalle.cdEmpresa },
        ];
    }

    const handleGridDetallesBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnDetalles") {
            context.abortServerCall = true;
        }
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-70w">
            <Modal.Header closeButton>
                {isPedidosGridVisible ? (<Modal.Title>{t("PRD112_Sec0_modalTitle_TituloPedidos")}</Modal.Title>) : null}
                {isDetallesModalVisible ? (<Modal.Title>{t("PRD112_Sec0_modalTitle_TituloDetallesPedidos")}</Modal.Title>) : null}
            </Modal.Header>
            <Modal.Body>
                <Row>
                    {
                        isPedidosGridVisible ? (
                            <Col>
                                <Grid
                                    application="PRD110"
                                    id="PRD110Pedidos_grid_2"
                                    rowsToFetch={16}
                                    rowsToDisplay={8}
                                    enableExcelExport={true}
                                    enableExcelImport={false}
                                    onBeforeInitialize={handleGridPedidosBeforeInitialize}
                                    onBeforeButtonAction={handleGridPedidosBeforeButtonAction}
                                    onBeforeFetch={handleGridPedidosBeforeInitialize}
                                    onBeforeApplyFilter={handleGridPedidosBeforeInitialize}
                                    onBeforeApplySort={handleGridPedidosBeforeInitialize}
                                    onBeforeExportExcel={handleGridPedidosBeforeInitialize}
                                    onBeforeFetchStats={handleGridPedidosBeforeInitialize}

                                />
                            </Col>
                        ) : null
                    }
                    {
                        isDetallesModalVisible ? (
                            <Col>
                                <Grid
                                    application="PRD110"
                                    id="PRD110DetallesPedidos_grid_3"
                                    rowsToFetch={16}
                                    rowsToDisplay={8}
                                    enableExcelExport={true}
                                    enableExcelImport={false}
                                    onBeforeInitialize={handleGridDetallesBeforeInitialize}
                                    onBeforeButtonAction={handleGridDetallesBeforeButtonAction}
                                    onBeforeFetch={handleGridDetallesBeforeInitialize}
                                    onBeforeApplyFilter={handleGridDetallesBeforeInitialize}
                                    onBeforeApplySort={handleGridDetallesBeforeInitialize}
                                    onBeforeExportExcel={handleGridDetallesBeforeInitialize}
                                    onBeforeFetchStats={handleGridDetallesBeforeInitialize}

                                />
                            </Col>
                        ) : null
                    }
                </Row>
            </Modal.Body>
            {
                isDetallesModalVisible ? (
                    <Modal.Footer>
                        <Button variant="secondary" onClick={handleCancel}>
                            {t("PRD112_frm1_btn_Volver")}
                        </Button>
                    </Modal.Footer>
                ) : null
            }
        </Modal>
    );
}

export const PRD112PedidosModal = withPageContext(InternalPRD112PedidosModal);