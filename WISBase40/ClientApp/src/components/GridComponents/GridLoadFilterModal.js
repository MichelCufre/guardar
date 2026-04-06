import React from 'react';
import { Modal, Button, Row, Col, Table } from 'react-bootstrap';
import { Loading } from '../Loading';
import { GridLoadFilterActions } from './GridLoadFilterActions';
import { useTranslation } from 'react-i18next';

export function GridLoadFilterModal(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    const handleClose = () => {
        props.closeLoadFilterModal();
    };

    const handleClear = () => {
        props.clearFilter();
    };

    let content;

    if (!props.filterList) {
        content = (
            <div style={{ textAlign: "center" }}>
                <span className="text-muted">
                    <Loading size="sm" />
                </span>
            </div>
        );
    }
    else if (props.filterList.length === 0) {
        content = (
            <div style={{ textAlign: "center" }}>
                <span className="text-muted">
                    {t("General_Sec0_lbl_LOAD_FILTER_MISMATCH")}
                </span>
            </div>
        );
    }
    else {
        const rows = props.filterList.map(d => (
            <tr key={d.id}>
                <td>{d.name}</td>
                <td>{d.description}</td>
                <td>{(new Date(d.date)).toLocaleDateString()}</td>
                <td className="text-center"><i className={d.isGlobal ? "fas fa-circle" : "fas fa-circle-notch"} /></td>
                <td className="text-center"><i className={d.isDefault ? "fas fa-circle" : "fas fa-circle-notch"} /></td>
                <td>
                    <GridLoadFilterActions
                        filterId={d.id}
                        closeLoadFilterModal={props.closeLoadFilterModal}
                        loadFilter={props.loadFilter}
                        removeFilter={props.removeFilter}
                    />
                </td>
            </tr>
        ));

        content = (
            <Table className="table" size="sm">
                <thead>
                    <tr>
                        <th>{t("General_Sec0_lbl_FILTER_NAME")}</th>
                        <th>{t("General_Sec0_lbl_FILTER_DESCRIPTION")}</th>
                        <th>{t("General_Sec0_lbl_LOAD_FILTER_DATE")}</th>
                        <th className="text-center">{t("General_Sec0_lbl_FILTER_GLOBAL")}</th>
                        <th className="text-center">{t("General_Sec0_lbl_FILTER_INITIAL")}</th>
                        <th />
                    </tr>
                </thead>
                <tbody>
                    {rows}
                </tbody>
            </Table>
        );
    }

    return (
        <Modal show={props.isLoadFilterModalOpen} size="lg" onHide={handleClose}>
            <Modal.Header closeButton>
                <Modal.Title>{t("General_Sec0_lbl_LOAD_FILTER_TITLE")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Row>
                    <Col style={{ overflowX: "auto" }}>
                        {content}
                    </Col>
                </Row>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="danger" onClick={handleClear}>
                    {t("General_Sec0_btn_LOAD_FILTER_CLEAR")}
                </Button>
                <Button variant="secondary" onClick={handleClose}>
                    {t("General_Sec0_btn_LOAD_FILTER_CLOSE")}
                </Button>
            </Modal.Footer>
        </Modal>
    );
}