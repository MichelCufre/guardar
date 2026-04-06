import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import INV418 from './INV418';

export default function INV412(props) {
    const { t } = useTranslation();
    const [showPopupAtributos, setShowPopupAtributos] = useState(false);
    const [nuInventarioDetalle, setNuInventarioDetalle] = useState("");

    const openPopupAtributos = () => {
        setShowPopupAtributos(true);
    }

    const closePopupAtributos = () => {
        setShowPopupAtributos(false);
    }

    const onBeforeButtonAction = (context, data, nexus) => {
        if (data.buttonId === "btnAtributosDetalleTemporal") {
            context.abortServerCall = true;
            setNuInventarioDetalle(data.row.cells.find(w => w.column == "NU_INVENTARIO_ENDERECO_DET").value);
            openPopupAtributos();
        }
    }

    return (

        <Page
            title={t("INV412_Sec0_pageTitle_Titulo")}
            application="INV412"
            {...props}
        >
            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="INV412_grid_1"
                        application="INV412"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        onBeforeButtonAction={onBeforeButtonAction}
                    />
                </div>
            </div>

            <INV418 show={showPopupAtributos} onHide={closePopupAtributos} nuInventarioDetalle={nuInventarioDetalle} />

        </Page>
    );
}