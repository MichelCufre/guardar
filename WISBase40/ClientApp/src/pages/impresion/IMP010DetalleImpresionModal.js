import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../../components/WithPageContext';
import { Form, Field, FieldSelect, FieldSelectAsync, FieldTextArea, StatusMessage, SubmitButton, FieldDate, FieldDateTime } from '../../components/FormComponents/Form';
import { Modal, Button, Row, Col, Tab, Tabs, Container } from 'react-bootstrap';
import * as Yup from 'yup';

function InternalIMP010DetalleImpresion(props) {

    const { t } = useTranslation();

    const [infoImpresion, setInfoImpresion] = useState({
        numero: ""
    });

    const [infoDisplayed, setInfoDisplayed] = useState(false);

    const addParameters = (context, data, nexus) => {
        if (props.impresion) {
            let parameters =
                [
                    { id: "impresion", value: props.impresion.find(x => x.id === "impresion").value }

                ];

            data.parameters = parameters;
        }
    };

    const onAfterInitialize = (context, data, nexus) => {
        if (props.impresion) {
            setInfoImpresion({
                numero: props.impresion.find(x => x.id === "impresion").value
            });
            setInfoDisplayed(true);
        } else {
            setInfoDisplayed(false);
        }
    };

    const handleClose = () => {
        props.onHide();
    };

    return (

        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-90w" backdrop="static">
            <Modal.Header closeButton>
                <Modal.Title>{t("IMP010DetalleImpresion_Sec0_mdl_Titulo")} - {t("IMP010DetalleImpresion_frm1_lbl_Impresion")} {infoImpresion.numero} </Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Page
                    application="IMP010DetImpresion"
                    {...props}
                >
                    <div className="row mb-4">
                        <div className="col-12">
                            <Grid id="IMP010DetImpresion_grid_1" application="IMP010DetImpresion" rowsToFetch={30} rowsToDisplay={10} enableExcelExport
                                onBeforeInitialize={addParameters}
                                onBeforeFetchStats={addParameters}
                                onBeforeExportExcel={addParameters}
                                onAfterInitialize={onAfterInitialize}
                                onBeforeFetch={addParameters}
                                onBeforeApplyFilter={addParameters}
                                onBeforeApplySort={addParameters}
                            />
                        </div>
                    </div>
                </Page>
                <Modal.Footer>
                    <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("IMP010DetalleImpresion_frm1_btn_cerrar")} </Button>

                </Modal.Footer>
            </Modal.Body>
        </Modal>
    );
}

export const IMP010DetalleImpresionModal = withPageContext(InternalIMP010DetalleImpresion);