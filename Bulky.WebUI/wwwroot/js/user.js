var dataTable;

$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        ajax: { url: '/admin/user/getall' },
        buttons: ["copy", "excel", "pdf"],
        order: [[4, "asc"]],
        columns: [
            { data: 'name', width: '20%' },
            { data: 'email', width: '15%' },
            { data: 'phoneNumber', width: '15%' },
            { data: 'company.name', width: '15%' },
            { data: 'role', width: '15%', },
            {
                data: { id: "id", lockoutEnd: "lockoutEnd" },
                width: '30%',
                render: function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();

                    var permissionButton =
                        `<a href="/admin/user/RoleManagement?userId=${data.id}" class="btn btn-success mx-2">
                            <i class="bi bi-pencil-square"></i>&nbsp;Permission
                        </a>`

                    var lockUnlockButton =
                        `<a onclick="DoLockUnlock('${data.id}','Lock')" class="btn btn-primary mx-2">
                            <i class="bi bi-unlock-fill"></i>&nbsp;Unlock
                        </a>`;

                    if (lockout > today) {
                        lockUnlockButton =
                            `<a onclick="DoLockUnlock('${data.id}','Unlock')" class="btn btn-danger mx-2">
                                <i class="bi bi-lock-fill"></i>&nbsp;Lock
                            </a>`;
                    }

                    return `<div class="w-75 btn-group" role="group">
                        ${lockUnlockButton}
                        ${permissionButton}
                    </div>`
                }
            },
        ]
    });
}

function DoLockUnlock(id, text) {
    Swal.fire({
        title: "Are you sure?",
        text: `You want to ${text} the user!`,
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: `Yes, ${text} it!`
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Admin/User/LockUnlock',
                data: JSON.stringify(id),
                contentType: "application/json",
                type: "POST",
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