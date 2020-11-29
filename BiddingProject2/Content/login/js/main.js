$(document).ready(function () {
    // Check validation in back-end
    $msg = $('#toastMsg')[0].textContent;
    if ($msg != "") {
        if ($msg.indexOf('successful') >= 0) {
            toastr.success($msg, 'Notification');
        } else if ($msg.indexOf('exist') >= 0 || $msg.indexOf('matched') >= 0 || $msg.indexOf('Please login')>= 0) {
            toastr.warning($msg, 'Notification');
        } else if ($msg.indexOf('Invalid') >= 0) {
            toastr.error($msg, 'Notification');
        }
        resetField();
    }
});

function resetField() {
    $('#displayName-input').val('');
    $('#email-input').val('');
    $('#re-pass-input').val('');
    $('#pass-input').val('');
    $('#user-input').val('');
    $('#toastMsg')[0].textContent = '';
}