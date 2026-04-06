import React, { useState } from 'react';
import { Modal, Button, Card, ListGroup } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';
import { GridShowHideColumnsItem } from './GridShowHideColumnsItem';

export function GridShowHideColumnsModal(props) {
    const { t } = useTranslation();

    const hiddenColumns = props.columns.filter(d => d.hidden).map(d => (
        <GridShowHideColumnsItem
            key={d.id}
            showHideEvent={props.showColumn}
            columnId={d.id}
            columnName={d.name}
            type={d.type}
            disabled={d.insertable}
        />
    ));

    const visibleColumns = props.columns.filter(d => !d.hidden).map(d => (
        <GridShowHideColumnsItem
            key={d.id}
            showHideEvent={props.hideColumn}
            columnId={d.id}
            columnName={d.name}
            type={d.type}
            disabled={d.insertable}
        />
    ));

    return (
        <Modal show={props.show} onHide={props.onClose}>
            <Modal.Header closeButton>
                <Modal.Title>{t("General_Sec0_btn_ToolTip_showHideColumns")}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <div className="row">
                    <div className="col-6">
                        <Card>
                            <Card.Header>
                                {t("General_Sec0_btn_ToolTip_showHideColumns_V")}
                            </Card.Header>
                            <ListGroup variant="flush">
                                {visibleColumns}
                            </ListGroup>
                        </Card>
                    </div>
                    <div className="col-6">
                        <Card>
                            <Card.Header>
                                {t("General_Sec0_btn_ToolTip_showHideColumns_O")}
                            </Card.Header>
                            <ListGroup variant="flush">
                                {hiddenColumns}
                            </ListGroup>
                        </Card>
                    </div>
                </div>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="secondary" onClick={props.onClose}>
                    {t("General_Sec0_btn_Cerrar")}
                </Button>
            </Modal.Footer>
        </Modal>
    );
}