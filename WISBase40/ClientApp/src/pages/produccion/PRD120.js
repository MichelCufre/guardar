import React, { useRef, useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { PRD120CreateInstanciaAccionModal } from './PRD120CreateInstanciaAccionModal';

export default function PRD120(props) {
    const { t } = useTranslation();

    const [showPopup, setShowPopup] = useState(false);

    const openFormDialog = () => {
        setShowPopup(true);
    }

    const closeFormDialog = () => {
        setShowPopup(false);
    }

    return (
        <Page
            load
            icon="fas fa-list"
            title={t("PRD120_Sec0_pageTitle_Titulo")}
            {...props}
        >

            <div style={{ textAlign: "center" }}>
                <button className="btn btn-primary" onClick={openFormDialog}>{t("PRD120_Sec0_btn_AbrirModal")}</button>
            </div>

            <Grid
                id="PRD120_grid_1"
                rowsToFetch={30}
                rowsToDisplay={15}
                enableExcelExport
                enableExcelImport={false}
            />

            <PRD120CreateInstanciaAccionModal show={showPopup} onHide={closeFormDialog} />

        </Page>
    );
}