import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { IMP200ImpresionUTModal } from '../impresion/IMP200ImpresionUTModal'

export default function STO750(props) {
    const { t } = useTranslation();

    const [showPopupImprimir, setShowPopupImprimir] = useState(false);

    const [selectedKeys, setSelectedKeys] = useState(null);

    const [reimprimir, setTipoImpresion] = useState(false);

    const openImprimirDialog = () => {
        setTipoImpresion(false);
        setShowPopupImprimir(true);
    }

    const openReimprimirDialog= () => {
        setTipoImpresion(true);
        setShowPopupImprimir(true);
    }

    const closeImprimirDialog = () => {
        setShowPopupImprimir(false);
    }

    const onAfterMenuItemAction = (context, data, nexus) => {

        let selectedKeys = data.parameters.find(w => w.id === "selectedKeys").value;

        setSelectedKeys(selectedKeys);
        openReimprimirDialog();
    }



    return (
        <Page            
            title={t("STO750_Sec0_pageTitle_Titulo")}
            application="STO750"

            {...props}
        >
            <div style={{ textAlign: "center" }}>
                <button id="btnImprimir" className="btn btn-primary" onClick={openImprimirDialog}>{t("STO750_Sec0_btn_GenerarEtiqueta")}</button>
            </div>
            <div className="row mb-4">
                <div className="col">
                    <Grid
                        id="STO750_grid_1"
                        application="STO750"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        enableSelection
                        onAfterMenuItemAction={onAfterMenuItemAction}

                    />
                </div>
            </div>
            <IMP200ImpresionUTModal show={showPopupImprimir} onHide={closeImprimirDialog} selectedKeys={selectedKeys} reimprimir={reimprimir} />
        </Page>
    );
}
