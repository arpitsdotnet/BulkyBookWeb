var dataTable;
$(document).ready(function () {
    var url = window.location.search;

    if (url.includes("inprocess"))
        loadDataTable("inprocess");
    else if (url.includes("pending"))
        loadDataTable("pending");
    else if (url.includes("completed"))
        loadDataTable("completed");
    else if (url.includes("approved"))
        loadDataTable("approved");
    else
        loadDataTable("all");
})

function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        ajax: { url: `/admin/order/getall?status=${status}` },
        buttons: ["copy", "excel", "pdf"],
        order: [[1, "asc"]],
        columns: [
            { data: 'id', width: '5%' },
            { data: 'name', width: '10%' },
            { data: 'phoneNumber', width: '10%' },
            { data: 'applicationUser.email', width: '15%' },
            { data: 'orderStatus', width: '15%', class: 'text-center' },
            { data: 'orderTotal', width: '15%', class: 'text-center' },
            {
                data: 'id',
                width: '10%',
                render: function (data) {
                    return `<div class="w-75 btn-group" role="group">
                        <a href="/admin/order/details?orderId=${data}" class="btn btn-primary mx-2">
                            <i class="bi bi-pencil-square"></i>
                        </a>
                    </div>`
                }
            },
        ]
    });
}


//Swal.fire({
//    title: "Deleted!",
//    text: "Your file has been deleted.",
//    icon: "success"
//});