import React, { useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { Page } from '../../components/Page';
import { useTranslation } from 'react-i18next';
import { FileUploadComponentModal } from '../../components/FileUploadComponentModal';

export default function DOC095(props) {
    const { t } = useTranslation();
    const [codigoEntidad, setCodigoEntidad] = useState("");
    const [showPopup, setShowPopup] = useState(false);
    const [camposClave, setCamposClave] = useState(["DOC080_Sec0_lbl_TpDocumento", "DOC080_Sec0_lbl_NuDocumento"]);

    const closeFormDialog = () => {
        setShowPopup(false);
    }

    const onAfterButtonAction = (data, nexus) => {
        if (data.buttonId === "btnDocumentos") {
            setCodigoEntidad(data.parameters.find(f => f.id === "codigoEntidad").value);
            setShowPopup(true);
        };
    };

    return (

        <Page
            icon="fas fa-file"
            title={t("DOC095_Sec0_pageTitle_Titulo")}
            {...props}
        >
            <FileUploadComponentModal
                show={showPopup}
                onHide={closeFormDialog}
                permiteAlta={true}
                permiteBaja={true}
                tipoEntidad="DOCUMENTO"
                codigoEntidad={codigoEntidad}
                camposClave={camposClave}
            />

            <div className="row mb-4">
                <div className="col-12">
                    <Grid id="DOC095_grid_1"
                        rowsToFetch={30}
                        rowsToDisplay={15}
                        onAfterButtonAction={onAfterButtonAction}
                        enableExcelExport
                        enableExcelImport={false}
                    />
                </div>
            </div>
        </Page>
    );
}