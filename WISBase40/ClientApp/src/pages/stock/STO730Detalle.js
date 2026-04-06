import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { SubmitButton, Form } from '../../components/FormComponents/Form';
import { useTranslation } from 'react-i18next';
import { Modal, Container } from 'react-bootstrap';
import { STO730DetalleAtributos } from './STO730DetalleAtributos'
export function STO730Detalle(props) {

    const { t } = useTranslation();
    const [showModalAtributos, setShowModalAtributos] = useState(false);
    const [numeroAuditoria, setNumeroAuditoria] = useState(false);
    const [opacityIsActive, setOpacityIsActive] = useState(false);
    const [isActiveButton, setIsActiveButton] = useState(false);

    const handleCloseModalAtributos = () => {
        setOpacityIsActive(false);
        setShowModalAtributos(false);
    }

    const GridOnBeforeButtonAction = (context, data, nexus) => {
        context.abortServerCall = true;
        let auditoria = data.row.cells.find(w => w.column == "NU_AUDITORIA").value;
        setNumeroAuditoria(
            auditoria
        );
        openFormDialog();
    }
    const openFormDialog = () => {
        setShowModalAtributos(true);
        setOpacityIsActive(true);
    }
    const addParameters = (context, data, nexus) => {
        data.parameters = [
            { id: "numeroAuditoriaAgrupado", value: props.numeroAuditoriaAgrupado }
        ];
    }
    const addParametersForm = (context, form, data, nexus) => {
        data.parameters.push({ id: "numeroAuditoriaAgrupado", value: props.numeroAuditoriaAgrupado });
    }
    const onAfterInitialize = (context, grid, query, nexus) => {
        let isActiveButton = query.parameters.find(p => p.id === "isDisableButton").value == "T" ? true : false;
        setIsActiveButton(isActiveButton);

    };
    const handleFormBeforeSubmit = (context, form, query, nexus) => {

        query.parameters = [
            { id: "numeroAuditoriaAgrupado", value: props.numeroAuditoriaAgrupado },
        ];

    }

    const onAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("STO730Detalle_grid_1").refresh();
    }

    const onAfterSubmit = (context, form, query, nexus) => {
        nexus.getGrid("STO730Detalle_grid_1").refresh();

        props.onHide();
    }

    return (
        <div className={!opacityIsActive ? '' : 'hidden'}>
            <Page
                {...props}
            >

                <Modal.Header closeButton>
                    <Modal.Title>{t("STO730Detalle_Sec0_modalTitle_Titulo")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Container fluid>
                        <div className="row mb-4">
                            <div className="col-12">
                                <Grid
                                    application="STO730Detalle"
                                    id="STO730Detalle_grid_1"
                                    rowsToFetch={30}
                                    rowsToDisplay={15}
                                    enableExcelExport={true}
                                    onBeforeButtonAction={GridOnBeforeButtonAction}
                                    enableSelection
                                    onBeforeApplyFilter={addParameters}
                                    onBeforeApplySort={addParameters}
                                    onBeforeFetch={addParameters}
                                    onBeforeFetchStats={addParameters}
                                    onBeforeExportExcel={addParameters}
                                    onBeforeInitialize={addParameters}
                                    onBeforeMenuItemAction={addParameters}
                                    onAfterMenuItemAction={onAfterMenuItemAction}
                                />
                            </div>
                        </div>
                    </Container>
                </Modal.Body>
                <Form
                    application="STO730Detalle"
                    id="STO730Detalle_form_AuditoriaLpn"
                    onBeforeInitialize={addParametersForm}
                    onAfterInitialize={onAfterInitialize}
                    onBeforeSubmit={handleFormBeforeSubmit}
                    onAfterSubmit={onAfterSubmit}
                >

                    <div className={!isActiveButton ? '' : 'hidden'}>
                        <Modal.Footer                    >
                            <SubmitButton id="btnRechazar" variant="primary" label="STO730_frm1_btn_Rechazar" />
                            <SubmitButton id="btnAprobar" variant="primary" label="STO730_frm1_btn_Aprobar" />
                        </Modal.Footer>
                    </div>
                </Form >
                <Modal show={showModalAtributos} dialogClassName="modal-50w" onHide={handleCloseModalAtributos} backdrop="static">
                    <STO730DetalleAtributos numeroAuditoria={numeroAuditoria} />
                </Modal>

            </Page>
        </div>
    );
}