import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { ORT020AsignarEmpresasModal } from './ORT020AsignarEmpresasModal';


export default function ORT020(props) {

    const { t } = useTranslation();

    const [showAsignarEmpresasModal, setShowAsignarEmpresasModal] = useState(false);
    const [codigo, setCodigo] = useState(null);
    const [descripcion, setDescripcion] = useState(null);

    const closeAsignarEmpresasDialog = () => {
        setShowAsignarEmpresasModal(false);
    }

    const handleBeforeButtonAction = (context, data, nexus) => {

        if (data.buttonId === "btnAsignarEmpresas") {

            context.abortServerCall = true;

            setCodigo(data.row.cells.find(w => w.column == "CD_INSUMO_MANIPULEO").value);
            setDescripcion(data.row.cells.find(d => d.column === "DS_INSUMO_MANIPULEO").value);

            setShowAsignarEmpresasModal(true);
        }
    }

    const handleAfterButtonAction = (data, nexus) => {
        if (data.buttonId === "btnAsignarEmpresas") {

            setCodigo(data.row.cells.find(w => w.column == "CD_INSUMO_MANIPULEO").value);
            setDescripcion(data.row.cells.find(d => d.column === "DS_INSUMO_MANIPULEO").value);

            setShowAsignarEmpresasModal(true);
        }
    }

    return (
        <Page
            title={t("ORT020_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <div className="row mb-4">
                <div className="col-12">
                    <Grid
                        id="ORT020_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        enableExcelExport
                        enableExcelImport={false}
                        onBeforeButtonAction={handleBeforeButtonAction}
                        onAfterButtonAction={handleAfterButtonAction}
                    />
                </div>
            </div>

            <ORT020AsignarEmpresasModal show={showAsignarEmpresasModal} onHide={closeAsignarEmpresasDialog} codigo={codigo} descripcion={descripcion} />
        </Page>
    );
}