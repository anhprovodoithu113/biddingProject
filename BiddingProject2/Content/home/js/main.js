$(document).ready(() => {
    var descriptionCollection = document.querySelectorAll('div.description-card');
    descriptionCollection.forEach(item => {
        var arrayText = item.innerText.split('');
        var text = '';
        $count = 0;
        for (let index = 0; index < arrayText.length; index++) {
            if ($count === 20) {
                text += '...';
                break;
            }
            text += arrayText[index];
            if (arrayText[index] === ' ') {
                $count++;
            }
        }
        item.innerText = text;
    });

    var cusName = $('span.name');
    if (cusName.text() != "") {
        $.ajax({
            url: '/Home/GetInfo',
            type: 'GET',
            success: function (result) {
                if (result != null) {
                    if (result.data[0].img != null) {
                        var path = result.data[0].img.slice(1);
                        console.log(path);
                        $('.icon_wrap img').attr('src', path);
                    }

                    $('.icon_wrap .name').text(result.data[0].name);
                    var total = result.data[0].total;
                    if (total != 0) {
                        $('.total-items').removeClass('hidden');
                        $('.total-items span').text(total);
                    }
                }
            }, error: function (err) {
                console.log('Can not get data');
            }
        });
    }
    

    showNotify();

    /* Category */
    $msg = $('.myMsg').text();
    var lstOption = document.getElementById('category');
    if (lstOption != null) {
        var lstChild = lstOption.children;
        for (var i = 0; i < lstChild.length; i++) {
            if (lstChild[i].innerHTML === $msg) {
                lstChild[i].selected = true;
            }
        }
    }

    showTimeAll();

    /* Dropdown */
    $(".private-profile").click(function () {
        $('.profile').toggleClass("active");
        $(".notifications").removeClass("active");
    });

    //$('.header-container').not('.private-profile').click(function () {
    //    $(".profile").removeClass("active");
    //});

    $(".notify-section").click(function () {
        $(this).parent().toggleClass("active");
        $(".profile").removeClass("active");
    });

    $(".show_all .link").click(function () {
        $(".notifications").removeClass("active");
        $(".popup").show();
    });

    $(".close, .shadow").click(function () {
        $(".popup").hide();
    });

    $('.myGroupChat').animate({ scrollTop: $('.myGroupChat').height() }, 1000);
    var getName = document.getElementById('customerName');
    if (getName != null) {
        var name = getName.innerHTML;
        var price = $('#cusPrice').text();
        if (name != "") {
            $('.subImage').removeClass('hidden');
            $('#notificationMessage').text(name + ' has just bid with price is $' + price);
        }
    }
});

function deleteItem(idItem) {
    $.ajax({
        url: "/Customer/Customer/DeleteItem?idItem=" + idItem,
        type: "GET",
        contentType: "application/json;charset=utf-8",
        success: function (result) {
            location.reload();
        },
        error: function (errmessage) {
            toastr.warning(errmessage.msg);
        }
    });
}

function showNotify() {
    $msg = $('#myMessage').text();
    if ($msg != "") {
        if ($msg.indexOf('Please') >= 0) {
            toastr.warning($msg, 'Notification');
        } else if ($msg.indexOf('successful!') >= 0) {
            toastr.success($msg, 'Notification');
        }
    }
}


