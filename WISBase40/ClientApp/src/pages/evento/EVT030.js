import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { EVT030ModificarGrupoModal } from './EVT030ModificarGrupoModal';

export default function EVT030(props) {
    const { t } = useTranslation();

    const [nuGrupo, setNuGrupo] = useState(null);
    const [nmGrupo, setNmGrupo] = useState("");
    const [showModalEditarGrupo, setShowModalEditarGrupo] = useState(false);

    const closeModalEditarGrupo = (nexus) => {
        setShowModalEditarGrupo(false);
    }

    const handleBeforeButtonAction  = (context, data, nexus) => {
        context.abortServerCall = true;

        if(data.buttonId === "btnEditar") {
            setNuGrupo(data.row.cells.find(d => d.column === "NU_CONTACTO_GRUPO").value);
            setNmGrupo(data.row.cells.find(d => d.column === "NM_GRUPO").value);
            setShowModalEditarGrupo(true);
        }
    };

    return (

        <Page
            title={t("EVT030_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div className="row mt-5 mb-4">
                <div className="col-12">
                    <Grid
                        id="EVT030_grid_Grupos"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        onBeforeButtonAction={handleBeforeButtonAction}
                    />
                </div>
            </div>
            <EVT030ModificarGrupoModal
                show={showModalEditarGrupo}
                onHide={closeModalEditarGrupo}
                nuGrupo={nuGrupo}
                nmGrupo={nmGrupo}
            />
        </Page>
    );
}
