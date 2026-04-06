import React, { useState, useRef } from 'react';
import { Modal, Button, Row, Col, Tab, Tabs, Container } from 'react-bootstrap';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useCustomTranslation } from '../../components/TranslationHook';
import { withPageContext } from '../../components/WithPageContext';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldTextArea, StatusMessage, SubmitButton, FieldDate, FieldDateTime } from '../../components/FormComponents/Form';
import { notificationType } from '../../components/Enums';
import * as Yup from 'yup';
import { ConfirmationBox } from '../../components/ConfirmationBox';
import { AddRemovePanel } from '../../components/AddRemovePanel';
import { CheckboxList } from '../../components/CheckboxList';


function InternalREC170SeleccionReferencias(props) {

    const { t } = useCustomTranslation();

    const [itemsListDisponibles, setItemsListDisponibles] = useState([]);
    const [itemsListSeleccionados, setItemsListSeleccionados] = useState([]);
    const [opcional, setOpcional] = useState(null);


    const handleClose = () => {
        props.onHide();
    };

    const handleGuardar = () => {

        props.seleccionadas.current =
            props.onHide();
    };

    const handleOnAfterInitialize = (context, form, query, nexus) => {


        let jsonAdded = query.parameters.find(w => w.id === "referenciasDisponibles").value;
        var arrayAdded = JSON.parse(jsonAdded.toString());

        setItemsListDisponibles(arrayAdded);

        let jsonAsociadas = query.parameters.find(w => w.id === "referenciasAsociadas").value;
        var arrayAsociadas = JSON.parse(jsonAsociadas.toString());

        setItemsListSeleccionados(arrayAsociadas);
    }

    const handleOnBeforeInitialize = (context, form, query, nexus) => {


        query.parameters = [

            { id: "idEmpresa", value: props.nexus.getForm("REC170Create_form_1").getFieldValue("idEmpresa") },
            { id: "codigoInternoAgente", value: props.nexus.getForm("REC170Create_form_1").getFieldValue("codigoInternoAgente") },
            { id: "tipoRecepcionExterno", value: props.nexus.getForm("REC170Create_form_1").getFieldValue("tipoRecepcionExterno") },
            { id: "numeroPredio", value: props.nexus.getForm("REC170Create_form_1").getFieldValue("numeroPredio") },
            { id: "referenciasSeleccionadasPrevias", value: JSON.stringify(props.seleccionadas) },
        ];

    }

    const handleChangeDisponibles = (event, id) => {

        let itemsListDisponibles = [...itemsListDisponibles];

        console.log(itemsListDisponibles);

        let item = itemsListDisponibles.find(d => d.id === id);

        item.selected = !item.selected;

        setItemsListDisponibles(itemsListDisponibles);
    };

    const handleChangeSelecciondas = (event, id) => {

        let itemsListSeleccionados = [...itemsListSeleccionados];

        console.log(itemsListSeleccionados);

        let item = itemsListSeleccionados.find(d => d.id === id);

        item.selected = !item.selected;

        setItemsListSeleccionados(itemsListSeleccionados);
    };


    const handleFormBeforeSubmit = (context, form, query, nexus) => {

        props.seleccionadas = itemsListSeleccionados;
        //  query.parameters.push({ id: "listaPerfiles", value: JSON.stringify(itemsList) });
    }

    const handleAdd = (evt, nexus) => {

        let seleccionadosParaAgregar = itemsListDisponibles.find(d => d.selected === true);
        let itemsListSeleccionados = [...itemsListSeleccionados];

        // Agrego los seleccionados disponibles a los seleccinados para asociar
        itemsListSeleccionados.push(seleccionadosParaAgregar);
        setItemsListSeleccionados(itemsListSeleccionados);

        // Elimino de la lista de disponibles
        let itemsListDisponibles = [...itemsListDisponibles];

        for (let i = 0; i < itemsListSeleccionados.length; i++) {

            let pos = itemsListDisponibles.indexOf(itemsListSeleccionados[i]);
            itemsListDisponibles.splice(pos, 1);

        }

        setItemsListDisponibles(itemsListDisponibles);
    };

    const handleRemove = (evt, nexus) => {

        let seleccionadosParaQuitar = itemsListSeleccionados.find(d => d.selected === true);
        let itemsListDisponibles = [...itemsListDisponibles];

        // Agrego los seleccionados para asociar a los seleccinados disponibles
        itemsListDisponibles.push(seleccionadosParaQuitar);
        setItemsListDisponibles(itemsListDisponibles);

        // Elimino de la lista de selecciondas
        let itemsListSeleccionados = [...itemsListSeleccionados];

        for (let i = 0; i < itemsListDisponibles.length; i++) {

            let pos = itemsListSeleccionados.indexOf(itemsListDisponibles[i]);
            itemsListSeleccionados.splice(pos, 1);

        }

        setItemsListSeleccionados(itemsListSeleccionados);

    };

    return (

        <div>


            <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w" backdrop="static" >
                <Form
                    application="CREC170Referencia"
                    id="CREC170Referencia_form_1"
                    onBeforeInitialize={handleOnBeforeInitialize}
                    onAfterInitialize={handleOnAfterInitialize}
                    onBeforeSubmit={handleFormBeforeSubmit}
                >
                    <Modal.Header >
                        <Modal.Title>{t("REC170_Sec0_mdlSelect_Titulo")} </Modal.Title>
                    </Modal.Header>
                    <Modal.Body>

                        <Container fluid>

                            <AddRemovePanel
                                onAdd={handleAdd}
                                onRemove={handleRemove}
                                from={(
                                    <CheckboxList
                                        name="listaDisponibles"
                                        items={itemsListDisponibles}
                                        onChange={handleChangeDisponibles}
                                        maxHeight={opcional}
                                        className={opcional}
                                    />
                                )}
                                to={(
                                    <CheckboxList
                                        name="listaSeleccionadas"
                                        items={itemsListSeleccionados}
                                        onChange={handleChangeSelecciondas}
                                        maxHeight={opcional}
                                        className={opcional}
                                    />
                                )}
                            />

                        </Container>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("REC170_frm1_btn_cerrar")} </Button>
                        <SubmitButton id="btnSubmitGuardar" onClick={handleGuardar} variant="primary" label="REC170_frm1_btn_guardar" />

                    </Modal.Footer>
                </Form>
            </Modal>

        </div>

    );
}


export const REC170SeleccionReferencias = withPageContext(InternalREC170SeleccionReferencias);