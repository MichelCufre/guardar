import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { Modal, Row, Col, Container } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { CON010ParametrosConfig } from './CON010ParametrosConfig';


export default function CON010(props) {

    const { t } = useTranslation();

    const [showModal, setShowModal] = useState(false);

    const [infoParametroConfiguracion, setinfoParametroConfiguracion] = useState(null);

    const openFormDialog = () => {
        setShowModal(true);
    }

    const closeFormDialog = (nexus) => {
        setShowModal(false);

        if (nexus)
            nexus.getGrid("CON010_grid_1").refresh();

        setShowModal(
            null
        );
    }

    const GridOnBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnCargarValores") {

            context.abortServerCall = true;

            setinfoParametroConfiguracion(
                [
                    { id: "cdParametro", value: data.row.cells.find(w => w.column == "CD_PARAMETRO").value },
                    { id: "doEntidad", value: data.row.cells.find(w => w.column == "DO_ENTIDAD_PARAMETRIZABLE").value },
                    { id: "dsDominio", value: data.row.cells.find(w => w.column == "DS_DOMINIO").value },
                ]
            );

            openFormDialog();
        };
    }

    return (
        <Page
            title={t("CON010_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="CON010_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport enableExcelImport={false}
                        onBeforeButtonAction={GridOnBeforeButtonAction}
                    />
                </div>
            </div>

            <CON010ParametrosConfig show={showModal} onHide={closeFormDialog} ParametroConfiguracion={infoParametroConfiguracion} />

        </Page>
    );
}