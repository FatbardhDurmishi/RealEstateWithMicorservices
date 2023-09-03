var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $("#properties-list").DataTable({
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
            url: '/Company/Properties/GetProperties',
            dataSrc: ""
        },
        columns: [
            { data: "id", title: "Id", "width": "5%" },
            { data: "name", title: "Name" },
            { data: "user.name", title: "User", },
            { data: "propertyType.name", title: "Property Type", "width": "15%" },
            { data: "transactionType", title: "Transaction Type", "width": "15%" },
            { data: "status", title: "Status" },
            { data: "price", title: "Price" },
            {
                data: { id: "id", showButtons: "showButtons", title: "Actions" },

                render: function (data) {
                    if (data.showButtons == true) {
                        return `
                          <div class="w-75 btn-group align-items-center" role="group">
                             <a href="/Properties/AddProperty?id=${data.id}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i></a>
                             <a onClick=Delete('/Properties/DeleteProperty/${data.id}') class="btn btn-danger mx-2"><i class="bi bi-trash-fill"></i></a>
                             <a href="/Company/Properties/Details?propertyId=${data.id}" class="btn btn-secondary mx-2"><i class="fa-solid fa-circle-info"></i></a>
                          </div>
                            `
                    } else {
                        return `
                          <div class="w-75 btn-group align-items-center" role="group">
                            <a href="/Properties/Details?propertyId=${data.id}" class="btn btn-secondary mx-2"><i class="fa-solid fa-circle-info"></i></a>
                        </div>
                            `;
                    }
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