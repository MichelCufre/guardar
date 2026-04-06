import React, { useState } from 'react';
import { Button, Col, Row } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { notificationType } from '../Enums';
import { withPageContext } from '../WithPageContext';
import withToaster from '../WithToaster';
import { GridImportExcelButton } from './GridImportExcelButton';

function GridImportExcelFileHandlerInternal(props) {
    const { t } = useTranslation("translation", { useSuspense: false });
    const [isDraggingOver, setDragOver] = useState(false);
    const [isUploading, setUploading] = useState(false);

    const handleClose = () => {
        if (!isUploading) props.closeImportExcelModal();
    };

    const handleDrop = (e) => {
        e.preventDefault();
        e.stopPropagation();
        setDragOver(false);

        const file = e.dataTransfer.files[0];

        if (file.type !== "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") {
            props.toaster.toastError(t('IExcel_Modal_lbl_InvalidDataType'));
            return false;
        }

        handleFileUpload(file);
    }

    const importExcel = (fileName, fileData, properties) => {
        var found = props.eventHandlers.find(item => item.id === "importExcel");

        if (found) {
            var functionValue = found.value;
            return functionValue(fileName, fileData, null, null, null, properties)
        }

        return null;
    }

    const generateExcelTemplate = () => {
        var found = props.eventHandlers.find(item => item.id === "generateExcelTemplate");

        if (found) {
            var functionValue = found.value;
            return functionValue()
        }

        return null;
    }


    const handleFileUpload = (file) => {
        setUploading(true);
        const reader = new FileReader();

        reader.onload = (e) => {


            const properties = props.getProperties === null ? undefined : props.getProperties();

            if (properties) {
                importExcel(file.name, reader.result.replace(/^data:.+;base64,/, ''), properties)
                    .then((response) => {
                        setUploading(false);
                        if (response && response.Status === "OK") props.closeImportExcelModal();
                    });
            }
            else {
                importExcel(file.name, reader.result.replace(/^data:.+;base64,/, ''))
                    .then((response) => {
                        setUploading(false);
                        if (response && response.Status === "OK") props.closeImportExcelModal();
                    });
            }

        };

        reader.readAsDataURL(file);
    };

    const handleClickDownloadTemplate = (e) => {
        e.preventDefault();
        generateExcelTemplate();
    }

    const handleDragOver = (e, context, data, nexus) => {
        setDragOver(true);
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

        const extension = file.name.substr(file.name.lastIndexOf('.') + 1)

        if (extension !== "xls" && extension !== "xlsx") {
            props.nexus.toast(notificationType.error, t('Tipo de archivo no válido'));
            return false;
        }

        handleFileUpload(file);
    }

    const handleDrag = (e) => {
        e.stopPropagation();
        e.preventDefault();
    }


    return (
        <div>
            <Row>
                <Col>
                    <Button variant="outline-primary" block="true" onClick={handleClickDownloadTemplate}>{t("IExcel_Modal_lbl_DownloadTemplate")}</Button>
                </Col>
            </Row>
            <Row>
                <Col>
                    <GridImportExcelButton
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
        </div>
    );

}

export const GridImportExcelFileHandler = withToaster(withPageContext(GridImportExcelFileHandlerInternal));

