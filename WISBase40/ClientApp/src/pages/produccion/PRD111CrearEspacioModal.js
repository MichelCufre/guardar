import React from 'react';
import { Modal, Button, Row, Col } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { Form, FieldSelect, FieldTextArea, StatusMessage, FieldToggle } from '../../components/FormComponents/Form';
import * as Yup from 'yup';
import { withPageContext } from '../../components/WithPageContext';

function InternalPRD111CrearEspacioModal(props) {
    const { t } = useTranslation();

    const initialValues = {
        descripcion: ""
    };

    const validationSchema = {
        descripcion: Yup.string(),
        tipoEspacio: Yup.string(),
        predio: Yup.string(),
        flConfMan: Yup.boolean()
    };

    const handleClose = () => {
        props.onHide();
    };

    const handleSubmit = () => {
        props.nexus.getForm("PRD111_form_1").submit();
    }

    const handleFormBeforeSubmit = (context, form, query, nexus) => {
        console.log("entra submit");
    }

    const handleFormAfterSubmit = (context, form, query, nexus) => {
        if (context.responseStatus === "OK") {
            nexus.getGrid("PRD111_grid_1").refresh();
            props.onHide();
        }
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w">
            <Modal.Header closeButton>
                <Modal.Title>{t("PRD111_Sec0_modalTitle_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Row>
                    <Col>
                        <Form
                            id="PRD111_form_1"
                            application="PRD111CrearEspaacio"
                            initialValues={initialValues}
                            validationSchema={validationSchema}
                            onBeforeSubmit={handleFormBeforeSubmit}
                            onAfterSubmit={handleFormAfterSubmit}
                        >
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="tipoEspacio">{t("PRD111_form_colname_tipoEspacio")}</label>
                                        <FieldSelect name="tipoEspacio" />
                                        <StatusMessage for="tipoEspacio" />
                                    </div>
                                </Col>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="predio">{t("PRD111_form_colname_predio")}</label>
                                        <FieldSelect name="predio" />
                                        <StatusMessage for="predio" />
                                    </div>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <div className="form-group" >
                                        <label htmlFor="descripcion">{t("PRD111_frm1_lbl_DS_PRDC_LINEA")}</label>
                                        <FieldTextArea style={{ width: "100%" }} name="descripcion" />
                                        <StatusMessage for="descripcion" />
                                    </div>
                                </Col>
                                <Col>
                                <br/>
                                    <div className="form-group">
                                        <FieldToggle name="flConfMan" label={t("PRD111_frm1_lbl_FL_CONF_MAN")} />
                                    </div>
                                </Col>
                            </Row>
                        </Form>
                    </Col>
                </Row>


            </Modal.Body>
            <Modal.Footer>
                <Button variant="secondary" onClick={handleClose}>
                    {t("PRD111_frm1_btn_CANCELAR")}
                </Button>
                <Button variant="primary" onClick={handleSubmit}>
                    {t("PRD111_frm1_btn_CREAR")}
                </Button>
            </Modal.Footer>
        </Modal>
    );
}

export const PRD111CrearEspacioModal = withPageContext(InternalPRD111CrearEspacioModal);
