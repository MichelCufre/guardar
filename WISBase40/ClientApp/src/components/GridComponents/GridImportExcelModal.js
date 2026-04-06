import React from 'react';
import { Button, Modal } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { withPageContext } from '../WithPageContext';
import withToaster from '../WithToaster';
import { GridImportExcelFileHandler } from './GridImportExcelFileHandler';

function GridImportExcelModalInternal(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    const handleClose = () => {
        props.closeImportExcelModal();
    };

    const eventHandlers = [
        { id: "importExcel", value: props.importExcel },
        { id: "generateExcelTemplate", value: props.generateExcelTemplate },
        { id: "closeImportExcelModal", value: handleClose },
    ];

    if (props.importExcelCustom && props.isImportExcelModalOpen) {

        return (props.importExcelCustom(eventHandlers, props.isImportExcelModalOpen));
    }

    return (
        <Modal show={props.isImportExcelModalOpen} size="lg" onHide={handleClose} className="gr-import-excel">
            <Modal.Header closeButton>
                <Modal.Title>{t("IExcel_Modal_lbl_Title")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>

                <GridImportExcelFileHandler
                    getProperties={null}
                    eventHandlers={eventHandlers}
                    closeImportExcelModal={handleClose}
                />

            </Modal.Body>
            <Modal.Footer>
                <Button variant="secondary" onClick={handleClose}>
                    {t("General_Sec0_btn_LOAD_FILTER_CLOSE")}
                </Button>
            </Modal.Footer>
        </Modal>
    );
}

export const GridImportExcelModal = withToaster(withPageContext(GridImportExcelModalInternal));