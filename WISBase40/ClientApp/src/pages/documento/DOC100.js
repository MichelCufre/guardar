import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { DOC100Create } from './DOC100Create';
import { DOC100Update } from './DOC100Update';

export default function DOC100(props) {
    const { t } = useTranslation();

    const [showPopup, setShowPopup] = useState(false);
    const [showUpdatePopup, setShowUpdatePopup] = useState(false);
    const [nroDocPrep, setNroDocPrep] = useState(null);

    const openFormDialog = () => {
        setShowPopup(true);
    }

    const closeFormDialog = (datos) => {
        setShowPopup(false);
    }

    const closeUpdateFormDialog = () => {
        setShowUpdatePopup(false);
    }

    const onAfterButtonAction = (data, nexus) => {
        nexus.getGrid("DOC100_grid_1").refresh();
    }

    const onBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnEditar") {
            context.abortServerCall = true;
            setNroDocPrep(data.row.cells.find(d => d.column === "NU_DOCUMENTO_PREPARACION").value);
            setShowUpdatePopup(true);
        }
    }

    const onAfterCommit = (context, rows, parameters, nexus) => {
        nexus.getGrid("DOC100_grid_1").refresh();
    }

    return (
        <Page
            title={t("DOC100_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button className="btn btn-primary" onClick={openFormDialog}>{t("DOC100_Sec0_btn_AsociarDocumentos")}</button>
            </div>
            <div className="row mb-4">
                <div className="col-12">
                    <Grid
                        id="DOC100_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        onAfterButtonAction={onAfterButtonAction}
                        onBeforeButtonAction={onBeforeButtonAction}
                        onAfterCommit={onAfterCommit}
                    />
                </div>
            </div>
            <DOC100Create show={showPopup} onHide={closeFormDialog} />
            <DOC100Update show={showUpdatePopup} onHide={closeUpdateFormDialog} nroDocPrep={nroDocPrep}/>
        </Page>
    );
}