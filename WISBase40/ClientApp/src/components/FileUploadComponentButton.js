import React from 'react';
import { useTranslation } from 'react-i18next';
import { Loading } from './Loading';

export function FileUploadComponentButton(props) {
    const { t } = useTranslation("translation", { useSuspense: false });

    if (!props.isUploading) {
        const buttonClassName = props.isDraggingOver ? "file-import-btn is-dragging" : "file-import-btn";
        const buttonLabel = props.isDraggingOver ? t('IFile_Modal_lbl_DropOver') : t('IFile_Modal_lbl_DropZone');

        return (
            <React.Fragment>
                <input
                    id="FileImportField"
                    className="file-import-input"
                    type="file"
                    onChange={props.onChange}
                />
                <label
                    className={buttonClassName}
                    onDragOver={props.onDragOver}
                    onDragLeave={props.onDragLeave}
                    onDragEnter={props.onDragEnter}
                    onDragEnd={props.onDragEnd}
                    onDrop={props.onDrop}
                    htmlFor="FileImportField"
                >
                    <div className="file-import-btn-content">
                        <i className="fas fa-upload"></i>
                        <span>{buttonLabel}</span>
                    </div>
                </label>
            </React.Fragment>
        )
    }
    else {
        const uploadMessage = t("IFile_Modal_lbl_Uploading");

        return (
            <div className="file-loading-container">
                <span>{uploadMessage}</span>
                <Loading />
            </div>
        )
    }
}