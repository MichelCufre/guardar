import $ from 'jquery';

export const useLoading = () => {

    const showLoadingOverlay = (id = "layout") => {
        let existe = $(".loadingoverlay").length > 0;
        if (!existe) {
            $("#" + id).LoadingOverlay("show", {
                image: "",
                fontawesome: "fa fa-cog fa-spin",
                background: "rgba(22, 25, 28, 0.2)"
            });
        }
    }


    const hideLoadingOverlay = (id = "layout") => {
        $("#" + id).LoadingOverlay("hide");
    }

    return {
        showLoadingOverlay,
        hideLoadingOverlay,
    };
};