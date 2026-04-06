import React, { useEffect, useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { useTranslation } from 'react-i18next';
import { Modal } from 'react-bootstrap';

export default function INT070Modal(props) {
    const { t } = useTranslation();
    const [nuInterfaz, setNuInterfaz] = useState(null);

    useEffect(() => {
        if (props.nuInterfaz !== null) {
            setNuInterfaz(props.nuInterfaz);
        }
    }, [props]);

    const applyParameters = (context, data, nexus) => {
        if (props.nuInterfaz) {
            setNuInterfaz(props.nuInterfaz);
            data.parameters = [{ id: "interfaz", value: props.nuInterfaz }];
        }
    }

    return (
        <Modal show={props.show} onHide={props.onHide} dialogClassName="modal-90w">
            <Modal.Header closeButton>
                <Modal.Title>{t("INT070_Sec0_modalTitle_Titulo") + " " + nuInterfaz}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <div className="row mb-4">
                    <div className="col-12">
                        <Grid id="INT070_grid_1"
                            application="INT070"
                            rowsToFetch={30}
                            rowsToDisplay={10}
                            enableExcelExport
                            onBeforeFetch={applyParameters}
                            onBeforeApplySort={applyParameters}
                            onBeforeInitialize={applyParameters}
                            onBeforeFetchStats={applyParameters}
                            onBeforeApplyFilter={applyParameters}
                            onBeforeExportExcel={applyParameters}
                        />
                    </div>
                </div>
            </Modal.Body>
        </Modal>

    );
}