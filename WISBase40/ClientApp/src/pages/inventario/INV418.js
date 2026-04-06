import React from 'react';
import { useTranslation } from 'react-i18next';
import { Modal, Button} from 'react-bootstrap';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';

export default function INV418(props) {
    const { t } = useTranslation();

    const handleClose = () => {
        props.onHide(null, props.nexus);
    };

    const applyParameters = (context, data, nexus) => {
        data.parameters = [
            { id: "nuInventarioDetalle", value: props.nuInventarioDetalle },
        ];
    };

    return (
        <Modal show={props.show} onHide={handleClose} dialogClassName="modal-70w" backdrop="static">
            <Page
                application="INV418"
                {...props}
            >
                <Modal.Header closeButton>
                    <Modal.Title>{t("INV418_lbl_Title_AtributosDetalle")}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Grid application="INV418"
                        id="INV418_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        onBeforeFetch={applyParameters}
                        onBeforeInitialize={applyParameters}
                        onBeforeExportExcel={applyParameters}
                        onBeforeFetchStats={applyParameters}
                        onBeforeApplyFilter={applyParameters}
                        onBeforeApplySort={applyParameters}
                    />
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="btn btn-outline-secondary" onClick={handleClose}> {t("General_Sec0_btn_Cerrar")} </Button>
                </Modal.Footer>
            </Page>
        </Modal >

    );
}