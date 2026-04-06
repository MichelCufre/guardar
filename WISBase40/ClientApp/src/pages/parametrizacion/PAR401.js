import React, { useState } from 'react';
import { Modal } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { PAR401CrearTipoLpn } from './PAR401CrearTipoLpn';
import '../../css/InputCellMayus.css';


export default function PAR401(props) {

    const { t } = useTranslation();
    const [showModal, setshowModal] = useState(false);
    const [showPopupAdd, setShowPopupAdd] = useState(false);
    const [_nexus, setnexus] = useState(null);

    const openFormDialog = () => {
        setShowPopupAdd(true);
        setshowModal(true);
    }
    const closeFormDialog = (datos) => {
        setshowModal(false);
        setShowPopupAdd(false);
        _nexus.getGrid("PAR401_grid_1").refresh();
    }

    const onBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnAtributos") {
            data.parameters.push({ id: "TP_LPN_TIPO", value: data.row.cells.find(w => w.column == "TP_LPN_TIPO").value });
        }
    };

    const onBeforeInitialize = (context, data, nexus) => {
        setnexus(nexus);
    }

    return (
        <Page
            title={t("PAR401_Sec0_pageTitle_Titulo")}

            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button className="btn btn-primary" onClick={openFormDialog}>{t("PAR401_Sec0_btn_CrearTipoLpn")}</button>
            </div>
            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="PAR401_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        onBeforeButtonAction={onBeforeButtonAction}
                        onBeforeInitialize={onBeforeInitialize}
                        enableExcelExport
                        enableExcelImport={false}
                    />
                </div>
            </div>
            <Modal show={showModal} onHide={closeFormDialog} dialogClassName="modal-90w" backdrop="static" >
                <PAR401CrearTipoLpn show={showPopupAdd} onHide={closeFormDialog} />
            </Modal>
        </Page>
    );
}