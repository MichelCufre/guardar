import React, { useState, useEffect } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Modal, Button, Row, Col, Tab, Tabs, Container } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { Form, Field, StatusMessage } from '../../components/FormComponents/Form';
import { Page } from '../../components/Page';
import { AddRemovePanel } from '../../components/AddRemovePanel';
import { PAR400AtributoValidacionValor } from './PAR400AtributoValidacionValor';

export function PAR400AtributoValidacion(props) {
    const { t } = useTranslation("translation", { useSuspense: false });
    const [tipoAtributo, setTipoAtributo] = useState("");
    const [nmAtributo, setNmAtributo] = useState("");
    const [codigoAtributo, setCodigoAtributo] = useState("");
    const [listValidaciones, setListValidaciones] = useState("");
    const [nexus, setNexus] = useState("");
    const [showModal, setShowModal] = useState(false);
    const [showAtributoValidacionValor, setShowAtributoValidacionValor] = useState(false);
    const [BtnDisable, setBtnDisable] = useState(false);

    useEffect(() => {
        setShowAtributoValidacionValor(false);
        setShowModal(false);

        if (listValidaciones) {
            setShowAtributoValidacionValor(true);
            setShowModal(true);
        } 
    }, [listValidaciones]);

    const handleBeforeLoad = (data) => {
        setTipoAtributo(props.tipoAtributo);
        setNmAtributo(props.nmAtributo);
        setCodigoAtributo(props.codigoAtributo);     
    }

    const onBeforeInitialize = (context, form, data, nexus) => {
        data.parameters = [
            { id: "codigoAtributo", value: codigoAtributo },
            { id: "nmAtributo", value: nmAtributo },
        ];
        setNexus(nexus);
    };

    const applyParameters = (context, data, nexus) => {
        data.parameters = [
            { id: "codigoAtributo", value: codigoAtributo },
            { id: "tipoAtributo", value: tipoAtributo },            
        ];
        setNexus(nexus);
    };

    const handleAfterMenuItemAction = (context, data, nexus) => {
        if (data.gridId == "PAR400ValidacionesDisp_grid_1") {

            if (data.parameters.find(d => d.id === "ListValidacion").value === "") {
                nexus.getGrid("PAR400ValidacionesDisp_grid_1").refresh();
                nexus.getGrid("PAR400ValidacionesAso_grid_2").refresh();
            } else {
                setListValidaciones(data.parameters.find(d => d.id === "ListValidacion").value);
            }
        } else {
            nexus.getGrid("PAR400ValidacionesDisp_grid_1").refresh();
            nexus.getGrid("PAR400ValidacionesAso_grid_2").refresh();
        }
    }

    const handleAdd = (evt, nexus) => {
        nexus.getGrid("PAR400ValidacionesDisp_grid_1").triggerMenuAction("btnAgregar", false, evt.ctrlKey);
    };

    const handleRemove = (evt, nexus) => {
        nexus.getGrid("PAR400ValidacionesAso_grid_2").triggerMenuAction("btnQuitar", false, evt.ctrlKey);
    };

    const handleClose = () => {
        props.onHide();
    };

    const onBeforeSubmit = (context, form, query, nexus) => {
        context.abortServerCall = true;
    }

    const handleCloseAtributoValor = (data) => {
        setListValidaciones(data);

        nexus.getGrid("PAR400ValidacionesDisp_grid_1").refresh();
        nexus.getGrid("PAR400ValidacionesAso_grid_2").refresh();
    };

    const handleGridOnAfterInitialize = (context, form, parameters) => {
        setBtnDisable(parameters.find(w => w.id === "BtnDisable").value === "F");
    };

    return (

        <Page
            {...props}
            application="PAR400AtributoValidacion"
            onBeforeLoad={handleBeforeLoad}
        >
            <Modal.Header closeButton>
                <Modal.Title>{t("PAR400AtributoValidacion_Sec0_modalTitle_Titulo")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Container fluid>
                    <Form
                        application="PAR400AtributoValidacion"
                        id="PAR400AtributoValidacion_form_1"
                        onBeforeInitialize={onBeforeInitialize}
                        onBeforeSubmit={onBeforeSubmit}
                    >
                        <Row>
                            <Col md={6}>
                                <div className="form-group">
                                    <label htmlFor="ID_ATRIBUTO">{t("PAR401_frm1_lbl_ID_ATRIBUTO")}</label>
                                    <Field name="ID_ATRIBUTO" value={tipoAtributo} readOnly />
                                    <StatusMessage for="ID_ATRIBUTO" />
                                </div>
                            </Col>
                            <Col md={6}>
                                <div className="form-group">
                                    <label htmlFor="NM_ATRIBUTO">{t("PAR401_frm1_lbl_NM_ATRIBUTO")}</label>
                                    <Field name="NM_ATRIBUTO" value={nmAtributo} readOnly />
                                    <StatusMessage for="NM_ATRIBUTO" />
                                </div>
                            </Col>
                        </Row>
          
                        <AddRemovePanel
                            onAdd={handleAdd}
                            onRemove={handleRemove}
                            BtnDisabled={BtnDisable}
                            from={(
                                <div>
                                    <h5 className='form-title'>{t("PAR401_frm_sub_Disponible")}</h5>
                                    <Grid
                                        application="PAR400AtributoValidacion"
                                        id="PAR400ValidacionesDisp_grid_1"
                                        rowsToFetch={30}
                                        rowsToDisplay={15}
                                        onBeforeInitialize={applyParameters}
                                        onBeforeFetch={applyParameters}
                                        onBeforeFetchStats={applyParameters}
                                        onBeforeMenuItemAction={applyParameters}
                                        onBeforeApplyFilter={applyParameters}
                                        onBeforeApplySort={applyParameters}
                                        onAfterMenuItemAction={handleAfterMenuItemAction}
                                        onBeforeExportExcel={applyParameters}
                                        enableExcelExport
                                        enableSelection
                                        onAfterInitialize={handleGridOnAfterInitialize}
                                    />
                                </div>
                            )}
                            to={(<div>
                                <h5 className='form-title'>{t("PAR401_frm_sub_Asociado")}</h5>
                                <Grid
                                    application="PAR400AtributoValidacion"
                                    id="PAR400ValidacionesAso_grid_2"
                                    rowsToFetch={30}
                                    rowsToDisplay={15}
                                    onBeforeInitialize={applyParameters}
                                    onBeforeFetch={applyParameters}
                                    onBeforeFetchStats={applyParameters}
                                    onBeforeMenuItemAction={applyParameters}
                                    onBeforeApplyFilter={applyParameters}
                                    onBeforeApplySort={applyParameters}
                                    onAfterMenuItemAction={handleAfterMenuItemAction}
                                    onBeforeExportExcel={applyParameters}
                                    enableExcelExport
                                    enableSelection
                                />
                            </div>
                            )}
                        />
                    </Form>
                </Container>
            </Modal.Body>
         
            <Modal show={showModal} onHide={handleCloseAtributoValor} backdrop="static">
                <PAR400AtributoValidacionValor
                    show={showAtributoValidacionValor}
                    onHide={handleCloseAtributoValor}
                    listValidaciones={listValidaciones}
                    tipoAtributo={tipoAtributo}
                    nmAtributo={nmAtributo}
                    codigoAtributo={codigoAtributo}
                />
            </Modal>
        </Page>
    );
}