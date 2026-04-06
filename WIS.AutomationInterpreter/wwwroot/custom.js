window.addEventListener("load", function () {
    var checkExist = setInterval(function () {
        if (document.getElementsByClassName('info__contact').length > 0) {
            clearInterval(checkExist);
            customizeSwaggerUI();
        }
    }, 100);
});
function customizeSwaggerUI() {
    var tag = '<rapi-pdf style="display:none" id="thedoc" include-example="true"> </rapi-pdf>';
    var btn = '<button id="btn" style="font-size:16px;padding: 6px 16px;text-align: center;white-space: nowrap;background-color: orangered;color: white;border: 0px solid #333;cursor: pointer;" type="button" onclick="downloadPDF()">Download API Document </button>';
    var oldhtml = document.getElementsByClassName('info')[0].innerHTML;
    document.getElementsByClassName('info')[0].innerHTML = oldhtml + '</br>' + tag + '</br>' + btn;
}
function downloadPDF() {
    var jsonUrl = document.getElementById("select").value;
    $.getJSON(jsonUrl, function (data) {
        if (data != "") {
            let docEl = document.getElementById("thedoc");
            docEl.generatePdf(data);
        }
    });
}
