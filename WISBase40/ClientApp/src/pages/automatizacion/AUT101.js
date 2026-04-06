import React from 'react';
import { Page } from '../../components/Page';
import { Grid } from '../../components/GridComponents/Grid';
import { useTranslation } from 'react-i18next';
import { EjecucionesAutomatismoGrid } from './EjecucionesAutomatismoGrid';

export default function AUT101(props) {

    const { t } = useTranslation();

    //const closeEditarEjecucionModal = () => {
    //    setShowPopup(false);
    //}

    return (
        <Page
            application="AUT100Ejecuciones"
            title={t("AUT101_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <EjecucionesAutomatismoGrid id="AUT101_grid_1"/>

        </Page>
    );
}
