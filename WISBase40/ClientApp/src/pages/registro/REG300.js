import React, { useState, useRef } from 'react';
import { Page } from '../../components/Page';
import { Grid } from '../../components/GridComponents/Grid';
import { useTranslation } from 'react-i18next';
import { Modal } from 'react-bootstrap';
import { REG300CreateGrupoModal } from './REG300CreateGrupoModal';

export default function REG300(props) {
    const { t } = useTranslation();
    const [showModal, setShowModal] = useState(false);
    const [showPopupGrupo, setShowPopupGrupo] = useState(false);

    const [codigoGrupo, setcodigoGrupo] = useState(null);

    const openFormGrupo = () => {
        setShowModal(true);
        setShowPopupGrupo(true);
    }

    const closeFormDialog = () => {
        setShowPopupGrupo(false);
        setShowModal(false);
    }

    const onAfterCommit = (context, rows, parameters, nexus) => {
        nexus.getGrid("REG300_grid_1").refresh();
    }

    const showGrupo = () => { return (<REG300CreateGrupoModal show={showPopupGrupo} onHide={closeFormDialog} />); }

    return (
        <Page
            title={t("REG300_Sec0_Title_Grupos")}
            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button className="btn btn-primary" onClick={openFormGrupo}>{t("REG300_Sec0_btn_CrearGrupo")}</button>
            </div>

            <Grid
                id="REG300_grid_1"
                rowsToFetch={30}
                rowsToDisplay={15}
                enableExcelExport
                onAfterCommit={onAfterCommit}
            />

            <Modal show={showModal} onHide={closeFormDialog} dialogClassName="modal-70w" backdrop="static" >
                {showPopupGrupo ? showGrupo() : null}
            </Modal>
        </Page>
    );
}