import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';

export default function ORT030(props) {

    const { t } = useTranslation();

    const [orden, setOrden] = useState(false);

    const gridOnAfterButtonAction = (data, nexus) => {
        if (data.buttonId === "btnCerrarOrden") {
            if (data.parameters.find(x => x.id == "SHOW_CONFIRMACION_CERRAR_ORDEN")) {
                const cantidadTareasNoResueltas = data.parameters.find(x => x.id == "cantidadTareasNoResueltas").value;
                const cantidadTareasAmigablesPorCerrar = data.parameters.find(x => x.id == "cantidadTareasAmigablesPorCerrar").value;

                setOrden(data.parameters.find(x => x.id == "nuOrden").value)
                nexus.showConfirmation({
                    message: "General_Sec0_Info_CerrarOrdenConfirm",
                    argsMessage: [cantidadTareasNoResueltas, cantidadTareasAmigablesPorCerrar],
                    onAccept: () => {
                        nexus.getGrid("ORT030_grid_1").triggerMenuAction("btnCerrarOrdenConfirm");
                    },
                    onCancel: () => data.abortServerCall = true,
                });
            }
            else {
                nexus.getGrid("ORT030_grid_1").refresh();
            }

        }

        if (data.buttonId === "btnCerrarOrdenConfirm") {
            nexus.getGrid("ORT030_grid_1").refresh();

        }
    }

    const gridOnBeforMenuItemAction = (context, data, nexus) =>
    {
        data.parameters = [{id: "nuOrden", value: orden}]
    }

    return (
        <Page
            title={t("ORT030_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="ORT030_grid_1" rowsToFetch={30} rowsToDisplay={15} enableExcelExport enableExcelImport={false}
                        rowsToDisplay={15}
                        enableExcelExport
                        enableExcelImport={false}
                        onAfterButtonAction={gridOnAfterButtonAction}
                        onBeforeMenuItemAction={gridOnBeforMenuItemAction}
                    />
                </div>
            </div>
        </Page>
    );
}