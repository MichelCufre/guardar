import React, { useState } from 'react';
import { Modal, Button, Row, Col, Container } from 'react-bootstrap';
import { Grid } from '../../components/GridComponents/Grid';
import { Form } from '../../components/FormComponents/Form';
import * as Yup from 'yup';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../../components/WithPageContext';

import './PRE052SelecLibOndaModal.css';


function InternalPRE052SelecLibOndaModal(props) {

    const { t } = useTranslation();

    const [informacionCalculada, setInformacionCalculada] = useState([{
        unidadesCalculadas: "0",
        lineasCalculadas: "0",
        pesoCalculado: "0",
        volumenCalculado: "0",
    }]);

    const [unidadesCalculadas, setUnidadesCalculadas] = useState("0");
    const [lineasCalculadas, setLineasCalculadas] = useState("0");
    const [pesoCalculado, setPesoCalculado] = useState("0");
    const [volumenCalculado, setVolumenCalculado] = useState("0");
    const [filasSeleccionadas, setFilasSeleccionadasCalculado] = useState("0");

    const validationSchema = {

        idEmpresa: Yup.string().required(),
        predio: Yup.string(),
        onda: Yup.string(),
        condicionLiberacion: Yup.string(),
        documento: Yup.string(),
        agrupacion: Yup.string(),
        ubicacionCompleta: Yup.string(),
        ubicacionIncompleta: Yup.string(),
        prepSoloCamion: Yup.string(),
        agrupPorCamion: Yup.string(),
        manejaVidaUtil: Yup.string(),
        priorizarDesborde: Yup.string(),
        stock: Yup.string(),
        pedidos: Yup.string(),
        repartirEscasez: Yup.string(),
        liberarPorUnidades: Yup.string(),
        stockDtmi: Yup.string(),
        respetaFifo: Yup.string(),

    };

    const handleFormBeforeInitialize = (context, form, query, nexus) => {
        query.parameters = props.formulario;
    };

    const handleGridBeforeInitialize = (context, data, nexus) => {
        data.parameters = props.formulario;

    };

    const handleGridAfterInitialize = (context, grid, data, nexus) => {
        setInformacionCalculada([{

            unidadesCalculadas: "0",
            lineasCalculadas: "0",
            pesoCalculado: "0",
            volumenCalculado: "0",

        }]);
    };

    const handleSelection = () => {
        let grid1 = props.nexus.getGrid("PRE052SelecLibOnda_grid_1");
    };


    const addParameters = (context, data, nexus) => {
        data.parameters = props.formulario;
    }

    const handleClose = (id) => {
        let idTest = id.currentTarget.id;
        if (idTest == "btnAtras") {
            props.onHide(props.formulario, idTest, props.nexus);
        } else {

            props.onHide(null, null, props.nexus);
        }
    };


    const handleGoBack = () => {
        props.onAfterButtonAction();
    }

    const onAfterSubmit = (context, data, nexus) => {
        if (context.status === "ERROR")
            return false;
        nexus.getGrid("PRE052SelecLibOnda_grid_1").refresh();
    }

    const handleOnAfterNotifySeleccion = (context, data, nexus) => {
        if (data.parameters.find(x => x.id === "unidadesCalculadas")) {

            setUnidadesCalculadas(data.parameters.find(x => x.id === "unidadesCalculadas").value);
            setLineasCalculadas(data.parameters.find(x => x.id === "lineasCalculadas").value);
            setPesoCalculado(data.parameters.find(x => x.id === "pesoCalculado").value);
            setVolumenCalculado(data.parameters.find(x => x.id === "volumenCalculado").value);
            setFilasSeleccionadasCalculado(data.parameters.find(x => x.id === "filasSeleccionadas").value);


        }
    }

    return (

        <Form
            application="PRE052SelecLibOnda"
            id="PRE052SelecLibOnda_form_1"
            validationSchema={validationSchema}
            onBeforeInitialize={handleFormBeforeInitialize}

        >

            <Modal.Header closeButton>
                <Modal.Title>{t("PRE052_Sec0_mdlLibOnda_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Container fluid>
                    <Row className="d-flex justify-content-center">
                        <Col className="d-flex justify-content-center">
                            <div className="btn ultPrepVaciaCabezal btn-sm">{t("PRE052SelecLibOnda_frm1_lbl_PedidoNoLiberado")}</div>
                        </Col>

                        <Col className="d-flex justify-content-center label-warning">
                            <div className="btn liberadoCabezal  btn-sm">{t("PRE052SelecLibOnda_frm1_lbl_PedidoLiberadoParcialmente")}</div>
                        </Col>
                        <Col className="d-flex justify-content-center">
                            <div className="btn ultPrepCabezal btn-sm">{t("PRE052SelecLibOnda_frm1_lbl_PedidoLiberadoRechazdo")}</div>
                        </Col>
                        <Col className="d-flex justify-content-center">

                            <div className="btn btn-default btn-sm"> &nbsp; <i className='fa fa-times-circle fa-lg' />{t("PRE052SelecLibOnda_frm1_lbl_ProdSinVol")}</div>
                        </Col>
                        <Col className="d-flex justify-content-center">

                            <div className="btn btn-default btn-sm"> &nbsp; <i className='fa fa-exclamation-triangle fa-lg' />{t("PRE052SelecLibOnda_frm1_lbl_ProdSinPesoBruto")}</div>
                        </Col>
                    </Row>
                </Container>
                <hr></hr>
                <Container fluid>
                    <Row>
                        <Col>
                            <Row>
                                <Col sm={4}>
                                    <span style={{ fontWeight: "bold" }}>{t("PRE052SelecLibOnda_frm1_lbl_TUNIDADES")}: </span>
                                </Col>
                                <Col className='p-0'>
                                    <span>{`${unidadesCalculadas}`}</span>
                                </Col>
                            </Row>
                            <Row>
                                <Col sm={4}>
                                    <span style={{ fontWeight: "bold" }}> {t("PRE052SelecLibOnda_frm1_lbl_SPESO")}: </span>

                                </Col>
                                <Col className='p-0'>
                                    <span>{`${pesoCalculado}`}</span>
                                </Col>
                            </Row>
                        </Col>
                        <Col>
                            <Row>
                                <Col sm={4}>
                                    <span style={{ fontWeight: "bold" }}> {t("PRE052SelecLibOnda_frm1_lbl_RS")}: </span>
                                </Col>
                                <Col className='p-0'>
                                    <span>{`${filasSeleccionadas}`}</span>
                                </Col>
                            </Row>
                            <Row>
                                <Col sm={4}>
                                    <span style={{ fontWeight: "bold" }}> {t("PRE052SelecLibOnda_frm1_lbl_TLINEAS")}: </span>
                                </Col>
                                <Col className='p-0'>
                                    <span>{`${lineasCalculadas}`}</span>
                                </Col>
                            </Row>
                            <Row>
                                <Col sm={4}>
                                    <span style={{ fontWeight: "bold" }}> {t("PRE052SelecLibOnda_frm1_lbl_SVOL")}: </span>
                                </Col>
                                <Col className='p-0'>
                                    <span>{`${volumenCalculado}`}</span>
                                </Col>
                            </Row>
                        </Col>
                    </Row>

                </Container>
                <hr></hr>
                <Container fluid>
                    <Row className='mt-3'>
                        <Col>
                            <Grid
                                application="PRE052SelecLibOnda"
                                id="PRE052SelecLibOnda_grid_1" rowsToFetch={30} rowsToDisplay={10} enableExcelExport
                                onBeforeInitialize={handleGridBeforeInitialize}
                                onBeforeFetch={handleGridBeforeInitialize}
                                onBeforeApplyFilter={handleGridBeforeInitialize}
                                onBeforeApplySort={handleGridBeforeInitialize}
                                onAfterInitialize={handleGridAfterInitialize}
                                onBeforeNotifySelection={addParameters}
                                onBeforeNotifyInvertSelection={addParameters}
                                onAfterNotifySelection={handleOnAfterNotifySeleccion}
                                onAfterNotifyInvertSelection={handleOnAfterNotifySeleccion}
                                onChange={handleSelection}
                                onBeforeMenuItemAction={addParameters}
                                onAfterMenuItemAction={onAfterSubmit}
                                onBeforeExportExcel={addParameters}
                                onBeforeFetchStats={addParameters}
                                enableSelection
                                notifySelection
                            />

                        </Col>
                    </Row>
                </Container>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("PRE052_frm1_btn_cerrar")} </Button>
                <Button id="btnAtras" variant="btn btn-outline-secondary" onClick={handleClose.bind(this)}> {t("PRE052_frm1_btn_atras")} </Button>
                <Button variant="btn btn-outline-info" onClick={handleClose}> {t("PRE052_frm1_btn_verLiberaciones")} </Button>
            </Modal.Footer>
        </Form >

    );
}

export const PRE052SelecLibOndaModal = withPageContext(InternalPRE052SelecLibOndaModal);