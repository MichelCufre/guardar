import React, { useState, useEffect } from 'react';
import { Modal, Button, Row, Col, Container, Card } from 'react-bootstrap';
import { Loading } from '../Loading';
import withScrollContext from './WithScrollContext';

const InternalGridStatsPanel = React.memo((props) => {
    const [totalRecords, setTotalRecords] = useState(null);

    useEffect(() => {
        if (props.isStatsPanelOpen) {
            props.fetchStats().then((stats) => {
                if (stats) {
                    setTotalRecords(stats.count);
                }
                else {
                    setTotalRecords("No disponible");
                }
            });
        }
    }, [props.isStatsPanelOpen]);

    const handleClose = () => {
        props.closeStatsPanel();
    };

    const totalRecordsValue = totalRecords || totalRecords === 0 ? totalRecords : <Loading size="sm" />;

    let totalRecordsDownloaded = props.rowCount;

    /*if (!isNaN(totalRecords)) {
        totalRecordsDownloaded = totalRecords === 0 ? `${props.rowCount} (100%)` : `${props.rowCount} (${Math.round(props.rowCount * 100 / totalRecords)}%)`;
    }*/

    const style = {
        height: `calc(100% - ${props.scrollContext.scrollbarHeight}px)`
    };

    const cardStyle = {
        border: "none",
        borderRadius: 0
    };

    return (
        <div className="gr-stats-panel" style={style}>
            <Card style={cardStyle}>
                <Card.Header>Información</Card.Header>
                <Card.Body>
                    <Row>
                        <Col>Total de filas: </Col>
                        <Col>{totalRecordsValue}</Col>
                    </Row>
                    <Row>
                        <Col>Filas cargadas: </Col>
                        <Col>{totalRecordsDownloaded}</Col>
                    </Row>
                    <Row>
                        <Col>Filas visibles: </Col>
                        <Col>{props.visibleRows}</Col>
                    </Row>
                </Card.Body>
            </Card>
        </div>
    );
}, (prevProps, nextProps) => { prevProps.rowCount === nextProps.rowCount && prevProps.visibleRows === nextProps.visibleRows })

export const GridStatsPanel = withScrollContext(InternalGridStatsPanel);