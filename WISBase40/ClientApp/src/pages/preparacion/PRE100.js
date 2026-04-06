import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Form, FieldToggle, StatusMessage } from '../../components/FormComponents/Form';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { Row, Col } from 'react-bootstrap';
import { PRE100CreatePedido } from './PRE100CreatePedido';
import { PRE100UpdatePedido } from './PRE100UpdatePedido';
import PRE100DetallePedido from './PRE100DetallePedido';
import PRE100PedidoLpn from './PRE100PedidoLpn';
import PRE100DetallePedidoLpn from './PRE100DetallePedidoLpn';
import PRE100DetallePedidoAtributo from './PRE100DetallePedidoAtributo';
import * as Yup from 'yup';

export default function PRE100(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    const [showPopup, setShowPopup] = useState(false);
    const [showUpdatePopup, setShowUpdatePopup] = useState(false);
    const [showDetallesPopup, setShowDetallesPopup] = useState(false);
    const [showLpnPopup, setShowLpnPopup] = useState(false);
    const [showDetallesLpnPopup, setShowDetallesLpnPopup] = useState(false);
    const [showDetallesAtributoPopup, setShowDetallesAtributoPopup] = useState(false);

    const [pedido, setPedido] = useState(null);
    const [cliente, setCliente] = useState(null);
    const [empresa, setEmpresa] = useState(null);
    const [pedidoActivos, setPedidoActivos] = useState(true);

    const initialValues = {
        ND_ACTIVIDAD: true,
    };

    const validationSchema = {
        ND_ACTIVIDAD: Yup.string()
    };

    const openFormDialog = () => {
        setShowPopup(true);

        setShowUpdatePopup(false);
        setShowDetallesPopup(false);
        setShowLpnPopup(false);
        setShowDetallesLpnPopup(false);
    }

    const closeFormDialog = (datos) => {
        setShowPopup(false);

        if (datos) {

            setPedido(datos.pedido);
            setCliente(datos.cliente);
            setEmpresa(datos.empresa);

            if (datos.flujoLpn) {

                setShowLpnPopup(true);
            }
            else {

                setShowDetallesPopup(true);
            }
        }
    }

    const closeUpdateFormDialog = () => {
        setShowUpdatePopup(false);
    }

    const closeDetalleFormDialog = () => {
        setShowDetallesPopup(false);
    }

    const closLpnFormDialog = () => {
        setShowLpnPopup(false);
    }

    const closeDetalleLpnFormDialog = () => {
        setShowDetallesLpnPopup(false);
    }

    const closeDetalleAtributoFormDialog = () => {
        setShowDetallesAtributoPopup(false);
    }

    const handleBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnDetalle") {
            context.abortServerCall = true;

            setPedido(data.row.cells.find(d => d.column === "NU_PEDIDO").value);
            setCliente(data.row.cells.find(d => d.column === "CD_CLIENTE").value);
            setEmpresa(data.row.cells.find(d => d.column === "CD_EMPRESA").value);

            setShowDetallesPopup(true);

            setShowPopup(false);
            setShowLpnPopup(false);
            setShowUpdatePopup(false);
            setShowDetallesLpnPopup(false);
            setShowDetallesAtributoPopup(false);
        }
        else if (data.buttonId === "btnDetallesPedidoLpn") {
            context.abortServerCall = true;

            setPedido(data.row.cells.find(d => d.column === "NU_PEDIDO").value);
            setCliente(data.row.cells.find(d => d.column === "CD_CLIENTE").value);
            setEmpresa(data.row.cells.find(d => d.column === "CD_EMPRESA").value);

            setShowDetallesLpnPopup(true);

            setShowPopup(false);
            setShowLpnPopup(false);
            setShowUpdatePopup(false);
            setShowDetallesPopup(false);
            setShowDetallesAtributoPopup(false);
        }
        else if (data.buttonId === "btnDetallesPedidoAtributo") {
            context.abortServerCall = true;

            setPedido(data.row.cells.find(d => d.column === "NU_PEDIDO").value);
            setCliente(data.row.cells.find(d => d.column === "CD_CLIENTE").value);
            setEmpresa(data.row.cells.find(d => d.column === "CD_EMPRESA").value);

            setShowDetallesAtributoPopup(true);

            setShowPopup(false);
            setShowLpnPopup(false);
            setShowUpdatePopup(false);
            setShowDetallesPopup(false);
            setShowDetallesLpnPopup(false);
        }
    }

    const handleAfterButtonAction = (data, nexus) => {
        if (data.buttonId === "btnEditar") {

            setPedido(data.row.cells.find(d => d.column === "NU_PEDIDO").value);
            setCliente(data.row.cells.find(d => d.column === "CD_CLIENTE").value);
            setEmpresa(data.row.cells.find(d => d.column === "CD_EMPRESA").value);

            setShowUpdatePopup(true);

            setShowPopup(false);
            setShowDetallesPopup(false);
            setShowLpnPopup(false);
            setShowDetallesLpnPopup(false);
        }
        else if (data.buttonId === "btnPedidoLpn") {

            setPedido(data.row.cells.find(d => d.column === "NU_PEDIDO").value);
            setCliente(data.row.cells.find(d => d.column === "CD_CLIENTE").value);
            setEmpresa(data.row.cells.find(d => d.column === "CD_EMPRESA").value);

            setShowLpnPopup(true);

            setShowPopup(false);
            setShowDetallesPopup(false);
            setShowUpdatePopup(false);
            setShowDetallesLpnPopup(false);
        }
    }

    const addParameters = (context, data, nexus) => {

        data.parameters = [
            { id: "pedidoActivos", value: pedidoActivos },
        ];
    }

    const onBeforeValidateField = (context, form, query, nexus) => {
        var pedidoActivos = nexus.getForm("PRE100_form_1").getFieldValue("ND_ACTIVIDAD");
        setPedidoActivos(pedidoActivos);

        query.parameters = [
            { id: "pedidoActivos", value: pedidoActivos},
        ];

        nexus.getGrid("PRE100_grid_1").refresh();
    };
  
    return (
        <Page
            title={t("PRE100_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Row>
                <Col sm={5}>
                    <Form id="PRE100_form_1"
                        initialValues={initialValues}
                        validationSchema={validationSchema}
                        onBeforeValidateField={onBeforeValidateField}
                    >
                        <div className="form-group">
                            <FieldToggle name="ND_ACTIVIDAD" label={t("PRE100_frm1_lbl_ND_ACTIVIDAD")} />
                            <StatusMessage for="ND_ACTIVIDAD" />
                        </div>
                    </Form>
                </Col>
                <Col sm={7} className="d-flex justify-content-left">
                    <button className="btn btn-primary" style={{ marginLeft: '7%' }}  onClick={openFormDialog}>{t("PRE100_Sec0_btn_CrearPedido")}</button>
                </Col>
            </Row>
            <br>
            </br>
            <div className="row mb-4">
                <div className="col-12">
                    <Grid
                        id="PRE100_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        onBeforeInitialize={addParameters}
                        onBeforeFetch={addParameters}
                        onBeforeFetchStats={addParameters}
                        onBeforeExportExcel={addParameters}
                        onBeforeApplyFilter={addParameters}
                        onBeforeApplySort={addParameters}
                        onBeforeButtonAction={handleBeforeButtonAction}
                        onAfterButtonAction={handleAfterButtonAction}
                        enableExcelExport
                    />
                </div>
            </div>

            <PRE100CreatePedido show={showPopup} onHide={closeFormDialog} />
            <PRE100UpdatePedido show={showUpdatePopup} onHide={closeUpdateFormDialog} pedido={pedido} cliente={cliente} empresa={empresa} />
            <PRE100DetallePedido show={showDetallesPopup} onHide={closeDetalleFormDialog} pedido={pedido} cliente={cliente} empresa={empresa} />
            <PRE100PedidoLpn show={showLpnPopup} onHide={closLpnFormDialog} pedido={pedido} cliente={cliente} empresa={empresa} />
            <PRE100DetallePedidoLpn show={showDetallesLpnPopup} onHide={closeDetalleLpnFormDialog} pedido={pedido} cliente={cliente} empresa={empresa} />
            <PRE100DetallePedidoAtributo show={showDetallesAtributoPopup} onHide={closeDetalleAtributoFormDialog} pedido={pedido} cliente={cliente} empresa={empresa} />
        </Page>
    );
}