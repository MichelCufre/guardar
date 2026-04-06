import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { STO730Detalle } from './STO730Detalle';
import { Modal, Row, Col, FormGroup, Button, Tab, Tabs } from 'react-bootstrap';

export default function STO730(props) {

    const { t } = useTranslation();
    const [showModal, setshowModal] = useState(false);
    const [showDetallesAuditoriaDialog, setShowPopupDetallesAuditoriaDialog] = useState(false);
    const [numeroAuditoriaAgrupado, setNumeroAuditoriaAgrupado] = useState(null);
    const onAfterMenuItemAction = (context, data, nexus) => {
        nexus.getGrid("STO730_grid_1").refresh();
    }

    const GridOnBeforeButtonAction = (context, data, nexus) => {

        if (data.buttonId === "btnLineas") {

            context.abortServerCall = true;
            let auditoriaAgrupado = data.row.cells.find(w => w.column == "NU_AUDITORIA_AGRUPADOR").value;
            setNumeroAuditoriaAgrupado(
                auditoriaAgrupado
            );
            openDetallesAuditoriaDialog();

        }
    };
    const openDetallesAuditoriaDialog = () => {
        setShowPopupDetallesAuditoriaDialog(true);
        setshowModal(true);
    }
    const closeDetallesAuditoriaDialog = () => {
        setShowPopupDetallesAuditoriaDialog(false);
        setshowModal(false);
    }
    const closeFormDialog = () => {
        setShowPopupDetallesAuditoriaDialog(false);
        setshowModal(false);
    }
    
    return (
        <Page
            title={t("STO730_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div className="row mb-4">
                <div className="col-12">
                    <Grid
                        id="STO730_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport={true}
                        onAfterMenuItemAction={onAfterMenuItemAction}
                        onBeforeButtonAction={GridOnBeforeButtonAction}
                        enableSelection
                    />
                </div>
            </div>

            <Modal show={showModal} onHide={closeFormDialog} dialogClassName="modal-90w" backdrop="static" >
                <STO730Detalle show={showDetallesAuditoriaDialog} onHide={closeDetallesAuditoriaDialog} numeroAuditoriaAgrupado={numeroAuditoriaAgrupado} />
            </Modal>
        </Page>
    );
}