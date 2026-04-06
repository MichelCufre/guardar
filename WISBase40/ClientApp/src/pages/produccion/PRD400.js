import React, { useState, useRef } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldDate, SubmitButton, FormButton, StatusMessage } from '../../components/FormComponents/Form';
import { useTranslation } from 'react-i18next';
import { Modal, Button, Row, Col, FormGroup, Tab, Tabs } from 'react-bootstrap';
import { withPageContext } from '../../components/WithPageContext';


export default function PRD400(props) {
    const { t } = useTranslation();
    const [NuContenedor, setNuContenedor] = useState(null);
    const [NuPreparacion, setNuPreparacion] = useState(null);

    const [noPuedeProducir, setNoPuedeProducir] = useState(true);

    const [showPopup, setShowPopup] = useState(false);
    const [summary, setSummary] = useState({});


    const closePopup = () => {
        setShowPopup(false);
    }

    const initialValues = {
        NuContenedor: "",
    };



    const addParameters = (context, data, nexus) => {
        if (data.parameters.length == 0) {
            data.parameters = [
                {
                    id: "FILTROS",
                    value: JSON.stringify({ NU_CONTENEDOR: NuContenedor, NU_PREPARACION: NuPreparacion })
                }
            ];
        }

    };


    const onBeforeSubmit = (context, form, query, nexus) => {

        setSummary({});
    }

    const onAfterSubmit = (context, form, query, nexus) => {

        if (query.parameters.length > 0) {

            setNoPuedeProducir(false);

            const _Contenedor = query.parameters.find(d => d.id === "NuContenedor");
            const _Preparacion = query.parameters.find(d => d.id === "NuPreparacion");
            const _Summary = query.parameters.find(d => d.id === "summary");

            setNuContenedor(_Contenedor ? _Contenedor.value : null);
            setNuPreparacion(_Preparacion ? _Preparacion.value : null);

            const summary = (_Summary ? JSON.parse(_Summary.value) : []).reduce((result, parameter) => {
                result[parameter.Key] = parameter.Value;

                return result;
            }, {});


            setSummary(summary);

            var parameters = [
                {
                    id: "FILTROS",
                    value: JSON.stringify({ NU_CONTENEDOR: (_Contenedor ? _Contenedor.value : null), NU_PREPARACION: (_Preparacion ? _Preparacion.value : null) })
                }
            ];

            nexus.getGrid("PRD400_grid_ProductosEntrada").refresh(parameters);
            nexus.getGrid("PRD400_grid_ProductosSobrantes").refresh(parameters);
            nexus.getGrid("PRD400_grid_ProductosSalida").refresh(parameters);

        }

    };

    const validationSchema = {
    };

    const onBeforeButtonAction = (context, form, query, nexus) => {

        if (query.buttonId == "BtnProducir" && noPuedeProducir) {
            // context.abortServerCall = true;
        }
        else {
            setNoPuedeProducir(true);
        }

        query.parameters = [
            {
                id: "FILTROS",
                value: JSON.stringify({ NU_CONTENEDOR: NuContenedor, NU_PREPARACION: NuPreparacion })
            }
        ];

    };

    const onAfterButtonAction = (context, form, query, nexus) => {

        setNoPuedeProducir(false);

        nexus.getForm("PRD400_form_1").reset();

        var parameters = [
            {
                id: "FILTROS",
                value: null
            }
        ];

        setNuContenedor(null);
        setNuPreparacion(null);

        nexus.getGrid("PRD400_grid_ProductosEntrada").refresh(parameters);
        nexus.getGrid("PRD400_grid_ProductosSobrantes").refresh(parameters);
        nexus.getGrid("PRD400_grid_ProductosSalida").refresh(parameters);

        document.getElementById("NuContenedor").focus();
    };

    const onBeforeValidateField = (context, form, query, nexus) => {
        context.abortServerCall = true;
    }

    const onAfterInitialize = (context, form, query, nexus) => {

        document.getElementById("NuContenedor").focus();
    };

    return (

        <Page
            title={t("PRD400_Sec0_pageTitle_Titulo")}
            application="PRD400"
            {...props}
        >

            <Form
                id="PRD400_form_1"
                application="PRD400"
                validationSchema={validationSchema}
                initialValues={initialValues}
                onBeforeSubmit={onBeforeSubmit}
                onAfterSubmit={onAfterSubmit}
                onAfterButtonAction={onAfterButtonAction}
                onBeforeButtonAction={onBeforeButtonAction}
                onBeforeValidateField={onBeforeValidateField}
                onAfterInitialize={onAfterInitialize}
            >
                <Row>

                    <Col lg="3">
                        <fieldset>

                            <FormGroup>
                                <label htmlFor="NuContenedor">{t("EXP330_frm1_lbl_NU_CONTENEDOR")}</label>
                                <Field name="NuContenedor" id="NuContenedor" />
                                <StatusMessage for="NuContenedor" />
                            </FormGroup>

                            <button type="submit" hidden></button>

                        </fieldset>

                    </Col>



                    <Col lg="6">
                        <FormGroup>
                            <br />
                            <h4 className="mt-2 ml-2 mr-5">{t("PRD400_frm1_lbl_NU_CONTENEDOR")} : {NuContenedor}</h4>
                        </FormGroup>

                    </Col>

                    <Col lg="1">
                        <FormGroup>
                            <br />
                            <Button id="BtnInfo" className="mt-2 ml-2 mr-5" variant="outline-primary" onClick={() => {
                                setShowPopup(true);

                            }} ><i className="fa fa-info-circle" /></Button>
                        </FormGroup>
                    </Col>

                    <Col lg="2">
                        <FormGroup>
                            <br />
                            <FormButton id="BtnProducir" label="PRD400_frm1_btn_Producir" className="mt-2 ml-2 mr-5" />
                        </FormGroup>
                    </Col>

                </Row>

            </Form>


            <Row>
                <Col lg="6">
                    <Row>
                        <Col>
                            <h4>{t("PRD400_grid1_title_ProductosEntrada")}</h4>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <Grid id="PRD400_grid_ProductosEntrada" rowsToFetch={30} rowsToDisplay={15}
                                onBeforeFetch={addParameters}
                                application="PRD400"
                            />
                        </Col>
                    </Row>
                </Col>
                <Col lg="6">
                    <Row>
                        <Col>
                            <h4>{t("PRD400_grid_title_ProductosSobrantes")}</h4>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <Grid id="PRD400_grid_ProductosSobrantes" rowsToFetch={30} rowsToDisplay={15}
                                onBeforeFetch={addParameters}
                                application="PRD400"
                            />
                        </Col>
                    </Row>
                </Col>

            </Row>

            <br />
            <hr />

            <Row>
                <Col>

                    <Row>
                        <Col>
                            <h4>{t("PRD400_grid_title_ProductosSalida")}</h4>
                        </Col>
                    </Row>
                    <Row>
                        <Col>
                            <Grid id="PRD400_grid_ProductosSalida" rowsToFetch={30} rowsToDisplay={15}
                                onBeforeFetch={addParameters}
                                application="PRD400"
                            />
                        </Col>
                    </Row>
                </Col>
            </Row>

            <PRD400Modal show={showPopup} onHide={closePopup} summary={summary} />

        </Page >
    );

}




