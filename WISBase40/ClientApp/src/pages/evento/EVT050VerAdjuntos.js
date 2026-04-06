import React from 'react';
import { Button, Modal } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { useFileDownloader } from '../../components/FileHook';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useReportDownloader } from '../../components/ReportHook';

export function EVT050VerAdjuntos(props) {
    const { t } = useTranslation();

    const reportDownloader = useReportDownloader();
    const fileDownloader = useFileDownloader();

    const initialValues = {
    };

    const validationSchema = {
    };

    const handleClose = () => {
        props.onHide();
    }

    const addParameters = (context, data, nexus) => {
        data.parameters = [
            {
                id: "notificacion",
                value: props.notificacion
            }
        ];
    };

    const onBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnDescargar") {
            var tipoReferencia = data.row.cells.find(d => d.column === "TP_REFERENCIA").value;
            var idReferencia = data.row.cells.find(d => d.column === "ID_REFERENCIA").value

            context.abortServerCall = true

            if (tipoReferencia == 'REPORTE') {
                reportDownloader.downloadReport(idReferencia);
            } else {
                var fileId = tipoReferencia + '.' + idReferencia;
                fileDownloader.downloadFile(fileId);
            }
        }
    }

    return (
        <Page
            {...props}
        >
            <Modal show={props.show} onHide={handleClose} dialogClassName="modal-70w" backdrop="static">
                <Modal.Header closeButton>
                    <Modal.Title>{t("EVT050_Sec0_btn_VerAdjuntos")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <div className="row mb-4">
                        <div className="col-12">
                            <Grid id="EVT050_grid_1"
                                application="EVT050VerAdjuntos"
                                rowsToFetch={30}
                                rowsToDisplay={15}
                                enableExcelExport
                                onBeforeInitialize={addParameters}
                                onBeforeFetchStats={addParameters}
                                onBeforeFetch={addParameters}
                                onBeforeApplyFilter={addParameters}
                                onBeforeApplySort={addParameters}
                                onBeforeExportExcel={addParameters}
                                onBeforeButtonAction={onBeforeButtonAction}
                            />
                        </div>
                    </div>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("EVT050VerAdjuntos_frm1_btn_Cerrar")} </Button>
                </Modal.Footer>
            </Modal>
        </Page>
    );
}