var customConfirm = new function () {
    var idItem = null;
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


/* Initial the connection to Bidding Hub */
var connection = $.hubConnection();
var hubProxy = connection.createHubProxy('BiddingHub');

/* This is the dynamic function in Hub  */
//hubProxy.on('addMessage', function (mess) {
//    $('.myGroupChat').append('<p>' + mess + '</p>');
//});

hubProxy.on('notificationMessage', function (name, price) {
    $('.currentTop').attr('style', 'display: flex');
    $('#originPrice')[0].textContent = price;
    $('.subImage').removeClass('hidden');
    $('#notificationMessage').text(name + ' has just bid with price is $' + price);
});

hubProxy.on('sendUserMessage', (user, message) => {
    $('.myGroupChat').append('<p>' + user + ' ' + message + '</p>');
});

hubProxy.on('updatePrice', (price, idProd) => {
    $('#price').text(price);
});

hubProxy.on('displayPopup', (message) => {
    var total = parseInt($('.total-items span').text()) + 1;
    $('.total-items span').text(total);
    $('#showIndividual').click();
});

/*--- End dynamic function ----*/

/* This function will show when connection to Bidding Hub is succeeded */
connection.start().done(function () {
    showTimeDetail();
    var product = $('#productName')[0];
    if (product != undefined) {
        hubProxy.invoke('JoinGroup', $('#productName')[0].textContent);
    } else {
        var lstProductName = $('.header-card h4');
        lstProductName.each(m => {
            hubProxy.invoke('LeaveGroup', lstProductName[m].innerHTML);
        });
    }

    // Tạo thêm leave group
});

/* When customer click on "Send" button in Chat Section */

$('[value="Send"]').on('click', () => {
    var idProduct = parseInt($('#myIdProduct')[0].innerText);
    var message = $('.message').val();
    hubProxy.invoke('SendMessage', $('#productName')[0].textContent, message);
    $('.message').val('');

    $.ajax({
        url: "/Home/AddChat?idProduct=" + idProduct + "&message=" + message,
        type: "GET",
        contentType: "application/json;charset=utf-8",
        success: function (result) {
            console.log(result);
        },
        error: function (errmessage) {
            console.log(errormessage);;
        }
    });
});

/* When customer click on "Bidding" button in Bidding Section */

$('#btnSendPrice').on('click', function () {
    var currentPrice = parseFloat($('#customerPrice').val());
    var productPrice = parseFloat($('#originPrice')[0].textContent);
    if (document.getElementById('timeLeft').innerText == "Sorry. This product is out of time.") {

    }
    else if (currentPrice <= productPrice) {
        toastr.warning('Your price is lower than currentPrice', 'Notification');
    } else {
        hubProxy.invoke('Notification', $('#productName')[0].textContent, currentPrice);
        var idProduct = parseInt($('#myIdProduct')[0].innerText);
        $.ajax({
            url: "/Home/AddPrice?idProduct=" + idProduct + "&price=" + currentPrice,
            type: "GET",
            contentType: "application/json;charset=utf-8",
            success: function (result) {
                toastr.success('You bid for "' + $('#productName')[0].textContent + '" with price is $' + currentPrice, 'Success');
            },
            error: function (errmessage) {
                toastr.error("Can't not bid", 'Error');
            }
        });
    }
});

/* Calculating the remaining time of that product */
function showTimeDetail() {
    var myTime = $('.myTime')[0];
    if (myTime != null) {
        myTime = myTime.textContent;
    }
    var date = new Date(myTime);
    date.setMinutes(date.getMinutes() + 30);
    var dH = date.getHours();
    var dM = date.getMinutes();
    var dS = date.getSeconds();

    var current = new Date();
    var cH = current.getHours();
    var cM = current.getMinutes();
    var cS = current.getSeconds();
    var time = "";

    if (date != undefined) {
        if (date < current) {
            $('#customerPrice').attr('readonly', 'readonly');
            var price = $('#originPrice').text();
            var idProd = $('#myIdProduct').text();
            if (idProd != "") {
                var idProduct = parseInt(idProd);
                var days = Math.floor((current - date) / 1000 / 60 / 60 / 24);
                console.log(days <= 30);
                if (time == "" && days <= 30 ) {
                    $.ajax({
                        url: "/Home/AddProductToCart?idProduct=" + idProduct + "&price=" + price,
                        type: "GET",
                        contentType: "application/json;charset=utf-8",
                        success: function (result) {
                            if (result.username != '') {
                                console.log('success');
                                hubProxy.invoke('SendIndividual', result.username, "Congratulation");
                            }
                        },
                        error: function (errmessage) {
                            console.log(errmessage);;
                        }
                    });
                }
                time = "Sorry. This product is out of time.";
            }
        } else {
            var m, s;
            var temptM = dM;
            if ((dS - cS) <= 0) {
                temptM = dM - 1;
                s = 59 - cS;
                if ((temptM - cM) <= 0) {
                    dH -= 1;
                    m = temptM + (60 - cM);
                    if (m == 60) {
                        m = 0;
                        temptM = '0' + m;
                    }
                } else if (temptM > cM) {
                    m = temptM - cM;
                }
            } else {
                s = dS - cS;
            }

            m = (m < 10 && m > 0) ? '0' + m : m;
            s = (s < 10) ? '0' + s : s;
            time = m + ":" + s;
        }

        $('#timeLeft').text(time);
        setTimeout(showTimeDetail, 1000);
    }
    
}

/* Category function */
function changeDropdown() {
    var idCategory = document.getElementById('category').value;
    location.href = "/Home/Index?idCategory=" + idCategory;
}

function showTimeAll() {
    var d0 = new Date(1970, 0, 1).getTime();
    var listTime = document.querySelectorAll('.myTime');
    var listSpecificProduct = document.querySelectorAll('.specificProduct');

    var currentTime = new Date();
    if (listSpecificProduct.length > 0) {
        for (var i = 0; i < listTime.length; i++) {
            var myTime = new Date(listTime[i].innerHTML);
            var bidTime = new Date(listTime[i].innerHTML);
            bidTime.setMinutes(bidTime.getMinutes() + 30);
            if (myTime.getTime() - currentTime.getTime() > 0) {
                var day, hour, min, sec;
                day = myTime.getDay() - currentTime.getDay();
                hour = myTime.getHours() - currentTime.getHours();
                min = myTime.getMinutes() - currentTime.getMinutes();
                sec = myTime.getSeconds() - currentTime.getSeconds();
                if (sec <= 0) {
                    sec = (59 + myTime.getSeconds()) - currentTime.getSeconds()
                    min--;
                }
                if (min < 0) {
                    min = (59 + myTime.getMinutes()) - currentTime.getMinutes();
                    hour--;
                }
                if (hour < 0) {
                    hour = (24 + myTime.getHours()) - currentTime.getHours();
                    if (hour == 24) {
                        hour = 0;
                    }
                    day--;
                }
                if (day <= 0) {
                    day = 0
                }

                var tempS, tempM, tempH, tempD;
                tempS = sec;
                tempM = min;
                tempH = hour;
                tempD = day;
                if (tempH < 10) {
                    tempH = '0' + tempH;
                }
                if (tempM < 10) {
                    tempM = '0' + tempM;
                }
                if (tempS < 10) {
                    tempS = '0' + tempS;
                }

                listSpecificProduct[i].innerHTML = tempD + 'd ' + tempH + ':' + tempM + ':' + tempS;
            }
            else if (bidTime - currentTime <= 0) {
                var classBackground = '.card_' + listSpecificProduct[i].id.substring(8);
                $(classBackground).addClass('hidden');
                var button = listSpecificProduct[i].parentNode.parentNode.parentNode.children[1].children[1].children[3];
                button.innerHTML = "Time out";
                button.setAttribute('style', 'background-color:red');
            }
            else {
                var classBackground = '.card_' + listSpecificProduct[i].id.substring(8);
                $(classBackground).addClass('hidden');
            }
        }
        setTimeout(showTimeAll, 1000);
    }
}