$(document).ready(function () {
    var table = $('#employeeTable').DataTable({
        scrollX: true,
        scrollCollapse: true,
        paging: false,
        columnDefs: [{
            sortable: false,
            orderable: false,
            "class": "index",
            targets: 0
        }],
        fixedColumns: true
    });

    table.on('order.dt search.dt', function () {
        table.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();

    $('#employeeTable > tbody  > tr').on("click", function () {
        let id = this.dataset.id;
        document.location.href = document.location.origin + `/Home/Edit?id=${id}`;
    });
});

function uploadCsvFile() {
    let file = document.getElementById('fileUpload').files[0];
    if (typeof file === 'undefined') {
        alert("Wasn't imported any file");
        return;
    }

    var formData = new FormData();
    formData.append("file", file);

    $.ajax({
        type: "POST",
        url: "/Home/LoadCSV",
        data: formData,
        processData: false,
        contentType: false,
        success: function (data) {
            alert(`${data} rows were successfully processed`);
            document.location.reload();
        },
        error: function (error) {
            alert(`Error: ${error}`)
        }
    });
}