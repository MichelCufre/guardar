import { buttonVariant } from "./Enums";

const mapButtonVariant = (variant) => {
    switch (variant) {
        case buttonVariant.primary: return "primary";
        case buttonVariant.secondary: return "secondary";
        case buttonVariant.success: return "success";
        case buttonVariant.warning: return "warning";
        case buttonVariant.danger: return "danger";
        case buttonVariant.info: return "info";
        case buttonVariant.light: return "light";
        case buttonVariant.dark: return "dark";
        case buttonVariant.link: return "link";
        case buttonVariant.outlineSecondary: return "outline-secondary";
    }

    return "";
};

const mapButtonVariantToEnum = (variant) => {
    switch (variant) {
        case "primary": return buttonVariant.primary;
        case "secondary": return buttonVariant.secondary;
        case "success": return buttonVariant.success;
        case "warning": return buttonVariant.warning;
        case "danger": return buttonVariant.danger;
        case "info": return buttonVariant.info;
        case "light": return buttonVariant.light;
        case "dark": return buttonVariant.dark;
        case "link": return buttonVariant.link;
        case "outline-secondary": return buttonVariant.outlineSecondary;
    }

    return buttonVariant.unknown;
};

export { mapButtonVariant, mapButtonVariantToEnum };