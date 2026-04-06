import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { Row, Col, Container } from 'react-bootstrap';
import { IMP010DetalleImpresionModal } from './IMP010DetalleImpresionModal';

export default function IMP010(props) {

    const { t } = useTranslation();

    const [showPopupDetalle, setShowPopupDetalle] = useState(false);

    const [infoImpresion, setInfoImpresion] = useState(null);


    const openFormDialog = () => {
        setShowPopupDetalle(true);

    }

    const closeFormDialog = () => {
        setShowPopupDetalle(false);
    }



    const GridOnBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnDetalle") {
            context.abortServerCall = true;

            setInfoImpresion(
                [
                    { id: "impresion", value: data.row.cells.find(w => w.column == "NU_IMPRESION").value }
                ]
            );

            openFormDialog();
        } else {
            nexus.getGrid("IMP010_grid_1").refresh();
        }
    };

    return (

        <Page
            title={t("IMP010_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="IMP010_grid_1" application="IMP010" rowsToFetch={30} rowsToDisplay={15} enableExcelExport
                        onBeforeButtonAction={GridOnBeforeButtonAction}
                    />
                </div>
            </div>

            <IMP010DetalleImpresionModal show={showPopupDetalle} onHide={closeFormDialog} impresion={infoImpresion} />
        </Page>
    );
}