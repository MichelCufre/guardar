import { connect, getIn } from 'formik';
import React, { useEffect, useLayoutEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { withFormContext } from './WithFormContext';

function InternalFieldFile(props) {
    const { t } = useTranslation();

    const fieldData = {
        props: {
            name: props.name,
            hidden: props.hidden,
            readOnly: props.readOnly,
            disabled: props.disabled
        }
    };

    useLayoutEffect(() => props.formProps.registerField(fieldData), []);
    useEffect(() => () => props.formProps.unregisterField(props.name), []);

    const handleValidate = (event) => {

    }

    const handleChange = (event) => {
        processFiles(event.target.files[0]);
    };

    const processFiles = (file) => {

        if (!file) {

            updateValue("", "", "");
            return;
        }

        const fileExtension = file.name.substr(file.name.lastIndexOf('.') + 1);

        //if (!(["png", "jpeg", "xlsx", "xls", "doc", "docx", "txt", "pdf"].some(extension))) {
        //props.toaster.toastError(t('Tipo de archivo no válido'));
        //return false;
        //}

        var fileSize = ((file.size / 1024) / 1024).toFixed(4); // MB
        var fileName = file.name.replace(("." + fileExtension), "");

        if (fileSize <= 10) {


            const reader = new FileReader();

            reader.onload = function (e) {

                updateValue(fileName, fileExtension, reader.result.replace(/^data:.+;base64,/, ''));

            };

            reader.readAsDataURL(file);

        }
        else {
            updateValue(fileName, fileExtension, "");
        }

    }

    const updateValue = (fileName, extension, payload) => {

        props.formProps.updateOptions(props.name, [
            { value: payload, label: "PAYLOAD" },
            { value: fileName, label: "FILENAME" },
            { value: extension, label: "FILEEXTENSION" },
        ]);

        document.getElementById("file-upload").value = "";

    };

    const getProps = () => {
        const { formProps, formik, ...result } = props;

        return result;
    }
    const getStatusClass = (error, touch) => {
        const errorEmpty = isErrorEmptyObject(error);

        if (props.readOnly || props.disabled)
            return "";

        return (touch && error && !errorEmpty ? "is-invalid" : (errorEmpty ? '' : (touch ? "is-valid" : "")));
    }

    const isErrorEmptyObject = (error) => {
        //Si error es vacío, por lo general significa que la validación con Yup tiró excepción
        return error && (typeof error === "object") && !Object.keys(error).length;
    }

    const error = getIn(props.formik.errors, props.name);
    const touch = getIn(props.formik.touched, props.name);
    const fieldProps = props.formProps.getFieldProps(props.name);

    let { className } = getIn(getProps());

    className = (className || "") + " " + getStatusClass(error, touch);

    return (

        <label for="file-upload" class="custom-file-upload" autocomplete="off">
            <img src="/img/upload.png" /><i></i> {'     '} {t("General_Sec0_btn_SeleccionarArchivo")}
            <input
                id="file-upload"
                className={className}
                validate={handleValidate}
                onChange={handleChange}
                type="file"
                accept="image/*,.xlsx,.xls,.doc, .docx,.txt,.pdf,video/mp4,video/x-m4v,video/*"
            />
        </label>

    );
}

export const FieldFile = withFormContext(connect(InternalFieldFile));