function InternalPRD400Modal(props) {

    const { t } = useTranslation();



    const handleClose = () => {
        props.onHide();
    };

    const summaryContent = (
        <React.Fragment>

            <Row>
                <h4 className="ml-5">{t("PRD400_frm1_lbl_Info")}</h4>
            </Row>
            <hr />
            <Row>
                <Col lg="6">
                    <ul className="list-unstyled">
                        <li>{t("PRD400_frm1_lbl_NU_PEDIDO")}: {props.summary.NU_PEDIDO}</li>
                        <li>{t("PRD400_frm1_lbl_CD_CLIENTE")}: {props.summary.CD_CLIENTE} / {props.summary.DS_CLIENTE}</li>
                        <li>{t("PRD400_frm1_lbl_CD_EMPRESA")}: {props.summary.CD_EMPRESA} / {props.summary.NM_EMPRESA}</li>
                        <li><hr /></li>
                        <li>{t("PRD400_frm1_lbl_DS_ANEXO1")}: {props.summary.DS_ANEXO1}</li>
                        <li>{t("PRD400_frm1_lbl_DS_ANEXO2")}: {props.summary.DS_ANEXO2}</li>
                        <li>{t("PRD400_frm1_lbl_DS_ANEXO3")}: {props.summary.DS_ANEXO3}</li>
                        <li>{t("PRD400_frm1_lbl_DS_ANEXO4")}: {props.summary.DS_ANEXO4}</li>
                        <li>{t("PRD400_frm1_lbl_DS_MEMO")}: {props.summary.DS_MEMO}</li>
                        <li>{t("PRD400_frm1_lbl_DS_MEMO_1")}: {props.summary.DS_MEMO_1}</li>
                    </ul>
                </Col>
                <Col lg="6">
                    <ul className="list-unstyled">
                        <li>{t("PRD400_frm1_lbl_TP_PEDIDO")}: {props.summary.TP_PEDIDO}</li>
                        <li>{t("PRD400_frm1_lbl_DS_ENDERECO")}: {props.summary.DS_ENDERECO}</li>
                        <li>{t("PRD400_frm1_lbl_CD_ROTA")}: {props.summary.CD_ROTA}</li>
                        <li>{t("PRD400_frm1_lbl_DT_ENTREGA")}: {props.summary.DT_ENTREGA}</li>
                        <li>{t("PRD400_frm1_lbl_NU_PRDC_INGRESO")}: {props.summary.NU_PRDC_INGRESO}</li>
                    </ul>
                </Col>
            </Row>
        </React.Fragment>
    );

    return (
        <Modal show={props.show} onHide={props.onHide} dialogClassName="modal-90w">
            <Modal.Header closeButton>
                <Modal.Title>{t("PRD400_frm1_lbl_NU_CONTENEDOR") + " - " + props.summary.NU_CONTENEDOR}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Row>
                    <Col>
                        {summaryContent}
                    </Col>
                </Row>

            </Modal.Body>
            <Modal.Footer>
                <Button variant="danger" onClick={handleClose}>
                    {t("General_Sec0_btn_Cancelar")}
                </Button>
            </Modal.Footer>
        </Modal>


    );

}


export const PRD400Modal = withPageContext(InternalPRD400Modal);