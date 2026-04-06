import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Form } from '../../components/FormComponents/Form';
import { Page } from '../../components/Page';
import { Modal, Row, Col, Container } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { FAC007CreateModal } from './FAC007CreateModal';

export default function FAC007(props) {

    const { t } = useTranslation();

    const [infoFacturacion, setInfoFacturacion] = useState({});

    const [showModal, setShowModal] = useState(false);

    const [showPopupCreate, setShowPopupCreate] = useState(false);

    const openFormDialog = () => {
        setShowPopupCreate(true);
        setShowModal(true);
    }

    const closeFormDialog = (nexus) => {

        setShowPopupCreate(false);
        setShowModal(false);

        if (nexus)
            nexus.getGrid("FAC007_grid_1").refresh();
    }

    const onAfterInitialize = (context, form, query, nexus) => {
        if (query.parameters.find(x => x.id === "nuEjecucion") != null) {
            setInfoFacturacion({
                NuEjecucion: query.parameters.find(x => x.id === "nuEjecucion").value,
            });
        }
    };

    const onAfterButtonAction = (data, nexus) => {
        nexus.getGrid("FAC007_grid_1").refresh();
    }

    const showFormCreate = () => { return (<FAC007CreateModal show={showFormCreate} onHide={closeFormDialog} infoFacturacion={infoFacturacion} />); }

    return (
        <Page
            title={t("FAC007_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <Form
                application="FAC007"
                id="FAC007_form_1"
                application="FAC007"
                onAfterInitialize={onAfterInitialize}
            >

                <Container fluid>
                    <div style={{ marginBottom: '6vh' }}>
                        <Row>
                            <Col sm={2}>
                                <span style={{ fontWeight: "bold" }}>{t("FAC007_frm1_lbl_NroEjecucion")}: </span>
                            </Col>
                            <Col className='p-0'>
                                <span>{`${infoFacturacion.NuEjecucion ? infoFacturacion.NuEjecucion : "-"}`}</span>
                            </Col>
                        </Row>
                    </div>
                </Container>

                <div style={{ textAlign: "center" }}>
                    <button className="btn btn-primary" onClick={openFormDialog}>{t("FAC007_Sec0_btn_NuevoResultado")}</button>
                </div>

                <div className="row mb-4">
                    <div className="col-12">
                        <Grid id="FAC007_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport enableExcelImport={false}
                            onAfterButtonAction={onAfterButtonAction}
                        />
                    </div>
                </div>

                <Modal id="modal" show={showModal} onHide={closeFormDialog} dialogClassName={"modal-50w"} backdrop="static" >
                    {showPopupCreate ? showFormCreate() : null}
                </Modal>

            </Form>

        </Page>
    );
}