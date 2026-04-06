import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { IMP080TransferenciaModal } from '../impresion/IMP080TransferenciaModal';

export default function STO498(props) {

    const { t } = useTranslation();


    const [showPopupImprimir, setShowPopupImprimir] = useState(false);

    const [rowSeleccionadasImprimir, setRowSeleccionadasImprimir] = useState(null);

    const [tipoImpresion, setTipoImpresion] = useState(null);

    const openImprimirDialog = () => {
        setTipoImpresion("Generar");
        setShowPopupImprimir(true);
    }
    
    const openImprimirDialogBotonReimprimir = () => {
        setTipoImpresion("Reimprimir");
        setShowPopupImprimir(true);

    }

    const closeImprimirDialog = () => {
        setShowPopupImprimir(false);
    }

    const GridOnAfterMenuItemAction = (context, data, nexus) => {

        let jsonAdded = data.parameters.find(w => w.id === "ListaFilasSeleccionadas").value;

        setRowSeleccionadasImprimir(jsonAdded);

        openImprimirDialogBotonReimprimir();
    }



    return (
        <Page
            icon="fas fa-cubes"
            title={t("STO498_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button id="btnReimprimir" className="btn btn-primary" onClick={openImprimirDialog}>{t("STO498_Sec0_btn_GenerarEtiqueta")}</button>
            </div>
            <div className="row mb-4">
                <div className="col">
                    <Grid
                        id="STO498_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        enableSelection
                        onAfterMenuItemAction={GridOnAfterMenuItemAction}

                    />
                </div>
            </div>
            <IMP080TransferenciaModal show={showPopupImprimir} onHide={closeImprimirDialog} rowSeleccionadas={rowSeleccionadasImprimir} tipoImpresion={tipoImpresion} />
        </Page>
    );
}


