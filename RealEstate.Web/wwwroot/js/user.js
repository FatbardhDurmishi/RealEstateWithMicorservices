var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $("#user-list").DataTable({
        buttons: [
            'copy', 'excel', 'pdf'
        ],
        dom: 'Bfrtip',
        pageLength: 15,
        responsive: true,
        autoWidth: false,
        order: [[0, "desc"]],
        //"processing": true,
        //"serverSide": true,
        //"filter": true,
        //"orderMulti": false,
        ajax: {
            url: '/User/GetUsersJson',
            dataSrc: ""
        },
        columns: [
            { data: "name", title: "Name" },
            { data: "phoneNumber", title: "PhoneNumber", },
            { data: "email", title: "Email" },
            { data: "role", title: "Role" },
            {
                data: "emailConfirmed", title: "Email Confirmed", sClass: "text-center", render: function (a, b, data, d) {
                    if (data.emailConfirmed == true) {
                        return '<i class="fa fa-check text-success"></i>';
                    }
                    else {
                        return '<i class="fa fa-check text-danger"></i>';
                    }
                }
            },
            {
                data: "id",
                render: function (data) {
                    return `
                          <div class=" btn-group align-items-center" role="group">
                            <a href="/User/CreateUser?id=${data}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i></a>
                            <a onClick=Delete('/User/Delete/${data}') class="btn btn-danger mx-2"><i class="bi bi-trash-fill"></i></a>
                        </div>
                            `
                }
            },

        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        toastr.success(data.message);
                    } else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}