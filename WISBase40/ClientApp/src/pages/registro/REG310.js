import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Page } from '../../components/Page';
import { Modal } from 'react-bootstrap';
import { Grid } from '../../components/GridComponents/Grid';
import { REG310ReglasCreateModal } from './REG310ReglasCreateModal';
import { REG310ReglasUpdateModal } from './REG310ReglasUpdateModal';

export default function REG310Reglas(props) {
    const { t } = useTranslation();

    const [nuRegla, setRegla] = useState("");
    const [showModal, setShowModal] = useState(false);
    const [showPopupAddRegla, setShowPopupAddRegla] = useState(false);
    const [showPopupUpdateRegla, setShowPopupUpdateRegla] = useState(false);

    const openFormAddRegla = () => {
        setShowPopupAddRegla(true);
        setShowPopupUpdateRegla(false)
        setShowModal(true);
    }

    const openFormUpdateRegla = () => {
        setShowPopupUpdateRegla(true)
        setShowPopupAddRegla(false);
        setShowModal(true);
    }

    const closeFormDialog = (nexus) => {
        setShowPopupAddRegla(false);
        setShowPopupUpdateRegla(false);
        setShowModal(false);

        if (nexus) {
            nexus.getGrid("REG310_grid_1").refresh();
        }
    };

    const onAfterButtonAction = (data, nexus) => {
        if (data.buttonId === "btnDown" || data.buttonId === "btnUp") {
            nexus.getGrid("REG310_grid_1").refresh();
        }
    }

    const onBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnEditar") {
            context.abortServerCall = true;
            setRegla(data.row.cells.find(w => w.column == "NU_GRUPO_REGLA").value);
            openFormUpdateRegla();
        }
    }

    const onAfterCommit = (context, rows, parameters, nexus) => {
        nexus.getGrid("REG310_grid_1").refresh();
    }

    const showFormAddRegla = () => {
        return (<REG310ReglasCreateModal show={showPopupAddRegla} onHide={closeFormDialog} />);
    }

    const showFormUpdateRegla = () => {
        return (<REG310ReglasUpdateModal show={showPopupUpdateRegla} onHide={closeFormDialog} nuRegla={nuRegla} />);
    }

    return (

        <Page
            title={t("REG300_Sec0_Title_Reglas")}
            {...props}
        >

            <div style={{ textAlign: "center" }}>
                <button className="btn btn-primary" onClick={openFormAddRegla}>{t("REG300_Sec0_btn_AgregarRegla")}</button>
            </div>

            <Grid
                application="REG310"
                id="REG310_grid_1"
                rowsToFetch={30}
                rowsToDisplay={15}
                enableExcelExport
                onAfterButtonAction={onAfterButtonAction}
                onBeforeButtonAction={onBeforeButtonAction}
                onAfterCommit={onAfterCommit}
            />

            <Modal show={showModal} onHide={closeFormDialog} dialogClassName="modal-70w" backdrop="static" >
                {showPopupAddRegla ? showFormAddRegla() : null}
                {showPopupUpdateRegla ? showFormUpdateRegla() : null}
            </Modal>
        </Page>
    );
}