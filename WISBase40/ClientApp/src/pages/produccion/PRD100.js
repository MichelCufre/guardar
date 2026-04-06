import React, { useState, useRef, useEffect } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { PRD100CreateFormulaModal } from './PRD100CreateFormulaModal';
import { PRD100UpdateFormulaModal } from './PRD100UpdateFormulaModal';

export default function PRD100(props) {
    const { t } = useTranslation();
    const [showAddPopup, setShowAddPopup] = useState(false);
    const [showUpdatePopup, setShowUpdatePopup] = useState(false);
    const [formula, setFormula] = useState("");

    const openFormDialog = () => {
        setShowAddPopup(true);
    }

    const closeFormDialog = () => {
        setShowAddPopup(false);
    }
    const closeFormUpdateDialog = () => {
        setShowUpdatePopup(false);
    }
    const handleGridBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "editar") {
            context.abortServerCall = true;

            setFormula(data.row.cells.find(d => d.column === "CD_PRDC_DEFINICION").value);

            setShowUpdatePopup(true);
        }
    }

    const onAfterCommit = (context, rows, parameters, nexus) => {
        nexus.getGrid("PRD100_grid_1").refresh();
    }
    return (
        <Page
            title={t("PRD100_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button className="btn btn-primary" onClick={openFormDialog}>{t("PRD100_modal_btn_Abrir")}</button>
            </div>

            <hr />
            <Grid
                id="PRD100_grid_1"
                rowsToFetch={30}
                rowsToDisplay={15}
                enableExcelExport
                enableExcelImport={false}
                onBeforeButtonAction={handleGridBeforeButtonAction}
                onAfterCommit={onAfterCommit}
            />

            <PRD100CreateFormulaModal show={showAddPopup} onHide={closeFormDialog} />
            <PRD100UpdateFormulaModal show={showUpdatePopup} onHide={closeFormUpdateDialog} formula={formula} />
        </Page>
    );
}