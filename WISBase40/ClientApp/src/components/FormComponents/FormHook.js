import { useState, useEffect, useLayoutEffect } from 'react';
import $ from 'jquery';

export const useForm = (initialValue = []) => {

    const [position, setPosition] = useState(0);
    const [focus, setFocus] = useState(initialValue[position]);
    const [loading, setLoading] = useState(false);
    const [focusForm, setFocusForm] = useState(false);
    useLayoutEffect(() => {
        if (focusForm) {
            setTimeout(() => {
                setFocusAndReadOnlyFields();
                setFocusForm(false);
            }, 300);
        }

    }, [focus, loading]);

    const siguiente = (form, focusField = "", value = 1) => {

        let inputAnterior = initialValue[position];
        form.fields.find(f => f.id === inputAnterior).readOnly = true;

        if (focusField !== "") {
            setPosition(initialValue.indexOf(focusField));
            setFocus(focusField);
            form.fields.find(f => f.id === focusField).readOnly = false;
        } else {
            if (position + value >= initialValue.length) return;
            let siguiente = position + value;
            setPosition(siguiente);
            let input = initialValue[siguiente];
            setFocus(input);
            form.fields.find(f => f.id === input).readOnly = false;
        }
        setFocusForm(true);
    };

    const anterior = (form = null, focusField = "", value = 1, borrarCampo = true) => {
        let inputAnterior = initialValue[position];

        form.fields.find(f => f.id === inputAnterior).readOnly = true;
        form.fields.find(f => f.id === inputAnterior).value = "";
        document.getElementsByName(inputAnterior)[0].value = "";

        if (focusField !== "") {
            setPosition(initialValue.indexOf(focusField));
            setFocus(focusField);
            form.fields.find(f => f.id === focusField).readOnly = false;
            if (borrarCampo) {
                form.fields.find(f => f.id === focusField).value = "";
            }

        } else {
            if (position - value < 0) return;

            let anterior = position - value;
            setPosition(anterior);
            let input = initialValue[anterior];
            setFocus(input);
            form.fields.find(f => f.id === input).readOnly = false;
            if (borrarCampo) {
                form.fields.find(f => f.id === input).value = "";
            }
        }
        setFocusForm(true);
    };

    const reset = (form, focusField = "", fieldsToClean = []) => {

        if (fieldsToClean && fieldsToClean.length > 0) {
            fieldsToClean.forEach(element => {
                form.setFieldValue(element, "");
            });
        } else {
            form.reset();
        }

        if (focusField !== "") {
            setPosition(initialValue.indexOf(focusField));
            setFocus(focusField);
        }
    };

    const resetFocus = (focusField) => {
        setPosition(initialValue.indexOf(focusField));
        setFocus(focusField);
        setTimeout(() => {
            setFocusAndReadOnlyFields();
        }, 300);
    }

    const setFocusAndReadOnlyFields = () => {
        initialValue.forEach(ref => {
            setReadOnly(ref, ref !== focus);
        });
    };

    const setReadOnly = (ref, isReadOnly) => {
        if (ref) {
            document.getElementsByName(ref).forEach(field => {
                field.readOnly = isReadOnly;
                if (!isReadOnly) {
                    field.focus();
                }
            });
        }
    };

    const showLoadingOverlay = (id = "layout") => {
        setLoading(true);
        let existe = $(".loadingoverlay").length > 0;
        if (!existe) { 
        $("." + id).LoadingOverlay("show", {
            image: "",
            fontawesome: "fa fa-cog fa-spin",
            background: "rgba(22, 25, 28, 0.2)"
        });
        }
        
    }

    const hideLoadingOverlay = (id = "layout") => {
        setLoading(false);
        $("." + id).LoadingOverlay("hide");
    }

    const isLoading = () => {
        return loading;
    }
    const clearErrors = (forms) => {
        forms.fields.forEach((element) => {
            element.error = undefined;
            element.status = "Ok"
        });

    }

    return {
        focus,
        siguiente,
        anterior,
        reset,
        resetFocus,
        showLoadingOverlay,
        hideLoadingOverlay,
        isLoading,
        clearErrors
    };
};