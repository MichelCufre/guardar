import React, { useState } from 'react';
import { Button, Col, Modal, Row, Container } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { withPageContext } from './WithPageContext';
import withToaster from './WithToaster';
import { FileUploadComponentButton } from './FileUploadComponentButton';
import { Grid } from './GridComponents/Grid';
import { withTranslation } from 'react-i18next';

function FileUploadComponentModalInternal(props) {
    const { t } = useTranslation("translation", { useSuspense: false });
    const [isDraggingOver, setDragOver] = useState(false);
    const [isUploading, setUploading] = useState(false);

    const handleClose = () => {
        if (!isUploading)
            props.onHide();
    };

    const handleDrop = (e) => {
        e.preventDefault();
        e.stopPropagation();

        setDragOver(false);

        var file = e.dataTransfer.files[0];

        if (props.permiteAlta) {
            const reader = new FileReader();

            setUploading(true);

            reader.onload = function (e) {

                const size = file.size / 1048576;

                uploadFile(file.name, reader.result, size)
                    .then((response) => {
                        setUploading(false);
                    });
            };

            reader.readAsDataURL(file);
        }
    }

    const handleDragOver = (e) => {
        setDragOver(true);

        e.preventDefault();
    }

    const handleDrag = (e) => {
        e.stopPropagation();
        e.preventDefault();
    }

    const handleDragLeave = (e) => {
        setDragOver(false);
        e.preventDefault();
    }

    const handleChange = (e) => {
        e.preventDefault();
        e.stopPropagation();

        const file = e.target.files[0];

        processFiles(file);
    }

    const processFiles = (file) => {
        setDragOver(false);

        console.log(file);

        if (props.permiteAlta) {
            const reader = new FileReader();

            setUploading(true);

            reader.onload = function (e) {

                const size = file.size / 1048576;

                uploadFile(file.name, reader.result, size)
                    .then((response) => {
                        setUploading(false);
                    });
            };

            reader.readAsDataURL(file);
        }
    }

    const uploadFile = (filename, payloadData, size) => {

        try {
            const data = {
                FileName: filename,
                Size: size,
                //Nos quedamos solo con el base64
                Payload: payloadData.split(',').pop(),
            };

            return gridUploadFile(data).then(uploadFileProcessResponse);
        }
        catch (ex) {
            console.error(ex);
        }
    };

    const uploadFileProcessResponse = (response) => {
        try {

            if (!response)
                return false;

            if (response.Status === "OK") {
                const data = JSON.parse(response.Data);

                let rows = data.rows;

                const context = {
                    abortUpdate: false
                };

                if (onAfterUploadFileSuccess)
                    onAfterUploadFileSuccess(context, rows, data, props.nexus);

                if (context.abortUpdate) {
                    return false;
                }

                return response;
            }
            else if (response.Status === "ERROR") {

                props.toaster.toastError(response.Message);
                return response;
            }
        }
        catch (ex) {
            console.error(ex);
        }
    };

    const gridUploadFile = (data) => {

        const request = {
            method: "POST",
            cache: "no-cache",
            headers: {
                "Content-Type": "application/json",
                "X-Requested-With": "XMLHttpRequest"
            },
            body: JSON.stringify({
                filename: data.FileName,
                payload: data.Payload,
                size: data.Size,
                tipoEntidad: props.tipoEntidad,
                codigoEntidad: props.codigoEntidad
            })
        };

        return fetch("api/File/Upload", request)
            .then(response => {
                if (response.status === 401) {
                    window.location = "/api/Security/Logout";

                    return null;
                }

                return response;
            })
            .then((response) => response ? response.json() : response);
    };

    const onAfterUploadFileSuccess = (context, rows, data, nexus) => {
        nexus.getGrid("FUC001_grid_1").refresh();
    }

    const onBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnDescargar") {
            var fileId = data.row.cells.find(d => d.column === "CD_ARCHIVO").value;
            downloadFile(fileId);
        }

        if (data.buttonId === "btnBorrar") {
            if (props.permiteBaja) {
                var fileId = data.row.cells.find(d => d.column === "CD_ARCHIVO").value;
                deleteFile(fileId);
            }
        }
    }

    const onAfterButtonAction = (data, nexus) => {
        if (data.buttonId === "btnBorrar") {
            nexus.getGrid("FUC001_grid_1").refresh();
        }
    }

    const downloadFile = (fileId) => {
        const path = window.location.pathname;
        const application = path.substring(path.lastIndexOf('/') + 1);
        const link = document.createElement('a');

        link.style.display = 'none';
        link.target = "_blank";
        link.href = "/api/File/Download?fileId=" + fileId + "&application=" + application;

        document.body.appendChild(link);

        link.click();

        link.parentNode.removeChild(link);
    };

    const deleteFile = (fileId) => {
        const path = window.location.pathname;

        const application = path.substring(path.lastIndexOf('/') + 1);

        const request = {
            method: "POST",
            cache: "no-cache",
            headers: {
                "Content-Type": "application/json",
                "X-Requested-With": "XMLHttpRequest"
            },
            body: JSON.stringify({
                application: application,
                fileId: fileId
            })
        };

        return fetch("api/File/Delete", request)
            .then(response => {
                if (response.status === 401) {
                    window.location = "/api/Security/Logout";

                    return null;
                }

                if (response.status === 403) {
                    throw new Error(response.status);
                }

                return response;
            })
            .then(response => response ? response.json() : response)

    };

    const addParameters = (context, data, nexus) => {
        data.parameters = [
            { id: "tipoEntidad", value: props.tipoEntidad },
            { id: "codigoEntidad", value: props.codigoEntidad },
            { id: "permiteAlta", value: props.permiteAlta },
            { id: "permiteBaja", value: props.permiteBaja }
        ];
    }

    const renderCamposClave = () => {
        var keys = props.codigoEntidad.split('$');
        if (props.camposClave && props.camposClave.length > 0 && props.camposClave.length == keys.length) {
            return (
                <Container fluid>
                    <div>
                        {props.camposClave.map((campo, i) => {
                            return (
                                <Row>
                                    <Col>
                                        <span style={{ fontWeight: "bold" }}>{t(campo)}: </span>
                                    </Col>
                                    <Col className='p-0'>
                                        <span>{keys[i]}</span>
                                    </Col>
                                </Row>
                            );
                        })}
                    </div>
                    <br />
                </Container>
            );
        } else {
            return (null);
        }
    }

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-50w" backdrop="static" className="gr-import-file">
            <Modal.Header closeButton>
                <Modal.Title>{t("General_Sec0_btn_Documentos")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                {renderCamposClave()}
                <Row style={{ display: props.permiteAlta ? 'block' : 'none' }}>
                    <Col>
                        <FileUploadComponentButton
                            onDragOver={handleDragOver}
                            onDragLeave={handleDragLeave}
                            onDragEnter={handleDrag}
                            onDragEnd={handleDrag}
                            onDrop={handleDrop}
                            onChange={handleChange}
                            isDraggingOver={isDraggingOver}
                            isUploading={isUploading}
                        />
                    </Col>
                </Row>
                <Row>
                    <div className="col-12">
                        <Grid
                            application="FUC001"
                            id="FUC001_grid_1"
                            rowsToFetch={30}
                            rowsToDisplay={15}
                            enableExcelExport={true}
                            onBeforeInitialize={addParameters}
                            onBeforeFetch={addParameters}
                            onBeforeFetchStats={addParameters}
                            onBeforeExportExcel={addParameters}
                            onBeforeButtonAction={onBeforeButtonAction}
                            onAfterButtonAction={onAfterButtonAction}
                        />
                    </div>
                </Row>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="secondary" onClick={handleClose}>
                    {t("General_Sec0_btn_LOAD_FILTER_CLOSE")}
                </Button>
            </Modal.Footer>
        </Modal>
    );
}

export const FileUploadComponentModal = withTranslation()(withToaster(withPageContext(FileUploadComponentModalInternal)));