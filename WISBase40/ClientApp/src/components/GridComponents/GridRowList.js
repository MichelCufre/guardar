import React from 'react';
import { Row } from './GridRow';
import { useTranslation } from 'react-i18next';

export function RowList(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    const getEmptyMessage = (props) => {
        if (props.showEmptyMessage) {
            return (
                <div className="gr-row-placeholder">
                    <span><i className="fas fa-exclamation-triangle" /> {t("General_Sec0_lbl_SinResultados")}</span>
                </div>
            );
        }

        return (
            <div className="gr-row-placeholder" />
        );
    }

    if (props.rows.length === 0) {
        return (
            <div className="gr-body empty" style={props.getEmptyBodyStyle()} >
                {getEmptyMessage(props)}
            </div>
        );
    }

    return props.rows.map(row => {
        return (
            <Row
                isNew={row.isNew}
                isDeleted={row.isDeleted}
                isModified={row.isModified}
                key={row.id}
                id={row.id}
                index={row.index}
                cells={row.cells}
                columns={props.columns}
                cssClass={row.cssClass}
                disabledButtons={row.disabledButtons}

                isControlActive={props.isControlActive}

                isBodyAdaptive={props.isBodyAdaptive}

                getRowHighlights={props.getRowHighlights}
                toggleHighlight={props.toggleHighlight}
                clearHighlighted={props.clearHighlighted}
                isHighlighted={props.isHighlighted}

                openRelatedLink={props.openRelatedLink}

                fixSelectGridHeight={props.fixSelectGridHeight}

                gridClientLeft={props.gridClientLeft}

                cellDisplayBorderLeft={props.cellDisplayBorderLeft}
                updateCellValue={props.updateCellValue}
                performButtonAction={props.performButtonAction}
                openDropdown={props.openDropdown}
                searchSelectValue={props.searchSelectValue}

                isEditingEnabled={props.isEditingEnabled}
                editingRow={props.editingRow}
                editingColumn={props.editingColumn}
                setEditingCell={props.setEditingCell}
                clearEditingCell={props.clearEditingCell}
                moveToPreviousEditableCell={props.moveToPreviousEditableCell}
                moveToNextEditableCell={props.moveToNextEditableCell}
                moveToFirstEditableCell={props.moveToFirstEditableCell}
                moveToLastEditableCell={props.moveToLastEditableCell}
                moveToNewRowEditableCell={props.moveToNewRowEditableCell}

                translator={props.translator}
            />
        );
    });
}