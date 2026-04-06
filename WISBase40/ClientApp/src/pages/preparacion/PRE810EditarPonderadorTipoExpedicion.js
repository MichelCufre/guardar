import React from 'react';
import { Button, Modal } from 'react-bootstrap';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useCustomTranslation } from '../../components/TranslationHook';
import { withPageContext } from '../../components/WithPageContext';

function InternalPRE810EditarPonderadorTipoExpedicion(props) {

    const { t } = useCustomTranslation();

    const handleClose = () => {
        props.onHide();
    };

    const onAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("PRE810PonderadorExpedicion_grid_1").refresh();
    };

    const onAfterCommit = (context, rows, parameters, nexus) => {
        nexus.getGrid("PRE810PonderadorExpedicion_grid_1").refresh();
    }

    const addParameters = (context, data, nexus) => {
        data.parameters = [
            {
                id: "nuColaDeTrabajo",
                value: props.nuColaDeTrabajo
            },
        ];
    };


    return (
        <Modal dialogClassName="modal-50w" show={props.show} onHide={props.onHide} >
            <Modal.Header closeButton>
                <Modal.Title>{t("PRE810_Sec0_lbl_Title_TP_EXPEDICION")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Page
                    load
                    {...props}
                >
                    <Grid
                        id="PRE810PonderadorExpedicion_grid_1"
                        application="PRE810PonderadorExpedicion"
                        rowsToFetch={30}
                        rowsToDisplay={10}
                        onBeforeInitialize={addParameters}
                        onAfterMenuItemAction={onAfterMenuItemAction}
                        onBeforeFetch={addParameters}
                        onBeforeMenuItemAction={addParameters}
                        onBeforeApplyFilter={addParameters}
                        onBeforeApplySort={addParameters}
                        onAfterCommit={onAfterCommit}
                        onBeforeCommit={addParameters}
                        onBeforeFetchStats={addParameters}
                    />
                </Page>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="outline-secondary" onClick={handleClose}>
                    {t("General_Sec0_btn_Cerrar")}
                </Button>
            </Modal.Footer>
        </Modal>
    );
}

export const PRE810EditarPonderadorTipoExpedicion = withPageContext(InternalPRE810EditarPonderadorTipoExpedicion);