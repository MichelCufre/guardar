import React, { useEffect, useState } from 'react';
import { Grid } from '../../components/GridComponents/Grid';
import { useTranslation } from 'react-i18next';
import { Modal } from 'react-bootstrap';

export default function INT107Modal(props) {
    const { t } = useTranslation();
    const [nuInterfaz, setNuInterfaz] = useState(null);

    useEffect(() => {
        if (props.nuInterfaz !== null) {
            setNuInterfaz(props.nuInterfaz);
        }
    }, [props]);

    const onBeforeInitialize = (context, data, nexus) => {
        if (data.parameters.length > 0) {
            setNuInterfaz(data.parameters.find(e => e.id === "interfaz").value);
        }
    }

    return (
        <Modal show={props.show} onHide={props.onHide} dialogClassName="modal-90w">
            <Modal.Header closeButton>
                <Modal.Title>{t("INT107Modal_Sec0_modalTitle_Titulo") + " " + nuInterfaz}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <div className="row mb-4">
                    <div className="col-12">
                        <Grid id="INT107Modal_grid_1"
                            application="INT107Modal"
                            onBeforeInitialize={onBeforeInitialize}
                            rowsToFetch={30}
                            rowsToDisplay={15}
                            enableExcelExport
                        />
                    </div>
                </div>
            </Modal.Body>
        </Modal>
    )
}