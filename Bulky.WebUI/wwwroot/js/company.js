var dataTable;

$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        ajax: { url: '/admin/company/getall' },
        buttons: ["copy", "excel", "pdf"],
        order: [[0, "asc"]],
        columns: [
            { data: 'name', width: '20%' },
            { data: 'streetAddress', width: '15%' },
            { data: 'city', width: '15%' },
            { data: 'state', width: '15%' },
            { data: 'phoneNumber', width: '15%', },
            {
                data: 'id',
                width: '30%',
                render: function (data) {
                    return `<div class="w-75 btn-group" role="group">
                        <a href="/admin/company/upsert?id=${data}" class="btn btn-primary mx-2">
                            <i class="bi bi-pencil-square"></i>&nbsp;Edit
                        </a>
                        <a onClick="Delete('/admin/company/delete?id=${data}')" class="btn btn-danger mx-2">
                            <i class="bi bi-trash3"></i>&nbsp;Delete
                        </a>
                    </div>`
                }
            },
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            })
        }
    });
}

//Swal.fire({
//    title: "Deleted!",
//    text: "Your file has been deleted.",
//    icon: "success"
//});