import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldDate, SubmitButton, FormButton, StatusMessage } from '../../components/FormComponents/Form';
import { useTranslation } from 'react-i18next';
import { Modal, Button, Row, Col, FormGroup, Tab, Tabs, fieldset, Badge, Table } from 'react-bootstrap';
import { EXP150VerContenedores } from './EXP150VerContenedores';
import { EXP150VerDetPicking } from './EXP150VerDetPicking';
import CloseButton from 'react-bootstrap/CloseButton';

export default function EXP150(props) {

    const { t } = useTranslation();

    const [showVerContenedoresPopup, setshowVerContenedoresPopup] = useState(false);
    const [showPickingContePopup, setshowPickingContePopup] = useState(false);
    const [showModal, setshowModal] = useState(false);
    const [Pedido, setPedido] = useState("");
    const [Cliente, setCliente] = useState("");
    const [Empresa, setEmpresa] = useState("");
    const [Contenedor, setContenedor] = useState(0);
    const [Preparacion, setPreparacion] = useState(0);

    const showFormVerContenedoresPopup = () => {
        return (<EXP150VerContenedores show={showVerContenedoresPopup} onHide={closeFormDialog} addParameters={addParameters} />);
    }
    const showFormshowInfoDetPickingContPopup = () => {
        return (<EXP150VerDetPicking show={showPickingContePopup} onHide={closeFormDialog} addParameters={addParameters} />);
    }
    const addParameters = (context, data, nexus) => {
        data.parameters = [
            {
                id: "NU_PEDIDO",
                value: Pedido
            },
            {
                id: "CD_CLIENTE",
                value: Cliente
            },
            {
                id: "CD_EMPRESA",
                value: Empresa
            },
            {
                id: "NU_CONTENEDOR",
                value: Contenedor
            },
            {
                id: "NU_PREPARACION",
                value: Preparacion
            }
        ];
    };

    const onAfterButtonAction = (context, data, nexus) => {
        if (context.buttonId === "BtnVerContenedor") {
            setPedido(context.parameters.find(x => x.id == "NU_PEDIDO").value);
            setCliente(context.parameters.find(x => x.id == "CD_CLIENTE").value);
            setEmpresa(context.parameters.find(x => x.id == "CD_EMPRESA").value);

            context.abortServerCall = true;
            setshowModal(true);
            setshowVerContenedoresPopup(true);


        } else {
            data.getGrid("EXP150_grid_1").reset();
        }
    };


    const closeFormDialog = (context, data, nexus) => {
        if (context) {
            if (context.parameters.some(x => x.id == "BTNID")) {

                if (context.parameters.find(x => x.id == "BTNID").value == "BtnVerDetPickContenedor") {

                    setContenedor(context.parameters.find(x => x.id == "NU_CONTENEDOR").value);
                    setPreparacion(context.parameters.find(x => x.id == "NU_PREPARACION").value);
                    setshowVerContenedoresPopup(false);
                    setshowPickingContePopup(true);
                }

            }
        }
    }
    const closeXFormDialog = (context, data, nexus) => {
        setshowVerContenedoresPopup(false);
        setshowPickingContePopup(false)
        setshowModal(false);
    }

    const closeVolverFormDialog = () => {

        if (showVerContenedoresPopup == false) {
            setshowVerContenedoresPopup(true);
            setshowPickingContePopup(false)
        } else {
            setshowVerContenedoresPopup(false);
            setshowModal(false);

        }
    }
    const greenStyle = {
        backgroundColor: "#7EF95D",
        color: "black"

    };
    const yellowStyle = {
        backgroundColor: "#f4f870",
        color: "black"
    };
    return (

        <Page
            title={t("EXP150_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Form
                id="EXP150_form"
                application="EXP150Form"
            >
                <Row>
                    <Col md={6}>
                        <h5 align="center">
                            <span class="badge" style={greenStyle}>{t("EXP150_frm1_lbl_PedidoSinAsignar")}</span>
                        </h5>
                    </Col>
                    <Col md={6}>
                        <h5 align="center">
                            <span class="badge" style={yellowStyle}>{t("EXP150_frm1_lbl_PedidoAsignado")}</span>
                        </h5>
                    </Col>
                </Row>
            </Form>
            <br />
            <row>
                <div className="row mb-4">
                    <div className="col-12">
                        <Grid id="EXP150_grid_1"
                            application="EXP150PedidoComplSinEmpaque"
                            rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                            onAfterButtonAction={onAfterButtonAction}
                        />
                    </div>
                </div>
            </row>

            <Modal show={showModal} onHide={closeXFormDialog} dialogClassName="modal-90w" backdrop="static" >
                <Modal.Header>
                    <div className="left">  <i
                        onClick={closeVolverFormDialog} ><i className="fa fa-arrow-left" /></i>
                        {showVerContenedoresPopup ? <Modal.Title>{t("EXP150_Sec0_pageTitle_VerContenedores")}</Modal.Title> :
                            <Modal.Title>{t("EXP150_Sec0_pageTitle_VerDetPicking")}</Modal.Title>}
                    </div>
                    <CloseButton onClick={closeXFormDialog} />

                </Modal.Header>

                {showVerContenedoresPopup ? showFormVerContenedoresPopup() : null}
                {showPickingContePopup ? showFormshowInfoDetPickingContPopup() : null}

                <Modal.Footer>
                    <Button variant="btn btn-outline-secondary" onClick={closeVolverFormDialog}> {t("EXP150ConfInicial_frm1_btn_volver")} </Button>
                </Modal.Footer>
            </Modal>
        </Page>
    );
}