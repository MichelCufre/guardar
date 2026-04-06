import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../../components/WithPageContext';
import { Form, Field, FieldSelect, StatusMessage, SubmitButton } from '../../components/FormComponents/Form';
import { Modal, Button, Row, Col, Container } from 'react-bootstrap';
import * as Yup from 'yup';

function InternalIMP080ImpresionEtiquetasUTRecModal(props) {

    const { t } = useTranslation();


    const [modalShowImpresora, setModalShowImpresora] = useState(false);

    const [rowSeleccionadasImprimir, setRowSeleccionadasImprimir] = useState(null);
    const [showPopupRemp, setshowPopupRemp] = useState(false);
    const applyParameters = (context, data, nexus) => {
        if (props.agenda)
            data.parameters = [{ id: "agenda", value: props.agenda.find(x => x.id === "idAgenda").value }];
    }

    const GridOnAfterMenuItemAction = (context, data, nexus) => {
        let jsonAdded = data.parameters.find(w => w.id === "ListaFilasSeleccionadas").value;

        setRowSeleccionadasImprimir(jsonAdded);

        changeModal();
    }

    const changeModal = () => {
        setModalShowImpresora(true);
        setshowPopupRemp(true);
    }

    const handleClose = () => {
        setModalShowImpresora(false);
        setshowPopupRemp(false);
        props.onHide();
    };


    const addParameters = (context, data, nexus) => {
        data.parameters = [
            {
                id: "ETIQUETAS_IMP",
                value: rowSeleccionadasImprimir
            }
        ];
    };
    const addParametersForm = (context, form, data, nexus) => {
        data.parameters = [
            {
                id: "ETIQUETAS_IMP",
                value: rowSeleccionadasImprimir
            }
        ];
    };

    return (
        <Page
            application="IMP080ImpresionEtiqRec"
            {...props}
        >
            <Modal show={props.show && !modalShowImpresora} onHide={handleClose} dialogClassName="modal-90w" backdrop="static">
                <Modal.Header closeButton>
                    <Modal.Title>{t("REC170ImpresionEtiq_Sec0_mdl_ImpresionEtiqModal_Titulo")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Page
                        application="REC170ImpresionEtiqUT"
                        {...props}
                    >
                        <Grid
                            application="REC170ImpresionEtiqUT"
                            id="REC170ImpresionEtiqUT_grid_1"
                            rowsToFetch={30}
                            rowsToDisplay={15}
                            enableExcelExport
                            enableSelection
                            onBeforeInitialize={applyParameters}
                            onBeforeFetch={applyParameters}
                            onBeforeExportExcel={applyParameters}
                            onBeforeButtonAction={applyParameters}
                            onBeforeMenuItemAction={applyParameters}
                            onBeforeFetchStats={applyParameters}
                            onBeforeApplyFilter={applyParameters}
                            onBeforeApplySort={applyParameters}
                            onAfterMenuItemAction={GridOnAfterMenuItemAction}
                            onAfterValidateRow={applyParameters}
                        />
                    </Page>
                </Modal.Body>
            </Modal>
        </Page>
    );
}

export const IMP080ImpresionEtiquetasUTRecModal = withPageContext(InternalIMP080ImpresionEtiquetasUTRecModal);