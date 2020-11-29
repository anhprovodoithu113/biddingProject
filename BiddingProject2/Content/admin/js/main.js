
$(document).ready(() => {

    // Khởi tạo Animation Library
    AOS.init();

    changeStyleSideBar();

    changeStylePageNumber();

    showNotify();

    renderDashboard();
});

function changeStyleSideBar() {
    $title = $('.body-page h1').text();
    $info = $('.info-link a')[0].textContent;
    if ($title === $info) {
        $('.info-link a')[0].classList.add('changeInfo');
    }
    var arrays = document.querySelectorAll('.link-list .link-items a');
    arrays.forEach(item => {
        if (item.innerHTML === $title) {
            item.parentElement.classList.add('link-active');
        }
    });
}

function showNotify() {
    $msg = $('#myMessage').text();
    if ($msg != "") {
        if ($msg.indexOf('Please') >= 0 || $msg.indexOf('not') >= 0 || $msg.indexOf('required') > 0 || $msg.indexOf('error') > 0) {
            toastr.warning($msg, 'Notification');
        } else if ($msg.indexOf('successful!') >= 0) {
            toastr.success($msg, 'Notification');
        }
    }
}

function changeStylePageNumber() {
    $page = $('#currentPage').text();
    var arrays = document.querySelectorAll('.pageNumber ul li a');
    arrays.forEach(item => {
        if (item.innerHTML == $page) {
            item.parentElement.classList.add('number-active');
        }
    })
}

/* Add Product */
function addProduct(count) {
    if (count > 0) {
        location.href = '/Admin/Admin/AddProduct';
    } else {
        toastr.warning('Please add Category first', 'Notification');
    }
}

/*  Message Dialog  */

/* Delete product */
function deleteProduct(idProduct) {
    $.ajax({
        url: '/Admin/Admin/DeleteProducts?idProduct=' + idProduct,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (result) {
            location.href = '/Admin/Admin/Products?msg=' + result.msg;
        }, error: function (errorMsg) {
            toastr.error(errorMsg.msg, 'Notification');
        }
    });
}

/* Delete Category */
function deleteCategory(idCategory) {
    $.ajax({
        url: '/Admin/Admin/DeleteCategory?categoryId=' + idCategory,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (result) {
            location.href = '/Admin/Admin/Category?msg=' + result.msg;
        }, error: function (errorMsg) {
            toastr.error(errorMsg.msg, 'Notification');
        }
    });
}

/* Reset password in Customers page */
function resetPassword(idCustomer) {
    $.ajax({
        url: '/Admin/Admin/ResetPassword?idCustomer=' + idCustomer,
        type: 'GET',
        contentType: "application/json;charset=utf-8",
        success: function (result) {
            toastr.success(result.message, 'Notification');
        },
        error: function (msgError) {
            toastr.error(result.message, 'Notification');
        }
    });
}

/* Delete customer */
function deleteCustomer(customerId) {
    $.ajax({
        url: '/Admin/Admin/DeleteCustomer?idCustomer=' + customerId,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        success: function (result) {
            location.href = '/Admin/Admin/Customers?msg=' + result.msg;
        }, error: function (errorMsg) {
            toastr.error(errorMsg.msg, 'Notification');
        }
    });
}

/* Initial a custom Dialog box */
var customConfirm = new function () {
    var idItem;
    this.show = function (msg, callback, id) {
        var dlg = document.getElementById('msg-container');
        $message = $('.message-body p');
        dlg.style.top = '30%';
        dlg.style.opacity = 1;
        $message.text(msg);
        this.callback = callback;
        idItem = id;
        document.getElementById('freezeLayer').style.display = '';
    };

    this.okay = function () {
        this.callback(idItem);
        this.close();
    }

    this.close = function () {
        var dlg = document.getElementById('msg-container');
        dlg.style.top = '-30%';
        dlg.style.opacity = 0;
        document.getElementById('freezeLayer').style.display = 'none';
    }
}

// Change Image
$('#inputImage').change(function () {
    if (this.files && this.files[0]) {
        var fileReader = new FileReader();
        fileReader.readAsDataURL(this.files[0]);
        fileReader.onload = function (e) {
            var myImage = $('img[alt="noImage"]');
            myImage.attr('src', e.target.result);
        }
    }
});


// Render Dashboard
function renderDashboard() {
    Chart.defaults.global.defaultFontColor = '#000000';
    Chart.defaults.global.defaultFontFamily = 'Arial';
    var lineChart = document.getElementById('myDashboard');
    $.ajax({
        url: '/Admin/Admin/GetSeparateData',
        type: 'GET',
        success: function (result) {
            console.log(result);
            $('#data-customer').text(result.users.thisMonth);
            if (result.users.lastMonth != null) {
                displayData($('#difference-customer'), result.users);
            }
            $('#data-storage').text(result.products.thisMonth);
            if (result.products.lastMonth != null) {
                displayData($('#difference-storage'), result.products);
            }
            $('#data-sold').text(result.soldProducts.thisMonth);
            if (result.soldProducts.lastMonth != null) {
                displayData($('#difference-sold'), result.soldProducts);
            }

            if (lineChart != null) {
                var myChart = new Chart(lineChart, {
                    type: 'line',
                    data: {
                        labels: ["Jan", "Feb", "Mar", "Apr", "May", "June", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
                        datasets: [
                            {
                                label: 'This year',
                                data: result.listRevenueThisYear,
                                backgroundColor: 'rgba(0, 128, 128, 0.3)',
                                borderColor: 'rgba(0, 128, 128, 0.7)',
                                borderWidth: 1
                            },
                            {
                                label: 'Last year',
                                data: result.listRevenueLastYear,
                                backgroundColor: 'rgba(0, 128, 128, 0.7)',
                                borderColor: 'rgba(0, 128, 128, 1)',
                                borderWidth: 1
                            }
                        ]
                    },
                    options: {
                        title: {
                            display: true,
                            text: 'Revenue of Bidding',
                            fontSize: 25
                        },
                        scales: {
                            yAxes: [{
                                ticks: {
                                    beginAtZero: true
                                }
                            }]
                        },
                    }
                });
            }

        }, error: function (errorMsg) {
            console.log('Can not get any data');
        }
    });
    
    
}

function displayData(object, data) {
    object.css('display', 'block');
    if (data.lastMonth > data.thisMonth) {
        object.text('-' + data.difference + '%');
        object.css('background-color', 'red');
    } else {
        object.text('+' + data.difference + '%');
        object.css('background-color', 'green');
    }
}