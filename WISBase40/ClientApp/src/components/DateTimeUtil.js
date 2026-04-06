function AddLeadingZero(number) {
    return (number > 9 ? "" : "0") + number;
}

export const getDateString = (date) => {
    if (!date)
        return null;

    const day = date.getDate();
    const month = date.getMonth() + 1; //Arranca en 0, figures

    return [
        (day > 9 ? "" : "0") + day,
        (month > 9 ? "" : "0") + month,
        date.getFullYear()
    ].join("/");
};

export const getDateTimeString = (date) => {
    if (!date)
        return null;

    const day = date.getDate();
    const month = date.getMonth() + 1;
    const hours = date.getHours();
    const minutes = date.getMinutes();

    return [
        [
            AddLeadingZero(day),
            AddLeadingZero(month),
            date.getFullYear()
        ].join("/"),
        [
            AddLeadingZero(hours),
            AddLeadingZero(minutes)
        ].join(":")
    ].join(" ");
};